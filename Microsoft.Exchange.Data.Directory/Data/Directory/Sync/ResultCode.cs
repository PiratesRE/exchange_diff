using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11")]
	[Serializable]
	public enum ResultCode
	{
		Success,
		PartitionUnavailable,
		ObjectNotFound,
		UnspecifiedError
	}
}
