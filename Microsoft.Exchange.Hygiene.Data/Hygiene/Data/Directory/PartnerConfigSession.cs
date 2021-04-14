using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class PartnerConfigSession
	{
		public PartnerConfigSession(string callerId)
		{
			if (string.IsNullOrEmpty(callerId))
			{
				throw new ArgumentNullException("callerId");
			}
			this.callerId = callerId;
		}

		internal PartnerConfigSession(string callerId, string partnerName) : this(callerId)
		{
			if (string.IsNullOrEmpty(partnerName))
			{
				throw new ArgumentNullException("partnerName");
			}
			this.partnerName = partnerName;
		}

		public static IEnumerable<PartnerCertificate> FindAllPartnerCertificatesForAllCallerIds()
		{
			IConfigurable[] source = PartnerConfigSession.dataProvider.Find<PartnerCertificate>(null, null, false, null);
			return source.Cast<PartnerCertificate>();
		}

		public static IEnumerable<PartnerRole> FindAllPartnerRolesForAllCallerIds()
		{
			IConfigurable[] source = PartnerConfigSession.dataProvider.Find<PartnerRole>(null, null, false, null);
			return source.Cast<PartnerRole>();
		}

		public void SavePartnerCertificate(string certificateSubjectName)
		{
			if (string.IsNullOrEmpty(certificateSubjectName))
			{
				throw new ArgumentNullException("certificateSubjectName");
			}
			PartnerCertificate partnerCertificate = new PartnerCertificate(this.callerId, certificateSubjectName);
			if (string.IsNullOrEmpty(this.partnerName))
			{
				partnerCertificate.PartnerName = this.partnerName;
			}
			ConfigurablePropertyBag configurablePropertyBag = partnerCertificate;
			if (configurablePropertyBag != null)
			{
				configurablePropertyBag[PartnerCertificate.PartnerCertificateIdDef] = this.GenerateNewGuid();
				configurablePropertyBag[PartnerCertificate.PartnerIdDef] = this.GenerateNewGuid();
				configurablePropertyBag[PartnerCertificate.CertificateIdDef] = this.GenerateNewGuid();
			}
			PartnerConfigSession.dataProvider.Save(partnerCertificate);
		}

		public void DeletePartnerCertificate(string certificateSubjectName)
		{
			if (string.IsNullOrEmpty(certificateSubjectName))
			{
				throw new ArgumentNullException("certificateSubjectName");
			}
			PartnerConfigSession.dataProvider.Delete(new PartnerCertificate(this.callerId, certificateSubjectName));
		}

		public void SavePartnerRole(string roleName)
		{
			if (string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException("roleName");
			}
			PartnerConfigSession.dataProvider.Save(new PartnerRole(this.callerId, roleName));
		}

		public void DeletePartnerRole(string roleName)
		{
			if (string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException("roleName");
			}
			PartnerConfigSession.dataProvider.Delete(new PartnerRole(this.callerId, roleName));
		}

		public IEnumerable<PartnerCertificate> FindPartnerCertificates()
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, PartnerCertificate.PartnerCallerIdDef, this.callerId);
			IConfigurable[] source = PartnerConfigSession.dataProvider.Find<PartnerCertificate>(filter, null, false, null);
			return source.Cast<PartnerCertificate>();
		}

		public IEnumerable<PartnerRole> FindPartnerRole()
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, PartnerRole.PartnerCallerIdDef, this.callerId);
			IConfigurable[] source = PartnerConfigSession.dataProvider.Find<PartnerRole>(filter, null, false, null);
			return source.Cast<PartnerRole>();
		}

		private Guid GenerateNewGuid()
		{
			return CombGuidGenerator.NewGuid();
		}

		private static IConfigDataProvider dataProvider = ConfigDataProviderFactory.Default.Create(DatabaseType.Directory);

		private readonly string callerId;

		private readonly string partnerName;
	}
}
