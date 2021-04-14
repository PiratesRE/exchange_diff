using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class MservRecord
	{
		public string Key { get; private set; }

		public byte ResourceId { get; private set; }

		public byte Flags { get; private set; }

		public byte UpdatedFlagsMask { get; private set; }

		public string SourceKey { get; private set; }

		public string Value { get; private set; }

		public MservValueFormat ValueFormat { get; private set; }

		public string ExoForestFqdn { get; private set; }

		public string ExoDatabaseId { get; private set; }

		public string HotmailClusterIp { get; private set; }

		public string HotmailDGroupId { get; private set; }

		public MservRecord(string key, byte resourceId, string value, string sourceKey, byte flags)
		{
			this.Key = key;
			this.ResourceId = resourceId;
			this.SourceKey = sourceKey;
			this.Flags = flags;
			if (value == null)
			{
				this.ValueFormat = (string.IsNullOrEmpty(sourceKey) ? MservValueFormat.Undefined : MservValueFormat.Alias);
				return;
			}
			Match match = MservRecord.MservExoValueRegex.Match(value);
			if (match.Success)
			{
				this.ValueFormat = MservValueFormat.Exo;
				this.ExoForestFqdn = match.Result("${resourceForestFqdn}");
				this.ExoDatabaseId = match.Result("${guidString}");
				this.Value = value;
				return;
			}
			match = MservRecord.MservHotmailValueRegex.Match(value);
			if (match.Success)
			{
				this.ValueFormat = MservValueFormat.Hotmail;
				this.HotmailClusterIp = match.Result("${clusterIp}");
				this.HotmailDGroupId = match.Result("${dGroup}");
				this.Value = value;
				return;
			}
			this.ValueFormat = MservValueFormat.Unknown;
			this.Value = value;
		}

		public MservRecord GetUpdatedExoRecord(string exoForestFqdn, string exoDatabaseId)
		{
			if (this.ValueFormat == MservValueFormat.Hotmail)
			{
				throw new MservOperationException(DirectoryStrings.RecordValueFormatChange(this.Key, this.Value));
			}
			string value = string.Format("{0}{1} {2}", "EXO:", exoForestFqdn, exoDatabaseId);
			return new MservRecord(this.Key, this.ResourceId, value, this.SourceKey, this.Flags);
		}

		public MservRecord GetUpdatedHotmailRecord(string hotmailClusterIp, string hotmailDGroupId)
		{
			if (this.ValueFormat == MservValueFormat.Exo)
			{
				throw new MservOperationException(DirectoryStrings.RecordValueFormatChange(this.Key, this.Value));
			}
			string value = string.Format("{0} {1}", hotmailClusterIp, hotmailDGroupId);
			return new MservRecord(this.Key, this.ResourceId, value, this.SourceKey, this.Flags)
			{
				ValueFormat = MservValueFormat.Hotmail,
				HotmailClusterIp = hotmailClusterIp,
				HotmailDGroupId = hotmailDGroupId
			};
		}

		public MservRecord GetUpdatedRecord(byte newResourceId)
		{
			return new MservRecord(this.Key, newResourceId, this.Value, this.SourceKey, this.Flags);
		}

		public MservRecord GetUpdatedRecordFlag(bool flagValue, byte mask)
		{
			byte flags = flagValue ? (this.Flags | mask) : (this.Flags & ~mask);
			MservRecord mservRecord = new MservRecord(this.Key, this.ResourceId, this.Value, this.SourceKey, flags);
			MservRecord mservRecord2 = mservRecord;
			mservRecord2.UpdatedFlagsMask |= mask;
			return mservRecord;
		}

		public bool IsEmpty
		{
			get
			{
				switch (this.ValueFormat)
				{
				case MservValueFormat.Undefined:
					return true;
				case MservValueFormat.Exo:
				case MservValueFormat.Hotmail:
				case MservValueFormat.Unknown:
					return string.IsNullOrWhiteSpace(this.Value);
				case MservValueFormat.Alias:
					return string.IsNullOrEmpty(this.SourceKey);
				default:
					throw new ArgumentException("ValueFormat");
				}
			}
		}

		public bool IsXmr
		{
			get
			{
				return this.HotmailClusterIp == "65.54.241.216" && this.HotmailDGroupId == "51999";
			}
		}

		public override string ToString()
		{
			return string.Format("Key:{0}, ResourceId:{1}, Value:{2}, SourceKey:{3}", new object[]
			{
				this.Key,
				this.ResourceId,
				this.Value,
				this.SourceKey
			});
		}

		public static string KeyFromPuid(ulong puid)
		{
			return string.Format("({0:X16})", puid);
		}

		public bool TryGetPuid(out ulong puid)
		{
			puid = 0UL;
			return this.Key.StartsWith("(") && this.Key.EndsWith(")") && ulong.TryParse(this.Key.Substring(1, this.Key.Length - 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out puid);
		}

		public bool SameRecord(MservRecord record)
		{
			return record != null && string.Equals(this.Key, record.Key, StringComparison.OrdinalIgnoreCase) && this.ResourceId == record.ResourceId;
		}

		private const string ExoPrefix = "EXO:";

		private static readonly Regex MservExoValueRegex = new Regex("EXO:(?<resourceForestFqdn>\\S+)\\s+(?<guidString>\\S+)", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1.0));

		private static readonly Regex MservHotmailValueRegex = new Regex("(?<clusterIp>\\S+)\\s+(?<dGroup>\\S+)", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1.0));
	}
}
