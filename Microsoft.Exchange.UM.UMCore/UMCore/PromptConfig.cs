using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PromptConfig<TPrompt> : PromptConfigBase where TPrompt : Prompt, new()
	{
		internal PromptConfig(string name, string type, string conditionString, ActivityManagerConfig managerConfig) : base(name, type, conditionString, managerConfig)
		{
		}

		internal override void AddPrompts(ArrayList promptList, ActivityManager manager, CultureInfo culture)
		{
			TPrompt tprompt = Activator.CreateInstance<TPrompt>();
			PromptSetting config = new PromptSetting(this);
			tprompt.Initialize(config, culture);
			if (manager != null)
			{
				tprompt.SetProsodyRate(manager.ProsodyRate);
				tprompt.TTSLanguage = manager.MessagePlayerContext.Language;
			}
			promptList.Add(tprompt);
		}
	}
}
