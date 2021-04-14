using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	public enum CAUserAccountControlStatusEntry
	{
		[EnumMember]
		AlwaysNotify = 1,
		[EnumMember]
		NotifyAppChanges,
		[EnumMember]
		NotifyAppChangesDoNotDimdesktop,
		[EnumMember]
		NeverNotify
	}
}
