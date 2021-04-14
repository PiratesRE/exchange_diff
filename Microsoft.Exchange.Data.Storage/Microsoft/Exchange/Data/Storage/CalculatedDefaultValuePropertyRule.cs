using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CalculatedDefaultValuePropertyRule<TDepend, TSet> : BinPropertyRule<TDepend, TSet>
	{
		public CalculatedDefaultValuePropertyRule(string name, Action<ILocationIdentifierSetter> onSetWriteEnforceLocationIdentifier, NativeStorePropertyDefinition propertyToDepend, NativeStorePropertyDefinition propertyToSet, CalculatedDefaultValuePropertyRule<TDepend, TSet>.CalculateDefaultValueDelegate calculateDefaultValueDelegate) : base(name, onSetWriteEnforceLocationIdentifier, propertyToDepend, propertyToSet)
		{
			if (calculateDefaultValueDelegate == null)
			{
				throw new ArgumentNullException("calculateDefaultValueDelegate");
			}
			this.calculateDelegate = calculateDefaultValueDelegate;
		}

		protected override bool CalculateValue(TDepend propertyToDependValue, TSet propertyToSetValue, out TSet calculatedResult)
		{
			return this.calculateDelegate(propertyToDependValue, out calculatedResult);
		}

		protected override bool ShouldEnforce(bool isPropertyDirty, object propertyValue)
		{
			return PropertyError.IsPropertyNotFound(propertyValue) || propertyValue == null;
		}

		private CalculatedDefaultValuePropertyRule<TDepend, TSet>.CalculateDefaultValueDelegate calculateDelegate;

		internal delegate bool CalculateDefaultValueDelegate(TDepend propertyToDependValue, out TSet calculatedResult);
	}
}
