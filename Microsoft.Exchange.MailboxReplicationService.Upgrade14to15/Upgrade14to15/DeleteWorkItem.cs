﻿using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "DeleteWorkItem", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class DeleteWorkItem : IExtensibleDataObject
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
		public string workItemId
		{
			get
			{
				return this.workItemIdField;
			}
			set
			{
				this.workItemIdField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string workItemIdField;
	}
}
