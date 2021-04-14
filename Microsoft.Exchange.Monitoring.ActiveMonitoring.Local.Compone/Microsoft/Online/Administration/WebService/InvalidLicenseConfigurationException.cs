using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "InvalidLicenseConfigurationException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class InvalidLicenseConfigurationException : InvalidParameterException
	{
		[DataMember]
		public LicenseErrorDetail[] LicenseErrors
		{
			get
			{
				return this.LicenseErrorsField;
			}
			set
			{
				this.LicenseErrorsField = value;
			}
		}

		private LicenseErrorDetail[] LicenseErrorsField;
	}
}
