using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class AboutOptions : OptionsBase
	{
		public AboutOptions(OwaContext owaContext, TextWriter writer) : base(owaContext, writer)
		{
			this.Load(owaContext);
		}

		private void Load(OwaContext owaContext)
		{
			this.about = new AboutDetails(owaContext);
		}

		public override void Render()
		{
			string s = null;
			bool flag = false;
			base.RenderHeaderRow(ThemeFileId.AboutOwa, 282998996, 2);
			this.writer.Write("<tr><td colspan=2 class=\"optAbSpt\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(2138124634));
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td colspan=2 class=\"optAbSpc\"></td></tr>");
			for (int i = 0; i < this.about.Count; i++)
			{
				Strings.IDs localizedID;
				this.about.GetDetails(i, out localizedID, out s, out flag);
				this.writer.Write("<tr><td class=\"optAb\">");
				if (flag)
				{
					this.writer.Write("&nbsp; &nbsp; &nbsp;");
				}
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(localizedID));
				this.writer.Write(":</td><td class=\"optAbVal\">");
				this.writer.Write(Utilities.HtmlEncode(s));
				this.writer.Write("</td></tr>");
			}
		}

		public override void RenderScript()
		{
		}

		private AboutDetails about;
	}
}
