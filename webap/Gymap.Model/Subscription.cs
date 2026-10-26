using System;
using System.Collections.Generic;

namespace gymap.Model;

public partial class Subscription
{
    public Guid SubsId { get; set; }

    public int Weekly { get; set; }

    public int Duration { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    public virtual ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
}
