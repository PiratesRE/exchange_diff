using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Schema
	{
		protected internal Schema()
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			HashSet<PropertyDefinition> hashSet2 = new HashSet<PropertyDefinition>();
			HashSet<PropertyDefinition> hashSet3 = new HashSet<PropertyDefinition>();
			List<StoreObjectConstraint> list = new List<StoreObjectConstraint>();
			HashSet<PropertyDefinition> propertySet = new HashSet<PropertyDefinition>();
			HashSet<PropertyDefinition> propertySet2 = new HashSet<PropertyDefinition>();
			HashSet<StorePropertyDefinition> propertySet3 = new HashSet<StorePropertyDefinition>();
			HashSet<StorePropertyDefinition> propertySet4 = new HashSet<StorePropertyDefinition>();
			foreach (FieldInfo fieldInfo in ReflectionHelper.AggregateTypeHierarchy<FieldInfo>(base.GetType(), new AggregateType<FieldInfo>(ReflectionHelper.AggregateStaticFields)))
			{
				object value = fieldInfo.GetValue(null);
				StorePropertyDefinition storePropertyDefinition = value as StorePropertyDefinition;
				if (storePropertyDefinition != null && (storePropertyDefinition.PropertyFlags & PropertyFlags.FilterOnly) == PropertyFlags.None)
				{
					hashSet2.Add(storePropertyDefinition);
					if (fieldInfo.IsPublic)
					{
						hashSet.Add(storePropertyDefinition);
						if (storePropertyDefinition is SmartPropertyDefinition)
						{
							hashSet3.Add(storePropertyDefinition);
						}
					}
					if (fieldInfo.GetCustomAttribute<DetectCodepageAttribute>() != null)
					{
						Schema.AddPropertyAndDependentPropertiesToSet<StorePropertyDefinition>(propertySet3, storePropertyDefinition);
						Schema.AddPropertyAndDependentPropertiesToSet<PropertyDefinition>(propertySet, storePropertyDefinition);
						Schema.AddPropertyAndDependentPropertiesToSet<PropertyDefinition>(propertySet2, storePropertyDefinition);
					}
					else if (fieldInfo.GetCustomAttribute<AutoloadAttribute>() != null)
					{
						Schema.AddPropertyAndDependentPropertiesToSet<PropertyDefinition>(propertySet, storePropertyDefinition);
						Schema.AddPropertyAndDependentPropertiesToSet<PropertyDefinition>(propertySet2, storePropertyDefinition);
					}
					else if (storePropertyDefinition is SmartPropertyDefinition)
					{
						SmartPropertyDefinition smartPropertyDefinition = (SmartPropertyDefinition)storePropertyDefinition;
						Schema.AddSmartPropertyToSet<PropertyDefinition>(propertySet, smartPropertyDefinition.Dependencies, PropertyDependencyType.NeedToReadForWrite);
						Schema.AddSmartPropertyToSet<PropertyDefinition>(propertySet2, smartPropertyDefinition.Dependencies, PropertyDependencyType.NeedToReadForWrite);
					}
					else if (fieldInfo.GetCustomAttribute<OptionalAutoloadAttribute>() != null)
					{
						Schema.AddPropertyAndDependentPropertiesToSet<PropertyDefinition>(propertySet, storePropertyDefinition);
					}
					if (fieldInfo.GetCustomAttribute<LegalTrackingAttribute>() != null)
					{
						Schema.AddPropertyAndDependentLegalTrackingPropertiesToSet<StorePropertyDefinition>(propertySet4, storePropertyDefinition);
					}
					foreach (object obj in fieldInfo.GetCustomAttributes(typeof(ConstraintAttribute), false))
					{
						ConstraintAttribute constraintAttribute = obj as ConstraintAttribute;
						if (constraintAttribute != null)
						{
							list.Add(constraintAttribute.GetConstraint(storePropertyDefinition));
						}
					}
				}
			}
			this.allProperties = hashSet;
			this.allPropertiesInternal = hashSet2;
			this.smartProperties = hashSet3;
			this.autoloadProperties = propertySet;
			this.requiredAutoloadProperties = propertySet2;
			this.detectCodepageProperties = propertySet3;
			this.legalTrackingProperties = propertySet4;
			this.AddConstraints(list);
			this.constraints = list.ToArray();
		}

		protected virtual void AddConstraints(List<StoreObjectConstraint> constraints)
		{
		}

		protected void RemoveConstraints(StoreObjectConstraint[] constraints)
		{
			List<StoreObjectConstraint> list = new List<StoreObjectConstraint>(this.constraints);
			for (int i = 0; i < constraints.Length; i++)
			{
				list.Remove(constraints[i]);
			}
			this.constraints = list.ToArray();
		}

		private static void AddPropertyAndDependentLegalTrackingPropertiesToSet<T>(HashSet<T> propertySet, StorePropertyDefinition propertyDefinition) where T : PropertyDefinition
		{
			SmartPropertyDefinition smartPropertyDefinition = propertyDefinition as SmartPropertyDefinition;
			if (smartPropertyDefinition != null)
			{
				Schema.AddSmartPropertyToSet<T>(propertySet, smartPropertyDefinition.LegalTrackingDependencies, PropertyDependencyType.AllRead);
				return;
			}
			propertySet.Add(propertyDefinition as T);
		}

		private static void AddPropertyAndDependentPropertiesToSet<T>(HashSet<T> propertySet, StorePropertyDefinition propertyDefinition) where T : PropertyDefinition
		{
			SmartPropertyDefinition smartPropertyDefinition = propertyDefinition as SmartPropertyDefinition;
			if (smartPropertyDefinition != null)
			{
				Schema.AddSmartPropertyToSet<T>(propertySet, smartPropertyDefinition.Dependencies, PropertyDependencyType.AllRead);
				return;
			}
			propertySet.Add(propertyDefinition as T);
		}

		private static void AddSmartPropertyToSet<T>(HashSet<T> propertySet, PropertyDependency[] dependencies, PropertyDependencyType dependencyType) where T : PropertyDefinition
		{
			if (dependencies == null)
			{
				return;
			}
			foreach (PropertyDependency propertyDependency in dependencies)
			{
				if ((propertyDependency.Type & dependencyType) != PropertyDependencyType.None)
				{
					propertySet.Add(propertyDependency.Property as T);
				}
			}
		}

		protected void AddDependencies(params Schema[] dependencies)
		{
			List<StoreObjectConstraint> list = new List<StoreObjectConstraint>(this.Constraints);
			foreach (Schema schema in dependencies)
			{
				this.allProperties.UnionWith(schema.AllProperties);
				this.allPropertiesInternal.UnionWith(schema.InternalAllProperties);
				this.autoloadProperties.UnionWith(schema.AutoloadProperties);
				this.requiredAutoloadProperties.UnionWith(schema.RequiredAutoloadProperties);
				this.detectCodepageProperties.UnionWith(schema.DetectCodepageProperties);
				this.legalTrackingProperties.UnionWith(schema.LegalTrackingProperties);
				list.AddRange(schema.Constraints);
			}
			this.constraints = list.ToArray();
		}

		public static void FlushCache()
		{
			PropertyTagCache.Cache.Reset();
		}

		public static Schema Instance
		{
			get
			{
				if (Schema.instance == null)
				{
					Schema.instance = new Schema();
				}
				return Schema.instance;
			}
		}

		public ICollection<PropertyDefinition> AllProperties
		{
			get
			{
				return this.allProperties;
			}
		}

		internal ICollection<PropertyDefinition> InternalAllProperties
		{
			get
			{
				return this.allPropertiesInternal;
			}
		}

		public ICollection<PropertyDefinition> SmartProperties
		{
			get
			{
				return this.smartProperties;
			}
		}

		public ICollection<PropertyDefinition> AutoloadProperties
		{
			get
			{
				return this.autoloadProperties;
			}
		}

		internal ICollection<PropertyDefinition> RequiredAutoloadProperties
		{
			get
			{
				return this.requiredAutoloadProperties;
			}
		}

		internal StoreObjectConstraint[] Constraints
		{
			get
			{
				return this.constraints;
			}
		}

		internal ICollection<StorePropertyDefinition> DetectCodepageProperties
		{
			get
			{
				return this.detectCodepageProperties;
			}
		}

		internal ICollection<StorePropertyDefinition> LegalTrackingProperties
		{
			get
			{
				return this.legalTrackingProperties;
			}
		}

		private static Schema instance;

		private HashSet<PropertyDefinition> allProperties;

		private HashSet<PropertyDefinition> allPropertiesInternal;

		private HashSet<PropertyDefinition> smartProperties;

		private HashSet<PropertyDefinition> autoloadProperties;

		private HashSet<PropertyDefinition> requiredAutoloadProperties;

		private HashSet<StorePropertyDefinition> detectCodepageProperties;

		private HashSet<StorePropertyDefinition> legalTrackingProperties;

		private StoreObjectConstraint[] constraints;
	}
}
