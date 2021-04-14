using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class ConfigurableObjectLogSchema<T, Tschema> : ObjectLogSchema where T : ConfigurableObject where Tschema : ObjectSchema, new()
	{
		private static ConfigurableObjectLogSchema<T, Tschema>.SchemaEntryList ComputeSchemaEntries()
		{
			ConfigurableObjectLogSchema<T, Tschema>.SchemaEntryList schemaEntryList = new ConfigurableObjectLogSchema<T, Tschema>.SchemaEntryList();
			ObjectSchema objectSchema = ObjectSchema.GetInstance<Tschema>();
			foreach (PropertyDefinition propertyDefinition in objectSchema.AllProperties)
			{
				SimpleProviderPropertyDefinition simpleProviderPropertyDefinition = (SimpleProviderPropertyDefinition)propertyDefinition;
				if (simpleProviderPropertyDefinition.IsMultivalued)
				{
					schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.MultiValuedProperty(), simpleProviderPropertyDefinition);
				}
				else
				{
					Type type = simpleProviderPropertyDefinition.Type;
					if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
					{
						type = type.GetGenericArguments()[0];
					}
					if (type.IsEnum && type.GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0)
					{
						schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.SimpleProperty<int>(), simpleProviderPropertyDefinition);
					}
					else if (type == typeof(ADObjectId))
					{
						schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.ADObjectIdGuidProperty(), simpleProviderPropertyDefinition);
						schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.StringProperty(), simpleProviderPropertyDefinition);
					}
					else if (type == typeof(int))
					{
						schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.SimpleProperty<int>(), simpleProviderPropertyDefinition);
					}
					else if (type == typeof(Unlimited<int>))
					{
						schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.UnlimitedProperty<int, ConfigurableObjectLogSchema<T, Tschema>.SimpleProperty<int>>(), simpleProviderPropertyDefinition);
					}
					else if (type == typeof(byte[]))
					{
						schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.SimpleProperty<byte[]>(), simpleProviderPropertyDefinition);
					}
					else if (type == typeof(Guid))
					{
						schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.GuidProperty(), simpleProviderPropertyDefinition);
					}
					else if (type == typeof(DateTime))
					{
						schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.DateTimeProperty(), simpleProviderPropertyDefinition);
					}
					else if (type == typeof(ExDateTime))
					{
						schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.ExDateTimeProperty(), simpleProviderPropertyDefinition);
					}
					else if (type == typeof(TimeSpan))
					{
						schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.TimeSpanProperty(), simpleProviderPropertyDefinition);
					}
					else if (type == typeof(Unlimited<TimeSpan>))
					{
						schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.UnlimitedProperty<TimeSpan, ConfigurableObjectLogSchema<T, Tschema>.TimeSpanProperty>(), simpleProviderPropertyDefinition);
					}
					else if (type == typeof(EnhancedTimeSpan))
					{
						schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.EnhancedTimeSpanProperty(), simpleProviderPropertyDefinition);
					}
					else if (type == typeof(Unlimited<EnhancedTimeSpan>))
					{
						schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.UnlimitedProperty<EnhancedTimeSpan, ConfigurableObjectLogSchema<T, Tschema>.EnhancedTimeSpanProperty>(), simpleProviderPropertyDefinition);
					}
					else
					{
						schemaEntryList.Add(new ConfigurableObjectLogSchema<T, Tschema>.StringProperty(), simpleProviderPropertyDefinition);
					}
				}
			}
			return schemaEntryList;
		}

		public static readonly IEnumerable<IObjectLogPropertyDefinition<T>> SchemaEntries = ConfigurableObjectLogSchema<T, Tschema>.ComputeSchemaEntries();

		private class SchemaEntryList : List<IObjectLogPropertyDefinition<T>>
		{
			public void Add(ConfigurableObjectLogSchema<T, Tschema>.PropertyBase schemaEntry, SimpleProviderPropertyDefinition property)
			{
				schemaEntry.Property = property;
				base.Add(schemaEntry);
			}
		}

		private abstract class PropertyBase : IObjectLogPropertyDefinition<T>
		{
			public SimpleProviderPropertyDefinition Property { get; set; }

			public abstract object GetValueInternal(object val);

			public virtual string GetFieldName()
			{
				return this.Property.Name;
			}

			string IObjectLogPropertyDefinition<!0>.FieldName
			{
				get
				{
					return this.GetFieldName();
				}
			}

			object IObjectLogPropertyDefinition<!0>.GetValue(T objectToLog)
			{
				object obj = objectToLog[this.Property];
				if (obj == null)
				{
					return null;
				}
				if (obj.Equals(this.Property.DefaultValue))
				{
					return null;
				}
				return this.GetValueInternal(obj);
			}
		}

		private class SimpleProperty<Tprop> : ConfigurableObjectLogSchema<T, Tschema>.PropertyBase
		{
			public override object GetValueInternal(object val)
			{
				return (Tprop)((object)val);
			}
		}

		private class StringProperty : ConfigurableObjectLogSchema<T, Tschema>.PropertyBase
		{
			public override object GetValueInternal(object val)
			{
				return val.ToString();
			}
		}

		private class GuidProperty : ConfigurableObjectLogSchema<T, Tschema>.PropertyBase
		{
			public override object GetValueInternal(object val)
			{
				Guid a = (Guid)val;
				if (a == Guid.Empty)
				{
					return null;
				}
				return val;
			}
		}

		private class DateTimeProperty : ConfigurableObjectLogSchema<T, Tschema>.PropertyBase
		{
			public override object GetValueInternal(object val)
			{
				return ((DateTime)val).ToUniversalTime();
			}
		}

		private class ExDateTimeProperty : ConfigurableObjectLogSchema<T, Tschema>.PropertyBase
		{
			public override object GetValueInternal(object val)
			{
				return ((ExDateTime)val).ToUtc();
			}
		}

		private class TimeSpanProperty : ConfigurableObjectLogSchema<T, Tschema>.PropertyBase
		{
			public override object GetValueInternal(object val)
			{
				return ((TimeSpan)val).Ticks;
			}
		}

		private class EnhancedTimeSpanProperty : ConfigurableObjectLogSchema<T, Tschema>.PropertyBase
		{
			public override object GetValueInternal(object val)
			{
				return ((EnhancedTimeSpan)val).Ticks;
			}
		}

		private class UnlimitedProperty<Tprop, TUnderlyingPropertyBase> : ConfigurableObjectLogSchema<T, Tschema>.PropertyBase where Tprop : struct, IComparable where TUnderlyingPropertyBase : ConfigurableObjectLogSchema<T, Tschema>.PropertyBase, new()
		{
			public override object GetValueInternal(object val)
			{
				Unlimited<Tprop> unlimited = (Unlimited<Tprop>)val;
				if (unlimited.IsUnlimited)
				{
					return null;
				}
				return ConfigurableObjectLogSchema<T, Tschema>.UnlimitedProperty<Tprop, TUnderlyingPropertyBase>.underlyingProp.GetValueInternal(unlimited.Value);
			}

			private static ConfigurableObjectLogSchema<T, Tschema>.PropertyBase underlyingProp = Activator.CreateInstance<TUnderlyingPropertyBase>();
		}

		private class MultiValuedProperty : ConfigurableObjectLogSchema<T, Tschema>.PropertyBase
		{
			public override object GetValueInternal(object val)
			{
				MultiValuedPropertyBase multiValuedPropertyBase = val as MultiValuedPropertyBase;
				if (multiValuedPropertyBase == null)
				{
					return null;
				}
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				foreach (object arg in ((IEnumerable)multiValuedPropertyBase))
				{
					if (flag)
					{
						stringBuilder.AppendFormat("{0}", arg);
						flag = false;
					}
					else
					{
						stringBuilder.AppendFormat(";{0}", arg);
					}
				}
				return stringBuilder.ToString();
			}
		}

		private class ADObjectIdGuidProperty : ConfigurableObjectLogSchema<T, Tschema>.PropertyBase
		{
			public override string GetFieldName()
			{
				return string.Format("{0}_Guid", base.Property.Name);
			}

			public override object GetValueInternal(object val)
			{
				ADObjectId adobjectId = val as ADObjectId;
				if (adobjectId != null && adobjectId.ObjectGuid != Guid.Empty)
				{
					return adobjectId.ObjectGuid;
				}
				return null;
			}
		}
	}
}
