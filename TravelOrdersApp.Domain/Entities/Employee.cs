namespace TravelOrdersApp.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string PersonalNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string PersonalIdentificationNumber { get; set; } = string.Empty;

    public string FullName { 
        get  { 
            return $"{FirstName} {LastName}"; 
        } 
    }
}