using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Services.jsonservice.types.Weather.Weather4
{
	[Serializable]
	public class WeatherLocation : IWeatherLocation
	{
		[XmlAttribute("errormessage")]
		public string ErrorMessage { get; set; }

		[XmlElement("current")]
		public WeatherConditions Conditions { get; set; }

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("fullName")]
		public string FullName { get; set; }

		[XmlAttribute("url")]
		public string Url { get; set; }

		[XmlAttribute("degreetype")]
		public string DegreeUnit { get; set; }

		[XmlAttribute("lat")]
		public double Latitude { get; set; }

		[XmlAttribute("long")]
		public double Longitude { get; set; }

		[XmlAttribute("entityId")]
		public long EntityId { get; set; }

		[XmlElement("forecast")]
		public WeatherForecast[] Forecast { get; set; }

		[XmlAttribute("attribution")]
		public string Attribution { get; set; }

		[XmlAttribute("attribution2")]
		public string ShortAttribution { get; set; }

		[XmlAttribute("provider")]
		public string Provider { get; set; }
	}
}
