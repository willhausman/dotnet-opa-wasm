namespace Opa.Wasm.DependencyInjection;

/// <summary>
/// Injectable Policy for a specific client.
/// </summary>
/// <typeparam name="TClient">The intended client.</typeparam>
public interface IPolicy<TClient> : IPolicy
{
}
