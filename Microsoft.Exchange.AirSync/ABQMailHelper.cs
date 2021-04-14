using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.AirSync
{
	internal class ABQMailHelper
	{
		public ABQMailHelper(IGlobalInfo globalInfo, IAirSyncContext context, IOrganizationSettingsData organizationSettings)
		{
			if (globalInfo == null)
			{
				throw new ArgumentNullException("globalInfo");
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (organizationSettings == null)
			{
				throw new ArgumentNullException("organizationSettings");
			}
			this.globalInfo = globalInfo;
			this.context = context;
			this.organizationSettings = organizationSettings;
		}

		internal void SendABQNotificationMail()
		{
			using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(this.context.User.ExchangePrincipal, this.context.Request.Culture, "Client=ActiveSync;Action=ABQMail"))
			{
				this.cultureInfo = mailboxSession.PreferedCulture;
				MicrosoftExchangeRecipient exchangeRecipient = this.organizationSettings.GetExchangeRecipient();
				StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
				using (MessageItem messageItem = MessageItem.Create(mailboxSession, defaultFolderId))
				{
					this.ConstructUserNotificationMail(messageItem);
					messageItem.From = ((exchangeRecipient == null) ? new Participant(mailboxSession.MailboxOwner) : new Participant(exchangeRecipient));
					messageItem.Recipients.Add(new Participant(mailboxSession.MailboxOwner), RecipientItemType.To);
					messageItem[MessageItemSchema.IsDraft] = false;
					messageItem[MessageItemSchema.IsRead] = false;
					messageItem.Save(SaveMode.NoConflictResolution);
					messageItem.Load(new PropertyDefinition[0]);
					this.globalInfo.ABQMailId = messageItem.Id.ObjectId;
					this.globalInfo.ABQMailState = ABQMailState.MailPosted;
				}
				defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts);
				if (this.globalInfo.DeviceAccessState == DeviceAccessState.Quarantined)
				{
					using (MessageItem messageItem2 = MessageItem.Create(mailboxSession, defaultFolderId))
					{
						foreach (SmtpAddress smtpAddress in this.organizationSettings.AdminMailRecipients)
						{
							Participant participant;
							if (smtpAddress.IsValidAddress && Participant.TryParse(smtpAddress.ToString(), out participant) && participant != null)
							{
								messageItem2.Recipients.Add(participant, RecipientItemType.To);
							}
						}
						if (messageItem2.Recipients.Count > 0)
						{
							this.ConstructAdminNotificationMail(messageItem2);
							messageItem2.From = ((exchangeRecipient == null) ? new Participant(mailboxSession.MailboxOwner) : new Participant(exchangeRecipient));
							messageItem2.SendWithoutSavingMessage();
						}
						else
						{
							AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, null, "ABQMail:No valid AdminMailRecipients to send Admin mail to!");
							if (VariantConfiguration.InvariantNoFlightingSnapshot.ActiveSync.ActiveSyncDiagnosticsLogABQPeriodicEvent.Enabled)
							{
								AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_NoAdminMailRecipientsError, "NoAdminMailRecipients", new string[0]);
							}
						}
					}
				}
			}
		}

		internal void SendMdmQuarantineEmail(bool isEnrolled)
		{
			using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(this.context.User.ExchangePrincipal, this.context.Request.Culture, "Client=ActiveSync;Action=MDMQuarantineMail"))
			{
				this.cultureInfo = mailboxSession.PreferedCulture;
				MicrosoftExchangeRecipient exchangeRecipient = this.organizationSettings.GetExchangeRecipient();
				StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
				using (MessageItem messageItem = MessageItem.CreateAssociated(mailboxSession, defaultFolderId))
				{
					string empty = string.Empty;
					string empty2 = string.Empty;
					this.ConstructMDMQuarantineNotificationMail(isEnrolled, out empty, out empty2);
					messageItem.Subject = empty;
					using (TextWriter textWriter = messageItem.Body.OpenTextWriter(BodyFormat.TextHtml))
					{
						textWriter.Write(empty2);
					}
					messageItem.From = ((exchangeRecipient == null) ? new Participant(mailboxSession.MailboxOwner) : new Participant(exchangeRecipient));
					messageItem.Recipients.Add(new Participant(mailboxSession.MailboxOwner), RecipientItemType.To);
					messageItem[MessageItemSchema.IsDraft] = false;
					messageItem[MessageItemSchema.IsRead] = false;
					messageItem.Save(SaveMode.NoConflictResolution);
					messageItem.Load();
					this.context.ProtocolLogger.AppendValue(ProtocolLoggerData.GraphApiCallData, string.Format("qes:{0}", true));
					this.globalInfo.ABQMailId = messageItem.Id.ObjectId;
					this.globalInfo.ABQMailState = ABQMailState.MailPosted;
				}
			}
		}

		internal void ConstructMDMQuarantineNotificationMail(bool IsEnrolled, out string messageSubject, out string messageItemBody)
		{
			messageSubject = (IsEnrolled ? Strings.MDMNonCompliantMailSubject.ToString(this.cultureInfo) : Strings.MDMQuarantinedMailSubject.ToString(this.cultureInfo));
			StringBuilder stringBuilder = new StringBuilder("<html>");
			stringBuilder.Append("<style>\r\n        body\r\n        {\r\n            font-family: Tahoma;\r\n            background-color: rgb(255,255,255);\r\n            color: #000000; font-size:x-small;\r\n            width: 600px;\r\n        }\r\n        p\r\n        {\r\n            margin-left:0px ;\r\n            margin-bottom:8px\r\n        }\r\n        table\r\n        {\r\n            font-family: Tahoma;\r\n            background-color: rgb(255,255,255);\r\n            color: #000000;\r\n            font-size:x-small;\r\n            border:0px ;\r\n            text-align:left;\r\n            margin-left:20px\r\n        }\r\n        </style>");
			stringBuilder.Append("<body>");
			DeviceIdentity deviceIdentity = this.context.DeviceIdentity;
			OrganizationId organizationId = this.context.User.OrganizationId;
			string text = (organizationId == OrganizationId.ForestWideOrgId) ? string.Empty : organizationId.OrganizationalUnit.Name;
			Guid guid = Guid.NewGuid();
			if (IsEnrolled)
			{
				stringBuilder.AppendFormat("<p>{0}</p>", AirSyncUtility.HtmlEncode(Strings.MDMNonCompliantMailBody, this.cultureInfo, false));
				stringBuilder.AppendFormat("<a href=\"{0}\">{1}</a>", AirSyncUtility.HtmlEncode(string.Format(GlobalSettings.MDMComplianceStatusUrl.AbsoluteUri, deviceIdentity.DeviceId), false), AirSyncUtility.HtmlEncode(Strings.MDMNonCompliantMailBodyLinkText, this.cultureInfo, false));
			}
			else if (GlobalSettings.DeviceTypesWithBasicMDMNotification.Contains(deviceIdentity.DeviceType.ToLower()))
			{
				stringBuilder.AppendFormat("<p>{0}</p>", AirSyncUtility.HtmlEncode(Strings.MDMQuarantinedMailBody, this.cultureInfo, false));
				stringBuilder.AppendLine(AirSyncUtility.HtmlEncode(Strings.MDMQuarantinedMailBodyStep1, this.cultureInfo, false));
				stringBuilder.AppendFormat("<a href=\"{0}\">{1}</a>", AirSyncUtility.HtmlEncode(string.Format(GlobalSettings.MdmEnrollmentUrlWithBasicSteps.ToString(), deviceIdentity.DeviceId), false), AirSyncUtility.HtmlEncode(Strings.MDMQuarantinedMailBodyEnrollLink, this.cultureInfo, false));
				stringBuilder.Append("<br/>");
				stringBuilder.AppendLine(AirSyncUtility.HtmlEncode(Strings.MDMQuarantinedMailBodyStep2, this.cultureInfo, false));
				stringBuilder.AppendFormat("<a href=\"{0}\">{1}</a>", AirSyncUtility.HtmlEncode(string.Format(GlobalSettings.MDMComplianceStatusUrl.AbsoluteUri, deviceIdentity.DeviceId), false), AirSyncUtility.HtmlEncode(Strings.MDMNonCompliantMailBodyLinkText, this.cultureInfo, false));
				string arg = string.Format("<a href=\"{0}\">{1}</a>", AirSyncUtility.HtmlEncode(string.Format(GlobalSettings.MdmActivationUrlWithBasicSteps, deviceIdentity.DeviceId), false), AirSyncUtility.HtmlEncode(Strings.MDMQuarantinedMailBodyRetryLink, this.cultureInfo, false));
				stringBuilder.AppendFormat("<p>{0}</p>", string.Format(Strings.MDMQuarantinedMailBasicRetryText.ToString(this.cultureInfo), arg));
			}
			else
			{
				stringBuilder.AppendFormat("<p>{0}</p>", AirSyncUtility.HtmlEncode(Strings.MDMQuarantinedMailBody, this.cultureInfo, false));
				stringBuilder.AppendLine(AirSyncUtility.HtmlEncode(Strings.MDMQuarantinedMailBodyStep1, this.cultureInfo, false));
				stringBuilder.AppendFormat("<a href=\"{0}\">{1}</a>", AirSyncUtility.HtmlEncode(string.Format(GlobalSettings.MDMEnrollmentUrl.AbsoluteUri, deviceIdentity.DeviceId), false), AirSyncUtility.HtmlEncode(Strings.MDMQuarantinedMailBodyEnrollLink, this.cultureInfo, false));
				stringBuilder.Append("<br/>");
				stringBuilder.AppendLine(AirSyncUtility.HtmlEncode(Strings.MDMQuarantinedMailBodyStep2, this.cultureInfo, false));
				stringBuilder.AppendFormat("<a href=\"{0}\">{1}</a>", AirSyncUtility.HtmlEncode(string.Format(GlobalSettings.MDMActivationUrl, new object[]
				{
					GlobalSettings.ADRegistrationServiceUrl,
					text,
					deviceIdentity.DeviceId,
					guid
				}), false), AirSyncUtility.HtmlEncode(Strings.MDMQuarantinedMailBodyActivateLink, this.cultureInfo, false));
				stringBuilder.Append("<br/>");
				stringBuilder.AppendLine(AirSyncUtility.HtmlEncode(Strings.MDMQuarantinedMailBodyStep3, this.cultureInfo, false));
				stringBuilder.AppendFormat("<a href=\"{0}\">{1}</a>", AirSyncUtility.HtmlEncode(string.Format(GlobalSettings.MDMComplianceStatusUrl.AbsoluteUri, deviceIdentity.DeviceId), false), AirSyncUtility.HtmlEncode(Strings.MDMNonCompliantMailBodyLinkText, this.cultureInfo, false));
				AirSyncDiagnostics.TraceDebug<Guid>(ExTraceGlobals.RequestsTracer, null, "ABQMail:MdmQuarantine email traceID {0}", guid);
				this.context.ProtocolLogger.AppendValue(ProtocolLoggerData.GraphApiCallData, string.Format("tId:{0}", guid));
			}
			stringBuilder.Append("</body></html>");
			messageItemBody = stringBuilder.ToString();
		}

		internal void SendAutoBlockNotificationMail(TimeSpan blockTime, string adminEmailInsert)
		{
			using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(this.context.User.ExchangePrincipal, this.context.Request.Culture, "Client=ActiveSync;Action=AutoBlockMail"))
			{
				this.cultureInfo = mailboxSession.PreferedCulture;
				MicrosoftExchangeRecipient exchangeRecipient = this.organizationSettings.GetExchangeRecipient();
				StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
				using (MessageItem messageItem = MessageItem.Create(mailboxSession, defaultFolderId))
				{
					this.ConstructAutoBlockNotificationMail(messageItem, blockTime, adminEmailInsert);
					messageItem.From = ((exchangeRecipient == null) ? new Participant(mailboxSession.MailboxOwner) : new Participant(exchangeRecipient));
					messageItem.Recipients.Add(new Participant(mailboxSession.MailboxOwner), RecipientItemType.To);
					messageItem[MessageItemSchema.IsDraft] = false;
					messageItem[MessageItemSchema.IsRead] = false;
					messageItem.Save(SaveMode.NoConflictResolution);
				}
			}
		}

		private void ConstructUserNotificationMail(MessageItem messageToUser)
		{
			StringBuilder stringBuilder = new StringBuilder("<html>");
			stringBuilder.Append("<style>\r\n        body\r\n        {\r\n            font-family: Tahoma;\r\n            background-color: rgb(255,255,255);\r\n            color: #000000; font-size:x-small;\r\n            width: 600px;\r\n        }\r\n        p\r\n        {\r\n            margin-left:0px ;\r\n            margin-bottom:8px\r\n        }\r\n        table\r\n        {\r\n            font-family: Tahoma;\r\n            background-color: rgb(255,255,255);\r\n            color: #000000;\r\n            font-size:x-small;\r\n            border:0px ;\r\n            text-align:left;\r\n            margin-left:20px\r\n        }\r\n        </style>");
			stringBuilder.Append("<body>");
			switch (this.globalInfo.DeviceAccessState)
			{
			case DeviceAccessState.Blocked:
				messageToUser.Subject = Strings.AccessBlockedMailSubject.ToString(this.cultureInfo);
				stringBuilder.AppendFormat("<p>{0}</p>", AirSyncUtility.HtmlEncode(Strings.AccessBlockedMailBody1.ToString(this.cultureInfo), false));
				break;
			case DeviceAccessState.Quarantined:
				messageToUser.Subject = Strings.QuarantinedMailSubject.ToString(this.cultureInfo);
				stringBuilder.AppendFormat("<p>{0}</p>", AirSyncUtility.HtmlEncode(Strings.QuarantinedMailBody1.ToString(this.cultureInfo), false));
				break;
			case DeviceAccessState.DeviceDiscovery:
				messageToUser.Subject = Strings.DeviceDiscoveryMailSubject.ToString(this.cultureInfo);
				stringBuilder.AppendFormat("<p>{0}</p>", AirSyncUtility.HtmlEncode(Strings.DeviceDiscoveryMailBody1.ToString(this.cultureInfo), false));
				break;
			default:
				throw new InvalidOperationException(string.Format("Don't know how to construct user mail for AccessState: {0}", this.globalInfo.DeviceAccessState));
			}
			if (!string.IsNullOrEmpty(this.organizationSettings.UserMailInsert))
			{
				stringBuilder.AppendFormat("<p>{0}</p>", this.organizationSettings.UserMailInsert);
			}
			stringBuilder.Append("<p>");
			stringBuilder.Append(AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceInformation.ToString(this.cultureInfo), false));
			stringBuilder.Append(this.ConstructUserMailDeviceInformationTable());
			stringBuilder.Append("</p>");
			stringBuilder.AppendFormat("<p>{0}</p>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodySentAt(ExDateTime.Now.ToString(this.cultureInfo), this.context.User.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()).ToString(this.cultureInfo), false));
			stringBuilder.Append("</body></html>");
			using (TextWriter textWriter = messageToUser.Body.OpenTextWriter(BodyFormat.TextHtml))
			{
				textWriter.Write(stringBuilder.ToString());
			}
		}

		private void ConstructAutoBlockNotificationMail(MessageItem messageToUser, TimeSpan blockTime, string adminEmailInsert)
		{
			messageToUser.Subject = Strings.AutoBlockedMailSubject.ToString(this.cultureInfo);
			StringBuilder stringBuilder = new StringBuilder("<html>");
			stringBuilder.Append("<style>\r\n        body\r\n        {\r\n            font-family: Tahoma;\r\n            background-color: rgb(255,255,255);\r\n            color: #000000; font-size:x-small;\r\n            width: 600px;\r\n        }\r\n        p\r\n        {\r\n            margin-left:0px ;\r\n            margin-bottom:8px\r\n        }\r\n        table\r\n        {\r\n            font-family: Tahoma;\r\n            background-color: rgb(255,255,255);\r\n            color: #000000;\r\n            font-size:x-small;\r\n            border:0px ;\r\n            text-align:left;\r\n            margin-left:20px\r\n        }\r\n        </style>");
			stringBuilder.Append("<body>");
			stringBuilder.AppendFormat("<p>{0}</p>", AirSyncUtility.HtmlEncode(string.Format(Strings.AutoBlockedMailBody1.ToString(this.cultureInfo), (int)blockTime.TotalHours + 1), false));
			if (!string.IsNullOrEmpty(adminEmailInsert))
			{
				stringBuilder.AppendFormat("<p>{0}</p>", adminEmailInsert);
			}
			stringBuilder.Append("<p>");
			stringBuilder.Append(AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceInformation.ToString(this.cultureInfo), false));
			stringBuilder.Append(this.ConstructUserMailDeviceInformationTable());
			stringBuilder.Append("</p>");
			stringBuilder.AppendFormat("<p>{0}</p>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodySentAt(ExDateTime.Now.ToString(this.cultureInfo), this.context.User.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()).ToString(this.cultureInfo), false));
			stringBuilder.Append("</body></html>");
			using (TextWriter textWriter = messageToUser.Body.OpenTextWriter(BodyFormat.TextHtml))
			{
				textWriter.Write(stringBuilder.ToString());
			}
		}

		private Uri GetUserActiveSyncOptionsPageUrl()
		{
			Uri result = null;
			ExchangePrincipal exchangePrincipal = this.context.User.ExchangePrincipal;
			Uri ecpUrl = this.GetEcpUrl(exchangePrincipal);
			if (ecpUrl != null)
			{
				string text;
				if (!GlobalSettings.IsMultiTenancyEnabled)
				{
					text = string.Format("UsersGroups/EditMobileMailbox.aspx?id={0}&dtm=Isolation", exchangePrincipal.MailboxInfo.MailboxGuid.ToString());
				}
				else
				{
					text = string.Format("UsersGroups/EditMobileMailbox.aspx?id={0}&dtm=Isolation&Realm={1}&exsvurl=1", exchangePrincipal.MailboxInfo.MailboxGuid.ToString(), AirSyncUtility.DefaultAcceptedDomainName);
				}
				text = Path.Combine(ecpUrl.AbsolutePath, text);
				result = new Uri(ecpUrl, text);
			}
			return result;
		}

		private Uri GetEcpUrl(ExchangePrincipal mailbox)
		{
			Uri uri = null;
			Exception ex = null;
			try
			{
				if (mailbox.MailboxInfo.Location.ServerVersion >= Server.E15MinVersion && GlobalSettings.IsMultiTenancyEnabled)
				{
					uri = FrontEndLocator.GetFrontEndEcpUrl(mailbox);
					AirSyncDiagnostics.TraceDebug<ExchangePrincipal, Uri>(ExTraceGlobals.RequestsTracer, null, "ABQMail:ECP URL for user {0} - {1} (E15 Multitenancy)", mailbox, uri);
				}
				else
				{
					ServiceTopology serviceTopology = GlobalSettings.IsMultiTenancyEnabled ? ServiceTopology.GetCurrentLegacyServiceTopology("f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\ABQMailHelper.cs", "GetEcpUrl", 605) : ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\ABQMailHelper.cs", "GetEcpUrl", 605);
					ServerVersion serverVersion = new ServerVersion(mailbox.MailboxInfo.Location.ServerVersion);
					int majorversion = serverVersion.Major;
					IList<EcpService> list = serviceTopology.FindAll<EcpService>(mailbox, ClientAccessType.External, (EcpService service) => new ServerVersion(service.ServerVersionNumber).Major == majorversion, "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\ABQMailHelper.cs", "GetEcpUrl", 610);
					foreach (EcpService ecpService in list)
					{
						if (ecpService != null && ecpService.Url != null)
						{
							uri = ecpService.Url;
							AirSyncDiagnostics.TraceDebug<ExchangePrincipal, Uri>(ExTraceGlobals.RequestsTracer, null, "ABQMail:ECP URL for user {0} - {1} (not E15 Multitenancy)", mailbox, uri);
							break;
						}
					}
				}
			}
			catch (ServerNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (ADTransientException ex3)
			{
				ex = ex3;
			}
			catch (DataSourceOperationException ex4)
			{
				ex = ex4;
			}
			catch (DataValidationException ex5)
			{
				ex = ex5;
			}
			finally
			{
				if (ex != null)
				{
					AirSyncDiagnostics.TraceError<ExchangePrincipal, Exception>(ExTraceGlobals.RequestsTracer, null, "ABQMail:Could not retrieve ECP URL for user {0} - {1}", mailbox, ex);
				}
			}
			return uri;
		}

		private void ConstructAdminNotificationMail(MessageItem messageToAdmin)
		{
			StringBuilder stringBuilder = new StringBuilder("<html>");
			stringBuilder.Append("<style>\r\n        body\r\n        {\r\n            font-family: Tahoma;\r\n            background-color: rgb(255,255,255);\r\n            color: #000000; font-size:x-small;\r\n            width: 600px;\r\n        }\r\n        p\r\n        {\r\n            margin-left:0px ;\r\n            margin-bottom:8px\r\n        }\r\n        table\r\n        {\r\n            font-family: Tahoma;\r\n            background-color: rgb(255,255,255);\r\n            color: #000000;\r\n            font-size:x-small;\r\n            border:0px ;\r\n            text-align:left;\r\n            margin-left:20px\r\n        }\r\n        </style>");
			stringBuilder.Append("<body>");
			messageToAdmin.Subject = EASServerStrings.AdminMailSubject(this.context.User.ADUser.DisplayName, this.context.User.ADUser.Alias);
			stringBuilder.AppendFormat("<p>{0}</p>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailBody1, false));
			Uri userActiveSyncOptionsPageUrl = this.GetUserActiveSyncOptionsPageUrl();
			if (userActiveSyncOptionsPageUrl == null)
			{
				stringBuilder.AppendFormat("<p>{0}</p>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailBody2, false));
			}
			else
			{
				string arg = string.Format("<a href=\"{0}\">{1}</a>", userActiveSyncOptionsPageUrl.AbsoluteUri, userActiveSyncOptionsPageUrl.AbsoluteUri);
				string arg2 = string.Format(AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailBody4, false), arg);
				stringBuilder.AppendFormat("<p>{0}</p>", arg2);
			}
			stringBuilder.Append("<p>");
			stringBuilder.Append(AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailDeviceInformation, false));
			stringBuilder.Append("<table>");
			stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailUser, false), AirSyncUtility.HtmlEncode(this.context.User.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), false));
			stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailBodyDeviceModel, false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceModel, false));
			stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailBodyDeviceType, false), AirSyncUtility.HtmlEncode(this.context.DeviceIdentity.DeviceType, false));
			stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailBodyDeviceID, false), AirSyncUtility.HtmlEncode(this.context.DeviceIdentity.DeviceId, false));
			stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailBodyDeviceOS, false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceOS, false));
			stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailBodyDeviceUserAgent, false), AirSyncUtility.HtmlEncode(this.globalInfo.UserAgent, false));
			stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailDevicePhoneNumber, false), AirSyncUtility.HtmlEncode(DeviceInfo.ObfuscatePhoneNumber(this.globalInfo.DevicePhoneNumber), false));
			stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailBodyDeviceIMEI, false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceImei, false));
			stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailBodyEASVersion, false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceActiveSyncVersion, false));
			stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailDevicePolicyApplied, false), (this.globalInfo.DevicePolicyApplied != null) ? AirSyncUtility.HtmlEncode(this.globalInfo.DevicePolicyApplied.ToString(), false) : null);
			stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailDevicePolicyStatus, false), AirSyncUtility.HtmlEncode(this.globalInfo.DevicePolicyApplicationStatus.ToString(), false));
			stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailBodyDeviceAccessState, false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceAccessState.ToString(), false));
			stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailBodyDeviceAccessStateReason, false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceAccessStateReason.ToString(), false));
			stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailDeviceAccessControlRule, false), (this.globalInfo.DeviceAccessControlRule != null) ? AirSyncUtility.HtmlEncode(this.globalInfo.DeviceAccessControlRule.ToString(), false) : null);
			stringBuilder.Append("</table></p>");
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (SmtpAddress smtpAddress in this.organizationSettings.AdminMailRecipients)
			{
				stringBuilder2.AppendFormat("{0},", smtpAddress.ToString());
			}
			stringBuilder.AppendFormat("<p>{0}</p>", AirSyncUtility.HtmlEncode(EASServerStrings.AdminMailBodySentAt(ExDateTime.Now.ToString(), stringBuilder2.ToString()), false));
			stringBuilder.Append("</body></html>");
			using (TextWriter textWriter = messageToAdmin.Body.OpenTextWriter(BodyFormat.TextHtml))
			{
				textWriter.Write(stringBuilder.ToString());
			}
		}

		private string ConstructUserMailDeviceInformationTable()
		{
			StringBuilder stringBuilder = new StringBuilder("<table>");
			if (this.globalInfo.DeviceAccessState != DeviceAccessState.DeviceDiscovery)
			{
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceModel.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceModel, false));
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceType.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.context.DeviceIdentity.DeviceType, false));
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceID.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.context.DeviceIdentity.DeviceId, false));
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceOS.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceOS, false));
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceUserAgent.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.globalInfo.UserAgent, false));
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceIMEI.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceImei, false));
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyEASVersion.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceActiveSyncVersion, false));
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceAccessState.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceAccessState.ToString(), false));
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceAccessStateReason.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceAccessStateReason.ToString(), false));
			}
			else
			{
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceType.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.context.DeviceIdentity.DeviceType, false));
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceID.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.context.DeviceIdentity.DeviceId, false));
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceUserAgent.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.context.Request.UserAgent, false));
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyEASVersion.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceActiveSyncVersion, false));
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceAccessState.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceAccessState.ToString(), false));
				stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", AirSyncUtility.HtmlEncode(Strings.ABQMailBodyDeviceAccessStateReason.ToString(this.cultureInfo), false), AirSyncUtility.HtmlEncode(this.globalInfo.DeviceAccessStateReason.ToString(), false));
			}
			stringBuilder.Append("</table>");
			return stringBuilder.ToString();
		}

		private const string ABQMailStyle = "<style>\r\n        body\r\n        {\r\n            font-family: Tahoma;\r\n            background-color: rgb(255,255,255);\r\n            color: #000000; font-size:x-small;\r\n            width: 600px;\r\n        }\r\n        p\r\n        {\r\n            margin-left:0px ;\r\n            margin-bottom:8px\r\n        }\r\n        table\r\n        {\r\n            font-family: Tahoma;\r\n            background-color: rgb(255,255,255);\r\n            color: #000000;\r\n            font-size:x-small;\r\n            border:0px ;\r\n            text-align:left;\r\n            margin-left:20px\r\n        }\r\n        </style>";

		private const string ABQMailBodyParagraph = "<p>{0}</p>";

		private const string ABQMailNewLine = "<br/>";

		private const string ABQMailDeviceInformationTableRow = "<tr><td>{0}</td><td>{1}</td></tr>";

		private const string ABQMailLink = "<a href=\"{0}\">{1}</a>";

		private const string UserActiveSyncOptionsPageExtension = "UsersGroups/EditMobileMailbox.aspx?id={0}&dtm=Isolation";

		private const string UserActiveSyncOptionsPageExtensionInDC = "UsersGroups/EditMobileMailbox.aspx?id={0}&dtm=Isolation&Realm={1}&exsvurl=1";

		private IGlobalInfo globalInfo;

		private IAirSyncContext context;

		private IOrganizationSettingsData organizationSettings;

		private CultureInfo cultureInfo;
	}
}
