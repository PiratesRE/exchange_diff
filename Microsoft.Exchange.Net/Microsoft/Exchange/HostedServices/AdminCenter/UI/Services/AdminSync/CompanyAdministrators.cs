using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DataContract(Name = "CompanyAdministrators", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[Serializable]
	internal class CompanyAdministrators : IExtensibleDataObject
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
		internal Dictionary<Role, Guid[]> AdminGroups
		{
			get
			{
				return this.AdminGroupsField;
			}
			set
			{
				this.AdminGroupsField = value;
			}
		}

		[DataMember]
		internal Dictionary<Role, string[]> AdminUsers
		{
			get
			{
				return this.AdminUsersField;
			}
			set
			{
				this.AdminUsersField = value;
			}
		}

		[DataMember]
		internal int CompanyId
		{
			get
			{
				return this.CompanyIdField;
			}
			set
			{
				this.CompanyIdField = value;
			}
		}

		[NonSerialized]
		private ExtensionDataObject extensionDataField;

		[OptionalField]
		private Dictionary<Role, Guid[]> AdminGroupsField;

		[OptionalField]
		private Dictionary<Role, string[]> AdminUsersField;

		[OptionalField]
		private int CompanyIdField;
	}
}
