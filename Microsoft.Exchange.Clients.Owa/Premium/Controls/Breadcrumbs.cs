using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class Breadcrumbs
	{
		public Breadcrumbs(ISessionContext sessionContext, NavigationModule navigationModule)
		{
			this.sessionContext = sessionContext;
			this.navigationModule = navigationModule;
		}

		public void Render(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write("<div id=\"divBreadcrumbs\" iModule=\"");
			output.Write((int)this.navigationModule);
			output.Write("\">");
			output.Write("<span id=\"spnMdNm\">");
			this.RenderModuleName(output);
			output.Write("</span>");
			this.sessionContext.RenderThemeImage(output, this.sessionContext.IsRtl ? ThemeFileId.BreadcrumbsArrowRtl : ThemeFileId.BreadcrumbsArrow, null, new object[]
			{
				"id=\"imgArw\""
			});
			output.Write("<span id=\"spnFldNm\"></span><span id=\"spnSpc\">&nbsp;&nbsp;</span>");
			output.Write("<span id=\"spnData\"></span>");
			output.Write("&nbsp;&nbsp;<a href=\"#\" id=\"lnkShwELC\" style=\"display:none\">");
			output.Write(LocalizedStrings.GetHtmlEncoded(-1591704162));
			output.Write("</a>");
			output.Write("<span id=\"spnPrg\" style=\"display:none\"><img src=\"");
			this.sessionContext.RenderThemeFileUrl(output, ThemeFileId.ProgressSmall);
			output.Write("\">&nbsp;");
			output.Write(LocalizedStrings.GetHtmlEncoded(-1961594409));
			output.Write("</span>");
			output.Write("</div>");
		}

		private void RenderModuleName(TextWriter output)
		{
			switch (this.navigationModule)
			{
			case NavigationModule.Mail:
				output.Write(LocalizedStrings.GetHtmlEncoded(405905481));
				return;
			case NavigationModule.Calendar:
				output.Write(LocalizedStrings.GetHtmlEncoded(1292798904));
				return;
			case NavigationModule.Contacts:
				output.Write(LocalizedStrings.GetHtmlEncoded(1716044995));
				return;
			case NavigationModule.Tasks:
				output.Write(LocalizedStrings.GetHtmlEncoded(-1328808356));
				return;
			case NavigationModule.AddressBook:
				output.Write(LocalizedStrings.GetHtmlEncoded(346766088));
				return;
			case NavigationModule.Documents:
				output.Write(LocalizedStrings.GetHtmlEncoded(-406393320));
				return;
			case NavigationModule.PublicFolders:
				output.Write(LocalizedStrings.GetHtmlEncoded(-1116491328));
				return;
			}
			output.Write(LocalizedStrings.GetHtmlEncoded(405905481));
		}

		private ISessionContext sessionContext;

		private NavigationModule navigationModule;
	}
}
