using System;

namespace Opa.Wasm
{
    public interface IPolicy : IDisposable
    {
		string Evaluate(string json, bool disableFastEvaluate = false);

		string Evaluate(string json, int entrypoint, bool disableFastEvaluate = false);

		string Evaluate(string json, string entrypoint, bool disableFastEvaluate = false);

		void SetData(string json);
    }
}
