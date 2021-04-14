using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.AirSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoActiveSyncOrganizationSettingsException : LocalizedException
	{
		public NoActiveSyncOrganizationSettingsException(string organizationId) : base(Strings.NoActiveSyncOrganizationSettingsException(organizationId))
		{
			this.organizationId = organizationId;
		}

		public NoActiveSyncOrganizationSettingsException(string organizationId, Exception innerException) : base(Strings.NoActiveSyncOrganizationSettingsException(organizationId), innerException)
		{
			this.organizationId = organizationId;
		}

		protected NoActiveSyncOrganizationSettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.organizationId = (string)info.GetValue("organizationId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("organizationId", this.organizationId);
		}

		public string OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		private readonly string organizationId;
	}
}
