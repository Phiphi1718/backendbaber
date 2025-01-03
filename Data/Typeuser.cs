using System;
using System.Collections.Generic;

namespace backend1.Data;

public partial class Typeuser
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
