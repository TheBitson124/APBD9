namespace Zadanie9.DTOs;

public class Client
{
    public int IdClient { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string Pesel { get; set; }
    public virtual ICollection<ClientTrip> ClientTrips { get; set; } = new List<ClientTrip>();

}