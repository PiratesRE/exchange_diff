using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public enum DirectoryObjectClass
	{
		Account,
		Company,
		Contact,
		Contract,
		Datacenter,
		ForeignPrincipal,
		Group,
		KeyGroup,
		Region,
		Role,
		RoleTemplate,
		Scope,
		Service,
		ServiceInstance,
		ServicePlan,
		ServicePrincipal,
		SliceInstance,
		StockKeepingUnit,
		SubscribedPlan,
		Subscription,
		Task,
		TaskSet,
		ThrottlePolicy,
		User
	}
}
