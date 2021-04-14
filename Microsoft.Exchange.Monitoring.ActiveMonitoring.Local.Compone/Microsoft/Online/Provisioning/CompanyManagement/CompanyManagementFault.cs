using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Provisioning.CompanyManagement
{
	[KnownType(typeof(InvalidServiceInstanceFault))]
	[KnownType(typeof(ServiceUnavailableFault))]
	[KnownType(typeof(CompanyNotFoundFault))]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "CompanyManagementFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Provisioning.CompanyManagement")]
	[KnownType(typeof(BindingRedirectionFault))]
	[KnownType(typeof(DifferentServiceInstanceAlreadyExistsFault))]
	public class CompanyManagementFault : IExtensibleDataObject
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

		[DataMember(IsRequired = true)]
		public TimeSpan BackOffDuration
		{
			get
			{
				return this.BackOffDurationField;
			}
			set
			{
				this.BackOffDurationField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public bool CanRetry
		{
			get
			{
				return this.CanRetryField;
			}
			set
			{
				this.CanRetryField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private TimeSpan BackOffDurationField;

		private bool CanRetryField;
	}
}
