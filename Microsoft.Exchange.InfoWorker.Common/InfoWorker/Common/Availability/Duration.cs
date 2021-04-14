using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class Duration
	{
		public Duration()
		{
		}

		public Duration(DateTime startTime, DateTime endTime)
		{
			this.startTime = startTime;
			this.endTime = endTime;
		}

		[IgnoreDataMember]
		[XmlElement]
		public DateTime StartTime
		{
			get
			{
				return this.startTime;
			}
			set
			{
				this.startTime = value;
			}
		}

		[XmlElement]
		[IgnoreDataMember]
		public DateTime EndTime
		{
			get
			{
				return this.endTime;
			}
			set
			{
				this.endTime = value;
			}
		}

		[DataMember(Name = "StartTime", IsRequired = true)]
		[XmlIgnore]
		public string StartTimeString
		{
			get
			{
				return this.StartTime.ToIso8061();
			}
			set
			{
				this.StartTime = DateTime.Parse(value);
			}
		}

		[DataMember(Name = "EndTime", IsRequired = true)]
		[XmlIgnore]
		public string EndTimeString
		{
			get
			{
				return this.EndTime.ToIso8061();
			}
			set
			{
				this.EndTime = DateTime.Parse(value);
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Start={0}, End={1}", new object[]
			{
				this.startTime,
				this.endTime
			});
		}

		internal void Validate(string propertyName)
		{
			if (this.startTime >= this.EndTime)
			{
				throw new InvalidTimeIntervalException(propertyName);
			}
		}

		private DateTime startTime;

		private DateTime endTime;
	}
}
