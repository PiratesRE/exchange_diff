using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public class UMMailboxPolicyService : DDICodeBehind
	{
		public static string ToDaysString(object value)
		{
			string result = string.Empty;
			if (!DDIHelper.IsEmptyValue(value))
			{
				Unlimited<EnhancedTimeSpan> unlimited = (Unlimited<EnhancedTimeSpan>)value;
				result = (unlimited.IsUnlimited ? unlimited.ToString() : unlimited.Value.Days.ToString());
			}
			return result;
		}
	}
}
