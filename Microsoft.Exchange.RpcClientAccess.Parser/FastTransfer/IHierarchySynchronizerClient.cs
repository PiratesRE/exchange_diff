using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IHierarchySynchronizerClient : IDisposable
	{
		IPropertyBag LoadFolderChanges();

		IPropertyBag LoadFolderDeletion();

		IIcsState LoadFinalState();
	}
}
