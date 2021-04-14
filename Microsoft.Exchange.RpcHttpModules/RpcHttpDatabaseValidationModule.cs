using System;
using System.Web;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.RpcHttpModules
{
	public class RpcHttpDatabaseValidationModule : RpcHttpModule
	{
		internal override void InitializeModule(HttpApplication application)
		{
			application.PostAuthorizeRequest += delegate(object sender, EventArgs args)
			{
				this.OnPostAuthorizeRequest(new HttpContextWrapper(((HttpApplication)sender).Context));
			};
		}

		internal override void OnPostAuthorizeRequest(HttpContextBase context)
		{
			HttpDatabaseValidationHelper.ValidateHttpDatabaseHeader(context, delegate
			{
				if (context.Request.HttpMethod == "HEAD")
				{
					this.Application.CompleteRequest();
				}
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
