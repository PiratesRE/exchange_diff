using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Conversion
{
	internal class MdbefPropertyCollection : IDictionary<uint, object>, ICollection<KeyValuePair<uint, object>>, IEnumerable<KeyValuePair<uint, object>>, IEnumerable
	{
		public int Count
		{
			get
			{
				return this.properties.Count;
			}
		}

		public ICollection<uint> Keys
		{
			get
			{
				return this.properties.Keys;
			}
		}

		public ICollection<object> Values
		{
			get
			{
				return this.properties.Values;
			}
		}

		bool ICollection<KeyValuePair<uint, object>>.IsReadOnly
		{
			get
			{
				ICollection<KeyValuePair<uint, object>> collection = this.properties;
				return collection.IsReadOnly;
			}
		}

		public object this[uint id]
		{
			get
			{
				return this.properties[id];
			}
			set
			{
				if (value == null)
				{
					this.properties.Remove(id);
					return;
				}
				MapiPropType propType = (MapiPropType)(id & 65535U);
				if (!MdbefPropertyCollection.TypeValid(propType, value))
				{
					throw new MdbefException("Property type is invalid for this tag");
				}
				this.properties[id] = value;
			}
		}

		public static MdbefPropertyCollection Create(byte[] blob, int startIndex, int length)
		{
			MdbefPropertyCollection mdbefPropertyCollection = new MdbefPropertyCollection();
			MdbefReader mdbefReader = new MdbefReader(blob, startIndex, length);
			while (mdbefReader.ReadNextProperty())
			{
				mdbefPropertyCollection[(uint)mdbefReader.PropertyId] = mdbefReader.Value;
			}
			return mdbefPropertyCollection;
		}

		public bool ContainsKey(uint key)
		{
			return this.properties.ContainsKey(key);
		}

		public bool Remove(uint key)
		{
			return this.properties.Remove(key);
		}

		public byte[] GetBytes()
		{
			int num = 4;
			IEnumerator<KeyValuePair<uint, object>> enumerator = this.properties.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, object> keyValuePair = enumerator.Current;
				MapiPropType propType = (MapiPropType)(keyValuePair.Key & 65535U);
				num += MdbefWriter.SizeOf(propType, keyValuePair.Value);
			}
			byte[] array = new byte[num];
			int offset = 0;
			offset = ExBitConverter.Write((uint)this.properties.Count, array, offset);
			enumerator.Reset();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, object> keyValuePair2 = enumerator.Current;
				MdbefWriter.SerializeProperty(keyValuePair2.Key, keyValuePair2.Value, array, ref offset);
			}
			return array;
		}

		public virtual void Clear()
		{
			this.properties.Clear();
		}

		public void Add(uint key, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			MapiPropType propType = (MapiPropType)(key & 65535U);
			if (!MdbefPropertyCollection.TypeValid(propType, value))
			{
				throw new MdbefException("Property type is invalid for this tag");
			}
			this.properties.Add(key, value);
		}

		public bool TryGetValue(uint key, out object value)
		{
			return this.properties.TryGetValue(key, out value);
		}

		public IEnumerator<KeyValuePair<uint, object>> GetEnumerator()
		{
			return this.properties.GetEnumerator();
		}

		void ICollection<KeyValuePair<uint, object>>.Add(KeyValuePair<uint, object> item)
		{
			this.Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<uint, object>>.Contains(KeyValuePair<uint, object> item)
		{
			ICollection<KeyValuePair<uint, object>> collection = this.properties;
			return collection.Contains(item);
		}

		void ICollection<KeyValuePair<uint, object>>.CopyTo(KeyValuePair<uint, object>[] array, int arrayIndex)
		{
			ICollection<KeyValuePair<uint, object>> collection = this.properties;
			collection.CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<uint, object>>.Remove(KeyValuePair<uint, object> item)
		{
			ICollection<KeyValuePair<uint, object>> collection = this.properties;
			return collection.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			IEnumerable enumerable = this.properties;
			return enumerable.GetEnumerator();
		}

		private static bool TypeValid(MapiPropType propType, object value)
		{
			if (propType <= MapiPropType.ServerId)
			{
				if (propType <= MapiPropType.String)
				{
					switch (propType)
					{
					case MapiPropType.Short:
						return value is short;
					case MapiPropType.Int:
						return value is int;
					case MapiPropType.Float:
						return value is float;
					case MapiPropType.Double:
					case MapiPropType.AppTime:
						return value is double;
					case MapiPropType.Currency:
						break;
					case (MapiPropType)8:
					case (MapiPropType)9:
					case MapiPropType.Error:
						return false;
					case MapiPropType.Boolean:
						return value is bool;
					default:
						if (propType != MapiPropType.Long)
						{
							switch (propType)
							{
							case MapiPropType.AnsiString:
							case MapiPropType.String:
								return value is string;
							default:
								return false;
							}
						}
						break;
					}
					return value is long;
				}
				if (propType == MapiPropType.SysTime)
				{
					return value is DateTime;
				}
				if (propType == MapiPropType.Guid)
				{
					return value is Guid;
				}
				if (propType != MapiPropType.ServerId)
				{
					return false;
				}
			}
			else if (propType <= MapiPropType.LongArray)
			{
				if (propType != MapiPropType.Binary)
				{
					switch (propType)
					{
					case MapiPropType.ShortArray:
						return value is short[];
					case MapiPropType.IntArray:
						return value is int[];
					case MapiPropType.FloatArray:
						return value is float[];
					case MapiPropType.DoubleArray:
					case MapiPropType.AppTimeArray:
						return value is double[];
					case MapiPropType.CurrencyArray:
						break;
					default:
						if (propType != MapiPropType.LongArray)
						{
							return false;
						}
						break;
					}
					return value is long[];
				}
			}
			else if (propType <= MapiPropType.SysTimeArray)
			{
				switch (propType)
				{
				case MapiPropType.AnsiStringArray:
				case MapiPropType.StringArray:
					return value is string[];
				default:
					if (propType != MapiPropType.SysTimeArray)
					{
						return false;
					}
					return value is DateTime[];
				}
			}
			else
			{
				if (propType == MapiPropType.GuidArray)
				{
					return value is Guid[];
				}
				if (propType != MapiPropType.BinaryArray)
				{
					return false;
				}
				return value is byte[][];
			}
			return value is byte[];
		}

		private Dictionary<uint, object> properties = new Dictionary<uint, object>();
	}
}
