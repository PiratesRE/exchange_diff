using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClientAccessArrayNotFoundException : LocalizedException
	{
		public ClientAccessArrayNotFoundException(string site, ServerIdParameter serverId) : base(Strings.messageClientAccessArrayNotFoundException(site, serverId))
		{
			this.site = site;
			this.serverId = serverId;
		}

		public ClientAccessArrayNotFoundException(string site, ServerIdParameter serverId, Exception innerException) : base(Strings.messageClientAccessArrayNotFoundException(site, serverId), innerException)
		{
			this.site = site;
			this.serverId = serverId;
		}

		protected ClientAccessArrayNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.site = (string)info.GetValue("site", typeof(string));
			this.serverId = (ServerIdParameter)info.GetValue("serverId", typeof(ServerIdParameter));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("site", this.site);
			info.AddValue("serverId", this.serverId);
		}

		public string Site
		{
			get
			{
				return this.site;
			}
		}

		public ServerIdParameter ServerId
		{
			get
			{
				return this.serverId;
			}
		}

		private readonly string site;

		private readonly ServerIdParameter serverId;
	}
}
