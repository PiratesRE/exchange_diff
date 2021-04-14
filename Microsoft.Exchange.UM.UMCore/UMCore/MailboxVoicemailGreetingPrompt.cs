using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class MailboxVoicemailGreetingPrompt : MailboxGreetingPrompt
	{
		protected override string PromptType
		{
			get
			{
				return "mbxVoicemailGreeting";
			}
		}

		protected override PromptConfigBase PromptConfig
		{
			get
			{
				return GlobCfg.DefaultPrompts.VoicemailGreeting;
			}
		}

		protected override PromptConfigBase PreviewConfig
		{
			get
			{
				return GlobCfg.DefaultPromptsForPreview.MbxVoicemailGreeting;
			}
		}
	}
}
