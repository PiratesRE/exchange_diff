using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.DataModel.Serialization;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel
{
	public abstract class PropertyChangeTrackingObject : IPropertyChangeTracker<PropertyDefinition>
	{
		public static IDataContractSurrogate DataContractSurrogate
		{
			get
			{
				return PropertyChangeTrackingObject.DataContractSurrogateInstance;
			}
		}

		public static IDataContractSurrogate LoggingSurrogate
		{
			get
			{
				return PropertyChangeTrackingObject.LoggingSurrogateInstance;
			}
		}

		public bool IsPropertySet(PropertyDefinition property)
		{
			return this.propertyBag.IsPropertySet(property);
		}

		protected TPropertyValue GetPropertyValueOrDefault<TPropertyValue>(TypedPropertyDefinition<TPropertyValue> typedProperty)
		{
			return this.propertyBag.GetValueOrDefault<TPropertyValue>(typedProperty);
		}

		protected void SetPropertyValue<TPropertyValue>(TypedPropertyDefinition<TPropertyValue> typedProperty, TPropertyValue value)
		{
			this.propertyBag.SetValue<TPropertyValue>(typedProperty, value);
			if (typedProperty.IsLoggable)
			{
				this.loggableProperties[typedProperty.Name] = value;
			}
		}

		[OnDeserializing]
		private void SetValuesOnDeserialzing(StreamingContext context)
		{
			this.propertyBag = PropertyBag.CreateInstance();
		}

		private static readonly PropertyChangeTrackingObject.PropertyChangeTrackingSurrogate DataContractSurrogateInstance = new PropertyChangeTrackingObject.PropertyChangeTrackingSurrogate();

		private static readonly PropertyChangeTrackingObject.PropertyChangeTrackingLoggingSurrogate LoggingSurrogateInstance = new PropertyChangeTrackingObject.PropertyChangeTrackingLoggingSurrogate();

		private PropertyBag propertyBag = PropertyBag.CreateInstance();

		private Dictionary<string, object> loggableProperties = new Dictionary<string, object>();

		[DataContract]
		private class EntitySerializationData
		{
			[DataMember]
			public PropertyBag ChangedProperties { get; set; }

			[DataMember]
			public string OriginalTypeAssembly { get; set; }

			[DataMember]
			public string OriginalTypeName { get; set; }
		}

		[DataContract]
		private class EntityLoggingData
		{
			public Dictionary<string, object> Properties
			{
				get
				{
					return this.properties;
				}
				set
				{
					this.properties = value;
				}
			}

			[DataMember]
			private Dictionary<string, object> properties;
		}

		private abstract class BasePropertyChangeTrackingSurrogate<T> : IDataContractSurrogate
		{
			public Type GetDataContractType(Type type)
			{
				if (typeof(PropertyChangeTrackingObject).IsAssignableFrom(type))
				{
					return typeof(T);
				}
				if (typeof(ExDateTime) == type)
				{
					return typeof(PropertyChangeTrackingObject.SerializableExDateTime);
				}
				return type;
			}

			public abstract object GetDeserializedObject(object obj, Type targetType);

			public object GetObjectToSerialize(object obj, Type targetType)
			{
				PropertyChangeTrackingObject propertyChangeTrackingObject = obj as PropertyChangeTrackingObject;
				if (propertyChangeTrackingObject != null)
				{
					return this.GetDataToSerialize(propertyChangeTrackingObject);
				}
				if (obj is ExDateTime)
				{
					return new PropertyChangeTrackingObject.SerializableExDateTime((ExDateTime)obj);
				}
				return obj;
			}

			public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
			{
				return null;
			}

			public object GetCustomDataToExport(Type clrType, Type dataContractType)
			{
				return null;
			}

			public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
			{
			}

			public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
			{
				return null;
			}

			public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
			{
				return null;
			}

			protected abstract object GetDataToSerialize(PropertyChangeTrackingObject outerObject);
		}

		private class PropertyChangeTrackingSurrogate : PropertyChangeTrackingObject.BasePropertyChangeTrackingSurrogate<PropertyChangeTrackingObject.EntitySerializationData>
		{
			public override object GetDeserializedObject(object obj, Type targetType)
			{
				PropertyChangeTrackingObject.EntitySerializationData entitySerializationData = obj as PropertyChangeTrackingObject.EntitySerializationData;
				if (entitySerializationData != null)
				{
					string originalTypeName = entitySerializationData.OriginalTypeName;
					Type type = Type.GetType(originalTypeName, false);
					if (type == null)
					{
						string originalTypeAssembly = entitySerializationData.OriginalTypeAssembly;
						Assembly assembly = Assembly.Load(originalTypeAssembly);
						type = assembly.GetType(originalTypeName);
					}
					if (type == null)
					{
						throw new CorruptDataException(Strings.ErrorUnknownType(originalTypeName));
					}
					Type typeFromHandle = typeof(PropertyChangeTrackingObject);
					if (!typeFromHandle.IsAssignableFrom(type))
					{
						throw new CorruptDataException(Strings.ErrorNonEntityType(originalTypeName, typeFromHandle.Name));
					}
					try
					{
						PropertyChangeTrackingObject propertyChangeTrackingObject = (PropertyChangeTrackingObject)Activator.CreateInstance(type);
						propertyChangeTrackingObject.propertyBag = ((PropertyChangeTrackingObject.EntitySerializationData)obj).ChangedProperties;
						return propertyChangeTrackingObject;
					}
					catch (MissingMethodException)
					{
						throw new CorruptDataException(Strings.ErrorNoDefaultConstructor(originalTypeName));
					}
				}
				PropertyChangeTrackingObject.SerializableExDateTime serializableExDateTime = obj as PropertyChangeTrackingObject.SerializableExDateTime;
				if (serializableExDateTime != null)
				{
					return serializableExDateTime.ToExDateTime();
				}
				return obj;
			}

			protected override object GetDataToSerialize(PropertyChangeTrackingObject outerObject)
			{
				return new PropertyChangeTrackingObject.EntitySerializationData
				{
					ChangedProperties = outerObject.propertyBag,
					OriginalTypeName = outerObject.GetType().FullName,
					OriginalTypeAssembly = outerObject.GetType().Assembly.FullName
				};
			}
		}

		[DataContract]
		private class SerializableExDateTime
		{
			public SerializableExDateTime(ExDateTime value)
			{
				this.utcDateTime = value.UniversalTime;
				this.timeZoneId = value.TimeZone.Id;
			}

			public ExDateTime ToExDateTime()
			{
				ExTimeZone utcTimeZone;
				if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(this.timeZoneId, out utcTimeZone))
				{
					utcTimeZone = ExTimeZone.UtcTimeZone;
				}
				return new ExDateTime(utcTimeZone, this.utcDateTime);
			}

			[DataMember]
			private readonly DateTime utcDateTime;

			[DataMember]
			private readonly string timeZoneId;
		}

		private class PropertyChangeTrackingLoggingSurrogate : PropertyChangeTrackingObject.BasePropertyChangeTrackingSurrogate<PropertyChangeTrackingObject.EntityLoggingData>
		{
			public override object GetDeserializedObject(object obj, Type targetType)
			{
				return new NotImplementedException();
			}

			protected override object GetDataToSerialize(PropertyChangeTrackingObject outerObject)
			{
				PropertyChangeTrackingObject.EntityLoggingData result = null;
				if (outerObject.loggableProperties.Count > 0)
				{
					result = new PropertyChangeTrackingObject.EntityLoggingData
					{
						Properties = outerObject.loggableProperties
					};
				}
				return result;
			}
		}
	}
}
