namespace DTO
{
    public class MemberAddDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public Guid SubsId { get; set; } // Samo ID, bez navigacijskog svojstva Subs
    }
    public class MemberEditDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public Guid SubsId { get; set; }
    }
}