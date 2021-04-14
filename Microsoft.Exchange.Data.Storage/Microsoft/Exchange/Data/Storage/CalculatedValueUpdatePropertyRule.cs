using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CalculatedValueUpdatePropertyRule<TDepend, TSet> : BinPropertyRule<TDepend, TSet>
	{
		public CalculatedValueUpdatePropertyRule(string name, Action<ILocationIdentifierSetter> onSetWriteEnforceLocationIdentifier, NativeStorePropertyDefinition propertyToDepend, NativeStorePropertyDefinition propertyToSet, CalculatedValueUpdatePropertyRule<TDepend, TSet>.CalculateNewValueDelegate calculateNewValueDelegate) : base(name, onSetWriteEnforceLocationIdentifier, propertyToDepend, propertyToSet)
		{
			if (calculateNewValueDelegate == null)
			{
				throw new ArgumentNullException("calculateNewValueDelegate");
			}
			this.calculateDelegate = calculateNewValueDelegate;
		}

		protected override bool CalculateValue(TDepend propertyToDependValue, TSet propertyToSetValue, out TSet calculatedResult)
		{
			return this.calculateDelegate(propertyToDependValue, propertyToSetValue, out calculatedResult);
		}

		protected override bool ShouldEnforce(bool isPropertyDirty, object propertyValue)
		{
			return isPropertyDirty;
		}

		private CalculatedValueUpdatePropertyRule<TDepend, TSet>.CalculateNewValueDelegate calculateDelegate;

		internal delegate bool CalculateNewValueDelegate(TDepend propertyToDependValue, TSet propertyToSetValue, out TSet calculatedResult);
	}
}
