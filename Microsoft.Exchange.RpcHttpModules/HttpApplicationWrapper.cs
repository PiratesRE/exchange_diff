using System;
using System.Web;

namespace Microsoft.Exchange.RpcHttpModules
{
	public class HttpApplicationWrapper : HttpApplicationBase
	{
		public HttpApplicationWrapper(HttpApplication application)
		{
			this.application = application;
		}

		public HttpApplication Instance
		{
			get
			{
				return this.application;
			}
		}

		public override void CompleteRequest()
		{
			this.application.CompleteRequest();
		}

		private readonly HttpApplication application;
	}
}
