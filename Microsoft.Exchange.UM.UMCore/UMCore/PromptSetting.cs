using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal struct PromptSetting
	{
		public PromptSetting(PromptConfigBase promptConfig)
		{
			this.ProsodyRate = promptConfig.ProsodyRate;
			this.PromptName = promptConfig.PromptName;
			this.Language = promptConfig.Language;
		}

		public PromptSetting(string name)
		{
			this.PromptName = string.Intern(name.Trim());
			this.Language = string.Empty;
			this.ProsodyRate = "+0%";
		}

		public string ProsodyRate;

		public string PromptName;

		public string Language;
	}
}
