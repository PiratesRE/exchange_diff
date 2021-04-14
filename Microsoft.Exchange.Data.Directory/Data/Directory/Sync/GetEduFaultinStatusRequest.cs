using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[MessageContract(WrapperName = "GetEduFaultinStatus", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", IsWrapped = true)]
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public class GetEduFaultinStatusRequest
	{
		public GetEduFaultinStatusRequest()
		{
		}

		public GetEduFaultinStatusRequest(string[] contextIds)
		{
			this.contextIds = contextIds;
		}

		[XmlArrayItem("guid", Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", Order = 0)]
		[XmlArray(IsNullable = true)]
		public string[] contextIds;
	}
}
