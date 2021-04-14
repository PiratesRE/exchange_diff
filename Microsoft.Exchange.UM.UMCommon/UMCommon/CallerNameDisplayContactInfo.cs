using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	internal class CallerNameDisplayContactInfo : SimpleContactInfoBase
	{
		internal CallerNameDisplayContactInfo(string displayName)
		{
			this.displayName = displayName;
		}

		internal override string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		private string displayName;
	}
}
