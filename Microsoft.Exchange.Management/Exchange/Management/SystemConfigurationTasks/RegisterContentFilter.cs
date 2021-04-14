using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Register", "ContentFilter")]
	public sealed class RegisterContentFilter : ContentFilterRegistration
	{
		public RegisterContentFilter() : base(true)
		{
		}
	}
}
