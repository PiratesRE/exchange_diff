using System;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data
{
	internal static class ConsumerIdentityHelper
	{
		public static bool IsConsumerDomain(SmtpDomain domainName)
		{
			return domainName != null && Globals.IsDatacenter && ConsumerIdentityHelper.IsConsumerMailbox("@" + domainName.Domain);
		}

		public static bool IsConsumerMailbox(string memberName)
		{
			return !string.IsNullOrEmpty(memberName) && Globals.IsDatacenter && ConsumerIdentityHelper.consumerDomainRegex.IsMatch(memberName);
		}

		public static bool IsConsumerMailbox(ADObjectId id)
		{
			if (id == null || id.DistinguishedName == null || !Globals.IsDatacenter)
			{
				return false;
			}
			Match match = ConsumerIdentityHelper.RDNRegex.Match(id.DistinguishedName);
			if (match.Success)
			{
				string[] array = id.DistinguishedName.Split(ConsumerIdentityHelper.OUSplitter, StringSplitOptions.None);
				return array.Length >= 2 && TemplateTenantConfiguration.IsTemplateTenantName(array[1]);
			}
			return false;
		}

		public static bool IsPuidBasedSecurityIdentifier(SecurityIdentifier sid)
		{
			return !(sid == null) && Globals.IsDatacenter && ConsumerIdentityHelper.SidRegex.Match(sid.ToString()).Success;
		}

		internal static bool IsPuidBasedLegacyExchangeDN(string legacyExchangeDN)
		{
			return legacyExchangeDN != null && Globals.IsDatacenter && ConsumerIdentityHelper.LegacyExchangeDNRegex.Match(legacyExchangeDN).Success;
		}

		internal static bool IsPuidBasedCanonicalName(string canonicalName)
		{
			return canonicalName != null && Globals.IsDatacenter && ConsumerIdentityHelper.CanonicalNameRegex.Match(canonicalName).Success;
		}

		public static bool IsMigratedConsumerMailbox(ADRawEntry userEntry)
		{
			return userEntry != null && Globals.IsDatacenter && ConsumerIdentityHelper.IsConsumerMailbox(userEntry.Id) && (PrimaryMailboxSourceType)userEntry[ADUserSchema.PrimaryMailboxSource] == PrimaryMailboxSourceType.Exo;
		}

		public static SecurityIdentifier GetSecurityIdentifierFromPuid(ulong puid)
		{
			StringBuilder stringBuilder = new StringBuilder();
			uint num = (uint)(puid >> 32);
			uint num2 = (uint)(puid << 32 >> 32);
			stringBuilder.AppendFormat("S-1-{0}-{1}-{2}", 2827L, num, num2);
			return new SecurityIdentifier(stringBuilder.ToString());
		}

		public static Guid GetExchangeGuidFromPuid(ulong puid)
		{
			uint a = (uint)(puid >> 32);
			ushort b = (ushort)(puid << 32 >> 48);
			ushort c = (ushort)puid;
			Guid result = new Guid(a, b, c, 0, 0, 0, 0, 0, 0, 0, 0);
			return result;
		}

		public static string GetLegacyExchangeDNFromPuid(ulong puid)
		{
			string arg = string.Format("{0:X16}", puid);
			return string.Format("{0}{1}", ConsumerIdentityHelper.LegacyExchangeDNPrefix, arg);
		}

		internal static string GetCommonNameFromPuid(ulong puid)
		{
			string arg = string.Format("{0:X16}", puid);
			return string.Format("{0}{1}", ConsumerIdentityHelper.CommonNamePrefix, arg);
		}

		public static string GetDistinguishedNameFromPuid(ulong puid)
		{
			ADObjectId organizationalUnit = ADSessionSettings.FromConsumerOrganization().CurrentOrganizationId.OrganizationalUnit;
			return organizationalUnit.GetChildId(ConsumerIdentityHelper.GetCommonNameFromPuid(puid)).DistinguishedName;
		}

		public static string GetExternalDirectoryObjectIdFromPuid(ulong puid)
		{
			return ConsumerIdentityHelper.GetExchangeGuidFromPuid(puid).ToString();
		}

		public static ADObjectId GetADObjectIdFromPuid(ulong puid)
		{
			Guid exchangeGuidFromPuid = ConsumerIdentityHelper.GetExchangeGuidFromPuid(puid);
			return new ADObjectId(ConsumerIdentityHelper.GetDistinguishedNameFromPuid(puid), exchangeGuidFromPuid);
		}

		public static ADObjectId GetADObjectIdFromSmtpAddress(SmtpAddress address)
		{
			ADObjectId organizationalUnitRoot = TemplateTenantConfiguration.GetLocalTemplateTenant().OrganizationalUnitRoot;
			return new ADObjectId(organizationalUnitRoot.GetChildId(address.ToString()).DistinguishedName);
		}

		public static bool TryGetPuidFromSecurityIdentifier(SecurityIdentifier sid, out ulong puid)
		{
			if (sid == null || !Globals.IsDatacenter)
			{
				puid = 0UL;
				return false;
			}
			string input = sid.ToString();
			Match match = ConsumerIdentityHelper.SidRegex.Match(input);
			if (!match.Success)
			{
				puid = 0UL;
				return false;
			}
			uint num = uint.Parse(match.Result("${hi}"));
			uint num2 = uint.Parse(match.Result("${lo}"));
			puid = ((ulong)num << 32) + (ulong)num2;
			return true;
		}

		public static bool TryGetPuidFromGuid(Guid guid, out ulong puid)
		{
			string input = string.Format("{0:X}", guid);
			Match match = ConsumerIdentityHelper.GuidRegex.Match(input);
			if (!match.Success || !Globals.IsDatacenter)
			{
				puid = 0UL;
				return false;
			}
			ulong num = (ulong)uint.Parse(match.Result("${a}"), NumberStyles.HexNumber);
			ulong num2 = (ulong)ushort.Parse(match.Result("${b}"), NumberStyles.HexNumber);
			ulong num3 = (ulong)ushort.Parse(match.Result("${c}"), NumberStyles.HexNumber);
			puid = (num << 32) + (num2 << 16) + num3;
			return true;
		}

		public static bool TryGetPuidFromLegacyExchangeDN(string legacyExchangeDN, out ulong puid)
		{
			if (!string.IsNullOrEmpty(legacyExchangeDN) && Globals.IsDatacenter)
			{
				Match match = ConsumerIdentityHelper.LegacyExchangeDNRegex.Match(legacyExchangeDN);
				if (match.Success)
				{
					puid = ulong.Parse(match.Result("${a}"), NumberStyles.HexNumber);
					return true;
				}
			}
			puid = 0UL;
			return false;
		}

		public static bool TryGetPuidFromCanonicalName(string canonicalName, out ulong puid)
		{
			if (!string.IsNullOrEmpty(canonicalName) && Globals.IsDatacenter)
			{
				Match match = ConsumerIdentityHelper.CanonicalNameRegex.Match(canonicalName);
				if (match.Success)
				{
					puid = ulong.Parse(match.Result("${a}"), NumberStyles.HexNumber);
					return true;
				}
			}
			puid = 0UL;
			return false;
		}

		public static bool TryGetPuidFromDN(string distinguishedName, out ulong puid)
		{
			if (!string.IsNullOrEmpty(distinguishedName) && Globals.IsDatacenter)
			{
				Match match = ConsumerIdentityHelper.RDNRegex.Match(distinguishedName);
				if (match.Success)
				{
					puid = ulong.Parse(match.Result("${a}"), NumberStyles.HexNumber);
					return true;
				}
			}
			puid = 0UL;
			return false;
		}

		public static bool TryGetPuidFromADObjectId(ADObjectId objectId, out ulong puid)
		{
			if (objectId != null && Globals.IsDatacenter)
			{
				if (!string.IsNullOrEmpty(objectId.DistinguishedName) && ConsumerIdentityHelper.TryGetPuidFromDN(objectId.DistinguishedName, out puid))
				{
					return true;
				}
				if (objectId.ObjectGuid != Guid.Empty && ConsumerIdentityHelper.TryGetPuidFromGuid(objectId.ObjectGuid, out puid))
				{
					return true;
				}
			}
			puid = 0UL;
			return false;
		}

		public static bool TryGetPuidByExternalDirectoryObjectId(string guidString, out ulong puid)
		{
			if (string.IsNullOrEmpty(guidString) || !Globals.IsDatacenter)
			{
				puid = 0UL;
				return false;
			}
			return ConsumerIdentityHelper.TryGetPuidFromGuid(new Guid(guidString), out puid);
		}

		public static bool TryGetDistinguishedNameFromPuidBasedCanonicalName(string canonicalName, out string distinguishedName)
		{
			distinguishedName = null;
			if (string.IsNullOrEmpty(canonicalName) || !Globals.IsDatacenter)
			{
				return false;
			}
			Match match = ConsumerIdentityHelper.CanonicalNameRegex.Match(canonicalName);
			if (!match.Success)
			{
				return false;
			}
			string canonicalName2 = match.Result("${cn}");
			string arg = match.Result("${a}");
			string arg2 = NativeHelpers.DistinguishedNameFromCanonicalName(canonicalName2);
			distinguishedName = string.Format("CN={0}{1},{2}", ConsumerIdentityHelper.CommonNamePrefix, arg, arg2);
			return true;
		}

		public static ulong ConvertPuidStringToPuidNumber(string puid)
		{
			return ulong.Parse(puid, NumberStyles.HexNumber);
		}

		public static string ConvertPuidNumberToPuidString(ulong puidNum)
		{
			return new NetID((long)puidNum).ToString();
		}

		private const long IdentifierAuthority = 2827L;

		private static readonly string LegacyExchangeDNPrefix = "/o=First Organization/ou=Exchange Administrative Group(FYDIBOHF23SPDLT)/cn=Recipients/cn=";

		public static readonly Regex LegacyExchangeDNRegex = new Regex("/o=First Organization/ou=Exchange Administrative Group\\(FYDIBOHF23SPDLT\\)/cn=Recipients/cn=(?<a>[0-9A-F]{16})$", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1.0));

		private static readonly Regex SidRegex = new Regex(string.Format("^S-1-{0}-(?<hi>\\d+)-(?<lo>\\d+)", 2827L), RegexOptions.Compiled, TimeSpan.FromSeconds(1.0));

		private static readonly Regex CanonicalNameRegex = new Regex("^(?<cn>.*/.*/.*\\.templateTenant)/puid-(?<a>[0-9A-F]{16})$", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1.0));

		public static readonly Regex GuidRegex = new Regex("^\\{0x(?<a>[0-9a-f]{8}),0x(?<b>[0-9a-f]{4}),0x(?<c>[0-9a-f]{4}),\\{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00\\}\\}", RegexOptions.Compiled, TimeSpan.FromSeconds(1.0));

		public static readonly string CommonNamePrefix = "puid-";

		private static readonly Regex RDNRegex = new Regex("^CN=puid-(?<a>[0-9A-F]{16})", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1.0));

		private static readonly string[] OUSplitter = new string[]
		{
			",OU=",
			",ou="
		};

		private static readonly Regex consumerDomainRegex = new Regex("^.*@(outlook\\.com|hotmail\\.com|live\\.com|outlook-int\\.com|hotmail-int\\.com|live-int\\.com)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	}
}
