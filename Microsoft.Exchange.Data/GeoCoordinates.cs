using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class GeoCoordinates
	{
		public GeoCoordinates(string expression)
		{
			this.ParseAndValidateGeoCoordinates(expression);
		}

		public GeoCoordinates(double latitude, double longitude)
		{
			this.latitude = new double?(latitude);
			this.longitude = new double?(longitude);
		}

		public GeoCoordinates(double latitude, double longitude, double altitude) : this(latitude, longitude)
		{
			this.altitude = new double?(altitude);
		}

		public double Latitude
		{
			get
			{
				return this.latitude.Value;
			}
			set
			{
				if (!GeoCoordinates.IsValidLatitude(value))
				{
					throw new FormatException(DataStrings.ExceptionInvalidLatitude(value));
				}
				this.latitude = new double?(value);
			}
		}

		public double Longitude
		{
			get
			{
				return this.longitude.Value;
			}
			set
			{
				if (!GeoCoordinates.IsValidLongitude(value))
				{
					throw new FormatException(DataStrings.ExceptionInvalidLongitude(value));
				}
				this.longitude = new double?(value);
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

		public override string ToString()
		{
			if (this.altitude == null)
			{
				return string.Format("{0};{1}", this.latitude, this.longitude);
			}
			return string.Format("{0};{1};{2}", this.latitude, this.longitude, this.altitude.Value);
		}

		private static bool IsValidLatitude(double latitude)
		{
			return latitude >= -90.0 && latitude <= 90.0;
		}

		private static bool IsValidLongitude(double longitude)
		{
			return longitude >= -180.0 && longitude <= 180.0;
		}

		private void ParseAndValidateGeoCoordinates(string geoCoordinates)
		{
			geoCoordinates = geoCoordinates.Trim();
			string[] array = geoCoordinates.Split(new char[]
			{
				';'
			});
			if (array.Length != 2 && array.Length != 3)
			{
				throw new FormatException(DataStrings.ExceptionGeoCoordinatesWithWrongFormat(geoCoordinates));
			}
			double value;
			if (!double.TryParse(array[0], out value) || !GeoCoordinates.IsValidLatitude(value))
			{
				throw new FormatException(DataStrings.ExceptionGeoCoordinatesWithInvalidLatitude(geoCoordinates));
			}
			this.latitude = new double?(value);
			double value2;
			if (!double.TryParse(array[1], out value2) || !GeoCoordinates.IsValidLongitude(value2))
			{
				throw new FormatException(DataStrings.ExceptionGeoCoordinatesWithInvalidLongitude(geoCoordinates));
			}
			this.longitude = new double?(value2);
			if (array.Length == 3)
			{
				double value3;
				if (!double.TryParse(array[2], out value3))
				{
					throw new FormatException(DataStrings.ExceptionGeoCoordinatesWithInvalidAltitude(geoCoordinates));
				}
				this.altitude = new double?(value3);
			}
		}

		private const double MaxLatitude = 90.0;

		private const double MinLatitude = -90.0;

		private const double MaxLongitude = 180.0;

		private const double MinLongitude = -180.0;

		private double? latitude;

		private double? longitude;

		private double? altitude;
	}
}
