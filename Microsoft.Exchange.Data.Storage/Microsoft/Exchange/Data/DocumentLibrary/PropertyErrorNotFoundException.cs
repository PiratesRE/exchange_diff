using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class PropertyErrorNotFoundException : PropertyErrorException
	{
		internal PropertyErrorNotFoundException(PropertyError propertyError) : base(propertyError)
		{
		}
	}
}
