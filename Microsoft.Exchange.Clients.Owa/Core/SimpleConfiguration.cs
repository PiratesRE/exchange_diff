using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class SimpleConfiguration<T> where T : new()
	{
		internal SimpleConfiguration(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			this.userContext = userContext;
			Type typeFromHandle = typeof(T);
			if (!SimpleConfiguration<T>.simpleConfigurationTable.ContainsKey(typeFromHandle))
			{
				SimpleConfiguration<T>.AddConfiguration(typeFromHandle);
			}
			this.configurationAttribute = SimpleConfiguration<T>.simpleConfigurationTable[typeFromHandle];
		}

		public void Load()
		{
			this.entries.Clear();
			using (UserConfiguration userConfiguration = UserConfigurationUtilities.GetUserConfiguration(this.configurationAttribute.ConfigurationName, this.userContext))
			{
				using (Stream xmlStream = userConfiguration.GetXmlStream())
				{
					if (xmlStream != null && xmlStream.Length > 0L)
					{
						this.reader = SafeXmlFactory.CreateSafeXmlTextReader(xmlStream);
						this.Parse(this.reader);
					}
				}
			}
		}

		public void Save()
		{
			using (UserConfiguration userConfiguration = UserConfigurationUtilities.GetUserConfiguration(this.configurationAttribute.ConfigurationName, this.userContext))
			{
				using (Stream xmlStream = userConfiguration.GetXmlStream())
				{
					xmlStream.SetLength(0L);
					using (StreamWriter streamWriter = Utilities.CreateStreamWriter(xmlStream))
					{
						using (XmlTextWriter xmlTextWriter = new XmlTextWriter(streamWriter))
						{
							xmlTextWriter.WriteStartElement(this.configurationAttribute.ConfigurationRootNodeName);
							foreach (T t in this.entries)
							{
								xmlTextWriter.WriteStartElement("entry");
								foreach (SimpleConfigurationPropertyAttribute simpleConfigurationPropertyAttribute in this.configurationAttribute.GetPropertyCollection())
								{
									xmlTextWriter.WriteAttributeString(simpleConfigurationPropertyAttribute.Name, simpleConfigurationPropertyAttribute.GetValue(t).ToString());
								}
								xmlTextWriter.WriteFullEndElement();
							}
							xmlTextWriter.WriteFullEndElement();
						}
					}
				}
				UserConfigurationUtilities.TrySaveConfiguration(userConfiguration);
			}
		}

		public IList<T> Entries
		{
			get
			{
				return this.entries;
			}
		}

		private static void AddConfiguration(Type configurationType)
		{
			lock (SimpleConfiguration<T>.simpleConfigurationTable)
			{
				if (!SimpleConfiguration<T>.simpleConfigurationTable.ContainsKey(configurationType))
				{
					object[] customAttributes = configurationType.GetCustomAttributes(typeof(SimpleConfigurationAttribute), false);
					if (customAttributes == null || customAttributes.Length == 0)
					{
						throw new OwaNotSupportedException("A SimpleConfigurationAttribute should be defined on the type");
					}
					SimpleConfigurationAttribute simpleConfigurationAttribute = (SimpleConfigurationAttribute)customAttributes[0];
					PropertyInfo[] properties = configurationType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
					foreach (PropertyInfo propertyInfo in properties)
					{
						object[] customAttributes2 = propertyInfo.GetCustomAttributes(typeof(SimpleConfigurationPropertyAttribute), false);
						if (customAttributes2 != null && customAttributes2.Length != 0)
						{
							SimpleConfigurationPropertyAttribute simpleConfigurationPropertyAttribute = (SimpleConfigurationPropertyAttribute)customAttributes2[0];
							simpleConfigurationPropertyAttribute.PropertyInfo = propertyInfo;
							simpleConfigurationAttribute.AddProperty(simpleConfigurationPropertyAttribute);
						}
					}
					SimpleConfiguration<T>.simpleConfigurationTable.Add(configurationType, simpleConfigurationAttribute);
				}
			}
		}

		private void Parse(XmlTextReader reader)
		{
			try
			{
				reader.WhitespaceHandling = WhitespaceHandling.All;
				this.state = SimpleConfiguration<T>.XmlParseState.Start;
				while (this.state != SimpleConfiguration<T>.XmlParseState.Finished && reader.Read())
				{
					switch (this.state)
					{
					case SimpleConfiguration<T>.XmlParseState.Start:
						this.ParseStart(reader);
						break;
					case SimpleConfiguration<T>.XmlParseState.Root:
						this.ParseRoot(reader);
						break;
					case SimpleConfiguration<T>.XmlParseState.Child:
						this.ParseChild(reader);
						break;
					}
				}
			}
			catch (XmlException ex)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Parser threw an XML exception: {0}'", ex.Message);
				this.entries.Clear();
				this.Save();
			}
			catch (OwaConfigurationParserException ex2)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Mru parser threw an exception: {0}'", ex2.Message);
				this.entries.Clear();
				this.Save();
			}
		}

		private void ParseStart(XmlTextReader reader)
		{
			if (XmlNodeType.Element != reader.NodeType || string.CompareOrdinal(this.configurationAttribute.ConfigurationRootNodeName, reader.Name) != 0)
			{
				this.ThrowParserException();
				return;
			}
			if (reader.IsEmptyElement)
			{
				this.state = SimpleConfiguration<T>.XmlParseState.Finished;
				return;
			}
			this.state = SimpleConfiguration<T>.XmlParseState.Root;
		}

		private void ParseRoot(XmlTextReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element)
			{
				if (reader.IsEmptyElement)
				{
					this.ThrowParserException();
					return;
				}
				if (string.CompareOrdinal("entry", reader.Name) == 0)
				{
					T item = this.ParseEntry(reader);
					this.entries.Add(item);
					this.state = SimpleConfiguration<T>.XmlParseState.Child;
					return;
				}
				this.ThrowParserException();
				return;
			}
			else
			{
				if (reader.NodeType == XmlNodeType.EndElement && string.CompareOrdinal(this.configurationAttribute.ConfigurationRootNodeName, reader.Name) == 0)
				{
					this.state = SimpleConfiguration<T>.XmlParseState.Finished;
					return;
				}
				this.ThrowParserException();
				return;
			}
		}

		private void ParseChild(XmlTextReader reader)
		{
			if (reader.NodeType == XmlNodeType.EndElement && string.CompareOrdinal(reader.Name, "entry") == 0)
			{
				this.state = SimpleConfiguration<T>.XmlParseState.Root;
				return;
			}
			this.ThrowParserException();
		}

		private T ParseEntry(XmlTextReader reader)
		{
			T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			ulong num = this.configurationAttribute.RequiredMask;
			if (reader.HasAttributes)
			{
				for (int i = 0; i < reader.AttributeCount; i++)
				{
					reader.MoveToAttribute(i);
					SimpleConfigurationPropertyAttribute simpleConfigurationPropertyAttribute = this.configurationAttribute.TryGetProperty(reader.Name);
					if (simpleConfigurationPropertyAttribute == null)
					{
						this.ThrowParserException();
					}
					object value = this.ConvertToStrongType(simpleConfigurationPropertyAttribute.Type, reader.Value);
					simpleConfigurationPropertyAttribute.SetValue(t, value);
					if (simpleConfigurationPropertyAttribute.IsRequired)
					{
						num &= ~simpleConfigurationPropertyAttribute.PropertyMask;
					}
				}
				reader.MoveToElement();
			}
			if (num != 0UL)
			{
				this.ThrowParserException();
			}
			return t;
		}

		private object ConvertToStrongType(Type type, string value)
		{
			try
			{
				if (type == typeof(string))
				{
					return value;
				}
				if (type == typeof(int))
				{
					return int.Parse(value, CultureInfo.InvariantCulture);
				}
				if (type == typeof(long))
				{
					return long.Parse(value, CultureInfo.InvariantCulture);
				}
				if (type == typeof(double))
				{
					return double.Parse(value, CultureInfo.InvariantCulture);
				}
				if (type == typeof(ExDateTime))
				{
					return new ExDateTime(this.userContext.TimeZone, Convert.ToDateTime(value));
				}
				if (type == typeof(DateTime))
				{
					return Convert.ToDateTime(value);
				}
				if (type == typeof(bool))
				{
					return Convert.ToBoolean(value);
				}
				this.ThrowParserException(string.Format("Internal error: unsupported type : {0}", type));
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}
			this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Failed to parse type. Type = {0}, Value = {1}", new object[]
			{
				type,
				value
			}));
			return null;
		}

		private void ThrowParserException()
		{
			this.ThrowParserException(null);
		}

		private void ThrowParserException(string description)
		{
			int num = 0;
			int num2 = 0;
			if (this.reader != null)
			{
				num = this.reader.LineNumber;
				num2 = this.reader.LinePosition;
			}
			throw new OwaConfigurationParserException(string.Format(CultureInfo.InvariantCulture, "Invalid simple configuration. Line number: {0} Position: {1}.{2}", new object[]
			{
				num.ToString(CultureInfo.InvariantCulture),
				num2.ToString(CultureInfo.InvariantCulture),
				(description != null) ? (" " + description) : string.Empty
			}), null, this);
		}

		private const string EntryNodeName = "entry";

		private static Dictionary<Type, SimpleConfigurationAttribute> simpleConfigurationTable = new Dictionary<Type, SimpleConfigurationAttribute>();

		private IList<T> entries = new List<T>();

		private SimpleConfigurationAttribute configurationAttribute;

		private UserContext userContext;

		private SimpleConfiguration<T>.XmlParseState state;

		private XmlTextReader reader;

		private enum XmlParseState
		{
			Start,
			Root,
			Child,
			Finished
		}
	}
}
