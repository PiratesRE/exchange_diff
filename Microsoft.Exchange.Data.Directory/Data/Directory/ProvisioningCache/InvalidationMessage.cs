using System;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	internal class InvalidationMessage
	{
		public InvalidationMessage(Guid orgId, Guid[] keys) : this(orgId, keys, InvalidationMessage.IsGlobalCacheEntry(keys))
		{
		}

		private InvalidationMessage(Guid orgId, Guid[] keys, bool isGlobal)
		{
			if (keys == null || keys.Length == 0)
			{
				throw new ArgumentException("The keys are either null or empty.");
			}
			this.OrganizationId = orgId;
			this.CacheKeys = keys;
			this.IsGlobal = isGlobal;
		}

		public Guid OrganizationId { get; private set; }

		public Guid[] CacheKeys { get; private set; }

		public bool IsCacheClearMessage
		{
			get
			{
				return this.CacheKeys[0].Equals(Guid.Empty);
			}
		}

		public bool IsGlobal { get; private set; }

		public static InvalidationMessage TryFromReceivedData(byte[] buffer, int bufLen, out Exception ex)
		{
			ex = null;
			InvalidationMessage result = null;
			try
			{
				result = InvalidationMessage.FromReceivedData(buffer, bufLen);
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			return result;
		}

		public static InvalidationMessage FromReceivedData(byte[] buffer, int bufLen)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (bufLen <= 0)
			{
				throw new ArgumentException("bufLen is less than zero.");
			}
			if (buffer.Length < bufLen)
			{
				throw new ArgumentException("The buffer is too small.");
			}
			string @string = Encoding.UTF8.GetString(buffer, 0, bufLen);
			string[] array = @string.Split(new char[]
			{
				'\n'
			});
			if (array == null || array.Length != 2)
			{
				throw new ArgumentException("Message is invalid.");
			}
			Guid empty = Guid.Empty;
			bool isGlobal = false;
			if (!array[0].Equals("GlobalCacheKeys", StringComparison.OrdinalIgnoreCase))
			{
				empty = new Guid(array[0]);
			}
			else
			{
				isGlobal = true;
			}
			string[] array2 = array[1].Split(new char[]
			{
				';'
			});
			Guid[] array3 = new Guid[array2.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array3[i] = new Guid(array2[i]);
			}
			return new InvalidationMessage(empty, array3, isGlobal);
		}

		public static bool IsGlobalCacheEntry(Guid[] keys)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			foreach (Guid a in keys)
			{
				foreach (Guid b in CannedProvisioningCacheKeys.GlobalCacheKeys)
				{
					if (a == b)
					{
						return true;
					}
				}
			}
			return false;
		}

		public byte[] ToSendMessage()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.IsGlobal)
			{
				stringBuilder.Append("GlobalCacheKeys");
			}
			else
			{
				stringBuilder.Append(this.OrganizationId.ToString());
			}
			stringBuilder.Append('\n');
			foreach (Guid guid in this.CacheKeys)
			{
				stringBuilder.Append(guid.ToString());
				stringBuilder.Append(';');
			}
			string s = stringBuilder.ToString().TrimEnd(new char[]
			{
				';'
			});
			return Encoding.UTF8.GetBytes(s);
		}

		private const char OrgIdDelimiter = '\n';

		private const char Separator = ';';

		private const string GlobalCacheKeys = "GlobalCacheKeys";

		public static readonly int Version = 1;
	}
}
