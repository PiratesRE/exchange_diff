using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[MessageContract(WrapperName = "GetDirSyncDrainageStatusResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", IsWrapped = true)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public class GetDirSyncDrainageStatusResponse
	{
		public GetDirSyncDrainageStatusResponse()
		{
		}

		public GetDirSyncDrainageStatusResponse(DirSyncDrainageCode[] GetDirSyncDrainageStatusResult)
		{
			this.GetDirSyncDrainageStatusResult = GetDirSyncDrainageStatusResult;
		}

		[XmlArray(IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 0)]
		public DirSyncDrainageCode[] GetDirSyncDrainageStatusResult;
	}
}
