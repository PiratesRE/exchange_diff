using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Security
{
	internal static class StringSanitizer<SanitizingPolicy> where SanitizingPolicy : ISanitizingPolicy, new()
	{
		public static bool TrustedStringsInitialized
		{
			get
			{
				return StringSanitizer<SanitizingPolicy>.trustedStrings.Count > 0;
			}
		}

		public static SanitizingPolicy PolicyObject
		{
			get
			{
				return StringSanitizer<SanitizingPolicy>.policy;
			}
		}

		public static bool InitializeTrustedStrings(params string[] assemblies)
		{
			bool flag = true;
			HashSet<string> hashSet = new HashSet<string>();
			for (int i = 0; i < assemblies.Length; i++)
			{
			}
			hashSet.Add(string.Empty);
			if (flag)
			{
				StringSanitizer<SanitizingPolicy>.trustedStrings = hashSet;
			}
			return flag;
		}

		public static void ExcludeStrings(params Regex[] regexes)
		{
			HashSet<string> hashSet = StringSanitizer<SanitizingPolicy>.trustedStrings;
			HashSet<string> hashSet2 = new HashSet<string>();
			foreach (string text in hashSet)
			{
				bool flag = true;
				foreach (Regex regex in regexes)
				{
					if (regex == null)
					{
						throw new ArgumentNullException("regexes");
					}
					if (regex.IsMatch(text))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					hashSet2.Add(text);
				}
			}
			hashSet2.Add(string.Empty);
			StringSanitizer<SanitizingPolicy>.trustedStrings = hashSet2;
		}

		public static string Sanitize(string str)
		{
			SanitizingPolicy sanitizingPolicy = StringSanitizer<SanitizingPolicy>.policy;
			return sanitizingPolicy.Sanitize(str);
		}

		public static string SanitizeFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			SanitizingPolicy sanitizingPolicy = StringSanitizer<SanitizingPolicy>.policy;
			return sanitizingPolicy.SanitizeFormat(formatProvider, format, args);
		}

		public static void Sanitize(TextWriter writer, string str)
		{
			SanitizingPolicy sanitizingPolicy = StringSanitizer<SanitizingPolicy>.policy;
			sanitizingPolicy.Sanitize(writer, str);
		}

		public static bool IsTrustedString(string str)
		{
			return object.ReferenceEquals(string.IsInterned(str), str);
		}

		private static readonly SanitizingPolicy policy = (default(SanitizingPolicy) == null) ? Activator.CreateInstance<SanitizingPolicy>() : default(SanitizingPolicy);

		private static HashSet<string> trustedStrings = new HashSet<string>();
	}
}
