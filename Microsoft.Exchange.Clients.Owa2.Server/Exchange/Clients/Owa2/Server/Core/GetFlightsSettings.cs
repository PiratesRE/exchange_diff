using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Clients.Owa2.Server.Web;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetFlightsSettings : ServiceCommand<ScopeFlightsSetting[]>
	{
		public GetFlightsSettings(CallContext callContext, ScopeFlightsSettingsProvider scopeFlightsSettingsProvider) : base(callContext)
		{
			this.scopeFlightsSettingsProvider = scopeFlightsSettingsProvider;
		}

		protected override ScopeFlightsSetting[] InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext);
			if (!userContext.FeaturesManager.ClientServerSettings.FlightsView.Enabled)
			{
				throw new OwaNotSupportedException("This method is not supported.");
			}
			IList<ScopeFlightsSetting> flightsForScope = this.scopeFlightsSettingsProvider.GetFlightsForScope();
			ScopeFlightsSetting item = new ScopeFlightsSetting(userContext.PrimarySmtpAddress.ToString(), userContext.FeaturesManager.ConfigurationSnapshot.Flights);
			flightsForScope.Add(item);
			return flightsForScope.ToArray<ScopeFlightsSetting>();
		}

		private readonly ScopeFlightsSettingsProvider scopeFlightsSettingsProvider;
	}
}
