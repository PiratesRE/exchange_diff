using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "LicenseErrorDetail", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class LicenseErrorDetail : IExtensibleDataObject
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
		public string[] DependsOnServicePlans
		{
			get
			{
				return this.DependsOnServicePlansField;
			}
			set
			{
				this.DependsOnServicePlansField = value;
			}
		}

		[DataMember]
		public LicenseErrorType ErrorType
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

		[DataMember]
		public bool IsWarning
		{
			get
			{
				return this.IsWarningField;
			}
			set
			{
				this.IsWarningField = value;
			}
		}

		[DataMember]
		public string[] ServicePlans
		{
			get
			{
				return this.ServicePlansField;
			}
			set
			{
				this.ServicePlansField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string[] DependsOnServicePlansField;

		private LicenseErrorType ErrorTypeField;

		private bool IsWarningField;

		private string[] ServicePlansField;
	}
}
