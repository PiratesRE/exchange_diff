using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class SubmissionQuotaExceededException : StoragePermanentException
	{
		public SubmissionQuotaExceededException(LocalizedString message) : base(message)
		{
		}

		protected SubmissionQuotaExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
