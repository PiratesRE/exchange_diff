using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Unregister", "ContentFilter")]
	public sealed class UnregisterContentFilter : ContentFilterRegistration
	{
		public UnregisterContentFilter() : base(false)
		{
		}
	}
}
