using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class UnsupportedPropertyForSortGroupExceptionMapping : StaticExceptionMapping
	{
		public UnsupportedPropertyForSortGroupExceptionMapping() : base(typeof(UnsupportedPropertyForSortGroupException), ResponseCodeType.ErrorUnsupportedPathForSortGroup, CoreResources.IDs.ErrorUnsupportedPathForSortGroup)
		{
		}

		protected override PropertyPath[] GetPropertyPaths(LocalizedException exception)
		{
			UnsupportedPropertyForSortGroupException ex = base.VerifyExceptionType<UnsupportedPropertyForSortGroupException>(exception);
			return new List<PropertyPath>
			{
				SearchSchemaMap.GetPropertyPath(ex.UnsupportedProperty)
			}.ToArray();
		}
	}
}
