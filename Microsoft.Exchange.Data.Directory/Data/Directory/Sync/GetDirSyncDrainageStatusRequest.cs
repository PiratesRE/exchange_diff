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
	[MessageContract(WrapperName = "GetDirSyncDrainageStatus", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", IsWrapped = true)]
	[DebuggerStepThrough]
	public class GetDirSyncDrainageStatusRequest
	{
		public GetDirSyncDrainageStatusRequest()
		{
		}

		public GetDirSyncDrainageStatusRequest(ContextDirSyncStatus[] contextDirSyncStatusList, byte[] getChangesCookie)
		{
			this.contextDirSyncStatusList = contextDirSyncStatusList;
			this.getChangesCookie = getChangesCookie;
		}

		[XmlArrayItem(IsNullable = false)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 0)]
		[XmlArray(IsNullable = true)]
		public ContextDirSyncStatus[] contextDirSyncStatusList;

		[XmlElement(DataType = "base64Binary", IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 1)]
		public byte[] getChangesCookie;
	}
}
