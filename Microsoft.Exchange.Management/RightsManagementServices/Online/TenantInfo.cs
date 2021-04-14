using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.RightsManagementServices.Online
{
	[DebuggerStepThrough]
	[DataContract(Name = "TenantInfo", Namespace = "http://microsoft.com/RightsManagementServiceOnline/2011/04")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class TenantInfo : TenantEnrollmentInfo
	{
		[DataMember]
		public TenantStatus Status
		{
			get
			{
				return this.StatusField;
			}
			set
			{
				this.StatusField = value;
			}
		}

		[DataMember(Order = 1)]
		public TrustedDocDomain ActivePublishingDomain
		{
			get
			{
				return this.ActivePublishingDomainField;
			}
			set
			{
				this.ActivePublishingDomainField = value;
			}
		}

		[DataMember(Order = 2)]
		public TrustedDocDomain[] ArchivedPublishingDomains
		{
			get
			{
				return this.ArchivedPublishingDomainsField;
			}
			set
			{
				this.ArchivedPublishingDomainsField = value;
			}
		}

		[DataMember(Order = 3)]
		public CommonFault ErrorInfo
		{
			get
			{
				return this.ErrorInfoField;
			}
			set
			{
				this.ErrorInfoField = value;
			}
		}

		[DataMember(Order = 4)]
		public Uri LicensingIntranetDistributionPointUrl
		{
			get
			{
				return this.LicensingIntranetDistributionPointUrlField;
			}
			set
			{
				this.LicensingIntranetDistributionPointUrlField = value;
			}
		}

		[DataMember(Order = 5)]
		public Uri LicensingExtranetDistributionPointUrl
		{
			get
			{
				return this.LicensingExtranetDistributionPointUrlField;
			}
			set
			{
				this.LicensingExtranetDistributionPointUrlField = value;
			}
		}

		[DataMember(Order = 6)]
		public Uri CertificationIntranetDistributionPointUrl
		{
			get
			{
				return this.CertificationIntranetDistributionPointUrlField;
			}
			set
			{
				this.CertificationIntranetDistributionPointUrlField = value;
			}
		}

		[DataMember(Order = 7)]
		public Uri CertificationExtranetDistributionPointUrl
		{
			get
			{
				return this.CertificationExtranetDistributionPointUrlField;
			}
			set
			{
				this.CertificationExtranetDistributionPointUrlField = value;
			}
		}

		private TenantStatus StatusField;

		private TrustedDocDomain ActivePublishingDomainField;

		private TrustedDocDomain[] ArchivedPublishingDomainsField;

		private CommonFault ErrorInfoField;

		private Uri LicensingIntranetDistributionPointUrlField;

		private Uri LicensingExtranetDistributionPointUrlField;

		private Uri CertificationIntranetDistributionPointUrlField;

		private Uri CertificationExtranetDistributionPointUrlField;
	}
}
