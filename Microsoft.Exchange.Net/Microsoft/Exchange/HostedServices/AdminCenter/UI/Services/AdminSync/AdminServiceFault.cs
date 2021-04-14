using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync
{
	[KnownType(typeof(InvalidCompanyFault))]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DataContract(Name = "AdminServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[KnownType(typeof(InvalidContractFault))]
	[DebuggerStepThrough]
	[KnownType(typeof(InternalFault))]
	[KnownType(typeof(AuthorizationFault))]
	[KnownType(typeof(InvalidGroupFault))]
	[KnownType(typeof(RemoveGroupErrorInfo))]
	[KnownType(typeof(InvalidUserFault))]
	[KnownType(typeof(ErrorInfo))]
	[Serializable]
	internal class AdminServiceFault : IExtensibleDataObject
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
		internal Dictionary<string, string> Data
		{
			get
			{
				return this.DataField;
			}
			set
			{
				this.DataField = value;
			}
		}

		[DataMember]
		internal ErrorType ErrorType
		{
			get
			{
				return this.ErrorTypeField;
			}
			set
			{
				this.ErrorTypeField = value;
			}
		}

		[NonSerialized]
		private ExtensionDataObject extensionDataField;

		[OptionalField]
		private Dictionary<string, string> DataField;

		[OptionalField]
		private ErrorType ErrorTypeField;
	}
}
