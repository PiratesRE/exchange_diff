using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.HttpProxy
{
	internal abstract class BackEndCookieEntryBase
	{
		protected BackEndCookieEntryBase(BackEndCookieEntryType entryType, ExDateTime expiryTime)
		{
			this.EntryType = entryType;
			this.ExpiryTime = expiryTime;
		}

		public ExDateTime ExpiryTime { get; protected set; }

		public bool Expired
		{
			get
			{
				return this.ExpiryTime < ExDateTime.UtcNow;
			}
		}

		public BackEndCookieEntryType EntryType { get; protected set; }

		public string ToObscureString()
		{
			return BackEndCookieEntryBase.Obscurify(this.ToString());
		}

		public virtual bool ShouldInvalidate(BackEndServer badTarget)
		{
			return false;
		}

		internal static string ConvertBackEndCookieEntryTypeToString(BackEndCookieEntryType entryType)
		{
			switch (entryType)
			{
			case BackEndCookieEntryType.Server:
				return BackEndCookieEntryBase.BackEndCookieEntryTypeServerName;
			case BackEndCookieEntryType.Database:
				return BackEndCookieEntryBase.BackEndCookieEntryTypeDatabaseName;
			default:
				throw new InvalidOperationException(string.Format("Unknown cookie type: {0}", entryType));
			}
		}

		internal static bool TryGetBackEndCookieEntryTypeFromString(string entryTypeString, out BackEndCookieEntryType entryType)
		{
			if (string.Equals(entryTypeString, BackEndCookieEntryBase.BackEndCookieEntryTypeDatabaseName, StringComparison.OrdinalIgnoreCase))
			{
				entryType = BackEndCookieEntryType.Database;
				return true;
			}
			if (string.Equals(entryTypeString, BackEndCookieEntryBase.BackEndCookieEntryTypeServerName, StringComparison.OrdinalIgnoreCase))
			{
				entryType = BackEndCookieEntryType.Server;
				return true;
			}
			entryType = BackEndCookieEntryType.Server;
			return false;
		}

		protected static string Obscurify(string clearString)
		{
			byte[] bytes = BackEndCookieEntryBase.Encoding.GetBytes(clearString);
			byte[] array = new byte[bytes.Length];
			for (int i = 0; i < bytes.Length; i++)
			{
				byte[] array2 = array;
				int num = i;
				byte[] array3 = bytes;
				int num2 = i;
				array2[num] = (array3[num2] ^= BackEndCookieEntryBase.ObfuscateValue);
			}
			return Convert.ToBase64String(array);
		}

		public const int MaxBackEndServerCookieEntries = 5;

		protected const char Separator = '~';

		public static readonly TimeSpan BackEndServerCookieLifeTime = TimeSpan.FromMinutes(10.0);

		public static readonly TimeSpan LongLivedBackEndServerCookieLifeTime = TimeSpan.FromDays(30.0);

		internal static readonly byte ObfuscateValue = byte.MaxValue;

		internal static readonly ASCIIEncoding Encoding = new ASCIIEncoding();

		internal static readonly string BackEndCookieEntryTypeServerName = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
		{
			Enum.GetName(typeof(BackEndCookieEntryType), BackEndCookieEntryType.Server)
		});

		internal static readonly string BackEndCookieEntryTypeDatabaseName = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
		{
			Enum.GetName(typeof(BackEndCookieEntryType), BackEndCookieEntryType.Database)
		});
	}
}
