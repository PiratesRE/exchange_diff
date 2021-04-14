using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync
{
	[DataContract(Name = "FailedAdminAccounts", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[Serializable]
	internal class FailedAdminAccounts : IExtensibleDataObject
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
		internal Dictionary<Guid, ErrorInfo> FailedGroups
		{
			get
			{
				return this.FailedGroupsField;
			}
			set
			{
				this.FailedGroupsField = value;
			}
		}

		[DataMember]
		internal Dictionary<string, ErrorInfo> FailedUsers
		{
			get
			{
				return this.FailedUsersField;
			}
			set
			{
				this.FailedUsersField = value;
			}
		}

		[NonSerialized]
		private ExtensionDataObject extensionDataField;

		[OptionalField]
		private Dictionary<Guid, ErrorInfo> FailedGroupsField;

		[OptionalField]
		private Dictionary<string, ErrorInfo> FailedUsersField;
	}
}
