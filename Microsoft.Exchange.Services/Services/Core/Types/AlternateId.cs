using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "AlternateId", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "AlternateIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class AlternateId : AlternateIdBase
	{
		public AlternateId()
		{
		}

		internal AlternateId(string id, string primarySmtpAddress, IdFormat format) : base(format)
		{
			this.id = id;
			this.mailbox = primarySmtpAddress;
		}

		internal AlternateId(string id, string primarySmtpAddress, IdFormat format, bool isArchive) : base(format)
		{
			this.id = id;
			this.mailbox = primarySmtpAddress;
			this.isArchive = isArchive;
		}

		[DataMember(IsRequired = true, Order = 1)]
		[XmlAttribute]
		public string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		[DataMember(IsRequired = true, Order = 2)]
		[XmlAttribute]
		public string Mailbox
		{
			get
			{
				return this.mailbox;
			}
			set
			{
				this.mailbox = value;
			}
		}

		[XmlIgnore]
		public bool IsArchiveSpecified
		{
			get
			{
				return this.isArchive;
			}
		}

		[XmlAttribute]
		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 3)]
		public bool IsArchive
		{
			get
			{
				return this.isArchive;
			}
			set
			{
				this.isArchive = value;
			}
		}

		internal override CanonicalConvertedId Parse()
		{
			return AlternateIdBase.GetIdConverter(base.Format).Parse(this);
		}

		private string id;

		private string mailbox;

		private bool isArchive;
	}
}
