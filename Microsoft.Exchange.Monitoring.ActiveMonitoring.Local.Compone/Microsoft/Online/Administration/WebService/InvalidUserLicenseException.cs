using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[KnownType(typeof(LicenseQuotaExceededException))]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "InvalidUserLicenseException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[KnownType(typeof(InvalidSubscriptionStatusException))]
	public class InvalidUserLicenseException : InvalidParameterException
	{
		[DataMember]
		public string Account
		{
			get
			{
				return this.AccountField;
			}
			set
			{
				this.AccountField = value;
			}
		}

		[DataMember]
		public string Sku
		{
			get
			{
				return this.SkuField;
			}
			set
			{
				this.SkuField = value;
			}
		}

		[DataMember]
		public Guid? SubscriptionId
		{
			get
			{
				return this.SubscriptionIdField;
			}
			set
			{
				this.SubscriptionIdField = value;
			}
		}

		private string AccountField;

		private string SkuField;

		private Guid? SubscriptionIdField;
	}
}
