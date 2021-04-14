using System;
using System.Xml;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.RightsManagementServices.Core;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AcquireUseLicensesRpcParameters : LicensingRpcParameters
	{
		public AcquireUseLicensesRpcParameters(byte[] data) : base(data)
		{
		}

		public AcquireUseLicensesRpcParameters(RmsClientManagerContext rmsClientManagerContext, XmlNode[] rightsAccountCertificate, XmlNode[] issuanceLicense, LicenseeIdentity[] licenseeIdentities) : base(rmsClientManagerContext)
		{
			if (rmsClientManagerContext == null)
			{
				throw new ArgumentNullException("rmsClientManagerContext");
			}
			if (rightsAccountCertificate == null || rightsAccountCertificate.Length <= 0)
			{
				throw new ArgumentNullException("rightsAccountCertificate");
			}
			if (issuanceLicense == null || issuanceLicense.Length <= 0)
			{
				throw new ArgumentNullException("issuanceLicense");
			}
			if (licenseeIdentities == null || licenseeIdentities.Length <= 0)
			{
				throw new ArgumentNullException("licenseeIdentities");
			}
			base.SetParameterValue("RightsAccountCertificateStringArray", DrmClientUtils.ConvertXmlNodeArrayToStringArray(rightsAccountCertificate));
			base.SetParameterValue("IssuanceLicenseStringArray", DrmClientUtils.ConvertXmlNodeArrayToStringArray(issuanceLicense));
			base.SetParameterValue("LicenseeIdentities", licenseeIdentities);
		}

		public XmlNode[] RightsAccountCertificate
		{
			get
			{
				if (this.rightsAccountCertificate == null)
				{
					string[] array = base.GetParameterValue("RightsAccountCertificateStringArray") as string[];
					if (array == null || array.Length <= 0)
					{
						throw new ArgumentNullException("rightsAccountCertificateStringArray");
					}
					if (!RMUtil.TryConvertCertChainStringArrayToXmlNodeArray(array, out this.rightsAccountCertificate))
					{
						throw new InvalidOperationException("Conversion from string array to XmlNode array failed for rightsAccountCertificate");
					}
				}
				return this.rightsAccountCertificate;
			}
		}

		public XmlNode[] IssuanceLicense
		{
			get
			{
				if (this.issuanceLicense == null)
				{
					string[] array = base.GetParameterValue("IssuanceLicenseStringArray") as string[];
					if (array == null || array.Length <= 0)
					{
						throw new ArgumentNullException("issuanceLicenseStringArray");
					}
					if (!RMUtil.TryConvertCertChainStringArrayToXmlNodeArray(array, out this.issuanceLicense))
					{
						throw new InvalidOperationException("Conversion from string array to XmlNode array failed for issuanceLicense");
					}
				}
				return this.issuanceLicense;
			}
		}

		public LicenseeIdentity[] LicenseeIdentities
		{
			get
			{
				if (this.licenseeIdentities == null)
				{
					this.licenseeIdentities = (base.GetParameterValue("LicenseeIdentities") as LicenseeIdentity[]);
					if (this.licenseeIdentities == null || this.licenseeIdentities.Length <= 0)
					{
						throw new ArgumentNullException("licenseeIdentities");
					}
				}
				return this.licenseeIdentities;
			}
		}

		private const string RightsAccountCertificateStringArrayParameterName = "RightsAccountCertificateStringArray";

		private const string IssuanceLicenseStringArrayParameterName = "IssuanceLicenseStringArray";

		private const string LicenseeIdentitiesParameterName = "LicenseeIdentities";

		private XmlNode[] rightsAccountCertificate;

		private XmlNode[] issuanceLicense;

		private LicenseeIdentity[] licenseeIdentities;
	}
}
