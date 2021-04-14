using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Management.Deployment
{
	internal delegate void ManagedLoggerDelegate([MarshalAs(UnmanagedType.LPWStr)] string str);
}
