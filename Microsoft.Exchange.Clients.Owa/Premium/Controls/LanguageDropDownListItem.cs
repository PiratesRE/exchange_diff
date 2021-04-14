using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class LanguageDropDownListItem : DropDownListItem
	{
		public LanguageDropDownListItem(object itemValue, string display, bool isRtl) : base(itemValue, display)
		{
			if (itemValue == null)
			{
				throw new ArgumentNullException("itemValue");
			}
			if (string.IsNullOrEmpty(display))
			{
				throw new ArgumentException("display cannot be null or empty string");
			}
			this.isRtl = isRtl;
		}

		public bool IsRtl
		{
			get
			{
				return this.isRtl;
			}
		}

		private bool isRtl;
	}
}
