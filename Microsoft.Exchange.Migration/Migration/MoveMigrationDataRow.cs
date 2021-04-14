using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;

namespace Microsoft.Exchange.Migration
{
	internal class MoveMigrationDataRow : IMigrationDataRow
	{
		public MoveMigrationDataRow(int rowIndex, string emailAddress, MigrationBatchDirection jobDirection, CsvRow row, bool jobArchiveOnly)
		{
			this.rowIndex = rowIndex;
			this.emailAddress = emailAddress;
			this.jobDirection = jobDirection;
			this.jobArchiveOnly = jobArchiveOnly;
			this.InitializeFromRow(row);
		}

		public MigrationType MigrationType
		{
			get
			{
				switch (this.jobDirection)
				{
				case MigrationBatchDirection.Local:
					return MigrationType.ExchangeLocalMove;
				case MigrationBatchDirection.Onboarding:
				case MigrationBatchDirection.Offboarding:
					return MigrationType.ExchangeRemoteMove;
				}
				throw new MigrationDataCorruptionException(string.Format("Unknown batch type '{0}'", this.jobDirection));
			}
		}

		public MigrationUserRecipientType RecipientType
		{
			get
			{
				bool value;
				if (this.ArchiveOnly != null && this.PrimaryOnly != null)
				{
					value = this.ArchiveOnly.Value;
				}
				else
				{
					value = this.jobArchiveOnly;
				}
				switch (this.jobDirection)
				{
				case MigrationBatchDirection.Local:
					if (value)
					{
						return MigrationUserRecipientType.MailboxOrMailuser;
					}
					return MigrationUserRecipientType.Mailbox;
				case MigrationBatchDirection.Onboarding:
					return MigrationUserRecipientType.Mailuser;
				case MigrationBatchDirection.Offboarding:
					if (value)
					{
						return MigrationUserRecipientType.Mailuser;
					}
					return MigrationUserRecipientType.Mailbox;
				}
				throw new MigrationDataCorruptionException(string.Format("Unknown batch direction '{0}'", this.jobDirection));
			}
		}

		public string Identifier
		{
			get
			{
				return this.emailAddress;
			}
		}

		public string LocalMailboxIdentifier
		{
			get
			{
				return this.Identifier;
			}
		}

		public int CursorPosition
		{
			get
			{
				return this.rowIndex;
			}
		}

		public string TargetDatabase { get; private set; }

		public string TargetArchiveDatabase { get; private set; }

		public string TargetDeliveryDomain { get; private set; }

		public Unlimited<int>? BadItemLimit { get; private set; }

		public Unlimited<int>? LargeItemLimit { get; private set; }

		public bool? PrimaryOnly { get; private set; }

		public bool? ArchiveOnly { get; private set; }

		public bool SupportsRemoteIdentifier
		{
			get
			{
				return false;
			}
		}

		public string RemoteIdentifier
		{
			get
			{
				return null;
			}
		}

		private static string GetValueOrDefault(CsvRow row, string columnName, string defaultValue)
		{
			string text;
			if (!row.TryGetColumnValue(columnName, out text) || string.IsNullOrEmpty(text))
			{
				return defaultValue;
			}
			return text;
		}

		private static Unlimited<int>? GetUnlimitedValueOrDefault(CsvRow row, string columnName, Unlimited<int>? defaultValue)
		{
			string text;
			if (!row.TryGetColumnValue(columnName, out text) || string.IsNullOrEmpty(text))
			{
				return defaultValue;
			}
			return new Unlimited<int>?(Unlimited<int>.Parse(text));
		}

		private static bool GetBoolValue(CsvRow row, string columnName, bool defaultValue)
		{
			string value;
			if (!row.TryGetColumnValue(columnName, out value) || string.IsNullOrEmpty(value))
			{
				return defaultValue;
			}
			return bool.Parse(value);
		}

		private static TEnum? GetEnumValue<TEnum>(CsvRow row, string columnName, TEnum? defaultValue) where TEnum : struct
		{
			string value;
			if (!row.TryGetColumnValue(columnName, out value) || string.IsNullOrEmpty(value))
			{
				return defaultValue;
			}
			return new TEnum?((TEnum)((object)Enum.Parse(typeof(TEnum), value)));
		}

		private void InitializeFromRow(CsvRow row)
		{
			this.TargetDatabase = MoveMigrationDataRow.GetValueOrDefault(row, "TargetDatabase", null);
			this.TargetArchiveDatabase = MoveMigrationDataRow.GetValueOrDefault(row, "TargetArchiveDatabase", null);
			this.BadItemLimit = MoveMigrationDataRow.GetUnlimitedValueOrDefault(row, "BadItemLimit", null);
			this.LargeItemLimit = MoveMigrationDataRow.GetUnlimitedValueOrDefault(row, "LargeItemLimit", null);
			MigrationMailboxType? enumValue = MoveMigrationDataRow.GetEnumValue<MigrationMailboxType>(row, "MailboxType", null);
			if (enumValue != null)
			{
				this.PrimaryOnly = new bool?(enumValue.Value == MigrationMailboxType.PrimaryOnly);
				this.ArchiveOnly = new bool?(enumValue.Value == MigrationMailboxType.ArchiveOnly);
			}
		}

		private readonly string emailAddress;

		private readonly MigrationBatchDirection jobDirection;

		private readonly bool jobArchiveOnly;

		private readonly int rowIndex;
	}
}
