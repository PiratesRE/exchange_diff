using System;
using System.Collections;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	[Serializable]
	internal class EnhancedLocationData : INestedData
	{
		public EnhancedLocationData(int protocolVersion)
		{
			this.subProperties = new Hashtable();
			this.ProtocolVersion = protocolVersion;
			this.Clear();
		}

		public int ProtocolVersion { get; private set; }

		public string DisplayName
		{
			get
			{
				return (string)this.subProperties["DisplayName"];
			}
			set
			{
				this.subProperties["DisplayName"] = value;
			}
		}

		public string Annotation
		{
			get
			{
				return (string)this.subProperties["Annotation"];
			}
			set
			{
				this.subProperties["Annotation"] = value;
			}
		}

		public string Street
		{
			get
			{
				return (string)this.subProperties["Street"];
			}
			set
			{
				this.subProperties["Street"] = value;
			}
		}

		public string City
		{
			get
			{
				return (string)this.subProperties["City"];
			}
			set
			{
				this.subProperties["City"] = value;
			}
		}

		public string State
		{
			get
			{
				return (string)this.subProperties["State"];
			}
			set
			{
				this.subProperties["State"] = value;
			}
		}

		public string Country
		{
			get
			{
				return (string)this.subProperties["Country"];
			}
			set
			{
				this.subProperties["Country"] = value;
			}
		}

		public string PostalCode
		{
			get
			{
				return (string)this.subProperties["PostalCode"];
			}
			set
			{
				this.subProperties["PostalCode"] = value;
			}
		}

		public string Latitude
		{
			get
			{
				return (string)this.subProperties["Latitude"];
			}
			set
			{
				this.subProperties["Latitude"] = value;
			}
		}

		public string Longitude
		{
			get
			{
				return (string)this.subProperties["Longitude"];
			}
			set
			{
				this.subProperties["Longitude"] = value;
			}
		}

		public string Accuracy
		{
			get
			{
				return (string)this.subProperties["Accuracy"];
			}
			set
			{
				this.subProperties["Accuracy"] = value;
			}
		}

		public string Altitude
		{
			get
			{
				return (string)this.subProperties["Altitude"];
			}
			set
			{
				this.subProperties["Altitude"] = value;
			}
		}

		public string AltitudeAccuracy
		{
			get
			{
				return (string)this.subProperties["AltitudeAccuracy"];
			}
			set
			{
				this.subProperties["AltitudeAccuracy"] = value;
			}
		}

		public string LocationUri
		{
			get
			{
				return (string)this.subProperties["LocationUri"];
			}
			set
			{
				this.subProperties["LocationUri"] = value;
			}
		}

		public string[] Keys
		{
			get
			{
				return this.keys;
			}
		}

		public IDictionary SubProperties
		{
			get
			{
				return this.subProperties;
			}
		}

		public void Clear()
		{
			this.subProperties.Clear();
			this.keys = EnhancedLocationData.postalAddressKeys;
		}

		private static readonly string[] postalAddressKeys = new string[]
		{
			"DisplayName",
			"Annotation",
			"Street",
			"City",
			"State",
			"Country",
			"PostalCode",
			"Latitude",
			"Longitude",
			"Accuracy",
			"Altitude",
			"AltitudeAccuracy",
			"LocationUri"
		};

		private string[] keys;

		private IDictionary subProperties;
	}
}
