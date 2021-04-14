using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Services.jsonservice.types.Weather.Weather4
{
	[XmlRoot("weatherdata")]
	[Serializable]
	public class WeatherData
	{
		[XmlElement("weather")]
		public WeatherLocation[] Items { get; set; }

		public const string RootElementName = "weatherdata";
	}
}
