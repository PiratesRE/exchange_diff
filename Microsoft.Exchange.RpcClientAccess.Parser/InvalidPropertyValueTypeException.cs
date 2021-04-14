using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InvalidPropertyValueTypeException : ArgumentException
	{
		public InvalidPropertyValueTypeException(string message, string propertyName) : base(message, propertyName)
		{
		}
	}
}
