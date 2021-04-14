using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class SubPageEventHandler : OwaForm, IRegistryOnlyForm
	{
		protected override void OnPreRender(EventArgs e)
		{
			if (!this.hasError)
			{
				base.Response.AppendHeader("X-OWA-EventResult", "0");
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "subPage", true);
			if (string.IsNullOrEmpty(queryStringParameter))
			{
				throw new OwaInvalidRequestException("subPagePath parameter cannot be null or empty");
			}
			UserControl userControl = (UserControl)this.Page.LoadControl(queryStringParameter);
			if (userControl is OwaSubPage)
			{
				this.subPage = (OwaSubPage)userControl;
				this.subPage.IsInOEHResponse = true;
				this.subPagePlaceHolder.Controls.Add(userControl);
				return;
			}
			throw new OwaInvalidRequestException("The user control is not of OwaSubPage type");
		}

		protected void RenderSubPageScripts()
		{
			this.subPage.RenderExternalScriptFiles();
		}

		protected void RenderTitle()
		{
			base.SanitizingResponse.Write(this.subPage.Title);
		}

		protected override void OnError(EventArgs e)
		{
			this.hasError = true;
			Exception lastError = base.Server.GetLastError();
			base.Server.ClearError();
			Utilities.HandleException(base.OwaContext, lastError, false);
		}

		protected void RenderPageType()
		{
			base.SanitizingResponse.Write("_PageType=\"");
			base.SanitizingResponse.Write(this.subPage.PageType);
			base.SanitizingResponse.Write("\"");
		}

		protected PlaceHolder subPagePlaceHolder;

		private OwaSubPage subPage;

		private bool hasError;
	}
}
