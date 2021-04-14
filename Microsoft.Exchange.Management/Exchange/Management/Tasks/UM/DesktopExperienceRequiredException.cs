using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DesktopExperienceRequiredException : LocalizedException
	{
		public DesktopExperienceRequiredException(string serverFqdn) : base(Strings.DesktopExperienceRequiredException(serverFqdn))
		{
			this.serverFqdn = serverFqdn;
		}

		public DesktopExperienceRequiredException(string serverFqdn, Exception innerException) : base(Strings.DesktopExperienceRequiredException(serverFqdn), innerException)
		{
			this.serverFqdn = serverFqdn;
		}

		protected DesktopExperienceRequiredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverFqdn = (string)info.GetValue("serverFqdn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverFqdn", this.serverFqdn);
		}

		public string ServerFqdn
		{
			get
			{
				return this.serverFqdn;
			}
		}

		private readonly string serverFqdn;
	}
}
