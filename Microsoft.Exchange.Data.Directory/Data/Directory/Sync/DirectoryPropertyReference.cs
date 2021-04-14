using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlInclude(typeof(DirectoryPropertyReferenceUserAndServicePrincipal))]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[XmlInclude(typeof(DirectoryPropertyReferenceUserAndServicePrincipalSingle))]
	[XmlInclude(typeof(DirectoryPropertyReferenceAddressList))]
	[DebuggerStepThrough]
	[XmlInclude(typeof(DirectoryPropertyReferenceAddressListSingle))]
	[XmlInclude(typeof(DirectoryPropertyReferenceAny))]
	[XmlInclude(typeof(DirectoryPropertyReferenceAnySingle))]
	[Serializable]
	public abstract class DirectoryPropertyReference : DirectoryProperty
	{
	}
}
