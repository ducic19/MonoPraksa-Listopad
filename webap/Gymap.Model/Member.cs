using gymap.Model;
public class Member 
{
    public Guid MemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // Sigurnosna polja
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // Zadana uloga je User, ne bira je klijent!

    public Guid? SubsId { get; set; }
    public Subscription? Subs { get; set; }
}