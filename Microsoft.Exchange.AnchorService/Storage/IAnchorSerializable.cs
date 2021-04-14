using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAnchorSerializable
	{
		PropertyDefinition[] PropertyDefinitions { get; }

		bool ReadFromMessageItem(IAnchorStoreObject message);

		void WriteToMessageItem(IAnchorStoreObject message, bool loaded);
	}
}
