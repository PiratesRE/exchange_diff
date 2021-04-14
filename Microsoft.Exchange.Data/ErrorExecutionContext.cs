using System;

namespace Microsoft.Exchange.Data
{
	internal class ErrorExecutionContext : IErrorExecutionContext
	{
		public ErrorExecutionContext(string executionContext)
		{
			this.executionContext = executionContext;
		}

		public string ExecutionHost
		{
			get
			{
				return this.executionContext;
			}
			set
			{
				this.executionContext = value;
			}
		}

		private string executionContext;
	}
}
