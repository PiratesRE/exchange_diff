using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	public enum AuditableOperations
	{
		[EnumMember]
		None,
		[EnumMember]
		Administrate,
		[EnumMember]
		CreateUpdate,
		[EnumMember]
		View,
		[EnumMember]
		MoveCopy,
		[EnumMember]
		Delete,
		[EnumMember]
		Forward,
		[EnumMember]
		SendAsOthers,
		[EnumMember]
		PermissionChange,
		[EnumMember]
		CheckOut,
		[EnumMember]
		CheckIn,
		[EnumMember]
		Workflow,
		[EnumMember]
		Search,
		[EnumMember]
		SchemaChange,
		[EnumMember]
		ProfileChange,
		Count
	}
}
