using System;
using System.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchObjectPropertyBag : ADPropertyBag
	{
		public SearchObjectPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
		}

		public SearchObjectPropertyBag() : base(false, 16)
		{
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return SearchObjectBaseSchema.ExchangeVersion;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return SearchObjectBaseSchema.ObjectState;
			}
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return SearchObjectBaseSchema.Id;
			}
		}

		internal override MultiValuedPropertyBase CreateMultiValuedProperty(ProviderPropertyDefinition propertyDefinition, bool createAsReadOnly, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage)
		{
			if (propertyDefinition.Type == typeof(KindKeyword))
			{
				return new MultiValuedProperty<KindKeyword>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			return ADValueConvertor.CreateGenericMultiValuedProperty(propertyDefinition, createAsReadOnly, values, invalidValues, readOnlyErrorMessage);
		}

		internal override object SerializeData(ProviderPropertyDefinition propertyDefinition, object input)
		{
			if (typeof(SearchObjectId) == propertyDefinition.Type)
			{
				return input;
			}
			return base.SerializeData(propertyDefinition, input);
		}

		internal override object DeserializeData(ProviderPropertyDefinition propertyDefinition, object input)
		{
			if (typeof(SearchObjectId) == propertyDefinition.Type)
			{
				return input;
			}
			return base.DeserializeData(propertyDefinition, input);
		}
	}
}
