using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[MessageContract(WrapperName = "NewCookie2", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", IsWrapped = true)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public class NewCookie2Request
	{
		public NewCookie2Request()
		{
		}

		public NewCookie2Request(int schemaRevision, string serviceInstance, SyncOptions options, string[] objectClassesOfInterest, string[] propertiesOfInterest, string[] linkClassesOfInterest, string[] alwaysReturnProperties)
		{
			this.schemaRevision = schemaRevision;
			this.serviceInstance = serviceInstance;
			this.options = options;
			this.objectClassesOfInterest = objectClassesOfInterest;
			this.propertiesOfInterest = propertiesOfInterest;
			this.linkClassesOfInterest = linkClassesOfInterest;
			this.alwaysReturnProperties = alwaysReturnProperties;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 0)]
		public int schemaRevision;

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 1)]
		[XmlElement(IsNullable = true)]
		public string serviceInstance;

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 2)]
		public SyncOptions options;

		[XmlArray(IsNullable = true)]
		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 3)]
		public string[] objectClassesOfInterest;

		[XmlArray(IsNullable = true)]
		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 4)]
		public string[] propertiesOfInterest;

		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
		[XmlArray(IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 5)]
		public string[] linkClassesOfInterest;

		[XmlArray(IsNullable = true)]
		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 6)]
		public string[] alwaysReturnProperties;
	}
}
