using System;
using System.Collections.Generic;

namespace backend1.Data;

public partial class Rating
{
    public int RatingId { get; set; }

    public int UserId { get; set; }

    public int ServiceId { get; set; }

    public int? Rating1 { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Service Service { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
