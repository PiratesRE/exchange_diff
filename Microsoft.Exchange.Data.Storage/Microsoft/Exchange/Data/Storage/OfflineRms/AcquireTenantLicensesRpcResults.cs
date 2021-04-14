using System;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AcquireTenantLicensesRpcResults : LicensingRpcResults
	{
		public XmlNode[] RacXml
		{
			get
			{
				if (this.racXml == null)
				{
					string[] array = base.GetParameterValue("RacStringArray") as string[];
					if (array == null || array.Length <= 0)
					{
						throw new ArgumentNullException("RacStringArray");
					}
					if (!RMUtil.TryConvertCertChainStringArrayToXmlNodeArray(array, out this.racXml))
					{
						throw new InvalidOperationException("Conversion from string array to XmlNode array failed for racXml");
					}
				}
				return this.racXml;
			}
		}

		public XmlNode[] ClcXml
		{
			get
			{
				if (this.clcXml == null)
				{
					string[] array = base.GetParameterValue("ClcStringArray") as string[];
					if (array == null || array.Length <= 0)
					{
						throw new ArgumentNullException("ClcStringArray");
					}
					if (!RMUtil.TryConvertCertChainStringArrayToXmlNodeArray(array, out this.clcXml))
					{
						throw new InvalidOperationException("Conversion from string array to XmlNode array failed for clcXml");
					}
				}
				return this.clcXml;
			}
		}

		public AcquireTenantLicensesRpcResults(byte[] data) : base(data)
		{
		}

		public AcquireTenantLicensesRpcResults(OverallRpcResult overallRpcResult, XmlNode[] rac, XmlNode[] clc) : base(overallRpcResult)
		{
			if (overallRpcResult == null)
			{
				throw new ArgumentNullException("overallRpcResult");
			}
			if (rac == null || rac.Length <= 0)
			{
				throw new ArgumentNullException("rac");
			}
			if (clc == null || clc.Length <= 0)
			{
				throw new ArgumentNullException("clc");
			}
			base.SetParameterValue("RacStringArray", DrmClientUtils.ConvertXmlNodeArrayToStringArray(rac));
			base.SetParameterValue("ClcStringArray", DrmClientUtils.ConvertXmlNodeArrayToStringArray(clc));
		}

		private const string RacStringArrayParameterName = "RacStringArray";

		private const string ClcStringArrayParameterName = "ClcStringArray";

		private XmlNode[] racXml;

		private XmlNode[] clcXml;
	}
}
