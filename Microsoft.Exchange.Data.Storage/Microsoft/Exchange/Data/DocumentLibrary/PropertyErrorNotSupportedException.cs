using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class PropertyErrorNotSupportedException : PropertyErrorException
	{
		internal PropertyErrorNotSupportedException(PropertyError propertyError) : base(propertyError)
		{
		}
	}
}
