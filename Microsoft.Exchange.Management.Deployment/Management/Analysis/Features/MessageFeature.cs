using System;

namespace Microsoft.Exchange.Management.Analysis.Features
{
	internal class MessageFeature : Feature
	{
		public MessageFeature(Func<Result, string> textFunction) : base(true, false)
		{
			this.TextFunction = textFunction;
		}

		public Func<Result, string> TextFunction { get; private set; }

		public string Text(Result result)
		{
			return this.TextFunction(result);
		}
	}
}
