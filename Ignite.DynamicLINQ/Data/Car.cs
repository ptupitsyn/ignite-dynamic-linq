namespace Ignite.DynamicLINQ.Data;

public record Car(
    string Make,
    string Model,
    int Year,
    string Color,
    string BodyType,
    string EngineType,
    int EngineCc,
    int EngineHp,
    decimal Price);
