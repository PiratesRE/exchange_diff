using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class ReplicationCheckSkippedException : ReplicationCheckException
	{
		public ReplicationCheckSkippedException() : this(LocalizedString.Empty)
		{
		}

		public ReplicationCheckSkippedException(LocalizedString localizedString) : base(localizedString)
		{
		}

		public ReplicationCheckSkippedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
