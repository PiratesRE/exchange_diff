using System;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.MapiHttp
{
	internal static class HttpContextLoggingExtensionMethods
	{
		public static void AppendLogResponseInfo(this HttpContextBase context, ResponseCode? responseCode, LID? failureLID, AsyncOperation asyncOperation)
		{
			StringBuilder stringBuilder = context.Items["MapiHttpLoggingModuleLogger"] as StringBuilder;
			if (stringBuilder == null)
			{
				return;
			}
			string value = null;
			if (asyncOperation != null)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				asyncOperation.AppendLogString(stringBuilder2);
				value = stringBuilder2.ToString();
			}
			if (responseCode != null || failureLID != null || !string.IsNullOrEmpty(value))
			{
				string arg = string.Empty;
				stringBuilder.Append("&ResponseInfo=");
				if (responseCode != null)
				{
					stringBuilder.Append(string.Format("XRC:{0}", (int)responseCode.Value));
					arg = ";";
				}
				if (failureLID != null)
				{
					stringBuilder.Append(string.Format("{0}LID:{1}", arg, (int)failureLID.Value));
				}
				if (!string.IsNullOrEmpty(value))
				{
					stringBuilder.Append(value);
				}
			}
		}

		public static void AppendLogExceptionInfo(this HttpContextBase context, Exception failureException)
		{
			StringBuilder stringBuilder = context.Items["MapiHttpLoggingModuleLogger"] as StringBuilder;
			if (stringBuilder == null)
			{
				return;
			}
			if (failureException != null)
			{
				stringBuilder.Append("&ExceptionInfo=");
				stringBuilder.Append("type:");
				stringBuilder.Append(failureException.GetType().ToString());
				stringBuilder.Append(";message:");
				stringBuilder.Append(failureException.Message);
			}
		}

		private const string ResponseInfoLogParameter = "&ResponseInfo=";

		private const string ExceptionInfoLogParameter = "&ExceptionInfo=";
	}
}
