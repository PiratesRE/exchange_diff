using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	public enum CAAutoUpdateStatusEntry
	{
		[EnumMember]
		AutomaticUpdatesRequired,
		[EnumMember]
		AutomaticCheckForUpdates,
		[EnumMember]
		AutomaticDownloadUpdates,
		[EnumMember]
		NeverCheckUpdates,
		[EnumMember]
		DeviceDefault
	}
}
