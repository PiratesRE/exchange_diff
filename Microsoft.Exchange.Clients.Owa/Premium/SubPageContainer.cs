using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class SubPageContainer : OwaForm, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "subPage", true);
			UserControl userControl = (UserControl)this.Page.LoadControl(queryStringParameter);
			this.subPage = (OwaSubPage)userControl;
			this.subPage.IsStandalonePage = true;
			this.subPagePlaceHolder.Controls.Add(userControl);
		}

		protected void RenderSubPageScripts()
		{
			base.RenderExternalScripts(ScriptFlags.IncludeUglobal, this.subPage.ExternalScriptFilesIncludeChildSubPages);
		}

		protected void RenderTitle()
		{
			base.SanitizingResponse.Write(this.subPage.Title);
		}

		protected void RenderPageType()
		{
			base.SanitizingResponse.Write("_PageType=\"");
			base.SanitizingResponse.Write(this.subPage.PageType);
			base.SanitizingResponse.Write("\"");
		}

		protected void RenderHtmlAdditionalAttributes()
		{
			string htmlAdditionalAttributes = this.subPage.HtmlAdditionalAttributes;
			if (!string.IsNullOrEmpty(htmlAdditionalAttributes))
			{
				base.SanitizingResponse.Write(" ");
				base.SanitizingResponse.Write(htmlAdditionalAttributes);
			}
		}

		protected void RenderIfSupportIM()
		{
			if (this.subPage.SupportIM)
			{
				base.SanitizingResponse.Write("_fIM=\"1\"");
			}
		}

		protected void RenderBodyCssClass()
		{
			base.SanitizingResponse.Write("class=\"");
			base.SanitizingResponse.Write(this.subPage.BodyCssClass);
			if (base.SessionContext.IsRtl)
			{
				base.SanitizingResponse.Write(" rtl");
			}
			base.SanitizingResponse.Write("\"");
		}

		protected PlaceHolder subPagePlaceHolder;

		private OwaSubPage subPage;
	}
}
