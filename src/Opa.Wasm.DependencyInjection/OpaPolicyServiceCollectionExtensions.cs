using Microsoft.Extensions.DependencyInjection;

namespace Opa.Wasm.DependencyInjection;

/// <summary>
/// Extension methods to configure an <see cref="IServiceCollection"/>.
/// </summary>
public static class OpaPolicyServiceCollectionExtensions
{
	private static readonly OpaPolicyFactory _factory = new OpaPolicyFactory();

	public static IServiceCollection AddOpaPolicy<TClient>(this IServiceCollection services, string opaWasmPath)
	{
		// TODO: inject this factory instead of static and add configurations
		_factory.AddRuntime<TClient>(opaWasmPath);
		services.AddTransient<IPolicy<TClient>>(_ => _factory.CreatePolicy<TClient>());
		return services;
	}
}
