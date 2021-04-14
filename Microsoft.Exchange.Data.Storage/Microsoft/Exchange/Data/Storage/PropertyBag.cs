using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class PropertyBag : IValidatablePropertyBag, IDirectPropertyBag, ILocationIdentifierSetter
	{
		private ExchangeOperationContext operationContext
		{
			get
			{
				if (this.Context == null || this.Context.Session == null)
				{
					return null;
				}
				return this.Context.Session.OperationContext;
			}
		}

		internal PropertyBag()
		{
			this.storageItf = new PropertyBag.BasicPropertyStore(this);
		}

		internal PropertyBag(PropertyBag propertyBag) : this()
		{
			if (propertyBag.context != null)
			{
				this.context = new PropertyBagContext(propertyBag.context);
			}
		}

		public static explicit operator PropertyBag.BasicPropertyStore(PropertyBag propertyBag)
		{
			return propertyBag.storageItf;
		}

		public static T CheckPropertyValue<T>(StorePropertyDefinition propertyDefinition, object propertyValue)
		{
			PropertyBag.EnsurePropertyIsNotStreamable(propertyDefinition);
			PropertyBag.ThrowIfPropertyError(propertyDefinition, propertyValue);
			return (T)((object)propertyValue);
		}

		public static T CheckPropertyValue<T>(StorePropertyDefinition propertyDefinition, object propertyValue, T defaultPropertyValue)
		{
			PropertyBag.EnsurePropertyIsNotStreamable(propertyDefinition);
			if (propertyValue == null)
			{
				return defaultPropertyValue;
			}
			PropertyError propertyError = propertyValue as PropertyError;
			if (propertyError == null)
			{
				return (T)((object)propertyValue);
			}
			if (propertyError.PropertyErrorCode == PropertyErrorCode.NotFound)
			{
				return defaultPropertyValue;
			}
			throw PropertyError.ToException(new PropertyError[]
			{
				propertyError
			});
		}

		internal static T? CheckNullablePropertyValue<T>(StorePropertyDefinition propertyDefinition, object propertyValue) where T : struct
		{
			PropertyBag.EnsurePropertyIsNotStreamable(propertyDefinition);
			if (propertyValue == null)
			{
				return null;
			}
			PropertyError propertyError = propertyValue as PropertyError;
			if (propertyError == null)
			{
				return new T?((T)((object)propertyValue));
			}
			if (propertyError.PropertyErrorCode == PropertyErrorCode.NotFound)
			{
				return null;
			}
			throw PropertyError.ToException(new PropertyError[]
			{
				propertyError
			});
		}

		private static void ThrowIfPropertyError(StorePropertyDefinition propertyDefinition, object propertyValue)
		{
			if (propertyValue == null)
			{
				throw PropertyError.ToException(new PropertyError[]
				{
					new PropertyError(propertyDefinition, PropertyErrorCode.NullValue)
				});
			}
			if (PropertyError.IsPropertyError(propertyValue))
			{
				throw PropertyError.ToException(new PropertyError[]
				{
					(PropertyError)propertyValue
				});
			}
		}

		private static void EnsurePropertyIsNotStreamable(StorePropertyDefinition propertyDefinition)
		{
		}

		private object FixType(StorePropertyDefinition propertyDefinition, object value)
		{
			if (value == null)
			{
				return value;
			}
			Type type = propertyDefinition.Type;
			Type type2 = value.GetType();
			if (type == type2)
			{
				return value;
			}
			if (type.GetTypeInfo().IsPrimitive && type2.GetTypeInfo().IsEnum)
			{
				if (type == Enum.GetUnderlyingType(type2))
				{
					return Convert.ChangeType(value, type);
				}
			}
			else if (type.GetTypeInfo().IsEnum && type2.GetTypeInfo().IsPrimitive && Enum.GetUnderlyingType(type) == type2)
			{
				return Enum.ToObject(type, value);
			}
			if (this.operationContext != null && this.operationContext.IsMoveUser && type == typeof(string[]) && (propertyDefinition == InternalSchema.Categories || propertyDefinition == InternalSchema.Contacts))
			{
				string text = value as string;
				if (text != null)
				{
					return new string[]
					{
						text
					};
				}
				string[] array = value as string[];
				if (array != null)
				{
					if (Array.Exists<string>(array, (string valueItem) => valueItem == null))
					{
						List<string> list = new List<string>(array.Length);
						foreach (string text2 in array)
						{
							if (text2 != null)
							{
								list.Add(text2);
							}
						}
						value = list.ToArray();
					}
				}
			}
			return value;
		}

		public object[] GetProperties<T>(ICollection<T> propertyDefinitionArray) where T : PropertyDefinition
		{
			if (propertyDefinitionArray == null)
			{
				throw new ArgumentNullException("propertyDefinitionArray");
			}
			object[] array = new object[propertyDefinitionArray.Count];
			int num = 0;
			foreach (T t in propertyDefinitionArray)
			{
				array[num++] = this.TryGetProperty(t);
			}
			return array;
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return this.GetProperty(propertyDefinition);
			}
			set
			{
				this.SetProperty(propertyDefinition, value);
			}
		}

		public object this[StorePropertyDefinition propertyDefinition]
		{
			get
			{
				return this.GetProperty(propertyDefinition);
			}
			set
			{
				this.SetProperty(propertyDefinition, value);
			}
		}

		public void SetProperty(PropertyDefinition propertyDefinition, object value)
		{
			StorePropertyDefinition propertyDefinition2 = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			this.SetProperty(propertyDefinition2, value);
		}

		private void SetProperty(StorePropertyDefinition propertyDefinition, object value)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			if (this.Context.IsValidationDisabled)
			{
				propertyDefinition.SetWithoutValidation(this.storageItf, this.FixType(propertyDefinition, value));
				return;
			}
			propertyDefinition.Set(this.operationContext, this.storageItf, this.FixType(propertyDefinition, value));
		}

		protected object GetProperty(PropertyDefinition propertyDefinition)
		{
			StorePropertyDefinition propertyDefinition2 = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			return this.GetProperty(propertyDefinition2);
		}

		protected object GetProperty(StorePropertyDefinition propertyDefinition)
		{
			object obj = this.TryGetProperty(propertyDefinition);
			PropertyBag.ThrowIfPropertyError(propertyDefinition, obj);
			return obj;
		}

		public object TryGetProperty(PropertyDefinition propertyDefinition)
		{
			StorePropertyDefinition propertyDefinition2 = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			return this.TryGetProperty(propertyDefinition2);
		}

		internal object TryGetProperty(StorePropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			return propertyDefinition.Get(this.storageItf);
		}

		public void Delete(PropertyDefinition propertyDefinition)
		{
			StorePropertyDefinition propertyDefinition2 = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			this.Delete(propertyDefinition2);
		}

		public void Delete(StorePropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			propertyDefinition.Delete(this.storageItf);
		}

		public void Load()
		{
			this.Load(null);
		}

		public abstract void Load(ICollection<PropertyDefinition> propsToLoad);

		public virtual PropertyValueTrackingData GetOriginalPropertyInformation(PropertyDefinition propertyDefinition)
		{
			return PropertyValueTrackingData.PropertyValueTrackDataNotTracked;
		}

		public abstract bool IsDirty { get; }

		public bool IsPropertyDirty(PropertyDefinition propertyDefinition)
		{
			StorePropertyDefinition propertyDefinition2 = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			return this.IsPropertyDirty(propertyDefinition2);
		}

		internal bool IsPropertyDirty(StorePropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			return propertyDefinition.IsDirty(this.storageItf);
		}

		public T GetValueOrDefault<T>(PropertyDefinition propertyDefinition)
		{
			StorePropertyDefinition propertyDefinition2 = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			return this.GetValueOrDefault<T>(propertyDefinition2);
		}

		public T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition)
		{
			return this.GetValueOrDefault<T>(propertyDefinition, default(T));
		}

		public T GetValueOrDefault<T>(PropertyDefinition propertyDefinition, T defaultValue)
		{
			StorePropertyDefinition propertyDefinition2 = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			return this.GetValueOrDefault<T>(propertyDefinition2, defaultValue);
		}

		public T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition, T defaultValue)
		{
			return PropertyBag.CheckPropertyValue<T>(propertyDefinition, this.TryGetProperty(propertyDefinition), defaultValue);
		}

		public T? GetValueAsNullable<T>(PropertyDefinition propertyDefinition) where T : struct
		{
			StorePropertyDefinition propertyDefinition2 = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			return this.GetValueAsNullable<T>(propertyDefinition2);
		}

		internal T? GetValueAsNullable<T>(StorePropertyDefinition propertyDefinition) where T : struct
		{
			return PropertyBag.CheckNullablePropertyValue<T>(propertyDefinition, this.TryGetProperty(propertyDefinition));
		}

		PropertyBagContext IDirectPropertyBag.Context
		{
			get
			{
				return this.Context;
			}
		}

		public bool IsNew
		{
			get
			{
				return this.Context.CoreState.Origin == Origin.New;
			}
		}

		bool IDirectPropertyBag.IsLoaded(NativeStorePropertyDefinition propertyDefinition)
		{
			return this.IsLoaded(propertyDefinition);
		}

		bool IDirectPropertyBag.IsDirty(AtomicStorePropertyDefinition propertyDefinition)
		{
			return this.InternalIsPropertyDirty(propertyDefinition);
		}

		void IDirectPropertyBag.SetValue(StorePropertyDefinition propertyDefinition, object propertyValue)
		{
			this.SetValidatedStoreProperty(propertyDefinition, propertyValue);
		}

		object IDirectPropertyBag.GetValue(StorePropertyDefinition propertyDefinition)
		{
			return this.TryGetStoreProperty(propertyDefinition);
		}

		void IDirectPropertyBag.Delete(StorePropertyDefinition propertyDefinition)
		{
			this.DeleteStoreProperty(propertyDefinition);
		}

		protected abstract void SetValidatedStoreProperty(StorePropertyDefinition propertyDefinition, object propertyValue);

		protected abstract object TryGetStoreProperty(StorePropertyDefinition propertyDefinition);

		protected abstract void DeleteStoreProperty(StorePropertyDefinition propertyDefinition);

		protected abstract bool InternalIsPropertyDirty(AtomicStorePropertyDefinition propertyDefinition);

		protected abstract bool IsLoaded(NativeStorePropertyDefinition propertyDefinition);

		internal abstract ExTimeZone ExTimeZone { get; set; }

		internal IStorePropertyBag AsIStorePropertyBag()
		{
			return new PropertyBag.StorePropertyBagAdaptor(this);
		}

		internal virtual PropertyBagContext Context
		{
			get
			{
				if (this.context == null)
				{
					this.context = new PropertyBagContext();
				}
				return this.context;
			}
		}

		public void SetOrDeleteProperty(PropertyDefinition propertyDefinition, object propertyValue)
		{
			StorePropertyDefinition propertyDefinition2 = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			this.SetOrDeleteProperty(propertyDefinition2, propertyValue);
		}

		public void SetOrDeleteProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
		{
			if (propertyValue == null || PropertyError.IsPropertyNotFound(propertyValue))
			{
				this.Delete(propertyDefinition);
				return;
			}
			this.SetProperty(propertyDefinition, propertyValue);
		}

		public virtual bool CanIgnoreUnchangedProperties
		{
			get
			{
				return true;
			}
		}

		void ILocationIdentifierSetter.SetLocationIdentifier(uint id)
		{
			if (this.OnLocationIdentifierReached != null)
			{
				this.OnLocationIdentifierReached(id);
			}
		}

		void ILocationIdentifierSetter.SetLocationIdentifier(uint id, LastChangeAction action)
		{
			if (this.OnNamedLocationIdentifierReached != null)
			{
				this.OnNamedLocationIdentifierReached(id, action);
			}
		}

		private readonly PropertyBag.BasicPropertyStore storageItf;

		private PropertyBagContext context;

		internal Action<uint> OnLocationIdentifierReached;

		internal Action<uint, LastChangeAction> OnNamedLocationIdentifierReached;

		internal struct BasicPropertyStore : ILocationIdentifierSetter
		{
			internal BasicPropertyStore(PropertyBag parent)
			{
				this.parent = parent;
			}

			public static explicit operator PropertyBag(PropertyBag.BasicPropertyStore propertyStore)
			{
				return propertyStore.parent;
			}

			public PropertyBagContext Context
			{
				get
				{
					return this.parent.Context;
				}
			}

			public ExTimeZone TimeZone
			{
				get
				{
					return this.parent.ExTimeZone;
				}
			}

			public bool CanIgnoreUnchangedProperties
			{
				get
				{
					return this.parent.CanIgnoreUnchangedProperties;
				}
			}

			public void SetValue(AtomicStorePropertyDefinition propertyDefinition, object propertyValue)
			{
				this.parent.SetValidatedStoreProperty(propertyDefinition, propertyValue);
			}

			public void SetOrDeleteProperty(AtomicStorePropertyDefinition propertyDefinition, object propertyValue)
			{
				this.parent.SetOrDeleteProperty(propertyDefinition, propertyValue);
			}

			public void SetValueWithFixup(AtomicStorePropertyDefinition propertyDefinition, object propertyValue)
			{
				this.parent.SetProperty(propertyDefinition, propertyValue);
			}

			public void Update(AtomicStorePropertyDefinition propertyDefinition, object propertyValue)
			{
				if (propertyValue == null || PropertyError.IsPropertyNotFound(propertyValue))
				{
					this.parent.DeleteStoreProperty(propertyDefinition);
					return;
				}
				this.parent.SetProperty(propertyDefinition, propertyValue);
			}

			public object GetValue(AtomicStorePropertyDefinition propertyDefinition)
			{
				return this.parent.TryGetStoreProperty(propertyDefinition);
			}

			public object GetValue(SmartPropertyDefinition propertyDefinition)
			{
				return this.parent.TryGetProperty(propertyDefinition);
			}

			public T GetValueOrDefault<T>(AtomicStorePropertyDefinition propertyDefinition)
			{
				return this.GetValueOrDefault<T>(propertyDefinition, default(T));
			}

			public T GetValueOrDefault<T>(AtomicStorePropertyDefinition propertyDefinition, T defaultValue)
			{
				return this.parent.GetValueOrDefault<T>(propertyDefinition, defaultValue);
			}

			public T? GetValueAsNullable<T>(AtomicStorePropertyDefinition propertyDefinition) where T : struct
			{
				return this.parent.GetValueAsNullable<T>(propertyDefinition);
			}

			public void Delete(AtomicStorePropertyDefinition propertyDefinition)
			{
				this.parent.DeleteStoreProperty(propertyDefinition);
			}

			public bool IsLoaded(NativeStorePropertyDefinition propertyDefinition)
			{
				return this.parent.IsLoaded(propertyDefinition);
			}

			public bool IsDirty(AtomicStorePropertyDefinition propertyDefinition)
			{
				return this.parent.InternalIsPropertyDirty(propertyDefinition);
			}

			public void SetLocationIdentifier(uint id)
			{
				((ILocationIdentifierSetter)this.parent).SetLocationIdentifier(id);
			}

			public void SetLocationIdentifier(uint id, LastChangeAction action)
			{
				((ILocationIdentifierSetter)this.parent).SetLocationIdentifier(id, action);
			}

			private readonly PropertyBag parent;
		}

		protected sealed class StorePropertyBagAdaptor : IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag
		{
			internal StorePropertyBagAdaptor(PropertyBag propertyBag)
			{
				this.propertyBag = propertyBag;
			}

			internal PropertyBag PropertyBag
			{
				get
				{
					return this.propertyBag;
				}
			}

			public bool IsDirty
			{
				get
				{
					return this.propertyBag.IsDirty;
				}
			}

			public bool IsPropertyDirty(PropertyDefinition propertyDefinition)
			{
				return this.propertyBag.IsPropertyDirty(propertyDefinition);
			}

			public void Load()
			{
				this.Load(null);
			}

			public void Load(ICollection<PropertyDefinition> propertyDefinitions)
			{
				this.propertyBag.Load(propertyDefinitions);
			}

			public Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
			{
				throw new NotSupportedException(ServerStrings.ExPropertyNotStreamable(propertyDefinition.ToString()));
			}

			public object TryGetProperty(PropertyDefinition propertyDefinition)
			{
				return this.propertyBag.TryGetProperty(propertyDefinition);
			}

			public void Delete(PropertyDefinition propertyDefinition)
			{
				this.propertyBag.Delete(propertyDefinition);
			}

			public T GetValueOrDefault<T>(PropertyDefinition propertyDefinition, T defaultValue)
			{
				return this.propertyBag.GetValueOrDefault<T>(propertyDefinition, defaultValue);
			}

			public void SetOrDeleteProperty(PropertyDefinition propertyDefinition, object propertyValue)
			{
				this.propertyBag.SetOrDeleteProperty(propertyDefinition, propertyValue);
			}

			public object this[PropertyDefinition propertyDefinition]
			{
				get
				{
					return this.propertyBag[propertyDefinition];
				}
				set
				{
					this.propertyBag[propertyDefinition] = value;
				}
			}

			public void SetProperties(ICollection<PropertyDefinition> propertyDefinitionArray, object[] propertyValuesArray)
			{
				int num = 0;
				foreach (PropertyDefinition propertyDefinition in propertyDefinitionArray)
				{
					this.propertyBag[propertyDefinition] = propertyValuesArray[num++];
				}
			}

			public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
			{
				return this.propertyBag.GetProperties<PropertyDefinition>(propertyDefinitionArray);
			}

			private readonly PropertyBag propertyBag;
		}
	}
}
