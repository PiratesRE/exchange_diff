using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[KnownType(typeof(RoleMemberSearchDefinition))]
	[KnownType(typeof(UserSearchDefinition))]
	[KnownType(typeof(ServicePrincipalSearchDefinition))]
	[DebuggerStepThrough]
	[DataContract(Name = "SearchDefinition", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[KnownType(typeof(PartnerContractSearchDefinition))]
	[KnownType(typeof(ContactSearchDefinition))]
	[KnownType(typeof(GroupSearchDefinition))]
	[KnownType(typeof(GroupMemberSearchDefinition))]
	public class SearchDefinition : IExtensibleDataObject
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
		public int PageSize
		{
			get
			{
				return this.PageSizeField;
			}
			set
			{
				this.PageSizeField = value;
			}
		}

		[DataMember]
		public string SearchString
		{
			get
			{
				return this.SearchStringField;
			}
			set
			{
				this.SearchStringField = value;
			}
		}

		[DataMember]
		public SortDirection SortDirection
		{
			get
			{
				return this.SortDirectionField;
			}
			set
			{
				this.SortDirectionField = value;
			}
		}

		[DataMember]
		public SortField SortField
		{
			get
			{
				return this.SortFieldField;
			}
			set
			{
				this.SortFieldField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private int PageSizeField;

		private string SearchStringField;

		private SortDirection SortDirectionField;

		private SortField SortFieldField;
	}
}
