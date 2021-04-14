using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[Flags]
	internal enum PropertyDefinitionFlags
	{
		None = 0,
		Navigation = 1,
		CanFilter = 2,
		CanCreate = 8,
		CanUpdate = 16,
		ChildOnlyEntity = 32
	}
}
