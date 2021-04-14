using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public abstract class ItemRecipientWell : RecipientWell
	{
		internal abstract IEnumerator<Participant> GetRecipientsCollection(RecipientWellType type);

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
			IEnumerator<Participant> recipientsCollection = this.GetRecipientsCollection(type);
			RecipientWellNode.RenderFlags renderFlags = flags & ~RecipientWellNode.RenderFlags.RenderCommas;
			bool flag = true;
			bool flag2 = userContext.IsInstantMessageEnabled();
			Result<ADRawEntry>[] array = null;
			int num = 0;
			while (recipientsCollection.MoveNext())
			{
				Participant participant = recipientsCollection.Current;
				string smtpAddress = null;
				string alias = null;
				string text = null;
				int num2 = 0;
				ADObjectId adObjectId = null;
				string mobilePhoneNumber = null;
				if (participant.RoutingType == "EX" && !string.IsNullOrEmpty(participant.EmailAddress))
				{
					bool flag3 = (flags & RecipientWellNode.RenderFlags.ReadOnly) != RecipientWellNode.RenderFlags.None;
					if (flag3)
					{
						alias = Utilities.GetParticipantProperty<string>(participant, ParticipantSchema.Alias, null);
					}
					bool participantProperty = Utilities.GetParticipantProperty<bool>(participant, ParticipantSchema.IsDistributionList, false);
					if (participantProperty)
					{
						num2 |= 1;
					}
					bool participantProperty2 = Utilities.GetParticipantProperty<bool>(participant, ParticipantSchema.IsRoom, false);
					if (participantProperty2)
					{
						num2 |= 2;
					}
					smtpAddress = Utilities.GetParticipantProperty<string>(participant, ParticipantSchema.SmtpAddress, null);
					if (flag2 && !participantProperty && !participantProperty2)
					{
						text = Utilities.GetParticipantProperty<string>(participant, ParticipantSchema.SipUri, null);
						if (text == null || text.Trim().Length == 0)
						{
							if (array == null)
							{
								array = AdRecipientBatchQuery.FindAdResultsByLegacyExchangeDNs(this.GetRecipientsCollection(type), userContext);
							}
							ADRawEntry data = array[num].Data;
							if (data != null)
							{
								adObjectId = (ADObjectId)data[ADObjectSchema.Id];
								text = InstantMessageUtilities.GetSipUri((ProxyAddressCollection)data[ADRecipientSchema.EmailAddresses]);
								if (text != null && text.Trim().Length == 0)
								{
									text = null;
								}
							}
						}
					}
					if (userContext.IsSmsEnabled)
					{
						if (array == null)
						{
							array = AdRecipientBatchQuery.FindAdResultsByLegacyExchangeDNs(this.GetRecipientsCollection(type), userContext);
						}
						ADRawEntry data2 = array[num].Data;
						if (data2 != null)
						{
							mobilePhoneNumber = (string)data2[ADOrgPersonSchema.MobilePhone];
						}
					}
					num++;
				}
				else if (participant.RoutingType == "SMTP")
				{
					smtpAddress = participant.EmailAddress;
					if (flag2)
					{
						text = participant.EmailAddress;
					}
				}
				else if (string.CompareOrdinal(participant.RoutingType, "MAPIPDL") == 0)
				{
					num2 |= 1;
				}
				StoreObjectId storeObjectId = null;
				EmailAddressIndex emailAddressIndex = EmailAddressIndex.None;
				StoreParticipantOrigin storeParticipantOrigin = participant.Origin as StoreParticipantOrigin;
				if (storeParticipantOrigin != null && storeParticipantOrigin.OriginItemId != null)
				{
					storeObjectId = storeParticipantOrigin.OriginItemId;
					emailAddressIndex = storeParticipantOrigin.EmailAddressIndex;
				}
				if (wellNode(writer, userContext, participant.DisplayName, smtpAddress, participant.EmailAddress, participant.RoutingType, alias, RecipientAddress.ToAddressOrigin(participant), num2, storeObjectId, emailAddressIndex, adObjectId, renderFlags, text, mobilePhoneNumber) && flag)
				{
					flag = false;
					if ((flags & RecipientWellNode.RenderFlags.RenderCommas) != RecipientWellNode.RenderFlags.None)
					{
						renderFlags |= RecipientWellNode.RenderFlags.RenderCommas;
					}
				}
			}
		}

		public override bool HasRecipients(RecipientWellType type)
		{
			IEnumerator<Participant> recipientsCollection = this.GetRecipientsCollection(type);
			return recipientsCollection.MoveNext();
		}
	}
}
