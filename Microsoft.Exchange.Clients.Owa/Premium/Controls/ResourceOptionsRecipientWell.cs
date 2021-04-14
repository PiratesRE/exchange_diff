using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class ResourceOptionsRecipientWell : RecipientWell
	{
		internal ResourceOptionsRecipientWell(CalendarConfiguration resourceConfig)
		{
			this.calendarConfig = resourceConfig;
		}

		protected override void RenderAdditionalExpandos(TextWriter writer)
		{
			writer.Write(" _fRsrc=1");
		}

		public override bool HasRecipients(RecipientWellType type)
		{
			MultiValuedProperty<string> addressList = this.GetAddressList(type);
			return addressList != null && addressList.Count > 0;
		}

		internal override void RenderContents(TextWriter writer, UserContext userContext, RecipientWellType type, RecipientWellNode.RenderFlags flags, RenderRecipientWellNode wellNode)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (!this.HasRecipients(type))
			{
				return;
			}
			RecipientWellNode.RenderFlags renderFlags = flags & ~RecipientWellNode.RenderFlags.RenderCommas;
			bool flag = true;
			string smtpAddress = null;
			string alias = null;
			int num = 0;
			MultiValuedProperty<string> addressList = this.GetAddressList(type);
			IRecipientSession recipientSession = Utilities.CreateADRecipientSession(ConsistencyMode.IgnoreInvalid, userContext);
			foreach (string text in addressList)
			{
				ADObjectId adObjectId = null;
				ADRecipient adrecipient = recipientSession.FindByLegacyExchangeDN(text);
				bool flag2 = (flags & RecipientWellNode.RenderFlags.ReadOnly) != RecipientWellNode.RenderFlags.None;
				if (adrecipient != null)
				{
					adObjectId = adrecipient.Id;
					smtpAddress = adrecipient.PrimarySmtpAddress.ToString();
					if (flag2)
					{
						alias = adrecipient.Alias;
					}
					if (adrecipient is IADDistributionList)
					{
						num |= 1;
					}
					if (DirectoryAssistance.IsADRecipientRoom(adrecipient))
					{
						num |= 2;
					}
				}
				if (wellNode(writer, userContext, (adrecipient != null) ? adrecipient.DisplayName : text.ToString(), smtpAddress, (adrecipient != null) ? adrecipient.LegacyExchangeDN : text.ToString(), "EX", alias, (adrecipient != null) ? AddressOrigin.Directory : AddressOrigin.Unknown, num, null, EmailAddressIndex.None, adObjectId, renderFlags, null, null) && flag)
				{
					flag = false;
					if ((flags & RecipientWellNode.RenderFlags.RenderCommas) != RecipientWellNode.RenderFlags.None)
					{
						renderFlags |= RecipientWellNode.RenderFlags.RenderCommas;
					}
				}
			}
		}

		private MultiValuedProperty<string> GetAddressList(RecipientWellType type)
		{
			if (type == RecipientWellType.To)
			{
				return this.calendarConfig.BookInPolicy;
			}
			if (type == RecipientWellType.Cc)
			{
				return this.calendarConfig.RequestInPolicy;
			}
			if (type == RecipientWellType.Bcc)
			{
				return this.calendarConfig.RequestOutOfPolicy;
			}
			throw new ArgumentException("invalid RecipientWellType");
		}

		private CalendarConfiguration calendarConfig;
	}
}
