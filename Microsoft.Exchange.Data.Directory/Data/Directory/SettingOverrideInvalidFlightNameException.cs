using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SettingOverrideInvalidFlightNameException : SettingOverrideException
	{
		public SettingOverrideInvalidFlightNameException(string flightName, string availableFlights) : base(DirectoryStrings.ErrorSettingOverrideInvalidFlightName(flightName, availableFlights))
		{
			this.flightName = flightName;
			this.availableFlights = availableFlights;
		}

		public SettingOverrideInvalidFlightNameException(string flightName, string availableFlights, Exception innerException) : base(DirectoryStrings.ErrorSettingOverrideInvalidFlightName(flightName, availableFlights), innerException)
		{
			this.flightName = flightName;
			this.availableFlights = availableFlights;
		}

		protected SettingOverrideInvalidFlightNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.flightName = (string)info.GetValue("flightName", typeof(string));
			this.availableFlights = (string)info.GetValue("availableFlights", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("flightName", this.flightName);
			info.AddValue("availableFlights", this.availableFlights);
		}

		public string FlightName
		{
			get
			{
				return this.flightName;
			}
		}

		public string AvailableFlights
		{
			get
			{
				return this.availableFlights;
			}
		}

		private readonly string flightName;

		private readonly string availableFlights;
	}
}
