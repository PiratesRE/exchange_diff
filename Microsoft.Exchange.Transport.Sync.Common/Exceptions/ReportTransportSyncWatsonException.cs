using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Exceptions
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ReportTransportSyncWatsonException : Exception
	{
		public ReportTransportSyncWatsonException(string message, Exception innerException, string stackTrace) : base(message, innerException)
		{
			this.stackTrace = stackTrace;
		}

		public ReportTransportSyncWatsonException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override string StackTrace
		{
			get
			{
				return this.stackTrace;
			}
		}

		private readonly string stackTrace;
	}
}
