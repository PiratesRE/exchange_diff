using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Services.jsonservice.types.Weather.Weather4
{
	[Serializable]
	public class WeatherForecast : IWeatherForecast
	{
		[XmlAttribute("skytextday")]
		public string SkyTextDay { get; set; }

		[XmlAttribute("precip")]
		public int PrecipitationCertainty { get; set; }

		[XmlAttribute("high")]
		public int High { get; set; }

		[XmlAttribute("low")]
		public int Low { get; set; }

		[XmlAttribute("skycodeday")]
		public int SkyCodeDay { get; set; }

		[XmlAttribute("skycodenight")]
		public int SkyCodeNight { get; set; }
	}
}
