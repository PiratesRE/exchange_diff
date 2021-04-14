using System;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class AuthenticationImageHandler : IHttpHandler
	{
		bool IHttpHandler.IsReusable
		{
			get
			{
				return true;
			}
		}

		void IHttpHandler.ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "image/gif";
			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			context.Response.Cache.SetNoStore();
			context.Response.BinaryWrite(AuthenticationImageHandler.clearImageByteArray);
		}

		private static readonly byte[] clearImageByteArray = new byte[]
		{
			71,
			73,
			70,
			56,
			57,
			97,
			1,
			0,
			1,
			0,
			240,
			0,
			0,
			0,
			0,
			0,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			33,
			249,
			4,
			1,
			0,
			0,
			1,
			0,
			44,
			0,
			0,
			0,
			0,
			1,
			0,
			1,
			0,
			0,
			2,
			2,
			76,
			1,
			0,
			59
		};
	}
}
