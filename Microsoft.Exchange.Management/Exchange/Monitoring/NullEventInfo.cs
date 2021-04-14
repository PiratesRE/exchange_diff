using System;

namespace Microsoft.Exchange.Monitoring
{
	internal sealed class NullEventInfo : ReplicationEventBaseInfo
	{
		public NullEventInfo() : base(ReplicationEventType.Null, false, null)
		{
		}
	}
}
