using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class PropertyXML : XMLSerializableBase
	{
		public PropertyXML()
		{
			this.values = new List<PropertyValueBaseXML>();
		}

		[XmlAttribute("Name")]
		public string PropertyName { get; set; }

		[XmlAttribute("Class")]
		public string ClassName { get; set; }

		[XmlAttribute("IsDefault")]
		public string IsDefault
		{
			get
			{
				if (!this.isDefault)
				{
					return null;
				}
				return "true";
			}
			set
			{
				this.isDefault = !string.IsNullOrEmpty(value);
			}
		}

		[XmlElement(typeof(PropertyStringValueXML), ElementName = "Value")]
		[XmlElement(typeof(ADObjectIdXML), ElementName = "DNValue")]
		[XmlElement(typeof(PropertyBinaryValueXML), ElementName = "BinValue")]
		[XmlElement(typeof(OrganizationIdXML), ElementName = "OrgIdValue")]
		public PropertyValueBaseXML[] Values
		{
			get
			{
				return this.values.ToArray();
			}
			set
			{
				this.values.Clear();
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						PropertyValueBaseXML item = value[i];
						this.values.Add(item);
					}
				}
			}
		}

		public override string ToString()
		{
			if (this.values.Count == 0)
			{
				return string.Format("{0}: ", this.PropertyName);
			}
			if (this.values.Count != 1)
			{
				return string.Format("{0}: {1}", this.PropertyName, CommonUtils.ConcatEntries<PropertyValueBaseXML>(this.values, null));
			}
			PropertyValueBaseXML propertyValueBaseXML = this.values[0];
			if (propertyValueBaseXML is PropertyBinaryValueXML && this.PropertyName.ToUpper().EndsWith("GUID"))
			{
				return string.Format("{0}: {1}", this.PropertyName, ((PropertyBinaryValueXML)propertyValueBaseXML).AsGuid);
			}
			return string.Format("{0}: {1}", this.PropertyName, propertyValueBaseXML);
		}

		internal static PropertyXML Create(ProviderPropertyDefinition pdef, object value)
		{
			PropertyXML propertyXML = new PropertyXML();
			propertyXML.PropertyName = pdef.Name;
			propertyXML.ClassName = pdef.Type.FullName;
			propertyXML.values = new List<PropertyValueBaseXML>();
			propertyXML.isDefault = object.Equals(value, pdef.DefaultValue);
			if (pdef.IsMultivalued && value is MultiValuedPropertyBase)
			{
				MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)value;
				using (IEnumerator enumerator = ((IEnumerable)multiValuedPropertyBase).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object value2 = enumerator.Current;
						PropertyValueBaseXML propertyValueBaseXML = PropertyValueBaseXML.Create(pdef, value2);
						if (propertyValueBaseXML != null)
						{
							propertyXML.values.Add(propertyValueBaseXML);
						}
					}
					return propertyXML;
				}
			}
			if (value != null)
			{
				PropertyValueBaseXML propertyValueBaseXML2 = PropertyValueBaseXML.Create(pdef, value);
				if (propertyValueBaseXML2 != null)
				{
					propertyXML.values.Add(propertyValueBaseXML2);
				}
			}
			return propertyXML;
		}

		internal bool HasValue()
		{
			foreach (PropertyValueBaseXML propertyValueBaseXML in this.values)
			{
				if (propertyValueBaseXML.HasValue())
				{
					return true;
				}
			}
			return false;
		}

		internal bool TryGetValue(ProviderPropertyDefinition pdef, out object result)
		{
			if (this.values.Count == 0)
			{
				result = null;
				return true;
			}
			if (pdef.IsMultivalued)
			{
				List<object> list = new List<object>(this.values.Count);
				foreach (PropertyValueBaseXML propertyValueBaseXML in this.values)
				{
					object item;
					if (propertyValueBaseXML.TryGetValue(pdef, out item))
					{
						list.Add(item);
					}
				}
				MultiValuedPropertyBase multiValuedPropertyBase = null;
				List<object> invalidValues = new List<object>();
				if (!ADValueConvertor.TryCreateGenericMultiValuedProperty(pdef, false, list, invalidValues, null, out multiValuedPropertyBase))
				{
					MrsTracer.Common.Warning("Failed to convert array to MultiValued property {0}", new object[]
					{
						pdef.Name
					});
					result = null;
					return false;
				}
				result = multiValuedPropertyBase;
				return true;
			}
			else
			{
				if (this.values.Count != 1)
				{
					result = null;
					return false;
				}
				return this.values[0].TryGetValue(pdef, out result);
			}
		}

		internal bool TryApplyChange(ProviderPropertyDefinition pdef, ConfigurableObject targetObject, PropertyUpdateOperation op)
		{
			object obj = null;
			if (this.TryGetValue(pdef, out obj))
			{
				Exception ex = null;
				try
				{
					if (op == PropertyUpdateOperation.Replace)
					{
						object obj2 = targetObject[pdef];
						if ((!object.Equals(obj, pdef.DefaultValue) || obj2 != null) && !object.Equals(obj2, obj))
						{
							if (pdef == ADObjectSchema.ExchangeVersion)
							{
								targetObject.SetExchangeVersion((ExchangeObjectVersion)obj);
							}
							else
							{
								targetObject[pdef] = obj;
							}
						}
					}
					else
					{
						MultiValuedPropertyBase multiValuedPropertyBase = obj as MultiValuedPropertyBase;
						MultiValuedPropertyBase multiValuedPropertyBase2 = targetObject[pdef] as MultiValuedPropertyBase;
						if (multiValuedPropertyBase != null && multiValuedPropertyBase2 != null)
						{
							foreach (object item in ((IEnumerable)multiValuedPropertyBase))
							{
								switch (op)
								{
								case PropertyUpdateOperation.AddValues:
									multiValuedPropertyBase2.Add(item);
									break;
								case PropertyUpdateOperation.RemoveValues:
									multiValuedPropertyBase2.Remove(item);
									break;
								}
							}
						}
					}
					return true;
				}
				catch (DataValidationException ex2)
				{
					ex = ex2;
				}
				catch (ArgumentException ex3)
				{
					ex = ex3;
				}
				catch (FormatException ex4)
				{
					ex = ex4;
				}
				catch (LocalizedException ex5)
				{
					ex = ex5;
				}
				if (ex != null)
				{
					MrsTracer.Common.Warning("Property {0} could not be set to '{1}', error {2}", new object[]
					{
						pdef.Name,
						obj,
						CommonUtils.FullExceptionMessage(ex)
					});
					return false;
				}
				return false;
			}
			return false;
		}

		private List<PropertyValueBaseXML> values;

		private bool isDefault;
	}
}
