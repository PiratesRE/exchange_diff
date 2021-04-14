using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class WebServicesTestOutcome
	{
		public string Source { get; internal set; }

		public string ServiceEndpoint { get; internal set; }

		public WebServicesTestOutcome.TestScenario Scenario { get; internal set; }

		public string ScenarioDescription { get; internal set; }

		public CasTransactionResultEnum Result { get; internal set; }

		public long Latency { get; internal set; }

		public string Error { get; internal set; }

		public string Verbose { get; internal set; }

		public int MonitoringEventId
		{
			get
			{
				if (this.Result == CasTransactionResultEnum.Failure)
				{
					return (int)(this.Scenario + 1000);
				}
				return (int)this.Scenario;
			}
		}

		public override string ToString()
		{
			return Strings.WebServicesTestOutcomeToString(this.Source, this.ServiceEndpoint, this.Scenario.ToString(), this.Result.ToString(), this.Latency.ToString(), this.Error, this.Verbose);
		}

		public const int FailedEventIdBase = 1000;

		public enum TestScenario
		{
			[LocDescription(Strings.IDs.ScenarioAutoDiscoverOutlookProvider)]
			AutoDiscoverOutlookProvider = 5001,
			[LocDescription(Strings.IDs.ScenarioExchangeWebServices)]
			ExchangeWebServices,
			[LocDescription(Strings.IDs.ScenarioAvailabilityService)]
			AvailabilityService,
			[LocDescription(Strings.IDs.ScenarioOfflineAddressBook)]
			OfflineAddressBook,
			[LocDescription(Strings.IDs.ScenarioAutoDiscoverSoapProvider)]
			AutoDiscoverSoapProvider = 5051,
			[LocDescription(Strings.IDs.ScenarioEwsConvertId)]
			EwsConvertId,
			[LocDescription(Strings.IDs.ScenarioEwsGetFolder)]
			EwsGetFolder
		}
	}
}
