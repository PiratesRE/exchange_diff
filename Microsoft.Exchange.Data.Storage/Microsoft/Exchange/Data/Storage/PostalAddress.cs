using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class PostalAddress : IEquatable<PostalAddress>
	{
		public string Street
		{
			get
			{
				return this.street;
			}
			set
			{
				this.street = value;
			}
		}

		public string City
		{
			get
			{
				return this.city;
			}
			set
			{
				this.city = value;
			}
		}

		public string State
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		public string Country
		{
			get
			{
				return this.country;
			}
			set
			{
				this.country = value;
			}
		}

		public string PostalCode
		{
			get
			{
				return this.postalCode;
			}
			set
			{
				this.postalCode = value;
			}
		}

		public string PostOfficeBox
		{
			get
			{
				return this.postOfficeBox;
			}
			set
			{
				this.postOfficeBox = value;
			}
		}

		public double? Latitude
		{
			get
			{
				return this.latitude;
			}
			set
			{
				this.latitude = value;
			}
		}

		public double? Longitude
		{
			get
			{
				return this.longitude;
			}
			set
			{
				this.longitude = value;
			}
		}

		public double? Accuracy
		{
			get
			{
				return this.accuracy;
			}
			set
			{
				this.accuracy = value;
			}
		}

		public double? Altitude
		{
			get
			{
				return this.altitude;
			}
			set
			{
				this.altitude = value;
			}
		}

		public double? AltitudeAccuracy
		{
			get
			{
				return this.altitudeAccuracy;
			}
			set
			{
				this.altitudeAccuracy = value;
			}
		}

		public string LocationUri
		{
			get
			{
				return this.locationUri;
			}
			set
			{
				this.locationUri = value;
			}
		}

		public LocationSource LocationSource
		{
			get
			{
				return this.locationSource;
			}
			set
			{
				if (EnumValidator.IsValidValue<LocationSource>(value))
				{
					this.locationSource = value;
					return;
				}
				this.locationSource = LocationSource.None;
			}
		}

		public PostalAddressType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<PostalAddressType>(value, "Type");
				this.type = value;
			}
		}

		public bool IsEmpty()
		{
			return (this.accuracy == null || (this.accuracy != null && double.IsNaN(this.accuracy.Value))) && (this.altitude == null || (this.altitude != null && double.IsNaN(this.altitude.Value))) && (this.altitudeAccuracy == null || (this.altitudeAccuracy != null && double.IsNaN(this.altitudeAccuracy.Value))) && this.city == null && this.country == null && (this.latitude == null || (this.latitude != null && double.IsNaN(this.latitude.Value))) && this.locationUri == null && (this.longitude == null || (this.longitude != null && double.IsNaN(this.longitude.Value))) && this.postalCode == null && this.postOfficeBox == null && this.state == null && this.street == null;
		}

		public bool Equals(PostalAddress other)
		{
			return other != null && (string.Equals(this.street, other.street, StringComparison.OrdinalIgnoreCase) && string.Equals(this.city, other.city, StringComparison.OrdinalIgnoreCase) && string.Equals(this.state, other.state, StringComparison.OrdinalIgnoreCase) && string.Equals(this.country, other.country, StringComparison.OrdinalIgnoreCase) && string.Equals(this.postalCode, other.postalCode, StringComparison.OrdinalIgnoreCase)) && string.Equals(this.postOfficeBox, other.postOfficeBox, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as PostalAddress);
		}

		public override int GetHashCode()
		{
			int num = string.IsNullOrEmpty(this.street) ? 0 : this.street.GetHashCode();
			int num2 = string.IsNullOrEmpty(this.city) ? 0 : this.city.GetHashCode();
			int num3 = string.IsNullOrEmpty(this.state) ? 0 : this.state.GetHashCode();
			int num4 = string.IsNullOrEmpty(this.country) ? 0 : this.country.GetHashCode();
			int num5 = string.IsNullOrEmpty(this.postalCode) ? 0 : this.postalCode.GetHashCode();
			if (!string.IsNullOrEmpty(this.postOfficeBox))
			{
				this.postOfficeBox.GetHashCode();
			}
			return num ^ num2 ^ num3 ^ num4 ^ num5 ^ num5;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			PostalAddress.Append(stringBuilder, "street", this.street);
			PostalAddress.Append(stringBuilder, "city", this.city);
			PostalAddress.Append(stringBuilder, "state", this.state);
			PostalAddress.Append(stringBuilder, "country", this.country);
			PostalAddress.Append(stringBuilder, "postalCode", this.postalCode);
			PostalAddress.Append(stringBuilder, "postOfficeBox", this.postOfficeBox);
			PostalAddress.Append<double>(stringBuilder, "latitude", this.latitude);
			PostalAddress.Append<double>(stringBuilder, "longitude", this.longitude);
			PostalAddress.Append<double>(stringBuilder, "accuracy", this.accuracy);
			PostalAddress.Append<double>(stringBuilder, "altitude", this.altitude);
			PostalAddress.Append<double>(stringBuilder, "altitudeAccuracy", this.altitudeAccuracy);
			PostalAddress.Append(stringBuilder, "locationUri", this.locationUri);
			PostalAddress.Append(stringBuilder, "locationSource", this.locationSource.ToString());
			return stringBuilder.ToString();
		}

		private static void Append<T>(StringBuilder text, string name, T? value) where T : struct
		{
			if (value != null)
			{
				T value2 = value.Value;
				PostalAddress.AppendNameValue(text, name, value2.ToString());
			}
		}

		private static void Append(StringBuilder text, string name, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				PostalAddress.AppendNameValue(text, name, value);
			}
		}

		private static void AppendNameValue(StringBuilder text, string name, string value)
		{
			if (text.Length > 0)
			{
				text.Append(", ");
			}
			text.Append(name);
			text.Append("=");
			text.Append(value);
		}

		private string street;

		private string city;

		private string state;

		private string country;

		private string postalCode;

		private string postOfficeBox;

		private double? latitude;

		private double? longitude;

		private double? accuracy;

		private double? altitude;

		private double? altitudeAccuracy;

		private string locationUri;

		private LocationSource locationSource;

		private PostalAddressType type;
	}
}
