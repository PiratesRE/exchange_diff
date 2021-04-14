using System;

namespace Microsoft.Exchange.Servicelets.AuditLogSearch
{
	public class ExceptionDetails
	{
		public DateTime ExceptionTimeUtc { get; set; }

		public string ExceptionType { get; set; }

		public string Details { get; set; }

		internal static ExceptionDetails Create(Exception e)
		{
			ExceptionDetails result = null;
			if (e != null)
			{
				result = new ExceptionDetails
				{
					ExceptionTimeUtc = DateTime.UtcNow,
					ExceptionType = e.GetType().ToString(),
					Details = e.ToString()
				};
			}
			return result;
		}
	}
}
