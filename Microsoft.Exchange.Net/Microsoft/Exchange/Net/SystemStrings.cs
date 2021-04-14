using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.Net
{
	internal static class SystemStrings
	{
		static SystemStrings()
		{
			SystemStrings.stringIDs.Add(4266870920U, "InvalidDaylightTransition");
			SystemStrings.stringIDs.Add(3036516007U, "RegistryKeyDoesNotContainValidTimeZone");
			SystemStrings.stringIDs.Add(1348502081U, "TimeZoneKeyNameTooLong");
			SystemStrings.stringIDs.Add(522392603U, "NoPrivateKey");
			SystemStrings.stringIDs.Add(2126902361U, "InvalidSystemTime");
			SystemStrings.stringIDs.Add(874632872U, "CouldNotAccessPrivateKey");
			SystemStrings.stringIDs.Add(4261244944U, "InvalidNthDayOfWeek");
			SystemStrings.stringIDs.Add(2227579325U, "WrongPrivateKeyType");
			SystemStrings.stringIDs.Add(1181227695U, "PrivateKeyInvalid");
			SystemStrings.stringIDs.Add(1877416323U, "FailedToAddAccessRule");
		}

		public static string InvalidDaylightTransition
		{
			get
			{
				return SystemStrings.ResourceManager.GetString("InvalidDaylightTransition");
			}
		}

		public static string RegistryKeyDoesNotContainValidTimeZone
		{
			get
			{
				return SystemStrings.ResourceManager.GetString("RegistryKeyDoesNotContainValidTimeZone");
			}
		}

		public static string TimeZoneKeyNameTooLong
		{
			get
			{
				return SystemStrings.ResourceManager.GetString("TimeZoneKeyNameTooLong");
			}
		}

		public static string NoPrivateKey
		{
			get
			{
				return SystemStrings.ResourceManager.GetString("NoPrivateKey");
			}
		}

		public static string InvalidRelativeTransitionDay(int day)
		{
			return string.Format(SystemStrings.ResourceManager.GetString("InvalidRelativeTransitionDay"), day);
		}

		public static string UseLocalizableStringResource(string locMethod)
		{
			return string.Format(SystemStrings.ResourceManager.GetString("UseLocalizableStringResource"), locMethod);
		}

		public static string FailedToOpenRegistryKey(string path)
		{
			return string.Format(SystemStrings.ResourceManager.GetString("FailedToOpenRegistryKey"), path);
		}

		public static string InvalidSystemTime
		{
			get
			{
				return SystemStrings.ResourceManager.GetString("InvalidSystemTime");
			}
		}

		public static string InvalidBaseType(Type enumType)
		{
			return string.Format(SystemStrings.ResourceManager.GetString("InvalidBaseType"), enumType);
		}

		public static string InvalidTypeParam(Type enumType)
		{
			return string.Format(SystemStrings.ResourceManager.GetString("InvalidTypeParam"), enumType);
		}

		public static string CouldNotAccessPrivateKey
		{
			get
			{
				return SystemStrings.ResourceManager.GetString("CouldNotAccessPrivateKey");
			}
		}

		public static string BadEnumValue(Type enumType, object invalidValue)
		{
			return string.Format(SystemStrings.ResourceManager.GetString("BadEnumValue"), enumType, invalidValue);
		}

		public static string InvalidNthDayOfWeek
		{
			get
			{
				return SystemStrings.ResourceManager.GetString("InvalidNthDayOfWeek");
			}
		}

		public static string WrongPrivateKeyType
		{
			get
			{
				return SystemStrings.ResourceManager.GetString("WrongPrivateKeyType");
			}
		}

		public static string PrivateKeyInvalid
		{
			get
			{
				return SystemStrings.ResourceManager.GetString("PrivateKeyInvalid");
			}
		}

		public static string Win32GetTimeZoneInformationFailed(int error)
		{
			return string.Format(SystemStrings.ResourceManager.GetString("Win32GetTimeZoneInformationFailed"), error);
		}

		public static string FailedToAddAccessRule
		{
			get
			{
				return SystemStrings.ResourceManager.GetString("FailedToAddAccessRule");
			}
		}

		public static string GetLocalizedString(SystemStrings.IDs key)
		{
			return SystemStrings.ResourceManager.GetString(SystemStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(10);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.Net.SystemStrings", typeof(SystemStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InvalidDaylightTransition = 4266870920U,
			RegistryKeyDoesNotContainValidTimeZone = 3036516007U,
			TimeZoneKeyNameTooLong = 1348502081U,
			NoPrivateKey = 522392603U,
			InvalidSystemTime = 2126902361U,
			CouldNotAccessPrivateKey = 874632872U,
			InvalidNthDayOfWeek = 4261244944U,
			WrongPrivateKeyType = 2227579325U,
			PrivateKeyInvalid = 1181227695U,
			FailedToAddAccessRule = 1877416323U
		}

		private enum ParamIDs
		{
			InvalidRelativeTransitionDay,
			UseLocalizableStringResource,
			FailedToOpenRegistryKey,
			InvalidBaseType,
			InvalidTypeParam,
			BadEnumValue,
			Win32GetTimeZoneInformationFailed
		}
	}
}
