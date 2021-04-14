using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ConstFilePromptConfig : PromptConfigBase
	{
		internal ConstFilePromptConfig(string fileName, string conditionString, CultureInfo culture, ActivityManagerConfig managerConfig) : base(fileName, "wave", conditionString, managerConfig)
		{
			this.filePrompt = new FilePrompt();
			PromptSetting config = new PromptSetting(this);
			this.filePrompt.Initialize(config, culture);
		}

		internal override void AddPrompts(ArrayList promptList, ActivityManager manager, CultureInfo culture)
		{
			promptList.Add(this.filePrompt);
		}

		internal override void Validate()
		{
		}

		private FilePrompt filePrompt;
	}
}
