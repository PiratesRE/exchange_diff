using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	public abstract class XMLSerializableBase
	{
		public XMLSerializableBase()
		{
		}

		[XmlAnyElement]
		public XmlElement[] UnknownElements
		{
			get
			{
				return this.unknownElements;
			}
			set
			{
				this.unknownElements = value;
			}
		}

		[XmlAnyAttribute]
		public XmlAttribute[] UnknownAttributes
		{
			get
			{
				return this.unknownAttributes;
			}
			set
			{
				this.unknownAttributes = value;
			}
		}

		public static T Deserialize<T>(string serializedXML, bool throwOnError = true) where T : class
		{
			return XMLSerializableBase.DeserializeFromStringInternal<T>(serializedXML, delegate(Exception e)
			{
				if (throwOnError)
				{
					throw new UnableToDeserializeXMLException(e.Message, e);
				}
			});
		}

		public static T Deserialize<T>(XmlReader xmlReader, bool throwOnError = true) where T : class
		{
			return XMLSerializableBase.DeserializeFromXmlReaderInternal<T>(xmlReader, delegate(Exception e)
			{
				if (throwOnError)
				{
					throw new UnableToDeserializeXMLException(e.Message, e);
				}
			});
		}

		public static T Deserialize<T>(Stream stream, bool throwOnError = true) where T : class
		{
			return XMLSerializableBase.DeserializeFromStreamInternal<T>(stream, delegate(Exception e)
			{
				if (throwOnError)
				{
					throw new UnableToDeserializeXMLException(e.Message, e);
				}
			});
		}

		public static string Serialize(object objectToSerialize, bool indent = false)
		{
			if (objectToSerialize == null)
			{
				return null;
			}
			StringBuilder sbuilder = new StringBuilder();
			XMLSerializableBase.SerializeToNewXmlWriterInternal(objectToSerialize, (XmlWriterSettings xmlws) => XmlWriter.Create(sbuilder, xmlws), indent);
			return sbuilder.ToString();
		}

		public static void SerializeToStream(object objectToSerialize, Stream stream, bool indent = false)
		{
			XMLSerializableBase.SerializeToNewXmlWriterInternal(objectToSerialize, (XmlWriterSettings xmlws) => XmlWriter.Create(stream, xmlws), indent);
		}

		public string Serialize(bool indent = false)
		{
			return XMLSerializableBase.Serialize(this, indent);
		}

		public XElement ToDiagnosticInfo(string elementName = null)
		{
			return XElement.Parse(this.Serialize(false));
		}

		public override string ToString()
		{
			return this.Serialize(false);
		}

		internal static ADPropertyDefinition ConfigurationXmlRawProperty()
		{
			return new ADPropertyDefinition("ConfigurationXMLRaw", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchConfigurationXML", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
		}

		internal static ADPropertyDefinition ConfigurationXmlProperty<T>(ADPropertyDefinition configXmlRawProperty) where T : XMLSerializableBase
		{
			return new ADPropertyDefinition("ConfigurationXML", ExchangeObjectVersion.Exchange2003, typeof(T), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
			{
				configXmlRawProperty
			}, null, XMLSerializableBase.ConfigurationXMLGetterDelegate<T>(configXmlRawProperty), XMLSerializableBase.ConfigurationXMLSetterDelegate<T>(configXmlRawProperty), null, null);
		}

		internal static ADPropertyDefinition ConfigXmlProperty<T, ValueT>(string propertyName, ExchangeObjectVersion propertyVersion, ADPropertyDefinition configXmlProperty, ValueT defaultValue, Func<T, ValueT> getterDelegate, Action<T, ValueT> setterDelegate, SimpleProviderPropertyDefinition mservPropertyDefinition = null, SimpleProviderPropertyDefinition mbxPropertyDefinition = null) where T : XMLSerializableBase, new()
		{
			return XMLSerializableBase.ConfigXmlProperty<T, ValueT>(propertyName, propertyVersion, configXmlProperty, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, getterDelegate, setterDelegate, mservPropertyDefinition, mbxPropertyDefinition);
		}

		internal static ADPropertyDefinition ConfigXmlProperty<T, ValueT>(string propertyName, ExchangeObjectVersion propertyVersion, ADPropertyDefinition configXmlProperty, ValueT defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints, Func<T, ValueT> getterDelegate, Action<T, ValueT> setterDelegate, SimpleProviderPropertyDefinition mservPropertyDefinition = null, SimpleProviderPropertyDefinition mbxPropertyDefinition = null) where T : XMLSerializableBase, new()
		{
			ADPropertyDefinitionFlags adpropertyDefinitionFlags = ADPropertyDefinitionFlags.Calculated;
			if (setterDelegate == null)
			{
				adpropertyDefinitionFlags |= ADPropertyDefinitionFlags.ReadOnly;
			}
			return new ADPropertyDefinition(propertyName, propertyVersion, typeof(ValueT), null, adpropertyDefinitionFlags, defaultValue, readConstraints, writeConstraints, configXmlProperty.SupportingProperties.ToArray<ProviderPropertyDefinition>(), null, XMLSerializableBase.XmlElementGetterDelegate<T, ValueT>(getterDelegate, configXmlProperty, defaultValue), (setterDelegate != null) ? XMLSerializableBase.XmlElementSetterDelegate<T, ValueT>(setterDelegate, configXmlProperty) : null, mservPropertyDefinition, mbxPropertyDefinition);
		}

		internal static Unlimited<ByteQuantifiedSize> UlongToUnlimitedSize(ulong rawValue)
		{
			if (rawValue == 18446744073709551615UL)
			{
				return Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			}
			return new Unlimited<ByteQuantifiedSize>(new ByteQuantifiedSize(rawValue));
		}

		internal static ulong UnlimitedSizeToUlong(Unlimited<ByteQuantifiedSize> value)
		{
			if (!value.IsUnlimited)
			{
				return value.Value.ToBytes();
			}
			return ulong.MaxValue;
		}

		internal static T Deserialize<T>(string serializedXML, PropertyDefinition configXmlRawProperty) where T : class
		{
			return XMLSerializableBase.DeserializeFromStringInternal<T>(serializedXML, delegate(Exception e)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(configXmlRawProperty.Name, e.Message), configXmlRawProperty, serializedXML), e);
			});
		}

		internal static string GetNullableSerializationValue<T>(T? value) where T : struct
		{
			if (value != null)
			{
				T value2 = value.Value;
				return value2.ToString();
			}
			return null;
		}

		internal static T? GetNullableAttribute<T>(string value, XMLSerializableBase.TryParseDelegate<T> parseFunc) where T : struct
		{
			T value2;
			if (!parseFunc(value, out value2))
			{
				return null;
			}
			if (!string.IsNullOrWhiteSpace(value))
			{
				return new T?(value2);
			}
			return null;
		}

		protected virtual void OnDeserialized()
		{
		}

		private static T DeserializeFromStringInternal<T>(string serializedXML, Action<Exception> failureAction) where T : class
		{
			if (string.IsNullOrWhiteSpace(serializedXML))
			{
				return default(T);
			}
			T result;
			using (StringReader stringReader = new StringReader(serializedXML))
			{
				result = XMLSerializableBase.DeserializeFromTextReaderInternal<T>(stringReader, failureAction);
			}
			return result;
		}

		private static T DeserializeFromStreamInternal<T>(Stream stream, Action<Exception> failureAction) where T : class
		{
			T result;
			using (StreamReader streamReader = new StreamReader(stream))
			{
				result = XMLSerializableBase.DeserializeFromTextReaderInternal<T>(streamReader, failureAction);
			}
			return result;
		}

		private static T DeserializeFromTextReaderInternal<T>(TextReader textReader, Action<Exception> failureAction) where T : class
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				CheckCharacters = false
			};
			T result;
			using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
			{
				result = XMLSerializableBase.DeserializeFromXmlReaderInternal<T>(xmlReader, failureAction);
			}
			return result;
		}

		private static T DeserializeFromXmlReaderInternal<T>(XmlReader xmlReader, Action<Exception> failureAction) where T : class
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			T result = default(T);
			try
			{
				XMLSerializableBase.PerformSerializationOperation(delegate
				{
					result = (serializer.Deserialize(xmlReader) as T);
				});
			}
			catch (InvalidOperationException obj)
			{
				failureAction(obj);
			}
			catch (FormatException obj2)
			{
				failureAction(obj2);
			}
			XMLSerializableBase xmlserializableBase = result as XMLSerializableBase;
			if (xmlserializableBase != null)
			{
				xmlserializableBase.OnDeserialized();
			}
			return result;
		}

		private static void SerializeToNewXmlWriterInternal(object objectToSerialize, Func<XmlWriterSettings, XmlWriter> createWriter, bool indent)
		{
			XmlWriterSettings arg = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = indent,
				CheckCharacters = false
			};
			using (XmlWriter xmlWriter = createWriter(arg))
			{
				XMLSerializableBase.SerializeToXmlWriterInternal(objectToSerialize, xmlWriter);
			}
		}

		private static void SerializeToXmlWriterInternal(object objectToSerialize, XmlWriter writer)
		{
			XmlSerializer serializer = new XmlSerializer(objectToSerialize.GetType());
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add(string.Empty, string.Empty);
			XMLSerializableBase.PerformSerializationOperation(delegate
			{
				serializer.Serialize(writer, objectToSerialize, ns);
				writer.Flush();
			});
		}

		private static void PerformSerializationOperation(Action operation)
		{
			try
			{
				operation();
			}
			catch (InvalidOperationException ex)
			{
				LocalizedException ex2 = ex.InnerException as LocalizedException;
				if (ex2 != null)
				{
					ex2.PreserveExceptionStack();
					throw ex2;
				}
				throw;
			}
		}

		private static GetterDelegate ConfigurationXMLGetterDelegate<T>(ADPropertyDefinition configXmlRawProperty) where T : XMLSerializableBase
		{
			return delegate(IPropertyBag bag)
			{
				string serializedXML = (string)bag[configXmlRawProperty];
				return XMLSerializableBase.Deserialize<T>(serializedXML, configXmlRawProperty);
			};
		}

		private static SetterDelegate ConfigurationXMLSetterDelegate<T>(ADPropertyDefinition configXmlRawProperty) where T : XMLSerializableBase
		{
			return delegate(object value, IPropertyBag bag)
			{
				XMLSerializableBase xmlserializableBase = value as XMLSerializableBase;
				string value2 = null;
				if (xmlserializableBase != null)
				{
					value2 = xmlserializableBase.Serialize(false);
				}
				bag[configXmlRawProperty] = value2;
			};
		}

		private static GetterDelegate XmlElementGetterDelegate<T, ValueT>(Func<T, ValueT> getDelegate, ProviderPropertyDefinition configXmlPropertyDefinition, ValueT defaultValue) where T : XMLSerializableBase
		{
			return delegate(IPropertyBag bag)
			{
				T t = (T)((object)bag[configXmlPropertyDefinition]);
				if (t == null)
				{
					return defaultValue;
				}
				return getDelegate(t);
			};
		}

		private static SetterDelegate XmlElementSetterDelegate<T, ValueT>(Action<T, ValueT> setDelegate, ProviderPropertyDefinition configXmlPropertyDefinition) where T : XMLSerializableBase, new()
		{
			return delegate(object value, IPropertyBag bag)
			{
				T t = (T)((object)bag[configXmlPropertyDefinition]);
				if (t == null)
				{
					t = Activator.CreateInstance<T>();
				}
				setDelegate(t, (ValueT)((object)value));
				bag[configXmlPropertyDefinition] = t;
			};
		}

		public const string ConfigXMLRawPropertyName = "ConfigurationXMLRaw";

		private const string ConfigXMLPropertyName = "ConfigurationXML";

		[NonSerialized]
		private XmlElement[] unknownElements;

		[NonSerialized]
		private XmlAttribute[] unknownAttributes;

		internal delegate bool TryParseDelegate<T>(string value, out T result);
	}
}
