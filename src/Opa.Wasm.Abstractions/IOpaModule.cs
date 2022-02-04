namespace Opa.Wasm.Abstractions;

public interface IOpaModule : IDisposable
{
    IOpaPolicy CreatePolicy();
}
