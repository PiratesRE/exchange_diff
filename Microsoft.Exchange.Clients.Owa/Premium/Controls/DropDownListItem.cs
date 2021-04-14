using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class DropDownListItem
	{
		public string ItemValue
		{
			get
			{
				return this.itemValue;
			}
		}

		public bool IsBold
		{
			get
			{
				return this.isBold;
			}
		}

		public string Display
		{
			get
			{
				if (this.displayIDs != -1018465893)
				{
					return LocalizedStrings.GetNonEncoded(this.displayIDs);
				}
				return this.displayText;
			}
		}

		public string ItemId
		{
			get
			{
				return this.itemId;
			}
		}

		public bool IsDisplayTextHtmlEncoded
		{
			get
			{
				return this.isDisplayTextHtmlEncoded;
			}
		}

		public DropDownListItem(object itemValue, Strings.IDs display)
		{
			if (itemValue == null)
			{
				throw new ArgumentNullException("itemValue");
			}
			if (display == -1018465893)
			{
				throw new ArgumentException("display cannot be equal to Strings.IDs.NotSet");
			}
			this.itemValue = itemValue.ToString();
			this.displayIDs = display;
		}

		public DropDownListItem(object itemValue, string display) : this(itemValue, display, false)
		{
		}

		public DropDownListItem(object itemValue, string display, bool isDisplayTextHtmlEncoded)
		{
			if (itemValue == null)
			{
				throw new ArgumentNullException("itemValue");
			}
			if (string.IsNullOrEmpty(display))
			{
				throw new ArgumentException("display cannot be null or empty string");
			}
			this.itemValue = itemValue.ToString();
			this.displayText = display;
			this.isDisplayTextHtmlEncoded = isDisplayTextHtmlEncoded;
		}

		public DropDownListItem(object itemValue, Strings.IDs display, bool isBold) : this(itemValue, display)
		{
			this.isBold = isBold;
		}

		public DropDownListItem(object itemValue, Strings.IDs display, string id, bool isBold) : this(itemValue, display)
		{
			this.isBold = isBold;
			this.itemId = id;
		}

		private string itemValue;

		private Strings.IDs displayIDs = -1018465893;

		private string displayText;

		private bool isBold;

		private string itemId;

		private bool isDisplayTextHtmlEncoded;
	}
}
