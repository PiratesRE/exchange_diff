using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class PropertyErrorCorruptDataException : PropertyErrorException
	{
		internal PropertyErrorCorruptDataException(PropertyError propertyError) : base(propertyError)
		{
		}
	}
}
