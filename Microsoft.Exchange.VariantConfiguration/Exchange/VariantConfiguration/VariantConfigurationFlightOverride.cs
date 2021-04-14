using System;

namespace Microsoft.Exchange.VariantConfiguration
{
	public sealed class VariantConfigurationFlightOverride : VariantConfigurationOverride
	{
		public VariantConfigurationFlightOverride(string flightName, params string[] parameters) : base(flightName, flightName, parameters)
		{
		}

		public override string FileName
		{
			get
			{
				if (VariantConfiguration.Flights.Contains(this.FlightName))
				{
					return VariantConfiguration.Flights[this.FlightName].FileName;
				}
				return this.FlightName + ".flight.ini";
			}
		}

		public string FlightName
		{
			get
			{
				return base.SectionName;
			}
		}
	}
}
