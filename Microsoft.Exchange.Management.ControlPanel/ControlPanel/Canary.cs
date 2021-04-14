using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class Canary
	{
		public string UserContextId { get; private set; }

		public string LogonUniqueKey { get; private set; }

		static Canary()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 585, ".cctor", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\Utilities\\Canary.cs");
			byte[] array = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest().ObjectGuid.ToByteArray();
			byte[] array2 = topologyConfigurationSession.GetDatabasesContainerId().ObjectGuid.ToByteArray();
			Canary.adObjectIdsBinary = new byte[array.Length + array2.Length];
			array.CopyTo(Canary.adObjectIdsBinary, 0);
			array2.CopyTo(Canary.adObjectIdsBinary, array.Length);
		}

		private static byte[] ComputeHash(byte[] userContextIdBinary, byte[] timeStampBinary, string logonUniqueKey)
		{
			int num = 0;
			byte[] bytes = new UnicodeEncoding().GetBytes(logonUniqueKey);
			byte[] array = new byte[userContextIdBinary.Length + timeStampBinary.Length + bytes.Length + Canary.adObjectIdsBinary.Length];
			userContextIdBinary.CopyTo(array, num);
			num += userContextIdBinary.Length;
			timeStampBinary.CopyTo(array, num);
			num += timeStampBinary.Length;
			bytes.CopyTo(array, num);
			num += bytes.Length;
			Canary.adObjectIdsBinary.CopyTo(array, num);
			byte[] result;
			using (SHA256Cng sha256Cng = new SHA256Cng())
			{
				result = sha256Cng.ComputeHash(array);
				sha256Cng.Clear();
			}
			return result;
		}

		private static bool IsExpired(byte[] timeStampBinary)
		{
			long num = BitConverter.ToInt64(timeStampBinary, 0);
			return num < DateTime.UtcNow.Ticks;
		}

		private Canary(byte[] userContextIdBinary, byte[] timeStampBinary, string logonUniqueKey)
		{
			byte[] array = Canary.ComputeHash(userContextIdBinary, timeStampBinary, logonUniqueKey);
			byte[] array2 = new byte[userContextIdBinary.Length + timeStampBinary.Length + array.Length];
			userContextIdBinary.CopyTo(array2, 0);
			timeStampBinary.CopyTo(array2, userContextIdBinary.Length);
			array.CopyTo(array2, userContextIdBinary.Length + timeStampBinary.Length);
			this.UserContextId = new Guid(userContextIdBinary).ToString("N");
			this.LogonUniqueKey = logonUniqueKey;
			this.canaryString = Canary.Encode(array2);
		}

		public Canary(Guid userContextId, string logonUniqueKey) : this(userContextId.ToByteArray(), BitConverter.GetBytes(DateTime.UtcNow.Ticks + 1728000000000L), logonUniqueKey)
		{
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

		private static bool ParseCanary(string canaryString, out byte[] userContextIdBinary, out byte[] timeStampBinary, out byte[] hashBinary)
		{
			userContextIdBinary = null;
			timeStampBinary = null;
			hashBinary = null;
			if (string.IsNullOrEmpty(canaryString) || canaryString.Length != 76)
			{
				return false;
			}
			byte[] array;
			try
			{
				array = Canary.Decode(canaryString);
			}
			catch (FormatException)
			{
				return false;
			}
			if (array.Length != 56)
			{
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

		public static Canary RestoreCanary(string canaryString, string logonUniqueKey)
		{
			byte[] userContextIdBinary;
			byte[] timeStampBinary;
			byte[] b;
			if (Canary.ParseCanary(canaryString, out userContextIdBinary, out timeStampBinary, out b))
			{
				if (Canary.IsExpired(timeStampBinary))
				{
					return null;
				}
				byte[] a = Canary.ComputeHash(userContextIdBinary, timeStampBinary, logonUniqueKey);
				if (Canary.AreEqual(a, b))
				{
					return new Canary(userContextIdBinary, timeStampBinary, logonUniqueKey);
				}
			}
			return null;
		}

		public bool ValidateCanary(string canaryString)
		{
			byte[] userContextIdBinary;
			byte[] timeStampBinary;
			byte[] a;
			if (!Canary.ParseCanary(canaryString, out userContextIdBinary, out timeStampBinary, out a))
			{
				return false;
			}
			if (Canary.IsExpired(timeStampBinary))
			{
				return false;
			}
			byte[] b = Canary.ComputeHash(userContextIdBinary, timeStampBinary, this.LogonUniqueKey);
			return Canary.AreEqual(a, b);
		}

		public override string ToString()
		{
			return this.canaryString;
		}

		private const int UserContextIdLength = 16;

		private const int TimeStampLength = 8;

		private const int HashLength = 32;

		private const int CanaryBinaryLength = 56;

		private const long TimeStampLifetime = 1728000000000L;

		public const int CanaryStringLength = 76;

		private readonly string canaryString;

		private static byte[] adObjectIdsBinary;
	}
}
