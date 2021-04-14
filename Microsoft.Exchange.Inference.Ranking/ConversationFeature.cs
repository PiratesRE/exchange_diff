using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Inference.Ranking
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationFeature : IRankingFeature
	{
		internal ConversationFeature(IList<PropertyDefinition> supportingProperties, FeatureValueCalculator calculator)
		{
			this.supportingProperties = supportingProperties;
			this.calculator = calculator;
		}

		public IList<PropertyDefinition> SupportingProperties
		{
			get
			{
				return this.supportingProperties;
			}
		}

		public double FeatureValue(object conversation)
		{
			return this.calculator((IStorePropertyBag)conversation);
		}

		private readonly IList<PropertyDefinition> supportingProperties;

		private readonly FeatureValueCalculator calculator;
	}
}
