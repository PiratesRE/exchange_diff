using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "GetUserApplicationDataResponse", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class GetUserApplicationDataResponse : IExtensibleDataObject
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

		[DataMember]
		public XmlElement GetUserApplicationDataResult
		{
			get
			{
				return this.GetUserApplicationDataResultField;
			}
			set
			{
				this.GetUserApplicationDataResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private XmlElement GetUserApplicationDataResultField;
	}
}
