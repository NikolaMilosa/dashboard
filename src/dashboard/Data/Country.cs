namespace dashboard.Data;

public class Country {
    public Guid Id { get; set; }
    public string Name { get; set; }

    public IEnumerable<Store> Stores { get; set; }
}