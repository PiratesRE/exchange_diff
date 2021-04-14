using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class PersonaPostalAddressType
	{
		public string Street;

		public string City;

		public string State;

		public string Country;

		public string PostalCode;

		public string PostOfficeBox;

		public string Type;

		public double Latitude;

		[XmlIgnore]
		public bool LatitudeSpecified;

		public double Longitude;

		[XmlIgnore]
		public bool LongitudeSpecified;

		public double Accuracy;

		[XmlIgnore]
		public bool AccuracySpecified;

		public double Altitude;

		[XmlIgnore]
		public bool AltitudeSpecified;

		public double AltitudeAccuracy;

		[XmlIgnore]
		public bool AltitudeAccuracySpecified;

		public string FormattedAddress;

		public string LocationUri;

		public LocationSourceType LocationSource;

		[XmlIgnore]
		public bool LocationSourceSpecified;
	}
}
