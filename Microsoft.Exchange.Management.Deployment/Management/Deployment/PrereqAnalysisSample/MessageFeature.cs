using System;
using Microsoft.Exchange.Management.Deployment.Analysis;

namespace Microsoft.Exchange.Management.Deployment.PrereqAnalysisSample
{
	internal sealed class MessageFeature : Feature
	{
		public MessageFeature(Func<Result, string> textFunction)
		{
			this.textFunction = textFunction;
		}

		public Func<Result, string> TextFunction
		{
			get
			{
				return this.textFunction;
			}
		}

		public string Text(Result result)
		{
			return this.TextFunction(result);
		}

		private readonly Func<Result, string> textFunction;
	}
}
