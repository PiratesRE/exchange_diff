using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	[DataContract]
	internal class SimpleADObject : ISimpleADValue<SimpleADObject>, IExtensibleDataObject
	{
		public SimpleADObject()
		{
			this.Properties = new SimpleADObject.SimpleList<SimpleADObject.SimpleADProperty>(0);
			this.origin = SimpleADObject.ObjectOrigin.New;
		}

		public SimpleADObject.SimpleList<SimpleADObject.SimpleADProperty> Properties { get; set; }

		public string Name { get; set; }

		public string OriginatingServer { get; set; }

		public ObjectState ObjectState { get; set; }

		public DateTime WhenReadUTC { get; set; }

		public DirectoryBackendType DirectoryBackendType { get; set; }

		public ExchangeObjectVersion ExchangeVersion { get; set; }

		[DataMember(IsRequired = true)]
		public byte[] Data { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }

		public static TObject CreateFrom<TObject>(SimpleADObject simpleADObject, ADObjectSchema schema, IEnumerable<PropertyDefinition> additionalProperties) where TObject : ADRawEntry, new()
		{
			ArgumentValidator.ThrowIfNull("simpleADObject", simpleADObject);
			if (typeof(TObject).Equals(typeof(ADRawEntry)))
			{
				throw new ArgumentException("TObject cannot be ADRawEntry");
			}
			ADPropertyBag adpropertyBag = new ADPropertyBag();
			SimpleADObject.SimpleList<SimpleADObject.SimpleADProperty> properties = simpleADObject.Properties;
			SimpleADObject.SimpleADProperty simpleADProperty;
			if (!properties.TryGetValue(ADObjectSchema.Id.LdapDisplayName, out simpleADProperty))
			{
				throw new InvalidOperationException("dn is missing");
			}
			ValidationError validationError;
			ADObjectId value = (ADObjectId)ADValueConvertor.ConvertFromADAndValidateSingleValue(simpleADProperty.Value as string, ADObjectSchema.Id, false, out validationError);
			adpropertyBag.SetField(ADObjectSchema.Id, value);
			adpropertyBag.SetField(ADObjectSchema.ObjectState, simpleADObject.ObjectState);
			adpropertyBag.SetObjectVersion(simpleADObject.ExchangeVersion);
			TObject tobject = Activator.CreateInstance<TObject>();
			IEnumerable<PropertyDefinition> enumerable;
			if (schema != null)
			{
				enumerable = schema.AllProperties;
			}
			else
			{
				enumerable = ((ADObject)((object)tobject)).Schema.AllProperties;
			}
			if (additionalProperties != null)
			{
				enumerable = enumerable.Concat(additionalProperties);
			}
			foreach (PropertyDefinition propertyDefinition in enumerable)
			{
				ADPropertyDefinition propertyDefinition2 = (ADPropertyDefinition)propertyDefinition;
				SimpleADObject.AddPropertyToPropertyBag(propertyDefinition2, adpropertyBag, properties);
			}
			if (tobject is MiniObject)
			{
				adpropertyBag.SetIsReadOnly(true);
			}
			if (schema != null || (!(tobject is ADRecipient) && !(tobject is MiniRecipient)))
			{
				tobject = (TObject)((object)ADObjectFactory.CreateAndInitializeConfigObject<TObject>(adpropertyBag, tobject, null));
			}
			else
			{
				tobject = (TObject)((object)ADObjectFactory.CreateAndInitializeRecipientObject<TObject>(adpropertyBag, tobject, null));
			}
			tobject.OriginatingServer = simpleADObject.OriginatingServer;
			tobject.WhenReadUTC = new DateTime?(simpleADObject.WhenReadUTC);
			tobject.DirectoryBackendType = simpleADObject.DirectoryBackendType;
			tobject.IsCached = true;
			tobject.ValidateRead();
			tobject.ResetChangeTracking();
			return tobject;
		}

		public static ADRawEntry CreateFrom(SimpleADObject simpleADObject, IEnumerable<PropertyDefinition> properties)
		{
			ArgumentValidator.ThrowIfNull("simpleADObject", simpleADObject);
			ArgumentValidator.ThrowIfNull("properties", properties);
			ADPropertyBag adpropertyBag = new ADPropertyBag();
			SimpleADObject.SimpleList<SimpleADObject.SimpleADProperty> properties2 = simpleADObject.Properties;
			SimpleADObject.SimpleADProperty simpleADProperty;
			if (!properties2.TryGetValue(ADObjectSchema.Id.LdapDisplayName, out simpleADProperty))
			{
				throw new InvalidOperationException("dn is missing");
			}
			ValidationError validationError;
			ADObjectId value = (ADObjectId)ADValueConvertor.ConvertFromADAndValidateSingleValue(simpleADProperty.Value as string, ADObjectSchema.Id, false, out validationError);
			adpropertyBag.SetField(ADObjectSchema.Id, value);
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				ADPropertyDefinition propertyDefinition2 = (ADPropertyDefinition)propertyDefinition;
				SimpleADObject.AddPropertyToPropertyBag(propertyDefinition2, adpropertyBag, properties2);
			}
			adpropertyBag.SetField(ADObjectSchema.ObjectState, simpleADObject.ObjectState);
			adpropertyBag.SetObjectVersion(simpleADObject.ExchangeVersion);
			return new ADRawEntry(adpropertyBag)
			{
				OriginatingServer = simpleADObject.OriginatingServer,
				WhenReadUTC = new DateTime?(simpleADObject.WhenReadUTC),
				DirectoryBackendType = simpleADObject.DirectoryBackendType,
				IsCached = true
			};
		}

		public static SimpleADObject CreateFromRawEntry(ADRawEntry adRawEntry, IEnumerable<PropertyDefinition> properties, bool includeNullProperties)
		{
			ArgumentValidator.ThrowIfNull("adRawEntry", adRawEntry);
			ArgumentValidator.ThrowIfNull("properties", properties);
			SimpleADObject simpleADObject = new SimpleADObject();
			simpleADObject.OriginatingServer = adRawEntry.OriginatingServer;
			simpleADObject.ObjectState = adRawEntry.ObjectState;
			simpleADObject.WhenReadUTC = adRawEntry.WhenReadUTC.Value;
			simpleADObject.DirectoryBackendType = adRawEntry.DirectoryBackendType;
			simpleADObject.ExchangeVersion = adRawEntry.ExchangeVersion;
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (adpropertyDefinition == null)
				{
					ArgumentValidator.ThrowIfTypeInvalid("properties", properties, typeof(IEnumerable<ADPropertyDefinition>));
				}
				if (adpropertyDefinition.LdapDisplayName != null && !adpropertyDefinition.IsCalculated)
				{
					object obj = null;
					if (!adRawEntry.propertyBag.TryGetField(adpropertyDefinition, ref obj) || obj == null)
					{
						if (includeNullProperties)
						{
							simpleADObject.AddProperty(new SimpleADObject.SimpleADProperty
							{
								Name = adpropertyDefinition.LdapDisplayName,
								Value = null
							});
						}
					}
					else
					{
						if (adpropertyDefinition.IsMultivalued)
						{
							MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)obj;
							if (multiValuedPropertyBase == null || multiValuedPropertyBase.Count == 0)
							{
								if (includeNullProperties)
								{
									simpleADObject.AddProperty(new SimpleADObject.SimpleADProperty
									{
										Name = adpropertyDefinition.LdapDisplayName,
										Value = null
									});
									continue;
								}
								continue;
							}
						}
						SimpleADObject.SimpleADProperty simpleADProperty = new SimpleADObject.SimpleADProperty();
						simpleADProperty.Name = adpropertyDefinition.LdapDisplayName;
						ADObjectId adobjectId = obj as ADObjectId;
						if (!adpropertyDefinition.IsMultivalued)
						{
							if (adpropertyDefinition.IsBinary)
							{
								simpleADProperty.Value = ADValueConvertor.ConvertValueToBinary(obj, adpropertyDefinition.FormatProvider);
							}
							else if (adobjectId != null && adpropertyDefinition.IsSoftLinkAttribute && string.IsNullOrEmpty(adobjectId.DistinguishedName))
							{
								simpleADProperty.Value = adobjectId.ToSoftLinkValue();
							}
							else if (adobjectId != null)
							{
								simpleADProperty.Value = adobjectId.ToExtendedDN();
							}
							else
							{
								simpleADProperty.Value = ADValueConvertor.ConvertValueToString(obj, adpropertyDefinition.FormatProvider);
							}
						}
						else
						{
							IEnumerable enumerable = (IEnumerable)obj;
							if (adpropertyDefinition.IsBinary)
							{
								List<byte[]> list = new List<byte[]>();
								foreach (object originalValue in enumerable)
								{
									list.Add(ADValueConvertor.ConvertValueToBinary(originalValue, adpropertyDefinition.FormatProvider));
								}
								if (list.Count == 0)
								{
									list = null;
								}
								simpleADProperty.Value = list;
							}
							else
							{
								List<string> list2 = new List<string>();
								foreach (object obj2 in enumerable)
								{
									adobjectId = (obj2 as ADObjectId);
									if (adobjectId != null)
									{
										list2.Add(adobjectId.ToExtendedDN());
									}
									else
									{
										list2.Add(ADValueConvertor.ConvertValueToString(obj2, adpropertyDefinition.FormatProvider));
									}
								}
								if (list2.Count == 0)
								{
									list2 = null;
								}
								simpleADProperty.Value = list2;
							}
						}
						simpleADObject.AddProperty(simpleADProperty);
					}
				}
			}
			return simpleADObject;
		}

		public static SimpleADObject CreateFrom(ADObject adObject, IEnumerable<PropertyDefinition> additionalProperties = null)
		{
			ArgumentValidator.ThrowIfNull("adObject", adObject);
			if (additionalProperties != null)
			{
				return SimpleADObject.CreateFromRawEntry(adObject, adObject.Schema.AllProperties.Concat(additionalProperties), true);
			}
			return SimpleADObject.CreateFromRawEntry(adObject, adObject.Schema.AllProperties, false);
		}

		public static SimpleADObject.SimpleList<SimpleADObject> CreateList<T>(IList<T> list) where T : ADObject, new()
		{
			ArgumentValidator.ThrowIfNull("list", list);
			SimpleADObject.SimpleList<SimpleADObject> simpleList = new SimpleADObject.SimpleList<SimpleADObject>(list.Count);
			foreach (T t in list)
			{
				SimpleADObject simpleADObject = SimpleADObject.CreateFrom(t, null);
				simpleList.Add(simpleADObject.Name, simpleADObject);
			}
			return simpleList;
		}

		public static List<T> CreateList<T>(SimpleADObject.SimpleList<SimpleADObject> list, ADObjectSchema schema, IEnumerable<PropertyDefinition> additionalProperties = null) where T : ADObject, new()
		{
			ArgumentValidator.ThrowIfNull("list", list);
			ArgumentValidator.ThrowIfNull("schema", schema);
			List<T> list2 = new List<T>(list.Count);
			foreach (SimpleADObject simpleADObject in list.Values)
			{
				T item = SimpleADObject.CreateFrom<T>(simpleADObject, schema, additionalProperties);
				list2.Add(item);
			}
			return list2;
		}

		public static SimpleADObject.SimpleList<SimpleADObject> ReadList(BinaryReader reader)
		{
			return SimpleADObject.SimpleList<SimpleADObject>.Read(reader);
		}

		public static void WriteList(BinaryWriter writer, SimpleADObject.SimpleList<SimpleADObject> list)
		{
			SimpleADObject.SimpleList<SimpleADObject>.Write(writer, list);
		}

		public static bool ListEquals(SimpleADObject.SimpleList<SimpleADObject> left, SimpleADObject.SimpleList<SimpleADObject> right)
		{
			return SimpleADObject.SimpleList<SimpleADObject>.Equals(left, right);
		}

		public void Read(BinaryReader reader)
		{
			this.InitializeFrom(reader);
		}

		public void Write(BinaryWriter writer)
		{
			ArgumentValidator.ThrowIfNull("writer", writer);
			writer.Write(this.OriginatingServer);
			writer.Write((byte)this.ObjectState);
			writer.Write(this.WhenReadUTC.ToBinary());
			writer.Write((byte)this.DirectoryBackendType);
			writer.Write(this.ExchangeVersion.ToInt64());
			SimpleADObject.SimpleList<SimpleADObject.SimpleADProperty>.Write(writer, this.Properties);
		}

		[OnSerializing]
		public void OnSerializing(StreamingContext context)
		{
			if (SimpleADObject.ObjectOrigin.Serialized != this.origin || this.Data == null)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
					{
						this.Write(binaryWriter);
						this.Data = memoryStream.ToArray();
					}
				}
			}
		}

		[OnDeserialized]
		public void OnDeserialized(StreamingContext context)
		{
			this.origin = SimpleADObject.ObjectOrigin.Serialized;
		}

		public void Initialize(bool readOnly)
		{
			if (SimpleADObject.ObjectOrigin.Serialized != this.origin)
			{
				throw new InvalidOperationException(string.Format("Operation not supported on a {0} object", this.origin));
			}
			if (this.isObjectInitialized)
			{
				return;
			}
			if (this.Data == null)
			{
				throw new InvalidOperationException("Data is null");
			}
			using (MemoryStream memoryStream = new MemoryStream(this.Data))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					this.InitializeFrom(binaryReader);
				}
			}
			if (!readOnly)
			{
				this.Data = null;
			}
		}

		private void InitializeFrom(BinaryReader reader)
		{
			ArgumentValidator.ThrowIfNull("reader", reader);
			if (this.isObjectInitialized)
			{
				throw new NotSupportedException("Object is already initialized");
			}
			this.origin = SimpleADObject.ObjectOrigin.Serialized;
			this.OriginatingServer = reader.ReadString();
			this.ObjectState = (ObjectState)reader.ReadByte();
			this.WhenReadUTC = DateTime.FromBinary(reader.ReadInt64());
			this.DirectoryBackendType = (DirectoryBackendType)reader.ReadByte();
			this.ExchangeVersion = new ExchangeObjectVersion(reader.ReadInt64());
			this.Properties = SimpleADObject.SimpleList<SimpleADObject.SimpleADProperty>.Read(reader);
			this.Name = this.Properties[ADObjectSchema.Id.LdapDisplayName].Value.ToString();
			this.isObjectInitialized = true;
		}

		public override int GetHashCode()
		{
			return this.Properties.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SimpleADObject);
		}

		public bool Equals(SimpleADObject adObject)
		{
			return adObject != null && (object.ReferenceEquals(this, adObject) || SimpleADObject.SimpleList<SimpleADObject.SimpleADProperty>.Equals(this.Properties, adObject.Properties));
		}

		private static void AddPropertyToPropertyBag(ADPropertyDefinition propertyDefinition, PropertyBag propertyBag, SimpleADObject.SimpleList<SimpleADObject.SimpleADProperty> lookup)
		{
			if (propertyBag.Contains(propertyDefinition))
			{
				return;
			}
			if (propertyDefinition.IsCalculated)
			{
				foreach (ProviderPropertyDefinition providerPropertyDefinition in propertyDefinition.SupportingProperties)
				{
					ADPropertyDefinition propertyDefinition2 = (ADPropertyDefinition)providerPropertyDefinition;
					SimpleADObject.AddPropertyToPropertyBag(propertyDefinition2, propertyBag, lookup);
				}
				return;
			}
			if (string.IsNullOrEmpty(propertyDefinition.LdapDisplayName))
			{
				return;
			}
			SimpleADObject.SimpleADProperty simpleADProperty;
			if (!lookup.TryGetValue(propertyDefinition.LdapDisplayName, out simpleADProperty) || simpleADProperty == null)
			{
				propertyBag.SetField(propertyDefinition, null);
				return;
			}
			if (simpleADProperty.Value == null)
			{
				propertyBag.SetField(propertyDefinition, null);
				return;
			}
			object value5;
			if (propertyDefinition.IsMultivalued)
			{
				ArrayList arrayList = new ArrayList();
				ArrayList invalidValues = new ArrayList();
				if (propertyDefinition.IsBinary)
				{
					List<byte[]> list = simpleADProperty.Value as List<byte[]>;
					if (list == null)
					{
						return;
					}
					using (List<byte[]>.Enumerator enumerator2 = list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							byte[] value = enumerator2.Current;
							ValidationError validationError;
							object value2 = ADValueConvertor.ConvertFromADAndValidateSingleValue(value, propertyDefinition, propertyDefinition.Type == typeof(byte[]), out validationError);
							arrayList.Add(value2);
						}
						goto IL_17F;
					}
				}
				List<string> list2 = simpleADProperty.Value as List<string>;
				if (list2 == null)
				{
					return;
				}
				foreach (string value3 in list2)
				{
					ValidationError validationError2;
					object value4 = ADValueConvertor.ConvertFromADAndValidateSingleValue(value3, propertyDefinition, propertyDefinition.Type == typeof(string), out validationError2);
					arrayList.Add(value4);
				}
				IL_17F:
				MultiValuedPropertyBase multiValuedPropertyBase = ADValueConvertor.CreateGenericMultiValuedProperty(propertyDefinition, true, arrayList, invalidValues, null);
				multiValuedPropertyBase.IsCompletelyRead = true;
				value5 = multiValuedPropertyBase;
			}
			else
			{
				ValidationError validationError3;
				value5 = ADValueConvertor.ConvertFromADAndValidateSingleValue(simpleADProperty.Value, propertyDefinition, propertyDefinition.Type == (propertyDefinition.IsBinary ? typeof(byte[]) : typeof(string)), out validationError3);
			}
			propertyBag.SetField(propertyDefinition, value5);
		}

		private void AddProperty(SimpleADObject.SimpleADProperty simpleProperty)
		{
			if (simpleProperty.Name == ADObjectSchema.Id.LdapDisplayName)
			{
				this.Name = simpleProperty.Value.ToString();
			}
			if (!this.Properties.ContainsKey(simpleProperty.Name))
			{
				this.Properties.Add(simpleProperty.Name, simpleProperty);
			}
		}

		private SimpleADObject.ObjectOrigin origin;

		private bool isObjectInitialized;

		private enum ObjectOrigin
		{
			None,
			New,
			Serialized
		}

		internal class SimpleList<T> : Dictionary<string, T> where T : ISimpleADValue<T>, new()
		{
			public SimpleList(int count) : base(count, StringComparer.OrdinalIgnoreCase)
			{
			}

			public static SimpleADObject.SimpleList<T> Read(BinaryReader reader)
			{
				ArgumentValidator.ThrowIfNull("reader", reader);
				int num = reader.ReadInt32();
				SimpleADObject.SimpleList<T> simpleList = new SimpleADObject.SimpleList<T>(num);
				for (int i = 0; i < num; i++)
				{
					T value = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
					value.Read(reader);
					simpleList.Add(value.Name, value);
				}
				return simpleList;
			}

			public static void Write(BinaryWriter writer, SimpleADObject.SimpleList<T> list)
			{
				ArgumentValidator.ThrowIfNull("writer", writer);
				int num = (list == null) ? 0 : list.Count;
				writer.Write(num);
				if (num == 0)
				{
					return;
				}
				foreach (T t in list.Values)
				{
					t.Write(writer);
				}
			}

			public static bool Equals(SimpleADObject.SimpleList<T> left, SimpleADObject.SimpleList<T> right)
			{
				return object.ReferenceEquals(left, right) || (left != null && left.Equals(right));
			}

			public override int GetHashCode()
			{
				int num = 0;
				foreach (T t in base.Values)
				{
					num ^= t.GetHashCode();
				}
				return num;
			}

			public override bool Equals(object obj)
			{
				return this.Equals(obj as SimpleADObject.SimpleList<T>);
			}

			public bool Equals(SimpleADObject.SimpleList<T> right)
			{
				if (right == null)
				{
					return false;
				}
				if (object.ReferenceEquals(this, right))
				{
					return true;
				}
				if (base.Count != right.Count)
				{
					return false;
				}
				foreach (T t in base.Values)
				{
					T right2;
					if (!right.TryGetValue(t.Name, out right2) || !t.Equals(right2))
					{
						return false;
					}
				}
				return true;
			}
		}

		internal class SimpleADProperty : ISimpleADValue<SimpleADObject.SimpleADProperty>
		{
			public string Name { get; set; }

			public object Value { get; set; }

			public void Read(BinaryReader reader)
			{
				ArgumentValidator.ThrowIfNull("reader", reader);
				string name = reader.ReadString();
				this.Name = name;
				ValueFormat valueFormat = (ValueFormat)reader.ReadByte();
				ValueFormat valueFormat2 = valueFormat;
				if (valueFormat2 <= ValueFormat.String)
				{
					if (valueFormat2 == ValueFormat.FormatModifierShift)
					{
						this.Value = null;
						return;
					}
					if (valueFormat2 == ValueFormat.String)
					{
						this.Value = reader.ReadString();
						return;
					}
				}
				else
				{
					if (valueFormat2 == ValueFormat.Binary)
					{
						int num = reader.ReadInt32();
						this.Value = reader.ReadBytes(num);
						return;
					}
					if (valueFormat2 == ValueFormat.MVString)
					{
						int num = reader.ReadInt32();
						List<string> list = new List<string>(num);
						for (int i = 0; i < num; i++)
						{
							string item = reader.ReadString();
							list.Add(item);
						}
						this.Value = list;
						return;
					}
					if (valueFormat2 == ValueFormat.MVBinary)
					{
						int num = reader.ReadInt32();
						List<byte[]> list2 = new List<byte[]>(num);
						for (int j = 0; j < num; j++)
						{
							int count = reader.ReadInt32();
							byte[] item2 = reader.ReadBytes(count);
							list2.Add(item2);
						}
						this.Value = list2;
						return;
					}
				}
				throw new ArgumentException();
			}

			public void Write(BinaryWriter writer)
			{
				ArgumentValidator.ThrowIfNull("writer", writer);
				writer.Write(this.Name);
				if (this.Value == null)
				{
					writer.Write(0);
					return;
				}
				string text = this.Value as string;
				if (text != null)
				{
					writer.Write(72);
					writer.Write(text);
					return;
				}
				byte[] array = this.Value as byte[];
				if (array != null)
				{
					writer.Write(80);
					writer.Write(array.Length);
					writer.Write(array);
					return;
				}
				List<string> list = this.Value as List<string>;
				if (list != null)
				{
					writer.Write(200);
					writer.Write(list.Count);
					foreach (string value in list)
					{
						writer.Write(value);
					}
					return;
				}
				List<byte[]> list2 = this.Value as List<byte[]>;
				if (list2 != null)
				{
					writer.Write(208);
					writer.Write(list2.Count);
					foreach (byte[] array2 in list2)
					{
						writer.Write(array2.Length);
						writer.Write(array2);
					}
					return;
				}
				throw new ArgumentException();
			}

			public override int GetHashCode()
			{
				int num = this.Name.GetHashCode();
				string text = this.Value as string;
				if (text != null)
				{
					num ^= text.GetHashCode();
					return num;
				}
				byte[] array = this.Value as byte[];
				if (array != null)
				{
					num ^= ByteArrayComparer.Instance.GetHashCode(array);
					return num;
				}
				List<string> list = this.Value as List<string>;
				if (list != null)
				{
					foreach (string text2 in list)
					{
						num ^= text2.GetHashCode();
					}
					return num;
				}
				List<byte[]> list2 = this.Value as List<byte[]>;
				if (list2 != null)
				{
					foreach (byte[] bytes in list2)
					{
						num ^= ByteArrayComparer.Instance.GetHashCode(bytes);
					}
					return num;
				}
				return num;
			}

			public override bool Equals(object obj)
			{
				return this.Equals(obj as SimpleADObject.SimpleADProperty);
			}

			public bool Equals(SimpleADObject.SimpleADProperty property)
			{
				if (property == null)
				{
					return false;
				}
				if (object.ReferenceEquals(this, property))
				{
					return true;
				}
				if (this.Name != property.Name)
				{
					return false;
				}
				if (this.Value == null)
				{
					return property.Value == null;
				}
				string text = this.Value as string;
				if (text != null)
				{
					return text.Equals(property.Value);
				}
				byte[] array = this.Value as byte[];
				if (array != null)
				{
					return ByteArrayComparer.Instance.Equals(array, property.Value as byte[]);
				}
				List<string> list = this.Value as List<string>;
				if (list != null)
				{
					return list.Equals(property.Value as List<string>, StringComparer.Ordinal);
				}
				List<byte[]> list2 = this.Value as List<byte[]>;
				return list2 != null && list2.Equals(property.Value as List<byte[]>, ByteArrayComparer.Instance);
			}
		}
	}
}
