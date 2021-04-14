using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[MessageContract(WrapperName = "NewCookie2Response", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", IsWrapped = true)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public class NewCookie2Response
	{
		public NewCookie2Response()
		{
		}

		public NewCookie2Response(byte[] NewCookie2Result)
		{
			this.NewCookie2Result = NewCookie2Result;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 0)]
		[XmlElement(DataType = "base64Binary", IsNullable = true)]
		public byte[] NewCookie2Result;
	}
}
