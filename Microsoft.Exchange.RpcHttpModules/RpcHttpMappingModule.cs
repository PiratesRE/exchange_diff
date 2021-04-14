using System;
using System.Web;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.RpcHttpModules
{
	public class RpcHttpMappingModule : RpcHttpModule
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
			bool flag = false;
			string text;
			string text2;
			string str;
			RpcHttpMappingModule.ParseQueryString(context.Request.Url.Query, out text, out text2, out str);
			if (!string.IsNullOrEmpty(text2) && text2 != "6001" && text2 != "6002" && text2 != "6004")
			{
				base.SendErrorResponse(context, 400, string.Format("Invalid RPC Port: {0}", text2 ?? "<empty>"));
				return;
			}
			if (!string.IsNullOrEmpty(context.Request.Headers[WellKnownHeader.XIsFromCafe]) || text.Contains("@"))
			{
				text = "localhost";
				flag = true;
			}
			if (text2 == "6002" || text2 == "6004")
			{
				text2 = "6001";
				flag = true;
			}
			if (flag)
			{
				string queryString = text + ":" + text2 + str;
				context.RewritePath(context.Request.FilePath, context.Request.PathInfo, queryString);
			}
		}

		protected static void ParseQueryString(string queryString, out string rpcServer, out string rpcServerPort, out string additionalParameters)
		{
			rpcServer = string.Empty;
			rpcServerPort = string.Empty;
			additionalParameters = string.Empty;
			if (string.IsNullOrEmpty(queryString))
			{
				return;
			}
			int num = queryString.IndexOf(':', 1);
			if (num == -1)
			{
				num = queryString.Length;
			}
			rpcServer = queryString.Substring(1, num - 1);
			int num2 = queryString.IndexOf('&', num);
			if (num2 == -1)
			{
				num2 = queryString.Length;
			}
			int num3 = num2 - (num + 1);
			if (num3 > 0)
			{
				rpcServerPort = queryString.Substring(num + 1, num3);
			}
			if (num2 < queryString.Length)
			{
				additionalParameters = queryString.Substring(num2);
			}
		}

		private const string ConsolidatedRcaServerPort = "6001";
	}
}
