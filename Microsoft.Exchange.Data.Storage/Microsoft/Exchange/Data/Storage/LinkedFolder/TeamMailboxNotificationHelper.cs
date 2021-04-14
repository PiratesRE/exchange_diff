using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TeamMailboxNotificationHelper
	{
		public TeamMailboxNotificationHelper(TeamMailbox tm, IRecipientSession dataSession)
		{
			if (tm == null)
			{
				throw new ArgumentNullException("tm");
			}
			if (dataSession == null)
			{
				throw new ArgumentNullException("dataSession");
			}
			this.tm = tm;
			this.dataSession = dataSession;
			this.tmSmtpAddress = this.tm.PrimarySmtpAddress.ToString();
		}

		public static void SendWelcomeMessageIfNeeded(MailboxSession originalSession)
		{
			if (!string.IsNullOrEmpty(originalSession.ClientInfoString) && (originalSession.ClientInfoString.Equals("Client=TeamMailbox;Action=SendWelcomeMessageToSiteMailbox;Interactive=False", StringComparison.OrdinalIgnoreCase) || originalSession.ClientInfoString.Equals("Client=TeamMailbox;Action=GetDiagnostics;Interactive=False", StringComparison.OrdinalIgnoreCase) || originalSession.ClientInfoString.Equals("Client=TeamMailbox;Action=Send_Notification", StringComparison.OrdinalIgnoreCase)))
			{
				return;
			}
			originalSession.Mailbox.Load(new PropertyDefinition[]
			{
				MailboxSchema.SiteMailboxInternalState
			});
			int? siteMailboxInternalState = originalSession.Mailbox.TryGetProperty(InternalSchema.SiteMailboxInternalState) as int?;
			if (siteMailboxInternalState != null)
			{
				if ((siteMailboxInternalState.Value & 1) == 1)
				{
					return;
				}
			}
			try
			{
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(originalSession.MailboxOwner, originalSession.InternalCulture, "Client=TeamMailbox;Action=SendWelcomeMessageToSiteMailbox;Interactive=False"))
				{
					string internetMessageId = "ed590c4ca1674effa0067475ab2b93b2_" + mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress;
					using (MessageItem messageItem = MessageItem.CreateForDelivery(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox), internetMessageId, new ExDateTime?(ExDateTime.MinValue)))
					{
						messageItem.Recipients.Add(new Participant(mailboxSession.MailboxOwner.MailboxInfo.DisplayName, mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), "SMTP"));
						messageItem.From = new Participant(mailboxSession.MailboxOwner);
						IRecipientSession recipientSession = null;
						TeamMailbox teamMailbox = TeamMailboxNotificationHelper.GetTeamMailbox(mailboxSession, out recipientSession);
						TeamMailboxNotificationHelper teamMailboxNotificationHelper = new TeamMailboxNotificationHelper(teamMailbox, recipientSession);
						messageItem.Subject = teamMailboxNotificationHelper.GetSubject(TeamMailboxNotificationType.Welcome);
						using (Stream stream = messageItem.Body.OpenWriteStream(new BodyWriteConfiguration(BodyFormat.TextHtml, Charset.Unicode)))
						{
							using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.Unicode))
							{
								streamWriter.WriteLine(teamMailboxNotificationHelper.GetBody(TeamMailboxNotificationType.Welcome));
							}
						}
						messageItem.AutoResponseSuppress = AutoResponseSuppress.All;
						messageItem.InternetMessageId = internetMessageId;
						messageItem.PropertyBag[InternalSchema.SentTime] = ExDateTime.UtcNow;
						mailboxSession.Deliver(messageItem, ProxyAddress.Parse(ProxyAddressPrefix.Smtp.PrimaryPrefix, mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString()), RecipientItemType.To);
					}
					TeamMailboxNotificationHelper.UpdateWelcomeMessageSentState(siteMailboxInternalState, mailboxSession);
				}
			}
			catch (StoragePermanentException ex)
			{
				if (ex.InnerException != null && ex.InnerException is MapiExceptionDuplicateDelivery)
				{
					using (MailboxSession mailboxSession2 = MailboxSession.OpenAsAdmin(originalSession.MailboxOwner, originalSession.Culture, "Client=TeamMailbox;Action=SendWelcomeMessageToSiteMailbox;Interactive=False"))
					{
						TeamMailboxNotificationHelper.UpdateWelcomeMessageSentState(siteMailboxInternalState, mailboxSession2);
						goto IL_27D;
					}
					goto IL_27B;
					IL_27D:
					return;
				}
				IL_27B:
				throw;
			}
		}

		public List<Exception> SendNotification(IList<ADObjectId> recipients, TeamMailboxNotificationType type)
		{
			return this.SendNotification(recipients, this.GetSubject(type), this.GetBody(type), RemotingOptions.LocalConnectionsOnly);
		}

		public List<Exception> SendNotification(IList<ADObjectId> recipients, string subject, string body, RemotingOptions remotingOptons = RemotingOptions.LocalConnectionsOnly)
		{
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (body == null)
			{
				throw new ArgumentNullException("body");
			}
			List<Exception> list = new List<Exception>();
			if (recipients.Count > 0)
			{
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(ExchangePrincipal.FromADUser((ADUser)this.tm.DataObject, remotingOptons), CultureInfo.InvariantCulture, "Client=TeamMailbox;Action=Send_Notification"))
				{
					using (MessageItem messageItem = MessageItem.Create(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts)))
					{
						foreach (ADObjectId adobjectId in recipients)
						{
							Exception ex = null;
							ADUser aduser = TeamMailboxADUserResolver.Resolve(this.dataSession, adobjectId, out ex);
							if (ex != null)
							{
								list.Add(new Exception(string.Format("When resolving recipient {0}, got an excetion: {1}", adobjectId, ex)));
							}
							else if (aduser == null)
							{
								list.Add(new Exception("Cannot find recipient: " + adobjectId));
							}
							else
							{
								messageItem.Recipients.Add(new Participant(aduser.DisplayName, aduser.PrimarySmtpAddress.ToString(), "SMTP"));
							}
						}
						messageItem.Subject = subject;
						using (Stream stream = messageItem.Body.OpenWriteStream(new BodyWriteConfiguration(BodyFormat.TextHtml, Charset.Unicode)))
						{
							using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.Unicode))
							{
								streamWriter.WriteLine(body);
							}
						}
						messageItem.AutoResponseSuppress = AutoResponseSuppress.All;
						messageItem.SendWithoutSavingMessage();
					}
				}
			}
			return list;
		}

		private static string GetLink(Uri link, string str)
		{
			string link2 = (link == null) ? string.Empty : link.AbsoluteUri;
			return TeamMailboxNotificationHelper.GetLink(link2, str);
		}

		private static string GetLink(string link, string str)
		{
			return string.Format("<a href=\"{0}\" target=_blank>{1}</a>", link, str);
		}

		private static void UpdateWelcomeMessageSentState(int? siteMailboxInternalState, MailboxSession session)
		{
			int num = (siteMailboxInternalState == null) ? 1 : (siteMailboxInternalState.Value | 1);
			session.Mailbox.SetProperties(new PropertyDefinition[]
			{
				MailboxSchema.SiteMailboxInternalState
			}, new object[]
			{
				num
			});
			CoreObject.GetPersistablePropertyBag((CoreMailboxObject)session.Mailbox.CoreObject).FlushChanges();
		}

		private static TeamMailbox GetTeamMailbox(MailboxSession session, out IRecipientSession recipientSession)
		{
			TeamMailbox result;
			try
			{
				ADUser aduser = DirectoryHelper.ReadADRecipient(session.MailboxOwner.MailboxInfo.MailboxGuid, session.MailboxOwner.MailboxInfo.IsArchive, session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid)) as ADUser;
				if (aduser == null)
				{
					throw new StorageTransientException(new LocalizedString("Failed to find the Site Mailbox"));
				}
				recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, CultureInfo.InvariantCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(session.MailboxOwner.MailboxInfo.OrganizationId), 382, "GetTeamMailbox", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\LinkedFolder\\TeamMailboxNotificationHelper.cs");
				result = TeamMailbox.FromDataObject(aduser);
			}
			catch (ADTransientException innerException)
			{
				throw new StorageTransientException(new LocalizedString("Failed to get the Site Mailbox because of AD error"), innerException);
			}
			catch (ADExternalException innerException2)
			{
				throw new StorageTransientException(new LocalizedString("Failed to get the Site Mailbox because of AD error"), innerException2);
			}
			catch (ADOperationException innerException3)
			{
				throw new StorageTransientException(new LocalizedString("Failed to get the Site Mailbox because of AD error"), innerException3);
			}
			catch (DataValidationException innerException4)
			{
				throw new StorageTransientException(new LocalizedString("Failed to get the Site Mailbox because of AD error"), innerException4);
			}
			return result;
		}

		private string GetSubject(TeamMailboxNotificationType type)
		{
			string result = string.Empty;
			switch (type)
			{
			case TeamMailboxNotificationType.Closed:
				result = ServerStrings.TeamMailboxMessageClosedSubject(this.tm.DisplayName);
				break;
			case TeamMailboxNotificationType.Reactivated:
				result = ServerStrings.TeamMailboxMessageReactivatedSubject(this.tm.DisplayName);
				break;
			case TeamMailboxNotificationType.MemberInvitation:
				result = ServerStrings.TeamMailboxMessageMemberInvitationSubject(this.tm.DisplayName);
				break;
			case TeamMailboxNotificationType.Welcome:
				result = ServerStrings.TeamMailboxMessageWelcomeSubject(this.tm.DisplayName);
				break;
			}
			return result;
		}

		private string GetBody(TeamMailboxNotificationType type)
		{
			string text = string.Empty;
			string text2 = this.tm.DisplayName;
			text2 = text2.Replace("<", "&lt;");
			text2 = text2.Replace(">", "&gt;");
			string str;
			if (this.tm.SharePointUrl == null)
			{
				str = ServerStrings.TeamMailboxMessageNotConnectedToSite;
			}
			else
			{
				string str2 = this.tm.SharePointUrl.ToString();
				str = TeamMailboxNotificationHelper.GetLink(this.tm.SharePointUrl, str2);
			}
			string link = TeamMailboxNotificationHelper.GetLink("mailto:" + this.tmSmtpAddress, this.tmSmtpAddress);
			switch (type)
			{
			case TeamMailboxNotificationType.Closed:
				text = string.Format("<P>{0}</P>", ServerStrings.TeamMailboxMessageClosedBodyIntroText);
				text += string.Format("<P><B>{0}</B></P>\r\n                            <UL>\r\n                                <LI>{1}</LI>\r\n                                <LI>{2}</LI>\r\n                                <LI>{3}</LI>\r\n                            </UL>", new object[]
				{
					ServerStrings.TeamMailboxMessageWhatYouCanDoNext,
					ServerStrings.TeamMailboxMessageNoActionText,
					ServerStrings.TeamMailboxMessageReactivatingText,
					ServerStrings.TeamMailboxMessageLearnMore + TeamMailboxNotificationHelper.GetLink(TeamMailboxNotificationHelper.HelpLinkReactivateClosedTeamMailboxes, ServerStrings.TeamMailboxMessageReopenClosedSiteMailbox)
				});
				text += string.Format("<P><B>{0}</B></P>\r\n                            <UL>\r\n                                <LI>{1}</LI>\r\n                                <LI>{2}</LI>\r\n                            </UL>", ServerStrings.TeamMailboxMessageSiteAndSiteMailboxDetails, ServerStrings.TeamMailboxMessageGoToTheSite + str, ServerStrings.TeamMailboxMessageSiteMailboxEmailAddress + link);
				break;
			case TeamMailboxNotificationType.Reactivated:
				text = string.Format("<P>{0}</P>", ServerStrings.TeamMailboxMessageReactivatedBodyIntroText);
				text += string.Format("<P><B>{0}</B></P>\r\n                            <UL>\r\n                                <LI>{1}</LI>\r\n                                <LI>{2}</LI>\r\n                            </UL>", ServerStrings.TeamMailboxMessageWhatYouCanDoNext, ServerStrings.TeamMailboxMessageGoToTheSite + str, ServerStrings.TeamMailboxMessageSendMailToTheSiteMailbox + link);
				break;
			case TeamMailboxNotificationType.MemberInvitation:
			case TeamMailboxNotificationType.Welcome:
				text = string.Format("<P>{0}</P>", string.Format(ServerStrings.TeamMailboxMessageMemberInvitationBodyIntroText, link, TeamMailboxNotificationHelper.GetLink(this.tm.SharePointUrl, text2)));
				text += string.Format("<P>{0}</P>", ServerStrings.TeamMailboxMessageToLearnMore + TeamMailboxNotificationHelper.GetLink(TeamMailboxNotificationHelper.HelpLinkLearnMoreWhatAreTeamMailboxes, ServerStrings.TeamMailboxMessageWhatIsSiteMailbox));
				break;
			}
			return string.Format("<html><body><font face='Segoe UI'>{0}</font></body></html>", text);
		}

		private const int BatchSize = 50;

		private static readonly Uri HelpLinkHideTeamMailboxFromYourOutlookView = new Uri("http://go.microsoft.com/fwlink/?LinkId=238722");

		private static readonly Uri HelpLinkSeeAllOfYourTeamMailboxes = new Uri("http://go.microsoft.com/fwlink/?LinkId=238724");

		private static readonly Uri HelpLinkLearnMoreWhatAreTeamMailboxes = new Uri("http://go.microsoft.com/fwlink/?LinkId=238725");

		private static readonly Uri HelpLinkTeamMailboxTipsAndTricks = new Uri("http://go.microsoft.com/fwlink/?LinkId=238726");

		private static readonly Uri HelpLinkReactivateClosedTeamMailboxes = new Uri("http://go.microsoft.com/fwlink/?LinkId=238727");

		private static readonly Uri HelpLinkLearnMoreTeamMailboxLifecycle = new Uri("http://go.microsoft.com/fwlink/?LinkId=238728");

		private readonly string tmSmtpAddress;

		private readonly TeamMailbox tm;

		private readonly IRecipientSession dataSession;
	}
}
