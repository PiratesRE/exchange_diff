using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "GetTenantApplicationDataResponse", Namespace = "http://tempuri.org/")]
	public class GetTenantApplicationDataResponse : IExtensibleDataObject
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
		public XmlElement GetTenantApplicationDataResult
		{
			get
			{
				return this.GetTenantApplicationDataResultField;
			}
			set
			{
				this.GetTenantApplicationDataResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private XmlElement GetTenantApplicationDataResultField;
	}
}
