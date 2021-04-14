using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RegexValidatorInfo : ValidatorInfo
	{
		internal RegexValidatorInfo(RegexConstraint constraint) : this(constraint.Pattern, constraint.PatternDescription, RegexValidatorInfo.ConvertToJavaScriptOptions(constraint.Options))
		{
		}

		public RegexValidatorInfo(string pattern, string patternDescription, string options) : this("RegexValidator", pattern, patternDescription, options)
		{
		}

		protected RegexValidatorInfo(string validatorType, string pattern, string patternDescription, string options) : base(validatorType)
		{
			this.Pattern = pattern;
			this.PatternDescription = patternDescription;
			this.Options = options;
		}

		[DataMember]
		public string Pattern { get; set; }

		[DataMember]
		public string PatternDescription { get; set; }

		[DataMember]
		public string Options { get; set; }

		private static string ConvertToJavaScriptOptions(RegexOptions regexOptions)
		{
			string text = string.Empty;
			if ((regexOptions & RegexOptions.Multiline) == RegexOptions.Multiline)
			{
				text += "m";
			}
			if ((regexOptions & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase)
			{
				text += "i";
			}
			return text;
		}
	}
}
