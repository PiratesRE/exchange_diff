using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmReferralException : AmServerException
	{
		public AmReferralException(string referredServer) : base(ServerStrings.AmReferralException(referredServer))
		{
			this.referredServer = referredServer;
		}

		public AmReferralException(string referredServer, Exception innerException) : base(ServerStrings.AmReferralException(referredServer), innerException)
		{
			this.referredServer = referredServer;
		}

		protected AmReferralException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.referredServer = (string)info.GetValue("referredServer", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("referredServer", this.referredServer);
		}

		public string ReferredServer
		{
			get
			{
				return this.referredServer;
			}
		}

		private readonly string referredServer;
	}
}
