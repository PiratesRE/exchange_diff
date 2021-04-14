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
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SimpleConfiguration<T> where T : new()
	{
		internal SimpleConfiguration()
		{
			Type typeFromHandle = typeof(T);
			if (!SimpleConfiguration<T>.simpleConfigurationTable.ContainsKey(typeFromHandle))
			{
				this.AddConfiguration(typeFromHandle);
			}
			this.configurationAttribute = SimpleConfiguration<T>.simpleConfigurationTable[typeFromHandle];
		}

		public IList<T> Entries
		{
			get
			{
				return this.entries;
			}
			set
			{
				this.entries = value;
			}
		}

		public void Load(CallContext callContext)
		{
			this.entries.Clear();
			using (UserConfiguration userConfiguration = this.GetUserConfiguration(this.configurationAttribute.ConfigurationName, callContext.SessionCache.GetMailboxIdentityMailboxSession()))
			{
				using (Stream xmlStream = userConfiguration.GetXmlStream())
				{
					if (xmlStream != null && xmlStream.Length > 0L)
					{
						this.reader = SafeXmlFactory.CreateSafeXmlTextReader(xmlStream);
						this.Parse(this.reader, callContext);
					}
				}
			}
		}

		public void Save(CallContext callContext)
		{
			if (callContext.SessionCache == null)
			{
				throw new InvalidOperationException("We cannot get the MailboxSession from the given callContext. Please make sure the callContext is not disposed when this method is called.");
			}
			this.Save(callContext.SessionCache.GetMailboxIdentityMailboxSession());
		}

		public void Save(MailboxSession mailboxSession)
		{
			using (UserConfiguration userConfiguration = this.GetUserConfiguration(this.configurationAttribute.ConfigurationName, mailboxSession))
			{
				using (Stream xmlStream = userConfiguration.GetXmlStream())
				{
					xmlStream.SetLength(0L);
					using (StreamWriter streamWriter = PendingRequestUtilities.CreateStreamWriter(xmlStream))
					{
						using (XmlTextWriter xmlTextWriter = new XmlTextWriter(streamWriter))
						{
							xmlTextWriter.WriteStartElement(this.configurationAttribute.ConfigurationRootNodeName);
							foreach (T t in this.entries)
							{
								SimpleConfigurationAttribute simpleConfigurationAttribute;
								lock (SimpleConfiguration<T>.simpleConfigurationTable)
								{
									simpleConfigurationAttribute = SimpleConfiguration<T>.simpleConfigurationTable[t.GetType()];
								}
								xmlTextWriter.WriteStartElement("entry");
								this.WriteCustomAttributes(xmlTextWriter, t);
								foreach (SimpleConfigurationPropertyAttribute simpleConfigurationPropertyAttribute in simpleConfigurationAttribute.GetPropertyCollection())
								{
									object value = simpleConfigurationPropertyAttribute.GetValue(t);
									if (value != null)
									{
										xmlTextWriter.WriteAttributeString(simpleConfigurationPropertyAttribute.Name, value.ToString());
									}
								}
								xmlTextWriter.WriteFullEndElement();
							}
							xmlTextWriter.WriteFullEndElement();
						}
					}
				}
				this.TrySaveConfiguration(userConfiguration, true);
			}
		}

		protected virtual void WriteCustomAttributes(XmlTextWriter xmlWriter, T entry)
		{
		}

		protected virtual void AddConfiguration(Type configurationType)
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

		protected virtual T ParseEntry(XmlTextReader reader)
		{
			Dictionary<string, string> attributes = this.ParseAttributes(reader);
			T t = this.CreateObject(attributes);
			this.SetAttributeValues(attributes, t);
			return t;
		}

		protected virtual T CreateObject(Dictionary<string, string> attributes)
		{
			if (default(T) != null)
			{
				return default(T);
			}
			return Activator.CreateInstance<T>();
		}

		private UserConfiguration GetUserConfiguration(string configurationName, MailboxSession mailboxSession)
		{
			if (string.IsNullOrEmpty(configurationName))
			{
				throw new ArgumentException("configurationName must not be null or empty");
			}
			UserConfiguration userConfiguration = null;
			try
			{
				userConfiguration = mailboxSession.UserConfigurationManager.GetMailboxConfiguration(configurationName, UserConfigurationTypes.XML);
			}
			catch (ObjectNotFoundException)
			{
				userConfiguration = mailboxSession.UserConfigurationManager.CreateMailboxConfiguration(configurationName, UserConfigurationTypes.XML);
				try
				{
					this.TrySaveConfiguration(userConfiguration, false);
				}
				catch (ObjectExistedException)
				{
					try
					{
						userConfiguration = mailboxSession.UserConfigurationManager.GetMailboxConfiguration(configurationName, UserConfigurationTypes.XML);
					}
					catch (ObjectNotFoundException thisObject)
					{
						throw new OwaSaveConflictException("A save conflict happened during the creation and save of the userconfiguration.", thisObject);
					}
				}
				catch (StoragePermanentException)
				{
				}
			}
			return userConfiguration;
		}

		private void TrySaveConfiguration(UserConfiguration configuration, bool ignoreStorePermanentExceptions)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			bool flag = false;
			bool flag2 = false;
			Exception ex = null;
			try
			{
				configuration.Save();
			}
			catch (StoragePermanentException ex2)
			{
				flag = true;
				ex = ex2;
				flag2 = true;
			}
			catch (StorageTransientException ex3)
			{
				flag = true;
				ex = ex3;
			}
			if (flag)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "UserConfigurationUtilities.TrySaveConfiguration: Failed. Exception: {0}", ex.Message);
				if (!ignoreStorePermanentExceptions && flag2)
				{
					throw ex;
				}
			}
		}

		private void Parse(XmlTextReader reader, CallContext callContext)
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
				this.Save(callContext);
			}
			catch (OwaConfigurationParserException ex2)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Mru parser threw an exception: {0}'", ex2.Message);
				this.entries.Clear();
				this.Save(callContext);
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

		private Dictionary<string, string> ParseAttributes(XmlTextReader reader)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(reader.AttributeCount);
			if (reader.HasAttributes)
			{
				for (int i = 0; i < reader.AttributeCount; i++)
				{
					reader.MoveToAttribute(i);
					dictionary[reader.Name] = reader.Value;
				}
				reader.MoveToElement();
			}
			return dictionary;
		}

		private void SetAttributeValues(Dictionary<string, string> attributes, object entry)
		{
			SimpleConfigurationAttribute simpleConfigurationAttribute;
			lock (SimpleConfiguration<T>.simpleConfigurationTable)
			{
				simpleConfigurationAttribute = SimpleConfiguration<T>.simpleConfigurationTable[entry.GetType()];
			}
			ulong num = simpleConfigurationAttribute.RequiredMask;
			foreach (KeyValuePair<string, string> keyValuePair in attributes)
			{
				SimpleConfigurationPropertyAttribute simpleConfigurationPropertyAttribute = simpleConfigurationAttribute.TryGetProperty(keyValuePair.Key);
				if (simpleConfigurationPropertyAttribute == null)
				{
					this.ThrowParserException();
				}
				object value = this.ConvertToStrongType(simpleConfigurationPropertyAttribute.Type, keyValuePair.Value);
				simpleConfigurationPropertyAttribute.SetValue(entry, value);
				if (simpleConfigurationPropertyAttribute.IsRequired)
				{
					num &= ~simpleConfigurationPropertyAttribute.PropertyMask;
				}
			}
			if (num != 0UL)
			{
				this.ThrowParserException();
			}
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
					return new ExDateTime(ExTimeZone.CurrentTimeZone, Convert.ToDateTime(value));
				}
				if (type == typeof(DateTime))
				{
					return Convert.ToDateTime(value);
				}
				if (type == typeof(bool))
				{
					return Convert.ToBoolean(value);
				}
				if (type.IsEnum)
				{
					return Enum.Parse(type, value);
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
