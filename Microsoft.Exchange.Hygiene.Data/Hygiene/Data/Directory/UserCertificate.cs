using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class UserCertificate : ConfigurablePropertyBag
	{
		public UserCertificate(Guid userId, string certSubjectName, string certIssuerName)
		{
			if (userId.Equals(Guid.Empty))
			{
				throw new ArgumentException("userId");
			}
			if (string.IsNullOrWhiteSpace(certSubjectName))
			{
				throw new ArgumentNullException("certSubjectName");
			}
			if (certSubjectName.Length >= 1024)
			{
				throw new ArgumentException("Certificate Subject Name cannot be greater than or equal to 1024 characters");
			}
			if (string.IsNullOrWhiteSpace(certIssuerName))
			{
				throw new ArgumentNullException("certIssuerName");
			}
			if (certIssuerName.Length >= 1024)
			{
				throw new ArgumentException("Certificate Issuer Name cannot be greater than or equal to 1024 characters");
			}
			this.UserId = userId;
			this.CertificateSubjectName = certSubjectName;
			this.CertificateIssuerName = certIssuerName;
		}

		public Guid UserId
		{
			get
			{
				return (Guid)this[UserCertificate.UserIdProp];
			}
			private set
			{
				this[UserCertificate.UserIdProp] = value;
			}
		}

		public string CertificateSubjectName
		{
			get
			{
				return this[UserCertificate.CertificateSubjectNameProp] as string;
			}
			private set
			{
				this[UserCertificate.CertificateSubjectNameProp] = value;
			}
		}

		public string CertificateIssuerName
		{
			get
			{
				return this[UserCertificate.CertificateIssuerNameProp] as string;
			}
			private set
			{
				this[UserCertificate.CertificateIssuerNameProp] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId((Guid)this[UserCertificate.UserIdProp] + (string)this[UserCertificate.CertificateIssuerNameProp]);
			}
		}

		internal static readonly HygienePropertyDefinition UserIdProp = ADMiniUserSchema.UserIdProp;

		internal static readonly HygienePropertyDefinition UserCertificateIdProp = new HygienePropertyDefinition("userCertificateId", typeof(Guid));

		internal static readonly HygienePropertyDefinition CertificateIdProp = new HygienePropertyDefinition("CertificateId", typeof(Guid));

		internal static readonly HygienePropertyDefinition CertificateSubjectNameProp = new HygienePropertyDefinition("certificateSubjectName", typeof(string));

		internal static readonly HygienePropertyDefinition CertificateIssuerNameProp = new HygienePropertyDefinition("certificateIssuerName", typeof(string));
	}
}
