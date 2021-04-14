using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Services.jsonservice.types.Weather.Weather4
{
	[Serializable]
	public class WeatherConditions : IWeatherConditions
	{
		[XmlAttribute("temperature")]
		public int Temperature { get; set; }

		[XmlAttribute("skycode")]
		public int SkyCode { get; set; }

		[XmlAttribute("skytext")]
		public string SkyText { get; set; }
	}
}
