using System.Collections.Concurrent;
using Wasmtime;

namespace Opa.Wasm.DependencyInjection;

/// <summary>
/// This factory specifically loads and manages WASMs for OPA, which are better off left in memory.
/// It should be a singleton.
/// </summary>
internal class OpaPolicyFactory : IDisposable
{
	private readonly ConcurrentDictionary<Type, Runtime> _loadedRuntimes = new ConcurrentDictionary<Type, Runtime>();

	/// <summary>
	/// Add a runtime to the factory.
	/// </summary>
	/// <param name="name">Name of the module.</param>
	/// <param name="wasmBytes">The bytes to load.</param>
	/// <typeparam name="TClient"></typeparam>
	public void AddRuntime<TClient>(string name, byte[] wasmBytes)
	{
		var runtime = new OpaRuntime();
		var module = runtime.Load(name, wasmBytes);
		if(!_loadedRuntimes.TryAdd(typeof(TClient), new Runtime(runtime, module)))
		{
			throw new InvalidOperationException("Policy runtime could not be loaded");
		}
	}

	/// <summary>
	/// Adds a runtime to the factory.
	/// </summary>
	/// <param name="wasmFilePath">Path to the wasm file.</param>
	/// <typeparam name="TClient"></typeparam>
	public void AddRuntime<TClient>(string wasmFilePath)
	{
		var runtime = new OpaRuntime();
		var module = runtime.Load(wasmFilePath);
		if(!_loadedRuntimes.TryAdd(typeof(TClient), new Runtime(runtime, module)))
		{
			throw new InvalidOperationException("Policy runtime could not be loaded");
		}
	}

	/// <summary>
	/// Instantiate the policy.
	/// </summary>
	/// <returns>A new instance of the policy that can be evaluated.</returns>
	public IPolicy<TClient> CreatePolicy<TClient>()
	{
		if (_loadedRuntimes.TryGetValue(typeof(TClient), out var runtime))
		{
			return new TypedOpaPolicy<TClient>(runtime.OpaRuntime, runtime.WasmModule);
		}

		throw new InvalidOperationException("Policy runtime has not been loaded.");
	}

	/// <inheritdoc />
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Dispose the thing.
	/// </summary>
	/// <param name="disposing">Are we disposing.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			foreach (var runtime in _loadedRuntimes.Values)
			{
				runtime.OpaRuntime.Dispose();
				runtime.WasmModule.Dispose();
			}
		}
	}

	private sealed record Runtime(OpaRuntime OpaRuntime, Module WasmModule);
}
