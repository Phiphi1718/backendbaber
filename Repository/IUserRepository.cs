using backend1.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace backend1.Repositories
{
    public interface IUserRepository
    {
        Task<User?> LoginAsync(string email, string password);
        Task<User> RegisterAsync(User newUser);
        Task<string> ForgotPasswordAsync(string email);
        Task<bool> UpdatePasswordAsync(string email, string oldPassword, string newPassword);

    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return null;
            }

            // Xác minh mật khẩu
            if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public async Task<User> RegisterAsync(User newUser)
        {
            // Kiểm tra xem email đã tồn tại chưa
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == newUser.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            // Hash mật khẩu trước khi lưu
            newUser.PasswordHash = PasswordHelper.HashPassword(newUser.PasswordHash);
            newUser.TypeId = 2; // Default TypeId for user
            newUser.CreatedAt = DateTime.UtcNow;

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        public async Task<string> ForgotPasswordAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return null; // Nếu không tìm thấy người dùng
            }

            // Tạo mật khẩu mới
            var newPassword = GenerateRandomPassword();

            // Mã hóa mật khẩu mới
            user.PasswordHash = PasswordHelper.HashPassword(newPassword);

            // Lưu người dùng với mật khẩu mới vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return newPassword;
        }

        public async Task<bool> UpdatePasswordAsync(string email, string oldPassword, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return false; // Nếu không tìm thấy người dùng
            }

            // Kiểm tra mật khẩu cũ
            if (!PasswordHelper.VerifyPassword(oldPassword, user.PasswordHash))
            {
                return false; // Mật khẩu cũ không đúng
            }

            // Kiểm tra mật khẩu mới và xác nhận mật khẩu mới
            if (newPassword != oldPassword)
            {
                user.PasswordHash = PasswordHelper.HashPassword(newPassword); // Mã hóa mật khẩu mới
                await _context.SaveChangesAsync();
                return true; // Thành công
            }

            return false; // Mật khẩu mới không hợp lệ
        }


        private string GenerateRandomPassword()
        {
            // Tạo mật khẩu ngẫu nhiên
            var random = new Random();
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var password = new char[8]; // Độ dài mật khẩu là 8 ký tự
            for (int i = 0; i < password.Length; i++)
            {
                password[i] = validChars[random.Next(validChars.Length)];
            }
            return new string(password);
        }
    }

    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }
    }
}
