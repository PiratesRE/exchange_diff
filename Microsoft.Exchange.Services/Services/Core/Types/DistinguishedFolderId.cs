using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "DistinguishedFolderIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "DistinguishedFolderId")]
	public class DistinguishedFolderId : BaseFolderId
	{
		[XmlElement]
		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 3)]
		public EmailAddressWrapper Mailbox { get; set; }

		[XmlAttribute("Id")]
		[IgnoreDataMember]
		public DistinguishedFolderIdName Id { get; set; }

		[XmlIgnore]
		[DataMember(Name = "Id", IsRequired = true, Order = 1)]
		public string IdString
		{
			get
			{
				return EnumUtilities.ToString<DistinguishedFolderIdName>(this.Id);
			}
			set
			{
				this.Id = EnumUtilities.Parse<DistinguishedFolderIdName>(value);
			}
		}

		[XmlAttribute]
		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 2)]
		public string ChangeKey { get; set; }

		public DistinguishedFolderId()
		{
		}

		public DistinguishedFolderId(EmailAddressWrapper emailAddress, DistinguishedFolderIdName distinguishedFolderIdName, string changeKey)
		{
			this.Mailbox = emailAddress;
			this.Id = distinguishedFolderIdName;
			this.ChangeKey = changeKey;
		}

		public override string GetId()
		{
			throw new InvalidOperationException();
		}

		public override string GetChangeKey()
		{
			return this.ChangeKey;
		}

		protected override void InitServerInfo(bool isHierarchicalOperation)
		{
			base.SetServerInfo(IdConverter.GetServerInfoForDistinguishedFolderId(CallContext.Current, this, isHierarchicalOperation));
		}
	}
}
