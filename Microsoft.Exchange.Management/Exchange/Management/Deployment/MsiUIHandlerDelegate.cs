using System;

namespace Microsoft.Exchange.Management.Deployment
{
	internal delegate int MsiUIHandlerDelegate(IntPtr context, uint messageType, string message);
}
