using System;

namespace Microsoft.Exchange.DxStore.Common
{
	[Serializable]
	public abstract class HttpReply
	{
		[Serializable]
		public sealed class ExceptionReply : HttpReply
		{
			public ExceptionReply(Exception e)
			{
				this.Exception = e;
			}

			public Exception Exception { get; private set; }
		}

		[Serializable]
		public sealed class DxStoreReply : HttpReply
		{
			public DxStoreReply(DxStoreReplyBase r)
			{
				this.Reply = r;
			}

			public DxStoreReplyBase Reply { get; set; }
		}

		[Serializable]
		public sealed class GetInstanceStatusReply : HttpReply
		{
			public GetInstanceStatusReply(InstanceStatusInfo r)
			{
				this.Reply = r;
			}

			public InstanceStatusInfo Reply { get; set; }
		}
	}
}
