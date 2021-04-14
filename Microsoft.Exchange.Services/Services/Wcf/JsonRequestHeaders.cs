using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class JsonRequestHeaders
	{
		[IgnoreDataMember]
		public ExchangeVersionType RequestServerVersion { get; set; }

		[DataMember(Name = "RequestServerVersion", IsRequired = true)]
		private string RequestServerVersionString
		{
			get
			{
				return EnumUtilities.ToString<ExchangeVersionType>(this.RequestServerVersion);
			}
			set
			{
				this.RequestServerVersion = EnumUtilities.Parse<ExchangeVersionType>(value);
			}
		}

		[DataMember(Name = "MailboxCulture", IsRequired = false)]
		public string MailboxCulture { get; set; }

		[DataMember(Name = "ExchangeImpersonation", IsRequired = false)]
		public ExchangeImpersonationType ExchangeImpersonation { get; set; }

		[DataMember(Name = "SerializedSecurityContext", IsRequired = false)]
		public SerializedSecurityContextType SerializedSecurityContext { get; set; }

		[DataMember(Name = "TimeZoneContext", IsRequired = false)]
		public TimeZoneContextType TimeZoneContext { get; set; }

		[IgnoreDataMember]
		public DateTimePrecision DateTimePrecision { get; set; }

		[DataMember(Name = "DateTimePrecision", IsRequired = false)]
		public string DateTimePrecisionString
		{
			get
			{
				return EnumUtilities.ToString<DateTimePrecision>(this.DateTimePrecision);
			}
			set
			{
				this.DateTimePrecision = EnumUtilities.Parse<DateTimePrecision>(value);
			}
		}

		[DataMember(Name = "ManagementRole", IsRequired = false, EmitDefaultValue = false)]
		public ManagementRoleType ManagementRole { get; set; }

		[DataMember(Name = "BackgroundLoad", IsRequired = false, EmitDefaultValue = false)]
		public bool BackgroundLoad { get; set; }
	}
}
