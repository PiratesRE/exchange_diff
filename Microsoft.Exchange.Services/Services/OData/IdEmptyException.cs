using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData
{
	internal class IdEmptyException : InvalidIdException
	{
		public IdEmptyException() : base(ResponseCodeType.ErrorInvalidIdEmpty, CoreResources.ErrorInvalidIdEmpty)
		{
		}

		public IdEmptyException(LocalizedString errorMessage) : base(ResponseCodeType.ErrorInvalidIdEmpty, errorMessage)
		{
		}
	}
}
