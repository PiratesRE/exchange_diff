﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal abstract class ProviderPropertyDefinition : PropertyDefinition, IEquatable<ProviderPropertyDefinition>
	{
		public CustomFilterBuilderDelegate CustomFilterBuilderDelegate
		{
			get
			{
				return this.customFilterBuilderDelegate;
			}
		}

		public object DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		public ReadOnlyCollection<CollectionPropertyDefinitionConstraint> AllCollectionConstraints
		{
			get
			{
				return this.readOnlyAllCollectionConstraints;
			}
		}

		public ReadOnlyCollection<CollectionPropertyDefinitionConstraint> ReadCollectionConstraints
		{
			get
			{
				return this.readOnlyReadCollectionConstraints;
			}
		}

		public ReadOnlyCollection<PropertyDefinitionConstraint> AllConstraints
		{
			get
			{
				return this.readOnlyAllConstraints;
			}
		}

		public ReadOnlyCollection<PropertyDefinitionConstraint> ReadConstraints
		{
			get
			{
				return this.readOnlyReadConstraints;
			}
		}

		public ReadOnlyCollection<ProviderPropertyDefinition> SupportingProperties
		{
			get
			{
				return this.supportingProperties;
			}
		}

		public ReadOnlyCollection<ProviderPropertyDefinition> DependentProperties
		{
			get
			{
				return this.readOnlyDependentProperties;
			}
		}

		public GetterDelegate GetterDelegate
		{
			get
			{
				return this.getterDelegate;
			}
		}

		public SetterDelegate SetterDelegate
		{
			get
			{
				return this.setterDelegate;
			}
		}

		public ExchangeObjectVersion VersionAdded
		{
			get
			{
				return this.versionAdded;
			}
		}

		public virtual bool IsFilterable
		{
			get
			{
				return !this.IsCalculated || null != this.CustomFilterBuilderDelegate;
			}
		}

		internal bool HasAutogeneratedConstraints
		{
			get
			{
				return this.hasAutogeneratedConstraints;
			}
		}

		public ProviderPropertyDefinition(string name, ExchangeObjectVersion versionAdded, Type type, object defaultValue) : this(name, versionAdded, type, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None)
		{
		}

		public ProviderPropertyDefinition(string name, ExchangeObjectVersion versionAdded, Type type, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints) : this(name, versionAdded, type, defaultValue, readConstraints, writeConstraints, ProviderPropertyDefinition.None, null, null, null)
		{
		}

		public ProviderPropertyDefinition(string name, ExchangeObjectVersion versionAdded, Type type, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints, ProviderPropertyDefinition[] supportingProperties, CustomFilterBuilderDelegate customFilterBuilderDelegate, GetterDelegate getterDelegate, SetterDelegate setterDelegate) : base(name, type)
		{
			if (supportingProperties == null)
			{
				throw new ArgumentNullException("supportingProperties");
			}
			if (readConstraints == null)
			{
				throw new ArgumentNullException("readConstraints");
			}
			if (writeConstraints == null)
			{
				throw new ArgumentNullException("writeConstraints");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (versionAdded == null)
			{
				throw new ArgumentNullException("versionAdded");
			}
			if (defaultValue != null && !ReflectionHelper.IsInstanceOfType(defaultValue, type))
			{
				throw new ArgumentException(DataStrings.ExceptionDefaultTypeMismatch.ToString(), "defaultValue (" + name + ")");
			}
			if (type == typeof(bool) && defaultValue != null)
			{
				defaultValue = BoxedConstants.GetBool((bool)defaultValue);
			}
			this.defaultValue = defaultValue;
			this.customFilterBuilderDelegate = customFilterBuilderDelegate;
			this.versionAdded = versionAdded;
			if (readConstraints.Length < 1)
			{
				this.readOnlyReadConstraints = ProviderPropertyDefinition.EmptyConstraint.Collection;
			}
			else
			{
				this.readOnlyReadConstraints = new ReadOnlyCollection<PropertyDefinitionConstraint>(readConstraints);
			}
			if (writeConstraints.Length < 1 && readConstraints.Length < 1)
			{
				this.allStaticConstraints = PropertyDefinitionConstraint.None;
				this.readOnlyAllConstraints = ProviderPropertyDefinition.EmptyConstraint.Collection;
			}
			else
			{
				this.allStaticConstraints = new PropertyDefinitionConstraint[readConstraints.Length + writeConstraints.Length];
				Array.Copy(writeConstraints, this.allStaticConstraints, writeConstraints.Length);
				Array.Copy(readConstraints, 0, this.allStaticConstraints, writeConstraints.Length, readConstraints.Length);
				this.readOnlyAllConstraints = new ReadOnlyCollection<PropertyDefinitionConstraint>(this.allStaticConstraints);
			}
			this.getterDelegate = getterDelegate;
			this.setterDelegate = setterDelegate;
			if (supportingProperties.Length < 1)
			{
				this.supportingProperties = ProviderPropertyDefinition.EmptyCollection;
			}
			else
			{
				this.supportingProperties = new ReadOnlyCollection<ProviderPropertyDefinition>(supportingProperties);
			}
			List<CollectionPropertyDefinitionConstraint> list = new List<CollectionPropertyDefinitionConstraint>();
			List<CollectionPropertyDefinitionConstraint> list2 = new List<CollectionPropertyDefinitionConstraint>();
			for (int i = 0; i < writeConstraints.Length; i++)
			{
				CollectionPropertyDefinitionConstraint collectionPropertyDefinitionConstraint = writeConstraints[i] as CollectionPropertyDefinitionConstraint;
				if (collectionPropertyDefinitionConstraint != null)
				{
					list.Add(collectionPropertyDefinitionConstraint);
				}
			}
			for (int j = 0; j < readConstraints.Length; j++)
			{
				CollectionPropertyDefinitionConstraint collectionPropertyDefinitionConstraint2 = readConstraints[j] as CollectionPropertyDefinitionConstraint;
				if (collectionPropertyDefinitionConstraint2 != null)
				{
					list.Add(collectionPropertyDefinitionConstraint2);
					list2.Add(collectionPropertyDefinitionConstraint2);
				}
			}
			if (list.Count < 1)
			{
				this.allStaticCollectionConstraints = ProviderPropertyDefinition.EmptyCollectionConstraint.Array;
				this.readOnlyAllCollectionConstraints = ProviderPropertyDefinition.EmptyCollectionConstraint.Collection;
			}
			else
			{
				this.allStaticCollectionConstraints = list.ToArray();
				this.readOnlyAllCollectionConstraints = new ReadOnlyCollection<CollectionPropertyDefinitionConstraint>(this.allStaticCollectionConstraints);
			}
			if (list2.Count < 1)
			{
				this.readOnlyReadCollectionConstraints = ProviderPropertyDefinition.EmptyCollectionConstraint.Collection;
			}
			else
			{
				this.readOnlyReadCollectionConstraints = new ReadOnlyCollection<CollectionPropertyDefinitionConstraint>(list2.ToArray());
			}
			this.readOnlyDependentProperties = ProviderPropertyDefinition.EmptyCollection;
			if (this.supportingProperties.Count == 0)
			{
				this.dependentProperties = new List<ProviderPropertyDefinition>();
			}
			foreach (ProviderPropertyDefinition providerPropertyDefinition in supportingProperties)
			{
				if (providerPropertyDefinition.IsCalculated)
				{
					throw new ArgumentException(string.Format("The calculated property '{0}' cannot depend on another calculated property '{1}'", base.Name, providerPropertyDefinition.Name), "supportingProperties");
				}
				if (this.VersionAdded.IsOlderThan(providerPropertyDefinition.VersionAdded))
				{
					throw new ArgumentException(string.Format("The calculated property '{0}' cannot depend on the newer property '{1}'", base.Name, providerPropertyDefinition.Name), "supportingProperties");
				}
				providerPropertyDefinition.AddDependency(this);
			}
			if (defaultValue != null && defaultValue != string.Empty)
			{
				PropertyValidationError propertyValidationError = this.ValidateValue(defaultValue, false);
				if (propertyValidationError != null)
				{
					throw new ArgumentException(propertyValidationError.Description, "defaultValue");
				}
			}
		}

		internal void SetAutogeneratedConstraints(PropertyDefinitionConstraint[] autogeneratedConstraints)
		{
			if (autogeneratedConstraints == null)
			{
				throw new ArgumentNullException("autogeneratedConstraints");
			}
			PropertyDefinitionConstraint[] array;
			CollectionPropertyDefinitionConstraint[] list;
			if (autogeneratedConstraints.Length == 0)
			{
				array = this.allStaticConstraints;
				list = this.allStaticCollectionConstraints;
			}
			else
			{
				array = new PropertyDefinitionConstraint[this.allStaticConstraints.Length + autogeneratedConstraints.Length];
				Array.Copy(this.allStaticConstraints, array, this.allStaticConstraints.Length);
				Array.Copy(autogeneratedConstraints, 0, array, this.allStaticConstraints.Length, autogeneratedConstraints.Length);
				List<CollectionPropertyDefinitionConstraint> list2 = new List<CollectionPropertyDefinitionConstraint>(this.allStaticCollectionConstraints);
				for (int i = 0; i < autogeneratedConstraints.Length; i++)
				{
					CollectionPropertyDefinitionConstraint collectionPropertyDefinitionConstraint = autogeneratedConstraints[i] as CollectionPropertyDefinitionConstraint;
					if (collectionPropertyDefinitionConstraint != null)
					{
						list2.Add(collectionPropertyDefinitionConstraint);
					}
				}
				list = list2.ToArray();
			}
			lock (this.autogeneratedConstraintsSyncObject)
			{
				this.readOnlyAllConstraints = new ReadOnlyCollection<PropertyDefinitionConstraint>(array);
				this.readOnlyAllCollectionConstraints = new ReadOnlyCollection<CollectionPropertyDefinitionConstraint>(list);
				this.hasAutogeneratedConstraints = (0 != autogeneratedConstraints.Length);
			}
		}

		[Conditional("DEBUG")]
		private void CheckConstraintConsistency(PropertyDefinitionConstraint[] staticConstraints, PropertyDefinitionConstraint[] autogeneratedConstraints)
		{
			for (int i = 0; i < autogeneratedConstraints.Length; i++)
			{
				Type type = autogeneratedConstraints[i].GetType();
				for (int j = 0; j < staticConstraints.Length; j++)
				{
					Type type2 = staticConstraints[j].GetType();
					if (type2 != type)
					{
						break;
					}
					if (typeof(StringLengthConstraint).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
					{
						StringLengthConstraint stringLengthConstraint = (StringLengthConstraint)autogeneratedConstraints[i];
						StringLengthConstraint stringLengthConstraint2 = (StringLengthConstraint)staticConstraints[j];
					}
					else if (typeof(ByteArrayLengthConstraint).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
					{
						ByteArrayLengthConstraint byteArrayLengthConstraint = (ByteArrayLengthConstraint)autogeneratedConstraints[i];
						ByteArrayLengthConstraint byteArrayLengthConstraint2 = (ByteArrayLengthConstraint)staticConstraints[j];
					}
					else if (typeof(RangedValueConstraint<ByteQuantifiedSize>).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
					{
						RangedValueConstraint<ByteQuantifiedSize> rangedValueConstraint = (RangedValueConstraint<ByteQuantifiedSize>)autogeneratedConstraints[i];
						RangedValueConstraint<ByteQuantifiedSize> rangedValueConstraint2 = (RangedValueConstraint<ByteQuantifiedSize>)staticConstraints[j];
					}
					else if (typeof(RangedValueConstraint<int>).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
					{
						RangedValueConstraint<int> rangedValueConstraint3 = (RangedValueConstraint<int>)autogeneratedConstraints[i];
						RangedValueConstraint<int> rangedValueConstraint4 = (RangedValueConstraint<int>)staticConstraints[j];
					}
					else if (typeof(RangedValueConstraint<long>).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
					{
						RangedValueConstraint<long> rangedValueConstraint5 = (RangedValueConstraint<long>)autogeneratedConstraints[i];
						RangedValueConstraint<long> rangedValueConstraint6 = (RangedValueConstraint<long>)staticConstraints[j];
					}
				}
			}
		}

		private void AddDependency(ProviderPropertyDefinition dependentProperty)
		{
			lock (this.dependentPropertiesSyncObject)
			{
				this.dependentProperties.Add(dependentProperty);
				this.readOnlyDependentProperties = new ReadOnlyCollection<ProviderPropertyDefinition>(this.dependentProperties.ToArray());
			}
		}

		public virtual PropertyValidationError ValidateValue(object value, bool useOnlyReadConstraints)
		{
			return this.ValidateSingleValue(value, useOnlyReadConstraints, null);
		}

		public PropertyValidationError ValidateCollection(IEnumerable collection, bool useOnlyReadConstraints)
		{
			return this.ValidateCollection(collection, useOnlyReadConstraints, null);
		}

		public virtual IList<ValidationError> ValidateProperty(object value, IPropertyBag propertyBag, bool useOnlyReadConstraints)
		{
			List<ValidationError> list = new List<ValidationError>();
			if (value == null)
			{
				if (this.IsMandatory)
				{
					list.Add(new PropertyValidationError(DataStrings.PropertyIsMandatory, this, null));
				}
				return list;
			}
			if (this.IsMultivalued)
			{
				IEnumerable enumerable = value as IEnumerable;
				if (enumerable == null)
				{
					list.Add(new PropertyValidationError(DataStrings.PropertyNotACollection(value.GetType().Name), this, value));
					return list;
				}
				int num = 0;
				foreach (object value2 in enumerable)
				{
					num++;
					PropertyValidationError propertyValidationError = this.ValidateSingleValue(value2, useOnlyReadConstraints, propertyBag);
					if (propertyValidationError != null)
					{
						list.Add(propertyValidationError);
					}
				}
				if (num == 0 && this.IsMandatory)
				{
					list.Add(new PropertyValidationError(DataStrings.PropertyIsMandatory, this, null));
				}
				PropertyValidationError propertyValidationError2 = this.ValidateCollection(enumerable, useOnlyReadConstraints, propertyBag);
				if (propertyValidationError2 != null)
				{
					list.Add(propertyValidationError2);
				}
			}
			else
			{
				PropertyValidationError propertyValidationError3 = this.ValidateSingleValue(value, useOnlyReadConstraints, propertyBag);
				if (propertyValidationError3 != null)
				{
					list.Add(propertyValidationError3);
				}
			}
			return list;
		}

		private PropertyValidationError ValidateCollection(IEnumerable collection, bool useOnlyReadConstraints, IPropertyBag propertyBag)
		{
			ReadOnlyCollection<CollectionPropertyDefinitionConstraint> readOnlyCollection = useOnlyReadConstraints ? this.readOnlyReadCollectionConstraints : this.readOnlyAllCollectionConstraints;
			int count = readOnlyCollection.Count;
			for (int i = 0; i < count; i++)
			{
				PropertyValidationError propertyValidationError = readOnlyCollection[i].Validate(collection, this, propertyBag);
				if (propertyValidationError != null)
				{
					return propertyValidationError;
				}
			}
			return null;
		}

		private PropertyValidationError ValidateSingleValue(object value, bool useOnlyReadConstraints, IPropertyBag propertyBag)
		{
			if (this.IsMandatory)
			{
				if (value == null)
				{
					return new PropertyValidationError(DataStrings.PropertyIsMandatory, this, null);
				}
				if (value.GetType() == typeof(string) && value.ToString().Length == 0)
				{
					return new PropertyValidationError(DataStrings.PropertyIsMandatory, this, value);
				}
			}
			if (value != null && !ReflectionHelper.IsInstanceOfType(value, base.Type))
			{
				return new PropertyValidationError(DataStrings.PropertyTypeMismatch(value.GetType().FullName, base.Type.FullName), this, value);
			}
			ReadOnlyCollection<PropertyDefinitionConstraint> readOnlyCollection = useOnlyReadConstraints ? this.readOnlyReadConstraints : this.readOnlyAllConstraints;
			int count = readOnlyCollection.Count;
			for (int i = 0; i < count; i++)
			{
				PropertyDefinitionConstraint propertyDefinitionConstraint = readOnlyCollection[i];
				if (!(propertyDefinitionConstraint is CollectionPropertyDefinitionConstraint))
				{
					PropertyValidationError propertyValidationError = propertyDefinitionConstraint.Validate(value, this, propertyBag);
					if (propertyValidationError != null)
					{
						return propertyValidationError;
					}
				}
			}
			return null;
		}

		public abstract bool IsMultivalued { get; }

		public abstract bool IsReadOnly { get; }

		public abstract bool IsCalculated { get; }

		public abstract bool IsFilterOnly { get; }

		public abstract bool IsMandatory { get; }

		public abstract bool PersistDefaultValue { get; }

		public abstract bool IsWriteOnce { get; }

		public abstract bool IsBinary { get; }

		public virtual bool IsTaskPopulated
		{
			get
			{
				return false;
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ProviderPropertyDefinition);
		}

		public override int GetHashCode()
		{
			if (this.hashcode == 0)
			{
				this.hashcode = base.Name.ToLower().GetHashCode();
			}
			return this.hashcode;
		}

		public override string ToString()
		{
			return string.Format("{0} ({1})", base.Name, base.Type);
		}

		public virtual bool Equals(ProviderPropertyDefinition other)
		{
			return other != null && (object.ReferenceEquals(other, this) || (!this.IsCalculated && StringComparer.OrdinalIgnoreCase.Equals(other.Name, base.Name) && !(other.Type != base.Type) && !(other.GetType() != base.GetType())));
		}

		public static ProviderPropertyDefinition[] None = new ProviderPropertyDefinition[0];

		private static readonly ReadOnlyCollection<ProviderPropertyDefinition> EmptyCollection = new ReadOnlyCollection<ProviderPropertyDefinition>(ProviderPropertyDefinition.None);

		private static readonly ProviderPropertyDefinition.Empty<CollectionPropertyDefinitionConstraint> EmptyCollectionConstraint = new ProviderPropertyDefinition.Empty<CollectionPropertyDefinitionConstraint>();

		private static readonly ProviderPropertyDefinition.Empty<PropertyDefinitionConstraint> EmptyConstraint = new ProviderPropertyDefinition.Empty<PropertyDefinitionConstraint>();

		[NonSerialized]
		private readonly object dependentPropertiesSyncObject = new object();

		[NonSerialized]
		protected int hashcode;

		[NonSerialized]
		private ExchangeObjectVersion versionAdded;

		[NonSerialized]
		private object defaultValue;

		[NonSerialized]
		private PropertyDefinitionConstraint[] allStaticConstraints;

		[NonSerialized]
		private CollectionPropertyDefinitionConstraint[] allStaticCollectionConstraints;

		[NonSerialized]
		private ReadOnlyCollection<PropertyDefinitionConstraint> readOnlyReadConstraints;

		[NonSerialized]
		private ReadOnlyCollection<PropertyDefinitionConstraint> readOnlyAllConstraints;

		[NonSerialized]
		private ReadOnlyCollection<CollectionPropertyDefinitionConstraint> readOnlyReadCollectionConstraints;

		[NonSerialized]
		private ReadOnlyCollection<CollectionPropertyDefinitionConstraint> readOnlyAllCollectionConstraints;

		[NonSerialized]
		private CustomFilterBuilderDelegate customFilterBuilderDelegate;

		[NonSerialized]
		private GetterDelegate getterDelegate;

		[NonSerialized]
		private SetterDelegate setterDelegate;

		[NonSerialized]
		private ReadOnlyCollection<ProviderPropertyDefinition> supportingProperties;

		[NonSerialized]
		private ReadOnlyCollection<ProviderPropertyDefinition> readOnlyDependentProperties;

		[NonSerialized]
		private List<ProviderPropertyDefinition> dependentProperties;

		[NonSerialized]
		private bool hasAutogeneratedConstraints;

		[NonSerialized]
		private object autogeneratedConstraintsSyncObject = new object();

		private class Empty<T>
		{
			public Empty()
			{
				this.Array = new T[0];
				this.Collection = new ReadOnlyCollection<T>(this.Array);
			}

			public readonly T[] Array;

			public readonly ReadOnlyCollection<T> Collection;
		}
	}
}
