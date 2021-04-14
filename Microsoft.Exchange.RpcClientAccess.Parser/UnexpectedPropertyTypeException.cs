using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UnexpectedPropertyTypeException : RopExecutionException
	{
		public UnexpectedPropertyTypeException(Type expectedType, PropertyValue sourcePropertyValue) : this(expectedType, sourcePropertyValue, null)
		{
		}

		public UnexpectedPropertyTypeException(Type expectedType, PropertyValue sourcePropertyValue, Exception innerException) : base(UnexpectedPropertyTypeException.GetErrorMessage(expectedType, sourcePropertyValue), (ErrorCode)2147942487U, innerException)
		{
		}

		private static string GetErrorMessage(Type expectedType, PropertyValue sourcePropertyValue)
		{
			return string.Format("Unable to cast {0} to {1}", sourcePropertyValue, expectedType);
		}
	}
}
