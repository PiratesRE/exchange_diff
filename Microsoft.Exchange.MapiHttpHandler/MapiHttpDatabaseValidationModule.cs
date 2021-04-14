using System;
using System.Web;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.MapiHttp
{
	public class MapiHttpDatabaseValidationModule : MapiHttpModule
	{
		internal override void InitializeModule(HttpApplication application)
		{
			application.PostAuthorizeRequest += delegate(object sender, EventArgs args)
			{
				this.OnPostAuthorizeRequest(MapiHttpContextWrapper.GetWrapper(((HttpApplication)sender).Context));
			};
		}

		internal override void OnPostAuthorizeRequest(HttpContextBase context)
		{
			if (string.Equals(context.Request.RequestType, "POST", StringComparison.OrdinalIgnoreCase))
			{
				string text = context.Request.Headers["X-RequestType"];
				if (!string.IsNullOrEmpty(text) && !string.Equals(text, "Connect", StringComparison.OrdinalIgnoreCase) && !string.Equals(text, "EcDoConnectEx", StringComparison.OrdinalIgnoreCase) && !string.Equals(text, "Bind", StringComparison.OrdinalIgnoreCase))
				{
					return;
				}
			}
			HttpDatabaseValidationHelper.ValidateHttpDatabaseHeader(context, delegate
			{
			}, delegate(string routingError)
			{
				this.SendErrorResponse(context, 555, 0, routingError, delegate(HttpResponseBase response)
				{
					response.Headers[WellKnownHeader.BEServerRoutingError] = routingError;
				});
			}, delegate
			{
				this.SendErrorResponse(context, 400, 0, "Invalid database guid");
			});
		}
	}
}
