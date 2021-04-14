using System;
using System.Security;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class WebServiceHandler : IHttpHandler
	{
		[SecurityCritical]
		public void ProcessRequest(HttpContext context)
		{
			this.actualHandler.ProcessRequest(context);
		}

		public bool IsReusable
		{
			get
			{
				return this.actualHandler.IsReusable;
			}
		}

		private IHttpHandler actualHandler = (IHttpHandler)Activator.CreateInstance("System.ServiceModel.Activation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "System.ServiceModel.Activation.HttpHandler").Unwrap();
	}
}
