using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class OrganizationSummaryEntry : IEquatable<OrganizationSummaryEntry>, IComparable<OrganizationSummaryEntry>, ICloneable
	{
		internal static bool IsValidKeyForCurrentRelease(string key)
		{
			return key.Equals(OrganizationSummaryEntry.SummaryInfoUpdateDate) || Array.LastIndexOf<string>(OrganizationSummaryEntry.SummaryInfoKeys, key) >= 0;
		}

		internal static bool IsValidKeyForCurrentAndPreviousRelease(string key)
		{
			return key.Equals(OrganizationSummaryEntry.SummaryInfoUpdateDate) || Array.LastIndexOf<string>(OrganizationSummaryEntry.SummaryInfoKeysInCurrentAndPreviousRelease, key) >= 0;
		}

		public int NumberOfFields
		{
			get
			{
				return this.numberOfFields;
			}
		}

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public bool HasError
		{
			get
			{
				return this.hasError;
			}
		}

		public OrganizationSummaryEntry(string key, string value, bool hasError)
		{
			if (!OrganizationSummaryEntry.ValidateKey(key))
			{
				throw new ArgumentException(DataStrings.InvalidOrganizationSummaryEntryKey(key));
			}
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException(DataStrings.InvalidOrganizationSummaryEntryValue(value));
			}
			this.numberOfFields = 3;
			this.key = key;
			this.value = value;
			this.hasError = hasError;
		}

		public OrganizationSummaryEntry(string s)
		{
			OrganizationSummaryEntry organizationSummaryEntry;
			if (!OrganizationSummaryEntry.TryParse(s, out organizationSummaryEntry))
			{
				throw new FormatException(DataStrings.InvalidOrganizationSummaryEntryFormat(s));
			}
			this.numberOfFields = s.Split(new char[]
			{
				OrganizationSummaryEntry.fieldSeparator
			}).Length;
			this.key = organizationSummaryEntry.Key;
			this.value = organizationSummaryEntry.Value;
			this.hasError = organizationSummaryEntry.HasError;
		}

		private static bool ValidateKey(string key)
		{
			return !string.IsNullOrEmpty(key);
		}

		public static OrganizationSummaryEntry Parse(string s)
		{
			return new OrganizationSummaryEntry(s);
		}

		private OrganizationSummaryEntry(OrganizationSummaryEntry from)
		{
			this.key = from.Key;
			this.value = from.Value;
			this.hasError = from.HasError;
		}

		public static bool operator ==(OrganizationSummaryEntry value1, OrganizationSummaryEntry value2)
		{
			if (value1 != null)
			{
				return value1.Equals(value2);
			}
			return value2 == null;
		}

		public static bool operator !=(OrganizationSummaryEntry value1, OrganizationSummaryEntry value2)
		{
			return !(value1 == value2);
		}

		public bool Equals(OrganizationSummaryEntry entry)
		{
			return entry != null && this.Key == entry.Key && this.Value == entry.Value && this.HasError == entry.HasError;
		}

		public override bool Equals(object entry)
		{
			return this.Equals(entry as OrganizationSummaryEntry);
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.Key,
				OrganizationSummaryEntry.fieldSeparator,
				this.Value,
				OrganizationSummaryEntry.fieldSeparator,
				this.HasError
			});
		}

		public override int GetHashCode()
		{
			return this.Key.GetHashCode() + this.Value.GetHashCode() + this.HasError.GetHashCode();
		}

		public static bool TryParse(string s, out OrganizationSummaryEntry entry)
		{
			entry = null;
			if (!string.IsNullOrEmpty(s))
			{
				string[] array = s.Split(new char[]
				{
					OrganizationSummaryEntry.fieldSeparator
				});
				if (array != null && array.Length >= 2)
				{
					string text = array[0];
					string text2 = array[1];
					bool flag = false;
					if (OrganizationSummaryEntry.ValidateKey(text) && !string.IsNullOrEmpty(text2) && (array.Length == 2 || bool.TryParse(array[2], out flag)))
					{
						entry = new OrganizationSummaryEntry(text, text2, flag);
						return true;
					}
				}
			}
			return false;
		}

		public int CompareTo(OrganizationSummaryEntry entry)
		{
			if (entry == null)
			{
				return 1;
			}
			return this.Key.CompareTo(entry.Key);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public OrganizationSummaryEntry Clone()
		{
			return new OrganizationSummaryEntry(this);
		}

		internal const int NumberOfFieldsConst = 3;

		internal static char fieldSeparator = ',';

		internal static readonly string[] SummaryInfoKeys = new string[]
		{
			"TotalDatabases",
			"TotalDatabasesCopy",
			"TotalDatabasesCopyUnhealthy",
			"TotalCALMailboxes",
			"StandardCALs",
			"EnterpriseCALs",
			"TotalExchangeServers",
			"Total2009ExchangeServers",
			"Total2007ExchangeServers",
			"Total2003ExchangeServers",
			"TotalMailboxes",
			"TotalDistributionGroups",
			"TotalDynamicDistributionGroups",
			"TotalMailContacts",
			"TotalMailUsers",
			"TotalLegacyMailbox",
			"TotalMessagingRecordManagementUser",
			"TotalJounalingUser",
			"TotalOWAUser",
			"TotalActiveSyncUser",
			"TotalUnifiedMessagingUser",
			"TotalMAPIUser",
			"TotalPOP3User",
			"TotalIMAP4User",
			"TotalMailboxServers",
			"TotalUMServers",
			"TotalTransportServers",
			"TotalClientAccessServers",
			"TotalUnlicensedExchangeServers",
			"TotalRecipients"
		};

		internal static readonly string[] SummaryInfoKeysInCurrentAndPreviousRelease = new string[]
		{
			"TotalDatabases",
			"TotalDatabasesCopy",
			"TotalDatabasesCopyUnhealthy",
			"TotalCALMailboxes",
			"StandardCALs",
			"EnterpriseCALs",
			"TotalExchangeServers",
			"Total2009ExchangeServers",
			"Total2007ExchangeServers",
			"Total2003ExchangeServers",
			"TotalMailboxes",
			"TotalDistributionGroups",
			"TotalDynamicDistributionGroups",
			"TotalMailContacts",
			"TotalMailUsers",
			"TotalLegacyMailbox",
			"TotalMessagingRecordManagementUser",
			"TotalJounalingUser",
			"TotalOWAUser",
			"TotalActiveSyncUser",
			"TotalUnifiedMessagingUser",
			"TotalMAPIUser",
			"TotalPOP3User",
			"TotalIMAP4User",
			"TotalMailboxServers",
			"TotalUMServers",
			"TotalTransportServers",
			"TotalClientAccessServers",
			"TotalUnlicensedExchangeServers",
			"TotalRecipients"
		};

		internal static readonly string SummaryInfoUpdateDate = "UpdateDate";

		private int numberOfFields;

		private string key;

		private string value;

		private bool hasError;
	}
}
