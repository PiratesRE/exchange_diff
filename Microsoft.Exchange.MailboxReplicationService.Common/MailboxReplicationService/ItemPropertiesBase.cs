using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[KnownType(typeof(AuditLogRecord))]
	[KnownType(typeof(MSInternal5))]
	[KnownType(typeof(OlcMessageProperties))]
	[KnownType(typeof(MSInternal1))]
	[KnownType(typeof(InboxRuleSettings))]
	[KnownType(typeof(FolderAcl))]
	[KnownType(typeof(CalendarItemProperties))]
	[KnownType(typeof(MsaSettings))]
	[KnownType(typeof(DeliverySettings))]
	[KnownType(typeof(UXSettings))]
	[KnownType(typeof(JunkEmailSettings))]
	[KnownType(typeof(PopAccountSettings))]
	[KnownType(typeof(CalendarUserSettings))]
	[KnownType(typeof(CalendarFolderSettings))]
	[KnownType(typeof(UnschematizedSettings))]
	[KnownType(typeof(SatchmoFolderSettings))]
	[KnownType(typeof(FeatureSetSettings))]
	[KnownType(typeof(OlcInboxRule))]
	[DataContract]
	[KnownType(typeof(AccountSettings))]
	internal abstract class ItemPropertiesBase
	{
		public virtual void Apply(MrsPSHandler psHandler, MailboxSession mailboxSession)
		{
			throw new OlcSettingNotImplementedPermanentException("Mailbox", base.GetType().ToString());
		}

		public virtual void Apply(CoreFolder folder)
		{
			throw new OlcSettingNotImplementedPermanentException("Folder", base.GetType().ToString());
		}

		public virtual void Apply(MailboxSession session, Item item)
		{
			throw new OlcSettingNotImplementedPermanentException("Item", base.GetType().ToString());
		}

		public virtual byte[] GetId()
		{
			return CommonUtils.GetSHA512Hash(this.ToString());
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriterSettings settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = true,
				CheckCharacters = false
			};
			DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(ItemPropertiesBase));
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
			{
				dataContractSerializer.WriteObject(xmlWriter, this);
				xmlWriter.Flush();
			}
			return stringBuilder.ToString();
		}
	}
}
