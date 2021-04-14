using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "RequestIdentity", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class RequestIdentity : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public string CallerId
		{
			get
			{
				return this.CallerIdField;
			}
			set
			{
				this.CallerIdField = value;
			}
		}

		[DataMember]
		public Guid RequestTrackingGuid
		{
			get
			{
				return this.RequestTrackingGuidField;
			}
			set
			{
				this.RequestTrackingGuidField = value;
			}
		}

		[DataMember]
		public Guid TrackingGuid
		{
			get
			{
				return this.TrackingGuidField;
			}
			set
			{
				this.TrackingGuidField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string CallerIdField;

		private Guid RequestTrackingGuidField;

		private Guid TrackingGuidField;
	}
}
