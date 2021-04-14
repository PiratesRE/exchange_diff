using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ILocationIdentifierSetter
	{
		void SetLocationIdentifier(uint id);

		void SetLocationIdentifier(uint id, LastChangeAction action);
	}
}
