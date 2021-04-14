using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SIPFEServerConfigurationNotFoundException : LocalizedException
	{
		public SIPFEServerConfigurationNotFoundException(string serverName) : base(Strings.SIPFEServerConfigurationNotFound(serverName))
		{
			this.serverName = serverName;
		}

		public SIPFEServerConfigurationNotFoundException(string serverName, Exception innerException) : base(Strings.SIPFEServerConfigurationNotFound(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected SIPFEServerConfigurationNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly string serverName;
	}
}
