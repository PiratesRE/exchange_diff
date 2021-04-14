using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class PriorityBasedDisplayNamePropertyRule
	{
		public PriorityBasedDisplayNamePropertyRule()
		{
			this.candidateProperties = this.GetCandidateProperties();
			this.allSourceProperties = this.GetAllSourceProperties();
		}

		public virtual List<PropertyReference> GetPropertyReferenceList()
		{
			List<PropertyReference> list = new List<PropertyReference>(this.allSourceProperties.Count + PriorityBasedDisplayNamePropertyRule.destinationProperties.Count);
			foreach (NativeStorePropertyDefinition usedProperty in this.allSourceProperties)
			{
				list.Add(new PropertyReference(usedProperty, PropertyAccess.Read));
			}
			foreach (NativeStorePropertyDefinition usedProperty2 in PriorityBasedDisplayNamePropertyRule.destinationProperties)
			{
				list.Add(new PropertyReference(usedProperty2, PropertyAccess.Write));
			}
			return list;
		}

		public virtual bool UpdateDisplayNameProperties(ICorePropertyBag propertyBag)
		{
			bool flag = false;
			foreach (NativeStorePropertyDefinition propertyDefinition in this.allSourceProperties)
			{
				if (propertyBag.IsPropertyDirty(propertyDefinition))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
			bool flag2 = false;
			for (int i = 0; i < this.candidateProperties.Count; i++)
			{
				PriorityBasedDisplayNamePropertyRule.CandidateProperty candidateProperty = this.candidateProperties[i];
				if (candidateProperty.HasNonEmptyValue(propertyBag))
				{
					string value = null;
					string value2 = null;
					candidateProperty.GetValue(propertyBag, out value, out value2);
					propertyBag.SetProperty(InternalSchema.DisplayNameFirstLast, value);
					propertyBag.SetProperty(InternalSchema.DisplayNameLastFirst, value2);
					propertyBag.SetProperty(InternalSchema.DisplayNamePriority, i);
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				propertyBag.Delete(InternalSchema.DisplayNameFirstLast);
				propertyBag.Delete(InternalSchema.DisplayNameLastFirst);
				propertyBag.Delete(InternalSchema.DisplayNamePriority);
			}
			return true;
		}

		protected abstract IList<PriorityBasedDisplayNamePropertyRule.CandidateProperty> GetCandidateProperties();

		protected List<NativeStorePropertyDefinition> GetAllSourceProperties()
		{
			List<NativeStorePropertyDefinition> list = new List<NativeStorePropertyDefinition>(this.candidateProperties.Count);
			foreach (PriorityBasedDisplayNamePropertyRule.CandidateProperty candidateProperty in this.candidateProperties)
			{
				list.AddRange(candidateProperty.PropertyDefinitions);
			}
			return list;
		}

		private static readonly IList<NativeStorePropertyDefinition> destinationProperties = new List<NativeStorePropertyDefinition>
		{
			InternalSchema.DisplayNameFirstLast,
			InternalSchema.DisplayNameLastFirst,
			InternalSchema.DisplayNamePriority
		};

		private readonly IList<PriorityBasedDisplayNamePropertyRule.CandidateProperty> candidateProperties;

		private readonly List<NativeStorePropertyDefinition> allSourceProperties;

		protected class CandidateProperty
		{
			public List<NativeStorePropertyDefinition> PropertyDefinitions { get; private set; }

			public CandidateProperty(List<NativeStorePropertyDefinition> propertyDefinitions, PriorityBasedDisplayNamePropertyRule.CandidateProperty.DisplayNameValueDelegate valueDelegate)
			{
				this.PropertyDefinitions = propertyDefinitions;
				this.valueDelegate = valueDelegate;
			}

			public void GetValue(ICorePropertyBag propertyBag, out string displayNameFirstLast, out string displayNameLastFirst)
			{
				this.valueDelegate(propertyBag, out displayNameFirstLast, out displayNameLastFirst);
			}

			public bool HasNonEmptyValue(ICorePropertyBag propertyBag)
			{
				foreach (NativeStorePropertyDefinition propertyDefinition in this.PropertyDefinitions)
				{
					if (propertyBag.GetValueOrDefault<string>(propertyDefinition, string.Empty).Trim() != string.Empty)
					{
						return true;
					}
				}
				return false;
			}

			private readonly PriorityBasedDisplayNamePropertyRule.CandidateProperty.DisplayNameValueDelegate valueDelegate;

			internal delegate void DisplayNameValueDelegate(ICorePropertyBag propertyBag, out string displayNameFirstLast, out string displayNameLastFirst);
		}

		protected sealed class SimpleCandidateProperty : PriorityBasedDisplayNamePropertyRule.CandidateProperty
		{
			public SimpleCandidateProperty(NativeStorePropertyDefinition propertyDefinition) : base(new List<NativeStorePropertyDefinition>(1)
			{
				propertyDefinition
			}, PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty.ValueDelegate(propertyDefinition))
			{
			}

			private static PriorityBasedDisplayNamePropertyRule.CandidateProperty.DisplayNameValueDelegate ValueDelegate(NativeStorePropertyDefinition propertyDefinition)
			{
				return delegate(ICorePropertyBag propertyBag, out string displayNameFirstLast, out string displayNameLastFirst)
				{
					string valueOrDefault;
					displayNameLastFirst = (valueOrDefault = propertyBag.GetValueOrDefault<string>(propertyDefinition, string.Empty));
					displayNameFirstLast = valueOrDefault;
				};
			}
		}
	}
}
