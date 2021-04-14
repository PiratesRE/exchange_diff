using System;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class GoHelpHandler : IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			string text = context.Request.QueryString["helpid"];
			if (string.IsNullOrEmpty(text) || (!Enum.IsDefined(typeof(EACHelpId), text) && !Enum.IsDefined(typeof(OptionsHelpId), text)))
			{
				throw new BadRequestException(new Exception("Invalid HelpId: \"" + text + "\" ."));
			}
			context.Response.Redirect(HelpUtil.BuildEhcHref(text));
		}
	}
}
