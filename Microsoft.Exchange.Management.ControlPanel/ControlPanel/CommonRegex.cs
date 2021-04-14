using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class CommonRegex
	{
		static CommonRegex()
		{
			CommonRegex.DsnCode = CommonRegex.CreateRegex("DsnCode", "^[2|4|5]\\.[0-9]\\.[0-9]$");
		}

		public static Regex Domain { get; private set; } = CommonRegex.CreateRegex("Domain", "^[-a-zA-Z0-9_.]+$");

		public static Regex DsnCode { get; private set; }

		public static Regex EmailOrDomain { get; private set; } = CommonRegex.CreateRegex("EmailOrDomain", "^@?[a-zA-Z0-9-_]+(\\.[a-z-A-Z0-9-_]+)+$|^[a-zA-Z0-9-_\\.]+@[a-zA-Z0-9-_]+(\\.[a-z-A-Z0-9-_]+)+$");

		public static Regex IpAddress { get; private set; } = CommonRegex.CreateRegex("IpAddress", "^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])([.]([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$");

		public static Regex UMNumberingPlanFormat { get; private set; } = CommonRegex.CreateRegex("UMNumberingPlanFormat", "^[+]?[x0-9]+$");

		public static Regex E164 { get; private set; } = CommonRegex.CreateRegex("E164", "^[+](([0-9]){1,15})$");

		public static Regex Url { get; private set; } = CommonRegex.CreateRegex("Url", "^(ht|f)tp(s?)\\:\\/\\/[0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*(:(0-9)*)*(\\/?)([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\\\+&amp;%\\$#_]*)?$");

		public static Regex NumbersOfSpecificLength(int minLength, int maxLength)
		{
			if (minLength < 1 || minLength > maxLength)
			{
				throw new ArgumentException("Must be greater than zero, and less than the maxLength", "minLength");
			}
			return new Regex(string.Format("^([0-9]){{{0},{1}}}$", minLength, maxLength));
		}

		internal static Regex GetRegexExpressionById(string key)
		{
			return CommonRegex.keyMapping[key];
		}

		private static Regex CreateRegex(string key, string value)
		{
			Regex regex = new Regex(value);
			CommonRegex.keyMapping.Add(key, regex);
			return regex;
		}

		private const string DomainRegex = "^[-a-zA-Z0-9_.]+$";

		private const string EmailOrDomainRegex = "^@?[a-zA-Z0-9-_]+(\\.[a-z-A-Z0-9-_]+)+$|^[a-zA-Z0-9-_\\.]+@[a-zA-Z0-9-_]+(\\.[a-z-A-Z0-9-_]+)+$";

		private const string IpAddressRegex = "^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])([.]([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";

		private const string UMNumberingPlanRegex = "^[+]?[x0-9]+$";

		private const string E164Regex = "^[+](([0-9]){1,15})$";

		private const string NumbersOfSpecificLengthFormat = "^([0-9]){{{0},{1}}}$";

		private const string DsnCodeRegex = "^[2|4|5]\\.[0-9]\\.[0-9]$";

		private const string UrlRegex = "^(ht|f)tp(s?)\\:\\/\\/[0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*(:(0-9)*)*(\\/?)([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\\\+&amp;%\\$#_]*)?$";

		private static readonly Dictionary<string, Regex> keyMapping = new Dictionary<string, Regex>(5);
	}
}
