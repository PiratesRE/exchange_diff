using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class MbxPropertyDefinition : SimpleProviderPropertyDefinition
	{
		public PropTag PropTag { get; private set; }

		internal MbxPropertyDefinition(string name, PropTag propTag, Type type, PropertyDefinitionFlags flags, object defaultValue, ProviderPropertyDefinition[] supportingProperties, GetterDelegate getterDelegate = null, SetterDelegate setterDelegate = null) : this(name, propTag, ExchangeObjectVersion.Exchange2003, type, flags, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, supportingProperties, getterDelegate, setterDelegate)
		{
		}

		internal MbxPropertyDefinition(string name, PropTag propTag, ExchangeObjectVersion versionAdded, Type type, PropertyDefinitionFlags flags, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints, ProviderPropertyDefinition[] supportingProperties, GetterDelegate getterDelegate, SetterDelegate setterDelegate) : base(name, versionAdded, type, flags, defaultValue, readConstraints, writeConstraints, supportingProperties, null, getterDelegate, setterDelegate)
		{
			this.PropTag = propTag;
			if (propTag == PropTag.Null && name == "Null")
			{
				throw new ArgumentException("Name should not be 'Null' is PropTag is Null");
			}
			if (propTag == PropTag.Null != this.IsCalculated)
			{
				throw new ArgumentException("PropTag must be Null IFF Calculated property");
			}
		}

		internal static MbxPropertyDefinition NullableBoolPropertyDefinition(PropTag propTag, string name = null, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name ?? propTag.ToString(), propTag, ExchangeObjectVersion.Exchange2003, typeof(bool?), multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null);
		}

		internal static MbxPropertyDefinition BoolFromNullableBoolPropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(bool), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, (IPropertyBag propertyBag) => propertyBag[rawPropertyDefinition] != null && (bool)propertyBag[rawPropertyDefinition], delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = value;
			});
		}

		internal static MbxPropertyDefinition StringPropertyDefinition(PropTag propTag, string name = null, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name ?? propTag.ToString(), propTag, ExchangeObjectVersion.Exchange2003, typeof(string), multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null);
		}

		internal static MbxPropertyDefinition SmtpDomainFromStringPropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(SmtpDomain), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, delegate(IPropertyBag propertyBag)
			{
				if (propertyBag[rawPropertyDefinition] != null)
				{
					return SmtpDomain.Parse((string)propertyBag[rawPropertyDefinition]);
				}
				return null;
			}, delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = ((SmtpDomain)value).ToString();
			});
		}

		internal static MbxPropertyDefinition CountryInfoFromStringPropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(CountryInfo), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, multivalued ? null : null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, delegate(IPropertyBag propertyBag)
			{
				if (propertyBag[rawPropertyDefinition] != null)
				{
					return CountryInfo.Parse((string)propertyBag[rawPropertyDefinition]);
				}
				return null;
			}, delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = ((CountryInfo)value).ToString();
			});
		}

		internal static MbxPropertyDefinition ProxyAddressFromStringPropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			GetterDelegate getterDelegate = delegate(IPropertyBag propertyBag)
			{
				if (propertyBag[rawPropertyDefinition] != null)
				{
					return ProxyAddress.Parse((string)propertyBag[rawPropertyDefinition]);
				}
				return null;
			};
			SetterDelegate setterDelegate = delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = ((ProxyAddress)value).ToString();
			};
			GetterDelegate getterDelegate2 = delegate(IPropertyBag propertyBag)
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[rawPropertyDefinition];
				MultiValuedProperty<ProxyAddress> multiValuedProperty2 = new MultiValuedProperty<ProxyAddress>();
				foreach (string proxyAddressString in multiValuedProperty)
				{
					multiValuedProperty2.Add(ProxyAddress.Parse(proxyAddressString));
				}
				return multiValuedProperty2;
			};
			SetterDelegate setterDelegate2 = delegate(object value, IPropertyBag propertyBag)
			{
				MultiValuedProperty<ProxyAddress> multiValuedProperty = (MultiValuedProperty<ProxyAddress>)value;
				MultiValuedProperty<string> multiValuedProperty2 = (MultiValuedProperty<string>)propertyBag[rawPropertyDefinition];
				for (int i = multiValuedProperty2.Count - 1; i >= 0; i--)
				{
					if (!string.IsNullOrEmpty(multiValuedProperty2[i]))
					{
						multiValuedProperty2.RemoveAt(i);
					}
				}
				foreach (ProxyAddress proxyAddress in multiValuedProperty)
				{
					multiValuedProperty2.Add(proxyAddress.ToString());
				}
			};
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(ProxyAddress), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, multivalued ? null : null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, multivalued ? getterDelegate2 : getterDelegate, multivalued ? setterDelegate2 : setterDelegate);
		}

		internal static MbxPropertyDefinition SmtpAddressFromStringPropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(SmtpAddress), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, multivalued ? null : default(SmtpAddress), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, (IPropertyBag propertyBag) => (propertyBag[rawPropertyDefinition] == null) ? default(SmtpAddress) : SmtpAddress.Parse((string)propertyBag[rawPropertyDefinition]), delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = ((SmtpAddress)value).ToString();
			});
		}

		internal static MbxPropertyDefinition CultureInfoFromStringPropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(CultureInfo), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, multivalued ? null : null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, delegate(IPropertyBag propertyBag)
			{
				if (propertyBag[rawPropertyDefinition] != null)
				{
					return new CultureInfo((string)propertyBag[rawPropertyDefinition]);
				}
				return null;
			}, delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = ((CultureInfo)value).ToString();
			});
		}

		internal static MbxPropertyDefinition ByteQuantifiedSizeFromStringPropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(ByteQuantifiedSize?), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, delegate(IPropertyBag propertyBag)
			{
				if (propertyBag[rawPropertyDefinition] != null)
				{
					return ByteQuantifiedSize.Parse((string)propertyBag[rawPropertyDefinition]);
				}
				return null;
			}, delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = ((ByteQuantifiedSize)value).ToString();
			});
		}

		internal static MbxPropertyDefinition UnlimitedByteQuantifiedSizeFromStringPropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(Unlimited<ByteQuantifiedSize>), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, multivalued ? null : default(Unlimited<ByteQuantifiedSize>), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, (IPropertyBag propertyBag) => (propertyBag[rawPropertyDefinition] == null) ? default(Unlimited<ByteQuantifiedSize>) : Unlimited<ByteQuantifiedSize>.Parse((string)propertyBag[rawPropertyDefinition]), delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = ((Unlimited<ByteQuantifiedSize>)value).ToString();
			});
		}

		internal static MbxPropertyDefinition UnlimitedInt32FromStringPropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(Unlimited<int>), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, multivalued ? null : default(Unlimited<int>), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, (IPropertyBag propertyBag) => (propertyBag[rawPropertyDefinition] == null) ? default(Unlimited<int>) : Unlimited<int>.Parse((string)propertyBag[rawPropertyDefinition]), delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = ((Unlimited<int>)value).ToString();
			});
		}

		internal static MbxPropertyDefinition TextMessagingStateBaseFromStringPropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(TextMessagingStateBase), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, multivalued ? null : null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, delegate(IPropertyBag propertyBag)
			{
				if (propertyBag[rawPropertyDefinition] != null)
				{
					return TextMessagingStateBase.ParseFromADString((string)propertyBag[rawPropertyDefinition]);
				}
				return null;
			}, delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = ((TextMessagingStateBase)value).ToADString();
			});
		}

		internal static MbxPropertyDefinition NullableInt32PropertyDefinition(PropTag propTag, string name = null, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name ?? propTag.ToString(), propTag, ExchangeObjectVersion.Exchange2003, typeof(int?), multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null);
		}

		internal static MbxPropertyDefinition Int32FromNullableInt32PropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			GetterDelegate getterDelegate = (IPropertyBag propertyBag) => (propertyBag[rawPropertyDefinition] == null) ? 0 : ((int)propertyBag[rawPropertyDefinition]);
			SetterDelegate setterDelegate = delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = (((int?)value) ?? 0);
			};
			GetterDelegate getterDelegate2 = delegate(IPropertyBag propertyBag)
			{
				MultiValuedProperty<int> multiValuedProperty = (MultiValuedProperty<int>)propertyBag[rawPropertyDefinition];
				MultiValuedProperty<int> multiValuedProperty2 = new MultiValuedProperty<int>();
				foreach (int value in multiValuedProperty)
				{
					int? num = new int?(value);
					if (num != null)
					{
						multiValuedProperty2.Add(num.Value);
					}
				}
				return multiValuedProperty2;
			};
			SetterDelegate setterDelegate2 = delegate(object value, IPropertyBag propertyBag)
			{
				MultiValuedProperty<int> multiValuedProperty = (MultiValuedProperty<int>)value;
				MultiValuedProperty<int> multiValuedProperty2 = (MultiValuedProperty<int>)propertyBag[rawPropertyDefinition];
				for (int i = multiValuedProperty2.Count - 1; i >= 0; i--)
				{
					multiValuedProperty2.RemoveAt(i);
				}
				foreach (int item in multiValuedProperty)
				{
					multiValuedProperty2.Add(item);
				}
			};
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(int), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, multivalued ? null : 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, multivalued ? getterDelegate2 : getterDelegate, multivalued ? setterDelegate2 : setterDelegate);
		}

		internal static MbxPropertyDefinition EnumFromNullableInt32PropertyDefinition<T>(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false) where T : struct
		{
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(T), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, multivalued ? null : default(T), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, (IPropertyBag propertyBag) => (propertyBag[rawPropertyDefinition] == null) ? default(T) : ((T)((object)propertyBag[rawPropertyDefinition])), delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = (int)value;
			});
		}

		internal static MbxPropertyDefinition NullableEnumFromNullableInt32PropertyDefinition<T>(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false) where T : struct
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enum type");
			}
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(T?), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, (IPropertyBag propertyBag) => (T?)propertyBag[rawPropertyDefinition], delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = value;
			});
		}

		internal static MbxPropertyDefinition BinaryPropertyDefinition(PropTag propTag, string name = null, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name ?? propTag.ToString(), propTag, ExchangeObjectVersion.Exchange2003, typeof(byte[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null);
		}

		internal static MbxPropertyDefinition GeoCoordinatesFromStringPropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(GeoCoordinates), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, multivalued ? null : null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, delegate(IPropertyBag propertyBag)
			{
				if (propertyBag[rawPropertyDefinition] != null)
				{
					return new GeoCoordinates((string)propertyBag[rawPropertyDefinition]);
				}
				return null;
			}, delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = ((GeoCoordinates)value).ToString();
			});
		}

		internal static MbxPropertyDefinition NullableDateTimePropertyDefinition(PropTag propTag, string name = null, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name ?? propTag.ToString(), propTag, ExchangeObjectVersion.Exchange2003, typeof(DateTime?), multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null);
		}

		internal static MbxPropertyDefinition DateTimeFromNullableDateTimePropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(DateTime), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, multivalued ? null : default(DateTime), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, (IPropertyBag propertyBag) => (propertyBag[rawPropertyDefinition] == null) ? default(DateTime) : ((DateTime)propertyBag[rawPropertyDefinition]), delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = value;
			});
		}

		internal static MbxPropertyDefinition NullableInt64PropertyDefinition(PropTag propTag, string name = null, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name ?? propTag.ToString(), propTag, ExchangeObjectVersion.Exchange2003, typeof(long?), multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null);
		}

		internal static MbxPropertyDefinition EnhancedTimeSpanFromNullableInt64PropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(EnhancedTimeSpan), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, multivalued ? null : EnhancedTimeSpan.FromDays(90.0), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, (IPropertyBag propertyBag) => (propertyBag[rawPropertyDefinition] == null) ? default(EnhancedTimeSpan) : new TimeSpan((long)propertyBag[rawPropertyDefinition]), delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = ((TimeSpan)value).Ticks;
			});
		}

		internal static MbxPropertyDefinition NullableGuidPropertyDefinition(PropTag propTag, string name = null, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name ?? propTag.ToString(), propTag, ExchangeObjectVersion.Exchange2003, typeof(Guid?), multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null);
		}

		internal static MbxPropertyDefinition GuidFromNullableGuidPropertyDefinition(string name, MbxPropertyDefinition rawPropertyDefinition, bool multivalued = false)
		{
			return new MbxPropertyDefinition(name, PropTag.Null, ExchangeObjectVersion.Exchange2003, typeof(Guid), (multivalued ? PropertyDefinitionFlags.MultiValued : PropertyDefinitionFlags.None) | PropertyDefinitionFlags.Calculated, multivalued ? null : Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new MbxPropertyDefinition[]
			{
				rawPropertyDefinition
			}, (IPropertyBag propertyBag) => (propertyBag[rawPropertyDefinition] == null) ? Guid.Empty : ((Guid)propertyBag[rawPropertyDefinition]), delegate(object value, IPropertyBag propertyBag)
			{
				propertyBag[rawPropertyDefinition] = value;
			});
		}
	}
}
