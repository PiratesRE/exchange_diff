using System;
using System.Runtime.Serialization;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	public sealed class TenantCookie
	{
		public TenantCookie(Guid tenantId, byte[] cookie, Workload workload, ConfigurationObjectType objectType, DateTime? deletedObjectTimeThreshold)
		{
			this.TenantId = tenantId;
			this.Cookie = cookie;
			this.Workload = workload;
			this.ObjectType = objectType;
			this.DeletedObjectTimeThreshold = deletedObjectTimeThreshold;
		}

		[DataMember]
		public Guid TenantId { get; private set; }

		[DataMember]
		public bool MoreData { get; set; }

		[DataMember]
		public byte[] Cookie { get; set; }

		[DataMember]
		public Workload Workload { get; set; }

		[DataMember]
		public ConfigurationObjectType ObjectType { get; set; }

		[DataMember]
		public DateTime? DeletedObjectTimeThreshold { get; private set; }
	}
}
