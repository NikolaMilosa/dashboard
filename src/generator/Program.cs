
using Cocona;
using dashboard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

CoconaApp.Run(Main);

async Task Main(
    string? countryName,
    string? storeName,
    string? personName,
    string? personLastName,
    string? connectionString,
    string beginningDate,
    string endingDate
)
{
    var loggerFactory = LoggerFactory.Create(options =>
    {
        options.AddSimpleConsole(opts => opts.TimestampFormat = "O");
        options.SetMinimumLevel(LogLevel.Information);
    });

    var logger = loggerFactory.CreateLogger("generator");

    var country = countryName ?? "Srbija";
    var store = storeName ?? "Generated store";
    var person = personName ?? "Generated";
    var lastName = personLastName ?? "Generated";

    logger.LogInformation("Starting generator with following parameters: {0}, {1}, {2}, {3}", country, store, person, lastName);

    var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(connectionString);
    var dbContext = new ApplicationDbContext(options.Options);
    await dbContext.Database.MigrateAsync();
    logger.LogInformation("Connected to database");

    var countryEntity = await GetOrCreateCountry(dbContext, country, logger);
    var storeEntity = await GetOrCreateStore(dbContext, countryEntity, store, logger);
    var personEntity = await GetOrCreatePerson(dbContext, storeEntity, person, lastName, logger);

    var beginning = DateTime.Parse(beginningDate);
    var ending = DateTime.Parse(endingDate);
    logger.LogInformation("Will generated data from {0} to {1}", beginning.ToString("yyyy-MM-dd"), ending.ToString("yyyy-MM-dd"));
    await FillWithRandomLogs(dbContext, logger, personEntity, beginning.Date, ending.Date);
    logger.LogInformation("Done");
}

async Task<Country> GetOrCreateCountry(ApplicationDbContext dbContext, string countryName, ILogger logger)
{
    var maybeCountry = await dbContext.Countries.Include(x => x.Stores).ThenInclude(x => x.People).FirstOrDefaultAsync(x => x.Name == countryName);
    if (maybeCountry != null)
    {
        logger.LogInformation("Country {0} already exists", countryName);
        return maybeCountry;
    }
    var newCountry = new Country()
    {
        Name = countryName,
        Stores = new List<Store>()
    };
    dbContext.Countries.Add(newCountry);
    await dbContext.SaveChangesAsync();
    logger.LogInformation("Country {0} created", countryName);
    return newCountry;
}

async Task<Store> GetOrCreateStore(ApplicationDbContext dbContext, Country country, string storeName, ILogger logger)
{
    var maybeStore = country.Stores.FirstOrDefault(x => x.Name == storeName);
    if (maybeStore != null)
    {
        logger.LogInformation("Store {0} already exists", storeName);
        return maybeStore;
    }
    var newStore = new Store()
    {
        Name = storeName,
        Country = country,
        People = new List<Person>()
    };
    dbContext.Stores.Add(newStore);
    await dbContext.SaveChangesAsync();
    logger.LogInformation("Store {0} created", storeName);
    return newStore;
}

async Task<Person> GetOrCreatePerson(ApplicationDbContext dbContext, Store store, string personName, string personLastName, ILogger logger)
{
    var maybePerson = store.People.FirstOrDefault(x => x.FirstName == personName && x.LastName == personLastName);
    if (maybePerson != null)
    {
        logger.LogInformation("Person {0} {1} already exists", personName, personLastName);
        return maybePerson;
    }
    var newPerson = new Person()
    {
        FirstName = personName,
        LastName = personLastName,
        Store = store,
    };
    dbContext.People.Add(newPerson);
    await dbContext.SaveChangesAsync();
    logger.LogInformation("Person {0} {1} created", personName, personLastName);
    return newPerson;
}

async Task FillWithRandomLogs(ApplicationDbContext dbContext, ILogger logger, Person person, DateTime beginning, DateTime ending)
{
    var random = new Random();
    while (beginning < ending)
    {
        if (beginning.DayOfWeek == DayOfWeek.Sunday)
        {
            // 20% of time person will not come to work
            beginning = beginning.AddDays(1);
            continue;
        }

        LogType logType;
        switch (random.NextDouble() * 100)
        {
            case double n when n < 10:
                logType = LogType.UnpaidLeave;
                break;
            case double n when n < 20:
                logType = LogType.SickLeave;
                break;
            case double n when n < 90:
                logType = LogType.CheckIn;
                break;
            default:
                logType = LogType.Vacation;
                break;
        }
        logger.LogInformation("For {0} will generate {1}", beginning.ToString("yyyy-MM-dd"), logType);

        switch (logType)
        {
            case LogType.CheckIn:
                var checkedInHours = random.NextDouble() * (10 - 7) + 7;
                var checkedIn = beginning.AddHours(checkedInHours);
                var checkInLog = new Log
                {
                    Description = "Generated check in",
                    LogType = LogType.CheckIn,
                    Person = person,
                    Timestamp = checkedIn.ToUniversalTime()
                };

                var checkedOutHours = random.NextDouble() * (18 - 15) + 15;
                var checkedOut = beginning.AddHours(checkedOutHours);
                var checkOutLog = new Log
                {
                    Description = "Generated check out",
                    LogType = LogType.CheckOut,
                    Person = person,
                    Timestamp = checkedOut.ToUniversalTime()
                };
                await dbContext.EventLogs.AddAsync(checkInLog);
                await dbContext.EventLogs.AddAsync(checkOutLog);
                break;
            case LogType.SickLeave:
                var sickLeaveLog = new Log
                {
                    Description = "Generated sick leave",
                    LogType = LogType.SickLeave,
                    Person = person,
                    Timestamp = beginning.AddHours(8).ToUniversalTime()
                };

                await dbContext.EventLogs.AddAsync(sickLeaveLog);
                break;
            case LogType.UnpaidLeave:
                var unpaidLeaveLog = new Log
                {
                    Description = "Generated unpaid leave",
                    LogType = LogType.UnpaidLeave,
                    Person = person,
                    Timestamp = beginning.AddHours(8).ToUniversalTime()
                };

                await dbContext.EventLogs.AddAsync(unpaidLeaveLog);
                break;
            case LogType.Vacation:
                var vacationLog = new Log
                {
                    Description = "Generated vacation",
                    LogType = LogType.Vacation,
                    Person = person,
                    Timestamp = beginning.AddHours(8).ToUniversalTime()
                };

                await dbContext.EventLogs.AddAsync(vacationLog);
                break;
        }

        await dbContext.SaveChangesAsync();
        beginning = beginning.AddDays(1);
    }
}
