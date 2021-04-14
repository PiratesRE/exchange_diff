using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.Cafe
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class BackEndLocatorException : ServerNotFoundException
	{
		public BackEndLocatorException(Exception innerException) : base(innerException.Message, innerException)
		{
		}
	}
}
