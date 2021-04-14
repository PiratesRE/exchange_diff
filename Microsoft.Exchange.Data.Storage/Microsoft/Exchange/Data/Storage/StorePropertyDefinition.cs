using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class StorePropertyDefinition : PropertyDefinition, IComparable<StorePropertyDefinition>, IComparable
	{
		protected internal StorePropertyDefinition(PropertyTypeSpecifier propertyTypeSpecifier, string displayName, Type type, PropertyFlags childFlags, PropertyDefinitionConstraint[] constraints) : base(displayName, type)
		{
			if (constraints == null)
			{
				throw new ArgumentNullException("constraints");
			}
			this.propertyTypeSpecifier = propertyTypeSpecifier;
			this.childFlags = childFlags;
			if (constraints.Length != 0)
			{
				this.constraints = new ReadOnlyCollection<PropertyDefinitionConstraint>(constraints);
				return;
			}
			this.constraints = StorePropertyDefinition.EmptyConstraints;
		}

		public static void PerformActionOnNativePropertyDefinitions<T>(PropertyDependencyType targetDependencyType, ICollection<T> propertyDefinitions, Action<NativeStorePropertyDefinition> action) where T : PropertyDefinition
		{
			EnumValidator.AssertValid<PropertyDependencyType>(targetDependencyType);
			if (propertyDefinitions == null)
			{
				return;
			}
			int actualDependencyCount = 0;
			foreach (T t in propertyDefinitions)
			{
				PropertyDefinition propertyDefinition = t;
				StorePropertyDefinition storePropertyDefinition = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
				storePropertyDefinition.ForEachMatch(targetDependencyType, delegate(NativeStorePropertyDefinition item)
				{
					action(item);
					actualDependencyCount++;
				});
			}
			int num = (propertyDefinitions.Count >= StorePropertyDefinition.dependencyEstimates.Length) ? propertyDefinitions.Count : StorePropertyDefinition.dependencyEstimates[propertyDefinitions.Count];
			if (actualDependencyCount != num && propertyDefinitions.Count < StorePropertyDefinition.dependencyEstimates.Length)
			{
				Interlocked.Exchange(ref StorePropertyDefinition.dependencyEstimates[propertyDefinitions.Count], actualDependencyCount);
			}
		}

		public static IList<NativeStorePropertyDefinition> GetNativePropertyDefinitions<T>(PropertyDependencyType targetDependencyType, ICollection<T> propertyDefinitions, Predicate<NativeStorePropertyDefinition> addToCollection) where T : PropertyDefinition
		{
			return (IList<NativeStorePropertyDefinition>)StorePropertyDefinition.GetNativePropertyDefinitions<T>(targetDependencyType, propertyDefinitions, false, addToCollection);
		}

		public static ICollection<NativeStorePropertyDefinition> GetNativePropertyDefinitions<T>(PropertyDependencyType targetDependencyType, ICollection<T> propertyDefinitions) where T : PropertyDefinition
		{
			return StorePropertyDefinition.GetNativePropertyDefinitions<T>(targetDependencyType, propertyDefinitions, true, null);
		}

		public abstract StorePropertyCapabilities Capabilities { get; }

		public int CompareTo(StorePropertyDefinition other)
		{
			if (other == null)
			{
				throw new ArgumentException(ServerStrings.ObjectMustBeOfType(base.GetType().Name));
			}
			int num = string.Compare(this.GetHashString(), other.GetHashString(), StringComparison.OrdinalIgnoreCase);
			if (num != 0)
			{
				return num;
			}
			return string.Compare(base.Type.ToString(), other.Type.ToString(), StringComparison.OrdinalIgnoreCase);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			StorePropertyDefinition other = obj as StorePropertyDefinition;
			return this.CompareTo(other);
		}

		public PropertyFlags PropertyFlags
		{
			get
			{
				if (this.propertyFlags == PropertyFlags.None)
				{
					this.CalculatePropertyFlagsFromType(this.childFlags);
				}
				return this.propertyFlags;
			}
		}

		public PropertyTypeSpecifier SpecifiedWith
		{
			get
			{
				return this.propertyTypeSpecifier;
			}
		}

		public override string ToString()
		{
			this.InitializePropertyDefinitionString();
			return string.Format("[{1}] {0}", base.Name, this.propertyDefinitionString);
		}

		public IList<PropertyDefinitionConstraint> Constraints
		{
			get
			{
				return this.constraints;
			}
		}

		internal abstract SortBy[] GetNativeSortBy(SortOrder sortOrder);

		internal abstract NativeStorePropertyDefinition GetNativeGroupBy();

		internal abstract GroupSort GetNativeGroupSort(SortOrder sortOrder, Aggregate aggregate);

		public PropertyValidationError[] Validate(ExchangeOperationContext context, object value)
		{
			if ((this.PropertyFlags & PropertyFlags.Multivalued) == PropertyFlags.Multivalued)
			{
				return this.ValidateMultiValue(context, value);
			}
			PropertyValidationError propertyValidationError = this.ValidateSingleValue(context, value, base.Type);
			if (propertyValidationError != null)
			{
				return new PropertyValidationError[]
				{
					propertyValidationError
				};
			}
			return StorePropertyDefinition.NoValidationError;
		}

		internal PropertyError GetNotFoundError()
		{
			if (this.errorNotFound == null)
			{
				this.errorNotFound = new PropertyError(this, PropertyErrorCode.NotFound);
			}
			return this.errorNotFound;
		}

		internal PropertyError GetNotEnoughMemoryError()
		{
			if (this.errorNotEnoughMemory == null)
			{
				this.errorNotEnoughMemory = new PropertyError(this, PropertyErrorCode.NotEnoughMemory);
			}
			return this.errorNotEnoughMemory;
		}

		internal string GetHashString()
		{
			this.InitializePropertyDefinitionString();
			return this.propertyDefinitionString;
		}

		protected internal void CalculatePropertyFlagsFromType(PropertyFlags childFlags)
		{
			PropertyFlags propertyFlags = childFlags & ~(PropertyFlags.Multivalued | PropertyFlags.Binary);
			if (base.Type.GetTypeInfo().IsSubclassOf(typeof(Array)))
			{
				if (base.Type.Equals(typeof(byte[])))
				{
					propertyFlags |= PropertyFlags.Binary;
				}
				else
				{
					propertyFlags |= PropertyFlags.Multivalued;
					if (base.Type.Equals(typeof(byte[][])))
					{
						propertyFlags |= PropertyFlags.Binary;
					}
				}
			}
			this.propertyFlags = propertyFlags;
		}

		private static ICollection<NativeStorePropertyDefinition> GetNativePropertyDefinitions<T>(PropertyDependencyType targetDependencyType, ICollection<T> propertyDefinitions, bool hashSetOrList, Predicate<NativeStorePropertyDefinition> addToCollection) where T : PropertyDefinition
		{
			EnumValidator.AssertValid<PropertyDependencyType>(targetDependencyType);
			if (propertyDefinitions == null)
			{
				return StorePropertyDefinition.EmptyNativeStoreProperties;
			}
			int num = (propertyDefinitions.Count >= StorePropertyDefinition.dependencyEstimates.Length) ? propertyDefinitions.Count : StorePropertyDefinition.dependencyEstimates[propertyDefinitions.Count];
			ICollection<NativeStorePropertyDefinition> collection = null;
			Action<NativeStorePropertyDefinition> action;
			if (hashSetOrList)
			{
				HashSet<NativeStorePropertyDefinition> nativePropertyDefinitionsSet = new HashSet<NativeStorePropertyDefinition>(num);
				action = delegate(NativeStorePropertyDefinition item)
				{
					if (addToCollection == null)
					{
						nativePropertyDefinitionsSet.TryAdd(item);
						return;
					}
					if (addToCollection(item))
					{
						nativePropertyDefinitionsSet.TryAdd(item);
					}
				};
				collection = nativePropertyDefinitionsSet;
			}
			else
			{
				IList<NativeStorePropertyDefinition> loadList = new List<NativeStorePropertyDefinition>(num);
				action = delegate(NativeStorePropertyDefinition item)
				{
					if (addToCollection == null)
					{
						loadList.Add(item);
						return;
					}
					if (addToCollection(item))
					{
						loadList.Add(item);
					}
				};
				collection = loadList;
			}
			foreach (T t in propertyDefinitions)
			{
				PropertyDefinition propertyDefinition = t;
				StorePropertyDefinition storePropertyDefinition = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
				storePropertyDefinition.ForEachMatch(targetDependencyType, action);
			}
			int count = collection.Count;
			if (count != num && propertyDefinitions.Count < StorePropertyDefinition.dependencyEstimates.Length)
			{
				Interlocked.Exchange(ref StorePropertyDefinition.dependencyEstimates[propertyDefinitions.Count], count);
			}
			return collection;
		}

		protected abstract void ForEachMatch(PropertyDependencyType targetDependencyType, Action<NativeStorePropertyDefinition> action);

		internal bool IsDirty(PropertyBag.BasicPropertyStore propertyBag)
		{
			return this.InternalIsDirty(propertyBag);
		}

		internal void Set(ExchangeOperationContext operationContext, PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			this.ValidateSetPropertyValue(operationContext, value);
			this.InternalSetValue(propertyBag, value);
		}

		internal void SetWithoutValidation(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			this.InternalSetValue(propertyBag, value);
		}

		internal object Get(PropertyBag.BasicPropertyStore propertyBag)
		{
			return this.InternalTryGetValue(propertyBag);
		}

		internal void Delete(PropertyBag.BasicPropertyStore propertyBag)
		{
			this.ValidateDeletePropertyValue();
			this.InternalDeleteValue(propertyBag);
		}

		private static int[] InitializeDependencyEstimates()
		{
			int[] array = new int[256];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 128;
			}
			return array;
		}

		private string InitializePropertyDefinitionString()
		{
			if (this.propertyDefinitionString == null)
			{
				this.propertyDefinitionString = this.GetPropertyDefinitionString();
			}
			return this.propertyDefinitionString;
		}

		private PropertyValidationError[] ValidateMultiValue(ExchangeOperationContext context, object value)
		{
			if (value == null)
			{
				return new PropertyValidationError[]
				{
					new NullValueError(this, value)
				};
			}
			IEnumerable enumerable = value as IEnumerable;
			if (enumerable == null)
			{
				return new PropertyValidationError[]
				{
					new TypeMismatchError(this, value)
				};
			}
			if (!value.GetType().Equals(base.Type))
			{
				return new PropertyValidationError[]
				{
					new TypeMismatchError(this, value)
				};
			}
			int num = 0;
			List<PropertyValidationError> list = new List<PropertyValidationError>();
			Type elementType = base.Type.GetElementType();
			int num2 = 0;
			foreach (object value2 in enumerable)
			{
				num++;
				PropertyValidationError propertyValidationError = this.ValidateSingleValue(context, value2, elementType);
				if (propertyValidationError != null)
				{
					PropertyValidationError item = new InvalidMultivalueElementError(propertyValidationError, value, num2);
					list.Add(item);
				}
				num2++;
			}
			return list.ToArray();
		}

		private PropertyValidationError ValidateSingleValue(ExchangeOperationContext context, object value, Type expectedType)
		{
			if (value == null)
			{
				return new NullValueError(this, value);
			}
			if (value is DateTime && expectedType.Equals(typeof(ExDateTime)))
			{
				expectedType = typeof(DateTime);
			}
			Type type = value.GetType();
			if (!expectedType.Equals(type) && !expectedType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
			{
				return new TypeMismatchError(this, value);
			}
			for (int i = 0; i < this.constraints.Count; i++)
			{
				PropertyValidationError propertyValidationError = this.constraints[i].Validate(context, value, this, null);
				if (propertyValidationError != null)
				{
					return propertyValidationError;
				}
			}
			return null;
		}

		private void ValidateSetPropertyValue(ExchangeOperationContext context, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(base.Name);
			}
			PropertyValidationError[] array = this.Validate(context, value);
			if (array.Length > 0)
			{
				PropertyValidationError propertyValidationError = array[0];
				throw new PropertyValidationException(propertyValidationError.Description, propertyValidationError.PropertyDefinition, array);
			}
		}

		private void ValidateDeletePropertyValue()
		{
			if ((this.PropertyFlags & PropertyFlags.ReadOnly) != PropertyFlags.None)
			{
				PropertyValidationError propertyValidationError = new PropertyValidationError(new LocalizedString(ServerStrings.PropertyIsReadOnly(base.Name)), this, null);
				throw new PropertyValidationException(ServerStrings.PropertyIsReadOnly(base.Name), this, new PropertyValidationError[]
				{
					propertyValidationError
				});
			}
		}

		protected abstract string GetPropertyDefinitionString();

		protected abstract void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value);

		protected abstract object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag);

		protected abstract void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag);

		protected abstract bool InternalIsDirty(PropertyBag.BasicPropertyStore propertyBag);

		private static int[] dependencyEstimates = StorePropertyDefinition.InitializeDependencyEstimates();

		private static readonly NativeStorePropertyDefinition[] EmptyNativeStoreProperties = Array<NativeStorePropertyDefinition>.Empty;

		private readonly PropertyTypeSpecifier propertyTypeSpecifier;

		private PropertyFlags propertyFlags;

		private readonly PropertyFlags childFlags;

		private string propertyDefinitionString;

		private readonly ReadOnlyCollection<PropertyDefinitionConstraint> constraints;

		private static readonly PropertyValidationError[] NoValidationError = Array<PropertyValidationError>.Empty;

		private static readonly ReadOnlyCollection<PropertyDefinitionConstraint> EmptyConstraints = new ReadOnlyCollection<PropertyDefinitionConstraint>(PropertyDefinitionConstraint.None);

		private PropertyError errorNotFound;

		private PropertyError errorNotEnoughMemory;
	}
}
