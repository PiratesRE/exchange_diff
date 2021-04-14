using System;
using System.Xml;
using Microsoft.Exchange.Data.Storage.Principal;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	public class EntitlementTokenData
	{
		public EntitlementTokenData(string licensePurchaser, LicenseType licenseType, int seatsPurchased, DateTime etokenExpirationDate)
		{
			this.LicenseType = licenseType;
			this.LicensePurchaser = licensePurchaser;
			this.SeatsPurchased = seatsPurchased;
			this.EtokenExpirationDate = etokenExpirationDate;
		}

		public string LicensePurchaser { get; private set; }

		public LicenseType LicenseType { get; private set; }

		public int SeatsPurchased { get; private set; }

		public DateTime EtokenExpirationDate { get; private set; }

		public bool IsRenewalNeeded
		{
			get
			{
				return this.EtokenExpirationDate < DateTime.UtcNow.AddDays(2.0);
			}
		}

		public bool IsExpired
		{
			get
			{
				return this.EtokenExpirationDate < DateTime.UtcNow;
			}
		}

		internal static void UpdatePaidAppSourceLocation(string urlElementName, ExtensionData extensionData)
		{
			if (string.IsNullOrWhiteSpace(extensionData.Etoken))
			{
				return;
			}
			Exception ex;
			extensionData.TryUpdateSourceLocation(null, urlElementName, out ex, new ExtensionDataHelper.TryModifySourceLocationDelegate(EntitlementTokenData.TryModifySourceLocation));
		}

		private static bool TryModifySourceLocation(IExchangePrincipal exchangePrincipal, XmlAttribute xmlAttribute, ExtensionData extensionData, out Exception exception)
		{
			exception = null;
			if (string.IsNullOrWhiteSpace(xmlAttribute.Value))
			{
				return false;
			}
			string str = (xmlAttribute.Value.IndexOf('?') > 0) ? "&" : "?";
			xmlAttribute.Value = xmlAttribute.Value + str + "et=" + extensionData.Etoken;
			return true;
		}

		internal const string ScenarioProcessEntitlementToken = "ProcessEntitlementToken";

		internal const string TokenResponseTagName = "r";

		internal const string TokenTagName = "t";

		internal const string AssetIdTagName = "aid";

		internal const string LicensePurchaserTagName = "cid";

		internal const string SiteLicenseTagName = "sl";

		internal const string DeploymentIdTagName = "did";

		internal const string EtokenExpirationDateTagName = "te";

		internal const string SeatsPurchasedTagName = "ts";

		internal const string LicenseTypeTagName = "et";

		internal const string EtokenParameter = "et=";

		internal const int DaysToRenewBeforeExpiry = 2;
	}
}
