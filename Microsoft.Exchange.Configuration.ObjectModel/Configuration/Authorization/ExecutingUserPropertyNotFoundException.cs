using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class ExecutingUserPropertyNotFoundException : Exception
	{
		public ExecutingUserPropertyNotFoundException(string propertyName) : base(Strings.ExecutingUserPropertyNotFound(propertyName))
		{
		}
	}
}
