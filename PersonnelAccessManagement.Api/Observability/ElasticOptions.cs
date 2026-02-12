namespace PersonnelAccessManagement.Api.Observability;

public sealed class ElasticOptions
{
    public required string[] Uris { get; init; }
    public DataStreamOptions DataStream { get; init; } = new();

    public sealed class DataStreamOptions
    {
        public string Type { get; init; } = "logs";
        public string Dataset { get; init; } = "pam";
        public string Namespace { get; init; } = "api";
    }
}