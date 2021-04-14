using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[MessageContract(WrapperName = "GetEduFaultinStatusResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", IsWrapped = true)]
	[DebuggerStepThrough]
	public class GetEduFaultinStatusResponse
	{
		public GetEduFaultinStatusResponse()
		{
		}

		public GetEduFaultinStatusResponse(ExchangeFaultinStatus[] GetEduFaultinStatusResult)
		{
			this.GetEduFaultinStatusResult = GetEduFaultinStatusResult;
		}

		[XmlArrayItem(IsNullable = false)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", Order = 0)]
		public ExchangeFaultinStatus[] GetEduFaultinStatusResult;
	}
}
