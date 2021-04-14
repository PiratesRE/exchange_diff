using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[MessageContract(WrapperName = "UpdateCookie", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", IsWrapped = true)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public class UpdateCookieRequest
	{
		public UpdateCookieRequest()
		{
		}

		public UpdateCookieRequest(byte[] getChangesCookie, int? schemaRevision, SyncOptions? options, string[] objectClassesOfInterest, string[] propertiesOfInterest, string[] linkClassesOfInterest, string[] alwaysReturnProperties)
		{
			this.getChangesCookie = getChangesCookie;
			this.schemaRevision = schemaRevision;
			this.options = options;
			this.objectClassesOfInterest = objectClassesOfInterest;
			this.propertiesOfInterest = propertiesOfInterest;
			this.linkClassesOfInterest = linkClassesOfInterest;
			this.alwaysReturnProperties = alwaysReturnProperties;
		}

		[XmlElement(DataType = "base64Binary", IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 0)]
		public byte[] getChangesCookie;

		[XmlElement(IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 1)]
		public int? schemaRevision;

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 2)]
		[XmlElement(IsNullable = true)]
		public SyncOptions? options;

		[XmlArray(IsNullable = true)]
		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 3)]
		public string[] objectClassesOfInterest;

		[XmlArray(IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 4)]
		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
		public string[] propertiesOfInterest;

		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 5)]
		[XmlArray(IsNullable = true)]
		public string[] linkClassesOfInterest;

		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
		[XmlArray(IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 6)]
		public string[] alwaysReturnProperties;
	}
}
