using Apache.Ignite.Core.Cache.Configuration;

namespace Ignite.DynamicLINQ.Data;

public record Car(
    [property:QuerySqlField] string Make,
    [property:QuerySqlField] string Model,
    [property:QuerySqlField] int Year,
    [property:QuerySqlField] string Color,
    [property:QuerySqlField] string BodyType,
    [property:QuerySqlField] string EngineType,
    [property:QuerySqlField] int EngineCc,
    [property:QuerySqlField] int EngineHp,
    [property:QuerySqlField] decimal Price);
