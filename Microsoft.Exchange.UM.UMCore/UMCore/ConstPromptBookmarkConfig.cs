using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ConstPromptBookmarkConfig : PromptConfigBase
	{
		internal ConstPromptBookmarkConfig(string promptName, string promptText, string conditionString, CultureInfo culture, ActivityManagerConfig managerConfig) : base(promptName, "bookmark", conditionString, managerConfig)
		{
			this.bookmark = new PromptBookmark();
			PromptSetting config = new PromptSetting(this);
			this.bookmark.Initialize(config, culture);
		}

		internal override void AddPrompts(ArrayList promptList, ActivityManager manager, CultureInfo culture)
		{
			promptList.Add(this.bookmark);
		}

		internal override void Validate()
		{
		}

		private PromptBookmark bookmark;
	}
}
