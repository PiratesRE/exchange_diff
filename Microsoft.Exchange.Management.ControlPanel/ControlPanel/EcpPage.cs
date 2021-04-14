using System;
using System.Threading;
using System.Web;
using System.Web.UI;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class EcpPage : Page
	{
		public FeatureSet FeatureSet { get; set; }

		protected override void OnInit(EventArgs e)
		{
			this.SetThreadPrincipal();
			base.OnInit(e);
			base.Response.Headers["X-BEResourcePath"] = HttpRuntime.AppDomainAppVirtualPath + "/" + ThemeResource.ApplicationVersion;
		}

		protected override void OnLoad(EventArgs e)
		{
			this.SetThreadPrincipal();
			base.OnLoad(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.SetThreadPrincipal();
			base.OnPreRender(e);
		}

		protected override void OnPreRenderComplete(EventArgs e)
		{
			this.SetThreadPrincipal();
			base.OnPreRenderComplete(e);
		}

		private void SetThreadPrincipal()
		{
			if (base.User == Thread.CurrentPrincipal)
			{
				((IRbacSession)base.User).SetCurrentThreadPrincipal();
			}
		}

		public const string BEResourcePathHeaderName = "X-BEResourcePath";
	}
}
