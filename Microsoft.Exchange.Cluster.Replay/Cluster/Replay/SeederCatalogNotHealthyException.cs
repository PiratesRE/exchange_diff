using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeederCatalogNotHealthyException : SeederServerException
	{
		public SeederCatalogNotHealthyException(string reason) : base(ReplayStrings.SeederCatalogNotHealthyErr(reason))
		{
			this.reason = reason;
		}

		public SeederCatalogNotHealthyException(string reason, Exception innerException) : base(ReplayStrings.SeederCatalogNotHealthyErr(reason), innerException)
		{
			this.reason = reason;
		}

		protected SeederCatalogNotHealthyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("reason", this.reason);
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string reason;
	}
}
