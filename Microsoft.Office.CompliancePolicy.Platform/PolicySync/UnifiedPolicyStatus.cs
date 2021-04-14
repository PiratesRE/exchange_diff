using System;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	[Serializable]
	public sealed class UnifiedPolicyStatus
	{
		public UnifiedPolicyStatus()
		{
			this.Version = PolicyVersion.Empty;
		}

		[DataMember]
		public Guid TenantId { get; set; }

		[DataMember]
		public ConfigurationObjectType ObjectType { get; set; }

		[DataMember]
		public Guid ObjectId { get; set; }

		[DataMember]
		public Guid? ParentObjectId { get; set; }

		[DataMember]
		public PolicyVersion Version { get; set; }

		[DataMember]
		public Workload Workload { get; set; }

		[DataMember]
		public UnifiedPolicyErrorCode ErrorCode { get; set; }

		[DataMember]
		public string ErrorMessage { get; set; }

		[DataMember]
		public DateTime WhenProcessedUTC { get; set; }

		[DataMember]
		public Mode Mode { get; set; }

		[DataMember]
		public string AdditionalDiagnostics { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("TenantId:{0},", this.TenantId));
			stringBuilder.Append(string.Format("ObjectType:{0},", this.ObjectType));
			StringBuilder stringBuilder2 = stringBuilder;
			string format = "ObjectId:{0},";
			Guid objectId = this.ObjectId;
			stringBuilder2.Append(string.Format(format, this.ObjectId.ToString()));
			stringBuilder.Append(string.Format("ParentObjectId:{0},", this.ParentObjectId));
			stringBuilder.Append(string.Format("Version:{0},", (this.Version == null) ? "<null>" : this.Version.ToString()));
			stringBuilder.Append(string.Format("Workload:{0},", this.Workload));
			stringBuilder.Append(string.Format("ErrorCode:{0},", this.ErrorCode));
			stringBuilder.Append(string.Format("ErrorMessage:{0},", this.ErrorMessage));
			stringBuilder.Append(string.Format("WhenProcessedUTC:{0},", this.WhenProcessedUTC));
			stringBuilder.Append(string.Format("Mode:{0},", this.Mode));
			stringBuilder.Append(string.Format("AdditionalDiagnostics:{0}", this.AdditionalDiagnostics));
			return stringBuilder.ToString();
		}
	}
}
