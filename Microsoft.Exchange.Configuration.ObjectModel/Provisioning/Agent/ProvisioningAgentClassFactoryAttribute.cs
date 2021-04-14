using System;

namespace Microsoft.Exchange.Provisioning.Agent
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ProvisioningAgentClassFactoryAttribute : Attribute
	{
	}
}
