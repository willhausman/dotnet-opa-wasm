using System.IO;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Opa.Wasm.DependencyInjection;

namespace Opa.Wasm.UnitTests
{
    public class DependencyInjectionTests
    {
        [Test]
		public void InstantiateFromDI()
		{
			var services = new ServiceCollection();
			services.AddOpaPolicy<DependencyInjectionTests>(WasmFiles.HelloWorldExample);
			using var provider = services.BuildServiceProvider();
			var policy = provider.GetRequiredService<IPolicy<DependencyInjectionTests>>();
			string data = new
			{
				world = "world"
			}.ToJson();
			policy.SetData(data);

			string input = new
			{
				message = "world"
			}.ToJson();
			string outputJson = policy.Evaluate(input, disableFastEvaluate: true);

			dynamic output = outputJson.ToDynamic();
			Assert.IsTrue(output[0].result);
		}

		[Test]
		public void ProvideSeparateImplementationsInOneFactory()
		{
			var services = new ServiceCollection();
			services.AddOpaPolicy<Implementation1>(WasmFiles.HelloWorldExample);
			services.AddOpaPolicy<Implementation2>(WasmFiles.RbacExample);
			services.AddScoped<Implementation1>();
			services.AddScoped<Implementation2>();
			using var provider = services.BuildServiceProvider();
			var impl1 = provider.GetRequiredService<Implementation1>();
			var impl2 = provider.GetRequiredService<Implementation2>();

			Assert.IsTrue(impl1.Result);
			Assert.IsTrue(impl2.Allow);
			Assert.IsTrue(impl2.UserIsAdmin);
		}

		private class Implementation1
		{
			private readonly IPolicy<Implementation1> policy;

			public Implementation1(IPolicy<Implementation1> policy)
			{
				this.policy = policy;
				var data = new
				{
					world = "world"
				}.ToJson();
				policy.SetData(data);
			}

			public bool Result
			{
				get
				{
					var input = new
					{
						message = "world"
					}.ToJson();
					var outputJson = policy.Evaluate(input);

					var output = outputJson.ToDynamic();
					return output[0].result;
				}
			}
		}

		private class Implementation2
		{
			private readonly IPolicy<Implementation2> policy;

			public Implementation2(IPolicy<Implementation2> policy)
			{
				this.policy = policy;
				string data = File.ReadAllText(Path.Combine("TestData", "basic_rbac_data.json"));
				this.policy.SetData(data);
				string input = File.ReadAllText(Path.Combine("TestData", "basic_rbac_input.json"));
				string outputJson = this.policy.Evaluate(input);
				var output = outputJson.ToDynamic();
				Allow = output[0].result.allow;
				UserIsAdmin = output[0].result.user_is_admin;
			}

			public bool Allow { get; }

			public bool UserIsAdmin { get; }
		}
    }
}
