using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class UnsupportedPathForQueryException : ServicePermanentExceptionWithPropertyPath
	{
		public UnsupportedPathForQueryException(PropertyPath propertyPath) : base(CoreResources.IDs.ErrorUnsupportedPathForQuery, propertyPath)
		{
		}

		public UnsupportedPathForQueryException(PropertyDefinition propertyDefinition, Exception innerException) : base(CoreResources.IDs.ErrorUnsupportedPathForQuery, SearchSchemaMap.GetPropertyPath(propertyDefinition), innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}
	}
}
