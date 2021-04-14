using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[MessageContract(WrapperName = "GetDirectoryObjectsResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", IsWrapped = true)]
	[DebuggerStepThrough]
	public class GetDirectoryObjectsResponse
	{
		public GetDirectoryObjectsResponse()
		{
		}

		public GetDirectoryObjectsResponse(DirectoryObjectsAndLinks GetDirectoryObjectsResult)
		{
			this.GetDirectoryObjectsResult = GetDirectoryObjectsResult;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 0)]
		[XmlElement(IsNullable = true)]
		public DirectoryObjectsAndLinks GetDirectoryObjectsResult;
	}
}
