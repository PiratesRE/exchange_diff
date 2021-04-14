using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Data;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal abstract class PropertyBag : ICollection, IEnumerable, IPropertyBag, IReadOnlyPropertyBag, ICloneable
	{
		public PropertyBag(bool readOnly, int initialSize)
		{
			ProviderPropertyDefinition objectVersionPropertyDefinition = this.ObjectVersionPropertyDefinition;
			this.readOnly = readOnly;
			this.storeValuesOnly = false;
			this.currentValues = new Dictionary<ProviderPropertyDefinition, object>(initialSize);
		}

		public PropertyBag() : this(false, 16)
		{
		}

		internal void SetUpdateErrorsCallback(UpdateErrorsDelegate callback)
		{
			this.updateErrors = callback;
		}

		public int Count
		{
			get
			{
				return this.currentValues.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		public bool Changed
		{
			get
			{
				if (this.IsReadOnly || this.originalValues == null || this.OriginalValues.Count == 0)
				{
					return false;
				}
				foreach (ProviderPropertyDefinition key in this.OriginalValues.Keys)
				{
					if (this.IsChanged(key))
					{
						return true;
					}
				}
				return false;
			}
		}

		public ICollection Keys
		{
			get
			{
				return this.currentValues.Keys;
			}
		}

		public ICollection Values
		{
			get
			{
				return this.currentValues.Values;
			}
		}

		private Dictionary<ProviderPropertyDefinition, object> OriginalValues
		{
			get
			{
				if (this.originalValues == null)
				{
					this.originalValues = new Dictionary<ProviderPropertyDefinition, object>(Math.Min(this.currentValues.Count / 2, 4));
				}
				return this.originalValues;
			}
		}

		internal virtual bool ProcessCalculatedProperies
		{
			get
			{
				return true;
			}
		}

		internal abstract ProviderPropertyDefinition ObjectVersionPropertyDefinition { get; }

		internal abstract ProviderPropertyDefinition ObjectStatePropertyDefinition { get; }

		internal abstract ProviderPropertyDefinition ObjectIdentityPropertyDefinition { get; }

		internal virtual MultiValuedPropertyBase CreateMultiValuedProperty(ProviderPropertyDefinition propertyDefinition, bool createAsReadOnly, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage)
		{
			return ValueConvertor.CreateGenericMultiValuedProperty(propertyDefinition, createAsReadOnly, values, invalidValues, readOnlyErrorMessage);
		}

		internal bool SaveCalculatedValues
		{
			get
			{
				return this.storeValuesOnly;
			}
		}

		internal void EnableSaveCalculatedValues()
		{
			this.storeValuesOnly = true;
		}

		public object Clone()
		{
			PropertyBag propertyBag = (PropertyBag)CloneHelper.SerializeObj(this);
			List<PropertyDefinition> list = new List<PropertyDefinition>(this.Keys.Count);
			foreach (object obj in this.Keys)
			{
				PropertyDefinition item = (PropertyDefinition)obj;
				list.Add(item);
			}
			propertyBag.SetConfigObjectSchema(list);
			return propertyBag;
		}

		internal bool IsReadOnlyProperty(ProviderPropertyDefinition propertyDefinition)
		{
			LocalizedString? localizedString;
			return this.IsReadOnlyProperty(propertyDefinition, out localizedString);
		}

		private bool IsReadOnlyProperty(ProviderPropertyDefinition propertyDefinition, out LocalizedString? reason)
		{
			reason = null;
			if (this.IsReadOnly && !propertyDefinition.IsTaskPopulated)
			{
				reason = new LocalizedString?(DataStrings.ErrorReadOnlyCauseObject(propertyDefinition.Name));
				return true;
			}
			if (propertyDefinition.IsReadOnly)
			{
				reason = new LocalizedString?(DataStrings.ErrorReadOnlyCauseProperty(propertyDefinition.Name));
				return true;
			}
			if (this.ObjectVersionPropertyDefinition != null)
			{
				if (object.Equals(propertyDefinition, this.ObjectVersionPropertyDefinition))
				{
					reason = new LocalizedString?(DataStrings.ErrorObjectVersionReadOnly(propertyDefinition.Name));
					return true;
				}
				ExchangeObjectVersion exchangeObjectVersion = (ExchangeObjectVersion)this[this.ObjectVersionPropertyDefinition];
				ExchangeObjectVersion versionAdded = propertyDefinition.VersionAdded;
				if (exchangeObjectVersion.IsOlderThan(versionAdded))
				{
					reason = new LocalizedString?(DataStrings.ExceptionPropertyTooNew(propertyDefinition.Name, versionAdded, exchangeObjectVersion));
					return true;
				}
			}
			if (this.ObjectStatePropertyDefinition != null && propertyDefinition.IsWriteOnce && (ObjectState)this[this.ObjectStatePropertyDefinition] != ObjectState.New)
			{
				reason = new LocalizedString?(DataStrings.ExceptionWriteOnceProperty(propertyDefinition.Name));
				return true;
			}
			return false;
		}

		private bool IsCalculatedProperty(ProviderPropertyDefinition propertyDefinition)
		{
			return propertyDefinition.IsCalculated && !this.storeValuesOnly;
		}

		internal bool TryGetField(ProviderPropertyDefinition key, ref object value)
		{
			return this.currentValues.TryGetValue(key, out value);
		}

		internal void DangerousSetValue(ProviderPropertyDefinition key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			IList<ValidationError> list = key.ValidateProperty(value, this, false);
			if (list.Count > 0)
			{
				throw new DataValidationException(list[0]);
			}
			this.SetField(key, value);
		}

		internal object SetField(ProviderPropertyDefinition key, object value)
		{
			ExTraceGlobals.PropertyBagTracer.TraceDebug<ProviderPropertyDefinition, object>((long)this.GetHashCode(), "SetField({0}, {1})", key, value ?? "<NULL>");
			if (key.IsMultivalued)
			{
				MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)value;
				IntRange valueRange = (multiValuedPropertyBase != null) ? multiValuedPropertyBase.ValueRange : null;
				bool isCompletelyRead = multiValuedPropertyBase == null || multiValuedPropertyBase.IsCompletelyRead;
				LocalizedString? readOnlyErrorMessage;
				bool createAsReadOnly = this.IsReadOnlyProperty(key, out readOnlyErrorMessage);
				multiValuedPropertyBase = this.CreateMultiValuedProperty(key, createAsReadOnly, multiValuedPropertyBase ?? PropertyBag.EmptyArray, null, readOnlyErrorMessage);
				multiValuedPropertyBase.IsCompletelyRead = isCompletelyRead;
				multiValuedPropertyBase.ValueRange = valueRange;
				value = multiValuedPropertyBase;
				object obj = null;
				this.TryGetField(key, ref obj);
				MultiValuedPropertyBase multiValuedPropertyBase2 = (MultiValuedPropertyBase)obj;
				if (multiValuedPropertyBase2 != null)
				{
					multiValuedPropertyBase2.CollectionChanging -= this.MvpChangingHandler;
					multiValuedPropertyBase2.CollectionChanged -= this.MvpChangedHandler;
				}
				multiValuedPropertyBase.CollectionChanging += this.MvpChangingHandler;
				multiValuedPropertyBase.CollectionChanged += this.MvpChangedHandler;
			}
			else if (!this.IsReadOnly)
			{
				object obj2 = null;
				if (!this.OriginalValues.ContainsKey(key))
				{
					this.OriginalValues[key] = (this.TryGetField(key, ref obj2) ? obj2 : null);
				}
			}
			this.currentValues[key] = value;
			return value;
		}

		private EventHandler MvpChangingHandler
		{
			get
			{
				if (this.mvpChangingHandler == null)
				{
					this.mvpChangingHandler = new EventHandler(this.MvpChanging);
				}
				return this.mvpChangingHandler;
			}
		}

		private EventHandler MvpChangedHandler
		{
			get
			{
				if (this.mvpChangedHandler == null)
				{
					this.mvpChangedHandler = new EventHandler(this.MvpChanged);
				}
				return this.mvpChangedHandler;
			}
		}

		private void MvpChanging(object sender, EventArgs e)
		{
			MultiValuedPropertyBase multiValuedPropertyBase = sender as MultiValuedPropertyBase;
			ProviderPropertyDefinition propertyDefinition = multiValuedPropertyBase.PropertyDefinition;
			object obj = null;
			if (this.TryGetField(propertyDefinition, ref obj) && obj == multiValuedPropertyBase && !this.readOnly && !this.OriginalValues.ContainsKey(propertyDefinition))
			{
				ExTraceGlobals.PropertyBagTracer.TraceDebug<ProviderPropertyDefinition>((long)this.GetHashCode(), "'{0}' MVP is about to change for the first time. Backing up original value.", propertyDefinition);
				this.OriginalValues[propertyDefinition] = this.CreateMultiValuedProperty(propertyDefinition, true, multiValuedPropertyBase, null, new LocalizedString?(DataStrings.ErrorOrignalMultiValuedProperty(propertyDefinition.Name)));
			}
		}

		private void MvpChanged(object sender, EventArgs e)
		{
			MultiValuedPropertyBase multiValuedPropertyBase = sender as MultiValuedPropertyBase;
			ProviderPropertyDefinition propertyDefinition = multiValuedPropertyBase.PropertyDefinition;
			object obj = null;
			if (this.TryGetField(propertyDefinition, ref obj) && obj == multiValuedPropertyBase)
			{
				ExTraceGlobals.PropertyBagTracer.TraceDebug<ProviderPropertyDefinition>((long)this.GetHashCode(), "{0} MVP has changed. Updating any dependent properties.", propertyDefinition);
				this.UpdatePropertyDependents(propertyDefinition, multiValuedPropertyBase);
			}
		}

		private void UpdatePropertyDependents(ProviderPropertyDefinition propertyDefinition, object value)
		{
			if (!this.ProcessCalculatedProperies)
			{
				ExTraceGlobals.PropertyBagTracer.TraceDebug<ProviderPropertyDefinition, object>((long)this.GetHashCode(), "Skip Calcualted Properties logic; Property = {0}; Value = {1};", propertyDefinition, value ?? "<NULL>");
				if (!propertyDefinition.IsMultivalued)
				{
					this.SetField(propertyDefinition, value);
				}
				return;
			}
			try
			{
				ExTraceGlobals.PropertyBagTracer.TraceDebug((long)this.GetHashCode(), "Recursion Depth = {0}; Property = {1}; Value = {2}; ShouldCallSetters = {3};", new object[]
				{
					this.recursionDepth,
					propertyDefinition,
					value ?? "<NULL>",
					this.shouldCallSetters
				});
				this.recursionDepth++;
				if (this.shouldCallSetters)
				{
					if (this.IsCalculatedProperty(propertyDefinition) && propertyDefinition.SetterDelegate != null)
					{
						propertyDefinition.SetterDelegate(value, this);
					}
					if (1 == this.recursionDepth)
					{
						this.shouldCallSetters = false;
					}
				}
				if (!propertyDefinition.IsMultivalued)
				{
					this.SetField(propertyDefinition, value);
				}
				if (!this.shouldCallSetters && 1 == this.recursionDepth)
				{
					if (this.IsCalculatedProperty(propertyDefinition))
					{
						HashSet<ProviderPropertyDefinition> hashSet = new HashSet<ProviderPropertyDefinition>();
						hashSet.TryAdd(propertyDefinition);
						using (ReadOnlyCollection<ProviderPropertyDefinition>.Enumerator enumerator = propertyDefinition.SupportingProperties.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ProviderPropertyDefinition propertyDefinition2 = enumerator.Current;
								this.UpdateDependentMvps(propertyDefinition2, hashSet);
							}
							goto IL_1A5;
						}
					}
					if (this.IsReadOnly)
					{
						foreach (ProviderPropertyDefinition providerPropertyDefinition in propertyDefinition.DependentProperties)
						{
							if (!providerPropertyDefinition.IsMultivalued)
							{
								object value2 = providerPropertyDefinition.GetterDelegate(this);
								this.SetField(providerPropertyDefinition, value2);
							}
						}
					}
					this.UpdateDependentMvps(propertyDefinition, null);
				}
				IL_1A5:;
			}
			finally
			{
				this.recursionDepth--;
				if (this.recursionDepth == 0)
				{
					this.shouldCallSetters = true;
				}
			}
		}

		private void UpdateDependentMvps(ProviderPropertyDefinition propertyDefinition, HashSet<ProviderPropertyDefinition> visitedProperties)
		{
			ExTraceGlobals.PropertyBagTracer.TraceDebug<ProviderPropertyDefinition>((long)this.GetHashCode(), "UpdateDependentMvps {0}", propertyDefinition);
			if (!this.IsChanged(propertyDefinition))
			{
				return;
			}
			foreach (ProviderPropertyDefinition providerPropertyDefinition in propertyDefinition.DependentProperties)
			{
				object obj = null;
				if (this.IsCalculatedProperty(providerPropertyDefinition) && providerPropertyDefinition.IsMultivalued && (visitedProperties == null || visitedProperties.TryAdd(providerPropertyDefinition)) && this.TryGetField(providerPropertyDefinition, ref obj))
				{
					ExTraceGlobals.PropertyBagTracer.TraceDebug<ProviderPropertyDefinition>((long)this.GetHashCode(), "Updating dependent MVP {0}", providerPropertyDefinition);
					MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)obj;
					MultiValuedPropertyBase newMvp = (MultiValuedPropertyBase)providerPropertyDefinition.GetterDelegate(this);
					multiValuedPropertyBase.UpdateValues(newMvp);
				}
			}
		}

		internal void MarkAsChanged(ProviderPropertyDefinition key)
		{
			if (this.IsReadOnly)
			{
				throw new InvalidObjectOperationException(DataStrings.ErrorReadOnlyCauseObject(key.Name));
			}
			object obj;
			if (this.currentValues.TryGetValue(key, out obj))
			{
				MultiValuedPropertyBase multiValuedPropertyBase = obj as MultiValuedPropertyBase;
				if (multiValuedPropertyBase != null)
				{
					multiValuedPropertyBase.MarkAsChanged();
					return;
				}
				this.OriginalValues[key] = PropertyBag.ChangeMarker;
			}
		}

		internal void ResetChangeTracking(ProviderPropertyDefinition key)
		{
			object obj;
			if (this.currentValues.TryGetValue(key, out obj))
			{
				MultiValuedPropertyBase multiValuedPropertyBase = obj as MultiValuedPropertyBase;
				if (multiValuedPropertyBase != null && multiValuedPropertyBase.Changed)
				{
					multiValuedPropertyBase.ResetChangeTracking();
				}
				if (this.originalValues != null)
				{
					this.OriginalValues.Remove(key);
				}
			}
		}

		internal bool TryGetOriginalValue(ProviderPropertyDefinition key, out object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			bool result = (!this.IsReadOnly && this.originalValues != null && this.OriginalValues.TryGetValue(key, out value)) || this.currentValues.TryGetValue(key, out value);
			if (value == null || PropertyBag.ChangeMarker.Equals(value))
			{
				value = ((!key.IsMultivalued) ? key.DefaultValue : this.CreateMultiValuedProperty(key, true, new object[0], null, new LocalizedString?(DataStrings.ErrorOrignalMultiValuedProperty(key.Name))));
			}
			else
			{
				MultiValuedPropertyBase multiValuedPropertyBase = value as MultiValuedPropertyBase;
				if (multiValuedPropertyBase != null && (key.IsCalculated || !multiValuedPropertyBase.IsReadOnly))
				{
					value = this.CreateMultiValuedProperty(key, true, multiValuedPropertyBase, null, new LocalizedString?(DataStrings.ErrorOrignalMultiValuedProperty(key.Name)));
				}
			}
			return result;
		}

		internal PropertyBag GetOriginalBag()
		{
			PropertyBag propertyBag = (PropertyBag)Activator.CreateInstance(base.GetType(), new object[]
			{
				true,
				this.Count
			});
			foreach (ProviderPropertyDefinition key in this.currentValues.Keys)
			{
				object value;
				this.TryGetOriginalValue(key, out value);
				propertyBag.SetField(key, value);
			}
			return propertyBag;
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitions)
		{
			if (propertyDefinitions == null)
			{
				throw new ArgumentNullException("propertyDefinitions");
			}
			object[] array = new object[propertyDefinitions.Count];
			int num = 0;
			foreach (PropertyDefinition key in propertyDefinitions)
			{
				array[num++] = this[key];
			}
			return array;
		}

		public void SetProperties(ICollection<PropertyDefinition> propertyDefinitions, object[] propertyValues)
		{
			if (propertyDefinitions == null)
			{
				throw new ArgumentNullException("propertyDefinitions");
			}
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues");
			}
			int num = 0;
			foreach (PropertyDefinition key in propertyDefinitions)
			{
				this[key] = propertyValues[num++];
			}
		}

		internal void SetObjectVersion(ExchangeObjectVersion newVersion)
		{
			ExTraceGlobals.PropertyBagTracer.TraceDebug<ExchangeObjectVersion>((long)this.GetHashCode(), "PropertyBag::SetObjectVersion({0})", newVersion);
			if (this.ObjectVersionPropertyDefinition != null)
			{
				ExchangeObjectVersion objB = (ExchangeObjectVersion)this[this.ObjectVersionPropertyDefinition];
				if (!object.Equals(newVersion, objB))
				{
					ExchangeObjectVersion value = object.Equals(newVersion, this.ObjectVersionPropertyDefinition.DefaultValue) ? null : newVersion;
					this.SetField(this.ObjectVersionPropertyDefinition, value);
					this.UpdateMvpsAsNecessary();
				}
			}
		}

		internal void SetIsReadOnly(bool readOnly)
		{
			ExTraceGlobals.PropertyBagTracer.TraceDebug<bool>((long)this.GetHashCode(), "PropertyBag::SetIsReadOnly({0})", readOnly);
			if (readOnly != this.IsReadOnly)
			{
				this.ResetChangeTracking();
				this.readOnly = readOnly;
				this.originalValues = null;
				this.UpdateMvpsAsNecessary();
			}
		}

		private void UpdateMvpsAsNecessary()
		{
			List<ProviderPropertyDefinition> list = new List<ProviderPropertyDefinition>(this.currentValues.Keys);
			foreach (ProviderPropertyDefinition providerPropertyDefinition in list)
			{
				if (!providerPropertyDefinition.IsReadOnly && providerPropertyDefinition.IsMultivalued)
				{
					object obj = null;
					if (this.TryGetField(providerPropertyDefinition, ref obj) && obj != null)
					{
						LocalizedString? readOnlyErrorMessage;
						bool isReadOnly = this.IsReadOnlyProperty(providerPropertyDefinition, out readOnlyErrorMessage);
						MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)obj;
						multiValuedPropertyBase.SetIsReadOnly(isReadOnly, readOnlyErrorMessage);
					}
				}
			}
		}

		internal bool IsChanged(ProviderPropertyDefinition key)
		{
			if (!this.IsModified(key))
			{
				return false;
			}
			if (key.IsMultivalued)
			{
				return ((MultiValuedPropertyBase)this.currentValues[key]).Changed;
			}
			return !object.Equals(this.OriginalValues[key], this.currentValues[key]);
		}

		internal bool IsModified(ProviderPropertyDefinition key)
		{
			if (this.IsReadOnly)
			{
				return false;
			}
			if (this.originalValues == null)
			{
				return false;
			}
			if (this.OriginalValues.ContainsKey(key))
			{
				return true;
			}
			if (!this.IsCalculatedProperty(key) || (key.IsMultivalued && this.currentValues.ContainsKey(key)) || !this.HasSupportingProperties(key))
			{
				return false;
			}
			if (key.IsMultivalued)
			{
				MultiValuedPropertyBase multiValuedPropertyValue = this.GetMultiValuedPropertyValue(key);
				return multiValuedPropertyValue.Changed;
			}
			foreach (ProviderPropertyDefinition key2 in key.SupportingProperties)
			{
				if (this.IsModified(key2))
				{
					this.SetField(key, key.GetterDelegate(this));
					return true;
				}
			}
			return false;
		}

		private bool HasSupportingProperties(ProviderPropertyDefinition key)
		{
			if (this.currentValues.ContainsKey(key))
			{
				return true;
			}
			foreach (ProviderPropertyDefinition key2 in key.SupportingProperties)
			{
				if (!this.Contains(key2))
				{
					return false;
				}
			}
			return true;
		}

		internal void ResetChangeTracking()
		{
			ExTraceGlobals.PropertyBagTracer.TraceDebug((long)this.GetHashCode(), "PropertyBag::ResetChangeTracking()");
			if (this.IsReadOnly)
			{
				return;
			}
			if (!this.storeValuesOnly)
			{
				List<ProviderPropertyDefinition> list = new List<ProviderPropertyDefinition>(this.currentValues.Keys);
				foreach (ProviderPropertyDefinition providerPropertyDefinition in list)
				{
					if (providerPropertyDefinition.IsCalculated && !providerPropertyDefinition.IsMultivalued)
					{
						try
						{
							this.SetField(providerPropertyDefinition, this[providerPropertyDefinition]);
						}
						catch (DataValidationException arg)
						{
							ExTraceGlobals.ValidationTracer.TraceError<ProviderPropertyDefinition, DataValidationException>(0L, "Calculated property {0} getter threw an exception {1}.", providerPropertyDefinition, arg);
						}
					}
				}
			}
			if (this.originalValues != null)
			{
				this.originalValues.Clear();
			}
			foreach (object obj in this.currentValues.Values)
			{
				MultiValuedPropertyBase multiValuedPropertyBase = obj as MultiValuedPropertyBase;
				if (multiValuedPropertyBase != null && multiValuedPropertyBase.Changed)
				{
					multiValuedPropertyBase.ResetChangeTracking();
				}
			}
		}

		internal void Remove(ProviderPropertyDefinition key)
		{
			if (this.IsReadOnly)
			{
				throw new InvalidObjectOperationException(DataStrings.ErrorReadOnlyCauseObject(key.Name));
			}
			this.currentValues.Remove(key);
			if (this.originalValues != null)
			{
				this.originalValues.Remove(key);
			}
		}

		public void Clear()
		{
			ExTraceGlobals.PropertyBagTracer.TraceDebug((long)this.GetHashCode(), "PropertyBag::Clear()");
			if (this.IsReadOnly)
			{
				throw new InvalidObjectOperationException(DataStrings.ExceptionReadOnlyPropertyBag);
			}
			this.currentValues.Clear();
			if (this.originalValues != null)
			{
				this.originalValues.Clear();
			}
		}

		internal bool Contains(ProviderPropertyDefinition key)
		{
			return this.currentValues.ContainsKey(key);
		}

		public Dictionary<ProviderPropertyDefinition, object>.Enumerator GetEnumerator()
		{
			return this.currentValues.GetEnumerator();
		}

		private object GetCalculatedPropertyValue(ProviderPropertyDefinition propertyDefinition)
		{
			object obj = null;
			if (!this.IsReadOnly || !this.TryGetField(propertyDefinition, ref obj))
			{
				obj = propertyDefinition.GetterDelegate(this);
				if (!this.currentValues.ContainsKey(propertyDefinition))
				{
					lock (this.lockObject)
					{
						this.currentValues[propertyDefinition] = obj;
					}
				}
			}
			return obj;
		}

		private MultiValuedPropertyBase GetMultiValuedPropertyValue(ProviderPropertyDefinition propertyDefinition)
		{
			return this.GetMultiValuedPropertyValue(propertyDefinition, true);
		}

		private MultiValuedPropertyBase GetMultiValuedPropertyValue(ProviderPropertyDefinition propertyDefinition, bool createDefaultValueEntry)
		{
			object obj = null;
			if (!this.TryGetField(propertyDefinition, ref obj) || obj == null)
			{
				if (!this.IsCalculatedProperty(propertyDefinition))
				{
					if (!createDefaultValueEntry)
					{
						return null;
					}
					obj = this.SetField(propertyDefinition, null);
				}
				else
				{
					bool flag = false;
					if (this.Changed)
					{
						foreach (ProviderPropertyDefinition providerPropertyDefinition in propertyDefinition.SupportingProperties)
						{
							if (this.IsModified(providerPropertyDefinition))
							{
								ExTraceGlobals.PropertyBagTracer.TracePerformance<ProviderPropertyDefinition, ProviderPropertyDefinition>(0L, "Initializing Calculated MVP '{0}' as modified because supporting property '{1}' is modified. This could degrade performance if it happens frequently.", propertyDefinition, providerPropertyDefinition);
								flag = true;
								break;
							}
						}
					}
					object obj2 = propertyDefinition.DefaultValue;
					if (this.HasSupportingProperties(propertyDefinition))
					{
						IPropertyBag propertyBag = flag ? new PropertyBag.OriginalPropertyBag(this) : this;
						try
						{
							obj2 = propertyDefinition.GetterDelegate(propertyBag);
						}
						catch (DataValidationException arg)
						{
							ExTraceGlobals.ValidationTracer.TraceError<ProviderPropertyDefinition, DataValidationException>(0L, "Calculated property {0} getter threw an exception {1}.", propertyDefinition, arg);
						}
					}
					object obj3 = flag ? propertyDefinition.GetterDelegate(this) : obj2;
					if (obj2 == null && obj3 == null && !createDefaultValueEntry)
					{
						return null;
					}
					obj = this.SetField(propertyDefinition, obj2);
					if (flag)
					{
						MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)obj;
						MultiValuedPropertyBase newMvp = (MultiValuedPropertyBase)obj3;
						multiValuedPropertyBase.CollectionChanged -= this.MvpChangedHandler;
						try
						{
							multiValuedPropertyBase.UpdateValues(newMvp);
						}
						finally
						{
							multiValuedPropertyBase.CollectionChanged += this.MvpChangedHandler;
						}
					}
				}
			}
			return obj as MultiValuedPropertyBase;
		}

		public bool TryGetValueWithoutDefault(PropertyDefinition key, out object returnValue)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)key;
			returnValue = null;
			if (providerPropertyDefinition.IsMultivalued)
			{
				returnValue = this.GetMultiValuedPropertyValue(providerPropertyDefinition, false);
				return returnValue != null;
			}
			if (this.IsCalculatedProperty(providerPropertyDefinition))
			{
				returnValue = this.GetCalculatedPropertyValue(providerPropertyDefinition);
				return returnValue != null;
			}
			this.TryGetField(providerPropertyDefinition, ref returnValue);
			return returnValue != null;
		}

		public virtual object this[PropertyDefinition key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)key;
				object obj = null;
				if (providerPropertyDefinition.IsMultivalued)
				{
					return this.GetMultiValuedPropertyValue(providerPropertyDefinition);
				}
				if (this.IsCalculatedProperty(providerPropertyDefinition))
				{
					return this.GetCalculatedPropertyValue(providerPropertyDefinition);
				}
				this.TryGetField(providerPropertyDefinition, ref obj);
				return obj ?? providerPropertyDefinition.DefaultValue;
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				ExTraceGlobals.PropertyBagTracer.TraceDebug<string, object>((long)this.GetHashCode(), "PropertyBag[{0}]={1}.", key.Name, value ?? "<null>");
				ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)key;
				LocalizedString? localizedString;
				if (this.IsReadOnlyProperty(providerPropertyDefinition, out localizedString))
				{
					throw new InvalidObjectOperationException(localizedString.Value);
				}
				IList<ValidationError> list = providerPropertyDefinition.ValidateProperty(value, this, false);
				if (list.Count > 0)
				{
					throw new DataValidationException(list[0]);
				}
				if (!providerPropertyDefinition.IsMultivalued)
				{
					if (!this.IsCalculatedProperty(providerPropertyDefinition) && !providerPropertyDefinition.PersistDefaultValue && providerPropertyDefinition.DefaultValue != null && value != null)
					{
						if (providerPropertyDefinition.DefaultValue.Equals(value))
						{
							ExTraceGlobals.PropertyBagTracer.TraceDebug<string, object>((long)this.GetHashCode(), "Set[{0}] replaces default {1} with null.", providerPropertyDefinition.Name, value ?? "<null>");
							value = null;
						}
						else if (providerPropertyDefinition.Type == typeof(string) && string.Empty.Equals(value))
						{
							ExTraceGlobals.PropertyBagTracer.TraceDebug<string>((long)this.GetHashCode(), "Setting [{0}] to null.", providerPropertyDefinition.Name);
							value = null;
						}
					}
					this.UpdatePropertyDependents(providerPropertyDefinition, value);
					return;
				}
				MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)this[providerPropertyDefinition];
				if (multiValuedPropertyBase == value)
				{
					multiValuedPropertyBase.MarkAsChanged();
					return;
				}
				MultiValuedPropertyBase multiValuedPropertyBase2 = (MultiValuedPropertyBase)value;
				if (multiValuedPropertyBase2 == null || !multiValuedPropertyBase2.IsChangesOnlyCopy)
				{
					multiValuedPropertyBase.UpdateValues(multiValuedPropertyBase2);
					return;
				}
				if (this.storeValuesOnly)
				{
					this.currentValues[providerPropertyDefinition] = multiValuedPropertyBase2;
					this.OriginalValues[providerPropertyDefinition] = multiValuedPropertyBase2;
					return;
				}
				multiValuedPropertyBase.CopyChangesFrom(multiValuedPropertyBase2);
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this.currentValues).CopyTo(array, index);
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return ((ICollection)this.currentValues).IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)this.currentValues).SyncRoot;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.currentValues.GetEnumerator();
		}

		internal void SetConfigObjectSchema(IEnumerable<PropertyDefinition> schema)
		{
			ExTraceGlobals.PropertyBagTracer.TraceDebug<IEnumerable<PropertyDefinition>>((long)this.GetHashCode(), "SetConfigObjectSchema({0})", schema);
			this.fullPropertyDefinitions = schema;
			this.FinalizeDeserialization();
		}

		private void FinalizeDeserialization()
		{
			ExTraceGlobals.PropertyBagTracer.TraceDebug<bool, IEnumerable<PropertyDefinition>, Dictionary<ProviderPropertyDefinition, object>>((long)this.GetHashCode(), "FinalizeDeserialization(); IsDeserializationComplete = {0}; FullPropertyDefinitions = {1}; CurrentValues = {2}", this.isDeserializationComplete, this.fullPropertyDefinitions, this.currentValues);
			if (this.isDeserializationComplete || this.fullPropertyDefinitions == null || this.currentValues == null || this.currentValues.Count == 0)
			{
				return;
			}
			Dictionary<ProviderPropertyDefinition, object> dictionary = this.currentValues;
			Dictionary<ProviderPropertyDefinition, object> dictionary2 = this.originalValues;
			this.currentValues = new Dictionary<ProviderPropertyDefinition, object>(dictionary.Count);
			this.originalValues = null;
			List<ProviderPropertyDefinition> list = new List<ProviderPropertyDefinition>();
			List<ProviderPropertyDefinition> list2 = new List<ProviderPropertyDefinition>();
			Dictionary<ProviderPropertyDefinition, object> dictionary3 = new Dictionary<ProviderPropertyDefinition, object>();
			foreach (PropertyDefinition propertyDefinition in this.fullPropertyDefinitions)
			{
				ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)propertyDefinition;
				if (providerPropertyDefinition.IsCalculated)
				{
					list.Add(providerPropertyDefinition);
				}
				else
				{
					object obj;
					if (dictionary.TryGetValue(providerPropertyDefinition, out obj))
					{
						MultiValuedPropertyBase multiValuedPropertyBase = obj as MultiValuedPropertyBase;
						if (this.DeserializeMultiValuedProperty(multiValuedPropertyBase, providerPropertyDefinition, this.serializationErrors))
						{
							multiValuedPropertyBase.CollectionChanging += this.MvpChangingHandler;
							multiValuedPropertyBase.CollectionChanged += this.MvpChangedHandler;
						}
						ExTraceGlobals.PropertyBagTracer.TraceDebug<ProviderPropertyDefinition, object>((long)this.GetHashCode(), "Restoring current value for {0}: {1}", providerPropertyDefinition, obj);
						this.currentValues.Add(providerPropertyDefinition, obj);
						dictionary3[providerPropertyDefinition] = obj;
					}
					else
					{
						list2.Add(providerPropertyDefinition);
					}
					object obj2;
					if (!this.IsReadOnly && dictionary2 != null && dictionary2.TryGetValue(providerPropertyDefinition, out obj2))
					{
						this.DeserializeMultiValuedProperty(obj2 as MultiValuedPropertyBase, providerPropertyDefinition, this.serializationErrors);
						ExTraceGlobals.PropertyBagTracer.TraceDebug<ProviderPropertyDefinition, object>((long)this.GetHashCode(), "Restoring original value for {0}: {1}", providerPropertyDefinition, obj2);
						this.OriginalValues.Add(providerPropertyDefinition, obj2);
						if (obj2 is PropertyBag.ChangeMarkerClass)
						{
							dictionary3[providerPropertyDefinition] = providerPropertyDefinition.DefaultValue;
						}
						else
						{
							dictionary3[providerPropertyDefinition] = obj2;
						}
					}
				}
			}
			for (int i = 0; i < list2.Count; i++)
			{
				ProviderPropertyDefinition providerPropertyDefinition2 = list2[i];
				ExTraceGlobals.PropertyBagTracer.TraceDebug<ProviderPropertyDefinition>((long)this.GetHashCode(), "Restoring empty value for {0}", providerPropertyDefinition2);
				if (providerPropertyDefinition2.IsMultivalued)
				{
					this.GetMultiValuedPropertyValue(providerPropertyDefinition2);
				}
				else
				{
					this.currentValues.Add(providerPropertyDefinition2, null);
				}
			}
			HashSet<ProviderPropertyDefinition> hashSet = new HashSet<ProviderPropertyDefinition>();
			for (int j = 0; j < list.Count; j++)
			{
				ProviderPropertyDefinition providerPropertyDefinition3 = list[j];
				ExTraceGlobals.PropertyBagTracer.TraceDebug<ProviderPropertyDefinition>((long)this.GetHashCode(), "Restoring calculated value for {0}", providerPropertyDefinition3);
				try
				{
					if (providerPropertyDefinition3.IsMultivalued)
					{
						hashSet.TryAdd(providerPropertyDefinition3);
						this.GetMultiValuedPropertyValue(providerPropertyDefinition3);
					}
					else
					{
						this.GetCalculatedPropertyValue(providerPropertyDefinition3);
					}
				}
				catch (DataValidationException)
				{
				}
			}
			if (!this.IsReadOnly && this.originalValues != null)
			{
				PropertyBag propertyBag = (PropertyBag)Activator.CreateInstance(base.GetType());
				propertyBag.currentValues = dictionary3;
				List<ProviderPropertyDefinition> list3 = new List<ProviderPropertyDefinition>(this.OriginalValues.Keys);
				foreach (ProviderPropertyDefinition providerPropertyDefinition4 in list3)
				{
					foreach (ProviderPropertyDefinition providerPropertyDefinition5 in providerPropertyDefinition4.DependentProperties)
					{
						if (hashSet.TryAdd(providerPropertyDefinition5) && list.Contains(providerPropertyDefinition5))
						{
							try
							{
								object value = providerPropertyDefinition5.GetterDelegate(propertyBag);
								this.OriginalValues.Add(providerPropertyDefinition5, value);
							}
							catch (DataValidationException)
							{
							}
						}
					}
				}
			}
			if (this.updateErrors != null)
			{
				this.updateErrors(this.serializationErrors.ToArray());
			}
			this.serializationErrors = null;
			this.fullPropertyDefinitions = null;
			this.isDeserializationComplete = true;
		}

		private bool DeserializeMultiValuedProperty(MultiValuedPropertyBase mvp, ProviderPropertyDefinition propDef, List<ValidationError> serializationErrors)
		{
			if (mvp != null)
			{
				mvp.UpdatePropertyDefinition(propDef);
				try
				{
					mvp.FinalizeDeserialization();
				}
				catch (DataValidationException ex)
				{
					this.serializationErrors.Add(ex.Error);
				}
				return true;
			}
			return false;
		}

		private object SerializeDataPrivate(ProviderPropertyDefinition propertyDefinition, object input)
		{
			if (this.NeedsToSuppressPii)
			{
				input = this.InternalSuppressPii(propertyDefinition, input);
			}
			if (SerializationTypeConverter.TypeIsSerializable(propertyDefinition.Type))
			{
				return input;
			}
			object result;
			try
			{
				result = this.SerializeData(propertyDefinition, input);
			}
			catch (FormatException innerException)
			{
				throw new SerializationException(DataStrings.ErrorConversionFailed(propertyDefinition, input), innerException);
			}
			catch (NotImplementedException innerException2)
			{
				throw new SerializationException(DataStrings.ErrorConversionFailed(propertyDefinition, input), innerException2);
			}
			return result;
		}

		internal virtual object SerializeData(ProviderPropertyDefinition propertyDefinition, object input)
		{
			return ValueConvertor.SerializeData(propertyDefinition, input);
		}

		private object DeserializeDataPrivate(ProviderPropertyDefinition propertyDefinition, object input)
		{
			if (SerializationTypeConverter.TypeIsSerializable(propertyDefinition.Type))
			{
				return input;
			}
			object result;
			try
			{
				result = this.DeserializeData(propertyDefinition, input);
			}
			catch (FormatException ex)
			{
				throw new DataValidationException(new PropertyConversionError(DataStrings.ErrorConversionFailed(propertyDefinition, input), propertyDefinition, input, ex), ex);
			}
			return result;
		}

		internal virtual object DeserializeData(ProviderPropertyDefinition propertyDefinition, object input)
		{
			return ValueConvertor.DeserializeData(propertyDefinition, input);
		}

		private void SerializeAndAddValuePair(List<PropertyBag.ValuePair> list, ProviderPropertyDefinition key, object value)
		{
			value = this.SerializeDataPrivate(key, value);
			PropertyBag.ValuePair item;
			item.Key = key;
			item.Value = value;
			ExTraceGlobals.PropertyBagTracer.TraceDebug<ProviderPropertyDefinition, object>((long)this.GetHashCode(), "Adding ValuePair ({0}, {1})", key, value);
			list.Add(item);
		}

		private bool NeedsToSuppressPii
		{
			get
			{
				return SuppressingPiiContext.NeedPiiSuppression;
			}
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			ExTraceGlobals.PropertyBagTracer.TraceDebug<bool, bool>((long)this.GetHashCode(), "PrepareForSerialization(); IsReadOnly = {0}; SaveCalculatedValues = {1}", this.IsReadOnly, this.storeValuesOnly);
			this.serializerAssemblyVersion = ConfigurableObject.ExecutingAssemblyVersion;
			List<PropertyBag.ValuePair> list = new List<PropertyBag.ValuePair>();
			foreach (KeyValuePair<ProviderPropertyDefinition, object> keyValuePair in this.currentValues)
			{
				ProviderPropertyDefinition key = keyValuePair.Key;
				object value = keyValuePair.Value;
				if ((!key.IsCalculated || this.NeedsToSuppressPii) && value != null)
				{
					MultiValuedPropertyBase multiValuedPropertyBase = value as MultiValuedPropertyBase;
					if (multiValuedPropertyBase == null || multiValuedPropertyBase.Changed || multiValuedPropertyBase.Count != 0)
					{
						this.SerializeAndAddValuePair(list, key, value);
					}
				}
			}
			this.serializedCurrentValues = list.ToArray();
			ExTraceGlobals.PropertyBagTracer.TraceDebug((long)this.GetHashCode(), "Serialize Original Values");
			if (!this.IsReadOnly)
			{
				List<PropertyBag.ValuePair> list2 = new List<PropertyBag.ValuePair>();
				if (this.originalValues != null)
				{
					foreach (KeyValuePair<ProviderPropertyDefinition, object> keyValuePair2 in this.OriginalValues)
					{
						if (!keyValuePair2.Key.IsCalculated || this.NeedsToSuppressPii)
						{
							this.SerializeAndAddValuePair(list2, keyValuePair2.Key, keyValuePair2.Value);
						}
					}
				}
				this.serializedOriginalValues = list2.ToArray();
			}
		}

		[OnSerialized]
		private void OnSerialized(StreamingContext context)
		{
			ExTraceGlobals.PropertyBagTracer.TraceDebug((long)this.GetHashCode(), "OnSerialized()");
			this.serializedCurrentValues = null;
			this.serializedOriginalValues = null;
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			ExTraceGlobals.PropertyBagTracer.TraceDebug((long)this.GetHashCode(), "OnDeserializing()");
			this.lockObject = new object();
			this.isDeserializationComplete = false;
			this.recursionDepth = 0;
			this.shouldCallSetters = true;
			this.serializationErrors = new List<ValidationError>();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			ExTraceGlobals.PropertyBagTracer.TraceDebug<bool, bool>((long)this.GetHashCode(), "DeserializeValues(); IsReadOnly = {0}; SaveCalculatedValues = {1}", this.IsReadOnly, this.storeValuesOnly);
			this.currentValues = new Dictionary<ProviderPropertyDefinition, object>(this.serializedCurrentValues.Length);
			this.originalValues = null;
			foreach (PropertyBag.ValuePair valuePair in this.serializedCurrentValues)
			{
				try
				{
					ProviderPropertyDefinition key = valuePair.Key;
					object value = this.DeserializeDataPrivate(key, valuePair.Value);
					this.currentValues.Add(key, value);
				}
				catch (DataValidationException ex)
				{
					this.serializationErrors.Add(ex.Error);
				}
			}
			this.serializedCurrentValues = null;
			if (this.serializedOriginalValues != null)
			{
				foreach (PropertyBag.ValuePair valuePair2 in this.serializedOriginalValues)
				{
					ProviderPropertyDefinition key2 = valuePair2.Key;
					object value2 = this.DeserializeDataPrivate(key2, valuePair2.Value);
					this.OriginalValues[key2] = value2;
				}
				this.serializedOriginalValues = null;
			}
			this.FinalizeDeserialization();
		}

		private bool InternalIsMarshalByRefObject(object value)
		{
			return typeof(MarshalByRefObject).IsInstanceOfType(value);
		}

		private object InternalSuppressPii(ProviderPropertyDefinition propertyDefinition, object input)
		{
			return SuppressingPiiProperty.TryRedact(propertyDefinition, input, SuppressingPiiContext.PiiMap);
		}

		public const int DefaultSize = 16;

		private static object[] EmptyArray = new object[0];

		private static readonly object ChangeMarker = PropertyBag.ChangeMarkerClass.Instance;

		[NonSerialized]
		private Dictionary<ProviderPropertyDefinition, object> currentValues;

		[NonSerialized]
		private object lockObject = new object();

		[NonSerialized]
		private Dictionary<ProviderPropertyDefinition, object> originalValues;

		[NonSerialized]
		private int recursionDepth;

		[NonSerialized]
		private bool shouldCallSetters = true;

		private bool readOnly;

		private bool storeValuesOnly;

		[NonSerialized]
		private bool isDeserializationComplete;

		[NonSerialized]
		private IEnumerable<PropertyDefinition> fullPropertyDefinitions;

		[NonSerialized]
		private EventHandler mvpChangingHandler;

		[NonSerialized]
		private EventHandler mvpChangedHandler;

		[NonSerialized]
		private UpdateErrorsDelegate updateErrors;

		[NonSerialized]
		private List<ValidationError> serializationErrors;

		private Version serializerAssemblyVersion;

		private PropertyBag.ValuePair[] serializedCurrentValues;

		private PropertyBag.ValuePair[] serializedOriginalValues;

		[Serializable]
		private struct ValuePair
		{
			public ProviderPropertyDefinition Key;

			public object Value;
		}

		[ImmutableObject(true)]
		[Serializable]
		private class ChangeMarkerClass : ICloneable, ISerializable
		{
			private ChangeMarkerClass()
			{
			}

			public object Clone()
			{
				return this;
			}

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.SetType(typeof(PropertyBag.ChangeMarkerClass.ChangeMarkerSerializationHelper));
			}

			public static readonly PropertyBag.ChangeMarkerClass Instance = new PropertyBag.ChangeMarkerClass();

			[Serializable]
			private class ChangeMarkerSerializationHelper : IObjectReference
			{
				public object GetRealObject(StreamingContext context)
				{
					return PropertyBag.ChangeMarkerClass.Instance;
				}
			}
		}

		private class OriginalPropertyBag : IPropertyBag, IReadOnlyPropertyBag
		{
			public OriginalPropertyBag(PropertyBag internalBag)
			{
				if (internalBag == null)
				{
					throw new ArgumentNullException("internalBag");
				}
				this.internalBag = internalBag;
			}

			public object this[PropertyDefinition propertyDefinition]
			{
				get
				{
					ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)propertyDefinition;
					object result;
					if (this.internalBag.TryGetOriginalValue(providerPropertyDefinition, out result))
					{
						return result;
					}
					return providerPropertyDefinition.DefaultValue;
				}
				set
				{
					throw new InvalidOperationException("Dev Error: Code is trying to modify the PropertyBag on a code path that should only be reading values.");
				}
			}

			public void SetProperties(ICollection<PropertyDefinition> propertyDefinitionArray, object[] propertyValuesArray)
			{
				throw new InvalidOperationException("Dev Error: Code is trying to modify the PropertyBag on a code path that should only be reading values.");
			}

			public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
			{
				if (propertyDefinitionArray == null)
				{
					throw new ArgumentNullException("propertyDefinitions");
				}
				object[] array = new object[propertyDefinitionArray.Count];
				int num = 0;
				foreach (PropertyDefinition propertyDefinition in propertyDefinitionArray)
				{
					array[num++] = this[propertyDefinition];
				}
				return array;
			}

			private PropertyBag internalBag;
		}
	}
}
