using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class PercentCompleteDropDownList : DropDownList
	{
		public PercentCompleteDropDownList(string id, string percentComplete) : base(id, false, percentComplete, null)
		{
			this.percentComplete = percentComplete;
		}

		protected override void RenderExpandoData(TextWriter writer)
		{
			base.RenderExpandoData(writer);
			writer.Write(" L_InvldPc=\"");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-1094601321));
			writer.Write("\"");
		}

		protected override void RenderSelectedValue(TextWriter writer)
		{
			writer.Write("<input type=\"text\" id=\"txtInput\" maxlength=\"3\" value=\"");
			writer.Write(this.percentComplete);
			writer.Write("\">");
		}

		protected override void RenderListItems(TextWriter writer)
		{
		}

		private string percentComplete;
	}
}
