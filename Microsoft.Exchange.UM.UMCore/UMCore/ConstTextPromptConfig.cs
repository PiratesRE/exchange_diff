using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ConstTextPromptConfig : PromptConfigBase
	{
		internal ConstTextPromptConfig(string promptName, string promptText, string conditionString, CultureInfo culture, ActivityManagerConfig managerConfig) : base(promptName, "text", conditionString, managerConfig)
		{
			TextPrompt textPrompt = new TextPrompt();
			PromptSetting config = new PromptSetting(this);
			textPrompt.Initialize(config, culture, promptText);
			this.textPrompts = new ArrayList();
			this.textPrompts.Add(textPrompt);
		}

		internal override void AddPrompts(ArrayList promptList, ActivityManager manager, CultureInfo culture)
		{
			promptList.AddRange(this.textPrompts);
		}

		internal override void Validate()
		{
		}

		private ArrayList textPrompts;
	}
}
