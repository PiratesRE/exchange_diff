using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class CrashData
	{
		public CrashData(int count, DateTime time)
		{
			this.count = count;
			this.time = time;
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public DateTime Time
		{
			get
			{
				return this.time;
			}
		}

		public static CrashData Read(RegistryKey parentKey, string subKeyName)
		{
			CrashData result;
			using (RegistryKey registryKey = parentKey.OpenSubKey(subKeyName))
			{
				try
				{
					DateTime dateTime = new DateTime(Util.ReadRegistryLong(registryKey, "CrashTime"));
					object value = registryKey.GetValue("CrashCount");
					if (!(value is int))
					{
						result = null;
					}
					else
					{
						result = new CrashData((int)value, dateTime);
					}
				}
				catch (FormatException)
				{
					result = null;
				}
				catch (OverflowException)
				{
					result = null;
				}
				catch (ArgumentOutOfRangeException)
				{
					result = null;
				}
			}
			return result;
		}

		public static void Write(RegistryKey parentKey, string subKeyName, int crashCount)
		{
			using (RegistryKey registryKey = parentKey.CreateSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree))
			{
				registryKey.SetValue("CrashCount", crashCount);
				Util.WriteRegistryLong(registryKey, "CrashTime", DateTime.UtcNow.Ticks);
			}
		}

		private const string RegistryNameCrashCount = "CrashCount";

		private const string RegistryNameCrashTime = "CrashTime";

		private int count;

		private DateTime time;
	}
}
