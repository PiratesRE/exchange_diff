using System;
using System.Xml;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AcquireTenantLicensesRpcParameters : LicensingRpcParameters
	{
		public AcquireTenantLicensesRpcParameters(byte[] data) : base(data)
		{
		}

		public AcquireTenantLicensesRpcParameters(RmsClientManagerContext rmsClientManagerContext, string identity, XmlNode[] machineCertificateChain) : base(rmsClientManagerContext)
		{
			if (rmsClientManagerContext == null)
			{
				throw new ArgumentNullException("rmsClientManagerContext");
			}
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("identity");
			}
			if (machineCertificateChain == null || machineCertificateChain.Length <= 0)
			{
				throw new ArgumentNullException("machineCertificateChain");
			}
			base.SetParameterValue("Identity", identity);
			base.SetParameterValue("MachineCertificateChainStringArray", DrmClientUtils.ConvertXmlNodeArrayToStringArray(machineCertificateChain));
		}

		public XmlNode[] MachineCertificateChain
		{
			get
			{
				if (this.machineCertificateChain == null)
				{
					string[] array = base.GetParameterValue("MachineCertificateChainStringArray") as string[];
					if (array == null || array.Length <= 0)
					{
						throw new ArgumentNullException("machineCertificateChainStringArray");
					}
					if (!RMUtil.TryConvertCertChainStringArrayToXmlNodeArray(array, out this.machineCertificateChain))
					{
						throw new InvalidOperationException("Conversion from string array to XmlNode array failed for machineCertificateChain");
					}
				}
				return this.machineCertificateChain;
			}
		}

		public string Identity
		{
			get
			{
				if (string.IsNullOrEmpty(this.identity))
				{
					this.identity = (base.GetParameterValue("Identity") as string);
				}
				return this.identity;
			}
		}

		private const string MachineCertificateChainStringArrayParameterName = "MachineCertificateChainStringArray";

		private const string IdentityParameterName = "Identity";

		private XmlNode[] machineCertificateChain;

		private string identity;
	}
}
