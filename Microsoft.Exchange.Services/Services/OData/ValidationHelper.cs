using System;
using System.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.OData
{
	internal static class ValidationHelper
	{
		public static void ValidateIdEmpty(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new IdEmptyException();
			}
		}

		public static void ValidateParameterEmpty(string parameterName, object parameterValue)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("parameterName", parameterName);
			if (parameterValue == null)
			{
				throw new ParameterValueEmptyException(parameterName);
			}
			string text = parameterValue as string;
			if (text != null && string.IsNullOrEmpty(text))
			{
				throw new ParameterValueEmptyException(parameterName);
			}
			ICollection collection = parameterValue as ICollection;
			if (collection != null && collection.Count == 0)
			{
				throw new ParameterValueEmptyException(parameterName);
			}
		}
	}
}
