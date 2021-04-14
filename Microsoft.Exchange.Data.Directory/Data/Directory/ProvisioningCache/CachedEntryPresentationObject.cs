using System;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	[Serializable]
	public class CachedEntryPresentationObject
	{
		public CachedEntryPresentationObject(Guid key, string value) : this(key, Guid.Empty, value)
		{
		}

		public CachedEntryPresentationObject(Guid key, Guid orgId, string value)
		{
			this.CacheKey = key;
			this.OrganizationId = orgId;
			this.Value = value;
		}

		public Guid CacheKey
		{
			get
			{
				return this.cacheKey;
			}
			private set
			{
				this.cacheKey = value;
			}
		}

		public Guid OrganizationId
		{
			get
			{
				return this.organizationId;
			}
			private set
			{
				this.organizationId = value;
			}
		}

		public string Value
		{
			get
			{
				return this.cacheValue;
			}
			private set
			{
				this.cacheValue = value;
			}
		}

		public static CachedEntryPresentationObject TryFromReceivedData(byte[] buffer, int bufLen, out Exception ex)
		{
			ex = null;
			CachedEntryPresentationObject result = null;
			try
			{
				result = CachedEntryPresentationObject.FromReceivedData(buffer, bufLen);
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			return result;
		}

		public static CachedEntryPresentationObject FromReceivedData(byte[] buffer, int bufLen)
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
			if (array.Length < 2)
			{
				throw new ArgumentException(string.Format("Invalid provisioning cache dump object received: {0}.", @string));
			}
			string value = null;
			if (array.Length == 3 && !string.IsNullOrWhiteSpace(array[2]))
			{
				value = array[2];
			}
			return new CachedEntryPresentationObject(new Guid(array[0]), new Guid(array[1]), value);
		}

		private const char Delimer = '\n';

		private Guid cacheKey;

		private Guid organizationId;

		private string cacheValue;
	}
}
