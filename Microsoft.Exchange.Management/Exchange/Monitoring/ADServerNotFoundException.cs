using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ADServerNotFoundException : LocalizedException
	{
		public ADServerNotFoundException(string serverId) : base(Strings.messageADServerNotFoundException(serverId))
		{
			this.serverId = serverId;
		}

		public ADServerNotFoundException(string serverId, Exception innerException) : base(Strings.messageADServerNotFoundException(serverId), innerException)
		{
			this.serverId = serverId;
		}

		protected ADServerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverId = (string)info.GetValue("serverId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverId", this.serverId);
		}

		public string ServerId
		{
			get
			{
				return this.serverId;
			}
		}

		private readonly string serverId;
	}
}
