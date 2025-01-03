using System;
using System.Collections.Generic;

namespace backend1.Data;

public partial class ComboProduct
{
    public int ComboProductId { get; set; }

    public int ComboId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ProductDescription { get; set; }

    public decimal ProductPrice { get; set; }

    public virtual ProductCombo Combo { get; set; } = null!;
}
