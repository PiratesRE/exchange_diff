using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal interface IPropertyLookup
	{
		ADRawEntry GetProperties(ADObjectId id);
	}
}
