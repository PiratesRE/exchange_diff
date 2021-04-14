using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class O12TimeZoneFormatter
	{
		public static byte[] GetTimeZoneBlob(ExTimeZone timeZone)
		{
			if (timeZone == null)
			{
				throw new ArgumentNullException("timeZone");
			}
			if (timeZone == ExTimeZone.UnspecifiedTimeZone)
			{
				throw new ArgumentException("timeZone should not be UnspecifiedTimeZone");
			}
			string keyName = timeZone.IsCustomTimeZone ? timeZone.AlternativeId : timeZone.Id;
			return O12TimeZoneFormatter.GenerateBlobFromRuleGroups(keyName, timeZone.TimeZoneInformation.Groups);
		}

		public static byte[] GetTimeZoneBlob(ExTimeZone timeZone, ExDateTime time)
		{
			if (timeZone == null)
			{
				throw new ArgumentNullException("timeZone");
			}
			if (timeZone == ExTimeZone.UnspecifiedTimeZone)
			{
				throw new ArgumentException("timeZone should not be UnspecifiedTimeZone");
			}
			DateTime universalTime = time.UniversalTime;
			ExTimeZoneRule ruleForUtcTime = timeZone.TimeZoneInformation.GetRuleForUtcTime(universalTime);
			if (ruleForUtcTime != null && ruleForUtcTime.RuleGroup != null)
			{
				List<ExTimeZoneRuleGroup> list = new List<ExTimeZoneRuleGroup>();
				list.Add(ruleForUtcTime.RuleGroup);
				return O12TimeZoneFormatter.GenerateBlobFromRuleGroups(timeZone.AlternativeId, list);
			}
			throw new ArgumentException("no time zone rule found for specified time: " + time.ToString());
		}

		public static bool TryParseTimeZoneBlob(byte[] bytes, string defaultDisplayName, out ExTimeZone timeZone)
		{
			timeZone = null;
			if (bytes == null || defaultDisplayName == null)
			{
				ExTraceGlobals.StorageTracer.TraceError(0L, "Time zone blob or default display name is null.");
				return false;
			}
			O12TimeZoneFormatter.ExchangeTimeZoneHeader exchangeTimeZoneHeader;
			if (!O12TimeZoneFormatter.ExchangeTimeZoneHeader.TryParse(bytes, out exchangeTimeZoneHeader))
			{
				ExTraceGlobals.StorageTracer.TraceError(0L, "Failed to parse time zone blob header. Invalid time zone found.");
				return false;
			}
			if (exchangeTimeZoneHeader.MajorVersion != 2 || exchangeTimeZoneHeader.MinVersion != 1)
			{
				ExTraceGlobals.StorageTracer.TraceError<byte, byte>(0L, "TimeZone blob version mismatch. Version read is Major: {0} Minor: {1}", exchangeTimeZoneHeader.MajorVersion, exchangeTimeZoneHeader.MinVersion);
			}
			if (exchangeTimeZoneHeader.RuleCount == 0)
			{
				ExTraceGlobals.StorageTracer.TraceError(0L, "Invalid time zone found. No rules in time zone blob");
				return false;
			}
			O12TimeZoneFormatter.ExchangeTimeZoneRule? exchangeTimeZoneRule = null;
			List<RegistryTimeZoneRule> list = new List<RegistryTimeZoneRule>((int)exchangeTimeZoneHeader.RuleCount);
			int num = exchangeTimeZoneHeader.GetSize();
			for (int i = 0; i < (int)exchangeTimeZoneHeader.RuleCount; i++)
			{
				if (bytes.Length < num + O12TimeZoneFormatter.ExchangeTimeZoneRule.Size)
				{
					ExTraceGlobals.StorageTracer.TraceError(0L, "Incomplete time zone blob found");
					return false;
				}
				O12TimeZoneFormatter.ExchangeTimeZoneRule value;
				if (!O12TimeZoneFormatter.ExchangeTimeZoneRule.TryParse(new ArraySegment<byte>(bytes, num, O12TimeZoneFormatter.ExchangeTimeZoneRule.Size), out value))
				{
					ExTraceGlobals.StorageTracer.TraceError(0L, "Invalid time zone found. Incomplete rules in time zone blob");
					return false;
				}
				if (value.MajorVersion != 2 || value.MinVersion != 1)
				{
					ExTraceGlobals.StorageTracer.TraceError(0L, string.Concat(new object[]
					{
						"Time zone rule version mismatch. Version read is Major: ",
						value.MajorVersion,
						" Minor: ",
						value.MinVersion
					}));
				}
				if (exchangeTimeZoneRule == null || (value.Flags & 2) != 0)
				{
					exchangeTimeZoneRule = new O12TimeZoneFormatter.ExchangeTimeZoneRule?(value);
				}
				int num2 = Math.Max(DateTime.MinValue.Year, Math.Min((int)value.Start.Year, DateTime.MaxValue.Year));
				if (num2 != (int)value.Start.Year)
				{
					ExTraceGlobals.StorageTracer.TraceError(0L, "Start year of rule in blob is out of range. Forced to valid range. Value in blob is " + value.Start.Year);
				}
				list.Add(new RegistryTimeZoneRule(num2, value.RegTimeZoneInfo));
				num += O12TimeZoneFormatter.ExchangeTimeZoneRule.Size;
			}
			ExTraceGlobals.StorageTracer.TraceInformation(0, 0L, "Effective rule is from year " + exchangeTimeZoneRule.Value.Start.Year);
			string displayName = defaultDisplayName;
			ExTimeZone exTimeZone;
			if (ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(exchangeTimeZoneHeader.KeyName, out exTimeZone))
			{
				displayName = exTimeZone.LocalizableDisplayName;
			}
			bool result;
			try
			{
				timeZone = TimeZoneHelper.CreateCustomExTimeZoneFromRegRules(exchangeTimeZoneRule.Value.RegTimeZoneInfo, exchangeTimeZoneHeader.KeyName, displayName, list);
				result = true;
			}
			catch (ArgumentException ex)
			{
				ExTraceGlobals.StorageTracer.TraceError(0L, "Invalid time zone found. Inner message is " + ex.Message);
				result = false;
			}
			return result;
		}

		public static bool TryParseTruncatedTimeZoneBlob(byte[] bytes, out ExTimeZone timeZone)
		{
			timeZone = null;
			if (bytes == null)
			{
				ExTraceGlobals.StorageTracer.TraceError(0L, "Time zone blob is null.");
				return false;
			}
			O12TimeZoneFormatter.ExchangeTimeZoneHeader exchangeTimeZoneHeader;
			if (!O12TimeZoneFormatter.ExchangeTimeZoneHeader.TryParse(bytes, out exchangeTimeZoneHeader))
			{
				ExTraceGlobals.StorageTracer.TraceError(0L, "Failed to parse time zone blob header. Invalid time zone found.");
				return false;
			}
			if (exchangeTimeZoneHeader.MajorVersion != 2 || exchangeTimeZoneHeader.MinVersion != 1)
			{
				ExTraceGlobals.StorageTracer.TraceError<byte, byte>(0L, "TimeZone blob version mismatch. Version read is Major: {0} Minor: {1}", exchangeTimeZoneHeader.MajorVersion, exchangeTimeZoneHeader.MinVersion);
			}
			ExTimeZone exTimeZone;
			if (ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(exchangeTimeZoneHeader.KeyName, out exTimeZone))
			{
				timeZone = exTimeZone;
				return true;
			}
			return false;
		}

		internal static bool CompareBlob(byte[] left, byte[] right, bool compareKeyName)
		{
			if (object.Equals(left, right))
			{
				return true;
			}
			if (left == null || right == null)
			{
				return false;
			}
			bool result = false;
			if (compareKeyName)
			{
				result = O12TimeZoneFormatter.ByteArrayEqual(left, 0, right, 0);
			}
			else
			{
				int num = 2;
				if (num + 1 < left.Length && num + 1 < right.Length)
				{
					int num2 = num + (int)left[num] + ((int)left[num + 1] << 8);
					int num3 = num + (int)right[num] + ((int)right[num + 1] << 8);
					if (num2 < left.Length && num3 < right.Length)
					{
						result = O12TimeZoneFormatter.ByteArrayEqual(left, num2, right, num3);
					}
				}
			}
			return result;
		}

		private static bool ByteArrayEqual(byte[] left, int leftStartIndex, byte[] right, int rightStartIndex)
		{
			bool result = false;
			if (left.Length - leftStartIndex == right.Length - rightStartIndex)
			{
				result = true;
				for (int i = 0; i < left.Length - leftStartIndex; i++)
				{
					if (left[i + leftStartIndex] != right[i + rightStartIndex])
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		private static byte[] GenerateBlobFromRuleGroups(string keyName, IList<ExTimeZoneRuleGroup> groups)
		{
			O12TimeZoneFormatter.ExchangeTimeZoneHeader exchangeTimeZoneHeader = new O12TimeZoneFormatter.ExchangeTimeZoneHeader(keyName, (ushort)groups.Count);
			int size = exchangeTimeZoneHeader.GetSize();
			int num = size + O12TimeZoneFormatter.ExchangeTimeZoneRule.Size * groups.Count;
			byte[] array = new byte[num];
			int num2 = exchangeTimeZoneHeader.ToBytes(new ArraySegment<byte>(array, 0, size));
			foreach (ExTimeZoneRuleGroup exTimeZoneRuleGroup in groups)
			{
				O12TimeZoneFormatter.TzRuleFlags flag = O12TimeZoneFormatter.TzRuleFlags.None;
				if (groups.Count == 1 || (exTimeZoneRuleGroup.EffectiveUtcStart <= DateTime.UtcNow && exTimeZoneRuleGroup.EffectiveUtcEnd > DateTime.UtcNow))
				{
					flag = O12TimeZoneFormatter.TzRuleFlags.EffectiveTimeZone;
				}
				int year;
				if (exTimeZoneRuleGroup.EffectiveUtcStart != DateTime.MinValue)
				{
					ExTimeZoneRule ruleForUtcTime = exTimeZoneRuleGroup.GetRuleForUtcTime(exTimeZoneRuleGroup.EffectiveUtcStart);
					year = ruleForUtcTime.FromUtc(exTimeZoneRuleGroup.EffectiveUtcStart).Year;
				}
				else
				{
					year = exTimeZoneRuleGroup.EffectiveUtcStart.Year;
				}
				REG_TIMEZONE_INFO regInfo = TimeZoneHelper.RegTimeZoneInfoFromExTimeZoneRuleGroup(exTimeZoneRuleGroup);
				O12TimeZoneFormatter.ExchangeTimeZoneRule exchangeTimeZoneRule = new O12TimeZoneFormatter.ExchangeTimeZoneRule(flag, (ushort)year, regInfo);
				exchangeTimeZoneRule.ToBytes(new ArraySegment<byte>(array, num2, O12TimeZoneFormatter.ExchangeTimeZoneRule.Size));
				num2 += O12TimeZoneFormatter.ExchangeTimeZoneRule.Size;
			}
			return array;
		}

		private const byte TzBinVersionMajor = 2;

		private const byte TzBinVersionMinor = 1;

		[Flags]
		private enum TzDefinitionFlags
		{
			ValidGuid = 1,
			ValidName = 2
		}

		[Flags]
		private enum TzRuleFlags
		{
			None = 0,
			CurrentTimeZone = 1,
			EffectiveTimeZone = 2
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct ExchangeTimeZoneHeader
		{
			public ExchangeTimeZoneHeader(string keyName, ushort ruleCount)
			{
				if (keyName == null)
				{
					throw new ArgumentNullException("keyName");
				}
				if (ruleCount == 0)
				{
					throw new ArgumentOutOfRangeException("ruleCount", "rule count can not be zero.");
				}
				this.MajorVersion = 2;
				this.MinVersion = 1;
				this.flags = 2;
				this.KeyName = keyName;
				this.KeyNameLength = (ushort)keyName.Length;
				this.RuleCount = ruleCount;
				this.headerSize = 6 + this.KeyNameLength * 2;
			}

			public static bool TryParse(byte[] source, out O12TimeZoneFormatter.ExchangeTimeZoneHeader header)
			{
				header = default(O12TimeZoneFormatter.ExchangeTimeZoneHeader);
				if (source.Length < O12TimeZoneFormatter.ExchangeTimeZoneHeader.KeyNameOffset)
				{
					return false;
				}
				header.MajorVersion = source[O12TimeZoneFormatter.ExchangeTimeZoneHeader.MajorVersionOffset];
				header.MinVersion = source[O12TimeZoneFormatter.ExchangeTimeZoneHeader.MinVersionOffset];
				header.headerSize = BitConverter.ToUInt16(source, O12TimeZoneFormatter.ExchangeTimeZoneHeader.HeaderSizeOffset);
				header.flags = BitConverter.ToUInt16(source, O12TimeZoneFormatter.ExchangeTimeZoneHeader.FlagsOffset);
				header.KeyNameLength = BitConverter.ToUInt16(source, O12TimeZoneFormatter.ExchangeTimeZoneHeader.KeyNameLengthOffset);
				if (source.Length < O12TimeZoneFormatter.ExchangeTimeZoneHeader.KeyNameOffset + (int)(header.KeyNameLength * 2) + 2)
				{
					return false;
				}
				header.KeyName = Encoding.Unicode.GetString(source, O12TimeZoneFormatter.ExchangeTimeZoneHeader.KeyNameOffset, (int)(header.KeyNameLength * 2));
				header.RuleCount = BitConverter.ToUInt16(source, O12TimeZoneFormatter.ExchangeTimeZoneHeader.KeyNameOffset + (int)(header.KeyNameLength * 2));
				return true;
			}

			public int ToBytes(ArraySegment<byte> target)
			{
				int size = this.GetSize();
				if (target.Count < size)
				{
					throw new ArgumentOutOfRangeException();
				}
				ExBitConverter.Write((short)this.MajorVersion, target.Array, target.Offset + O12TimeZoneFormatter.ExchangeTimeZoneHeader.MajorVersionOffset);
				ExBitConverter.Write((short)this.MinVersion, target.Array, target.Offset + O12TimeZoneFormatter.ExchangeTimeZoneHeader.MinVersionOffset);
				ExBitConverter.Write(this.headerSize, target.Array, target.Offset + O12TimeZoneFormatter.ExchangeTimeZoneHeader.HeaderSizeOffset);
				ExBitConverter.Write(this.flags, target.Array, target.Offset + O12TimeZoneFormatter.ExchangeTimeZoneHeader.FlagsOffset);
				ExBitConverter.Write(this.KeyNameLength, target.Array, target.Offset + O12TimeZoneFormatter.ExchangeTimeZoneHeader.KeyNameLengthOffset);
				ExBitConverter.Write(this.KeyName, true, target.Array, target.Offset + O12TimeZoneFormatter.ExchangeTimeZoneHeader.KeyNameOffset);
				ExBitConverter.Write(this.RuleCount, target.Array, target.Offset + O12TimeZoneFormatter.ExchangeTimeZoneHeader.KeyNameOffset + (int)(this.KeyNameLength * 2));
				return size;
			}

			public int GetSize()
			{
				return (int)(4 + this.headerSize);
			}

			private static readonly int MajorVersionOffset = (int)Marshal.OffsetOf(typeof(O12TimeZoneFormatter.ExchangeTimeZoneHeader), "MajorVersion");

			private static readonly int MinVersionOffset = (int)Marshal.OffsetOf(typeof(O12TimeZoneFormatter.ExchangeTimeZoneHeader), "MinVersion");

			private static readonly int HeaderSizeOffset = (int)Marshal.OffsetOf(typeof(O12TimeZoneFormatter.ExchangeTimeZoneHeader), "headerSize");

			private static readonly int FlagsOffset = (int)Marshal.OffsetOf(typeof(O12TimeZoneFormatter.ExchangeTimeZoneHeader), "flags");

			private static readonly int KeyNameLengthOffset = (int)Marshal.OffsetOf(typeof(O12TimeZoneFormatter.ExchangeTimeZoneHeader), "KeyNameLength");

			private static readonly int KeyNameOffset = (int)Marshal.OffsetOf(typeof(O12TimeZoneFormatter.ExchangeTimeZoneHeader), "KeyName");

			public byte MajorVersion;

			public byte MinVersion;

			private ushort headerSize;

			private ushort flags;

			public ushort KeyNameLength;

			public string KeyName;

			public ushort RuleCount;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct ExchangeTimeZoneRule
		{
			public ExchangeTimeZoneRule(O12TimeZoneFormatter.TzRuleFlags flag, ushort startYear, REG_TIMEZONE_INFO regInfo)
			{
				this.MajorVersion = 2;
				this.MinVersion = 1;
				this.ruleSize = (ushort)(O12TimeZoneFormatter.ExchangeTimeZoneRule.Size - 4);
				this.Flags = (ushort)flag;
				this.Start = default(NativeMethods.SystemTime);
				this.Start.Year = startYear;
				this.Start.Month = 1;
				this.Start.Day = 1;
				this.RegTimeZoneInfo = regInfo;
			}

			public static bool TryParse(ArraySegment<byte> source, out O12TimeZoneFormatter.ExchangeTimeZoneRule rule)
			{
				rule = default(O12TimeZoneFormatter.ExchangeTimeZoneRule);
				if (source.Count < O12TimeZoneFormatter.ExchangeTimeZoneRule.Size)
				{
					return false;
				}
				rule.MajorVersion = source.Array[source.Offset + O12TimeZoneFormatter.ExchangeTimeZoneRule.MajorVersionOffset];
				rule.MinVersion = source.Array[source.Offset + O12TimeZoneFormatter.ExchangeTimeZoneRule.MinVersionOffset];
				rule.ruleSize = BitConverter.ToUInt16(source.Array, source.Offset + O12TimeZoneFormatter.ExchangeTimeZoneRule.RuleSizeOffset);
				rule.Flags = BitConverter.ToUInt16(source.Array, source.Offset + O12TimeZoneFormatter.ExchangeTimeZoneRule.FlagsOffset);
				rule.Start = NativeMethods.SystemTime.Parse(new ArraySegment<byte>(source.Array, source.Offset + O12TimeZoneFormatter.ExchangeTimeZoneRule.StartOffset, NativeMethods.SystemTime.Size));
				rule.RegTimeZoneInfo = REG_TIMEZONE_INFO.Parse(new ArraySegment<byte>(source.Array, source.Offset + O12TimeZoneFormatter.ExchangeTimeZoneRule.RegTimeZoneInfoOffset, REG_TIMEZONE_INFO.Size));
				return true;
			}

			public int ToBytes(ArraySegment<byte> target)
			{
				if (target.Count < O12TimeZoneFormatter.ExchangeTimeZoneRule.Size)
				{
					throw new ArgumentOutOfRangeException();
				}
				ExBitConverter.Write((short)this.MajorVersion, target.Array, target.Offset + O12TimeZoneFormatter.ExchangeTimeZoneRule.MajorVersionOffset);
				ExBitConverter.Write((short)this.MinVersion, target.Array, target.Offset + O12TimeZoneFormatter.ExchangeTimeZoneRule.MinVersionOffset);
				ExBitConverter.Write(this.ruleSize, target.Array, target.Offset + O12TimeZoneFormatter.ExchangeTimeZoneRule.RuleSizeOffset);
				ExBitConverter.Write(this.Flags, target.Array, target.Offset + O12TimeZoneFormatter.ExchangeTimeZoneRule.FlagsOffset);
				this.Start.Write(new ArraySegment<byte>(target.Array, target.Offset + O12TimeZoneFormatter.ExchangeTimeZoneRule.StartOffset, NativeMethods.SystemTime.Size));
				this.RegTimeZoneInfo.Write(new ArraySegment<byte>(target.Array, target.Offset + O12TimeZoneFormatter.ExchangeTimeZoneRule.RegTimeZoneInfoOffset, REG_TIMEZONE_INFO.Size));
				return O12TimeZoneFormatter.ExchangeTimeZoneRule.Size;
			}

			private static readonly int MajorVersionOffset = (int)Marshal.OffsetOf(typeof(O12TimeZoneFormatter.ExchangeTimeZoneRule), "MajorVersion");

			private static readonly int MinVersionOffset = (int)Marshal.OffsetOf(typeof(O12TimeZoneFormatter.ExchangeTimeZoneRule), "MinVersion");

			private static readonly int RuleSizeOffset = (int)Marshal.OffsetOf(typeof(O12TimeZoneFormatter.ExchangeTimeZoneRule), "ruleSize");

			private static readonly int FlagsOffset = (int)Marshal.OffsetOf(typeof(O12TimeZoneFormatter.ExchangeTimeZoneRule), "Flags");

			private static readonly int StartOffset = (int)Marshal.OffsetOf(typeof(O12TimeZoneFormatter.ExchangeTimeZoneRule), "Start");

			private static readonly int RegTimeZoneInfoOffset = (int)Marshal.OffsetOf(typeof(O12TimeZoneFormatter.ExchangeTimeZoneRule), "RegTimeZoneInfo");

			public byte MajorVersion;

			public byte MinVersion;

			private ushort ruleSize;

			public ushort Flags;

			public NativeMethods.SystemTime Start;

			public REG_TIMEZONE_INFO RegTimeZoneInfo;

			public static readonly int Size = Marshal.SizeOf(typeof(O12TimeZoneFormatter.ExchangeTimeZoneRule));
		}
	}
}
