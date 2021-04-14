using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(TimeZoneDefinitionType))]
	[KnownType(typeof(ArrayOfTransitionsType))]
	[KnownType(typeof(TimeZoneDefinitionType))]
	[KnownType(typeof(ResponseMessage))]
	[XmlType("GetServerTimeZonesResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(PeriodType))]
	[KnownType(typeof(GetServerTimeZonesResponseMessage))]
	public class GetServerTimeZonesResponseMessage : ResponseMessage
	{
		public GetServerTimeZonesResponseMessage()
		{
		}

		internal GetServerTimeZonesResponseMessage(ServiceResultCode code, ServiceError error, GetServerTimeZoneResultType timeZones) : base(code, error)
		{
			this.TimeZoneResultType = timeZones;
		}

		[IgnoreDataMember]
		[XmlIgnore]
		internal GetServerTimeZoneResultType TimeZoneResultType { get; set; }

		[XmlArrayItem("TimeZoneDefinition", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArray("TimeZoneDefinitions", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "TimeZoneDefinitions", EmitDefaultValue = false)]
		public TimeZoneDefinitionType[] TimeZoneDefinitions
		{
			get
			{
				if (this.timeZoneDefinitions == null)
				{
					this.timeZoneDefinitions = this.TimeZoneResultType.ToTimeZoneDefinitionType();
				}
				return this.timeZoneDefinitions;
			}
			set
			{
				this.timeZoneDefinitions = value;
			}
		}

		private TimeZoneDefinitionType[] timeZoneDefinitions;
	}
}
