using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class UnsupportedPropertyDefinitionException : ServicePermanentException
	{
		public UnsupportedPropertyDefinitionException(string propertyDefinition) : base(CoreResources.IDs.ErrorUnsupportedPropertyDefinition)
		{
			base.ConstantValues.Add("PropertyDefinition", propertyDefinition);
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}

		private const string PropertyDefinitionKey = "PropertyDefinition";
	}
}
