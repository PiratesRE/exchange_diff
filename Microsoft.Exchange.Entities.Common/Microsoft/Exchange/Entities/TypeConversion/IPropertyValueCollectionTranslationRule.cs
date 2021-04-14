using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Entities.TypeConversion
{
	internal interface IPropertyValueCollectionTranslationRule<in TLeft, TLeftProperty, in TRight> : ITranslationRule<TLeft, TRight>
	{
		void FromPropertyValues(IDictionary<TLeftProperty, int> propertyIndices, IList values, TRight right);
	}
}
