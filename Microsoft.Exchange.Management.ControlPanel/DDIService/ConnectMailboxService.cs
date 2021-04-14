using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class ConnectMailboxService
	{
		public static string GetMatchedUserFilter(object disp, object legDN)
		{
			string text = disp as string;
			string text2 = legDN as string;
			string text3 = null;
			if (text2 != null && text2.Length != 0)
			{
				int num = text2.ToUpperInvariant().LastIndexOf("/CN=");
				if (num != -1)
				{
					text3 = text2.Substring(num + "/CN=".Length);
				}
			}
			string text4 = (text == null || text.Length == 0) ? null : string.Format("DisplayName -eq '{0}' -or SamAccountName -eq '{0}'", text.Replace("'", "''"));
			string text5 = (text3 == null || text3.Length == 0) ? null : string.Format("DisplayName -eq '{0}' -or SamAccountName -eq '{0}'", text3.Replace("'", "''"));
			if (text4 == null && text5 == null)
			{
				DDIHelper.Trace("Invalid argument: disp or legDN can not be both null");
				throw new ArgumentOutOfRangeException("argument disp or legDN can not be both null");
			}
			string str;
			if (text4 == null)
			{
				str = text5;
			}
			else if (text5 == null)
			{
				str = text4;
			}
			else
			{
				str = text4 + " -or " + text5;
			}
			return "(" + str + ") -and (RecipientTypeDetails -eq 'User' -or RecipientTypeDetails -eq 'DisabledUser')";
		}

		private const string NamePrefixInLegacyDN = "/CN=";

		private const string FilterFormat = "DisplayName -eq '{0}' -or SamAccountName -eq '{0}'";
	}
}
