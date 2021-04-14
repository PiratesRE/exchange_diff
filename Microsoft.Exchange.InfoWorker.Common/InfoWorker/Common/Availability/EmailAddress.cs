using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class EmailAddress
	{
		public EmailAddress()
		{
			this.smtpAddress = SmtpAddress.Empty;
			this.Init();
		}

		public EmailAddress(string name, string address, string routingType)
		{
			this.Init();
			this.name = name;
			this.address = address;
			if (!string.IsNullOrEmpty(routingType))
			{
				this.routingType = routingType;
			}
			this.smtpAddress = SmtpAddress.Empty;
		}

		public EmailAddress(string name, string address) : this(name, address, "SMTP")
		{
		}

		[DataMember]
		[XmlElement]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[XmlElement]
		[DataMember]
		public string Address
		{
			get
			{
				return this.address;
			}
			set
			{
				this.address = value;
			}
		}

		public string Domain
		{
			get
			{
				if (StringComparer.OrdinalIgnoreCase.Equals(this.routingType, "SMTP"))
				{
					if (this.smtpAddress == SmtpAddress.Empty)
					{
						this.smtpAddress = new SmtpAddress(this.address);
					}
					return this.smtpAddress.Domain;
				}
				return null;
			}
		}

		[XmlElement]
		[DataMember]
		public string RoutingType
		{
			get
			{
				return this.routingType;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.routingType = value;
				}
			}
		}

		public override string ToString()
		{
			return string.Format("<{0}>{1}:{2}", this.name, this.routingType, this.address);
		}

		[OnDeserializing]
		private void Init(StreamingContext context)
		{
			this.Init();
		}

		private void Init()
		{
			this.routingType = "SMTP";
		}

		private const string SmtpType = "SMTP";

		private string name;

		private string address;

		private string routingType;

		private SmtpAddress smtpAddress;
	}
}
