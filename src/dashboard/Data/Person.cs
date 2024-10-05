namespace dashboard.Data;

public class Person {
    public Guid Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public Store Store { get; set; }
}