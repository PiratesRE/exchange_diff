using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class Location
	{
		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public LocationSource Source { get; set; }

		[DataMember]
		public string Uri { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public double? Latitude { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public double? Longitude { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public double? Accuracy { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public double? Altitude { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public double? AltitudeAccuracy { get; set; }

		[DataMember]
		public string StreetAddress { get; set; }
	}
}
