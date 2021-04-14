using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Entities.TypeConversion
{
	internal class TranslationStrategy<TLeft, TLeftProperties, TRight> : IPropertyValueCollectionTranslationRule<TLeft, TLeftProperties, TRight>, ITranslationRule<TLeft, TRight>, IEnumerable<ITranslationRule<TLeft, TRight>>, IEnumerable
	{
		public TranslationStrategy(params ITranslationRule<TLeft, TRight>[] propertyTranslationRules) : this(propertyTranslationRules.ToList<ITranslationRule<TLeft, TRight>>())
		{
		}

		public TranslationStrategy(IList<ITranslationRule<TLeft, TRight>> propertyTranslationRules)
		{
			this.propertyTranslationRules = propertyTranslationRules;
		}

		public void Add(ITranslationRule<TLeft, TRight> propertyTranslationRule)
		{
			this.propertyTranslationRules.Add(propertyTranslationRule);
		}

		public void FromLeftToRightType(TLeft left, TRight right)
		{
			foreach (ITranslationRule<TLeft, TRight> translationRule in this.propertyTranslationRules)
			{
				translationRule.FromLeftToRightType(left, right);
			}
		}

		public void FromRightToLeftType(TLeft left, TRight right)
		{
			foreach (ITranslationRule<TLeft, TRight> translationRule in this.propertyTranslationRules)
			{
				translationRule.FromRightToLeftType(left, right);
			}
		}

		public void FromPropertyValues(IDictionary<TLeftProperties, int> propertyIndices, IList values, TRight right)
		{
			foreach (ITranslationRule<TLeft, TRight> translationRule in this.propertyTranslationRules)
			{
				IPropertyValueCollectionTranslationRule<TLeft, TLeftProperties, TRight> propertyValueCollectionTranslationRule = translationRule as IPropertyValueCollectionTranslationRule<TLeft, TLeftProperties, TRight>;
				if (propertyValueCollectionTranslationRule != null)
				{
					propertyValueCollectionTranslationRule.FromPropertyValues(propertyIndices, values, right);
				}
			}
		}

		public IEnumerator<ITranslationRule<TLeft, TRight>> GetEnumerator()
		{
			return this.propertyTranslationRules.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly IList<ITranslationRule<TLeft, TRight>> propertyTranslationRules;
	}
}
