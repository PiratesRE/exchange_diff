using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TenantOrgContainerNotFoundException : TenantContainerNotFoundException
	{
		public TenantOrgContainerNotFoundException(string orgId) : base(DirectoryStrings.TenantOrgContainerNotFoundException(orgId))
		{
			this.orgId = orgId;
		}

		public TenantOrgContainerNotFoundException(string orgId, Exception innerException) : base(DirectoryStrings.TenantOrgContainerNotFoundException(orgId), innerException)
		{
			this.orgId = orgId;
		}

		protected TenantOrgContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
