using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.Exchange.Security.OAuth
{
	public static class OAuthOutboundErrorsUtil
	{
		static OAuthOutboundErrorsUtil()
		{
			foreach (FieldInfo fieldInfo in typeof(OAuthOutboundErrorCodes).GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				OAuthOutboundErrorsUtil.errorDict.Add((OAuthOutboundErrorCodes)fieldInfo.GetValue(null), fieldInfo.GetCustomAttribute(false).Description);
			}
		}

		public static string GetDescription(OAuthOutboundErrorCodes error, object[] args)
		{
			if (args == null || args.Length <= 0)
			{
				return OAuthOutboundErrorsUtil.errorDict[error];
			}
			return string.Format(OAuthOutboundErrorsUtil.errorDict[error], args);
		}

		public static string GetDescription(OAuthOutboundErrorCodes error, string args = null)
		{
			if (args == null)
			{
				return OAuthOutboundErrorsUtil.errorDict[error];
			}
			return string.Format(OAuthOutboundErrorsUtil.errorDict[error], args);
		}

		private static readonly Dictionary<OAuthOutboundErrorCodes, string> errorDict = new Dictionary<OAuthOutboundErrorCodes, string>();
	}
}
