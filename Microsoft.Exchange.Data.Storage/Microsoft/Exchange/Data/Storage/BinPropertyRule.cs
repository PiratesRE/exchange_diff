using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class BinPropertyRule<TDepend, TSet> : PropertyRule
	{
		public BinPropertyRule(string name, Action<ILocationIdentifierSetter> onSetWriteEnforceLocationIdentifier, NativeStorePropertyDefinition propertyToDepend, NativeStorePropertyDefinition propertyToSet) : base(name, onSetWriteEnforceLocationIdentifier, new PropertyReference[]
		{
			new PropertyReference(propertyToDepend, PropertyAccess.Read),
			new PropertyReference(propertyToSet, PropertyAccess.ReadWrite)
		})
		{
			this.propertyToDepend = propertyToDepend;
			this.propertyToSet = propertyToSet;
		}

		protected sealed override bool WriteEnforceRule(ICorePropertyBag propertyBag)
		{
			bool result = false;
			bool isPropertyDirty = propertyBag.IsPropertyDirty(this.propertyToSet);
			object propertyValue = propertyBag.TryGetProperty(this.propertyToSet);
			if (this.ShouldEnforce(isPropertyDirty, propertyValue))
			{
				TSet valueOrDefault = propertyBag.GetValueOrDefault<TSet>(this.propertyToSet);
				TDepend valueOrDefault2 = propertyBag.GetValueOrDefault<TDepend>(this.propertyToDepend);
				TSet tset;
				if (this.CalculateValue(valueOrDefault2, valueOrDefault, out tset) && (PropertyError.IsPropertyNotFound(propertyValue) || !object.Equals(tset, valueOrDefault)))
				{
					propertyBag.SetOrDeleteProperty(this.propertyToSet, tset);
					result = true;
				}
			}
			return result;
		}

		protected abstract bool CalculateValue(TDepend propertyToDependValue, TSet propertyToSetValue, out TSet newValue);

		protected abstract bool ShouldEnforce(bool isPropertyDirty, object propertyValue);

		private readonly NativeStorePropertyDefinition propertyToSet;

		private readonly NativeStorePropertyDefinition propertyToDepend;
	}
}
