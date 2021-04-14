using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class UserAgentPattern
	{
		internal static bool IsMatch(string pattern, string userAgent)
		{
			if (pattern == null)
			{
				throw new ArgumentNullException("pattern");
			}
			if (string.Compare(pattern, "*", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(pattern, "**", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (pattern.StartsWith("*"))
			{
				string value;
				if (pattern.EndsWith("*"))
				{
					value = pattern.Substring(1, pattern.Length - 2);
					return !string.IsNullOrEmpty(userAgent) && userAgent.Contains(value);
				}
				value = pattern.Remove(0, 1);
				return !string.IsNullOrEmpty(userAgent) && userAgent.EndsWith(value);
			}
			else
			{
				if (pattern.EndsWith("*"))
				{
					string value = pattern.Remove(pattern.Length - 1, 1);
					return !string.IsNullOrEmpty(userAgent) && userAgent.StartsWith(value);
				}
				return string.Compare(pattern, userAgent, StringComparison.OrdinalIgnoreCase) == 0;
			}
		}
	}
}
