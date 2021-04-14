using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class GetTaskPiiRedactionModule : PiiRedactionModuleBase
	{
		public GetTaskPiiRedactionModule(TaskContext context) : base(context)
		{
		}

		public override void Dispose()
		{
			if (this.piiSuppressionContext != null)
			{
				this.piiSuppressionContext.Dispose();
				this.piiSuppressionContext = null;
			}
			base.Dispose();
		}

		public override bool WriteObject(object input, out object output)
		{
			if (this.piiSuppressionContext != null)
			{
				this.piiSuppressionContext.Dispose();
			}
			this.piiSuppressionContext = base.CreatePiiSuppressionContext(input as IConfigurable);
			output = input;
			return true;
		}

		private IDisposable piiSuppressionContext;
	}
}
