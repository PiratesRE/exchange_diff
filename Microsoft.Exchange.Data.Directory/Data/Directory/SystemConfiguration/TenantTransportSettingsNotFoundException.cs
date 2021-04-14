using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TenantTransportSettingsNotFoundException : TenantContainerNotFoundException
	{
		public TenantTransportSettingsNotFoundException(string orgId) : base(DirectoryStrings.TenantTransportSettingsNotFoundException(orgId))
		{
			this.orgId = orgId;
		}

		public TenantTransportSettingsNotFoundException(string orgId, Exception innerException) : base(DirectoryStrings.TenantTransportSettingsNotFoundException(orgId), innerException)
		{
			this.orgId = orgId;
		}

		protected TenantTransportSettingsNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.orgId = (string)info.GetValue("orgId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("orgId", this.orgId);
		}

		public string OrgId
		{
			get
			{
				return this.orgId;
			}
		}

		private readonly string orgId;
	}
}
