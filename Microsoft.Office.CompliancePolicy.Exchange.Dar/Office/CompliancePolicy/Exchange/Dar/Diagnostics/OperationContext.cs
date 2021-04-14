using System;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Diagnostics
{
	public class OperationContext : IDisposable
	{
		public OperationContext(string correlationId) : this()
		{
			this.correlationId = correlationId;
			OperationContext.current = this;
		}

		public OperationContext() : this(Guid.NewGuid().ToString())
		{
		}

		public static string CorrelationId
		{
			get
			{
				if (OperationContext.current == null)
				{
					return null;
				}
				return OperationContext.current.correlationId;
			}
		}

		public void Dispose()
		{
			OperationContext.current = null;
		}

		[ThreadStatic]
		private static OperationContext current;

		private readonly string correlationId;
	}
}
