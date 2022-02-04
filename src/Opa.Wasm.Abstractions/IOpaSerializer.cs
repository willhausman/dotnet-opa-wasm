namespace Opa.Wasm.Abstractions;

public interface IOpaSerializer
{
    string Serialize<T>(T? obj);

    T? Deserialize<T>(string json);
}
