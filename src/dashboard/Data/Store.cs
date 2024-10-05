namespace dashboard.Data;

public class Store {
    public Guid Id { get; set; }
    public string Name { get; set; }

    public Country Country { get; set; }
    public IEnumerable<Person> People { get; set; }
}