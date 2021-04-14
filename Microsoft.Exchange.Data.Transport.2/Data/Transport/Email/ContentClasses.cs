using System;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal static class ContentClasses
	{
		public const string Voice = "voice";

		public const string VoiceCa = "voice-ca";

		public const string MissedCall = "missedcall";

		public const string Fax = "fax";

		public const string FaxCa = "fax-ca";

		public const string VoiceUc = "voice-uc";

		public const string UMPartner = "MS-Exchange-UM-Partner";

		public const string UnauthenticatedContentClassPrefix = "unauthenticated,";

		public const string RssPost = "RSS";

		public const string Sharing = "Sharing";

		public const string OmsSms = "MS-OMS-SMS";

		public const string OmsMms = "MS-OMS-MMS";

		public const string InfoPathContentClassPrefix = "InfoPathForm.";

		public const string CustomContentClassPrefix = "urn:content-class:custom.";

		public const string RpmsgMessage = "rpmsg.message";

		public const string UmcaRpmsgMessage = "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA";

		public const string UMInterPersonalRpmsgMessage = "IPM.Note.rpmsg.Microsoft.Voicemail.UM";
	}
}
