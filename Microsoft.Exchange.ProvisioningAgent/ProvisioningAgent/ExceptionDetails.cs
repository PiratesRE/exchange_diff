using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.ProvisioningAgent
{
	public class ExceptionDetails
	{
		public DateTime ExceptionTimeUtc { get; set; }

		public string ExceptionType { get; set; }

		public string Details { get; set; }

		public string DiagnosticContext { get; set; }

		internal static ExceptionDetails FromException(Exception exception)
		{
			if (exception != null)
			{
				return new ExceptionDetails
				{
					ExceptionTimeUtc = DateTime.UtcNow,
					ExceptionType = exception.GetType().ToString(),
					Details = exception.ToString(),
					DiagnosticContext = (Microsoft.Exchange.Diagnostics.DiagnosticContext.HasData ? AuditingOpticsLogger.GetDiagnosticContextFromThread() : null)
				};
			}
			return null;
		}
	}
}
