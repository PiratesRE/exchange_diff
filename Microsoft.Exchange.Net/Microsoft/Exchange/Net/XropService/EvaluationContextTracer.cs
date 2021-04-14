using System;
using System.IdentityModel.Policy;
using Microsoft.Exchange.Net.Claim;

namespace Microsoft.Exchange.Net.XropService
{
	internal sealed class EvaluationContextTracer
	{
		public EvaluationContextTracer(EvaluationContext evaluationContext)
		{
			this.evaluationContext = evaluationContext;
		}

		public override string ToString()
		{
			return this.evaluationContext.GetTraceString();
		}

		private EvaluationContext evaluationContext;
	}
}
