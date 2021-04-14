using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	internal sealed class EvaluationModeFeature : Feature
	{
		public EvaluationModeFeature(Evaluate evaluationMode)
		{
			this.evaluationMode = evaluationMode;
		}

		public Evaluate EvaluationMode
		{
			get
			{
				return this.evaluationMode;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", base.ToString(), this.evaluationMode);
		}

		private readonly Evaluate evaluationMode;
	}
}
