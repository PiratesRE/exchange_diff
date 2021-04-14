using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFolderChange : IDisposable
	{
		IPropertyBag FolderPropertyBag { get; }
	}
}
