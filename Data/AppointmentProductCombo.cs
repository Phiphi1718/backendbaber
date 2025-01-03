using System;
using System.Collections.Generic;

namespace backend1.Data;

public partial class AppointmentProductCombo
{
    public int AppointmentProductComboId { get; set; }

    public int AppointmentId { get; set; }

    public int ComboId { get; set; }

    public int? Quantity { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual ProductCombo Combo { get; set; } = null!;
}
