using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UMRoleNotInstalledException : LocalizedException
	{
		public UMRoleNotInstalledException(string thumbprint, string serverName) : base(Strings.UMRoleNotInstalled(thumbprint, serverName))
		{
			this.thumbprint = thumbprint;
			this.serverName = serverName;
		}

		public UMRoleNotInstalledException(string thumbprint, string serverName, Exception innerException) : base(Strings.UMRoleNotInstalled(thumbprint, serverName), innerException)
		{
			this.thumbprint = thumbprint;
			this.serverName = serverName;
		}

		protected UMRoleNotInstalledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.thumbprint = (string)info.GetValue("thumbprint", typeof(string));
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("thumbprint", this.thumbprint);
			info.AddValue("serverName", this.serverName);
		}

		public string Thumbprint
		{
			get
			{
				return this.thumbprint;
			}
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly string thumbprint;

		private readonly string serverName;
	}
}
