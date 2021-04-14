using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface IConfigurationFolder : IPromptCounter
	{
		MailboxGreetingEnum CurrentMailboxGreetingType { get; set; }

		bool IsFirstTimeUser { get; set; }

		bool IsOof { get; set; }

		string PlayOnPhoneDialString { get; set; }

		string TelephoneAccessFolderEmail { get; set; }

		bool UseAsr { get; set; }

		bool ReceivedVoiceMailPreviewEnabled { get; set; }

		bool SentVoiceMailPreviewEnabled { get; set; }

		bool ReadUnreadVoicemailInFIFOOrder { get; set; }

		MultiValuedProperty<string> BlockedNumbers { get; set; }

		VoiceNotificationStatus VoiceNotificationStatus { get; set; }

		GreetingBase OpenCustomMailboxGreeting(MailboxGreetingEnum g);

		GreetingBase OpenNameGreeting();

		IPassword OpenPassword();

		void RemoveCustomMailboxGreeting(MailboxGreetingEnum g);

		bool HasCustomMailboxGreeting(MailboxGreetingEnum g);

		void Save();
	}
}
