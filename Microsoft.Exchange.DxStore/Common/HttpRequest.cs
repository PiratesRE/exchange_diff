using System;
using System.Runtime.Serialization;
using FUSE.Paxos;

namespace Microsoft.Exchange.DxStore.Common
{
	[KnownType(typeof(HttpRequest.PaxosMessage))]
	[DataContract]
	[KnownType(typeof(HttpRequest.GetStatusRequest))]
	[Serializable]
	public abstract class HttpRequest
	{
		public HttpRequest(string sender)
		{
			this.Sender = sender;
		}

		[DataMember]
		public string Sender { get; set; }

		[DataContract]
		[Serializable]
		public class GetStatusRequest : HttpRequest
		{
			public GetStatusRequest(string sender) : base(sender)
			{
			}

			[Serializable]
			public class Reply
			{
				public InstanceStatusInfo Info { get; set; }
			}
		}

		[Serializable]
		public sealed class PaxosMessage : HttpRequest
		{
			public PaxosMessage(string sender, Message msg) : base(sender)
			{
				this.Message = msg;
			}

			public Message Message { get; set; }
		}

		[Serializable]
		public sealed class DxStoreRequest : HttpRequest
		{
			public DxStoreRequest(string self, DxStoreRequestBase r) : base(self)
			{
				this.Request = r;
			}

			public DxStoreRequestBase Request { get; set; }
		}
	}
}
