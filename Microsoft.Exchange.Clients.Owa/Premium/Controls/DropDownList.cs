using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class DropDownList : DropDownCombo
	{
		protected string SelectedValue
		{
			get
			{
				return this.selectedValue;
			}
			set
			{
				this.selectedValue = value;
			}
		}

		public string AdditionalListStyles
		{
			get
			{
				return this.additionalListStyles;
			}
			set
			{
				this.additionalListStyles = value;
			}
		}

		protected DropDownList(string id, bool showOnValueMouseDown, string selectedValue, DropDownListItem[] listItems) : base(id, showOnValueMouseDown)
		{
			this.selectedValue = selectedValue;
			this.listItems = listItems;
		}

		public DropDownList(string id, string selectedValue, DropDownListItem[] listItems) : this(id, true, selectedValue, listItems)
		{
		}

		public static void RenderDropDownList(TextWriter writer, string id, string selectedValue, params DropDownListItem[] listItems)
		{
			DropDownList dropDownList = new DropDownList(id, selectedValue, listItems);
			dropDownList.Render(writer);
		}

		protected static void RenderListItemHtml(TextWriter writer, object value, SanitizedHtmlString display, string id, bool isBold, bool isRtl)
		{
			writer.Write("<div tabIndex=0 oV=\"");
			Utilities.SanitizeHtmlEncode(value.ToString(), writer);
			if (!string.IsNullOrEmpty(id))
			{
				writer.Write("\" id=\"");
				Utilities.SanitizeHtmlEncode(id, writer);
			}
			writer.Write("\">");
			writer.Write("<span id=\"spanListItm\" class=\"listItm");
			if (isBold)
			{
				writer.Write(" bT");
			}
			writer.Write("\">");
			Utilities.RenderDirectionEnhancedValue(writer, display, isRtl);
			writer.Write("</span>");
			writer.Write("</div>");
		}

		protected static void RenderListItem(TextWriter writer, object value, string display, string id, bool isBold, bool isRtl)
		{
			DropDownList.RenderListItemHtml(writer, value, Utilities.SanitizeHtmlEncode(display), id, isBold, isRtl);
		}

		protected override void RenderExpandoData(TextWriter writer)
		{
			base.RenderExpandoData(writer);
			if (!string.IsNullOrEmpty(this.selectedValue))
			{
				writer.Write(" oV=\"");
				Utilities.SanitizeHtmlEncode(this.selectedValue, writer);
				writer.Write("\"");
			}
		}

		protected override void RenderDropControl(TextWriter writer)
		{
			writer.Write("<div id=\"divCmbList\" style=\"display:none\"");
			if (this.AdditionalListStyles != null)
			{
				writer.Write(" class=\"");
				writer.Write(this.AdditionalListStyles);
				writer.Write("\"");
			}
			writer.Write(">");
			this.RenderListItems(writer);
			writer.Write("</div>");
		}

		protected override void RenderSelectedValue(TextWriter writer)
		{
			for (int i = 0; i < this.ListItems.Length; i++)
			{
				DropDownListItem dropDownListItem = this.ListItems[i];
				if (dropDownListItem.ItemValue == this.selectedValue)
				{
					Utilities.SanitizeHtmlEncode(dropDownListItem.Display, writer);
					return;
				}
			}
		}

		protected virtual void RenderListItems(TextWriter writer)
		{
			for (int i = 0; i < this.ListItems.Length; i++)
			{
				DropDownListItem dropDownListItem = this.ListItems[i];
				if (dropDownListItem.IsDisplayTextHtmlEncoded)
				{
					DropDownList.RenderListItemHtml(writer, dropDownListItem.ItemValue, SanitizedHtmlString.GetSanitizedStringWithoutEncoding(dropDownListItem.Display), dropDownListItem.ItemId, dropDownListItem.IsBold, this.sessionContext.IsRtl);
				}
				else
				{
					DropDownList.RenderListItem(writer, dropDownListItem.ItemValue, dropDownListItem.Display, dropDownListItem.ItemId, dropDownListItem.IsBold, this.sessionContext.IsRtl);
				}
			}
		}

		private DropDownListItem[] ListItems
		{
			get
			{
				if (this.listItems == null)
				{
					this.listItems = this.CreateListItems();
				}
				return this.listItems;
			}
		}

		protected virtual DropDownListItem[] CreateListItems()
		{
			return null;
		}

		private string selectedValue;

		protected string additionalListStyles;

		private DropDownListItem[] listItems;
	}
}
