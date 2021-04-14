using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Clients.Owa2.Server.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class LogEventCommonData
	{
		internal static LogEventCommonData NullInstance
		{
			get
			{
				return new LogEventCommonData();
			}
		}

		internal string Platform { get; private set; }

		internal string DeviceModel { get; private set; }

		internal string Browser { get; private set; }

		internal string BrowserVersion { get; private set; }

		internal string OperatingSystem { get; private set; }

		internal string OperatingSystemVersion { get; private set; }

		internal string TenantDomain { get; private set; }

		internal string Flights { get; private set; }

		internal string Features { get; private set; }

		internal string Layout { get; private set; }

		internal string Culture { get; private set; }

		internal string TimeZone { get; private set; }

		internal Guid DatabaseGuid { get; private set; }

		internal bool ClientDataInitialized { get; private set; }

		internal string OfflineEnabled { get; private set; }

		internal string PalBuild { get; private set; }

		internal string ClientBuild { get; private set; }

		internal LogEventCommonData(UserContext userContext)
		{
			ArgumentValidator.ThrowIfNull("userContext cannot be null", userContext);
			if (userContext.FeaturesManager != null)
			{
				this.Flights = this.GetFlights(userContext.FeaturesManager);
				this.Features = this.GetFeatures(userContext.FeaturesManager);
			}
			if (userContext.ExchangePrincipal != null)
			{
				this.TenantDomain = this.GetTenantDomain(userContext.ExchangePrincipal);
				this.DatabaseGuid = userContext.ExchangePrincipal.MailboxInfo.GetDatabaseGuid();
			}
		}

		private LogEventCommonData()
		{
			this.Flights = (this.Features = (this.TenantDomain = string.Empty));
			this.DatabaseGuid = Guid.Empty;
		}

		internal void UpdateClientData(Dictionary<string, object> sessionInfoData)
		{
			if (!this.ClientDataInitialized)
			{
				this.Platform = this.SafeGetStringValue(sessionInfoData, "pl");
				this.Browser = this.SafeGetStringValue(sessionInfoData, "brn");
				this.BrowserVersion = this.SafeGetStringValue(sessionInfoData, "brv");
				this.OperatingSystem = this.SafeGetStringValue(sessionInfoData, "osn");
				this.OperatingSystemVersion = this.SafeGetStringValue(sessionInfoData, "osv");
				this.DeviceModel = this.SafeGetStringValue(sessionInfoData, "dm");
				this.PalBuild = this.SafeGetStringValue(sessionInfoData, "pbld");
				this.ClientDataInitialized = true;
			}
			this.Layout = this.SafeGetStringValue(sessionInfoData, "l");
			this.Culture = this.SafeGetStringValue(sessionInfoData, "clg");
			this.TimeZone = this.SafeGetStringValue(sessionInfoData, "tz");
			this.OfflineEnabled = this.SafeGetStringValue(sessionInfoData, "oe");
			this.ClientBuild = this.SafeGetStringValue(sessionInfoData, "cbld");
		}

		private string SafeGetStringValue(Dictionary<string, object> sessionInfoData, string key)
		{
			object obj;
			if (!sessionInfoData.TryGetValue(key, out obj))
			{
				return null;
			}
			if (obj is string)
			{
				return (string)obj;
			}
			return null;
		}

		private string GetTenantDomain(ExchangePrincipal exchangePrincipal)
		{
			string result = string.Empty;
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
			{
				try
				{
					result = SmtpAddress.Parse(exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()).Domain;
				}
				catch (FormatException)
				{
				}
			}
			return result;
		}

		private string GetFeatures(FeaturesManager featuresManager)
		{
			HashSet<string> enabledFlightedFeatures = featuresManager.GetEnabledFlightedFeatures(FlightedFeatureScope.Any);
			IOrderedEnumerable<string> values = from feature in enabledFlightedFeatures
			orderby feature
			select feature;
			return string.Join(",", values);
		}

		private string GetFlights(FeaturesManager featuresManager)
		{
			string result = null;
			if (featuresManager.ConfigurationSnapshot != null)
			{
				string[] flights = featuresManager.ConfigurationSnapshot.Flights;
				Array.Sort<string>(flights);
				result = string.Join(",", flights);
			}
			return result;
		}

		internal const string PlatformKey = "pl";

		internal const string DeviceModelKey = "dm";

		internal const string BrowserKey = "brn";

		internal const string BrowserVersionKey = "brv";

		internal const string OperatingSystemKey = "osn";

		internal const string OperatingSystemVersionKey = "osv";

		internal const string TenantDomainKey = "dom";

		internal const string ServicePlanKey = "sku";

		internal const string DatabaseGuidKey = "db";

		internal const string FeaturesKey = "ftr";

		internal const string FlightsKey = "flt";

		internal const string LayoutKey = "l";

		internal const string CultureKey = "clg";

		internal const string TimeZoneKey = "tz";

		internal const string OfflineEnabledKey = "oe";

		internal const string PalBuildKey = "pbld";

		internal const string ClientBuildKey = "cbld";
	}
}
