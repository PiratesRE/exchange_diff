using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Unregister", "ComInteropTlb")]
	public sealed class UnregisterComInteropTlbTask : ComInteropTlbTaskBase
	{
		public UnregisterComInteropTlbTask() : base(false)
		{
		}
	}
}
