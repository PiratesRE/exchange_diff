using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class RequestIndexId : XMLSerializableBase
	{
		public RequestIndexId() : this(RequestIndexLocation.ADLegacy)
		{
		}

		public RequestIndexId(RequestIndexLocation location)
		{
			this.Location = location;
		}

		public RequestIndexId(ADObjectId mailbox) : this(RequestIndexLocation.Mailbox)
		{
			this.mailbox = mailbox;
		}

		[XmlIgnore]
		public RequestIndexLocation Location
		{
			get
			{
				return this.location;
			}
			set
			{
				this.location = value;
			}
		}

		[XmlIgnore]
		public Type RequestIndexEntryType
		{
			get
			{
				switch (this.location)
				{
				case RequestIndexLocation.AD:
					return typeof(MRSRequestWrapper);
				case RequestIndexLocation.UserMailbox:
					return typeof(AggregatedAccountConfigurationWrapper);
				case RequestIndexLocation.Mailbox:
					return typeof(MRSRequestMailboxEntry);
				case RequestIndexLocation.UserMailboxList:
					return typeof(AggregatedAccountListConfigurationWrapper);
				}
				return null;
			}
		}

		[XmlElement(ElementName = "Location")]
		public int LocationInt
		{
			get
			{
				return (int)this.Location;
			}
			set
			{
				this.Location = (RequestIndexLocation)value;
			}
		}

		[XmlIgnore]
		public ADObjectId Mailbox
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

		[DefaultValue(null)]
		[XmlElement(ElementName = "Mailbox", IsNullable = true)]
		public ADObjectIdXML MailboxXml
		{
			get
			{
				return ADObjectIdXML.Serialize(this.Mailbox);
			}
			set
			{
				this.Mailbox = ADObjectIdXML.Deserialize(value);
			}
		}

		public override bool Equals(object obj)
		{
			RequestIndexId requestIndexId = obj as RequestIndexId;
			return requestIndexId != null && this.Location == requestIndexId.Location && this.Mailbox == requestIndexId.Mailbox;
		}

		public override int GetHashCode()
		{
			if (this.Mailbox != null)
			{
				return this.Mailbox.GetHashCode();
			}
			return this.LocationInt;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.Location);
			if (this.Mailbox != null)
			{
				stringBuilder.AppendFormat(";{0}", this.Mailbox);
			}
			return stringBuilder.ToString();
		}

		private RequestIndexLocation location;

		private ADObjectId mailbox;
	}
}
