using System;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.FailFast;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Configuration.FailFast
{
	internal class BasicLiveIDAuthUserTokenParser : IUserTokenParser
	{
		private BasicLiveIDAuthUserTokenParser()
		{
		}

		internal static BasicLiveIDAuthUserTokenParser Instance
		{
			get
			{
				return BasicLiveIDAuthUserTokenParser.instance;
			}
		}

		public bool TryParseUserToken(HttpContext context, out string userToken)
		{
			Logger.EnterFunction(ExTraceGlobals.FailFastModuleTracer, "BasicLiveIDAuthUserTokenParser.TryParseUserToken");
			userToken = null;
			if (context == null || context.Request == null)
			{
				Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "Context or Context.Request is null.", new object[0]);
				return false;
			}
			string text = context.Request.Headers["X-WLID-MemberName"];
			if (!string.IsNullOrEmpty(text))
			{
				userToken = text;
			}
			else
			{
				string text2 = context.Request.Headers["Authorization"];
				byte[] bytes;
				byte[] array;
				if (!LiveIdBasicAuthModule.ParseCredentials(context, text2, false, out bytes, out array))
				{
					Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "Auth header \"{0}\" is not Basic LiveID Auth format. Ignore this request in BasicLiveIDAuthUserTokenParser.", new object[]
					{
						text2
					});
					return false;
				}
				userToken = Encoding.ASCII.GetString(bytes);
			}
			Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "Parse auth header and get user token {0}.", new object[]
			{
				userToken
			});
			Logger.ExitFunction(ExTraceGlobals.FailFastModuleTracer, "BasicLiveIDAuthUserTokenParser.TryParseUserToken");
			return true;
		}

		private static BasicLiveIDAuthUserTokenParser instance = new BasicLiveIDAuthUserTokenParser();
	}
}
