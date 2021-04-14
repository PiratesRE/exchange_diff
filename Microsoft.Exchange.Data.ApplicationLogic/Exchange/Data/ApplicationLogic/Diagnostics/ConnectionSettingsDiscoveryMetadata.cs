using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	internal enum ConnectionSettingsDiscoveryMetadata
	{
		[DisplayName("CSD", "RDE")]
		RequestDataFromEss,
		[DisplayName("CSD", "PER")]
		ProcessEssResponse,
		[DisplayName("CSD", "GOCS")]
		GetOffice365ConnectionSettings,
		[DisplayName("CSD", "ECSF")]
		EssConnectionSettingsFound,
		[DisplayName("CSD", "EX")]
		EssException,
		[DisplayName("CSD", "OCSF")]
		Office365ConnectionSettingsFound,
		[DisplayName("CSD", "D")]
		Domain
	}
}
