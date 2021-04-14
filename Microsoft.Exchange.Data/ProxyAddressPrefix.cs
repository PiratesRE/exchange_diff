using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[ImmutableObject(true)]
	[Serializable]
	public abstract class ProxyAddressPrefix
	{
		protected ProxyAddressPrefix(string prefixString)
		{
			ProxyAddressPrefix.CheckPrefixString(prefixString, true);
			this.primaryPrefix = prefixString.ToUpperInvariant();
			this.secondaryPrefix = prefixString.ToLowerInvariant();
		}

		private static bool CheckPrefixString(string prefixString, bool canThrow)
		{
			Exception ex;
			if (prefixString == null)
			{
				ex = new ArgumentNullException("prefixString");
			}
			else if (-1 != prefixString.IndexOf(':'))
			{
				ex = new ArgumentException(DataStrings.ColonPrefix, "prefixString");
			}
			else if (prefixString.Length != 0 && string.IsNullOrEmpty(prefixString.Trim()))
			{
				ex = new ArgumentOutOfRangeException(DataStrings.ProxyAddressPrefixShouldNotBeAllSpace, null);
			}
			else if (prefixString.Length > 9)
			{
				ex = new ArgumentOutOfRangeException(DataStrings.ProxyAddressPrefixTooLong, null);
			}
			else if (!ProxyAddressPrefix.asciiRegex.IsMatch(prefixString))
			{
				ex = new ArgumentOutOfRangeException(DataStrings.ConstraintViolationStringDoesContainsNonASCIICharacter(prefixString), null);
			}
			else
			{
				ex = null;
			}
			if (ex != null && canThrow)
			{
				throw ex;
			}
			return ex == null;
		}

		public static bool IsPrefixStringValid(string prefixString)
		{
			return ProxyAddressPrefix.CheckPrefixString(prefixString, false);
		}

		public virtual string DisplayName
		{
			get
			{
				return this.PrimaryPrefix;
			}
		}

		public string PrimaryPrefix
		{
			get
			{
				return this.primaryPrefix;
			}
		}

		public string SecondaryPrefix
		{
			get
			{
				return this.secondaryPrefix;
			}
		}

		public sealed override string ToString()
		{
			return this.PrimaryPrefix;
		}

		public abstract ProxyAddress GetProxyAddress(string address, bool isPrimaryAddress);

		public abstract ProxyAddressTemplate GetProxyAddressTemplate(string addressTemplate, bool isPrimaryAddressTemplate);

		public sealed override int GetHashCode()
		{
			return this.PrimaryPrefix.GetHashCode();
		}

		public sealed override bool Equals(object obj)
		{
			return this == obj as ProxyAddressPrefix;
		}

		public static bool operator ==(ProxyAddressPrefix a, ProxyAddressPrefix b)
		{
			return a == b || (a != null && b != null && a.PrimaryPrefix == b.PrimaryPrefix);
		}

		public static bool operator !=(ProxyAddressPrefix a, ProxyAddressPrefix b)
		{
			return !(a == b);
		}

		public static ProxyAddressPrefix GetPrefix(string prefixString)
		{
			if (prefixString == null)
			{
				throw new ArgumentNullException("prefixString");
			}
			ProxyAddressPrefix result;
			if (!ProxyAddressPrefix.standardPrefixes.TryGetValue(prefixString, out result))
			{
				result = new CustomProxyAddressPrefix(prefixString);
			}
			return result;
		}

		public static ProxyAddressPrefix GetCustomProxyAddressPrefix()
		{
			return new CustomProxyAddressPrefix("", DataStrings.CustomProxyAddressPrefixDisplayName);
		}

		public static ProxyAddressPrefix[] GetStandardPrefixes()
		{
			ProxyAddressPrefix[] array = new ProxyAddressPrefix[ProxyAddressPrefix.standardPrefixes.Count + 1];
			ProxyAddressPrefix.standardPrefixes.Values.CopyTo(array, 0);
			array[array.Length - 1] = ProxyAddressPrefix.GetCustomProxyAddressPrefix();
			return array;
		}

		static ProxyAddressPrefix()
		{
			ProxyAddressPrefix.standardPrefixes = new Dictionary<string, ProxyAddressPrefix>(8, StringComparer.OrdinalIgnoreCase);
			ProxyAddressPrefix.standardPrefixes.Add(ProxyAddressPrefix.Smtp.PrimaryPrefix, ProxyAddressPrefix.Smtp);
			ProxyAddressPrefix.standardPrefixes.Add(ProxyAddressPrefix.LegacyDN.PrimaryPrefix, ProxyAddressPrefix.LegacyDN);
			ProxyAddressPrefix.standardPrefixes.Add(ProxyAddressPrefix.X500.PrimaryPrefix, ProxyAddressPrefix.X500);
			ProxyAddressPrefix.standardPrefixes.Add(ProxyAddressPrefix.X400.PrimaryPrefix, ProxyAddressPrefix.X400);
			ProxyAddressPrefix.standardPrefixes.Add(ProxyAddressPrefix.MsMail.PrimaryPrefix, ProxyAddressPrefix.MsMail);
			ProxyAddressPrefix.standardPrefixes.Add(ProxyAddressPrefix.CcMail.PrimaryPrefix, ProxyAddressPrefix.CcMail);
			ProxyAddressPrefix.standardPrefixes.Add(ProxyAddressPrefix.Notes.PrimaryPrefix, ProxyAddressPrefix.Notes);
			ProxyAddressPrefix.standardPrefixes.Add(ProxyAddressPrefix.GroupWise.PrimaryPrefix, ProxyAddressPrefix.GroupWise);
			ProxyAddressPrefix.standardPrefixes.Add(ProxyAddressPrefix.UM.PrimaryPrefix, ProxyAddressPrefix.UM);
			ProxyAddressPrefix.standardPrefixes.Add(ProxyAddressPrefix.Meum.PrimaryPrefix, ProxyAddressPrefix.Meum);
			ProxyAddressPrefix.standardPrefixes.Add(ProxyAddressPrefix.Invalid.PrimaryPrefix, ProxyAddressPrefix.Invalid);
			ProxyAddressPrefix.standardPrefixes.Add(ProxyAddressPrefix.JRNL.primaryPrefix, ProxyAddressPrefix.JRNL);
		}

		public const int MaxLength = 9;

		public const int MaxAddressTypeLength = 64;

		public const string AllowedCharacters = "[^\u0080-￿]";

		internal const string SipPrefix = "sip:";

		private static readonly Regex asciiRegex = new Regex(string.Format("^{0}*$", "[^\u0080-￿]"), RegexOptions.Compiled);

		private readonly string primaryPrefix;

		private readonly string secondaryPrefix;

		private static readonly Dictionary<string, ProxyAddressPrefix> standardPrefixes;

		public static readonly ProxyAddressPrefix Smtp = new SmtpProxyAddressPrefix();

		public static readonly ProxyAddressPrefix LegacyDN = new CustomProxyAddressPrefix("EX", DataStrings.LegacyDNProxyAddressPrefixDisplayName);

		public static readonly ProxyAddressPrefix X500 = new CustomProxyAddressPrefix("X500");

		public static readonly ProxyAddressPrefix X400 = new X400ProxyAddressPrefix();

		public static readonly ProxyAddressPrefix MsMail = new CustomProxyAddressPrefix("MS", DataStrings.MsMailProxyAddressPrefixDisplayName);

		public static readonly ProxyAddressPrefix CcMail = new CustomProxyAddressPrefix("CCMAIL", DataStrings.CcMailProxyAddressPrefixDisplayName);

		public static readonly ProxyAddressPrefix Notes = new CustomProxyAddressPrefix("NOTES", DataStrings.NotesProxyAddressPrefixDisplayName);

		public static readonly ProxyAddressPrefix GroupWise = new CustomProxyAddressPrefix("GWISE", DataStrings.GroupWiseProxyAddressPrefixDisplayName);

		public static readonly ProxyAddressPrefix UM = new EumProxyAddressPrefix();

		public static readonly ProxyAddressPrefix ASUM = new CustomProxyAddressPrefix("ASUM", DataStrings.AirSyncProxyAddressPrefixDisplayName);

		public static readonly ProxyAddressPrefix Meum = new MeumProxyAddressPrefix();

		public static readonly ProxyAddressPrefix SIP = new CustomProxyAddressPrefix("SIP");

		public static readonly ProxyAddressPrefix Invalid = new CustomProxyAddressPrefix("INVALID");

		public static readonly ProxyAddressPrefix JRNL = new CustomProxyAddressPrefix("JRNL");
	}
}
