using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class FilterNotSupportedExceptionMapping : StaticExceptionMapping
	{
		public FilterNotSupportedExceptionMapping() : base(typeof(FilterNotSupportedException), ResponseCodeType.ErrorUnsupportedPathForQuery, CoreResources.IDs.ErrorUnsupportedPathForQuery)
		{
		}

		protected override PropertyPath[] GetPropertyPaths(LocalizedException exception)
		{
			FilterNotSupportedException ex = base.VerifyExceptionType<FilterNotSupportedException>(exception);
			List<PropertyPath> list = new List<PropertyPath>();
			foreach (PropertyDefinition propertyDefinition in ex.Properties)
			{
				list.Add(SearchSchemaMap.GetPropertyPath(propertyDefinition));
			}
			return list.ToArray();
		}
	}
}
