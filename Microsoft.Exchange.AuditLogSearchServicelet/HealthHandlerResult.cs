using System;

namespace Microsoft.Exchange.Servicelets.AuditLogSearch
{
	public class HealthHandlerResult
	{
		public HealthHandlerResult() : this(string.Empty)
		{
		}

		public HealthHandlerResult(string message)
		{
			this.Message = message;
		}

		public string Message { get; set; }
	}
}
