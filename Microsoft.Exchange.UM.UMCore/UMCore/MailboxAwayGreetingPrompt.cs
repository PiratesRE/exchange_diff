using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class MailboxAwayGreetingPrompt : MailboxGreetingPrompt
	{
		protected override string PromptType
		{
			get
			{
				return "mbxAwayGreeting";
			}
		}

		protected override PromptConfigBase PromptConfig
		{
			get
			{
				return GlobCfg.DefaultPrompts.AwayGreeting;
			}
		}

		protected override PromptConfigBase PreviewConfig
		{
			get
			{
				return GlobCfg.DefaultPromptsForPreview.MbxAwayGreeting;
			}
		}
	}
}
