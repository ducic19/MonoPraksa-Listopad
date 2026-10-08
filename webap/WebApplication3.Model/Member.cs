using System;
using System.Collections.Generic;

namespace WebApplication3.Model;

public partial class Member
{
    public Guid MemId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public Guid SubsId { get; set; }

    public virtual Subscription Subs { get; set; } = null!;
}
