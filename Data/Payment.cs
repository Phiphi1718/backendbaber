using System;
using System.Collections.Generic;

namespace backend1.Data;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int AppointmentId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? PaymentStatus { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;
}
