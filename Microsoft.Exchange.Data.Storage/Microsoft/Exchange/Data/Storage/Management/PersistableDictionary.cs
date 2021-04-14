using System;
using System.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PersistableDictionary : ConfigurationDictionary
	{
		public object this[object key]
		{
			get
			{
				return base.Dictionary[key];
			}
			set
			{
				base.Dictionary[key] = value;
			}
		}

		public ICollection Values
		{
			get
			{
				return base.Dictionary.Values;
			}
		}

		public static PersistableDictionary Create(string rawXml)
		{
			return MigrationXmlSerializer.Deserialize<PersistableDictionary>(rawXml);
		}

		public T GetRequired<T>(object key)
		{
			object obj = this[key];
			if (obj == null)
			{
				throw new MigrationDataCorruptionException("expected to find key " + key);
			}
			return PersistableDictionary.DeserializeProperty<T>(obj);
		}

		public T Get<T>(object key)
		{
			return PersistableDictionary.DeserializeProperty<T>(this[key]);
		}

		public bool TryGetValue<T>(object key, out T value)
		{
			value = default(T);
			if (!this.Contains(key))
			{
				return false;
			}
			bool result;
			try
			{
				value = this.Get<T>(key);
				result = true;
			}
			catch (NotInBagPropertyErrorException)
			{
				result = false;
			}
			return result;
		}

		public T Get<T>(string key, T defaultValue)
		{
			object obj = this[key];
			if (obj == null)
			{
				return defaultValue;
			}
			return PersistableDictionary.DeserializeProperty<T>(obj);
		}

		public void Set<T>(string key, T value)
		{
			this[key] = PersistableDictionary.SerializeProperty<T>(value);
		}

		public void Remove(string key)
		{
			base.Dictionary.Remove(key);
		}

		public bool Contains(object key)
		{
			return base.Dictionary.Contains(key);
		}

		public string Serialize()
		{
			return MigrationXmlSerializer.Serialize(this);
		}

		public void Add(object key, object value)
		{
			base.Dictionary.Add(key, value);
		}

		public void SetMultiValuedProperty(string key, MultiValuedProperty<string> value)
		{
			this[key] = MigrationXmlSerializer.Serialize(value);
		}

		public MultiValuedProperty<string> GetMultiValuedStringProperty(string key)
		{
			if (this.Contains(key))
			{
				string required = this.GetRequired<string>(key);
				return (MultiValuedProperty<string>)MigrationXmlSerializer.Deserialize(required, typeof(MultiValuedProperty<string>));
			}
			return null;
		}

		private static T DeserializeProperty<T>(object val)
		{
			if (val != null)
			{
				Type typeFromHandle = typeof(T);
				if (typeFromHandle.IsEnum)
				{
					try
					{
						val = Enum.ToObject(typeFromHandle, (int)val);
						goto IL_B6;
					}
					catch (ArgumentException innerException)
					{
						throw new MigrationDataCorruptionException("can't convert serialized version", innerException);
					}
				}
				if (typeFromHandle == typeof(TimeSpan?) || typeFromHandle == typeof(TimeSpan))
				{
					val = new TimeSpan((long)val);
				}
				else
				{
					if (!(typeFromHandle == typeof(Guid?)))
					{
						if (!(typeFromHandle == typeof(Guid)))
						{
							goto IL_B6;
						}
					}
					try
					{
						val = new Guid((byte[])val);
					}
					catch (ArgumentException innerException2)
					{
						throw new MigrationDataCorruptionException("couldn't deserialize guid", innerException2);
					}
				}
			}
			IL_B6:
			return (T)((object)val);
		}

		private static object SerializeProperty<T>(object val)
		{
			if (val != null)
			{
				Type typeFromHandle = typeof(T);
				if (typeFromHandle.IsEnum)
				{
					return (int)val;
				}
				if (typeFromHandle == typeof(TimeSpan))
				{
					return ((TimeSpan)val).Ticks;
				}
				if (typeFromHandle == typeof(TimeSpan?))
				{
					return ((TimeSpan?)val).Value.Ticks;
				}
				if (typeFromHandle == typeof(Guid?) || typeFromHandle == typeof(Guid))
				{
					return ((Guid)val).ToByteArray();
				}
			}
			return val;
		}
	}
}
