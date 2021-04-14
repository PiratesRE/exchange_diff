using System;
using System.Collections;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	public class CachedEntryObject
	{
		public CachedEntryObject(Guid key, object value) : this(key, Guid.Empty, value)
		{
		}

		public CachedEntryObject(Guid key, Guid orgId, object value)
		{
			this.CacheKey = key;
			this.OrganizationId = orgId;
			this.RawValue = value;
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

		public object RawValue
		{
			get
			{
				return this.rawValue;
			}
			private set
			{
				this.rawValue = value;
			}
		}

		public byte[] ToSendMessage()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.CacheKey.ToString()).Append('\n');
			stringBuilder.Append(this.OrganizationId.ToString()).Append('\n');
			string value = CachedEntryObject.RawValueToString(this.CacheKey, this.rawValue);
			if (!string.IsNullOrWhiteSpace(value))
			{
				stringBuilder.Append(value);
			}
			else
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return Encoding.UTF8.GetBytes(stringBuilder.ToString());
		}

		private static string RawValueToString(Guid cacheKey, object value)
		{
			if (value == null)
			{
				return null;
			}
			if (value is IDictionary)
			{
				return CachedEntryObject.BuildStringFromDictionary(cacheKey, (IDictionary)value);
			}
			if (value is IEnumerable)
			{
				return CachedEntryObject.BuildStringFromCollection(cacheKey, (IEnumerable)value);
			}
			return CachedEntryObject.BuildStringFromSingleValue(cacheKey, value);
		}

		private static string BuildStringFromDictionary(Guid key, IDictionary dictionary)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in dictionary.Keys)
			{
				string value = CachedEntryObject.BuildStringFromSingleValue(key, obj);
				string value2 = CachedEntryObject.RawValueToString(key, dictionary[obj]);
				stringBuilder.Append(value).Append('=');
				if (!string.IsNullOrWhiteSpace(value2))
				{
					stringBuilder.Append(value2).Append(';');
				}
				else
				{
					stringBuilder.Append("Null").Append(';');
				}
			}
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				';'
			});
		}

		private static string BuildStringFromCollection(Guid key, IEnumerable collection)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object value in collection)
			{
				string value2 = CachedEntryObject.RawValueToString(key, value);
				if (!string.IsNullOrWhiteSpace(value2))
				{
					stringBuilder.Append(value2).Append(';');
				}
				else
				{
					stringBuilder.Append("Null").Append(';');
				}
			}
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				';'
			});
		}

		private static string BuildStringFromSingleValue(Guid key, object value)
		{
			if (value == null)
			{
				return null;
			}
			if (value is ADObjectId)
			{
				return ((ADObjectId)value).DistinguishedName;
			}
			if (value is ADObject)
			{
				return ((ADObject)value).DistinguishedName;
			}
			return value.ToString();
		}

		private const char Separator = ';';

		private const char Equator = '=';

		private const char Delimiter = '\n';

		private Guid cacheKey;

		private Guid organizationId = Guid.Empty;

		private object rawValue;
	}
}
