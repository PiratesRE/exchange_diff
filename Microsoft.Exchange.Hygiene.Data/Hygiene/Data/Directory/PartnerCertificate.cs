using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class PartnerCertificate : ConfigurablePropertyBag
	{
		public PartnerCertificate()
		{
		}

		internal PartnerCertificate(string callerId, string certSubjectName)
		{
			if (string.IsNullOrEmpty(callerId))
			{
				throw new ArgumentNullException("callerId");
			}
			if (string.IsNullOrEmpty(certSubjectName))
			{
				throw new ArgumentNullException("certSubjectName");
			}
			this.PartnerCallerId = callerId;
			this.CertificateSubjectName = certSubjectName;
		}

		internal PartnerCertificate(string callerId, string partnerName, string certSubjectName) : this(callerId, certSubjectName)
		{
			if (string.IsNullOrEmpty(partnerName))
			{
				throw new ArgumentNullException("partnerName");
			}
			this.PartnerName = partnerName;
		}

		public string PartnerCallerId
		{
			get
			{
				return this[PartnerCertificate.PartnerCallerIdDef] as string;
			}
			internal set
			{
				this[PartnerCertificate.PartnerCallerIdDef] = value;
			}
		}

		public string PartnerName
		{
			get
			{
				return this[PartnerCertificate.PartnerNameDef] as string;
			}
			internal set
			{
				this[PartnerCertificate.PartnerNameDef] = value;
			}
		}

		public string CertificateSubjectName
		{
			get
			{
				return this[PartnerCertificate.CertificateSubjectNameDef] as string;
			}
			internal set
			{
				this[PartnerCertificate.CertificateSubjectNameDef] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId((string)this[PartnerCertificate.PartnerCallerIdDef] + (string)this[PartnerCertificate.CertificateSubjectNameDef]);
			}
		}

		internal static readonly HygienePropertyDefinition PartnerCallerIdDef = new HygienePropertyDefinition("callerId", typeof(string));

		internal static readonly HygienePropertyDefinition PartnerCertificateIdDef = new HygienePropertyDefinition("partnerCertificateId", typeof(Guid));

		internal static readonly HygienePropertyDefinition PartnerIdDef = new HygienePropertyDefinition("partnerId", typeof(Guid));

		internal static readonly HygienePropertyDefinition PartnerNameDef = new HygienePropertyDefinition("partnerName", typeof(string));

		internal static readonly HygienePropertyDefinition CertificateIdDef = new HygienePropertyDefinition("certificateId", typeof(Guid));

		internal static readonly HygienePropertyDefinition CertificateSubjectNameDef = new HygienePropertyDefinition("certificateSubjectName", typeof(string));
	}
}
