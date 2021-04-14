using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class FeaturesManagerFactory
	{
		public FeaturesManagerFactory(MiniRecipient recipient, IConfigurationContext configurationContext, ScopeFlightsSettingsProvider scopeFlightsSettingsProvider) : this(recipient, configurationContext, scopeFlightsSettingsProvider, null, string.Empty, false)
		{
		}

		public FeaturesManagerFactory(MiniRecipient recipient, IConfigurationContext configurationContext, ScopeFlightsSettingsProvider scopeFlightsSettingsProvider, Func<VariantConfigurationSnapshot, IFeaturesStateOverride> featureStateOverrideFactory, string rampId, bool isFirstRelease)
		{
			this.recipient = recipient;
			this.configurationContext = configurationContext;
			this.scopeFlightsSettingsProvider = scopeFlightsSettingsProvider;
			this.featureStateOverrideFactory = featureStateOverrideFactory;
			this.rampId = rampId;
			this.isFirstRelease = isFirstRelease;
		}

		public FeaturesManager GetFeaturesManager(HttpContext httpContext)
		{
			string flightsOverride = (httpContext == null) ? null : RequestDispatcherUtilities.GetStringUrlParameter(httpContext, "flights");
			HttpCookieCollection httpCookieCollection = (httpContext == null) ? null : httpContext.Request.Cookies;
			HttpCookie httpCookie = (httpCookieCollection == null) ? null : httpCookieCollection.Get("flights");
			if (httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value) && httpCookie.Value.Equals("none", StringComparison.InvariantCultureIgnoreCase))
			{
				flightsOverride = "none";
			}
			return this.GetFeaturesManagerInternal(flightsOverride);
		}

		protected virtual FeaturesManager CreateFeaturesManager()
		{
			IList<string> flightsFromQueryString = this.GetFlightsFromQueryString(this.currentFlightsOverride);
			VariantConfigurationSnapshot configurationSnapshot = string.IsNullOrWhiteSpace(this.rampId) ? VariantConfiguration.GetSnapshot(this.recipient.GetContext(null), null, flightsFromQueryString) : VariantConfiguration.GetSnapshot(this.recipient.GetContext(this.rampId, this.isFirstRelease), null, flightsFromQueryString);
			return FeaturesManager.Create(configurationSnapshot, this.configurationContext, this.featureStateOverrideFactory);
		}

		private IList<string> GetFlightsFromQueryString(string flightsOverride)
		{
			if (flightsOverride == null)
			{
				return null;
			}
			List<string> list = new List<string>();
			string[] array = flightsOverride.Split(new char[]
			{
				',',
				' ',
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				if (ScopeFlightsSettingsProvider.IsLogicalScope(text))
				{
					list.AddRange(this.scopeFlightsSettingsProvider.GetFlightsForScope(text));
				}
				else
				{
					list.Add(text);
				}
			}
			return list;
		}

		private FeaturesManager GetFeaturesManagerInternal(string flightsOverride)
		{
			if (this.currentValue == null || flightsOverride != this.currentFlightsOverride)
			{
				this.currentFlightsOverride = flightsOverride;
				this.currentValue = this.CreateFeaturesManager();
			}
			return this.currentValue;
		}

		public const string FlightCookieParameterName = "flights";

		public const string FlightDisabledOverrideValue = "none";

		private const string FlightQueryStringParameterName = "flights";

		private readonly MiniRecipient recipient;

		private readonly IConfigurationContext configurationContext;

		private readonly ScopeFlightsSettingsProvider scopeFlightsSettingsProvider;

		private readonly Func<VariantConfigurationSnapshot, IFeaturesStateOverride> featureStateOverrideFactory;

		private readonly string rampId;

		private readonly bool isFirstRelease;

		private FeaturesManager currentValue;

		private string currentFlightsOverride;
	}
}
