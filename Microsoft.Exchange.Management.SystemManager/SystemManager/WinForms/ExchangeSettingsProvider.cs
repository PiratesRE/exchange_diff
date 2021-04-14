using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.SnapIn;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeSettingsProvider : SettingsProvider, IApplicationSettingsProvider, ISettingsProviderService
	{
		public byte[] ByteData
		{
			get
			{
				MemoryStream memoryStream = new MemoryStream();
				byte[] result;
				try
				{
					BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
					binaryFormatter.Serialize(memoryStream, this.settingsStore);
					result = memoryStream.ToArray();
				}
				finally
				{
					memoryStream.Close();
				}
				return result;
			}
			set
			{
				if (value != null)
				{
					MemoryStream memoryStream = new MemoryStream(value);
					try
					{
						try
						{
							BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null, new string[]
							{
								"System.Collections.Hashtable"
							});
							Hashtable hashtable = (Hashtable)binaryFormatter.Deserialize(memoryStream);
							this.settingsStore = hashtable;
						}
						catch (Exception)
						{
							this.settingsStore = new Hashtable();
						}
						return;
					}
					finally
					{
						memoryStream.Close();
					}
				}
				this.settingsStore = new Hashtable();
			}
		}

		public override void Initialize(string name, NameValueCollection values)
		{
			if (string.IsNullOrEmpty(name))
			{
				name = this.ApplicationName;
			}
			base.Initialize(name, values);
		}

		public override string ApplicationName
		{
			get
			{
				return this.appName;
			}
			set
			{
				this.appName = value;
			}
		}

		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeSettingsProvider>(0L, "-->ExchangeSettingsProvider.GetPropertyValues: {0}", this);
			SettingsPropertyValueCollection settingsPropertyValueCollection = new SettingsPropertyValueCollection();
			string key = (string)context["SettingsKey"];
			IDictionary dictionary = (IDictionary)this.settingsStore[key];
			foreach (object obj in properties)
			{
				SettingsProperty settingsProperty = (SettingsProperty)obj;
				string name = settingsProperty.Name;
				SettingsPropertyValue settingsPropertyValue = new SettingsPropertyValue(settingsProperty);
				if (dictionary == null || !dictionary.Contains(name))
				{
					goto IL_114;
				}
				ExTraceGlobals.DataFlowTracer.TraceFunction<string, object, Type>(0L, "*--ExchangeSettingsProvider.GetPropertyValues: Converting and setting value: {0} = {1} as {2}", name, dictionary[name], settingsProperty.PropertyType);
				if (dictionary[name] != null || !settingsProperty.PropertyType.IsValueType)
				{
					try
					{
						settingsPropertyValue.PropertyValue = Convert.ChangeType(dictionary[name], settingsProperty.PropertyType);
						goto IL_24D;
					}
					catch (InvalidCastException arg)
					{
						ExTraceGlobals.DataFlowTracer.TraceError<InvalidCastException>(0L, "Exception in ExchangeSettingsProvider.GetPropertyValues: {0}", arg);
						settingsPropertyValue.PropertyValue = settingsPropertyValue.Property.DefaultValue;
						goto IL_24D;
					}
					goto IL_114;
				}
				settingsPropertyValue.PropertyValue = settingsPropertyValue.Property.DefaultValue;
				IL_24D:
				settingsPropertyValueCollection.Add(settingsPropertyValue);
				continue;
				IL_114:
				if (string.IsNullOrEmpty((string)settingsProperty.DefaultValue))
				{
					ExTraceGlobals.DataFlowTracer.TraceFunction<string, Type>(0L, "*--ExchangeSettingsProvider.GetPropertyValues: Setting to null: {0} as {1}", name, settingsProperty.PropertyType);
					settingsPropertyValue.PropertyValue = null;
					goto IL_24D;
				}
				if (typeof(Enum).IsAssignableFrom(settingsProperty.PropertyType))
				{
					ExTraceGlobals.DataFlowTracer.TraceFunction<string, object, Type>(0L, "*--ExchangeSettingsProvider.GetPropertyValues: Converting and setting enum value: {0} = {1} as {2}", name, settingsProperty.DefaultValue, settingsProperty.PropertyType);
					settingsPropertyValue.PropertyValue = EnumValidator.Parse(settingsProperty.PropertyType, (string)settingsProperty.DefaultValue, EnumParseOptions.IgnoreCase);
					goto IL_24D;
				}
				MethodInfo method = settingsProperty.PropertyType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[]
				{
					typeof(string)
				}, null);
				if (null != method)
				{
					ExTraceGlobals.DataFlowTracer.TraceFunction<string, object, Type>(0L, "*--ExchangeSettingsProvider.GetPropertyValues: Parsing value: {0} = {1} as {2}", name, settingsProperty.DefaultValue, settingsProperty.PropertyType);
					settingsPropertyValue.PropertyValue = method.Invoke(null, new object[]
					{
						settingsProperty.DefaultValue
					});
					goto IL_24D;
				}
				ExTraceGlobals.DataFlowTracer.TraceFunction<string, object, Type>(0L, "*--ExchangeSettingsProvider.GetPropertyValues: Using default value: {0} = {1} as {2}", name, settingsProperty.DefaultValue, settingsProperty.PropertyType);
				settingsPropertyValue.SerializedValue = settingsProperty.DefaultValue;
				goto IL_24D;
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeSettingsProvider>(0L, "<--ExchangeSettingsProvider.GetPropertyValues: {0}", this);
			return settingsPropertyValueCollection;
		}

		public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeSettingsProvider>(0L, "-->ExchangeSettingsProvider.SetPropertyValues: {0}", this);
			IDictionary dictionary = new Hashtable();
			foreach (object obj in values)
			{
				SettingsPropertyValue settingsPropertyValue = (SettingsPropertyValue)obj;
				ExTraceGlobals.DataFlowTracer.Information<string, object, Type>(0L, "ExchangeSettingsProvider.SetPropertyValues: {0} = {1} as {2}", settingsPropertyValue.Name, settingsPropertyValue.PropertyValue, settingsPropertyValue.Property.PropertyType);
				dictionary.Add(settingsPropertyValue.Name, settingsPropertyValue.PropertyValue);
			}
			string text = (string)context["SettingsKey"];
			this.settingsStore[text] = (PSConnectionInfoSingleton.GetInstance().Enabled ? dictionary : new Hashtable());
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeSettingsProvider, string>(0L, "<--ExchangeSettingsProvider.SetPropertyValues: {0}. settingsKey: {1}", this, text);
		}

		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeSettingsProvider>(0L, "*--ExchangeSettingsProvider.GetPreviousVersion: {0}", this);
			throw new NotSupportedException();
		}

		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public void Reset(SettingsContext context)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeSettingsProvider>(0L, "-->ExchangeSettingsProvider.Reset: {0}", this);
			string text = (string)context["SettingsKey"];
			if (this.settingsStore.ContainsKey(text))
			{
				IDictionary dictionary = (IDictionary)this.settingsStore[text];
				dictionary.Clear();
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeSettingsProvider, string>(0L, "<--ExchangeSettingsProvider.Reset: {0}. settingsKey: {1}", this, text);
		}

		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeSettingsProvider>(0L, "*--ExchangeSettingsProvider.Upgrade: {0}", this);
			throw new NotSupportedException();
		}

		public SettingsProvider GetSettingsProvider(SettingsProperty property)
		{
			if (property.Provider.GetType() == base.GetType())
			{
				return this;
			}
			return null;
		}

		private string appName = "ExchangeSettingsProvider";

		private Hashtable settingsStore = new Hashtable();
	}
}
