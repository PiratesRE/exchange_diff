using System;

namespace Microsoft.Exchange.Services.OData
{
	internal class ParameterValueEmptyException : InvalidParameterException
	{
		public ParameterValueEmptyException(string parameterName) : base(parameterName, CoreResources.ErrorParameterValueEmpty(parameterName))
		{
		}
	}
}
