using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[MessageContract(WrapperName = "MergeCookies", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", IsWrapped = true)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DebuggerStepThrough]
	public class MergeCookiesRequest
	{
		public MergeCookiesRequest()
		{
		}

		public MergeCookiesRequest(byte[] lastGetChangesCookie, byte[] lastGetContextPageToken, byte[] lastMergeCookie)
		{
			this.lastGetChangesCookie = lastGetChangesCookie;
			this.lastGetContextPageToken = lastGetContextPageToken;
			this.lastMergeCookie = lastMergeCookie;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 0)]
		[XmlElement(DataType = "base64Binary", IsNullable = true)]
		public byte[] lastGetChangesCookie;

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 1)]
		[XmlElement(DataType = "base64Binary", IsNullable = true)]
		public byte[] lastGetContextPageToken;

		[XmlElement(DataType = "base64Binary", IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 2)]
		public byte[] lastMergeCookie;
	}
}
