using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FxProxyAttachWrapper : FxProxyWrapper, IAttach, IMAPIProp
	{
		internal FxProxyAttachWrapper(IMapiFxCollector iFxCollector) : base(iFxCollector)
		{
		}
	}
}
