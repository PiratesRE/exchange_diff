using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.CssConverter
{
	internal class CssFragment
	{
		public CssFragment(IList<string> mediaDevices, string cssText)
		{
			this.MediaDevices = mediaDevices;
			this.CssText = cssText;
		}

		public IList<string> MediaDevices { get; private set; }

		public string CssText { get; private set; }
	}
}
