using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Common
{
	internal class Canary15
	{
		public Canary15(string logonUniqueKey)
		{
			byte[] userContextIdBinary = Guid.NewGuid().ToByteArray();
			byte[] bytes = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
			long keyIndex;
			int segmentIndex;
			byte[] hashBinary = Canary15DataManager.ComputeHash(userContextIdBinary, bytes, logonUniqueKey, out keyIndex, out segmentIndex);
			this.Init(userContextIdBinary, bytes, logonUniqueKey, hashBinary, Canary15.FormatLogData(keyIndex, segmentIndex));
			this.IsRenewed = true;
			this.IsAboutToExpire = false;
		}

		private Canary15(byte[] userContextIdBinary, byte[] timeStampBinary, string logonUniqueKey, byte[] hashBinary, string logData)
		{
			this.Init(userContextIdBinary, timeStampBinary, logonUniqueKey, hashBinary, logData);
		}

		public string UserContextId { get; private set; }

		public string LogonUniqueKey { get; private set; }

		internal bool IsRenewed { get; private set; }

		internal bool IsAboutToExpire { get; private set; }

		internal DateTime CreationTime { get; private set; }

		internal string LogData { get; private set; }

		public static Canary15 RestoreCanary15(string canaryString, string logonUniqueKey)
		{
			byte[] userContextIdBinary;
			byte[] array;
			byte[] array2;
			if (Canary15.ParseCanary15(canaryString, out userContextIdBinary, out array, out array2))
			{
				if (Canary15.IsExpired(array))
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(5L, "Canary is expired, timeStampBinary={0}", Canary15.GetHexString(array));
					return null;
				}
				long keyIndex;
				int segmentIndex;
				byte[] array3 = Canary15DataManager.ComputeHash(userContextIdBinary, array, logonUniqueKey, out keyIndex, out segmentIndex);
				if (Canary15.AreEqual(array3, array2))
				{
					return new Canary15(userContextIdBinary, array, logonUniqueKey, array2, Canary15.FormatLogData(keyIndex, segmentIndex));
				}
				ExTraceGlobals.CoreTracer.TraceDebug<string, string>(5L, "testHashBinary={0}!=hashBinary={1}", Canary15.GetHexString(array3), Canary15.GetHexString(array2));
			}
			ExTraceGlobals.CoreTracer.TraceDebug<string, string>(5L, "RestoreCanary failed, logonUniqueKey={0}, canaryString={1}", logonUniqueKey, canaryString);
			return null;
		}

		public static bool ValidateCanary15(string canaryString, string logonUniqueKey)
		{
			byte[] userContextIdBinary;
			byte[] array;
			byte[] array2;
			if (!Canary15.ParseCanary15(canaryString, out userContextIdBinary, out array, out array2))
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(10L, "ValidateCanary failed, canaryString={0}", canaryString);
				return false;
			}
			if (Canary15.IsExpired(array))
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(10L, "Canary is expired, timeStampBinary={0}", Canary15.GetHexString(array));
				return false;
			}
			long num;
			int num2;
			byte[] array3 = Canary15DataManager.ComputeHash(userContextIdBinary, array, logonUniqueKey, out num, out num2);
			if (Canary15.AreEqual(array2, array3))
			{
				return true;
			}
			ExTraceGlobals.CoreTracer.TraceDebug<string, string>(10L, "testHashBinary={0}!=hashBinary={1}", Canary15.GetHexString(array3), Canary15.GetHexString(array2));
			return false;
		}

		public override string ToString()
		{
			return this.canaryString;
		}

		private static string GetHexString(byte[] bytes)
		{
			if (!ExTraceGlobals.CoreTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return null;
			}
			if (bytes == null)
			{
				return "NULL_BYTE_ARRAY";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in bytes)
			{
				stringBuilder.AppendFormat("{0:x2}", b);
			}
			return stringBuilder.ToString();
		}

		private static bool IsExpired(byte[] timeStampBinary)
		{
			long num = BitConverter.ToInt64(timeStampBinary, 0);
			long ticks = DateTime.UtcNow.Ticks;
			if (num + 864000000000L < ticks)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<long, long, long>(3L, "timeStamp{0}+timeStampLifetime{1} < utcNow={2}", num, 864000000000L, ticks);
				return true;
			}
			return false;
		}

		private static bool IsNearExpiration(byte[] timeStampBinary)
		{
			long num = BitConverter.ToInt64(timeStampBinary, 0);
			long ticks = DateTime.UtcNow.Ticks;
			if (num + 36000000000L < ticks)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<long, long, long>(3L, "timeStamp{0}+timeStampHalfLifetime{1} < utcNow={2}", num, 36000000000L, ticks);
				return true;
			}
			return false;
		}

		private static string Encode(byte[] canaryBinary)
		{
			int num = (canaryBinary.Length + 2 - (canaryBinary.Length + 2) % 3) / 3 * 4;
			char[] array = new char[num];
			int num2 = Convert.ToBase64CharArray(canaryBinary, 0, canaryBinary.Length, array, 0);
			for (int i = 0; i < num2; i++)
			{
				char c = array[i];
				if (c != '+')
				{
					if (c != '/')
					{
						if (c == '=')
						{
							array[i] = '.';
						}
					}
					else
					{
						array[i] = '_';
					}
				}
				else
				{
					array[i] = '-';
				}
			}
			return new string(array);
		}

		private static byte[] Decode(string canaryString)
		{
			char[] array = canaryString.ToCharArray();
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				char c = array[i];
				switch (c)
				{
				case '-':
					array[i] = '+';
					break;
				case '.':
					array[i] = '=';
					break;
				default:
					if (c == '_')
					{
						array[i] = '/';
					}
					break;
				}
			}
			return Convert.FromBase64CharArray(array, 0, num);
		}

		private static bool ParseCanary15(string canaryString, out byte[] userContextIdBinary, out byte[] timeStampBinary, out byte[] hashBinary)
		{
			userContextIdBinary = null;
			timeStampBinary = null;
			hashBinary = null;
			if (canaryString == null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(4L, "Canary string is null");
				return false;
			}
			if (canaryString.Length != 76)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<int>(4L, "canaryString.length={0}", canaryString.Length);
				return false;
			}
			byte[] array;
			try
			{
				array = Canary15.Decode(canaryString);
			}
			catch (FormatException ex)
			{
				if (ExTraceGlobals.CoreTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(4L, "Format Exception {0}", ex.ToString());
				}
				return false;
			}
			if (array.Length != 56)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<int, int>(4L, "canaryBinary.Length={0}!=CanaryBinaryLength={1}", array.Length, 56);
				return false;
			}
			userContextIdBinary = new byte[16];
			timeStampBinary = new byte[8];
			hashBinary = new byte[32];
			Array.Copy(array, 0, userContextIdBinary, 0, 16);
			Array.Copy(array, 16, timeStampBinary, 0, 8);
			Array.Copy(array, 24, hashBinary, 0, 32);
			return true;
		}

		private static bool AreEqual(byte[] a, byte[] b)
		{
			if (a == null || b == null || a.Length != b.Length)
			{
				return false;
			}
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			return true;
		}

		private static string FormatLogData(long keyIndex, int segmentIndex)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				keyIndex,
				segmentIndex
			});
		}

		private void Init(byte[] userContextIdBinary, byte[] timeStampBinary, string logonUniqueKey, byte[] hashBinary, string logData)
		{
			byte[] array = new byte[userContextIdBinary.Length + timeStampBinary.Length + hashBinary.Length];
			userContextIdBinary.CopyTo(array, 0);
			timeStampBinary.CopyTo(array, userContextIdBinary.Length);
			hashBinary.CopyTo(array, userContextIdBinary.Length + timeStampBinary.Length);
			this.UserContextId = new Guid(userContextIdBinary).ToString("N");
			this.LogonUniqueKey = logonUniqueKey;
			this.canaryString = Canary15.Encode(array);
			long ticks = BitConverter.ToInt64(timeStampBinary, 0);
			this.CreationTime = new DateTime(ticks, DateTimeKind.Utc);
			this.LogData = logData;
			this.IsRenewed = false;
			this.IsAboutToExpire = Canary15.IsNearExpiration(timeStampBinary);
		}

		public const int CanaryStringLength = 76;

		private const int UserContextIdLength = 16;

		private const int TimeStampLength = 8;

		private const int HashLength = 32;

		private const int CanaryBinaryLength = 56;

		private const long TimeStampHalfLifetime = 36000000000L;

		private const long TimeStampLifetime = 864000000000L;

		private string canaryString;
	}
}
