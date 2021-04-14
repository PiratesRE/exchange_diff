using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.Exchange.Security.OAuth
{
	internal static class OAuthErrorsUtil
	{
		static OAuthErrorsUtil()
		{
			foreach (FieldInfo fieldInfo in typeof(OAuthErrors).GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				OAuthErrorsUtil.errorDict.Add((OAuthErrors)fieldInfo.GetValue(null), fieldInfo.GetCustomAttribute(false).Description);
			}
		}

		public static string GetDescription(OAuthErrors error)
		{
			return OAuthErrorsUtil.errorDict[error];
		}

		public static OAuthErrorCategory GetErrorCategory(OAuthErrors error)
		{
			if (error >= OAuthErrors.OfficeSharedErrorCodes)
			{
				return OAuthErrorCategory.OAuthNotAvailable + (error - OAuthErrors.OfficeSharedErrorCodes) / 1000 - 1;
			}
			return OAuthErrorCategory.InvalidSignature + (int)(error / (OAuthErrors)1000) - 1;
		}

		private static Dictionary<OAuthErrors, string> errorDict = new Dictionary<OAuthErrors, string>();
	}
}
