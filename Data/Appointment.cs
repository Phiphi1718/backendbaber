using System;
using System.Collections.Generic;

namespace backend1.Data;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public DateTime AppointmentDate { get; set; }

    public int ServiceId { get; set; }

    public string? Notes { get; set; }

    public decimal TotalPrice { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AppointmentProductCombo> AppointmentProductCombos { get; set; } = new List<AppointmentProductCombo>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Service Service { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
