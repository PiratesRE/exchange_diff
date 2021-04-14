using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyTranslationRules
{
	internal class PassThruPropertyTranslationRule<TLeft, TRight, TLeftProperty, TPropertyValue> : PropertyTranslationRule<TLeft, TRight, TLeftProperty, TPropertyValue, TPropertyValue>
	{
		public PassThruPropertyTranslationRule(IPropertyAccessor<TLeft, TPropertyValue> leftAccessor, IPropertyAccessor<TRight, TPropertyValue> rightAccessor) : base(leftAccessor, rightAccessor, PassThruConverter<TPropertyValue>.SingletonInstance, PassThruConverter<TPropertyValue>.SingletonInstance)
		{
		}
	}
}
