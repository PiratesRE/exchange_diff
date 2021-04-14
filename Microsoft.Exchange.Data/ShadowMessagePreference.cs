using System;

namespace Microsoft.Exchange.Data
{
	public enum ShadowMessagePreference
	{
		[LocDescription(DataStrings.IDs.ShadowMessagePreferencePreferRemote)]
		PreferRemote,
		[LocDescription(DataStrings.IDs.ShadowMessagePreferenceLocalOnly)]
		LocalOnly,
		[LocDescription(DataStrings.IDs.ShadowMessagePreferenceRemoteOnly)]
		RemoteOnly
	}
}
