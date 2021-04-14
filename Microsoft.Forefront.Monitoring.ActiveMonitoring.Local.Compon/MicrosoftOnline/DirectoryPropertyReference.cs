using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlInclude(typeof(DirectoryPropertyReferenceAnySingle))]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[XmlInclude(typeof(DirectoryPropertyReferenceAddressList))]
	[XmlInclude(typeof(DirectoryPropertyReferenceServicePlan))]
	[XmlInclude(typeof(DirectoryPropertyReferenceContact))]
	[XmlInclude(typeof(DirectoryPropertyReferenceContactSingle))]
	[XmlInclude(typeof(DirectoryPropertyReferenceAny))]
	[XmlInclude(typeof(DirectoryPropertyReferenceAddressListSingle))]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public abstract class DirectoryPropertyReference : DirectoryProperty
	{
	}
}
