using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal struct TelephonyInfo
	{
		public TelephonyInfo(PhoneNumber accessNumber, PhoneNumber voicemailNumber)
		{
			this.AccessNumber = accessNumber;
			this.VoicemailNumber = voicemailNumber;
		}

		public static readonly TelephonyInfo Empty = new TelephonyInfo(null, null);

		public PhoneNumber AccessNumber;

		public PhoneNumber VoicemailNumber;
	}
}
