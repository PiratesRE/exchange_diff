using System;
using Microsoft.BDM.Pets.DNSManagement;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public sealed class WebDnsRecord : ConfigurableObject
	{
		public WebDnsRecord() : base(new SimplePropertyBag())
		{
		}

		internal WebDnsRecord(ResourceRecord record) : base(new SimplePropertyBag())
		{
			this.RecordType = record.RecordType.ToString();
			this.TTL = record.TTL;
			this.DomainName = record.DomainName;
			this.Value = record.Value;
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return WebDnsRecord.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public string RecordType
		{
			get
			{
				return (string)this[WebDnsRecordSchema.RecordType];
			}
			internal set
			{
				this[WebDnsRecordSchema.RecordType] = value;
			}
		}

		public int TTL
		{
			get
			{
				return (int)this[WebDnsRecordSchema.TTL];
			}
			internal set
			{
				this[WebDnsRecordSchema.TTL] = value;
			}
		}

		public string DomainName
		{
			get
			{
				return (string)this[WebDnsRecordSchema.DomainName];
			}
			internal set
			{
				this[WebDnsRecordSchema.DomainName] = value;
			}
		}

		public string Value
		{
			get
			{
				return (string)this[WebDnsRecordSchema.Value];
			}
			internal set
			{
				this[WebDnsRecordSchema.Value] = value;
			}
		}

		private static WebDnsRecordSchema schema = ObjectSchema.GetInstance<WebDnsRecordSchema>();
	}
}
