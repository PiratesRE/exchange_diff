using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PublicFolderDatabaseDoesNotBelongToTenant : LocalizedException
	{
		public PublicFolderDatabaseDoesNotBelongToTenant(string pfId, string tenantId) : base(Strings.PublicFolderDatabaseDoesNotBelongToTenant(pfId, tenantId))
		{
			this.pfId = pfId;
			this.tenantId = tenantId;
		}

		public PublicFolderDatabaseDoesNotBelongToTenant(string pfId, string tenantId, Exception innerException) : base(Strings.PublicFolderDatabaseDoesNotBelongToTenant(pfId, tenantId), innerException)
		{
			this.pfId = pfId;
			this.tenantId = tenantId;
		}

		protected PublicFolderDatabaseDoesNotBelongToTenant(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.pfId = (string)info.GetValue("pfId", typeof(string));
			this.tenantId = (string)info.GetValue("tenantId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("pfId", this.pfId);
			info.AddValue("tenantId", this.tenantId);
		}

		public string PfId
		{
			get
			{
				return this.pfId;
			}
		}

		public string TenantId
		{
			get
			{
				return this.tenantId;
			}
		}

		private readonly string pfId;

		private readonly string tenantId;
	}
}
