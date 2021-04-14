using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public abstract class ConfigurableObject : IPropertyBag, IReadOnlyPropertyBag, IConfigurable, ICloneable, IVersionable, ICmdletProxyable
	{
		internal event ConvertOutputPropertyDelegate OutputPropertyConverter
		{
			add
			{
				this.outputPropertyConverter = (ConvertOutputPropertyDelegate)Delegate.Combine(this.outputPropertyConverter, value);
			}
			remove
			{
				this.outputPropertyConverter = (ConvertOutputPropertyDelegate)Delegate.Remove(this.outputPropertyConverter, value);
			}
		}

		internal ConfigurableObject(PropertyBag propertyBag)
		{
			if (propertyBag == null)
			{
				throw new ArgumentNullException("propertyBag");
			}
			this.propertyBag = propertyBag;
			if (propertyBag.Count == 0)
			{
				this.ObjectState = (ObjectState)propertyBag.ObjectStatePropertyDefinition.DefaultValue;
				this.propertyBag.ResetChangeTracking(propertyBag.ObjectStatePropertyDefinition);
			}
			if (this.ObjectState == ObjectState.New && this.propertyBag.ObjectVersionPropertyDefinition != null && !this.propertyBag.Contains(this.propertyBag.ObjectVersionPropertyDefinition))
			{
				this.propertyBag.SetField(this.propertyBag.ObjectVersionPropertyDefinition, this.MaximumSupportedExchangeObjectVersion);
				this.propertyBag.ResetChangeTracking(this.propertyBag.ObjectVersionPropertyDefinition);
			}
		}

		public object GetProxyInfo()
		{
			return this.proxyInfo;
		}

		public void SetProxyInfo(object proxyInfoValue)
		{
			if (this.proxyInfo != null && proxyInfoValue != null)
			{
				return;
			}
			this.proxyInfo = proxyInfoValue;
		}

		internal virtual ExchangeObjectVersion ExchangeVersion
		{
			get
			{
				if (this.propertyBag.ObjectVersionPropertyDefinition == null)
				{
					throw new NotSupportedException(DataStrings.ExceptionVersionlessObject);
				}
				return (ExchangeObjectVersion)this[this.propertyBag.ObjectVersionPropertyDefinition];
			}
		}

		ExchangeObjectVersion IVersionable.ExchangeVersion
		{
			get
			{
				return this.ExchangeVersion;
			}
		}

		internal void SetExchangeVersion(ExchangeObjectVersion newVersion)
		{
			this.CheckWritable();
			if (this.propertyBag.ObjectVersionPropertyDefinition == null)
			{
				throw new NotSupportedException(DataStrings.ExceptionVersionlessObject);
			}
			ExchangeObjectVersion exchangeVersion = this.ExchangeVersion;
			if (null == newVersion)
			{
				newVersion = (ExchangeObjectVersion)this.propertyBag.ObjectVersionPropertyDefinition.DefaultValue;
			}
			if (newVersion.IsOlderThan(exchangeVersion))
			{
				List<ProviderPropertyDefinition> list = new List<ProviderPropertyDefinition>(this.propertyBag.Keys.Cast<ProviderPropertyDefinition>());
				foreach (ProviderPropertyDefinition providerPropertyDefinition in list)
				{
					if (providerPropertyDefinition.VersionAdded.IsNewerThan(newVersion) && !providerPropertyDefinition.VersionAdded.IsNewerThan(exchangeVersion))
					{
						if (this.propertyBag.IsReadOnlyProperty(providerPropertyDefinition) || providerPropertyDefinition.IsCalculated)
						{
							this.propertyBag.SetField(providerPropertyDefinition, null);
						}
						else
						{
							this[providerPropertyDefinition] = null;
						}
					}
				}
			}
			this.propertyBag.SetObjectVersion(newVersion);
		}

		internal abstract ObjectSchema ObjectSchema { get; }

		ObjectSchema IVersionable.ObjectSchema
		{
			get
			{
				return this.ObjectSchema;
			}
		}

		internal virtual ObjectSchema DeserializationSchema
		{
			get
			{
				return this.ObjectSchema;
			}
		}

		internal virtual ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2007;
			}
		}

		ExchangeObjectVersion IVersionable.MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return this.MaximumSupportedExchangeObjectVersion;
			}
		}

		internal bool IsReadOnly
		{
			get
			{
				return this.propertyBag.IsReadOnly;
			}
		}

		bool IVersionable.IsReadOnly
		{
			get
			{
				return this.IsReadOnly;
			}
		}

		internal virtual void SetIsReadOnly(bool valueToSet)
		{
			this.propertyBag.SetIsReadOnly(valueToSet);
		}

		internal virtual bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return false;
			}
		}

		bool IVersionable.ExchangeVersionUpgradeSupported
		{
			get
			{
				return this.ExchangeVersionUpgradeSupported;
			}
		}

		internal virtual bool IsPropertyAccessible(PropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			ProviderPropertyDefinition providerPropertyDefinition = propertyDefinition as ProviderPropertyDefinition;
			return providerPropertyDefinition == null || !providerPropertyDefinition.VersionAdded.ExchangeBuild.IsNewerThan(this.ExchangeVersion.ExchangeBuild) || this.ExchangeVersionUpgradeSupported;
		}

		bool IVersionable.IsPropertyAccessible(PropertyDefinition propertyDefinition)
		{
			return this.IsPropertyAccessible(propertyDefinition);
		}

		public virtual object Clone()
		{
			ConfigurableObject configurableObject = (ConfigurableObject)CloneHelper.SerializeObj(this);
			if (this.DeserializationSchema == null)
			{
				List<PropertyDefinition> list = new List<PropertyDefinition>(this.propertyBag.Keys.Count);
				foreach (object obj in this.propertyBag.Keys)
				{
					PropertyDefinition item = (PropertyDefinition)obj;
					list.Add(item);
				}
				configurableObject.propertyBag.SetConfigObjectSchema(list);
			}
			return configurableObject;
		}

		public virtual ObjectId Identity
		{
			get
			{
				return (ObjectId)this[this.propertyBag.ObjectIdentityPropertyDefinition];
			}
		}

		public ValidationError[] Validate()
		{
			List<ValidationError> list = new List<ValidationError>();
			this.ValidateRead(list);
			this.ValidateWrite(list);
			return list.ToArray();
		}

		internal ValidationError[] ValidateRead()
		{
			List<ValidationError> list = new List<ValidationError>();
			this.ValidateRead(list);
			return list.ToArray();
		}

		internal ValidationError[] ValidateWrite()
		{
			List<ValidationError> list = new List<ValidationError>();
			this.ValidateWrite(list);
			return list.ToArray();
		}

		protected bool TryConvertOutputProperty(object value, PropertyDefinition property, out object convertedValue)
		{
			convertedValue = null;
			return this.outputPropertyConverter != null && ObjectOutputHelper.TryConvertOutputProperty(value, this, property, null, this.outputPropertyConverter.GetInvocationList(), out convertedValue);
		}

		protected bool TryConvertOutputProperty(object value, string propertyInStr, out object convertedValue)
		{
			convertedValue = null;
			return this.outputPropertyConverter != null && ObjectOutputHelper.TryConvertOutputProperty(value, this, null, propertyInStr, this.outputPropertyConverter.GetInvocationList(), out convertedValue);
		}

		protected virtual void ValidateRead(List<ValidationError> errors)
		{
			this.ValidateRead(errors, (this.ObjectSchema != null) ? this.ObjectSchema.AllProperties : null);
		}

		internal void ValidateRead(List<ValidationError> errors, IEnumerable<PropertyDefinition> propertiesToValidate)
		{
			ExchangeObjectVersion exchangeObjectVersion = (this.propertyBag.ObjectVersionPropertyDefinition == null) ? null : this.ExchangeVersion;
			if (propertiesToValidate != null)
			{
				foreach (PropertyDefinition propertyDefinition in propertiesToValidate)
				{
					ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)propertyDefinition;
					if (providerPropertyDefinition.IsCalculated)
					{
						bool onlyCacheValue = null != exchangeObjectVersion && exchangeObjectVersion.IsOlderThan(providerPropertyDefinition.VersionAdded);
						this.ValidateCalculatedProperty(providerPropertyDefinition, this.propertyBag, errors, true, onlyCacheValue);
					}
				}
			}
			List<ValidationError> list = this.instantiationErrors;
			if (list == null || list.Count == 0)
			{
				return;
			}
			list.RemoveAll(delegate(ValidationError error)
			{
				PropertyValidationError propertyValidationError = error as PropertyValidationError;
				if (propertyValidationError == null)
				{
					return false;
				}
				ProviderPropertyDefinition providerPropertyDefinition2 = propertyValidationError.PropertyDefinition as ProviderPropertyDefinition;
				if (providerPropertyDefinition2 == null)
				{
					return false;
				}
				bool flag = providerPropertyDefinition2.IsMultivalued && ((MultiValuedPropertyBase)this[providerPropertyDefinition2]).Changed;
				if (flag)
				{
					ExTraceGlobals.ValidationTracer.TraceDebug<ValidationError>((long)this.GetHashCode(), "Removing instantiation error '{0}'.", error);
				}
				return flag;
			});
			errors.AddRange(list);
		}

		protected virtual void ValidateWrite(List<ValidationError> errors)
		{
			if (!this.IsReadOnly)
			{
				this.RunFullPropertyValidation(errors);
			}
		}

		internal virtual bool SkipFullPropertyValidation(ProviderPropertyDefinition propertyDefinition)
		{
			return !propertyDefinition.IsMultivalued && !propertyDefinition.HasAutogeneratedConstraints && this.propertyBag.IsChanged(propertyDefinition);
		}

		private void RunFullPropertyValidation(List<ValidationError> errors)
		{
			IEnumerable<PropertyDefinition> enumerable = (this.ObjectSchema == null) ? ((IEnumerable<PropertyDefinition>)this.propertyBag.Keys) : this.ObjectSchema.AllProperties;
			foreach (PropertyDefinition propertyDefinition in enumerable)
			{
				ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)propertyDefinition;
				if (!this.SkipFullPropertyValidation(providerPropertyDefinition))
				{
					if (providerPropertyDefinition.IsCalculated)
					{
						this.ValidateCalculatedProperty(providerPropertyDefinition, this.propertyBag, errors, false, false);
					}
					else
					{
						object value = null;
						this.propertyBag.TryGetField(providerPropertyDefinition, ref value);
						IList<ValidationError> list = providerPropertyDefinition.ValidateProperty(value, this.propertyBag, false);
						if (list.Count > 0)
						{
							foreach (ValidationError item in list)
							{
								if (!errors.Contains(item))
								{
									errors.Add(item);
								}
							}
						}
					}
				}
			}
		}

		private void ValidateCalculatedProperty(ProviderPropertyDefinition propertyDefinition, PropertyBag propertyBag, List<ValidationError> errors, bool useOnlyReadConstraints, bool onlyCacheValue)
		{
			try
			{
				object value = propertyBag[propertyDefinition];
				if (!onlyCacheValue)
				{
					IList<ValidationError> list = propertyDefinition.ValidateProperty(value, propertyBag, useOnlyReadConstraints);
					if (list.Count > 0)
					{
						foreach (ValidationError item in list)
						{
							if (!errors.Contains(item))
							{
								errors.Add(item);
							}
						}
					}
				}
			}
			catch (DataValidationException ex)
			{
				ExTraceGlobals.ValidationTracer.TraceWarning<ProviderPropertyDefinition, DataValidationException>(0L, "Calculated property {0} threw an exception {1}.", propertyDefinition, ex);
				if (useOnlyReadConstraints && !onlyCacheValue)
				{
					errors.Add(ex.Error);
				}
			}
		}

		public virtual bool IsValid
		{
			get
			{
				return 0 == this.Validate().Length;
			}
		}

		internal ObjectState ObjectState
		{
			get
			{
				ObjectState objectState = (ObjectState)this.propertyBag[this.propertyBag.ObjectStatePropertyDefinition];
				if (objectState == ObjectState.Unchanged && this.IsChanged())
				{
					objectState = ObjectState.Changed;
					this.propertyBag.SetField(this.propertyBag.ObjectStatePropertyDefinition, objectState);
					this.propertyBag.ResetChangeTracking(this.propertyBag.ObjectStatePropertyDefinition);
				}
				return objectState;
			}
			private set
			{
				this.propertyBag.SetField(this.propertyBag.ObjectStatePropertyDefinition, value);
				this.propertyBag.ResetChangeTracking(this.propertyBag.ObjectStatePropertyDefinition);
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return this.ObjectState;
			}
		}

		internal void CheckWritable()
		{
			if (!this.IsReadOnly)
			{
				return;
			}
			if (this.propertyBag.ObjectVersionPropertyDefinition != null && this.MaximumSupportedExchangeObjectVersion.IsOlderThan(this.ExchangeVersion))
			{
				throw new InvalidObjectOperationException(DataStrings.ExceptionReadOnlyBecauseTooNew(this.ExchangeVersion, this.MaximumSupportedExchangeObjectVersion));
			}
			throw new InvalidObjectOperationException(DataStrings.ExceptionReadOnlyPropertyBag);
		}

		internal virtual void CopyChangesFrom(ConfigurableObject changedObject)
		{
			if (changedObject == null)
			{
				throw new ArgumentNullException("changedObject");
			}
			this.CheckWritable();
			ProviderPropertyDefinition[] array = new ProviderPropertyDefinition[changedObject.propertyBag.Keys.Count];
			changedObject.propertyBag.Keys.CopyTo(array, 0);
			foreach (ProviderPropertyDefinition providerPropertyDefinition in array)
			{
				if (!providerPropertyDefinition.IsReadOnly && (!providerPropertyDefinition.IsWriteOnce || this.ObjectState == ObjectState.New) && (changedObject.propertyBag.SaveCalculatedValues || !providerPropertyDefinition.IsCalculated) && changedObject.IsModified(providerPropertyDefinition))
				{
					object obj = changedObject[providerPropertyDefinition];
					MultiValuedPropertyBase multiValuedPropertyBase = obj as MultiValuedPropertyBase;
					if (!providerPropertyDefinition.IsMultivalued || multiValuedPropertyBase == null || multiValuedPropertyBase.IsChangesOnlyCopy)
					{
						this[providerPropertyDefinition] = obj;
					}
					else
					{
						MultiValuedPropertyBase multiValuedPropertyBase2 = (MultiValuedPropertyBase)this[providerPropertyDefinition];
						multiValuedPropertyBase2.CopyChangesFrom(multiValuedPropertyBase);
						this.CleanupInstantiationErrors(providerPropertyDefinition);
					}
				}
			}
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
			this.CopyChangesFrom((ConfigurableObject)source);
		}

		private void CleanupInstantiationErrors(ProviderPropertyDefinition property)
		{
			List<ValidationError> list = this.instantiationErrors;
			if (list == null || list.Count == 0)
			{
				return;
			}
			List<ProviderPropertyDefinition> listToClear = new List<ProviderPropertyDefinition>();
			listToClear.Add(property);
			if (property.IsCalculated)
			{
				foreach (ProviderPropertyDefinition providerPropertyDefinition in property.SupportingProperties)
				{
					if (this.IsChanged(providerPropertyDefinition))
					{
						listToClear.Add(providerPropertyDefinition);
					}
				}
			}
			list.RemoveAll(delegate(ValidationError error)
			{
				PropertyValidationError propertyValidationError = error as PropertyValidationError;
				ProviderPropertyDefinition providerPropertyDefinition2 = (ProviderPropertyDefinition)propertyValidationError.PropertyDefinition;
				bool flag = false;
				if (providerPropertyDefinition2 != null)
				{
					flag = listToClear.Contains(providerPropertyDefinition2);
					if (flag)
					{
						this.propertyBag.MarkAsChanged(providerPropertyDefinition2);
						if (ExTraceGlobals.ValidationTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.ValidationTracer.TraceDebug<ValidationError, string, string>((long)this.GetHashCode(), "Removing instantiation error '{0}'. Because property {1} has been modified directly or it is a supporting property of {2}.", error, providerPropertyDefinition2.Name, property.Name);
						}
					}
				}
				return flag;
			});
		}

		internal void MarkAsDeleted()
		{
			this.ObjectState = ObjectState.Deleted;
		}

		internal void ResetChangeTracking(bool markAsUnchanged)
		{
			this.propertyBag.ResetChangeTracking();
			if (markAsUnchanged || ObjectState.Changed == this.ObjectState)
			{
				this.ObjectState = ObjectState.Unchanged;
			}
		}

		internal void ResetChangeTracking()
		{
			this.ResetChangeTracking(false);
		}

		void IConfigurable.ResetChangeTracking()
		{
			this.ResetChangeTracking();
		}

		private void UpdateInstantiationErrors(ValidationError[] errors)
		{
			ExTraceGlobals.SerializationTracer.TraceFunction((long)this.GetHashCode(), "ConfigurableObject.UpdateInstantiationErrors(). Errors = {0}", (object[])errors);
			if (errors == null)
			{
				throw new ArgumentNullException("errors");
			}
			if (errors.Length > 0)
			{
				bool flag = false;
				List<ValidationError> list = this.InstantiationErrors;
				foreach (ValidationError item in errors)
				{
					if (!list.Contains(item))
					{
						list.Add(item);
						flag = true;
					}
				}
				if (flag)
				{
					ExTraceGlobals.SerializationTracer.TraceDebug((long)this.GetHashCode(), "ConfigurableObject.UpdateInstantiationErrors(). Serialization added new Errors. Marking object as ReadOnly.", (object[])errors);
					this.SetIsReadOnly(true);
					list.Add(new ObjectValidationError(DataStrings.ErrorObjectSerialization(this.Identity, ConfigurableObject.ExecutingAssemblyVersion, this.serializerAssemblyVersion), this.Identity, null));
				}
			}
		}

		internal bool TryGetValueWithoutDefault(PropertyDefinition propertyDefinition, out object value)
		{
			return this.propertyBag.TryGetValueWithoutDefault(propertyDefinition, out value);
		}

		internal bool TryGetOriginalValue<T>(ProviderPropertyDefinition key, out T value)
		{
			object obj;
			bool result = this.propertyBag.TryGetOriginalValue(key, out obj);
			value = (T)((object)obj);
			return result;
		}

		internal virtual object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)propertyDefinition;
				object obj;
				try
				{
					obj = this.propertyBag[providerPropertyDefinition];
					obj = this.InternalSuppressPii(propertyDefinition, obj);
					object obj2;
					if (this.TryConvertOutputProperty(obj, providerPropertyDefinition, out obj2))
					{
						obj = obj2;
					}
				}
				catch (DataValidationException arg)
				{
					ExTraceGlobals.ValidationTracer.TraceError<ProviderPropertyDefinition, DataValidationException>(0L, "Calculated property {0} threw an exception {1}. Returning default value.", providerPropertyDefinition, arg);
					obj = this.propertyBag.SetField(providerPropertyDefinition, providerPropertyDefinition.DefaultValue);
					this.propertyBag.ResetChangeTracking(providerPropertyDefinition);
				}
				return obj;
			}
			set
			{
				ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)propertyDefinition;
				this.propertyBag[providerPropertyDefinition] = value;
				this.CleanupInstantiationErrors(providerPropertyDefinition);
			}
		}

		object IPropertyBag.this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return this[propertyDefinition];
			}
			set
			{
				this[propertyDefinition] = value;
			}
		}

		internal void SetProperties(ICollection<PropertyDefinition> propertyDefinitions, object[] propertyValues)
		{
			if (propertyDefinitions == null)
			{
				throw new ArgumentNullException("propertyDefinitions");
			}
			if (this.IsReadOnly && this.propertyBag.ObjectVersionPropertyDefinition != null && this.MaximumSupportedExchangeObjectVersion.IsOlderThan(this.ExchangeVersion))
			{
				throw new InvalidObjectOperationException(DataStrings.ExceptionReadOnlyBecauseTooNew(this.ExchangeVersion, this.MaximumSupportedExchangeObjectVersion));
			}
			ProviderPropertyDefinition[] array = new ProviderPropertyDefinition[propertyDefinitions.Count];
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
			{
				array[num++] = (ProviderPropertyDefinition)propertyDefinition;
			}
			this.propertyBag.SetProperties(array, propertyValues);
		}

		void IPropertyBag.SetProperties(ICollection<PropertyDefinition> propertyDefinitions, object[] propertyValues)
		{
			this.SetProperties(propertyDefinitions, propertyValues);
		}

		object IReadOnlyPropertyBag.this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return this[propertyDefinition];
			}
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitions)
		{
			if (propertyDefinitions == null)
			{
				throw new ArgumentNullException("propertyDefinitions");
			}
			ProviderPropertyDefinition[] array = new ProviderPropertyDefinition[propertyDefinitions.Count];
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
			{
				array[num++] = (ProviderPropertyDefinition)propertyDefinition;
			}
			return this.propertyBag.GetProperties(array);
		}

		internal List<ValidationError> InstantiationErrors
		{
			get
			{
				if (this.instantiationErrors == null)
				{
					this.instantiationErrors = new List<ValidationError>();
				}
				return this.instantiationErrors;
			}
			set
			{
				this.instantiationErrors = value;
			}
		}

		internal void EnableSaveCalculatedValues()
		{
			this.propertyBag.EnableSaveCalculatedValues();
		}

		internal virtual void InitializeSchema()
		{
		}

		internal IList<PropertyDefinition> GetChangedPropertyDefinitions()
		{
			IEnumerable<PropertyDefinition> enumerable = (this.ObjectSchema == null) ? ((IEnumerable<PropertyDefinition>)this.propertyBag.Keys) : this.ObjectSchema.AllProperties;
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			foreach (PropertyDefinition propertyDefinition in enumerable)
			{
				ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)propertyDefinition;
				if (this.propertyBag.IsChanged(providerPropertyDefinition))
				{
					list.Add(providerPropertyDefinition);
				}
			}
			return list;
		}

		internal ConfigurableObject GetOriginalObject()
		{
			ConfigurableObject configurableObject = (ConfigurableObject)Activator.CreateInstance(base.GetType());
			configurableObject.propertyBag = this.propertyBag.GetOriginalBag();
			return configurableObject;
		}

		internal void OverrideCorruptedValuesWithDefault()
		{
			ValidationError[] array = this.Validate();
			foreach (ValidationError validationError in array)
			{
				PropertyValidationError propertyValidationError = validationError as PropertyValidationError;
				if (propertyValidationError != null)
				{
					this[propertyValidationError.PropertyDefinition] = (propertyValidationError.PropertyDefinition as ProviderPropertyDefinition).DefaultValue;
				}
			}
		}

		private bool IsChanged()
		{
			return this.propertyBag.Changed;
		}

		internal bool IsModified(ProviderPropertyDefinition providerPropertyDefinition)
		{
			return this.propertyBag.IsModified(providerPropertyDefinition);
		}

		internal bool IsChanged(ProviderPropertyDefinition providerPropertyDefinition)
		{
			return this.propertyBag.IsChanged(providerPropertyDefinition);
		}

		internal static IList<ProviderPropertyDefinition> GetPropertiesThatDiffer(ConfigurableObject objA, ConfigurableObject objB, IList<ProviderPropertyDefinition> properties)
		{
			if (objA == null)
			{
				throw new ArgumentNullException("objA");
			}
			if (objB == null)
			{
				throw new ArgumentNullException("objB");
			}
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			List<ProviderPropertyDefinition> list = new List<ProviderPropertyDefinition>();
			for (int i = 0; i < properties.Count; i++)
			{
				ProviderPropertyDefinition providerPropertyDefinition = properties[i];
				if (!object.Equals(objA[providerPropertyDefinition], objB[providerPropertyDefinition]))
				{
					list.Add(providerPropertyDefinition);
				}
			}
			return list;
		}

		internal static bool AreEqual(ConfigurableObject objA, ConfigurableObject objB)
		{
			if (objA == null || objB == null)
			{
				return objA == null && objB == null;
			}
			if (objA.GetType() != objB.GetType())
			{
				return false;
			}
			ObjectSchema objectSchema = objA.ObjectSchema;
			foreach (PropertyDefinition propertyDefinition in objectSchema.AllProperties)
			{
				if (!object.Equals(objA[propertyDefinition], objB[propertyDefinition]))
				{
					return false;
				}
			}
			return true;
		}

		internal virtual bool SkipPiiRedaction
		{
			get
			{
				return false;
			}
		}

		private static void CheckCallStack()
		{
			if (!Environment.StackTrace.Contains("Clone"))
			{
				throw new NotSupportedException("ConfigurableObjects without schema can only be serialized for cloning.");
			}
		}

		private object InternalSuppressPii(PropertyDefinition propertyDefinition, object value)
		{
			if (SuppressingPiiContext.NeedPiiSuppression)
			{
				bool skipPiiRedaction;
				using (SuppressingPiiContext.Create(false, null))
				{
					skipPiiRedaction = this.SkipPiiRedaction;
				}
				if (!skipPiiRedaction)
				{
					value = SuppressingPiiProperty.TryRedact(propertyDefinition, value, SuppressingPiiContext.PiiMap);
				}
			}
			return value;
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			this.serializerAssemblyVersion = ConfigurableObject.ExecutingAssemblyVersion;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (this.propertyBag != null && this.DeserializationSchema != null)
			{
				this.propertyBag.SetUpdateErrorsCallback(new UpdateErrorsDelegate(this.UpdateInstantiationErrors));
				this.propertyBag.SetConfigObjectSchema(this.DeserializationSchema.AllProperties);
			}
		}

		private Version serializerAssemblyVersion;

		private List<ValidationError> instantiationErrors;

		internal PropertyBag propertyBag;

		[NonSerialized]
		private ConvertOutputPropertyDelegate outputPropertyConverter;

		[NonSerialized]
		private object proxyInfo;

		internal static Version ExecutingAssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
	}
}
