using System;
using System.Text;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Monitoring
{
	public sealed class TestOAuthWacConnectivityHelper
	{
		public static ResultType SendWacOAuthRequest(string wopiUrl, string wacTemplateUrl, ADUser user, out string diagnosticMessage)
		{
			StringBuilder stringBuilder = new StringBuilder();
			WacResult wacResult = null;
			try
			{
				wacResult = WacWorker.ExecuteWacRequest(wacTemplateUrl, wopiUrl, user, stringBuilder);
			}
			catch (Exception ex)
			{
				stringBuilder.AppendLine("Unhandled Exception while running Wac Probe.");
				stringBuilder.AppendLine(ex.ToString());
				diagnosticMessage = stringBuilder.ToString();
				return ResultType.Error;
			}
			diagnosticMessage = stringBuilder.ToString();
			if (wacResult.Error)
			{
				return ResultType.Error;
			}
			return ResultType.Success;
		}

		public const string ComponentId = "OAuthWacProbe:";
	}
}
