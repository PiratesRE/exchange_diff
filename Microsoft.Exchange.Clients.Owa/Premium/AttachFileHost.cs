using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class AttachFileHost : OwaPage, IRegistryOnlyForm
	{
		protected bool IsInline
		{
			get
			{
				return Utilities.GetQueryStringParameter(base.Request, "a", false) != null;
			}
		}

		protected void RenderWindowTitle()
		{
			if (this.IsInline)
			{
				base.SanitizingResponse.Write(SanitizedHtmlString.FromStringId(-1408141425));
				return;
			}
			base.SanitizingResponse.Write(SanitizedHtmlString.FromStringId(763095470));
		}

		protected void RenderIframeSource()
		{
			base.SanitizingResponse.Write("?ae=Dialog&t=AttachFileDialog");
			if (this.IsInline)
			{
				base.SanitizingResponse.Write("&a=InsertImage");
			}
		}
	}
}
