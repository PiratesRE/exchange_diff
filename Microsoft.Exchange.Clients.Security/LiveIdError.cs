using System;
using System.Threading;
using System.Web;
using System.Web.UI;

namespace Microsoft.Exchange.Clients.Security
{
	public class LiveIdError : Page
	{
		public bool IsRtl
		{
			get
			{
				return Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			this.errorInformation = (HttpContext.Current.Items["LiveIdErrorInformation"] as LiveIdErrorInformation);
			if (this.errorInformation == null)
			{
				this.errorInformation = new LiveIdErrorInformation();
				this.errorInformation.Message = Strings.ErrorTitle;
				this.errorInformation.MessageDetails = Strings.ErrorUnexpectedFailure;
			}
			else if (this.errorInformation.Exception != null)
			{
				HttpContext.Current.Response.Headers.Add("X-OWA-Error", this.errorInformation.Exception.GetType().FullName);
			}
			this.OnInit(e);
		}

		protected void RenderIcon()
		{
			base.Response.Write(this.errorInformation.Icon);
		}

		protected void RenderBackground()
		{
			base.Response.Write(this.errorInformation.Background);
		}

		protected void RenderErrorPageTitle()
		{
			base.Response.Write(Strings.ErrorTitle);
		}

		protected void RenderMessage()
		{
			base.Response.Write(this.errorInformation.Message);
		}

		protected void RenderMessageDetails()
		{
			base.Response.Write(this.errorInformation.MessageDetails);
		}

		protected LiveIdErrorInformation errorInformation;
	}
}
