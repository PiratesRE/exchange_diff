using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Services.jsonservice.types.Weather.Weather4
{
	[XmlRoot("searchLocations")]
	[Serializable]
	public class WeatherLocationData
	{
		[XmlElement("location")]
		public WeatherLocation[] Items { get; set; }
	}
}
