using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[KnownType(typeof(ListRoleMemberResults))]
	[KnownType(typeof(ListServicePrincipalResults))]
	[DataContract(Name = "ListResults", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[KnownType(typeof(ListGroupMemberResults))]
	[KnownType(typeof(ListGroupResults))]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[KnownType(typeof(ListUserResults))]
	[KnownType(typeof(ListPartnerContractResults))]
	[KnownType(typeof(ListContactResults))]
	public class ListResults : IExtensibleDataObject
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
		public bool IsFirstPage
		{
			get
			{
				return this.IsFirstPageField;
			}
			set
			{
				this.IsFirstPageField = value;
			}
		}

		[DataMember]
		public bool IsLastPage
		{
			get
			{
				return this.IsLastPageField;
			}
			set
			{
				this.IsLastPageField = value;
			}
		}

		[DataMember]
		public byte[] ListContext
		{
			get
			{
				return this.ListContextField;
			}
			set
			{
				this.ListContextField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private bool IsFirstPageField;

		private bool IsLastPageField;

		private byte[] ListContextField;
	}
}
