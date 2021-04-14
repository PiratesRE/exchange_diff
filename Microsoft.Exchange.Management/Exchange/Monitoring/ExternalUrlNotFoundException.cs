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
	public class ExternalUrlNotFoundException : LocalizedException
	{
		public ExternalUrlNotFoundException(ServerIdParameter serverId, Type type) : base(Strings.messageExternalUrlNotFoundException(serverId, type))
		{
			this.serverId = serverId;
			this.type = type;
		}

		public ExternalUrlNotFoundException(ServerIdParameter serverId, Type type, Exception innerException) : base(Strings.messageExternalUrlNotFoundException(serverId, type), innerException)
		{
			this.serverId = serverId;
			this.type = type;
		}

		protected ExternalUrlNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverId = (ServerIdParameter)info.GetValue("serverId", typeof(ServerIdParameter));
			this.type = (Type)info.GetValue("type", typeof(Type));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverId", this.serverId);
			info.AddValue("type", this.type);
		}

		public ServerIdParameter ServerId
		{
			get
			{
				return this.serverId;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		private readonly ServerIdParameter serverId;

		private readonly Type type;
	}
}
