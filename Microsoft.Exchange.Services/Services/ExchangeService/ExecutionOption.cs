using System;

namespace Microsoft.Exchange.Services.ExchangeService
{
	internal class ExecutionOption
	{
		public static ExecutionOption Default
		{
			get
			{
				return ExecutionOption.defaultOption;
			}
		}

		public bool WrapExecutionExceptions { get; set; }

		public ResponseValidationBehavior ResponseValidationBehavior { get; set; }

		private static readonly ExecutionOption defaultOption = new ExecutionOption
		{
			WrapExecutionExceptions = true,
			ResponseValidationBehavior = ResponseValidationBehavior.ThrowOnAnyResponseError
		};
	}
}
