using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public abstract class ItemRecipientWell : RecipientWell
	{
		internal ItemRecipientWell(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			this.userContext = userContext;
		}

		protected UserContext UserContext
		{
			get
			{
				return this.userContext;
			}
		}

		internal static void ParseRecipientIdString(string idString, out RecipientItemType recipientItemType, out int index)
		{
			recipientItemType = RecipientItemType.Unknown;
			index = -1;
			if (string.IsNullOrEmpty(idString))
			{
				throw new ArgumentNullException("idString", "idString cannot be null or empty");
			}
			string[] array = idString.Split(new char[]
			{
				','
			});
			if (array.Length != 2)
			{
				throw new FormatException(idString);
			}
			int num;
			if (int.TryParse(array[0], out num))
			{
				recipientItemType = (RecipientItemType)num;
			}
			int num2;
			if (int.TryParse(array[1], out num2))
			{
				index = num2;
			}
		}

		internal abstract IEnumerator<Participant> GetRecipientCollection(RecipientWellType type);

		protected override void RenderContents(TextWriter writer, RecipientWellType type, RecipientWellNode.RenderFlags flags)
		{
			if (!this.HasRecipients(type))
			{
				return;
			}
			AdRecipientBatchQuery adRecipientBatchQuery = new AdRecipientBatchQuery(this.GetRecipientCollection(type), this.UserContext);
			IEnumerator<Participant> recipientCollection = this.GetRecipientCollection(type);
			RecipientWellNode.RenderFlags renderFlags = flags & ~RecipientWellNode.RenderFlags.RenderCommas;
			bool flag = true;
			RecipientItemType recipientItemType = ItemRecipientWell.GetRecipientItemType(type);
			bool isWebPartRequest = OwaContext.Current.UserContext.IsWebPartRequest;
			int num = 0;
			while (recipientCollection.MoveNext())
			{
				Participant participant = recipientCollection.Current;
				int recipientFlags = 0;
				ADObjectId adObjectId = null;
				int readItemType = 1;
				string smtpAddressAndADObjectInfo = ItemRecipientWell.GetSmtpAddressAndADObjectInfo(participant, adRecipientBatchQuery, out adObjectId, out recipientFlags, out readItemType);
				StoreObjectId storeObjectId = null;
				StoreParticipantOrigin storeParticipantOrigin = participant.Origin as StoreParticipantOrigin;
				if (storeParticipantOrigin != null)
				{
					storeObjectId = storeParticipantOrigin.OriginItemId;
				}
				string idString = ItemRecipientWell.BuildRecipientIdString(recipientItemType, num++);
				if (RecipientWellNode.Render(this.UserContext, writer, participant.DisplayName, smtpAddressAndADObjectInfo, participant.EmailAddress, participant.RoutingType, RecipientAddress.ToAddressOrigin(participant), recipientFlags, readItemType, recipientItemType, adObjectId, storeObjectId, renderFlags, idString, isWebPartRequest) && flag)
				{
					flag = false;
					if ((flags & RecipientWellNode.RenderFlags.RenderCommas) != RecipientWellNode.RenderFlags.None)
					{
						renderFlags |= RecipientWellNode.RenderFlags.RenderCommas;
					}
				}
			}
		}

		protected override bool RenderAnrContents(TextWriter writer, UserContext userContext, RecipientWellType type, bool isTableStartRendered)
		{
			bool flag = isTableStartRendered;
			bool flag2 = false;
			if (this.HasRecipients(type))
			{
				AdRecipientBatchQuery adRecipientBatchQuery = new AdRecipientBatchQuery(this.GetRecipientCollection(type), this.UserContext);
				IEnumerator<Participant> recipientCollection = this.GetRecipientCollection(type);
				string empty = string.Empty;
				int num = 0;
				List<RecipientAddress> list = new List<RecipientAddress>();
				RecipientItemType recipientItemType = ItemRecipientWell.GetRecipientItemType(type);
				int num2 = 0;
				while (recipientCollection.MoveNext())
				{
					Participant participant = recipientCollection.Current;
					string text = ItemRecipientWell.BuildRecipientIdString(recipientItemType, num2++);
					if (string.CompareOrdinal(participant.RoutingType, "MAPIPDL") != 0 && (string.IsNullOrEmpty(participant.EmailAddress) || string.IsNullOrEmpty(participant.RoutingType)) && !string.IsNullOrEmpty(participant.DisplayName))
					{
						num = 0;
						ADObjectId adobjectId = null;
						int num3;
						ItemRecipientWell.GetSmtpAddressAndADObjectInfo(participant, adRecipientBatchQuery, out adobjectId, out num, out num3);
						StoreParticipantOrigin storeParticipantOrigin = participant.Origin as StoreParticipantOrigin;
						if (storeParticipantOrigin != null)
						{
							StoreObjectId originItemId = storeParticipantOrigin.OriginItemId;
						}
						if (!flag)
						{
							writer.Write("<table cellspacing=0 cellpadding=0 class=\"anrot\" id=\"tblANR\"><tr><td class=\"msg\"><h1 tabindex=0>");
							writer.Write(LocalizedStrings.GetHtmlEncoded(147143837));
							writer.Write("</h1></td></tr><tr><td class=\"h100\"><table cellspacing=0 cellpadding=0 class=\"anrit\">");
							flag = true;
						}
						if (!flag2)
						{
							string value = null;
							if (this is CalendarItemRecipientWell)
							{
								switch (type)
								{
								case RecipientWellType.To:
									value = LocalizedStrings.GetHtmlEncoded(609567352);
									break;
								case RecipientWellType.Cc:
									value = LocalizedStrings.GetHtmlEncoded(633037671);
									break;
								case RecipientWellType.Bcc:
									value = LocalizedStrings.GetHtmlEncoded(-1107183092);
									break;
								}
							}
							else
							{
								switch (type)
								{
								case RecipientWellType.To:
									value = LocalizedStrings.GetHtmlEncoded(-1105875540);
									break;
								case RecipientWellType.Cc:
									value = LocalizedStrings.GetHtmlEncoded(701860997);
									break;
								case RecipientWellType.Bcc:
									value = LocalizedStrings.GetHtmlEncoded(1482538735);
									break;
								}
							}
							writer.Write("<tr><td class=\"hdr\" tabindex=0>");
							writer.Write(value);
							writer.Write("</td></tr>");
							flag2 = true;
						}
						writer.Write("<tr><td class=\"rcpt\" nowrap><span tabindex=0>");
						Utilities.CropAndRenderText(writer, participant.DisplayName, 24);
						writer.Write("</span> [<a href=\"#\" title=\"");
						writer.Write(Utilities.HtmlEncode(participant.DisplayName));
						writer.Write("\" onClick=\"return onClkRmRcp('{0}')\">{1}</a>]", text, LocalizedStrings.GetHtmlEncoded(341226654));
						writer.Write("</td></tr>");
						list.Clear();
						AnrManager.ResolveOneRecipient(participant.DisplayName, userContext, list);
						if (list.Count == 0)
						{
							RecipientAddress recipientAddress = AnrManager.ResolveAnrStringToOneOffEmail(participant.DisplayName);
							if (recipientAddress != null)
							{
								list.Add(recipientAddress);
							}
						}
						int num4 = 0;
						while (num4 < list.Count && num4 < 100)
						{
							RecipientAddress recipientAddress2 = list[num4];
							if (recipientAddress2.DisplayName != null)
							{
								writer.Write("<tr><td><a href=\"#\" title=\"");
								Utilities.HtmlEncode(recipientAddress2.DisplayName, writer);
								if (recipientAddress2.SmtpAddress != null)
								{
									writer.Write(" [");
									Utilities.HtmlEncode(recipientAddress2.SmtpAddress, writer);
									writer.Write("]");
								}
								writer.Write("\" onClick=\"return onClkAddAnr(this,'");
								ResolvedRecipientDetail resolvedRecipientDetail = new ResolvedRecipientDetail(recipientAddress2);
								resolvedRecipientDetail.RenderConcatenatedDetails(true, writer);
								writer.Write("',");
								writer.Write((int)recipientItemType);
								writer.Write(",'");
								writer.Write(text);
								writer.Write("')\">");
								Utilities.CropAndRenderText(writer, recipientAddress2.DisplayName, 24);
								if (string.IsNullOrEmpty(recipientAddress2.DisplayName) && !string.IsNullOrEmpty(recipientAddress2.SmtpAddress))
								{
									writer.Write("(");
									Utilities.CropAndRenderText(writer, recipientAddress2.SmtpAddress, 24);
									writer.Write(")");
								}
								writer.Write("</a></td></tr>");
							}
							num4++;
						}
					}
				}
			}
			return flag;
		}

		public override bool HasRecipients(RecipientWellType type)
		{
			if (type < RecipientWellType.To)
			{
				throw new ArgumentOutOfRangeException("type", "type has an invalid value");
			}
			IEnumerator<Participant> recipientCollection = this.GetRecipientCollection(type);
			return recipientCollection != null && recipientCollection.MoveNext();
		}

		private static string GetSmtpAddressAndADObjectInfo(Participant recipient, AdRecipientBatchQuery adRecipientBatchQuery, out ADObjectId adObjectId, out int recipientAddressFlags, out int readItemType)
		{
			string result = string.Empty;
			recipientAddressFlags = 0;
			readItemType = 1;
			adObjectId = null;
			if (recipient.Origin is DirectoryParticipantOrigin)
			{
				ADRecipient adRecipient = adRecipientBatchQuery.GetAdRecipient(recipient.EmailAddress);
				if (adRecipient != null)
				{
					adObjectId = adRecipient.Id;
					result = adRecipient.PrimarySmtpAddress.ToString();
					if (adRecipient is IADDistributionList)
					{
						recipientAddressFlags |= 1;
						readItemType = 2;
					}
					else
					{
						readItemType = 1;
					}
					if (DirectoryAssistance.IsADRecipientRoom(adRecipient))
					{
						recipientAddressFlags |= 2;
					}
				}
			}
			else if (recipient.Origin is StoreParticipantOrigin)
			{
				if (Utilities.IsMapiPDL(recipient.RoutingType))
				{
					result = recipient.EmailAddress;
					recipientAddressFlags |= 1;
					readItemType = 4;
				}
				else
				{
					result = recipient.EmailAddress;
					readItemType = 3;
				}
			}
			else
			{
				result = recipient.EmailAddress;
			}
			return result;
		}

		private static RecipientItemType GetRecipientItemType(RecipientWellType type)
		{
			RecipientItemType result;
			switch (type)
			{
			case RecipientWellType.To:
				result = RecipientItemType.To;
				break;
			case RecipientWellType.Cc:
				result = RecipientItemType.Cc;
				break;
			case RecipientWellType.Bcc:
				result = RecipientItemType.Bcc;
				break;
			default:
				result = RecipientItemType.Unknown;
				break;
			}
			return result;
		}

		private static string BuildRecipientIdString(RecipientItemType recipientItemType, int index)
		{
			return string.Format("{0},{1}", (int)recipientItemType, index);
		}

		private const int MaximumAnrResolutionsPerRecipient = 100;

		private const int MaxCharactersInAnrRecipients = 24;

		private readonly UserContext userContext;
	}
}
