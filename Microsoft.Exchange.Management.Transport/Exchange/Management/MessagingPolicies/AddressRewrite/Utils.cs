using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MessagingPolicies.AddressRewrite
{
	internal static class Utils
	{
		public static ADObjectId RootId
		{
			get
			{
				return new ADObjectId("OU=MSExchangeGateway");
			}
		}

		internal static ADObjectId GetRelativeDnForEntryType(Utils.EntryType entryType, IConfigurationSession session)
		{
			string unescapedCommonName = (entryType == Utils.EntryType.SmtpAddress) ? Utils.EmailEntriesRDn : Utils.DomainEntriesRDn;
			ADObjectId childId = Utils.RootId.GetChildId(Utils.AddressRewriteConfigRDn);
			return childId.GetChildId(unescapedCommonName);
		}

		internal static Utils.EntryType ValidateAddressEntrySyntax(string address)
		{
			try
			{
				RoutingAddress.Parse(address);
				return Utils.EntryType.SmtpAddress;
			}
			catch (FormatException)
			{
			}
			try
			{
				SmtpDomain.Parse(address);
				return Utils.EntryType.Domain;
			}
			catch (FormatException)
			{
			}
			if (address == "*")
			{
				return Utils.EntryType.WildCardedDomain;
			}
			if (address.StartsWith("*.") && address.Length > 2)
			{
				SmtpDomain.Parse(address.Substring(2, address.Length - 2));
				return Utils.EntryType.WildCardedDomain;
			}
			throw new FormatException(Strings.AddressRewriteUnrecognizedAddress);
		}

		internal static void CheckConflicts(ADObjectId baseContainer, bool outboundOnly, string internalAddress, string externalAddress, IConfigurationSession session, Guid? skipGuid)
		{
			QueryFilter filter;
			if (outboundOnly)
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, AddressRewriteEntrySchema.InternalAddress, internalAddress);
			}
			else
			{
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, AddressRewriteEntrySchema.InternalAddress, internalAddress);
				QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, AddressRewriteEntrySchema.ExternalAddress, externalAddress);
				filter = new OrFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter2
				});
			}
			AddressRewriteEntry[] array = session.Find<AddressRewriteEntry>(baseContainer, QueryScope.OneLevel, filter, null, 2);
			if (array == null)
			{
				return;
			}
			AddressRewriteEntry[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				AddressRewriteEntry addressRewriteEntry = array2[i];
				if (skipGuid == null || !addressRewriteEntry.Guid.Equals(skipGuid))
				{
					if (addressRewriteEntry.InternalAddress.Equals(internalAddress, StringComparison.OrdinalIgnoreCase))
					{
						throw new ArgumentException(Strings.AddressRewriteInternalAddressExists, null);
					}
					throw new ArgumentException(Strings.AddressRewriteExternalAddressExists, null);
				}
				else
				{
					i++;
				}
			}
		}

		internal static ADObjectId ValidateEntryAddresses(string internalAddress, string externalAddress, bool outboundOnly, MultiValuedProperty<string> exceptionList, IConfigurationSession session, Guid? skipGuid)
		{
			if (!outboundOnly && exceptionList != null && exceptionList.Count != 0)
			{
				throw new ArgumentException(Strings.AddressRewriteExceptionListDisallowed, null);
			}
			Utils.EntryType entryType = Utils.ValidateSyntax(internalAddress, externalAddress, outboundOnly);
			outboundOnly = (outboundOnly || entryType == Utils.EntryType.WildCardedDomain);
			ADObjectId relativeDnForEntryType = Utils.GetRelativeDnForEntryType(entryType, session);
			Utils.CheckConflicts(relativeDnForEntryType, outboundOnly, internalAddress, externalAddress, session, skipGuid);
			return relativeDnForEntryType;
		}

		internal static Utils.EntryType ValidateSyntax(string internalAddress, string externalAddress, bool outboundOnly)
		{
			Utils.EntryType entryType = Utils.ValidateAddressEntrySyntax(internalAddress);
			Utils.EntryType entryType2 = Utils.ValidateAddressEntrySyntax(externalAddress);
			if (entryType == Utils.EntryType.SmtpAddress && entryType2 == Utils.EntryType.SmtpAddress)
			{
				return Utils.EntryType.SmtpAddress;
			}
			if ((entryType != Utils.EntryType.Domain && entryType != Utils.EntryType.WildCardedDomain) || entryType2 != Utils.EntryType.Domain)
			{
				throw new ArgumentException(Strings.AddressRewriteInvalidMapping, null);
			}
			if (entryType == Utils.EntryType.WildCardedDomain && !outboundOnly)
			{
				throw new ArgumentException(Strings.AddressRewriteWildcardWarning, null);
			}
			return entryType;
		}

		private static string DomainEntriesRDn = "Domain Entries";

		private static string EmailEntriesRDn = "Email Entries";

		private static string AddressRewriteConfigRDn = "Address Rewrite Configuration";

		internal enum EntryType
		{
			SmtpAddress,
			Domain,
			WildCardedDomain
		}
	}
}
