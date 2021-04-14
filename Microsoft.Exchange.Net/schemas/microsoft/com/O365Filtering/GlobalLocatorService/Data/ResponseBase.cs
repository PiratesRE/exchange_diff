using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[KnownType(typeof(DeleteUserResponse))]
	[KnownType(typeof(DeleteTenantResponse))]
	[KnownType(typeof(DeleteDomainResponse))]
	[KnownType(typeof(SaveDomainResponse))]
	[KnownType(typeof(SaveUserResponse))]
	[KnownType(typeof(FindUserResponse))]
	[KnownType(typeof(FindTenantResponse))]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[KnownType(typeof(FindDomainsResponse))]
	[DataContract(Name = "ResponseBase", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	[KnownType(typeof(FindDomainResponse))]
	[KnownType(typeof(SaveTenantResponse))]
	public class ResponseBase : IExtensibleDataObject
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
		public string Diagnostics
		{
			get
			{
				return this.DiagnosticsField;
			}
			set
			{
				this.DiagnosticsField = value;
			}
		}

		[DataMember]
		public string TransactionID
		{
			get
			{
				return this.TransactionIDField;
			}
			set
			{
				this.TransactionIDField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string DiagnosticsField;

		private string TransactionIDField;
	}
}
