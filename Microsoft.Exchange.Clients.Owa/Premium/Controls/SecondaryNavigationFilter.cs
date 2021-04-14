using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class SecondaryNavigationFilter
	{
		public SecondaryNavigationFilter(string elementId, string title, string onClickHandler)
		{
			this.elementId = elementId;
			this.title = title;
			this.onClickHandler = onClickHandler;
		}

		public void AddFilter(string displayString, int value, bool isSelected)
		{
			SecondaryNavigationFilter.FilterInfo item;
			item.DisplayString = displayString;
			item.Value = value;
			item.IsSelected = isSelected;
			this.filters.Add(item);
		}

		private string RadioButtonName
		{
			get
			{
				return "_" + this.elementId;
			}
		}

		public void Render(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write("<div id=\"");
			output.Write(this.elementId);
			output.Write("\" _onclick=\"");
			Utilities.HtmlEncode(this.onClickHandler, output);
			output.Write("\" fSNFlt=\"1\" class=\"secNvFltRg\">");
			output.Write("<div class=\"secNvFltTtl\">");
			Utilities.HtmlEncode(this.title, output);
			output.Write("</div>");
			for (int i = 0; i < this.filters.Count; i++)
			{
				this.RenderFilter(output, this.filters[i]);
			}
			output.Write("</div>");
		}

		private void RenderFilter(TextWriter output, SecondaryNavigationFilter.FilterInfo filter)
		{
			output.Write("<div id=\"divSecNvFltItm\" _iF=\"");
			output.Write(filter.Value);
			output.Write("\"><div class=\"secNvFltRdDiv\"><input id=\"inpRdo\" class=\"secNvFltRd\" type=radio name=");
			output.Write(this.RadioButtonName);
			if (filter.IsSelected)
			{
				output.Write(" checked");
			}
			output.Write("></div><div class=\"secNvFltTxt\">");
			Utilities.HtmlEncode(filter.DisplayString, output);
			output.Write("</div></div>");
		}

		private string elementId;

		private string title;

		private string onClickHandler;

		private List<SecondaryNavigationFilter.FilterInfo> filters = new List<SecondaryNavigationFilter.FilterInfo>();

		private struct FilterInfo
		{
			public string DisplayString;

			public int Value;

			public bool IsSelected;
		}
	}
}
