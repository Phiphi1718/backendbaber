using System;
using System.Collections.Generic;

namespace backend1.Data;

public partial class ProductCombo
{
    public int ComboId { get; set; }

    public string ComboName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AppointmentProductCombo> AppointmentProductCombos { get; set; } = new List<AppointmentProductCombo>();

    public virtual ICollection<ComboProduct> ComboProducts { get; set; } = new List<ComboProduct>();
}
