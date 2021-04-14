using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[TypeConverter(typeof(SimpleGenericsTypeConverter))]
	[Serializable]
	public class MultiValuedProperty<T> : MultiValuedPropertyBase, IList, ICollection, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IEquatable<MultiValuedProperty<T>>
	{
		public static MultiValuedProperty<T> Empty
		{
			get
			{
				return MultiValuedProperty<T>.empty;
			}
		}

		private LocalizedString ReadOnlyErrorMessage
		{
			get
			{
				LocalizedString? localizedString = this.readOnlyErrorMessage;
				if (localizedString == null)
				{
					return DataStrings.ExceptionReadOnlyMultiValuedProperty;
				}
				return localizedString.GetValueOrDefault();
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		public override bool Changed
		{
			get
			{
				return this.changed && (this.wasCleared || this.added.Count > 0 || this.removed.Count > 0);
			}
		}

		public override bool IsChangesOnlyCopy
		{
			get
			{
				return this.isChangesOnlyCopy;
			}
		}

		internal override bool WasCleared
		{
			get
			{
				return this.wasCleared;
			}
		}

		internal override object[] Added
		{
			get
			{
				if (this.HasChangeTracking)
				{
					return this.added.ToArray();
				}
				return new object[0];
			}
		}

		internal override object[] Removed
		{
			get
			{
				if (this.HasChangeTracking)
				{
					return this.removed.ToArray();
				}
				return new object[0];
			}
		}

		internal override ProviderPropertyDefinition PropertyDefinition
		{
			get
			{
				return this.propertyDefinition;
			}
		}

		private bool HasChangeTracking
		{
			get
			{
				return !this.IsReadOnly || this.IsChangesOnlyCopy;
			}
		}

		private MultiValuedProperty(bool readOnly, bool validate, ProviderPropertyDefinition propertyDefinition, IEnumerable values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			this.propertyDefinition = propertyDefinition;
			this.isReadOnly = readOnly;
			this.readOnlyErrorMessage = readOnlyErrorMessage;
			this.changed = false;
			this.wasCleared = false;
			this.propertyValues = new List<T>();
			if (this.HasChangeTracking)
			{
				this.added = new List<object>();
				this.removed = new List<object>();
			}
			foreach (object obj in values)
			{
				if (obj != null)
				{
					T item = this.ConvertInput(obj);
					if (validate)
					{
						this.ValidateValueAndThrow(item);
						if (this.Contains(item))
						{
							throw new InvalidOperationException(DataStrings.ErrorValueAlreadyPresent(obj.ToString()));
						}
					}
					this.propertyValues.Add(item);
				}
			}
			this.propertyValues.TrimExcess();
			if (invalidValues != null && invalidValues.Count > 0 && !readOnly)
			{
				foreach (object item2 in invalidValues)
				{
					this.removed.Add(item2);
				}
			}
		}

		internal MultiValuedProperty(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage) : this(readOnly, false, propertyDefinition, values, invalidValues, readOnlyErrorMessage)
		{
		}

		internal MultiValuedProperty(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values) : this(readOnly, true, propertyDefinition, values, null, null)
		{
		}

		public MultiValuedProperty(object value) : this(true, true, null, MultiValuedProperty<T>.GetObjectAsEnumerable(value), null, null)
		{
		}

		private static IEnumerable GetObjectAsEnumerable(object value)
		{
			if (value is IEnumerable<!0> || value is ICollection)
			{
				return (IEnumerable)value;
			}
			return new object[]
			{
				value
			};
		}

		public MultiValuedProperty() : this(false, false, null, new T[0], null, null)
		{
		}

		public MultiValuedProperty(Dictionary<string, object> table) : this(false, true, null, new T[0], null, null)
		{
			this.isChangesOnlyCopy = true;
			bool copyChangesOnly = base.CopyChangesOnly;
			base.CopyChangesOnly = true;
			foreach (string text in table.Keys)
			{
				object values = ValueConvertor.UnwrapPSObjectIfNeeded(table[text]);
				if (!this.TryAddValues(text, values) && !this.TryRemoveValues(text, values))
				{
					throw new NotSupportedException(DataStrings.ErrorUnknownOperation(text, MultiValuedProperty<T>.ArrayToString(MultiValuedProperty<T>.AddKeys), MultiValuedProperty<T>.ArrayToString(MultiValuedProperty<T>.RemoveKeys)));
				}
			}
			base.CopyChangesOnly = copyChangesOnly;
			this.isReadOnly = true;
		}

		private static string ArrayToString(string[] array)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append("'");
				stringBuilder.Append(array[i]);
				stringBuilder.Append("'");
				if (i != array.Length - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			return stringBuilder.ToString();
		}

		private bool TryAddValues(string key, object values)
		{
			foreach (string b in MultiValuedProperty<T>.AddKeys)
			{
				if (string.Equals(key, b, StringComparison.OrdinalIgnoreCase))
				{
					if (values is ICollection || values is IEnumerable<!0>)
					{
						using (IEnumerator enumerator = ((IEnumerable)values).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								object item = enumerator.Current;
								this.Add(item);
							}
							goto IL_6F;
						}
					}
					this.Add(values);
					IL_6F:
					return true;
				}
			}
			return false;
		}

		private bool TryRemoveValues(string key, object values)
		{
			foreach (string b in MultiValuedProperty<T>.RemoveKeys)
			{
				if (string.Equals(key, b, StringComparison.OrdinalIgnoreCase))
				{
					if (values is ICollection || values is IEnumerable<!0>)
					{
						using (IEnumerator enumerator = ((IEnumerable)values).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								object item = enumerator.Current;
								this.Remove(item);
							}
							goto IL_71;
						}
					}
					this.Remove(values);
					IL_71:
					return true;
				}
			}
			return false;
		}

		protected virtual T ConvertInput(object item)
		{
			return (T)((object)ValueConvertor.ConvertValue(item, typeof(T), null));
		}

		internal override void Add(object item)
		{
			this.Add(this.ConvertInput(item));
		}

		internal override bool Remove(object item)
		{
			return this.Remove(this.ConvertInput(item));
		}

		private static bool Contains(IList list, T item, StringComparison comparisonType)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				object obj = list[i];
				if (OpathFilterEvaluator.Equals(obj, item, comparisonType))
				{
					return true;
				}
			}
			return false;
		}

		private static int IndexOf(IList list, object item, StringComparison comparisonType)
		{
			if ((item is string || item is ProxyAddressBase) && typeof(T).GetTypeInfo().IsAssignableFrom(item.GetType().GetTypeInfo()))
			{
				if (item is string)
				{
					string a = item.ToString();
					for (int i = 0; i < list.Count; i++)
					{
						string b = list[i].ToString();
						if (string.Equals(a, b, comparisonType))
						{
							return i;
						}
					}
				}
				else if (item is ProxyAddressBase)
				{
					ProxyAddressBase a2 = (ProxyAddressBase)item;
					for (int j = 0; j < list.Count; j++)
					{
						ProxyAddressBase b2 = (ProxyAddressBase)list[j];
						if (ProxyAddressBase.Equals(a2, b2, comparisonType))
						{
							return j;
						}
					}
				}
				return -1;
			}
			return list.IndexOf(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public MultiValuedProperty<T>.Enumerator GetEnumerator()
		{
			return new MultiValuedProperty<T>.Enumerator(this.propertyValues);
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public override bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public override object SyncRoot
		{
			get
			{
				return ((ICollection)this.propertyValues).SyncRoot;
			}
		}

		public override void CopyTo(Array array, int index)
		{
			if (this.IsChangesOnlyCopy)
			{
				throw new InvalidOperationException(DataStrings.ErrorNotSupportedForChangesOnlyCopy);
			}
			Array.Copy(this.propertyValues.ToArray(), 0, array, index, this.Count);
		}

		public void Add(T item)
		{
			Exception ex;
			if (!this.TryAddInternal(item, out ex) && ex != null)
			{
				throw ex;
			}
		}

		public bool TryAdd(T item)
		{
			Exception ex;
			return this.TryAddInternal(item, out ex);
		}

		protected virtual bool TryAddInternal(T item, out Exception error)
		{
			error = null;
			if (this.IsReadOnly)
			{
				error = new InvalidOperationException(this.ReadOnlyErrorMessage.ToString());
				return false;
			}
			if (item == null)
			{
				error = new ArgumentNullException("item", DataStrings.ErrorCannotAddNullValue);
				return false;
			}
			if (this.Contains(item))
			{
				if (!base.CopyChangesOnly)
				{
					error = new InvalidOperationException(DataStrings.ErrorValueAlreadyPresent(item.ToString()));
				}
				return false;
			}
			ValidationError validationError = this.ValidateValue(item);
			if (validationError != null)
			{
				error = new DataValidationException(validationError);
				return false;
			}
			base.BeginUpdate();
			bool result;
			try
			{
				this.changed = true;
				if (!this.IsChangesOnlyCopy)
				{
					this.propertyValues.Add(item);
				}
				if (MultiValuedProperty<T>.Contains(this.removed, item, StringComparison.Ordinal))
				{
					this.removed.Remove(item);
				}
				else
				{
					this.added.Add(item);
				}
				result = true;
			}
			catch (Exception)
			{
				this.errorOnUpdate = true;
				throw;
			}
			finally
			{
				base.EndUpdate();
			}
			return result;
		}

		protected override void OnCollectionChanging(EventArgs e)
		{
			if (this.HasChangeTracking && this.propertyDefinition != null && this.propertyDefinition.AllCollectionConstraints != null)
			{
				this.backupCopy = (MultiValuedProperty<T>)base.MemberwiseClone();
				this.backupCopy.propertyValues = new List<T>(this.propertyValues);
				this.backupCopy.added = new List<object>(this.added);
				this.backupCopy.removed = new List<object>(this.removed);
			}
			this.errorOnUpdate = false;
			base.OnCollectionChanging(e);
		}

		protected override void OnCollectionChanged(EventArgs e)
		{
			if (!this.errorOnUpdate)
			{
				try
				{
					this.ValidateCollection();
				}
				catch (DataValidationException)
				{
					if (this.backupCopy != null)
					{
						this.changed = this.backupCopy.changed;
						this.wasCleared = this.backupCopy.wasCleared;
						this.propertyValues = this.backupCopy.propertyValues;
						this.added = this.backupCopy.added;
						this.removed = this.backupCopy.removed;
					}
					throw;
				}
			}
			this.backupCopy = null;
			base.OnCollectionChanged(e);
		}

		private void ValidateValueAndThrow(T item)
		{
			ValidationError validationError = this.ValidateValue(item);
			if (validationError != null)
			{
				throw new DataValidationException(validationError);
			}
		}

		protected virtual ValidationError ValidateValue(T item)
		{
			if (this.propertyDefinition != null)
			{
				return this.propertyDefinition.ValidateValue(item, false);
			}
			return null;
		}

		private void ValidateCollection()
		{
			if (this.propertyDefinition != null)
			{
				ValidationError validationError = this.propertyDefinition.ValidateCollection(this, false);
				if (validationError != null)
				{
					throw new DataValidationException(validationError);
				}
			}
		}

		public override void Clear()
		{
			if (this.IsReadOnly)
			{
				throw new InvalidOperationException(this.ReadOnlyErrorMessage.ToString());
			}
			base.BeginUpdate();
			try
			{
				this.propertyValues.Clear();
				this.changed = true;
				this.added.Clear();
				this.removed.Clear();
				this.wasCleared = true;
			}
			catch (Exception)
			{
				this.errorOnUpdate = true;
				throw;
			}
			finally
			{
				base.EndUpdate();
			}
		}

		public T Find(Predicate<T> match)
		{
			if (this.IsChangesOnlyCopy)
			{
				throw new InvalidOperationException(DataStrings.ErrorNotSupportedForChangesOnlyCopy);
			}
			return this.propertyValues.Find(match);
		}

		public bool Contains(T item)
		{
			return MultiValuedProperty<T>.Contains(this.propertyValues, item, StringComparison.OrdinalIgnoreCase);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.CopyTo(array, arrayIndex);
		}

		internal override void SetIsReadOnly(bool isReadOnly, LocalizedString? readOnlyErrorMessage)
		{
			this.readOnlyErrorMessage = readOnlyErrorMessage;
			if (this.IsReadOnly == isReadOnly)
			{
				return;
			}
			bool hasChangeTracking = this.HasChangeTracking;
			this.isReadOnly = isReadOnly;
			if (this.HasChangeTracking != hasChangeTracking)
			{
				this.added = (this.HasChangeTracking ? new List<object>() : null);
				this.removed = (this.HasChangeTracking ? new List<object>() : null);
			}
		}

		internal override void UpdateValues(MultiValuedPropertyBase newMvp)
		{
			if (newMvp != null && newMvp.IsChangesOnlyCopy)
			{
				throw new InvalidOperationException(DataStrings.ErrorNotSupportedForChangesOnlyCopy);
			}
			base.BeginUpdate();
			try
			{
				this.propertyValues.Clear();
				if (!this.IsReadOnly)
				{
					this.added.Clear();
					this.removed.Clear();
				}
				this.changed = true;
				this.wasCleared = true;
				if (newMvp != null)
				{
					foreach (object obj in ((IEnumerable)newMvp))
					{
						T t = (T)((object)obj);
						this.propertyValues.Add(t);
						if (!this.IsReadOnly)
						{
							this.added.Add(t);
						}
					}
				}
			}
			finally
			{
				base.EndUpdate();
			}
		}

		public override void CopyChangesFrom(MultiValuedPropertyBase changedMvp)
		{
			if (this.IsReadOnly)
			{
				throw new InvalidObjectOperationException(this.ReadOnlyErrorMessage);
			}
			if (changedMvp == null)
			{
				throw new ArgumentNullException("changedMvp");
			}
			if (!changedMvp.Changed)
			{
				return;
			}
			bool copyChangesOnly = base.CopyChangesOnly;
			base.BeginUpdate();
			try
			{
				base.CopyChangesOnly = true;
				if (changedMvp.WasCleared)
				{
					this.Clear();
				}
				foreach (object item in changedMvp.Removed)
				{
					this.Remove(item);
				}
				foreach (object item2 in changedMvp.Added)
				{
					this.Add(item2);
				}
			}
			catch (Exception)
			{
				this.errorOnUpdate = true;
				throw;
			}
			finally
			{
				base.CopyChangesOnly = copyChangesOnly;
				base.EndUpdate();
			}
		}

		public override int Count
		{
			get
			{
				return this.propertyValues.Count;
			}
		}

		public virtual bool Remove(T item)
		{
			if (this.IsReadOnly)
			{
				throw new InvalidOperationException(this.ReadOnlyErrorMessage.ToString());
			}
			if (item == null)
			{
				return false;
			}
			if (!this.IsChangesOnlyCopy)
			{
				int num = this.IndexOf(item);
				if (-1 == num)
				{
					return false;
				}
				item = this.propertyValues[num];
			}
			base.BeginUpdate();
			try
			{
				this.changed = true;
				this.propertyValues.Remove(item);
				if (MultiValuedProperty<T>.Contains(this.added, item, StringComparison.Ordinal))
				{
					this.added.Remove(item);
				}
				else
				{
					this.removed.Add(item);
				}
			}
			catch (Exception)
			{
				this.errorOnUpdate = true;
				throw;
			}
			finally
			{
				base.EndUpdate();
			}
			return true;
		}

		bool IList.IsFixedSize
		{
			get
			{
				return this.IsReadOnly;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = this.ConvertInput(value);
			}
		}

		int IList.Add(object value)
		{
			T t = this.ConvertInput(value);
			this.Add(t);
			return this.IndexOf(t);
		}

		bool IList.Contains(object value)
		{
			return this.Contains(this.ConvertInput(value));
		}

		int IList.IndexOf(object value)
		{
			T value2 = default(T);
			try
			{
				value2 = this.ConvertInput(value);
			}
			catch (InvalidCastException)
			{
				return -1;
			}
			return this.IndexOf(value2);
		}

		void IList.Insert(int index, object value)
		{
			this.Insert(index, this.ConvertInput(value));
		}

		void IList.Remove(object value)
		{
			this.Remove(this.ConvertInput(value));
		}

		public T this[int index]
		{
			get
			{
				return this.propertyValues[index];
			}
			set
			{
				this.SetAt(index, value);
			}
		}

		public int IndexOf(T value)
		{
			return MultiValuedProperty<T>.IndexOf(this.propertyValues, value, StringComparison.OrdinalIgnoreCase);
		}

		public virtual void Insert(int index, T value)
		{
			if (index < 0 || index > this.propertyValues.Count)
			{
				throw new ArgumentOutOfRangeException("index", index, DataStrings.ErrorOutOfRange(0, this.propertyValues.Count));
			}
			this.Add(value);
			T t = this[index];
			if (!t.Equals(value))
			{
				int index2 = this.IndexOf(value);
				T item = this.propertyValues[index2];
				this.propertyValues.RemoveAt(index2);
				this.propertyValues.Insert(index, item);
			}
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index > this.propertyValues.Count)
			{
				throw new ArgumentOutOfRangeException("index", index, DataStrings.ErrorOutOfRange(0, this.propertyValues.Count));
			}
			this.Remove(this[index]);
		}

		public override object Clone()
		{
			MultiValuedProperty<T> multiValuedProperty = (MultiValuedProperty<T>)CloneHelper.SerializeObj(this);
			multiValuedProperty.propertyDefinition = this.propertyDefinition;
			return multiValuedProperty;
		}

		protected virtual void SetAt(int index, T item)
		{
			if (this.IsReadOnly)
			{
				throw new InvalidOperationException(this.ReadOnlyErrorMessage.ToString());
			}
			if (item == null)
			{
				throw new ArgumentNullException("item", DataStrings.ErrorCannotAddNullValue);
			}
			if (index < 0 || index >= this.propertyValues.Count)
			{
				throw new ArgumentOutOfRangeException("index", index, DataStrings.ErrorOutOfRange(0, this.propertyValues.Count - 1));
			}
			if (MultiValuedProperty<T>.Contains(this.propertyValues, item, StringComparison.Ordinal))
			{
				T t = this[index];
				if (t.Equals(item))
				{
					return;
				}
			}
			int num = this.IndexOf(item);
			if (num >= 0 && num != index)
			{
				throw new InvalidOperationException(DataStrings.ErrorValueAlreadyPresent(item.ToString()));
			}
			this.ValidateValueAndThrow(item);
			base.BeginUpdate();
			try
			{
				this.RemoveAt(index);
				this.Insert(index, item);
			}
			catch (Exception)
			{
				this.errorOnUpdate = true;
				throw;
			}
			finally
			{
				base.EndUpdate();
			}
		}

		public void Sort()
		{
			this.propertyValues.Sort();
		}

		public static implicit operator MultiValuedProperty<T>(object[] array)
		{
			if (array == null)
			{
				return null;
			}
			return new MultiValuedProperty<T>(true, null, array);
		}

		public T[] ToArray()
		{
			if (this.IsChangesOnlyCopy)
			{
				throw new InvalidOperationException(DataStrings.ErrorNotSupportedForChangesOnlyCopy);
			}
			return this.propertyValues.ToArray();
		}

		internal override void ResetChangeTracking()
		{
			if (this.HasChangeTracking)
			{
				this.added.Clear();
				this.removed.Clear();
			}
			this.wasCleared = false;
			this.changed = false;
		}

		internal override void MarkAsChanged()
		{
			if (this.IsReadOnly)
			{
				throw new InvalidOperationException(this.ReadOnlyErrorMessage.ToString());
			}
			if (this.wasCleared || this.added.Count > 0 || this.removed.Count > 0)
			{
				base.BeginUpdate();
				this.changed = true;
				base.EndUpdate();
			}
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			this.serializerAssemblyVersion = ConfigurableObject.ExecutingAssemblyVersion;
			this.serializedPropertyValues = new object[this.propertyValues.Count];
			for (int i = 0; i < this.propertyValues.Count; i++)
			{
				this.serializedPropertyValues[i] = this.SerializeValuePrivate(this.propertyValues[i]);
			}
			if (this.HasChangeTracking)
			{
				if (this.added.Count > 0)
				{
					this.serializedAddedValues = new object[this.added.Count];
					for (int j = 0; j < this.added.Count; j++)
					{
						this.serializedAddedValues[j] = this.SerializeValuePrivate(this.added[j]);
					}
				}
				if (this.removed.Count > 0)
				{
					this.serializedRemovedValues = new object[this.removed.Count];
					for (int k = 0; k < this.removed.Count; k++)
					{
						this.serializedRemovedValues[k] = this.SerializeValuePrivate(this.removed[k]);
					}
				}
			}
		}

		[OnSerialized]
		private void OnSerialized(StreamingContext context)
		{
			this.serializedPropertyValues = null;
			this.serializedAddedValues = null;
			this.serializedRemovedValues = null;
		}

		internal override void FinalizeDeserialization()
		{
			if (this.isDeserializationComplete)
			{
				return;
			}
			if (this.propertyValues != null)
			{
				throw new InvalidOperationException("Cannot deserialize values when MVP has already been populated.");
			}
			if (this.serializedPropertyValues == null)
			{
				throw new InvalidOperationException("Cannot deserialize values when no serialization information is available.");
			}
			this.propertyValues = new List<T>(this.serializedPropertyValues.Length);
			for (int i = 0; i < this.serializedPropertyValues.Length; i++)
			{
				this.propertyValues.Add((T)((object)this.DeserializeValuePrivate(this.serializedPropertyValues[i])));
			}
			if (this.HasChangeTracking)
			{
				if (this.serializedAddedValues != null)
				{
					this.added = new List<object>(this.serializedAddedValues.Length);
					for (int j = 0; j < this.serializedAddedValues.Length; j++)
					{
						this.added.Add(this.DeserializeValuePrivate(this.serializedAddedValues[j]));
					}
				}
				else
				{
					this.added = new List<object>();
				}
				if (this.serializedRemovedValues != null)
				{
					this.removed = new List<object>(this.serializedRemovedValues.Length);
					for (int k = 0; k < this.serializedRemovedValues.Length; k++)
					{
						this.removed.Add(this.DeserializeValuePrivate(this.serializedRemovedValues[k]));
					}
				}
				else
				{
					this.removed = new List<object>();
				}
			}
			this.serializedPropertyValues = null;
			this.serializedAddedValues = null;
			this.serializedRemovedValues = null;
			this.isDeserializationComplete = true;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			this.FinalizeDeserialization();
		}

		private object SerializeValuePrivate(object value)
		{
			if (this.PropertyDefinition == null)
			{
				return value;
			}
			if (SerializationTypeConverter.TypeIsSerializable(this.PropertyDefinition.Type))
			{
				return value;
			}
			object result;
			try
			{
				result = this.SerializeValue(value);
			}
			catch (FormatException innerException)
			{
				throw new SerializationException(DataStrings.ErrorConversionFailed(this.PropertyDefinition, value), innerException);
			}
			catch (NotImplementedException innerException2)
			{
				throw new SerializationException(DataStrings.ErrorConversionFailed(this.PropertyDefinition, value), innerException2);
			}
			return result;
		}

		protected virtual object SerializeValue(object value)
		{
			return ValueConvertor.SerializeData(this.PropertyDefinition, value);
		}

		private object DeserializeValuePrivate(object value)
		{
			if (this.PropertyDefinition == null)
			{
				return value;
			}
			if (SerializationTypeConverter.TypeIsSerializable(this.PropertyDefinition.Type))
			{
				return value;
			}
			object result;
			try
			{
				result = this.DeserializeValue(value);
			}
			catch (FormatException ex)
			{
				throw new DataValidationException(new PropertyConversionError(DataStrings.ErrorConversionFailed(this.PropertyDefinition, value), this.PropertyDefinition, value, ex), ex);
			}
			return result;
		}

		protected virtual object DeserializeValue(object value)
		{
			return ValueConvertor.DeserializeData(this.PropertyDefinition, value);
		}

		internal override void UpdatePropertyDefinition(ProviderPropertyDefinition newPropertyDefinition)
		{
			if (newPropertyDefinition == null)
			{
				throw new ArgumentNullException("newPropertyDefinition");
			}
			this.propertyDefinition = newPropertyDefinition;
		}

		private static bool ListsAreEqual(IList listA, IList listB)
		{
			if (object.ReferenceEquals(listA, listB))
			{
				return true;
			}
			if (listA == null || listB == null)
			{
				return false;
			}
			if (listA.Count != listB.Count)
			{
				return false;
			}
			foreach (object value in listA)
			{
				if (!listB.Contains(value))
				{
					return false;
				}
			}
			return true;
		}

		public bool Equals(MultiValuedProperty<T> other)
		{
			return other != null && (object.ReferenceEquals(other, this) || (other.Count == this.Count && other.WasCleared == this.WasCleared && other.IsReadOnly == this.IsReadOnly && other.IsCompletelyRead == base.IsCompletelyRead && other.Changed == this.Changed && object.Equals(other.GetType(), base.GetType()) && object.Equals(other.PropertyDefinition, this.PropertyDefinition) && (MultiValuedProperty<T>.ListsAreEqual(other.added, this.added) && MultiValuedProperty<T>.ListsAreEqual(other.removed, this.removed) && MultiValuedProperty<T>.ListsAreEqual(other.propertyValues, this.propertyValues) && MultiValuedProperty<T>.ListsAreEqual(other.serializedAddedValues, this.serializedAddedValues) && MultiValuedProperty<T>.ListsAreEqual(other.serializedRemovedValues, this.serializedRemovedValues) && MultiValuedProperty<T>.ListsAreEqual(other.serializedPropertyValues, this.serializedPropertyValues))));
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as MultiValuedProperty<T>);
		}

		public override int GetHashCode()
		{
			int num = (this.PropertyDefinition == null) ? 0 : this.PropertyDefinition.GetHashCode();
			return num ^ base.GetType().GetHashCode() ^ this.Count;
		}

		public MultiValuedProperty(Hashtable table) : this(false, true, null, new T[0], null, null)
		{
			this.isChangesOnlyCopy = true;
			bool copyChangesOnly = base.CopyChangesOnly;
			base.CopyChangesOnly = true;
			foreach (object obj in table.Keys)
			{
				string text = (string)obj;
				object values = ValueConvertor.UnwrapPSObjectIfNeeded(table[text]);
				if (!this.TryAddValues(text, values) && !this.TryRemoveValues(text, values))
				{
					throw new NotSupportedException(DataStrings.ErrorUnknownOperation(text, MultiValuedProperty<T>.ArrayToString(MultiValuedProperty<T>.AddKeys), MultiValuedProperty<T>.ArrayToString(MultiValuedProperty<T>.RemoveKeys)));
				}
			}
			base.CopyChangesOnly = copyChangesOnly;
			this.isReadOnly = true;
		}

		public static MultiValuedProperty<T>operator +(MultiValuedProperty<T> oldCollection, object newValue)
		{
			if (oldCollection.IsChangesOnlyCopy)
			{
				throw new InvalidOperationException(DataStrings.ErrorNotSupportedForChangesOnlyCopy);
			}
			object[] args = new object[]
			{
				oldCollection.IsReadOnly,
				oldCollection.PropertyDefinition,
				oldCollection
			};
			MultiValuedProperty<T> multiValuedProperty = (MultiValuedProperty<T>)Activator.CreateInstance(oldCollection.GetType(), BindingFlags.Instance | BindingFlags.NonPublic, null, args, null);
			multiValuedProperty.Add(newValue);
			return multiValuedProperty;
		}

		public static MultiValuedProperty<T>operator -(MultiValuedProperty<T> oldCollection, object newValue)
		{
			if (oldCollection.IsChangesOnlyCopy)
			{
				throw new InvalidOperationException(DataStrings.ErrorNotSupportedForChangesOnlyCopy);
			}
			object[] args = new object[]
			{
				oldCollection.IsReadOnly,
				oldCollection.PropertyDefinition,
				oldCollection
			};
			MultiValuedProperty<T> multiValuedProperty = (MultiValuedProperty<T>)Activator.CreateInstance(oldCollection.GetType(), BindingFlags.Instance | BindingFlags.NonPublic, null, args, null);
			multiValuedProperty.Remove(newValue);
			return multiValuedProperty;
		}

		internal const string AddName = "Add";

		internal const string RemoveName = "Remove";

		private static MultiValuedProperty<T> empty = new MultiValuedProperty<T>(true, null, new object[0]);

		[NonSerialized]
		private MultiValuedProperty<T> backupCopy;

		[NonSerialized]
		private bool errorOnUpdate;

		[NonSerialized]
		private List<T> propertyValues;

		[NonSerialized]
		private List<object> added;

		[NonSerialized]
		private List<object> removed;

		[NonSerialized]
		private bool isDeserializationComplete;

		private LocalizedString? readOnlyErrorMessage;

		private bool isReadOnly;

		private bool isChangesOnlyCopy;

		private bool wasCleared;

		private bool changed;

		private ProviderPropertyDefinition propertyDefinition;

		private Version serializerAssemblyVersion;

		private object[] serializedPropertyValues;

		private object[] serializedAddedValues;

		private object[] serializedRemovedValues;

		internal static readonly string[] AddKeys = new string[]
		{
			"Add",
			"+"
		};

		internal static readonly string[] RemoveKeys = new string[]
		{
			"Remove",
			"-"
		};

		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			internal Enumerator(IList<T> list)
			{
				this.list = list;
				this.currentIndex = -1;
				this.currentItem = default(T);
				this.count = list.Count;
			}

			public T Current
			{
				get
				{
					return this.currentItem;
				}
			}

			public void Dispose()
			{
			}

			object IEnumerator.Current
			{
				get
				{
					return this.currentItem;
				}
			}

			public bool MoveNext()
			{
				if (++this.currentIndex < this.count)
				{
					this.currentItem = this.list[this.currentIndex];
					return true;
				}
				return false;
			}

			public void Reset()
			{
				this.currentIndex = -1;
				this.currentItem = default(T);
			}

			private IList<T> list;

			private int currentIndex;

			private T currentItem;

			private int count;
		}
	}
}
