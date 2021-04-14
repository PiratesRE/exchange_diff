using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MoreThanOneRecipientWithNetId : ADExternalException
	{
		public MoreThanOneRecipientWithNetId(string netId) : base(DirectoryStrings.MoreThanOneRecipientWithNetId(netId))
		{
			this.netId = netId;
		}

		public MoreThanOneRecipientWithNetId(string netId, Exception innerException) : base(DirectoryStrings.MoreThanOneRecipientWithNetId(netId), innerException)
		{
			this.netId = netId;
		}

		protected MoreThanOneRecipientWithNetId(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.netId = (string)info.GetValue("netId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("netId", this.netId);
		}

		public string NetId
		{
			get
			{
				return this.netId;
			}
		}

		private readonly string netId;
	}
}
