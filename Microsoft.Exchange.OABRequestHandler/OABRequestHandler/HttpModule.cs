using System;
using System.Web;

namespace Microsoft.Exchange.OABRequestHandler
{
	public class HttpModule : IHttpModule
	{
		public HttpModule()
		{
			this.fileHandler = new OABRequestHandler();
		}

		void IHttpModule.Init(HttpApplication application)
		{
			BITSDownloadManager.Instance.InitializeIfNecessary();
			application.PostAuthenticateRequest += this.PostAuthorizeRequest;
		}

		void IHttpModule.Dispose()
		{
		}

		private void PostAuthorizeRequest(object source, EventArgs args)
		{
			this.fileHandler.HandleRequest(HttpContext.Current);
		}

		private readonly OABRequestHandler fileHandler;
	}
}
