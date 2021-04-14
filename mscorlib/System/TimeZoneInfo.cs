using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32;

namespace System
{
	[TypeForwardedFrom("System.Core, Version=3.5.0.0, Culture=Neutral, PublicKeyToken=b77a5c561934e089")]
	[__DynamicallyInvokable]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public sealed class TimeZoneInfo : IEquatable<TimeZoneInfo>, ISerializable, IDeserializationCallback
	{
		[__DynamicallyInvokable]
		public string Id
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_id;
			}
		}

		[__DynamicallyInvokable]
		public string DisplayName
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.m_displayName != null)
				{
					return this.m_displayName;
				}
				return string.Empty;
			}
		}

		[__DynamicallyInvokable]
		public string StandardName
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.m_standardDisplayName != null)
				{
					return this.m_standardDisplayName;
				}
				return string.Empty;
			}
		}

		[__DynamicallyInvokable]
		public string DaylightName
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.m_daylightDisplayName != null)
				{
					return this.m_daylightDisplayName;
				}
				return string.Empty;
			}
		}

		[__DynamicallyInvokable]
		public TimeSpan BaseUtcOffset
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_baseUtcOffset;
			}
		}

		[__DynamicallyInvokable]
		public bool SupportsDaylightSavingTime
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_supportsDaylightSavingTime;
			}
		}

		public TimeZoneInfo.AdjustmentRule[] GetAdjustmentRules()
		{
			if (this.m_adjustmentRules == null)
			{
				return new TimeZoneInfo.AdjustmentRule[0];
			}
			return (TimeZoneInfo.AdjustmentRule[])this.m_adjustmentRules.Clone();
		}

		[__DynamicallyInvokable]
		public TimeSpan[] GetAmbiguousTimeOffsets(DateTimeOffset dateTimeOffset)
		{
			if (!this.SupportsDaylightSavingTime)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_DateTimeOffsetIsNotAmbiguous"), "dateTimeOffset");
			}
			DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset, this).DateTime;
			bool flag = false;
			TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = this.GetAdjustmentRuleForTime(dateTime);
			if (adjustmentRuleForTime != null && adjustmentRuleForTime.HasDaylightSaving)
			{
				DaylightTimeStruct daylightTime = TimeZoneInfo.GetDaylightTime(dateTime.Year, adjustmentRuleForTime);
				flag = TimeZoneInfo.GetIsAmbiguousTime(dateTime, adjustmentRuleForTime, daylightTime);
			}
			if (!flag)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_DateTimeOffsetIsNotAmbiguous"), "dateTimeOffset");
			}
			TimeSpan[] array = new TimeSpan[2];
			TimeSpan timeSpan = this.m_baseUtcOffset + adjustmentRuleForTime.BaseUtcOffsetDelta;
			if (adjustmentRuleForTime.DaylightDelta > TimeSpan.Zero)
			{
				array[0] = timeSpan;
				array[1] = timeSpan + adjustmentRuleForTime.DaylightDelta;
			}
			else
			{
				array[0] = timeSpan + adjustmentRuleForTime.DaylightDelta;
				array[1] = timeSpan;
			}
			return array;
		}

		[__DynamicallyInvokable]
		public TimeSpan[] GetAmbiguousTimeOffsets(DateTime dateTime)
		{
			if (!this.SupportsDaylightSavingTime)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_DateTimeIsNotAmbiguous"), "dateTime");
			}
			DateTime dateTime2;
			if (dateTime.Kind == DateTimeKind.Local)
			{
				TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
				dateTime2 = TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, this, TimeZoneInfoOptions.None, cachedData);
			}
			else if (dateTime.Kind == DateTimeKind.Utc)
			{
				TimeZoneInfo.CachedData cachedData2 = TimeZoneInfo.s_cachedData;
				dateTime2 = TimeZoneInfo.ConvertTime(dateTime, cachedData2.Utc, this, TimeZoneInfoOptions.None, cachedData2);
			}
			else
			{
				dateTime2 = dateTime;
			}
			bool flag = false;
			TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = this.GetAdjustmentRuleForTime(dateTime2);
			if (adjustmentRuleForTime != null && adjustmentRuleForTime.HasDaylightSaving)
			{
				DaylightTimeStruct daylightTime = TimeZoneInfo.GetDaylightTime(dateTime2.Year, adjustmentRuleForTime);
				flag = TimeZoneInfo.GetIsAmbiguousTime(dateTime2, adjustmentRuleForTime, daylightTime);
			}
			if (!flag)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_DateTimeIsNotAmbiguous"), "dateTime");
			}
			TimeSpan[] array = new TimeSpan[2];
			TimeSpan timeSpan = this.m_baseUtcOffset + adjustmentRuleForTime.BaseUtcOffsetDelta;
			if (adjustmentRuleForTime.DaylightDelta > TimeSpan.Zero)
			{
				array[0] = timeSpan;
				array[1] = timeSpan + adjustmentRuleForTime.DaylightDelta;
			}
			else
			{
				array[0] = timeSpan + adjustmentRuleForTime.DaylightDelta;
				array[1] = timeSpan;
			}
			return array;
		}

		[__DynamicallyInvokable]
		public TimeSpan GetUtcOffset(DateTimeOffset dateTimeOffset)
		{
			return TimeZoneInfo.GetUtcOffsetFromUtc(dateTimeOffset.UtcDateTime, this);
		}

		[__DynamicallyInvokable]
		public TimeSpan GetUtcOffset(DateTime dateTime)
		{
			return this.GetUtcOffset(dateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime, TimeZoneInfo.s_cachedData);
		}

		internal static TimeSpan GetLocalUtcOffset(DateTime dateTime, TimeZoneInfoOptions flags)
		{
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			return cachedData.Local.GetUtcOffset(dateTime, flags, cachedData);
		}

		internal TimeSpan GetUtcOffset(DateTime dateTime, TimeZoneInfoOptions flags)
		{
			return this.GetUtcOffset(dateTime, flags, TimeZoneInfo.s_cachedData);
		}

		private TimeSpan GetUtcOffset(DateTime dateTime, TimeZoneInfoOptions flags, TimeZoneInfo.CachedData cachedData)
		{
			if (dateTime.Kind == DateTimeKind.Local)
			{
				if (cachedData.GetCorrespondingKind(this) != DateTimeKind.Local)
				{
					DateTime time = TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, cachedData.Utc, flags);
					return TimeZoneInfo.GetUtcOffsetFromUtc(time, this);
				}
			}
			else if (dateTime.Kind == DateTimeKind.Utc)
			{
				if (cachedData.GetCorrespondingKind(this) == DateTimeKind.Utc)
				{
					return this.m_baseUtcOffset;
				}
				return TimeZoneInfo.GetUtcOffsetFromUtc(dateTime, this);
			}
			return TimeZoneInfo.GetUtcOffset(dateTime, this, flags);
		}

		[__DynamicallyInvokable]
		public bool IsAmbiguousTime(DateTimeOffset dateTimeOffset)
		{
			return this.m_supportsDaylightSavingTime && this.IsAmbiguousTime(TimeZoneInfo.ConvertTime(dateTimeOffset, this).DateTime);
		}

		[__DynamicallyInvokable]
		public bool IsAmbiguousTime(DateTime dateTime)
		{
			return this.IsAmbiguousTime(dateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime);
		}

		internal bool IsAmbiguousTime(DateTime dateTime, TimeZoneInfoOptions flags)
		{
			if (!this.m_supportsDaylightSavingTime)
			{
				return false;
			}
			DateTime dateTime2;
			if (dateTime.Kind == DateTimeKind.Local)
			{
				TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
				dateTime2 = TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, this, flags, cachedData);
			}
			else if (dateTime.Kind == DateTimeKind.Utc)
			{
				TimeZoneInfo.CachedData cachedData2 = TimeZoneInfo.s_cachedData;
				dateTime2 = TimeZoneInfo.ConvertTime(dateTime, cachedData2.Utc, this, flags, cachedData2);
			}
			else
			{
				dateTime2 = dateTime;
			}
			TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = this.GetAdjustmentRuleForTime(dateTime2);
			if (adjustmentRuleForTime != null && adjustmentRuleForTime.HasDaylightSaving)
			{
				DaylightTimeStruct daylightTime = TimeZoneInfo.GetDaylightTime(dateTime2.Year, adjustmentRuleForTime);
				return TimeZoneInfo.GetIsAmbiguousTime(dateTime2, adjustmentRuleForTime, daylightTime);
			}
			return false;
		}

		[__DynamicallyInvokable]
		public bool IsDaylightSavingTime(DateTimeOffset dateTimeOffset)
		{
			bool result;
			TimeZoneInfo.GetUtcOffsetFromUtc(dateTimeOffset.UtcDateTime, this, out result);
			return result;
		}

		[__DynamicallyInvokable]
		public bool IsDaylightSavingTime(DateTime dateTime)
		{
			return this.IsDaylightSavingTime(dateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime, TimeZoneInfo.s_cachedData);
		}

		internal bool IsDaylightSavingTime(DateTime dateTime, TimeZoneInfoOptions flags)
		{
			return this.IsDaylightSavingTime(dateTime, flags, TimeZoneInfo.s_cachedData);
		}

		private bool IsDaylightSavingTime(DateTime dateTime, TimeZoneInfoOptions flags, TimeZoneInfo.CachedData cachedData)
		{
			if (!this.m_supportsDaylightSavingTime || this.m_adjustmentRules == null)
			{
				return false;
			}
			DateTime dateTime2;
			if (dateTime.Kind == DateTimeKind.Local)
			{
				dateTime2 = TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, this, flags, cachedData);
			}
			else if (dateTime.Kind == DateTimeKind.Utc)
			{
				if (cachedData.GetCorrespondingKind(this) == DateTimeKind.Utc)
				{
					return false;
				}
				bool result;
				TimeZoneInfo.GetUtcOffsetFromUtc(dateTime, this, out result);
				return result;
			}
			else
			{
				dateTime2 = dateTime;
			}
			TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = this.GetAdjustmentRuleForTime(dateTime2);
			if (adjustmentRuleForTime != null && adjustmentRuleForTime.HasDaylightSaving)
			{
				DaylightTimeStruct daylightTime = TimeZoneInfo.GetDaylightTime(dateTime2.Year, adjustmentRuleForTime);
				return TimeZoneInfo.GetIsDaylightSavings(dateTime2, adjustmentRuleForTime, daylightTime, flags);
			}
			return false;
		}

		[__DynamicallyInvokable]
		public bool IsInvalidTime(DateTime dateTime)
		{
			bool result = false;
			if (dateTime.Kind == DateTimeKind.Unspecified || (dateTime.Kind == DateTimeKind.Local && TimeZoneInfo.s_cachedData.GetCorrespondingKind(this) == DateTimeKind.Local))
			{
				TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = this.GetAdjustmentRuleForTime(dateTime);
				if (adjustmentRuleForTime != null && adjustmentRuleForTime.HasDaylightSaving)
				{
					DaylightTimeStruct daylightTime = TimeZoneInfo.GetDaylightTime(dateTime.Year, adjustmentRuleForTime);
					result = TimeZoneInfo.GetIsInvalidTime(dateTime, adjustmentRuleForTime, daylightTime);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public static void ClearCachedData()
		{
			TimeZoneInfo.s_cachedData = new TimeZoneInfo.CachedData();
		}

		public static DateTimeOffset ConvertTimeBySystemTimeZoneId(DateTimeOffset dateTimeOffset, string destinationTimeZoneId)
		{
			return TimeZoneInfo.ConvertTime(dateTimeOffset, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId));
		}

		public static DateTime ConvertTimeBySystemTimeZoneId(DateTime dateTime, string destinationTimeZoneId)
		{
			return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId));
		}

		public static DateTime ConvertTimeBySystemTimeZoneId(DateTime dateTime, string sourceTimeZoneId, string destinationTimeZoneId)
		{
			if (dateTime.Kind == DateTimeKind.Local && string.Compare(sourceTimeZoneId, TimeZoneInfo.Local.Id, StringComparison.OrdinalIgnoreCase) == 0)
			{
				TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
				return TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId), TimeZoneInfoOptions.None, cachedData);
			}
			if (dateTime.Kind == DateTimeKind.Utc && string.Compare(sourceTimeZoneId, TimeZoneInfo.Utc.Id, StringComparison.OrdinalIgnoreCase) == 0)
			{
				TimeZoneInfo.CachedData cachedData2 = TimeZoneInfo.s_cachedData;
				return TimeZoneInfo.ConvertTime(dateTime, cachedData2.Utc, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId), TimeZoneInfoOptions.None, cachedData2);
			}
			return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId), TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId));
		}

		[__DynamicallyInvokable]
		public static DateTimeOffset ConvertTime(DateTimeOffset dateTimeOffset, TimeZoneInfo destinationTimeZone)
		{
			if (destinationTimeZone == null)
			{
				throw new ArgumentNullException("destinationTimeZone");
			}
			DateTime utcDateTime = dateTimeOffset.UtcDateTime;
			TimeSpan utcOffsetFromUtc = TimeZoneInfo.GetUtcOffsetFromUtc(utcDateTime, destinationTimeZone);
			long num = utcDateTime.Ticks + utcOffsetFromUtc.Ticks;
			if (num > DateTimeOffset.MaxValue.Ticks)
			{
				return DateTimeOffset.MaxValue;
			}
			if (num < DateTimeOffset.MinValue.Ticks)
			{
				return DateTimeOffset.MinValue;
			}
			return new DateTimeOffset(num, utcOffsetFromUtc);
		}

		[__DynamicallyInvokable]
		public static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo destinationTimeZone)
		{
			if (destinationTimeZone == null)
			{
				throw new ArgumentNullException("destinationTimeZone");
			}
			if (dateTime.Ticks == 0L)
			{
				TimeZoneInfo.ClearCachedData();
			}
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return TimeZoneInfo.ConvertTime(dateTime, cachedData.Utc, destinationTimeZone, TimeZoneInfoOptions.None, cachedData);
			}
			return TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, destinationTimeZone, TimeZoneInfoOptions.None, cachedData);
		}

		[__DynamicallyInvokable]
		public static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
		{
			return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, destinationTimeZone, TimeZoneInfoOptions.None, TimeZoneInfo.s_cachedData);
		}

		internal static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone, TimeZoneInfoOptions flags)
		{
			return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, destinationTimeZone, flags, TimeZoneInfo.s_cachedData);
		}

		private static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone, TimeZoneInfoOptions flags, TimeZoneInfo.CachedData cachedData)
		{
			if (sourceTimeZone == null)
			{
				throw new ArgumentNullException("sourceTimeZone");
			}
			if (destinationTimeZone == null)
			{
				throw new ArgumentNullException("destinationTimeZone");
			}
			DateTimeKind correspondingKind = cachedData.GetCorrespondingKind(sourceTimeZone);
			if ((flags & TimeZoneInfoOptions.NoThrowOnInvalidTime) == (TimeZoneInfoOptions)0 && dateTime.Kind != DateTimeKind.Unspecified && dateTime.Kind != correspondingKind)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_ConvertMismatch"), "sourceTimeZone");
			}
			TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = sourceTimeZone.GetAdjustmentRuleForTime(dateTime);
			TimeSpan t = sourceTimeZone.BaseUtcOffset;
			if (adjustmentRuleForTime != null)
			{
				t += adjustmentRuleForTime.BaseUtcOffsetDelta;
				if (adjustmentRuleForTime.HasDaylightSaving)
				{
					DaylightTimeStruct daylightTime = TimeZoneInfo.GetDaylightTime(dateTime.Year, adjustmentRuleForTime);
					if ((flags & TimeZoneInfoOptions.NoThrowOnInvalidTime) == (TimeZoneInfoOptions)0 && TimeZoneInfo.GetIsInvalidTime(dateTime, adjustmentRuleForTime, daylightTime))
					{
						throw new ArgumentException(Environment.GetResourceString("Argument_DateTimeIsInvalid"), "dateTime");
					}
					bool isDaylightSavings = TimeZoneInfo.GetIsDaylightSavings(dateTime, adjustmentRuleForTime, daylightTime, flags);
					t += (isDaylightSavings ? adjustmentRuleForTime.DaylightDelta : TimeSpan.Zero);
				}
			}
			DateTimeKind correspondingKind2 = cachedData.GetCorrespondingKind(destinationTimeZone);
			if (dateTime.Kind != DateTimeKind.Unspecified && correspondingKind != DateTimeKind.Unspecified && correspondingKind == correspondingKind2)
			{
				return dateTime;
			}
			long ticks = dateTime.Ticks - t.Ticks;
			bool isAmbiguousDst = false;
			DateTime dateTime2 = TimeZoneInfo.ConvertUtcToTimeZone(ticks, destinationTimeZone, out isAmbiguousDst);
			if (correspondingKind2 == DateTimeKind.Local)
			{
				return new DateTime(dateTime2.Ticks, DateTimeKind.Local, isAmbiguousDst);
			}
			return new DateTime(dateTime2.Ticks, correspondingKind2);
		}

		public static DateTime ConvertTimeFromUtc(DateTime dateTime, TimeZoneInfo destinationTimeZone)
		{
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			return TimeZoneInfo.ConvertTime(dateTime, cachedData.Utc, destinationTimeZone, TimeZoneInfoOptions.None, cachedData);
		}

		public static DateTime ConvertTimeToUtc(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime;
			}
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			return TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, cachedData.Utc, TimeZoneInfoOptions.None, cachedData);
		}

		internal static DateTime ConvertTimeToUtc(DateTime dateTime, TimeZoneInfoOptions flags)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime;
			}
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			return TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, cachedData.Utc, flags, cachedData);
		}

		public static DateTime ConvertTimeToUtc(DateTime dateTime, TimeZoneInfo sourceTimeZone)
		{
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, cachedData.Utc, TimeZoneInfoOptions.None, cachedData);
		}

		[__DynamicallyInvokable]
		public bool Equals(TimeZoneInfo other)
		{
			return other != null && string.Compare(this.m_id, other.m_id, StringComparison.OrdinalIgnoreCase) == 0 && this.HasSameRules(other);
		}

		public override bool Equals(object obj)
		{
			TimeZoneInfo timeZoneInfo = obj as TimeZoneInfo;
			return timeZoneInfo != null && this.Equals(timeZoneInfo);
		}

		public static TimeZoneInfo FromSerializedString(string source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidSerializedString", new object[]
				{
					source
				}), "source");
			}
			return TimeZoneInfo.StringSerializer.GetDeserializedTimeZoneInfo(source);
		}

		[__DynamicallyInvokable]
		public override int GetHashCode()
		{
			return this.m_id.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones()
		{
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			TimeZoneInfo.CachedData obj = cachedData;
			lock (obj)
			{
				if (cachedData.m_readOnlySystemTimeZones == null)
				{
					PermissionSet permissionSet = new PermissionSet(PermissionState.None);
					permissionSet.AddPermission(new RegistryPermission(RegistryPermissionAccess.Read, "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones"));
					permissionSet.Assert();
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones", RegistryKeyPermissionCheck.Default, RegistryRights.ExecuteKey))
					{
						if (registryKey != null)
						{
							foreach (string id in registryKey.GetSubKeyNames())
							{
								TimeZoneInfo timeZoneInfo;
								Exception ex;
								TimeZoneInfo.TryGetTimeZone(id, false, out timeZoneInfo, out ex, cachedData);
							}
						}
						cachedData.m_allSystemTimeZonesRead = true;
					}
					List<TimeZoneInfo> list;
					if (cachedData.m_systemTimeZones != null)
					{
						list = new List<TimeZoneInfo>(cachedData.m_systemTimeZones.Values);
					}
					else
					{
						list = new List<TimeZoneInfo>();
					}
					list.Sort(new TimeZoneInfo.TimeZoneInfoComparer());
					cachedData.m_readOnlySystemTimeZones = new ReadOnlyCollection<TimeZoneInfo>(list);
				}
			}
			return cachedData.m_readOnlySystemTimeZones;
		}

		public bool HasSameRules(TimeZoneInfo other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this.m_baseUtcOffset != other.m_baseUtcOffset || this.m_supportsDaylightSavingTime != other.m_supportsDaylightSavingTime)
			{
				return false;
			}
			TimeZoneInfo.AdjustmentRule[] adjustmentRules = this.m_adjustmentRules;
			TimeZoneInfo.AdjustmentRule[] adjustmentRules2 = other.m_adjustmentRules;
			bool flag = (adjustmentRules == null && adjustmentRules2 == null) || (adjustmentRules != null && adjustmentRules2 != null);
			if (!flag)
			{
				return false;
			}
			if (adjustmentRules != null)
			{
				if (adjustmentRules.Length != adjustmentRules2.Length)
				{
					return false;
				}
				for (int i = 0; i < adjustmentRules.Length; i++)
				{
					if (!adjustmentRules[i].Equals(adjustmentRules2[i]))
					{
						return false;
					}
				}
			}
			return flag;
		}

		[__DynamicallyInvokable]
		public static TimeZoneInfo Local
		{
			[__DynamicallyInvokable]
			get
			{
				return TimeZoneInfo.s_cachedData.Local;
			}
		}

		public string ToSerializedString()
		{
			return TimeZoneInfo.StringSerializer.GetSerializedString(this);
		}

		[__DynamicallyInvokable]
		public override string ToString()
		{
			return this.DisplayName;
		}

		[__DynamicallyInvokable]
		public static TimeZoneInfo Utc
		{
			[__DynamicallyInvokable]
			get
			{
				return TimeZoneInfo.s_cachedData.Utc;
			}
		}

		[SecurityCritical]
		private TimeZoneInfo(Win32Native.TimeZoneInformation zone, bool dstDisabled)
		{
			if (string.IsNullOrEmpty(zone.StandardName))
			{
				this.m_id = "Local";
			}
			else
			{
				this.m_id = zone.StandardName;
			}
			this.m_baseUtcOffset = new TimeSpan(0, -zone.Bias, 0);
			if (!dstDisabled)
			{
				Win32Native.RegistryTimeZoneInformation timeZoneInformation = new Win32Native.RegistryTimeZoneInformation(zone);
				TimeZoneInfo.AdjustmentRule adjustmentRule = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(timeZoneInformation, DateTime.MinValue.Date, DateTime.MaxValue.Date, zone.Bias);
				if (adjustmentRule != null)
				{
					this.m_adjustmentRules = new TimeZoneInfo.AdjustmentRule[1];
					this.m_adjustmentRules[0] = adjustmentRule;
				}
			}
			TimeZoneInfo.ValidateTimeZoneInfo(this.m_id, this.m_baseUtcOffset, this.m_adjustmentRules, out this.m_supportsDaylightSavingTime);
			this.m_displayName = zone.StandardName;
			this.m_standardDisplayName = zone.StandardName;
			this.m_daylightDisplayName = zone.DaylightName;
		}

		private TimeZoneInfo(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName, TimeZoneInfo.AdjustmentRule[] adjustmentRules, bool disableDaylightSavingTime)
		{
			bool flag;
			TimeZoneInfo.ValidateTimeZoneInfo(id, baseUtcOffset, adjustmentRules, out flag);
			if (!disableDaylightSavingTime && adjustmentRules != null && adjustmentRules.Length != 0)
			{
				this.m_adjustmentRules = (TimeZoneInfo.AdjustmentRule[])adjustmentRules.Clone();
			}
			this.m_id = id;
			this.m_baseUtcOffset = baseUtcOffset;
			this.m_displayName = displayName;
			this.m_standardDisplayName = standardDisplayName;
			this.m_daylightDisplayName = (disableDaylightSavingTime ? null : daylightDisplayName);
			this.m_supportsDaylightSavingTime = (flag && !disableDaylightSavingTime);
		}

		public static TimeZoneInfo CreateCustomTimeZone(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName)
		{
			return new TimeZoneInfo(id, baseUtcOffset, displayName, standardDisplayName, standardDisplayName, null, false);
		}

		public static TimeZoneInfo CreateCustomTimeZone(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName, TimeZoneInfo.AdjustmentRule[] adjustmentRules)
		{
			return new TimeZoneInfo(id, baseUtcOffset, displayName, standardDisplayName, daylightDisplayName, adjustmentRules, false);
		}

		public static TimeZoneInfo CreateCustomTimeZone(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName, TimeZoneInfo.AdjustmentRule[] adjustmentRules, bool disableDaylightSavingTime)
		{
			return new TimeZoneInfo(id, baseUtcOffset, displayName, standardDisplayName, daylightDisplayName, adjustmentRules, disableDaylightSavingTime);
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			try
			{
				bool flag;
				TimeZoneInfo.ValidateTimeZoneInfo(this.m_id, this.m_baseUtcOffset, this.m_adjustmentRules, out flag);
				if (flag != this.m_supportsDaylightSavingTime)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_CorruptField", new object[]
					{
						"SupportsDaylightSavingTime"
					}));
				}
			}
			catch (ArgumentException innerException)
			{
				throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"), innerException);
			}
			catch (InvalidTimeZoneException innerException2)
			{
				throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"), innerException2);
			}
		}

		[SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Id", this.m_id);
			info.AddValue("DisplayName", this.m_displayName);
			info.AddValue("StandardName", this.m_standardDisplayName);
			info.AddValue("DaylightName", this.m_daylightDisplayName);
			info.AddValue("BaseUtcOffset", this.m_baseUtcOffset);
			info.AddValue("AdjustmentRules", this.m_adjustmentRules);
			info.AddValue("SupportsDaylightSavingTime", this.m_supportsDaylightSavingTime);
		}

		private TimeZoneInfo(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.m_id = (string)info.GetValue("Id", typeof(string));
			this.m_displayName = (string)info.GetValue("DisplayName", typeof(string));
			this.m_standardDisplayName = (string)info.GetValue("StandardName", typeof(string));
			this.m_daylightDisplayName = (string)info.GetValue("DaylightName", typeof(string));
			this.m_baseUtcOffset = (TimeSpan)info.GetValue("BaseUtcOffset", typeof(TimeSpan));
			this.m_adjustmentRules = (TimeZoneInfo.AdjustmentRule[])info.GetValue("AdjustmentRules", typeof(TimeZoneInfo.AdjustmentRule[]));
			this.m_supportsDaylightSavingTime = (bool)info.GetValue("SupportsDaylightSavingTime", typeof(bool));
		}

		private TimeZoneInfo.AdjustmentRule GetAdjustmentRuleForTime(DateTime dateTime)
		{
			if (this.m_adjustmentRules == null || this.m_adjustmentRules.Length == 0)
			{
				return null;
			}
			DateTime date = dateTime.Date;
			for (int i = 0; i < this.m_adjustmentRules.Length; i++)
			{
				if (this.m_adjustmentRules[i].DateStart <= date && this.m_adjustmentRules[i].DateEnd >= date)
				{
					return this.m_adjustmentRules[i];
				}
			}
			return null;
		}

		[SecurityCritical]
		private static bool CheckDaylightSavingTimeNotSupported(Win32Native.TimeZoneInformation timeZone)
		{
			return timeZone.DaylightDate.Year == timeZone.StandardDate.Year && timeZone.DaylightDate.Month == timeZone.StandardDate.Month && timeZone.DaylightDate.DayOfWeek == timeZone.StandardDate.DayOfWeek && timeZone.DaylightDate.Day == timeZone.StandardDate.Day && timeZone.DaylightDate.Hour == timeZone.StandardDate.Hour && timeZone.DaylightDate.Minute == timeZone.StandardDate.Minute && timeZone.DaylightDate.Second == timeZone.StandardDate.Second && timeZone.DaylightDate.Milliseconds == timeZone.StandardDate.Milliseconds;
		}

		private static DateTime ConvertUtcToTimeZone(long ticks, TimeZoneInfo destinationTimeZone, out bool isAmbiguousLocalDst)
		{
			DateTime time;
			if (ticks > DateTime.MaxValue.Ticks)
			{
				time = DateTime.MaxValue;
			}
			else if (ticks < DateTime.MinValue.Ticks)
			{
				time = DateTime.MinValue;
			}
			else
			{
				time = new DateTime(ticks);
			}
			ticks += TimeZoneInfo.GetUtcOffsetFromUtc(time, destinationTimeZone, out isAmbiguousLocalDst).Ticks;
			DateTime result;
			if (ticks > DateTime.MaxValue.Ticks)
			{
				result = DateTime.MaxValue;
			}
			else if (ticks < DateTime.MinValue.Ticks)
			{
				result = DateTime.MinValue;
			}
			else
			{
				result = new DateTime(ticks);
			}
			return result;
		}

		[SecurityCritical]
		private static TimeZoneInfo.AdjustmentRule CreateAdjustmentRuleFromTimeZoneInformation(Win32Native.RegistryTimeZoneInformation timeZoneInformation, DateTime startDate, DateTime endDate, int defaultBaseUtcOffset)
		{
			if (timeZoneInformation.StandardDate.Month == 0)
			{
				if (timeZoneInformation.Bias == defaultBaseUtcOffset)
				{
					return null;
				}
				return TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(startDate, endDate, TimeSpan.Zero, TimeZoneInfo.TransitionTime.CreateFixedDateRule(DateTime.MinValue, 1, 1), TimeZoneInfo.TransitionTime.CreateFixedDateRule(DateTime.MinValue.AddMilliseconds(1.0), 1, 1), new TimeSpan(0, defaultBaseUtcOffset - timeZoneInformation.Bias, 0));
			}
			else
			{
				TimeZoneInfo.TransitionTime daylightTransitionStart;
				if (!TimeZoneInfo.TransitionTimeFromTimeZoneInformation(timeZoneInformation, out daylightTransitionStart, true))
				{
					return null;
				}
				TimeZoneInfo.TransitionTime transitionTime;
				if (!TimeZoneInfo.TransitionTimeFromTimeZoneInformation(timeZoneInformation, out transitionTime, false))
				{
					return null;
				}
				if (daylightTransitionStart.Equals(transitionTime))
				{
					return null;
				}
				return TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(startDate, endDate, new TimeSpan(0, -timeZoneInformation.DaylightBias, 0), daylightTransitionStart, transitionTime, new TimeSpan(0, defaultBaseUtcOffset - timeZoneInformation.Bias, 0));
			}
		}

		[SecuritySafeCritical]
		private static string FindIdFromTimeZoneInformation(Win32Native.TimeZoneInformation timeZone, out bool dstDisabled)
		{
			dstDisabled = false;
			try
			{
				PermissionSet permissionSet = new PermissionSet(PermissionState.None);
				permissionSet.AddPermission(new RegistryPermission(RegistryPermissionAccess.Read, "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones"));
				permissionSet.Assert();
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones", RegistryKeyPermissionCheck.Default, RegistryRights.ExecuteKey))
				{
					if (registryKey == null)
					{
						return null;
					}
					foreach (string text in registryKey.GetSubKeyNames())
					{
						if (TimeZoneInfo.TryCompareTimeZoneInformationToRegistry(timeZone, text, out dstDisabled))
						{
							return text;
						}
					}
				}
			}
			finally
			{
				PermissionSet.RevertAssert();
			}
			return null;
		}

		private static DaylightTimeStruct GetDaylightTime(int year, TimeZoneInfo.AdjustmentRule rule)
		{
			TimeSpan daylightDelta = rule.DaylightDelta;
			DateTime start = TimeZoneInfo.TransitionTimeToDateTime(year, rule.DaylightTransitionStart);
			DateTime end = TimeZoneInfo.TransitionTimeToDateTime(year, rule.DaylightTransitionEnd);
			return new DaylightTimeStruct(start, end, daylightDelta);
		}

		private static bool GetIsDaylightSavings(DateTime time, TimeZoneInfo.AdjustmentRule rule, DaylightTimeStruct daylightTime, TimeZoneInfoOptions flags)
		{
			if (rule == null)
			{
				return false;
			}
			DateTime startTime;
			DateTime endTime;
			if (time.Kind == DateTimeKind.Local)
			{
				startTime = (rule.IsStartDateMarkerForBeginningOfYear() ? new DateTime(daylightTime.Start.Year, 1, 1, 0, 0, 0) : (daylightTime.Start + daylightTime.Delta));
				endTime = (rule.IsEndDateMarkerForEndOfYear() ? new DateTime(daylightTime.End.Year + 1, 1, 1, 0, 0, 0).AddTicks(-1L) : daylightTime.End);
			}
			else
			{
				bool flag = rule.DaylightDelta > TimeSpan.Zero;
				startTime = (rule.IsStartDateMarkerForBeginningOfYear() ? new DateTime(daylightTime.Start.Year, 1, 1, 0, 0, 0) : (daylightTime.Start + (flag ? rule.DaylightDelta : TimeSpan.Zero)));
				endTime = (rule.IsEndDateMarkerForEndOfYear() ? new DateTime(daylightTime.End.Year + 1, 1, 1, 0, 0, 0).AddTicks(-1L) : (daylightTime.End + (flag ? (-rule.DaylightDelta) : TimeSpan.Zero)));
			}
			bool flag2 = TimeZoneInfo.CheckIsDst(startTime, time, endTime, false);
			if (flag2 && time.Kind == DateTimeKind.Local && TimeZoneInfo.GetIsAmbiguousTime(time, rule, daylightTime))
			{
				flag2 = time.IsAmbiguousDaylightSavingTime();
			}
			return flag2;
		}

		private static bool GetIsDaylightSavingsFromUtc(DateTime time, int Year, TimeSpan utc, TimeZoneInfo.AdjustmentRule rule, out bool isAmbiguousLocalDst, TimeZoneInfo zone)
		{
			isAmbiguousLocalDst = false;
			if (rule == null)
			{
				return false;
			}
			TimeSpan t = utc + rule.BaseUtcOffsetDelta;
			DaylightTimeStruct daylightTime = TimeZoneInfo.GetDaylightTime(Year, rule);
			bool ignoreYearAdjustment = false;
			DateTime dateTime;
			if (rule.IsStartDateMarkerForBeginningOfYear() && daylightTime.Start.Year > DateTime.MinValue.Year)
			{
				TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = zone.GetAdjustmentRuleForTime(new DateTime(daylightTime.Start.Year - 1, 12, 31));
				if (adjustmentRuleForTime != null && adjustmentRuleForTime.IsEndDateMarkerForEndOfYear())
				{
					dateTime = TimeZoneInfo.GetDaylightTime(daylightTime.Start.Year - 1, adjustmentRuleForTime).Start - utc - adjustmentRuleForTime.BaseUtcOffsetDelta;
					ignoreYearAdjustment = true;
				}
				else
				{
					dateTime = new DateTime(daylightTime.Start.Year, 1, 1, 0, 0, 0) - t;
				}
			}
			else
			{
				dateTime = daylightTime.Start - t;
			}
			DateTime dateTime2;
			if (rule.IsEndDateMarkerForEndOfYear() && daylightTime.End.Year < DateTime.MaxValue.Year)
			{
				TimeZoneInfo.AdjustmentRule adjustmentRuleForTime2 = zone.GetAdjustmentRuleForTime(new DateTime(daylightTime.End.Year + 1, 1, 1));
				if (adjustmentRuleForTime2 != null && adjustmentRuleForTime2.IsStartDateMarkerForBeginningOfYear())
				{
					if (adjustmentRuleForTime2.IsEndDateMarkerForEndOfYear())
					{
						dateTime2 = new DateTime(daylightTime.End.Year + 1, 12, 31) - utc - adjustmentRuleForTime2.BaseUtcOffsetDelta - adjustmentRuleForTime2.DaylightDelta;
					}
					else
					{
						dateTime2 = TimeZoneInfo.GetDaylightTime(daylightTime.End.Year + 1, adjustmentRuleForTime2).End - utc - adjustmentRuleForTime2.BaseUtcOffsetDelta - adjustmentRuleForTime2.DaylightDelta;
					}
					ignoreYearAdjustment = true;
				}
				else
				{
					dateTime2 = new DateTime(daylightTime.End.Year + 1, 1, 1, 0, 0, 0).AddTicks(-1L) - t - rule.DaylightDelta;
				}
			}
			else
			{
				dateTime2 = daylightTime.End - t - rule.DaylightDelta;
			}
			DateTime t2;
			DateTime t3;
			if (daylightTime.Delta.Ticks > 0L)
			{
				t2 = dateTime2 - daylightTime.Delta;
				t3 = dateTime2;
			}
			else
			{
				t2 = dateTime;
				t3 = dateTime - daylightTime.Delta;
			}
			bool flag = TimeZoneInfo.CheckIsDst(dateTime, time, dateTime2, ignoreYearAdjustment);
			if (flag)
			{
				isAmbiguousLocalDst = (time >= t2 && time < t3);
				if (!isAmbiguousLocalDst && t2.Year != t3.Year)
				{
					try
					{
						DateTime dateTime3 = t2.AddYears(1);
						DateTime dateTime4 = t3.AddYears(1);
						isAmbiguousLocalDst = (time >= t2 && time < t3);
					}
					catch (ArgumentOutOfRangeException)
					{
					}
					if (!isAmbiguousLocalDst)
					{
						try
						{
							DateTime dateTime3 = t2.AddYears(-1);
							DateTime dateTime4 = t3.AddYears(-1);
							isAmbiguousLocalDst = (time >= t2 && time < t3);
						}
						catch (ArgumentOutOfRangeException)
						{
						}
					}
				}
			}
			return flag;
		}

		private static bool CheckIsDst(DateTime startTime, DateTime time, DateTime endTime, bool ignoreYearAdjustment)
		{
			if (!ignoreYearAdjustment)
			{
				int year = startTime.Year;
				int year2 = endTime.Year;
				if (year != year2)
				{
					endTime = endTime.AddYears(year - year2);
				}
				int year3 = time.Year;
				if (year != year3)
				{
					time = time.AddYears(year - year3);
				}
			}
			bool result;
			if (startTime > endTime)
			{
				result = (time < endTime || time >= startTime);
			}
			else
			{
				result = (time >= startTime && time < endTime);
			}
			return result;
		}

		private static bool GetIsAmbiguousTime(DateTime time, TimeZoneInfo.AdjustmentRule rule, DaylightTimeStruct daylightTime)
		{
			bool flag = false;
			if (rule == null || rule.DaylightDelta == TimeSpan.Zero)
			{
				return flag;
			}
			DateTime t;
			DateTime t2;
			if (rule.DaylightDelta > TimeSpan.Zero)
			{
				if (rule.IsEndDateMarkerForEndOfYear())
				{
					return false;
				}
				t = daylightTime.End;
				t2 = daylightTime.End - rule.DaylightDelta;
			}
			else
			{
				if (rule.IsStartDateMarkerForBeginningOfYear())
				{
					return false;
				}
				t = daylightTime.Start;
				t2 = daylightTime.Start + rule.DaylightDelta;
			}
			flag = (time >= t2 && time < t);
			if (!flag && t.Year != t2.Year)
			{
				try
				{
					DateTime t3 = t.AddYears(1);
					DateTime t4 = t2.AddYears(1);
					flag = (time >= t4 && time < t3);
				}
				catch (ArgumentOutOfRangeException)
				{
				}
				if (!flag)
				{
					try
					{
						DateTime t3 = t.AddYears(-1);
						DateTime t4 = t2.AddYears(-1);
						flag = (time >= t4 && time < t3);
					}
					catch (ArgumentOutOfRangeException)
					{
					}
				}
			}
			return flag;
		}

		private static bool GetIsInvalidTime(DateTime time, TimeZoneInfo.AdjustmentRule rule, DaylightTimeStruct daylightTime)
		{
			bool flag = false;
			if (rule == null || rule.DaylightDelta == TimeSpan.Zero)
			{
				return flag;
			}
			DateTime t;
			DateTime t2;
			if (rule.DaylightDelta < TimeSpan.Zero)
			{
				if (rule.IsEndDateMarkerForEndOfYear())
				{
					return false;
				}
				t = daylightTime.End;
				t2 = daylightTime.End - rule.DaylightDelta;
			}
			else
			{
				if (rule.IsStartDateMarkerForBeginningOfYear())
				{
					return false;
				}
				t = daylightTime.Start;
				t2 = daylightTime.Start + rule.DaylightDelta;
			}
			flag = (time >= t && time < t2);
			if (!flag && t.Year != t2.Year)
			{
				try
				{
					DateTime t3 = t.AddYears(1);
					DateTime t4 = t2.AddYears(1);
					flag = (time >= t3 && time < t4);
				}
				catch (ArgumentOutOfRangeException)
				{
				}
				if (!flag)
				{
					try
					{
						DateTime t3 = t.AddYears(-1);
						DateTime t4 = t2.AddYears(-1);
						flag = (time >= t3 && time < t4);
					}
					catch (ArgumentOutOfRangeException)
					{
					}
				}
			}
			return flag;
		}

		[SecuritySafeCritical]
		private static TimeZoneInfo GetLocalTimeZone(TimeZoneInfo.CachedData cachedData)
		{
			Win32Native.DynamicTimeZoneInformation dynamicTimeZoneInformation = default(Win32Native.DynamicTimeZoneInformation);
			long num = (long)UnsafeNativeMethods.GetDynamicTimeZoneInformation(out dynamicTimeZoneInformation);
			if (num == -1L)
			{
				return TimeZoneInfo.CreateCustomTimeZone("Local", TimeSpan.Zero, "Local", "Local");
			}
			Win32Native.TimeZoneInformation timeZoneInformation = new Win32Native.TimeZoneInformation(dynamicTimeZoneInformation);
			bool dynamicDaylightTimeDisabled = dynamicTimeZoneInformation.DynamicDaylightTimeDisabled;
			TimeZoneInfo result;
			Exception ex;
			if (!string.IsNullOrEmpty(dynamicTimeZoneInformation.TimeZoneKeyName) && TimeZoneInfo.TryGetTimeZone(dynamicTimeZoneInformation.TimeZoneKeyName, dynamicDaylightTimeDisabled, out result, out ex, cachedData) == TimeZoneInfo.TimeZoneInfoResult.Success)
			{
				return result;
			}
			string text = TimeZoneInfo.FindIdFromTimeZoneInformation(timeZoneInformation, out dynamicDaylightTimeDisabled);
			TimeZoneInfo result2;
			Exception ex2;
			if (text != null && TimeZoneInfo.TryGetTimeZone(text, dynamicDaylightTimeDisabled, out result2, out ex2, cachedData) == TimeZoneInfo.TimeZoneInfoResult.Success)
			{
				return result2;
			}
			return TimeZoneInfo.GetLocalTimeZoneFromWin32Data(timeZoneInformation, dynamicDaylightTimeDisabled);
		}

		[SecurityCritical]
		private static TimeZoneInfo GetLocalTimeZoneFromWin32Data(Win32Native.TimeZoneInformation timeZoneInformation, bool dstDisabled)
		{
			try
			{
				return new TimeZoneInfo(timeZoneInformation, dstDisabled);
			}
			catch (ArgumentException)
			{
			}
			catch (InvalidTimeZoneException)
			{
			}
			if (!dstDisabled)
			{
				try
				{
					return new TimeZoneInfo(timeZoneInformation, true);
				}
				catch (ArgumentException)
				{
				}
				catch (InvalidTimeZoneException)
				{
				}
			}
			return TimeZoneInfo.CreateCustomTimeZone("Local", TimeSpan.Zero, "Local", "Local");
		}

		[__DynamicallyInvokable]
		public static TimeZoneInfo FindSystemTimeZoneById(string id)
		{
			if (string.Compare(id, "UTC", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return TimeZoneInfo.Utc;
			}
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (id.Length == 0 || id.Length > 255 || id.Contains("\0"))
			{
				throw new TimeZoneNotFoundException(Environment.GetResourceString("TimeZoneNotFound_MissingRegistryData", new object[]
				{
					id
				}));
			}
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			TimeZoneInfo.CachedData obj = cachedData;
			TimeZoneInfo result;
			Exception ex;
			TimeZoneInfo.TimeZoneInfoResult timeZoneInfoResult;
			lock (obj)
			{
				timeZoneInfoResult = TimeZoneInfo.TryGetTimeZone(id, false, out result, out ex, cachedData);
			}
			if (timeZoneInfoResult == TimeZoneInfo.TimeZoneInfoResult.Success)
			{
				return result;
			}
			if (timeZoneInfoResult == TimeZoneInfo.TimeZoneInfoResult.InvalidTimeZoneException)
			{
				throw new InvalidTimeZoneException(Environment.GetResourceString("InvalidTimeZone_InvalidRegistryData", new object[]
				{
					id
				}), ex);
			}
			if (timeZoneInfoResult == TimeZoneInfo.TimeZoneInfoResult.SecurityException)
			{
				throw new SecurityException(Environment.GetResourceString("Security_CannotReadRegistryData", new object[]
				{
					id
				}), ex);
			}
			throw new TimeZoneNotFoundException(Environment.GetResourceString("TimeZoneNotFound_MissingRegistryData", new object[]
			{
				id
			}), ex);
		}

		private static TimeSpan GetUtcOffset(DateTime time, TimeZoneInfo zone, TimeZoneInfoOptions flags)
		{
			TimeSpan timeSpan = zone.BaseUtcOffset;
			TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = zone.GetAdjustmentRuleForTime(time);
			if (adjustmentRuleForTime != null)
			{
				timeSpan += adjustmentRuleForTime.BaseUtcOffsetDelta;
				if (adjustmentRuleForTime.HasDaylightSaving)
				{
					DaylightTimeStruct daylightTime = TimeZoneInfo.GetDaylightTime(time.Year, adjustmentRuleForTime);
					bool isDaylightSavings = TimeZoneInfo.GetIsDaylightSavings(time, adjustmentRuleForTime, daylightTime, flags);
					timeSpan += (isDaylightSavings ? adjustmentRuleForTime.DaylightDelta : TimeSpan.Zero);
				}
			}
			return timeSpan;
		}

		private static TimeSpan GetUtcOffsetFromUtc(DateTime time, TimeZoneInfo zone)
		{
			bool flag;
			return TimeZoneInfo.GetUtcOffsetFromUtc(time, zone, out flag);
		}

		private static TimeSpan GetUtcOffsetFromUtc(DateTime time, TimeZoneInfo zone, out bool isDaylightSavings)
		{
			bool flag;
			return TimeZoneInfo.GetUtcOffsetFromUtc(time, zone, out isDaylightSavings, out flag);
		}

		internal static TimeSpan GetDateTimeNowUtcOffsetFromUtc(DateTime time, out bool isAmbiguousLocalDst)
		{
			isAmbiguousLocalDst = false;
			int year = time.Year;
			TimeZoneInfo.OffsetAndRule oneYearLocalFromUtc = TimeZoneInfo.s_cachedData.GetOneYearLocalFromUtc(year);
			TimeSpan timeSpan = oneYearLocalFromUtc.offset;
			if (oneYearLocalFromUtc.rule != null)
			{
				timeSpan += oneYearLocalFromUtc.rule.BaseUtcOffsetDelta;
				if (oneYearLocalFromUtc.rule.HasDaylightSaving)
				{
					bool isDaylightSavingsFromUtc = TimeZoneInfo.GetIsDaylightSavingsFromUtc(time, year, oneYearLocalFromUtc.offset, oneYearLocalFromUtc.rule, out isAmbiguousLocalDst, TimeZoneInfo.Local);
					timeSpan += (isDaylightSavingsFromUtc ? oneYearLocalFromUtc.rule.DaylightDelta : TimeSpan.Zero);
				}
			}
			return timeSpan;
		}

		internal static TimeSpan GetUtcOffsetFromUtc(DateTime time, TimeZoneInfo zone, out bool isDaylightSavings, out bool isAmbiguousLocalDst)
		{
			isDaylightSavings = false;
			isAmbiguousLocalDst = false;
			TimeSpan timeSpan = zone.BaseUtcOffset;
			TimeZoneInfo.AdjustmentRule adjustmentRuleForTime;
			int year;
			if (time > TimeZoneInfo.s_maxDateOnly)
			{
				adjustmentRuleForTime = zone.GetAdjustmentRuleForTime(DateTime.MaxValue);
				year = 9999;
			}
			else if (time < TimeZoneInfo.s_minDateOnly)
			{
				adjustmentRuleForTime = zone.GetAdjustmentRuleForTime(DateTime.MinValue);
				year = 1;
			}
			else
			{
				DateTime dateTime = time + timeSpan;
				year = dateTime.Year;
				adjustmentRuleForTime = zone.GetAdjustmentRuleForTime(dateTime);
			}
			if (adjustmentRuleForTime != null)
			{
				timeSpan += adjustmentRuleForTime.BaseUtcOffsetDelta;
				if (adjustmentRuleForTime.HasDaylightSaving)
				{
					isDaylightSavings = TimeZoneInfo.GetIsDaylightSavingsFromUtc(time, year, zone.m_baseUtcOffset, adjustmentRuleForTime, out isAmbiguousLocalDst, zone);
					timeSpan += (isDaylightSavings ? adjustmentRuleForTime.DaylightDelta : TimeSpan.Zero);
				}
			}
			return timeSpan;
		}

		[SecurityCritical]
		private static bool TransitionTimeFromTimeZoneInformation(Win32Native.RegistryTimeZoneInformation timeZoneInformation, out TimeZoneInfo.TransitionTime transitionTime, bool readStartDate)
		{
			if (timeZoneInformation.StandardDate.Month == 0)
			{
				transitionTime = default(TimeZoneInfo.TransitionTime);
				return false;
			}
			if (readStartDate)
			{
				if (timeZoneInformation.DaylightDate.Year == 0)
				{
					transitionTime = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.DaylightDate.Hour, (int)timeZoneInformation.DaylightDate.Minute, (int)timeZoneInformation.DaylightDate.Second, (int)timeZoneInformation.DaylightDate.Milliseconds), (int)timeZoneInformation.DaylightDate.Month, (int)timeZoneInformation.DaylightDate.Day, (DayOfWeek)timeZoneInformation.DaylightDate.DayOfWeek);
				}
				else
				{
					transitionTime = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.DaylightDate.Hour, (int)timeZoneInformation.DaylightDate.Minute, (int)timeZoneInformation.DaylightDate.Second, (int)timeZoneInformation.DaylightDate.Milliseconds), (int)timeZoneInformation.DaylightDate.Month, (int)timeZoneInformation.DaylightDate.Day);
				}
			}
			else if (timeZoneInformation.StandardDate.Year == 0)
			{
				transitionTime = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.StandardDate.Hour, (int)timeZoneInformation.StandardDate.Minute, (int)timeZoneInformation.StandardDate.Second, (int)timeZoneInformation.StandardDate.Milliseconds), (int)timeZoneInformation.StandardDate.Month, (int)timeZoneInformation.StandardDate.Day, (DayOfWeek)timeZoneInformation.StandardDate.DayOfWeek);
			}
			else
			{
				transitionTime = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.StandardDate.Hour, (int)timeZoneInformation.StandardDate.Minute, (int)timeZoneInformation.StandardDate.Second, (int)timeZoneInformation.StandardDate.Milliseconds), (int)timeZoneInformation.StandardDate.Month, (int)timeZoneInformation.StandardDate.Day);
			}
			return true;
		}

		private static DateTime TransitionTimeToDateTime(int year, TimeZoneInfo.TransitionTime transitionTime)
		{
			DateTime timeOfDay = transitionTime.TimeOfDay;
			DateTime result;
			if (transitionTime.IsFixedDateRule)
			{
				int num = DateTime.DaysInMonth(year, transitionTime.Month);
				result = new DateTime(year, transitionTime.Month, (num < transitionTime.Day) ? num : transitionTime.Day, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second, timeOfDay.Millisecond);
			}
			else if (transitionTime.Week <= 4)
			{
				result = new DateTime(year, transitionTime.Month, 1, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second, timeOfDay.Millisecond);
				int dayOfWeek = (int)result.DayOfWeek;
				int num2 = transitionTime.DayOfWeek - (DayOfWeek)dayOfWeek;
				if (num2 < 0)
				{
					num2 += 7;
				}
				num2 += 7 * (transitionTime.Week - 1);
				if (num2 > 0)
				{
					result = result.AddDays((double)num2);
				}
			}
			else
			{
				int day = DateTime.DaysInMonth(year, transitionTime.Month);
				result = new DateTime(year, transitionTime.Month, day, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second, timeOfDay.Millisecond);
				int dayOfWeek2 = (int)result.DayOfWeek;
				int num3 = dayOfWeek2 - (int)transitionTime.DayOfWeek;
				if (num3 < 0)
				{
					num3 += 7;
				}
				if (num3 > 0)
				{
					result = result.AddDays((double)(-(double)num3));
				}
			}
			return result;
		}

		[SecurityCritical]
		private static bool TryCreateAdjustmentRules(string id, Win32Native.RegistryTimeZoneInformation defaultTimeZoneInformation, out TimeZoneInfo.AdjustmentRule[] rules, out Exception e, int defaultBaseUtcOffset)
		{
			e = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}\\Dynamic DST", "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones", id), RegistryKeyPermissionCheck.Default, RegistryRights.ExecuteKey))
				{
					if (registryKey == null)
					{
						TimeZoneInfo.AdjustmentRule adjustmentRule = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(defaultTimeZoneInformation, DateTime.MinValue.Date, DateTime.MaxValue.Date, defaultBaseUtcOffset);
						if (adjustmentRule == null)
						{
							rules = null;
						}
						else
						{
							rules = new TimeZoneInfo.AdjustmentRule[1];
							rules[0] = adjustmentRule;
						}
						return true;
					}
					int num = (int)registryKey.GetValue("FirstEntry", -1, RegistryValueOptions.None);
					int num2 = (int)registryKey.GetValue("LastEntry", -1, RegistryValueOptions.None);
					if (num == -1 || num2 == -1 || num > num2)
					{
						rules = null;
						return false;
					}
					byte[] array = registryKey.GetValue(num.ToString(CultureInfo.InvariantCulture), null, RegistryValueOptions.None) as byte[];
					if (array == null || array.Length != 44)
					{
						rules = null;
						return false;
					}
					Win32Native.RegistryTimeZoneInformation timeZoneInformation = new Win32Native.RegistryTimeZoneInformation(array);
					if (num == num2)
					{
						TimeZoneInfo.AdjustmentRule adjustmentRule2 = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(timeZoneInformation, DateTime.MinValue.Date, DateTime.MaxValue.Date, defaultBaseUtcOffset);
						if (adjustmentRule2 == null)
						{
							rules = null;
						}
						else
						{
							rules = new TimeZoneInfo.AdjustmentRule[1];
							rules[0] = adjustmentRule2;
						}
						return true;
					}
					List<TimeZoneInfo.AdjustmentRule> list = new List<TimeZoneInfo.AdjustmentRule>(1);
					TimeZoneInfo.AdjustmentRule adjustmentRule3 = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(timeZoneInformation, DateTime.MinValue.Date, new DateTime(num, 12, 31), defaultBaseUtcOffset);
					if (adjustmentRule3 != null)
					{
						list.Add(adjustmentRule3);
					}
					for (int i = num + 1; i < num2; i++)
					{
						array = (registryKey.GetValue(i.ToString(CultureInfo.InvariantCulture), null, RegistryValueOptions.None) as byte[]);
						if (array == null || array.Length != 44)
						{
							rules = null;
							return false;
						}
						timeZoneInformation = new Win32Native.RegistryTimeZoneInformation(array);
						TimeZoneInfo.AdjustmentRule adjustmentRule4 = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(timeZoneInformation, new DateTime(i, 1, 1), new DateTime(i, 12, 31), defaultBaseUtcOffset);
						if (adjustmentRule4 != null)
						{
							list.Add(adjustmentRule4);
						}
					}
					array = (registryKey.GetValue(num2.ToString(CultureInfo.InvariantCulture), null, RegistryValueOptions.None) as byte[]);
					timeZoneInformation = new Win32Native.RegistryTimeZoneInformation(array);
					if (array == null || array.Length != 44)
					{
						rules = null;
						return false;
					}
					TimeZoneInfo.AdjustmentRule adjustmentRule5 = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(timeZoneInformation, new DateTime(num2, 1, 1), DateTime.MaxValue.Date, defaultBaseUtcOffset);
					if (adjustmentRule5 != null)
					{
						list.Add(adjustmentRule5);
					}
					rules = list.ToArray();
					if (rules != null && rules.Length == 0)
					{
						rules = null;
					}
				}
			}
			catch (InvalidCastException ex)
			{
				rules = null;
				e = ex;
				return false;
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				rules = null;
				e = ex2;
				return false;
			}
			catch (ArgumentException ex3)
			{
				rules = null;
				e = ex3;
				return false;
			}
			return true;
		}

		[SecurityCritical]
		private static bool TryCompareStandardDate(Win32Native.TimeZoneInformation timeZone, Win32Native.RegistryTimeZoneInformation registryTimeZoneInfo)
		{
			return timeZone.Bias == registryTimeZoneInfo.Bias && timeZone.StandardBias == registryTimeZoneInfo.StandardBias && timeZone.StandardDate.Year == registryTimeZoneInfo.StandardDate.Year && timeZone.StandardDate.Month == registryTimeZoneInfo.StandardDate.Month && timeZone.StandardDate.DayOfWeek == registryTimeZoneInfo.StandardDate.DayOfWeek && timeZone.StandardDate.Day == registryTimeZoneInfo.StandardDate.Day && timeZone.StandardDate.Hour == registryTimeZoneInfo.StandardDate.Hour && timeZone.StandardDate.Minute == registryTimeZoneInfo.StandardDate.Minute && timeZone.StandardDate.Second == registryTimeZoneInfo.StandardDate.Second && timeZone.StandardDate.Milliseconds == registryTimeZoneInfo.StandardDate.Milliseconds;
		}

		[SecuritySafeCritical]
		private static bool TryCompareTimeZoneInformationToRegistry(Win32Native.TimeZoneInformation timeZone, string id, out bool dstDisabled)
		{
			dstDisabled = false;
			bool result;
			try
			{
				PermissionSet permissionSet = new PermissionSet(PermissionState.None);
				permissionSet.AddPermission(new RegistryPermission(RegistryPermissionAccess.Read, "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones"));
				permissionSet.Assert();
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones", id), RegistryKeyPermissionCheck.Default, RegistryRights.ExecuteKey))
				{
					if (registryKey == null)
					{
						result = false;
					}
					else
					{
						byte[] array = (byte[])registryKey.GetValue("TZI", null, RegistryValueOptions.None);
						if (array == null || array.Length != 44)
						{
							result = false;
						}
						else
						{
							Win32Native.RegistryTimeZoneInformation registryTimeZoneInformation = new Win32Native.RegistryTimeZoneInformation(array);
							if (!TimeZoneInfo.TryCompareStandardDate(timeZone, registryTimeZoneInformation))
							{
								result = false;
							}
							else
							{
								bool flag = dstDisabled || TimeZoneInfo.CheckDaylightSavingTimeNotSupported(timeZone) || (timeZone.DaylightBias == registryTimeZoneInformation.DaylightBias && timeZone.DaylightDate.Year == registryTimeZoneInformation.DaylightDate.Year && timeZone.DaylightDate.Month == registryTimeZoneInformation.DaylightDate.Month && timeZone.DaylightDate.DayOfWeek == registryTimeZoneInformation.DaylightDate.DayOfWeek && timeZone.DaylightDate.Day == registryTimeZoneInformation.DaylightDate.Day && timeZone.DaylightDate.Hour == registryTimeZoneInformation.DaylightDate.Hour && timeZone.DaylightDate.Minute == registryTimeZoneInformation.DaylightDate.Minute && timeZone.DaylightDate.Second == registryTimeZoneInformation.DaylightDate.Second && timeZone.DaylightDate.Milliseconds == registryTimeZoneInformation.DaylightDate.Milliseconds);
								if (flag)
								{
									string strA = registryKey.GetValue("Std", string.Empty, RegistryValueOptions.None) as string;
									flag = (string.Compare(strA, timeZone.StandardName, StringComparison.Ordinal) == 0);
								}
								result = flag;
							}
						}
					}
				}
			}
			finally
			{
				PermissionSet.RevertAssert();
			}
			return result;
		}

		[SecuritySafeCritical]
		[FileIOPermission(SecurityAction.Assert, AllLocalFiles = FileIOPermissionAccess.PathDiscovery)]
		private static string TryGetLocalizedNameByMuiNativeResource(string resource)
		{
			if (string.IsNullOrEmpty(resource))
			{
				return string.Empty;
			}
			string[] array = resource.Split(new char[]
			{
				','
			}, StringSplitOptions.None);
			if (array.Length != 2)
			{
				return string.Empty;
			}
			string path = Environment.UnsafeGetFolderPath(Environment.SpecialFolder.System);
			string path2 = array[0].TrimStart(new char[]
			{
				'@'
			});
			string filePath;
			try
			{
				filePath = Path.Combine(path, path2);
			}
			catch (ArgumentException)
			{
				return string.Empty;
			}
			int num;
			if (!int.TryParse(array[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
			{
				return string.Empty;
			}
			num = -num;
			string result;
			try
			{
				StringBuilder stringBuilder = StringBuilderCache.Acquire(260);
				stringBuilder.Length = 260;
				int num2 = 260;
				int num3 = 0;
				long num4 = 0L;
				if (!UnsafeNativeMethods.GetFileMUIPath(16, filePath, null, ref num3, stringBuilder, ref num2, ref num4))
				{
					StringBuilderCache.Release(stringBuilder);
					result = string.Empty;
				}
				else
				{
					result = TimeZoneInfo.TryGetLocalizedNameByNativeResource(StringBuilderCache.GetStringAndRelease(stringBuilder), num);
				}
			}
			catch (EntryPointNotFoundException)
			{
				result = string.Empty;
			}
			return result;
		}

		[SecurityCritical]
		private static string TryGetLocalizedNameByNativeResource(string filePath, int resource)
		{
			using (SafeLibraryHandle safeLibraryHandle = UnsafeNativeMethods.LoadLibraryEx(filePath, IntPtr.Zero, 2))
			{
				if (!safeLibraryHandle.IsInvalid)
				{
					StringBuilder stringBuilder = StringBuilderCache.Acquire(500);
					stringBuilder.Length = 500;
					int num = UnsafeNativeMethods.LoadString(safeLibraryHandle, resource, stringBuilder, stringBuilder.Length);
					if (num != 0)
					{
						return StringBuilderCache.GetStringAndRelease(stringBuilder);
					}
				}
			}
			return string.Empty;
		}

		private static bool TryGetLocalizedNamesByRegistryKey(RegistryKey key, out string displayName, out string standardName, out string daylightName)
		{
			displayName = string.Empty;
			standardName = string.Empty;
			daylightName = string.Empty;
			string text = key.GetValue("MUI_Display", string.Empty, RegistryValueOptions.None) as string;
			string text2 = key.GetValue("MUI_Std", string.Empty, RegistryValueOptions.None) as string;
			string text3 = key.GetValue("MUI_Dlt", string.Empty, RegistryValueOptions.None) as string;
			if (!string.IsNullOrEmpty(text))
			{
				displayName = TimeZoneInfo.TryGetLocalizedNameByMuiNativeResource(text);
			}
			if (!string.IsNullOrEmpty(text2))
			{
				standardName = TimeZoneInfo.TryGetLocalizedNameByMuiNativeResource(text2);
			}
			if (!string.IsNullOrEmpty(text3))
			{
				daylightName = TimeZoneInfo.TryGetLocalizedNameByMuiNativeResource(text3);
			}
			if (string.IsNullOrEmpty(displayName))
			{
				displayName = (key.GetValue("Display", string.Empty, RegistryValueOptions.None) as string);
			}
			if (string.IsNullOrEmpty(standardName))
			{
				standardName = (key.GetValue("Std", string.Empty, RegistryValueOptions.None) as string);
			}
			if (string.IsNullOrEmpty(daylightName))
			{
				daylightName = (key.GetValue("Dlt", string.Empty, RegistryValueOptions.None) as string);
			}
			return true;
		}

		[SecuritySafeCritical]
		private static TimeZoneInfo.TimeZoneInfoResult TryGetTimeZoneByRegistryKey(string id, out TimeZoneInfo value, out Exception e)
		{
			e = null;
			TimeZoneInfo.TimeZoneInfoResult result;
			try
			{
				PermissionSet permissionSet = new PermissionSet(PermissionState.None);
				permissionSet.AddPermission(new RegistryPermission(RegistryPermissionAccess.Read, "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones"));
				permissionSet.Assert();
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones", id), RegistryKeyPermissionCheck.Default, RegistryRights.ExecuteKey))
				{
					if (registryKey == null)
					{
						value = null;
						result = TimeZoneInfo.TimeZoneInfoResult.TimeZoneNotFoundException;
					}
					else
					{
						byte[] array = registryKey.GetValue("TZI", null, RegistryValueOptions.None) as byte[];
						if (array == null || array.Length != 44)
						{
							value = null;
							result = TimeZoneInfo.TimeZoneInfoResult.InvalidTimeZoneException;
						}
						else
						{
							Win32Native.RegistryTimeZoneInformation registryTimeZoneInformation = new Win32Native.RegistryTimeZoneInformation(array);
							TimeZoneInfo.AdjustmentRule[] adjustmentRules;
							string displayName;
							string standardDisplayName;
							string daylightDisplayName;
							if (!TimeZoneInfo.TryCreateAdjustmentRules(id, registryTimeZoneInformation, out adjustmentRules, out e, registryTimeZoneInformation.Bias))
							{
								value = null;
								result = TimeZoneInfo.TimeZoneInfoResult.InvalidTimeZoneException;
							}
							else if (!TimeZoneInfo.TryGetLocalizedNamesByRegistryKey(registryKey, out displayName, out standardDisplayName, out daylightDisplayName))
							{
								value = null;
								result = TimeZoneInfo.TimeZoneInfoResult.InvalidTimeZoneException;
							}
							else
							{
								try
								{
									value = new TimeZoneInfo(id, new TimeSpan(0, -registryTimeZoneInformation.Bias, 0), displayName, standardDisplayName, daylightDisplayName, adjustmentRules, false);
									result = TimeZoneInfo.TimeZoneInfoResult.Success;
								}
								catch (ArgumentException ex)
								{
									value = null;
									e = ex;
									result = TimeZoneInfo.TimeZoneInfoResult.InvalidTimeZoneException;
								}
								catch (InvalidTimeZoneException ex2)
								{
									value = null;
									e = ex2;
									result = TimeZoneInfo.TimeZoneInfoResult.InvalidTimeZoneException;
								}
							}
						}
					}
				}
			}
			finally
			{
				PermissionSet.RevertAssert();
			}
			return result;
		}

		private static TimeZoneInfo.TimeZoneInfoResult TryGetTimeZone(string id, bool dstDisabled, out TimeZoneInfo value, out Exception e, TimeZoneInfo.CachedData cachedData)
		{
			TimeZoneInfo.TimeZoneInfoResult timeZoneInfoResult = TimeZoneInfo.TimeZoneInfoResult.Success;
			e = null;
			TimeZoneInfo timeZoneInfo = null;
			if (cachedData.m_systemTimeZones != null && cachedData.m_systemTimeZones.TryGetValue(id, out timeZoneInfo))
			{
				if (dstDisabled && timeZoneInfo.m_supportsDaylightSavingTime)
				{
					value = TimeZoneInfo.CreateCustomTimeZone(timeZoneInfo.m_id, timeZoneInfo.m_baseUtcOffset, timeZoneInfo.m_displayName, timeZoneInfo.m_standardDisplayName);
				}
				else
				{
					value = new TimeZoneInfo(timeZoneInfo.m_id, timeZoneInfo.m_baseUtcOffset, timeZoneInfo.m_displayName, timeZoneInfo.m_standardDisplayName, timeZoneInfo.m_daylightDisplayName, timeZoneInfo.m_adjustmentRules, false);
				}
				return timeZoneInfoResult;
			}
			if (!cachedData.m_allSystemTimeZonesRead)
			{
				timeZoneInfoResult = TimeZoneInfo.TryGetTimeZoneByRegistryKey(id, out timeZoneInfo, out e);
				if (timeZoneInfoResult == TimeZoneInfo.TimeZoneInfoResult.Success)
				{
					if (cachedData.m_systemTimeZones == null)
					{
						cachedData.m_systemTimeZones = new Dictionary<string, TimeZoneInfo>();
					}
					cachedData.m_systemTimeZones.Add(id, timeZoneInfo);
					if (dstDisabled && timeZoneInfo.m_supportsDaylightSavingTime)
					{
						value = TimeZoneInfo.CreateCustomTimeZone(timeZoneInfo.m_id, timeZoneInfo.m_baseUtcOffset, timeZoneInfo.m_displayName, timeZoneInfo.m_standardDisplayName);
					}
					else
					{
						value = new TimeZoneInfo(timeZoneInfo.m_id, timeZoneInfo.m_baseUtcOffset, timeZoneInfo.m_displayName, timeZoneInfo.m_standardDisplayName, timeZoneInfo.m_daylightDisplayName, timeZoneInfo.m_adjustmentRules, false);
					}
				}
				else
				{
					value = null;
				}
			}
			else
			{
				timeZoneInfoResult = TimeZoneInfo.TimeZoneInfoResult.TimeZoneNotFoundException;
				value = null;
			}
			return timeZoneInfoResult;
		}

		internal static bool UtcOffsetOutOfRange(TimeSpan offset)
		{
			return offset.TotalHours < -14.0 || offset.TotalHours > 14.0;
		}

		private static void ValidateTimeZoneInfo(string id, TimeSpan baseUtcOffset, TimeZoneInfo.AdjustmentRule[] adjustmentRules, out bool adjustmentRulesSupportDst)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (id.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidId", new object[]
				{
					id
				}), "id");
			}
			if (TimeZoneInfo.UtcOffsetOutOfRange(baseUtcOffset))
			{
				throw new ArgumentOutOfRangeException("baseUtcOffset", Environment.GetResourceString("ArgumentOutOfRange_UtcOffset"));
			}
			if (baseUtcOffset.Ticks % 600000000L != 0L)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_TimeSpanHasSeconds"), "baseUtcOffset");
			}
			adjustmentRulesSupportDst = false;
			if (adjustmentRules != null && adjustmentRules.Length != 0)
			{
				adjustmentRulesSupportDst = true;
				TimeZoneInfo.AdjustmentRule adjustmentRule = null;
				for (int i = 0; i < adjustmentRules.Length; i++)
				{
					TimeZoneInfo.AdjustmentRule adjustmentRule2 = adjustmentRule;
					adjustmentRule = adjustmentRules[i];
					if (adjustmentRule == null)
					{
						throw new InvalidTimeZoneException(Environment.GetResourceString("Argument_AdjustmentRulesNoNulls"));
					}
					if (TimeZoneInfo.UtcOffsetOutOfRange(baseUtcOffset + adjustmentRule.DaylightDelta))
					{
						throw new InvalidTimeZoneException(Environment.GetResourceString("ArgumentOutOfRange_UtcOffsetAndDaylightDelta"));
					}
					if (adjustmentRule2 != null && adjustmentRule.DateStart <= adjustmentRule2.DateEnd)
					{
						throw new InvalidTimeZoneException(Environment.GetResourceString("Argument_AdjustmentRulesOutOfOrder"));
					}
				}
			}
		}

		private string m_id;

		private string m_displayName;

		private string m_standardDisplayName;

		private string m_daylightDisplayName;

		private TimeSpan m_baseUtcOffset;

		private bool m_supportsDaylightSavingTime;

		private TimeZoneInfo.AdjustmentRule[] m_adjustmentRules;

		private const string c_timeZonesRegistryHive = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones";

		private const string c_timeZonesRegistryHivePermissionList = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones";

		private const string c_displayValue = "Display";

		private const string c_daylightValue = "Dlt";

		private const string c_standardValue = "Std";

		private const string c_muiDisplayValue = "MUI_Display";

		private const string c_muiDaylightValue = "MUI_Dlt";

		private const string c_muiStandardValue = "MUI_Std";

		private const string c_timeZoneInfoValue = "TZI";

		private const string c_firstEntryValue = "FirstEntry";

		private const string c_lastEntryValue = "LastEntry";

		private const string c_utcId = "UTC";

		private const string c_localId = "Local";

		private const int c_maxKeyLength = 255;

		private const int c_regByteLength = 44;

		private const long c_ticksPerMillisecond = 10000L;

		private const long c_ticksPerSecond = 10000000L;

		private const long c_ticksPerMinute = 600000000L;

		private const long c_ticksPerHour = 36000000000L;

		private const long c_ticksPerDay = 864000000000L;

		private const long c_ticksPerDayRange = 863999990000L;

		private static TimeZoneInfo.CachedData s_cachedData = new TimeZoneInfo.CachedData();

		private static DateTime s_maxDateOnly = new DateTime(9999, 12, 31);

		private static DateTime s_minDateOnly = new DateTime(1, 1, 2);

		private enum TimeZoneInfoResult
		{
			Success,
			TimeZoneNotFoundException,
			InvalidTimeZoneException,
			SecurityException
		}

		private class CachedData
		{
			private TimeZoneInfo CreateLocal()
			{
				TimeZoneInfo result;
				lock (this)
				{
					TimeZoneInfo timeZoneInfo = this.m_localTimeZone;
					if (timeZoneInfo == null)
					{
						timeZoneInfo = TimeZoneInfo.GetLocalTimeZone(this);
						timeZoneInfo = new TimeZoneInfo(timeZoneInfo.m_id, timeZoneInfo.m_baseUtcOffset, timeZoneInfo.m_displayName, timeZoneInfo.m_standardDisplayName, timeZoneInfo.m_daylightDisplayName, timeZoneInfo.m_adjustmentRules, false);
						this.m_localTimeZone = timeZoneInfo;
					}
					result = timeZoneInfo;
				}
				return result;
			}

			public TimeZoneInfo Local
			{
				get
				{
					TimeZoneInfo timeZoneInfo = this.m_localTimeZone;
					if (timeZoneInfo == null)
					{
						timeZoneInfo = this.CreateLocal();
					}
					return timeZoneInfo;
				}
			}

			private TimeZoneInfo CreateUtc()
			{
				TimeZoneInfo result;
				lock (this)
				{
					TimeZoneInfo timeZoneInfo = this.m_utcTimeZone;
					if (timeZoneInfo == null)
					{
						timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("UTC", TimeSpan.Zero, "UTC", "UTC");
						this.m_utcTimeZone = timeZoneInfo;
					}
					result = timeZoneInfo;
				}
				return result;
			}

			public TimeZoneInfo Utc
			{
				get
				{
					TimeZoneInfo timeZoneInfo = this.m_utcTimeZone;
					if (timeZoneInfo == null)
					{
						timeZoneInfo = this.CreateUtc();
					}
					return timeZoneInfo;
				}
			}

			public DateTimeKind GetCorrespondingKind(TimeZoneInfo timeZone)
			{
				DateTimeKind result;
				if (timeZone == this.m_utcTimeZone)
				{
					result = DateTimeKind.Utc;
				}
				else if (timeZone == this.m_localTimeZone)
				{
					result = DateTimeKind.Local;
				}
				else
				{
					result = DateTimeKind.Unspecified;
				}
				return result;
			}

			[SecuritySafeCritical]
			private static TimeZoneInfo GetCurrentOneYearLocal()
			{
				Win32Native.TimeZoneInformation timeZoneInformation = default(Win32Native.TimeZoneInformation);
				long num = (long)UnsafeNativeMethods.GetTimeZoneInformation(out timeZoneInformation);
				TimeZoneInfo result;
				if (num == -1L)
				{
					result = TimeZoneInfo.CreateCustomTimeZone("Local", TimeSpan.Zero, "Local", "Local");
				}
				else
				{
					result = TimeZoneInfo.GetLocalTimeZoneFromWin32Data(timeZoneInformation, false);
				}
				return result;
			}

			public TimeZoneInfo.OffsetAndRule GetOneYearLocalFromUtc(int year)
			{
				TimeZoneInfo.OffsetAndRule offsetAndRule = this.m_oneYearLocalFromUtc;
				if (offsetAndRule == null || offsetAndRule.year != year)
				{
					TimeZoneInfo currentOneYearLocal = TimeZoneInfo.CachedData.GetCurrentOneYearLocal();
					TimeZoneInfo.AdjustmentRule rule = (currentOneYearLocal.m_adjustmentRules == null) ? null : currentOneYearLocal.m_adjustmentRules[0];
					offsetAndRule = new TimeZoneInfo.OffsetAndRule(year, currentOneYearLocal.BaseUtcOffset, rule);
					this.m_oneYearLocalFromUtc = offsetAndRule;
				}
				return offsetAndRule;
			}

			private volatile TimeZoneInfo m_localTimeZone;

			private volatile TimeZoneInfo m_utcTimeZone;

			public Dictionary<string, TimeZoneInfo> m_systemTimeZones;

			public ReadOnlyCollection<TimeZoneInfo> m_readOnlySystemTimeZones;

			public bool m_allSystemTimeZonesRead;

			private volatile TimeZoneInfo.OffsetAndRule m_oneYearLocalFromUtc;
		}

		private class OffsetAndRule
		{
			public OffsetAndRule(int year, TimeSpan offset, TimeZoneInfo.AdjustmentRule rule)
			{
				this.year = year;
				this.offset = offset;
				this.rule = rule;
			}

			public int year;

			public TimeSpan offset;

			public TimeZoneInfo.AdjustmentRule rule;
		}

		[TypeForwardedFrom("System.Core, Version=3.5.0.0, Culture=Neutral, PublicKeyToken=b77a5c561934e089")]
		[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
		[Serializable]
		public sealed class AdjustmentRule : IEquatable<TimeZoneInfo.AdjustmentRule>, ISerializable, IDeserializationCallback
		{
			public DateTime DateStart
			{
				get
				{
					return this.m_dateStart;
				}
			}

			public DateTime DateEnd
			{
				get
				{
					return this.m_dateEnd;
				}
			}

			public TimeSpan DaylightDelta
			{
				get
				{
					return this.m_daylightDelta;
				}
			}

			public TimeZoneInfo.TransitionTime DaylightTransitionStart
			{
				get
				{
					return this.m_daylightTransitionStart;
				}
			}

			public TimeZoneInfo.TransitionTime DaylightTransitionEnd
			{
				get
				{
					return this.m_daylightTransitionEnd;
				}
			}

			internal TimeSpan BaseUtcOffsetDelta
			{
				get
				{
					return this.m_baseUtcOffsetDelta;
				}
			}

			internal bool HasDaylightSaving
			{
				get
				{
					return this.DaylightDelta != TimeSpan.Zero || this.DaylightTransitionStart.TimeOfDay != DateTime.MinValue || this.DaylightTransitionEnd.TimeOfDay != DateTime.MinValue.AddMilliseconds(1.0);
				}
			}

			public bool Equals(TimeZoneInfo.AdjustmentRule other)
			{
				bool flag = other != null && this.m_dateStart == other.m_dateStart && this.m_dateEnd == other.m_dateEnd && this.m_daylightDelta == other.m_daylightDelta && this.m_baseUtcOffsetDelta == other.m_baseUtcOffsetDelta;
				return flag && this.m_daylightTransitionEnd.Equals(other.m_daylightTransitionEnd) && this.m_daylightTransitionStart.Equals(other.m_daylightTransitionStart);
			}

			public override int GetHashCode()
			{
				return this.m_dateStart.GetHashCode();
			}

			private AdjustmentRule()
			{
			}

			public static TimeZoneInfo.AdjustmentRule CreateAdjustmentRule(DateTime dateStart, DateTime dateEnd, TimeSpan daylightDelta, TimeZoneInfo.TransitionTime daylightTransitionStart, TimeZoneInfo.TransitionTime daylightTransitionEnd)
			{
				TimeZoneInfo.AdjustmentRule.ValidateAdjustmentRule(dateStart, dateEnd, daylightDelta, daylightTransitionStart, daylightTransitionEnd);
				return new TimeZoneInfo.AdjustmentRule
				{
					m_dateStart = dateStart,
					m_dateEnd = dateEnd,
					m_daylightDelta = daylightDelta,
					m_daylightTransitionStart = daylightTransitionStart,
					m_daylightTransitionEnd = daylightTransitionEnd,
					m_baseUtcOffsetDelta = TimeSpan.Zero
				};
			}

			internal static TimeZoneInfo.AdjustmentRule CreateAdjustmentRule(DateTime dateStart, DateTime dateEnd, TimeSpan daylightDelta, TimeZoneInfo.TransitionTime daylightTransitionStart, TimeZoneInfo.TransitionTime daylightTransitionEnd, TimeSpan baseUtcOffsetDelta)
			{
				TimeZoneInfo.AdjustmentRule adjustmentRule = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateStart, dateEnd, daylightDelta, daylightTransitionStart, daylightTransitionEnd);
				adjustmentRule.m_baseUtcOffsetDelta = baseUtcOffsetDelta;
				return adjustmentRule;
			}

			internal bool IsStartDateMarkerForBeginningOfYear()
			{
				return this.DaylightTransitionStart.Month == 1 && this.DaylightTransitionStart.Day == 1 && this.DaylightTransitionStart.TimeOfDay.Hour == 0 && this.DaylightTransitionStart.TimeOfDay.Minute == 0 && this.DaylightTransitionStart.TimeOfDay.Second == 0 && this.m_dateStart.Year == this.m_dateEnd.Year;
			}

			internal bool IsEndDateMarkerForEndOfYear()
			{
				return this.DaylightTransitionEnd.Month == 1 && this.DaylightTransitionEnd.Day == 1 && this.DaylightTransitionEnd.TimeOfDay.Hour == 0 && this.DaylightTransitionEnd.TimeOfDay.Minute == 0 && this.DaylightTransitionEnd.TimeOfDay.Second == 0 && this.m_dateStart.Year == this.m_dateEnd.Year;
			}

			private static void ValidateAdjustmentRule(DateTime dateStart, DateTime dateEnd, TimeSpan daylightDelta, TimeZoneInfo.TransitionTime daylightTransitionStart, TimeZoneInfo.TransitionTime daylightTransitionEnd)
			{
				if (dateStart.Kind != DateTimeKind.Unspecified)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_DateTimeKindMustBeUnspecified"), "dateStart");
				}
				if (dateEnd.Kind != DateTimeKind.Unspecified)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_DateTimeKindMustBeUnspecified"), "dateEnd");
				}
				if (daylightTransitionStart.Equals(daylightTransitionEnd))
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_TransitionTimesAreIdentical"), "daylightTransitionEnd");
				}
				if (dateStart > dateEnd)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_OutOfOrderDateTimes"), "dateStart");
				}
				if (TimeZoneInfo.UtcOffsetOutOfRange(daylightDelta))
				{
					throw new ArgumentOutOfRangeException("daylightDelta", daylightDelta, Environment.GetResourceString("ArgumentOutOfRange_UtcOffset"));
				}
				if (daylightDelta.Ticks % 600000000L != 0L)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_TimeSpanHasSeconds"), "daylightDelta");
				}
				if (dateStart.TimeOfDay != TimeSpan.Zero)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_DateTimeHasTimeOfDay"), "dateStart");
				}
				if (dateEnd.TimeOfDay != TimeSpan.Zero)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_DateTimeHasTimeOfDay"), "dateEnd");
				}
			}

			void IDeserializationCallback.OnDeserialization(object sender)
			{
				try
				{
					TimeZoneInfo.AdjustmentRule.ValidateAdjustmentRule(this.m_dateStart, this.m_dateEnd, this.m_daylightDelta, this.m_daylightTransitionStart, this.m_daylightTransitionEnd);
				}
				catch (ArgumentException innerException)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"), innerException);
				}
			}

			[SecurityCritical]
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("DateStart", this.m_dateStart);
				info.AddValue("DateEnd", this.m_dateEnd);
				info.AddValue("DaylightDelta", this.m_daylightDelta);
				info.AddValue("DaylightTransitionStart", this.m_daylightTransitionStart);
				info.AddValue("DaylightTransitionEnd", this.m_daylightTransitionEnd);
				info.AddValue("BaseUtcOffsetDelta", this.m_baseUtcOffsetDelta);
			}

			private AdjustmentRule(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				this.m_dateStart = (DateTime)info.GetValue("DateStart", typeof(DateTime));
				this.m_dateEnd = (DateTime)info.GetValue("DateEnd", typeof(DateTime));
				this.m_daylightDelta = (TimeSpan)info.GetValue("DaylightDelta", typeof(TimeSpan));
				this.m_daylightTransitionStart = (TimeZoneInfo.TransitionTime)info.GetValue("DaylightTransitionStart", typeof(TimeZoneInfo.TransitionTime));
				this.m_daylightTransitionEnd = (TimeZoneInfo.TransitionTime)info.GetValue("DaylightTransitionEnd", typeof(TimeZoneInfo.TransitionTime));
				object valueNoThrow = info.GetValueNoThrow("BaseUtcOffsetDelta", typeof(TimeSpan));
				if (valueNoThrow != null)
				{
					this.m_baseUtcOffsetDelta = (TimeSpan)valueNoThrow;
				}
			}

			private DateTime m_dateStart;

			private DateTime m_dateEnd;

			private TimeSpan m_daylightDelta;

			private TimeZoneInfo.TransitionTime m_daylightTransitionStart;

			private TimeZoneInfo.TransitionTime m_daylightTransitionEnd;

			private TimeSpan m_baseUtcOffsetDelta;
		}

		[TypeForwardedFrom("System.Core, Version=3.5.0.0, Culture=Neutral, PublicKeyToken=b77a5c561934e089")]
		[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
		[Serializable]
		public struct TransitionTime : IEquatable<TimeZoneInfo.TransitionTime>, ISerializable, IDeserializationCallback
		{
			public DateTime TimeOfDay
			{
				get
				{
					return this.m_timeOfDay;
				}
			}

			public int Month
			{
				get
				{
					return (int)this.m_month;
				}
			}

			public int Week
			{
				get
				{
					return (int)this.m_week;
				}
			}

			public int Day
			{
				get
				{
					return (int)this.m_day;
				}
			}

			public DayOfWeek DayOfWeek
			{
				get
				{
					return this.m_dayOfWeek;
				}
			}

			public bool IsFixedDateRule
			{
				get
				{
					return this.m_isFixedDateRule;
				}
			}

			public override bool Equals(object obj)
			{
				return obj is TimeZoneInfo.TransitionTime && this.Equals((TimeZoneInfo.TransitionTime)obj);
			}

			public static bool operator ==(TimeZoneInfo.TransitionTime t1, TimeZoneInfo.TransitionTime t2)
			{
				return t1.Equals(t2);
			}

			public static bool operator !=(TimeZoneInfo.TransitionTime t1, TimeZoneInfo.TransitionTime t2)
			{
				return !t1.Equals(t2);
			}

			public bool Equals(TimeZoneInfo.TransitionTime other)
			{
				bool flag = this.m_isFixedDateRule == other.m_isFixedDateRule && this.m_timeOfDay == other.m_timeOfDay && this.m_month == other.m_month;
				if (flag)
				{
					if (other.m_isFixedDateRule)
					{
						flag = (this.m_day == other.m_day);
					}
					else
					{
						flag = (this.m_week == other.m_week && this.m_dayOfWeek == other.m_dayOfWeek);
					}
				}
				return flag;
			}

			public override int GetHashCode()
			{
				return (int)this.m_month ^ (int)this.m_week << 8;
			}

			public static TimeZoneInfo.TransitionTime CreateFixedDateRule(DateTime timeOfDay, int month, int day)
			{
				return TimeZoneInfo.TransitionTime.CreateTransitionTime(timeOfDay, month, 1, day, DayOfWeek.Sunday, true);
			}

			public static TimeZoneInfo.TransitionTime CreateFloatingDateRule(DateTime timeOfDay, int month, int week, DayOfWeek dayOfWeek)
			{
				return TimeZoneInfo.TransitionTime.CreateTransitionTime(timeOfDay, month, week, 1, dayOfWeek, false);
			}

			private static TimeZoneInfo.TransitionTime CreateTransitionTime(DateTime timeOfDay, int month, int week, int day, DayOfWeek dayOfWeek, bool isFixedDateRule)
			{
				TimeZoneInfo.TransitionTime.ValidateTransitionTime(timeOfDay, month, week, day, dayOfWeek);
				return new TimeZoneInfo.TransitionTime
				{
					m_isFixedDateRule = isFixedDateRule,
					m_timeOfDay = timeOfDay,
					m_dayOfWeek = dayOfWeek,
					m_day = (byte)day,
					m_week = (byte)week,
					m_month = (byte)month
				};
			}

			private static void ValidateTransitionTime(DateTime timeOfDay, int month, int week, int day, DayOfWeek dayOfWeek)
			{
				if (timeOfDay.Kind != DateTimeKind.Unspecified)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_DateTimeKindMustBeUnspecified"), "timeOfDay");
				}
				if (month < 1 || month > 12)
				{
					throw new ArgumentOutOfRangeException("month", Environment.GetResourceString("ArgumentOutOfRange_MonthParam"));
				}
				if (day < 1 || day > 31)
				{
					throw new ArgumentOutOfRangeException("day", Environment.GetResourceString("ArgumentOutOfRange_DayParam"));
				}
				if (week < 1 || week > 5)
				{
					throw new ArgumentOutOfRangeException("week", Environment.GetResourceString("ArgumentOutOfRange_Week"));
				}
				if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
				{
					throw new ArgumentOutOfRangeException("dayOfWeek", Environment.GetResourceString("ArgumentOutOfRange_DayOfWeek"));
				}
				int num;
				int num2;
				int num3;
				timeOfDay.GetDatePart(out num, out num2, out num3);
				if (num != 1 || num2 != 1 || num3 != 1 || timeOfDay.Ticks % 10000L != 0L)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_DateTimeHasTicks"), "timeOfDay");
				}
			}

			void IDeserializationCallback.OnDeserialization(object sender)
			{
				try
				{
					TimeZoneInfo.TransitionTime.ValidateTransitionTime(this.m_timeOfDay, (int)this.m_month, (int)this.m_week, (int)this.m_day, this.m_dayOfWeek);
				}
				catch (ArgumentException innerException)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"), innerException);
				}
			}

			[SecurityCritical]
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("TimeOfDay", this.m_timeOfDay);
				info.AddValue("Month", this.m_month);
				info.AddValue("Week", this.m_week);
				info.AddValue("Day", this.m_day);
				info.AddValue("DayOfWeek", this.m_dayOfWeek);
				info.AddValue("IsFixedDateRule", this.m_isFixedDateRule);
			}

			private TransitionTime(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				this.m_timeOfDay = (DateTime)info.GetValue("TimeOfDay", typeof(DateTime));
				this.m_month = (byte)info.GetValue("Month", typeof(byte));
				this.m_week = (byte)info.GetValue("Week", typeof(byte));
				this.m_day = (byte)info.GetValue("Day", typeof(byte));
				this.m_dayOfWeek = (DayOfWeek)info.GetValue("DayOfWeek", typeof(DayOfWeek));
				this.m_isFixedDateRule = (bool)info.GetValue("IsFixedDateRule", typeof(bool));
			}

			private DateTime m_timeOfDay;

			private byte m_month;

			private byte m_week;

			private byte m_day;

			private DayOfWeek m_dayOfWeek;

			private bool m_isFixedDateRule;
		}

		private sealed class StringSerializer
		{
			public static string GetSerializedString(TimeZoneInfo zone)
			{
				StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
				stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(zone.Id));
				stringBuilder.Append(';');
				stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(zone.BaseUtcOffset.TotalMinutes.ToString(CultureInfo.InvariantCulture)));
				stringBuilder.Append(';');
				stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(zone.DisplayName));
				stringBuilder.Append(';');
				stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(zone.StandardName));
				stringBuilder.Append(';');
				stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(zone.DaylightName));
				stringBuilder.Append(';');
				TimeZoneInfo.AdjustmentRule[] adjustmentRules = zone.GetAdjustmentRules();
				if (adjustmentRules != null && adjustmentRules.Length != 0)
				{
					foreach (TimeZoneInfo.AdjustmentRule adjustmentRule in adjustmentRules)
					{
						stringBuilder.Append('[');
						stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(adjustmentRule.DateStart.ToString("MM:dd:yyyy", DateTimeFormatInfo.InvariantInfo)));
						stringBuilder.Append(';');
						stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(adjustmentRule.DateEnd.ToString("MM:dd:yyyy", DateTimeFormatInfo.InvariantInfo)));
						stringBuilder.Append(';');
						stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(adjustmentRule.DaylightDelta.TotalMinutes.ToString(CultureInfo.InvariantCulture)));
						stringBuilder.Append(';');
						TimeZoneInfo.StringSerializer.SerializeTransitionTime(adjustmentRule.DaylightTransitionStart, stringBuilder);
						stringBuilder.Append(';');
						TimeZoneInfo.StringSerializer.SerializeTransitionTime(adjustmentRule.DaylightTransitionEnd, stringBuilder);
						stringBuilder.Append(';');
						if (adjustmentRule.BaseUtcOffsetDelta != TimeSpan.Zero)
						{
							stringBuilder.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(adjustmentRule.BaseUtcOffsetDelta.TotalMinutes.ToString(CultureInfo.InvariantCulture)));
							stringBuilder.Append(';');
						}
						stringBuilder.Append(']');
					}
				}
				stringBuilder.Append(';');
				return StringBuilderCache.GetStringAndRelease(stringBuilder);
			}

			public static TimeZoneInfo GetDeserializedTimeZoneInfo(string source)
			{
				TimeZoneInfo.StringSerializer stringSerializer = new TimeZoneInfo.StringSerializer(source);
				string nextStringValue = stringSerializer.GetNextStringValue(false);
				TimeSpan nextTimeSpanValue = stringSerializer.GetNextTimeSpanValue(false);
				string nextStringValue2 = stringSerializer.GetNextStringValue(false);
				string nextStringValue3 = stringSerializer.GetNextStringValue(false);
				string nextStringValue4 = stringSerializer.GetNextStringValue(false);
				TimeZoneInfo.AdjustmentRule[] nextAdjustmentRuleArrayValue = stringSerializer.GetNextAdjustmentRuleArrayValue(false);
				TimeZoneInfo result;
				try
				{
					result = TimeZoneInfo.CreateCustomTimeZone(nextStringValue, nextTimeSpanValue, nextStringValue2, nextStringValue3, nextStringValue4, nextAdjustmentRuleArrayValue);
				}
				catch (ArgumentException innerException)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"), innerException);
				}
				catch (InvalidTimeZoneException innerException2)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"), innerException2);
				}
				return result;
			}

			private StringSerializer(string str)
			{
				this.m_serializedText = str;
				this.m_state = TimeZoneInfo.StringSerializer.State.StartOfToken;
			}

			private static string SerializeSubstitute(string text)
			{
				text = text.Replace("\\", "\\\\");
				text = text.Replace("[", "\\[");
				text = text.Replace("]", "\\]");
				return text.Replace(";", "\\;");
			}

			private static void SerializeTransitionTime(TimeZoneInfo.TransitionTime time, StringBuilder serializedText)
			{
				serializedText.Append('[');
				serializedText.Append((time.IsFixedDateRule ? 1 : 0).ToString(CultureInfo.InvariantCulture));
				serializedText.Append(';');
				if (time.IsFixedDateRule)
				{
					serializedText.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(time.TimeOfDay.ToString("HH:mm:ss.FFF", DateTimeFormatInfo.InvariantInfo)));
					serializedText.Append(';');
					serializedText.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(time.Month.ToString(CultureInfo.InvariantCulture)));
					serializedText.Append(';');
					serializedText.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(time.Day.ToString(CultureInfo.InvariantCulture)));
					serializedText.Append(';');
				}
				else
				{
					serializedText.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(time.TimeOfDay.ToString("HH:mm:ss.FFF", DateTimeFormatInfo.InvariantInfo)));
					serializedText.Append(';');
					serializedText.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(time.Month.ToString(CultureInfo.InvariantCulture)));
					serializedText.Append(';');
					serializedText.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(time.Week.ToString(CultureInfo.InvariantCulture)));
					serializedText.Append(';');
					serializedText.Append(TimeZoneInfo.StringSerializer.SerializeSubstitute(((int)time.DayOfWeek).ToString(CultureInfo.InvariantCulture)));
					serializedText.Append(';');
				}
				serializedText.Append(']');
			}

			private static void VerifyIsEscapableCharacter(char c)
			{
				if (c != '\\' && c != ';' && c != '[' && c != ']')
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidEscapeSequence", new object[]
					{
						c
					}));
				}
			}

			private void SkipVersionNextDataFields(int depth)
			{
				if (this.m_currentTokenStartIndex < 0 || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
				}
				TimeZoneInfo.StringSerializer.State state = TimeZoneInfo.StringSerializer.State.NotEscaped;
				for (int i = this.m_currentTokenStartIndex; i < this.m_serializedText.Length; i++)
				{
					if (state == TimeZoneInfo.StringSerializer.State.Escaped)
					{
						TimeZoneInfo.StringSerializer.VerifyIsEscapableCharacter(this.m_serializedText[i]);
						state = TimeZoneInfo.StringSerializer.State.NotEscaped;
					}
					else if (state == TimeZoneInfo.StringSerializer.State.NotEscaped)
					{
						char c = this.m_serializedText[i];
						if (c == '\0')
						{
							throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
						}
						switch (c)
						{
						case '[':
							depth++;
							break;
						case '\\':
							state = TimeZoneInfo.StringSerializer.State.Escaped;
							break;
						case ']':
							depth--;
							if (depth == 0)
							{
								this.m_currentTokenStartIndex = i + 1;
								if (this.m_currentTokenStartIndex >= this.m_serializedText.Length)
								{
									this.m_state = TimeZoneInfo.StringSerializer.State.EndOfLine;
									return;
								}
								this.m_state = TimeZoneInfo.StringSerializer.State.StartOfToken;
								return;
							}
							break;
						}
					}
				}
				throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
			}

			private string GetNextStringValue(bool canEndWithoutSeparator)
			{
				if (this.m_state == TimeZoneInfo.StringSerializer.State.EndOfLine)
				{
					if (canEndWithoutSeparator)
					{
						return null;
					}
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
				}
				else
				{
					if (this.m_currentTokenStartIndex < 0 || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
					{
						throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
					}
					TimeZoneInfo.StringSerializer.State state = TimeZoneInfo.StringSerializer.State.NotEscaped;
					StringBuilder stringBuilder = StringBuilderCache.Acquire(64);
					for (int i = this.m_currentTokenStartIndex; i < this.m_serializedText.Length; i++)
					{
						if (state == TimeZoneInfo.StringSerializer.State.Escaped)
						{
							TimeZoneInfo.StringSerializer.VerifyIsEscapableCharacter(this.m_serializedText[i]);
							stringBuilder.Append(this.m_serializedText[i]);
							state = TimeZoneInfo.StringSerializer.State.NotEscaped;
						}
						else if (state == TimeZoneInfo.StringSerializer.State.NotEscaped)
						{
							char c = this.m_serializedText[i];
							if (c == '\0')
							{
								throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
							}
							if (c == ';')
							{
								this.m_currentTokenStartIndex = i + 1;
								if (this.m_currentTokenStartIndex >= this.m_serializedText.Length)
								{
									this.m_state = TimeZoneInfo.StringSerializer.State.EndOfLine;
								}
								else
								{
									this.m_state = TimeZoneInfo.StringSerializer.State.StartOfToken;
								}
								return StringBuilderCache.GetStringAndRelease(stringBuilder);
							}
							switch (c)
							{
							case '[':
								throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
							case '\\':
								state = TimeZoneInfo.StringSerializer.State.Escaped;
								break;
							case ']':
								if (canEndWithoutSeparator)
								{
									this.m_currentTokenStartIndex = i;
									this.m_state = TimeZoneInfo.StringSerializer.State.StartOfToken;
									return stringBuilder.ToString();
								}
								throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
							default:
								stringBuilder.Append(this.m_serializedText[i]);
								break;
							}
						}
					}
					if (state == TimeZoneInfo.StringSerializer.State.Escaped)
					{
						throw new SerializationException(Environment.GetResourceString("Serialization_InvalidEscapeSequence", new object[]
						{
							string.Empty
						}));
					}
					if (!canEndWithoutSeparator)
					{
						throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
					}
					this.m_currentTokenStartIndex = this.m_serializedText.Length;
					this.m_state = TimeZoneInfo.StringSerializer.State.EndOfLine;
					return StringBuilderCache.GetStringAndRelease(stringBuilder);
				}
			}

			private DateTime GetNextDateTimeValue(bool canEndWithoutSeparator, string format)
			{
				string nextStringValue = this.GetNextStringValue(canEndWithoutSeparator);
				DateTime result;
				if (!DateTime.TryParseExact(nextStringValue, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out result))
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
				}
				return result;
			}

			private TimeSpan GetNextTimeSpanValue(bool canEndWithoutSeparator)
			{
				int nextInt32Value = this.GetNextInt32Value(canEndWithoutSeparator);
				TimeSpan result;
				try
				{
					result = new TimeSpan(0, nextInt32Value, 0);
				}
				catch (ArgumentOutOfRangeException innerException)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"), innerException);
				}
				return result;
			}

			private int GetNextInt32Value(bool canEndWithoutSeparator)
			{
				string nextStringValue = this.GetNextStringValue(canEndWithoutSeparator);
				int result;
				if (!int.TryParse(nextStringValue, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result))
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
				}
				return result;
			}

			private TimeZoneInfo.AdjustmentRule[] GetNextAdjustmentRuleArrayValue(bool canEndWithoutSeparator)
			{
				List<TimeZoneInfo.AdjustmentRule> list = new List<TimeZoneInfo.AdjustmentRule>(1);
				int num = 0;
				for (TimeZoneInfo.AdjustmentRule nextAdjustmentRuleValue = this.GetNextAdjustmentRuleValue(true); nextAdjustmentRuleValue != null; nextAdjustmentRuleValue = this.GetNextAdjustmentRuleValue(true))
				{
					list.Add(nextAdjustmentRuleValue);
					num++;
				}
				if (!canEndWithoutSeparator)
				{
					if (this.m_state == TimeZoneInfo.StringSerializer.State.EndOfLine)
					{
						throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
					}
					if (this.m_currentTokenStartIndex < 0 || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
					{
						throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
					}
				}
				if (num == 0)
				{
					return null;
				}
				return list.ToArray();
			}

			private TimeZoneInfo.AdjustmentRule GetNextAdjustmentRuleValue(bool canEndWithoutSeparator)
			{
				if (this.m_state == TimeZoneInfo.StringSerializer.State.EndOfLine)
				{
					if (canEndWithoutSeparator)
					{
						return null;
					}
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
				}
				else
				{
					if (this.m_currentTokenStartIndex < 0 || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
					{
						throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
					}
					if (this.m_serializedText[this.m_currentTokenStartIndex] == ';')
					{
						return null;
					}
					if (this.m_serializedText[this.m_currentTokenStartIndex] != '[')
					{
						throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
					}
					this.m_currentTokenStartIndex++;
					DateTime nextDateTimeValue = this.GetNextDateTimeValue(false, "MM:dd:yyyy");
					DateTime nextDateTimeValue2 = this.GetNextDateTimeValue(false, "MM:dd:yyyy");
					TimeSpan nextTimeSpanValue = this.GetNextTimeSpanValue(false);
					TimeZoneInfo.TransitionTime nextTransitionTimeValue = this.GetNextTransitionTimeValue(false);
					TimeZoneInfo.TransitionTime nextTransitionTimeValue2 = this.GetNextTransitionTimeValue(false);
					TimeSpan baseUtcOffsetDelta = TimeSpan.Zero;
					if (this.m_state == TimeZoneInfo.StringSerializer.State.EndOfLine || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
					{
						throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
					}
					if ((this.m_serializedText[this.m_currentTokenStartIndex] >= '0' && this.m_serializedText[this.m_currentTokenStartIndex] <= '9') || this.m_serializedText[this.m_currentTokenStartIndex] == '-' || this.m_serializedText[this.m_currentTokenStartIndex] == '+')
					{
						baseUtcOffsetDelta = this.GetNextTimeSpanValue(false);
					}
					if (this.m_state == TimeZoneInfo.StringSerializer.State.EndOfLine || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
					{
						throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
					}
					if (this.m_serializedText[this.m_currentTokenStartIndex] != ']')
					{
						this.SkipVersionNextDataFields(1);
					}
					else
					{
						this.m_currentTokenStartIndex++;
					}
					TimeZoneInfo.AdjustmentRule result;
					try
					{
						result = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(nextDateTimeValue, nextDateTimeValue2, nextTimeSpanValue, nextTransitionTimeValue, nextTransitionTimeValue2, baseUtcOffsetDelta);
					}
					catch (ArgumentException innerException)
					{
						throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"), innerException);
					}
					if (this.m_currentTokenStartIndex >= this.m_serializedText.Length)
					{
						this.m_state = TimeZoneInfo.StringSerializer.State.EndOfLine;
					}
					else
					{
						this.m_state = TimeZoneInfo.StringSerializer.State.StartOfToken;
					}
					return result;
				}
			}

			private TimeZoneInfo.TransitionTime GetNextTransitionTimeValue(bool canEndWithoutSeparator)
			{
				if (this.m_state == TimeZoneInfo.StringSerializer.State.EndOfLine || (this.m_currentTokenStartIndex < this.m_serializedText.Length && this.m_serializedText[this.m_currentTokenStartIndex] == ']'))
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
				}
				if (this.m_currentTokenStartIndex < 0 || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
				}
				if (this.m_serializedText[this.m_currentTokenStartIndex] != '[')
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
				}
				this.m_currentTokenStartIndex++;
				int nextInt32Value = this.GetNextInt32Value(false);
				if (nextInt32Value != 0 && nextInt32Value != 1)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
				}
				DateTime nextDateTimeValue = this.GetNextDateTimeValue(false, "HH:mm:ss.FFF");
				nextDateTimeValue = new DateTime(1, 1, 1, nextDateTimeValue.Hour, nextDateTimeValue.Minute, nextDateTimeValue.Second, nextDateTimeValue.Millisecond);
				int nextInt32Value2 = this.GetNextInt32Value(false);
				TimeZoneInfo.TransitionTime result;
				if (nextInt32Value == 1)
				{
					int nextInt32Value3 = this.GetNextInt32Value(false);
					try
					{
						result = TimeZoneInfo.TransitionTime.CreateFixedDateRule(nextDateTimeValue, nextInt32Value2, nextInt32Value3);
						goto IL_15B;
					}
					catch (ArgumentException innerException)
					{
						throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"), innerException);
					}
				}
				int nextInt32Value4 = this.GetNextInt32Value(false);
				int nextInt32Value5 = this.GetNextInt32Value(false);
				try
				{
					result = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(nextDateTimeValue, nextInt32Value2, nextInt32Value4, (DayOfWeek)nextInt32Value5);
				}
				catch (ArgumentException innerException2)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"), innerException2);
				}
				IL_15B:
				if (this.m_state == TimeZoneInfo.StringSerializer.State.EndOfLine || this.m_currentTokenStartIndex >= this.m_serializedText.Length)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
				}
				if (this.m_serializedText[this.m_currentTokenStartIndex] != ']')
				{
					this.SkipVersionNextDataFields(1);
				}
				else
				{
					this.m_currentTokenStartIndex++;
				}
				bool flag = false;
				if (this.m_currentTokenStartIndex < this.m_serializedText.Length && this.m_serializedText[this.m_currentTokenStartIndex] == ';')
				{
					this.m_currentTokenStartIndex++;
					flag = true;
				}
				if (!flag && !canEndWithoutSeparator)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_InvalidData"));
				}
				if (this.m_currentTokenStartIndex >= this.m_serializedText.Length)
				{
					this.m_state = TimeZoneInfo.StringSerializer.State.EndOfLine;
				}
				else
				{
					this.m_state = TimeZoneInfo.StringSerializer.State.StartOfToken;
				}
				return result;
			}

			private string m_serializedText;

			private int m_currentTokenStartIndex;

			private TimeZoneInfo.StringSerializer.State m_state;

			private const int initialCapacityForString = 64;

			private const char esc = '\\';

			private const char sep = ';';

			private const char lhs = '[';

			private const char rhs = ']';

			private const string escString = "\\";

			private const string sepString = ";";

			private const string lhsString = "[";

			private const string rhsString = "]";

			private const string escapedEsc = "\\\\";

			private const string escapedSep = "\\;";

			private const string escapedLhs = "\\[";

			private const string escapedRhs = "\\]";

			private const string dateTimeFormat = "MM:dd:yyyy";

			private const string timeOfDayFormat = "HH:mm:ss.FFF";

			private enum State
			{
				Escaped,
				NotEscaped,
				StartOfToken,
				EndOfLine
			}
		}

		private class TimeZoneInfoComparer : IComparer<TimeZoneInfo>
		{
			int IComparer<TimeZoneInfo>.Compare(TimeZoneInfo x, TimeZoneInfo y)
			{
				int num = x.BaseUtcOffset.CompareTo(y.BaseUtcOffset);
				if (num != 0)
				{
					return num;
				}
				return string.Compare(x.DisplayName, y.DisplayName, StringComparison.Ordinal);
			}
		}
	}
}
