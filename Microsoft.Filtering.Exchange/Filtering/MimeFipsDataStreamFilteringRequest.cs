using System;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.UnifiedContent;
using Microsoft.Exchange.UnifiedContent.Exchange;

namespace Microsoft.Filtering
{
	internal sealed class MimeFipsDataStreamFilteringRequest : FipsDataStreamFilteringRequest
	{
		private MimeFipsDataStreamFilteringRequest(MailItem mailItem, string id, ContentManager contentManager) : base(id, contentManager)
		{
			this.MailItem = mailItem;
		}

		public MailItem MailItem { get; private set; }

		public override RecoveryOptions RecoveryOptions
		{
			get
			{
				Header header = this.MailItem.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Transport-Rules-Fips-Result");
				RecoveryOptions result;
				if (header != null && Enum.TryParse<RecoveryOptions>(header.Value, out result))
				{
					return result;
				}
				return RecoveryOptions.None;
			}
			set
			{
				Header header = this.MailItem.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Transport-Rules-Fips-Result");
				if (header == null)
				{
					header = Header.Create("X-MS-Exchange-Organization-Transport-Rules-Fips-Result");
					this.MailItem.MimeDocument.RootPart.Headers.AppendChild(header);
				}
				header.Value = value.ToString();
			}
		}

		public static MimeFipsDataStreamFilteringRequest CreateInstance(MailItem mailItem)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			string messageId = mailItem.Message.MessageId;
			ContentManager contentManager = (ContentManager)ContentManagerFactory.ExtractContentManager(mailItem.Message);
			return new MimeFipsDataStreamFilteringRequest(mailItem, messageId, contentManager);
		}

		protected override void Serialize(UnifiedContentSerializer unifiedContentSerializer, bool bypassBodyTextTruncation = true)
		{
			this.MailItem.Message.Serialize(unifiedContentSerializer, bypassBodyTextTruncation);
		}
	}
}
