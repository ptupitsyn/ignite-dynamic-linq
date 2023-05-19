using Apache.Ignite.Core.Cache.Configuration;

namespace Ignite.DynamicLINQ.Data;

public record Car(
    [property:QuerySqlField] string Make,
    [property:QuerySqlField] string Model,
    [property:QuerySqlField] int Year);
