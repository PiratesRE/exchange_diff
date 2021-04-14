using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[KnownType(typeof(InvalidUserLicenseException))]
	[KnownType(typeof(TenantNotPartnerTypeException))]
	[KnownType(typeof(LicenseQuotaExceededException))]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "InvalidParameterException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[KnownType(typeof(InvalidLicenseConfigurationException))]
	[KnownType(typeof(InvalidUserLicenseOptionException))]
	[KnownType(typeof(InvalidSubscriptionStatusException))]
	public class InvalidParameterException : MsolAdministrationException
	{
		[DataMember]
		public string ParameterName
		{
			get
			{
				return this.ParameterNameField;
			}
			set
			{
				this.ParameterNameField = value;
			}
		}

		private string ParameterNameField;
	}
}
