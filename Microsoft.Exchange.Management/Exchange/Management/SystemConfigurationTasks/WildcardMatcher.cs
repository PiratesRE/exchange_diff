using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class WildcardMatcher
	{
		internal WildcardMatcher(string wildcardPattern) : this(new MultiValuedProperty<string>(wildcardPattern))
		{
		}

		internal WildcardMatcher(MultiValuedProperty<string> wildcardPatterns)
		{
			this.regex = null;
			if (wildcardPatterns == null || wildcardPatterns.Count == 0)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string str in wildcardPatterns)
			{
				stringBuilder.Append("^" + Regex.Escape(str).Replace("\\*", ".*").Replace("\\?", ".") + "$|");
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				this.regex = new Regex(stringBuilder.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);
			}
		}

		internal bool IsMatch(string inputString)
		{
			return this.regex != null && inputString != null && this.regex.IsMatch(inputString);
		}

		private Regex regex;
	}
}
