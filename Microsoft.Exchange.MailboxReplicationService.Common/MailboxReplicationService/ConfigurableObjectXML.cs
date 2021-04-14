using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class ConfigurableObjectXML : XMLSerializableBase
	{
		static ConfigurableObjectXML()
		{
			ConfigurableObjectXML.PropertiesNotToSerialize.Add(ADRecipientSchema.Certificate, null);
			ConfigurableObjectXML.PropertiesNotToSerialize.Add(ADRecipientSchema.SMimeCertificate, null);
			ConfigurableObjectXML.PropertiesNotToSerialize.Add(ADUserSchema.DirectReports, null);
			ConfigurableObjectXML.PropertiesNotToSerialize.Add(ADRecipientSchema.AcceptMessagesOnlyFrom, null);
			ConfigurableObjectXML.PropertiesNotToSerialize.Add(ADRecipientSchema.RejectMessagesFrom, null);
			ConfigurableObjectXML.PropertiesNotToSerialize.Add(ADRecipientSchema.AcceptMessagesOnlyFromDLMembers, null);
			ConfigurableObjectXML.PropertiesNotToSerialize.Add(ADRecipientSchema.RejectMessagesFromDLMembers, null);
			ConfigurableObjectXML.PropertiesNotToSerialize.Add(ADRecipientSchema.AddressListMembership, null);
			ConfigurableObjectXML.PropertiesNotToSerialize.Add(ADRecipientSchema.GrantSendOnBehalfTo, null);
			ConfigurableObjectXML.PropertiesNotToSerialize.Add(ADMailboxRecipientSchema.DelegateListBL, null);
			ConfigurableObjectXML.PropertiesNotToSerialize.Add(ADMailboxRecipientSchema.DelegateListLink, null);
			ConfigurableObjectXML.PropertiesNotToSerialize.Add(ADUserSchema.SharingPartnerIdentities, null);
			ConfigurableObjectXML.PropertiesNotToSerialize.Add(ADUserSchema.SharingAnonymousIdentities, null);
		}

		public ConfigurableObjectXML()
		{
			this.properties = new Dictionary<string, PropertyXML>();
		}

		[XmlAttribute("Class")]
		public string ClassName { get; set; }

		[XmlElement("Property")]
		public PropertyXML[] Props
		{
			get
			{
				PropertyXML[] array = new PropertyXML[this.properties.Count];
				this.properties.Values.CopyTo(array, 0);
				return array;
			}
			set
			{
				this.properties.Clear();
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						PropertyXML propertyXML = value[i];
						this.properties[propertyXML.PropertyName] = propertyXML;
					}
				}
			}
		}

		internal object this[string propName]
		{
			get
			{
				PropertyXML propertyXML;
				if (!this.properties.TryGetValue(propName, out propertyXML))
				{
					return null;
				}
				PropertyValueBaseXML[] values = propertyXML.Values;
				if (values.Length == 0)
				{
					return null;
				}
				if (values.Length == 1)
				{
					return values[0].RawValue;
				}
				object[] array = new object[values.Length];
				for (int i = 0; i < values.Length; i++)
				{
					array[i] = values[i].RawValue;
				}
				return array;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			SortedList<string, PropertyXML> sortedList = new SortedList<string, PropertyXML>();
			foreach (PropertyXML propertyXML in this.properties.Values)
			{
				if (propertyXML.HasValue())
				{
					sortedList.Add(propertyXML.PropertyName, propertyXML);
				}
			}
			foreach (PropertyXML propertyXML2 in sortedList.Values)
			{
				stringBuilder.AppendFormat("{0}; ", propertyXML2.ToString());
			}
			return stringBuilder.ToString();
		}

		internal static ConfigurableObjectXML Create(ConfigurableObject obj)
		{
			if (obj == null)
			{
				return null;
			}
			ConfigurableObjectXML configurableObjectXML = new ConfigurableObjectXML();
			configurableObjectXML.ClassName = obj.GetType().Name;
			foreach (PropertyDefinition propertyDefinition in obj.ObjectSchema.AllProperties)
			{
				ProviderPropertyDefinition providerPropertyDefinition = propertyDefinition as ProviderPropertyDefinition;
				if (providerPropertyDefinition != null && !ConfigurableObjectXML.PropertiesNotToSerialize.ContainsKey(propertyDefinition))
				{
					object value = obj[providerPropertyDefinition];
					PropertyXML propertyXML = PropertyXML.Create(providerPropertyDefinition, value);
					if (propertyXML != null)
					{
						configurableObjectXML.properties[providerPropertyDefinition.Name] = propertyXML;
					}
				}
			}
			return configurableObjectXML;
		}

		internal static string Serialize<T>(T obj) where T : ConfigurableObject
		{
			ConfigurableObjectXML configurableObjectXML = ConfigurableObjectXML.Create(obj);
			if (configurableObjectXML == null)
			{
				return null;
			}
			return configurableObjectXML.Serialize(false);
		}

		internal static T Deserialize<T>(string xml) where T : ConfigurableObject, new()
		{
			ConfigurableObjectXML configurableObjectXML = ConfigurableObjectXML.Parse(xml);
			if (configurableObjectXML == null)
			{
				return default(T);
			}
			T t = Activator.CreateInstance<T>();
			configurableObjectXML.Populate(t);
			return t;
		}

		internal static ConfigurableObjectXML Parse(string serializedData)
		{
			return XMLSerializableBase.Deserialize<ConfigurableObjectXML>(serializedData, false);
		}

		internal void Populate(ConfigurableObject result)
		{
			if (result == null)
			{
				return;
			}
			foreach (PropertyDefinition propertyDefinition in result.ObjectSchema.AllProperties)
			{
				ProviderPropertyDefinition providerPropertyDefinition = propertyDefinition as ProviderPropertyDefinition;
				PropertyXML propertyXML;
				if (providerPropertyDefinition != null && !providerPropertyDefinition.IsCalculated && !providerPropertyDefinition.IsReadOnly && this.properties.TryGetValue(providerPropertyDefinition.Name, out propertyXML))
				{
					propertyXML.TryApplyChange(providerPropertyDefinition, result, PropertyUpdateOperation.Replace);
				}
			}
		}

		private static readonly Dictionary<PropertyDefinition, object> PropertiesNotToSerialize = new Dictionary<PropertyDefinition, object>();

		private Dictionary<string, PropertyXML> properties;
	}
}
