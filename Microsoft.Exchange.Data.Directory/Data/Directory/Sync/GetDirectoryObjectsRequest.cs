using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[MessageContract(WrapperName = "GetDirectoryObjects", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", IsWrapped = true)]
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public class GetDirectoryObjectsRequest
	{
		public GetDirectoryObjectsRequest()
		{
		}

		public GetDirectoryObjectsRequest(byte[] lastGetChangesCookieOrGetContextPageToken, DirectoryObjectIdentity[] objectIdentities, GetDirectoryObjectsOptions? options, byte[] lastPageToken)
		{
			this.lastGetChangesCookieOrGetContextPageToken = lastGetChangesCookieOrGetContextPageToken;
			this.objectIdentities = objectIdentities;
			this.options = options;
			this.lastPageToken = lastPageToken;
		}

		[XmlElement(DataType = "base64Binary", IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 0)]
		public byte[] lastGetChangesCookieOrGetContextPageToken;

		[XmlArrayItem(IsNullable = false)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 1)]
		[XmlArray(IsNullable = true)]
		public DirectoryObjectIdentity[] objectIdentities;

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 2)]
		[XmlElement(IsNullable = true)]
		public GetDirectoryObjectsOptions? options;

		[XmlElement(DataType = "base64Binary", IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 3)]
		public byte[] lastPageToken;
	}
}
