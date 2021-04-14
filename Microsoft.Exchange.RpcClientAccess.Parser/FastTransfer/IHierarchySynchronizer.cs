using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IHierarchySynchronizer : IDisposable
	{
		IEnumerator<IFolderChange> GetChanges();

		IPropertyBag GetDeletions();

		IIcsState GetFinalState();
	}
}
