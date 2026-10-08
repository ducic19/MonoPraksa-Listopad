using System;
using System.Collections.Generic;

namespace WebApplication3.Model;

public partial class Trainer
{
    public Guid TrainerId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Subscription> Subs { get; set; } = new List<Subscription>();
}
