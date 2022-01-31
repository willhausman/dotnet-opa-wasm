using Wasmtime;

namespace Opa.Wasm.DependencyInjection;

public class TypedOpaPolicy<TClient> : OpaPolicy, IOpaPolicy<TClient>
{
	public TypedOpaPolicy(string fileName)
		: base(fileName)
	{
	}

	public TypedOpaPolicy(OpaRuntime runtime, Module wasmModule)
		: base(runtime, wasmModule)
	{
	}

	public TypedOpaPolicy(string name, byte[] content)
		: base(name, content)
	{
	}
}
