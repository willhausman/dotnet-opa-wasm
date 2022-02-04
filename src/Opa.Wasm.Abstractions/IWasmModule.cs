namespace Opa.Wasm.Abstractions;

public interface IWasmModule : IDisposable
{
    IWasmInstance CreateInstance();
}
