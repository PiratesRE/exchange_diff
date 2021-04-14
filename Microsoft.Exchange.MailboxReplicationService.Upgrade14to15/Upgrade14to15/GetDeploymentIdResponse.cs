using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[DataContract(Name = "GetDeploymentIdResponse", Namespace = "http://tempuri.org/")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class GetDeploymentIdResponse : IExtensibleDataObject
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
		public string GetDeploymentIdResult
		{
			get
			{
				return this.GetDeploymentIdResultField;
			}
			set
			{
				this.GetDeploymentIdResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string GetDeploymentIdResultField;
	}
}
