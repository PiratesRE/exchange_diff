using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class PersonaPostalAddressType
	{
		public string Street
		{
			get
			{
				return this.streetField;
			}
			set
			{
				this.streetField = value;
			}
		}

		public string City
		{
			get
			{
				return this.cityField;
			}
			set
			{
				this.cityField = value;
			}
		}

		public string State
		{
			get
			{
				return this.stateField;
			}
			set
			{
				this.stateField = value;
			}
		}

		public string Country
		{
			get
			{
				return this.countryField;
			}
			set
			{
				this.countryField = value;
			}
		}

		public string PostalCode
		{
			get
			{
				return this.postalCodeField;
			}
			set
			{
				this.postalCodeField = value;
			}
		}

		public string PostOfficeBox
		{
			get
			{
				return this.postOfficeBoxField;
			}
			set
			{
				this.postOfficeBoxField = value;
			}
		}

		public string Type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		public double Latitude
		{
			get
			{
				return this.latitudeField;
			}
			set
			{
				this.latitudeField = value;
			}
		}

		[XmlIgnore]
		public bool LatitudeSpecified
		{
			get
			{
				return this.latitudeFieldSpecified;
			}
			set
			{
				this.latitudeFieldSpecified = value;
			}
		}

		public double Longitude
		{
			get
			{
				return this.longitudeField;
			}
			set
			{
				this.longitudeField = value;
			}
		}

		[XmlIgnore]
		public bool LongitudeSpecified
		{
			get
			{
				return this.longitudeFieldSpecified;
			}
			set
			{
				this.longitudeFieldSpecified = value;
			}
		}

		public double Accuracy
		{
			get
			{
				return this.accuracyField;
			}
			set
			{
				this.accuracyField = value;
			}
		}

		[XmlIgnore]
		public bool AccuracySpecified
		{
			get
			{
				return this.accuracyFieldSpecified;
			}
			set
			{
				this.accuracyFieldSpecified = value;
			}
		}

		public double Altitude
		{
			get
			{
				return this.altitudeField;
			}
			set
			{
				this.altitudeField = value;
			}
		}

		[XmlIgnore]
		public bool AltitudeSpecified
		{
			get
			{
				return this.altitudeFieldSpecified;
			}
			set
			{
				this.altitudeFieldSpecified = value;
			}
		}

		public double AltitudeAccuracy
		{
			get
			{
				return this.altitudeAccuracyField;
			}
			set
			{
				this.altitudeAccuracyField = value;
			}
		}

		[XmlIgnore]
		public bool AltitudeAccuracySpecified
		{
			get
			{
				return this.altitudeAccuracyFieldSpecified;
			}
			set
			{
				this.altitudeAccuracyFieldSpecified = value;
			}
		}

		public string FormattedAddress
		{
			get
			{
				return this.formattedAddressField;
			}
			set
			{
				this.formattedAddressField = value;
			}
		}

		public string LocationUri
		{
			get
			{
				return this.locationUriField;
			}
			set
			{
				this.locationUriField = value;
			}
		}

		public LocationSourceType LocationSource
		{
			get
			{
				return this.locationSourceField;
			}
			set
			{
				this.locationSourceField = value;
			}
		}

		[XmlIgnore]
		public bool LocationSourceSpecified
		{
			get
			{
				return this.locationSourceFieldSpecified;
			}
			set
			{
				this.locationSourceFieldSpecified = value;
			}
		}

		private string streetField;

		private string cityField;

		private string stateField;

		private string countryField;

		private string postalCodeField;

		private string postOfficeBoxField;

		private string typeField;

		private double latitudeField;

		private bool latitudeFieldSpecified;

		private double longitudeField;

		private bool longitudeFieldSpecified;

		private double accuracyField;

		private bool accuracyFieldSpecified;

		private double altitudeField;

		private bool altitudeFieldSpecified;

		private double altitudeAccuracyField;

		private bool altitudeAccuracyFieldSpecified;

		private string formattedAddressField;

		private string locationUriField;

		private LocationSourceType locationSourceField;

		private bool locationSourceFieldSpecified;
	}
}
