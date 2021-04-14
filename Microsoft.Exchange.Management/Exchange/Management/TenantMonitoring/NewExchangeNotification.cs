using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Security;
using System.Text;
using System.Web;
using Microsoft.Exchange.CommonHelpProvider;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.TenantMonitoring
{
	[Cmdlet("New", "ExchangeNotification", SupportsShouldProcess = true)]
	public sealed class NewExchangeNotification : NewTenantADTaskBase<Notification>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public uint EventInstanceId
		{
			get
			{
				return (uint)base.Fields["EventInstanceId"];
			}
			set
			{
				base.Fields["EventInstanceId"] = value;
			}
		}

		[ValidateLength(1, 256)]
		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string EventSource
		{
			get
			{
				return (string)base.Fields["EventSource"];
			}
			set
			{
				base.Fields["EventSource"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int EventCategoryId
		{
			get
			{
				if (base.Fields["EventCategoryId"] == null)
				{
					return 0;
				}
				return (int)base.Fields["EventCategoryId"];
			}
			set
			{
				base.Fields["EventCategoryId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime EventTime
		{
			get
			{
				if (base.Fields["EventTime"] == null)
				{
					return ExDateTime.UtcNow;
				}
				return (ExDateTime)base.Fields["EventTime"];
			}
			set
			{
				base.Fields["EventTime"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateCount(0, 100)]
		public string[] InsertionStrings
		{
			get
			{
				return (string[])base.Fields["InsertionStrings"];
			}
			set
			{
				base.Fields["InsertionStrings"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] NotificationRecipients
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["NotificationRecipients"];
			}
			set
			{
				base.Fields["NotificationRecipients"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime CreationTime
		{
			get
			{
				if (base.Fields["CreationTime"] == null)
				{
					return ExDateTime.UtcNow;
				}
				return (ExDateTime)base.Fields["CreationTime"];
			}
			set
			{
				base.Fields["CreationTime"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PeriodicKey
		{
			get
			{
				return ((string)base.Fields["PeriodicKey"]) ?? string.Empty;
			}
			set
			{
				base.Fields["PeriodicKey"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			this.federatedUser = (ADUser)base.GetDataObject<ADUser>(NewExchangeNotification.FederatedMailboxId, base.TenantGlobalCatalogSession, null, null, new LocalizedString?(Strings.ErrorUserNotFound(NewExchangeNotification.FederatedMailboxId.ToString())), new LocalizedString?(Strings.ErrorUserNotUnique(NewExchangeNotification.FederatedMailboxId.ToString())));
			return new NotificationDataProvider(this.federatedUser, base.SessionSettings);
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 210, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\TenantMonitoring\\NewExchangeNotification.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = true;
			tenantOrTopologyConfigurationSession.UseGlobalCatalog = false;
			ExchangeConfigurationUnit exchangeConfigurationUnit = (ExchangeConfigurationUnit)base.GetDataObject<ExchangeConfigurationUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
			if (exchangeConfigurationUnit.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				throw new ArgumentException("OrganizationId.ForestWideOrgId is not supported.", "Organization");
			}
			return exchangeConfigurationUnit.OrganizationId;
		}

		protected override IConfigurable PrepareDataObject()
		{
			if (this.federatedUser == null)
			{
				base.WriteError(new LocalizedException(Strings.ErrorRecipientNotFound(NewExchangeNotification.FederatedMailboxId.ToString())), ErrorCategory.InvalidArgument, NewExchangeNotification.FederatedMailboxId.ToString());
			}
			IEnumerable<NewExchangeNotification.LookupResult> enumerable = this.LookupSmtpAddressesAndLanguages();
			this.DataObject.EventInstanceId = (int)this.EventInstanceId;
			this.DataObject.EventSource = this.EventSource;
			((NotificationIdentity)this.DataObject.Identity).EventSource = this.EventSource;
			this.DataObject.EventCategoryId = this.EventCategoryId;
			this.DataObject.EventTimeUtc = this.EventTime.ToUtc();
			this.DataObject.InsertionStrings = this.InsertionStrings;
			this.DataObject.CreationTimeUtc = this.CreationTime.ToUtc();
			this.DataObject.EntryType = Utils.ExtractEntryTypeFromInstanceId(this.EventInstanceId);
			this.DataObject.PeriodicKey = this.PeriodicKey;
			if (this.ShouldSendNotificationEmail(enumerable))
			{
				this.DataObject.EmailSent = true;
				IEnumerable<string> enumerable2 = (from r in enumerable
				select r.SmtpAddress.ToString()).Distinct(StringComparer.OrdinalIgnoreCase);
				foreach (string item in enumerable2)
				{
					this.DataObject.NotificationRecipients.Add(item);
				}
				this.notificationEmailInfos = this.ForkRecipientsBasedOnLanguage(enumerable).ToList<NewExchangeNotification.NotificationEmailInfo>();
				IEnumerable<string> enumerable3 = (from e in this.notificationEmailInfos
				select e.MessageId).Distinct(StringComparer.OrdinalIgnoreCase);
				using (IEnumerator<string> enumerator2 = enumerable3.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						string item2 = enumerator2.Current;
						this.DataObject.NotificationMessageIds.Add(item2);
					}
					goto IL_1FB;
				}
			}
			this.DataObject.EmailSent = false;
			IL_1FB:
			return base.PrepareDataObject();
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			HelpProvider.Initialize(HelpProvider.HelpAppName.TenantMonitoring);
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (this.DataObject.EmailSent)
			{
				this.SendEmail();
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(StoragePermanentException).IsInstanceOfType(exception);
		}

		protected override void WriteResult(IConfigurable result)
		{
			Notification notification = result as Notification;
			if (notification == null)
			{
				base.WriteResult(result);
				return;
			}
			notification.EventMessage = notification.InsertionStrings.Aggregate(string.Empty, (string concatenated, string insertionString) => concatenated + insertionString);
			base.WriteResult(result);
		}

		private bool ShouldSendNotificationEmail(IEnumerable<NewExchangeNotification.LookupResult> recipients)
		{
			if (recipients == null || recipients.Count<NewExchangeNotification.LookupResult>() == 0)
			{
				base.WriteVerbose(Strings.TenantNotificationVerboseNotSendingEmailNoRecipients);
				return false;
			}
			switch (((NotificationDataProvider)base.DataSession).RecentNotificationEmailExists(this.DataObject))
			{
			case RecentNotificationEmailTestResult.DailyCapReached:
				base.WriteVerbose(Strings.TenantNotificationVerboseNotSendingEmailDailyCap);
				return false;
			case RecentNotificationEmailTestResult.PastDay:
				base.WriteVerbose(Strings.TenantNotificationVerboseNotSendingEmailPastDay);
				return false;
			}
			base.WriteVerbose(Strings.TenantNotificationVerboseSendingEmail);
			return true;
		}

		private void SendEmail()
		{
			if (this.notificationEmailInfos.Count<NewExchangeNotification.NotificationEmailInfo>() == 0)
			{
				return;
			}
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(ExchangePrincipal.FromADUser(base.SessionSettings, this.federatedUser), CultureInfo.InvariantCulture, "Client=Management;Action=Send-TenantNotification"))
			{
				foreach (NewExchangeNotification.NotificationEmailInfo emailInfo in this.notificationEmailInfos)
				{
					this.SendLocalizedEmail(mailboxSession, emailInfo);
				}
			}
		}

		private void SendLocalizedEmail(MailboxSession session, NewExchangeNotification.NotificationEmailInfo emailInfo)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (emailInfo.Recipients == null || emailInfo.Recipients.Count<SmtpAddress>() == 0)
			{
				return;
			}
			using (MessageItem messageItem = MessageItem.Create(session, session.GetDefaultFolderId(DefaultFolderType.Drafts)))
			{
				foreach (SmtpAddress smtpAddress in emailInfo.Recipients)
				{
					messageItem.Recipients.Add(new Participant(string.Empty, smtpAddress.ToString(), "SMTP"));
				}
				string eventCategory;
				string localizedEventMessageAndCategory = this.GetLocalizedEventMessageAndCategory(emailInfo.Language, out eventCategory);
				messageItem.Subject = Strings.TenantNotificationSubject(eventCategory, this.DataObject.EventDisplayId).ToString(emailInfo.Language);
				using (Stream stream = messageItem.Body.OpenWriteStream(new BodyWriteConfiguration(BodyFormat.TextHtml, Charset.Unicode)))
				{
					using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.Unicode))
					{
						string helpUrlForNotification = this.GetHelpUrlForNotification(emailInfo.Language);
						OrganizationId currentOrganizationId = base.CurrentTaskContext.UserInfo.CurrentOrganizationId;
						streamWriter.WriteLine(Strings.TenantNotificationBody(HttpUtility.HtmlEncode((currentOrganizationId == null) ? string.Empty : currentOrganizationId.GetFriendlyName()), HttpUtility.HtmlEncode(this.DataObject.EventTimeUtc.ToString("f", emailInfo.Language)), (!string.IsNullOrEmpty(localizedEventMessageAndCategory)) ? HttpUtility.HtmlEncode(localizedEventMessageAndCategory) : Strings.TenantNotificationUnavailableEventMessage.ToString(emailInfo.Language), HttpUtility.HtmlEncode(helpUrlForNotification)).ToString(emailInfo.Language));
					}
				}
				messageItem.AutoResponseSuppress = AutoResponseSuppress.All;
				messageItem.InternetMessageId = emailInfo.MessageId;
				messageItem.SendWithoutSavingMessage();
			}
		}

		private string GetLocalizedEventMessageAndCategory(CultureInfo language, out string category)
		{
			string text = string.Empty;
			category = string.Empty;
			try
			{
				text = Utils.GetResourcesFilePath(this.DataObject.EventSource);
				if (string.IsNullOrEmpty(text))
				{
					base.WriteDebug(Strings.TenantNotificationDebugEventResourceFileNotFound(this.DataObject.EventSource));
					return string.Empty;
				}
			}
			catch (IOException exception)
			{
				this.WriteError(exception, ErrorCategory.ResourceUnavailable, this.DataObject.EventSource, false);
				return string.Empty;
			}
			catch (SecurityException exception2)
			{
				this.WriteError(exception2, ErrorCategory.ResourceUnavailable, this.DataObject.EventSource, false);
				return string.Empty;
			}
			catch (UnauthorizedAccessException exception3)
			{
				this.WriteError(exception3, ErrorCategory.ResourceUnavailable, this.DataObject.EventSource, false);
				return string.Empty;
			}
			string result;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.CurrentUICulture))
			{
				string localizedEventMessageAndCategory = Utils.GetLocalizedEventMessageAndCategory(text, (uint)this.DataObject.EventInstanceId, (uint)this.DataObject.EventCategoryId, this.DataObject.InsertionStrings, language, stringWriter, out category);
				if (base.IsDebugOn)
				{
					string text2 = stringWriter.ToString();
					if (!string.IsNullOrEmpty(text2))
					{
						base.WriteDebug(text2);
					}
				}
				result = localizedEventMessageAndCategory;
			}
			return result;
		}

		private CultureInfo GetNotificationEmailFallbackLanguage()
		{
			TransportConfigContainer transportConfigContainer = this.ConfigurationSession.FindSingletonConfigurationObject<TransportConfigContainer>();
			if (transportConfigContainer == null)
			{
				return CultureInfo.CurrentUICulture;
			}
			CultureInfo result;
			if ((result = transportConfigContainer.InternalDsnDefaultLanguage) == null)
			{
				result = (transportConfigContainer.ExternalDsnDefaultLanguage ?? CultureInfo.CurrentUICulture);
			}
			return result;
		}

		private string GetHelpUrlForNotification(CultureInfo language)
		{
			string text = (language != null) ? language.Name : CultureInfo.InvariantCulture.Name;
			Uri uri = HelpProvider.ConstructTenantEventUrl(this.DataObject.EventSource, string.Empty, this.DataObject.EventDisplayId.ToString(CultureInfo.InvariantCulture), text);
			if (uri == null)
			{
				base.WriteDebug(Strings.TenantNotificationDebugHelpProviderReturnedEmptyUrl(this.DataObject.EventSource, this.DataObject.EventDisplayId, text));
				return "http://help.outlook.com/ms.exch.evt.default.aspx";
			}
			return uri.ToString();
		}

		private IEnumerable<NewExchangeNotification.LookupResult> LookupSmtpAddressesAndLanguages()
		{
			if (this.NotificationRecipients == null || this.NotificationRecipients.Length == 0)
			{
				base.WriteVerbose(Strings.TenantNotificationNoNotificationRecipientsSpecified);
				return NewExchangeNotification.EmptyLookupResultsArray;
			}
			LinkedList<NewExchangeNotification.LookupResult> linkedList = new LinkedList<NewExchangeNotification.LookupResult>();
			foreach (RecipientIdParameter recipientIdParameter in this.NotificationRecipients)
			{
				NewExchangeNotification.LookupResult value = NewExchangeNotification.LookupSmtpAddressAndLanguage(base.TenantGlobalCatalogSession, recipientIdParameter);
				if (value.Success)
				{
					linkedList.AddLast(value);
					if (linkedList.Count >= 64)
					{
						break;
					}
				}
				else
				{
					this.WriteWarning(value.ErrorMessage);
				}
			}
			return linkedList;
		}

		private static NewExchangeNotification.LookupResult LookupSmtpAddressAndLanguage(IRecipientSession recipientSession, RecipientIdParameter recipientIdParameter)
		{
			IEnumerable<ADRecipient> objects = recipientIdParameter.GetObjects<ADRecipient>(null, recipientSession);
			ADRecipient adrecipient = null;
			using (IEnumerator<ADRecipient> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					if (SmtpAddress.IsValidSmtpAddress(recipientIdParameter.RawIdentity))
					{
						return new NewExchangeNotification.LookupResult
						{
							Success = true,
							SmtpAddress = SmtpAddress.Parse(recipientIdParameter.RawIdentity),
							Languages = new CultureInfo[0]
						};
					}
					return new NewExchangeNotification.LookupResult
					{
						Success = false,
						ErrorMessage = Strings.NoRecipientsForRecipientId(recipientIdParameter.ToString())
					};
				}
				else
				{
					adrecipient = enumerator.Current;
					if (enumerator.MoveNext())
					{
						return new NewExchangeNotification.LookupResult
						{
							Success = false,
							ErrorMessage = Strings.MoreThanOneRecipientForRecipientId(recipientIdParameter.ToString())
						};
					}
				}
			}
			if (SmtpAddress.Empty.Equals(adrecipient.PrimarySmtpAddress))
			{
				return new NewExchangeNotification.LookupResult
				{
					Success = false,
					ErrorMessage = Strings.NoSmtpAddressForRecipientId(recipientIdParameter.ToString())
				};
			}
			IADOrgPerson iadorgPerson = adrecipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return new NewExchangeNotification.LookupResult
				{
					Success = true,
					SmtpAddress = adrecipient.PrimarySmtpAddress,
					Languages = new CultureInfo[0]
				};
			}
			return new NewExchangeNotification.LookupResult
			{
				Success = true,
				SmtpAddress = adrecipient.PrimarySmtpAddress,
				Languages = iadorgPerson.Languages
			};
		}

		private IEnumerable<NewExchangeNotification.NotificationEmailInfo> ForkRecipientsBasedOnLanguage(IEnumerable<NewExchangeNotification.LookupResult> recipients)
		{
			CultureInfo fallbackLanguage = this.GetNotificationEmailFallbackLanguage();
			string fqdn = this.GetLocalServerFqdn();
			IEnumerable<IGrouping<CultureInfo, SmtpAddress>> languageBuckets = from recipient in recipients
			group recipient.SmtpAddress by recipient.GetPreferredSupportedLanguage(fallbackLanguage);
			foreach (IGrouping<CultureInfo, SmtpAddress> languageBucket in languageBuckets)
			{
				string messageId = string.Format(CultureInfo.InvariantCulture, "<{0}@{1}>", new object[]
				{
					Guid.NewGuid(),
					fqdn
				});
				yield return new NewExchangeNotification.NotificationEmailInfo
				{
					Recipients = languageBucket,
					Language = languageBucket.Key,
					MessageId = messageId
				};
			}
			yield break;
		}

		private string GetLocalServerFqdn()
		{
			Server server = ((ITopologyConfigurationSession)this.ConfigurationSession).FindLocalServer();
			if (server == null)
			{
				return Environment.MachineName;
			}
			if (string.IsNullOrEmpty(server.Fqdn))
			{
				return Environment.MachineName;
			}
			return server.Fqdn;
		}

		private static readonly MailboxIdParameter FederatedMailboxId = new MailboxIdParameter("FederatedEmail.4c1f4d8b-8179-4148-93bf-00a95fa1e042");

		private static readonly HashSet<CultureInfo> SupportedClientLanguages = new HashSet<CultureInfo>(LanguagePackInfo.GetInstalledLanguagePackSpecificCultures(LanguagePackType.Client));

		private static readonly NewExchangeNotification.LookupResult[] EmptyLookupResultsArray = new NewExchangeNotification.LookupResult[0];

		private ADUser federatedUser;

		private IEnumerable<NewExchangeNotification.NotificationEmailInfo> notificationEmailInfos = new NewExchangeNotification.NotificationEmailInfo[0];

		private struct LookupResult
		{
			public CultureInfo GetPreferredSupportedLanguage(CultureInfo fallbackLanguage)
			{
				if (this.preferredSupportedLanguage == null)
				{
					this.preferredSupportedLanguage = fallbackLanguage;
					if (this.Languages != null)
					{
						foreach (CultureInfo item in this.Languages)
						{
							if (NewExchangeNotification.SupportedClientLanguages.Contains(item))
							{
								this.preferredSupportedLanguage = item;
								break;
							}
						}
					}
				}
				return this.preferredSupportedLanguage;
			}

			private CultureInfo preferredSupportedLanguage;

			public SmtpAddress SmtpAddress;

			public ICollection<CultureInfo> Languages;

			public bool Success;

			public LocalizedString ErrorMessage;
		}

		private sealed class NotificationEmailInfo
		{
			public IEnumerable<SmtpAddress> Recipients;

			public string MessageId;

			public CultureInfo Language;
		}
	}
}
