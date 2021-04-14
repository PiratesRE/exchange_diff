using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IModifyTableRestriction
	{
		void Enforce(IModifyTable modifyTable, IEnumerable<ModifyTableOperation> changingEntries);
	}
}
