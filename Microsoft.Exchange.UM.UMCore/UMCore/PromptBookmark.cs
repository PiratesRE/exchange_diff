using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PromptBookmark : Prompt
	{
		public override string ToString()
		{
			return base.Config.PromptName;
		}

		internal override string ToSsml()
		{
			return "<mark name=\"" + base.Config.PromptName + "\" />";
		}

		protected override void InternalInitialize()
		{
		}
	}
}
