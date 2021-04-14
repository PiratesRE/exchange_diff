using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UMServiceDisabled : LocalizedException
	{
		public UMServiceDisabled(string serviceName, string serverName) : base(Strings.UMServiceDisabledException(serviceName, serverName))
		{
			this.serviceName = serviceName;
			this.serverName = serverName;
		}

		public UMServiceDisabled(string serviceName, string serverName, Exception innerException) : base(Strings.UMServiceDisabledException(serviceName, serverName), innerException)
		{
			this.serviceName = serviceName;
			this.serverName = serverName;
		}

		protected UMServiceDisabled(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serviceName = (string)info.GetValue("serviceName", typeof(string));
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serviceName", this.serviceName);
			info.AddValue("serverName", this.serverName);
		}

		public string ServiceName
		{
			get
			{
				return this.serviceName;
			}
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly string serviceName;

		private readonly string serverName;
	}
}
