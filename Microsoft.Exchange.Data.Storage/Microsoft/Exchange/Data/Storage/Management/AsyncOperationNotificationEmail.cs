using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AsyncOperationNotificationEmail
	{
		static AsyncOperationNotificationEmail()
		{
			AsyncOperationNotificationEmail.bodyTable = new Dictionary<AsyncOperationType, Dictionary<AsyncOperationNotificationEmailType, string>>
			{
				{
					AsyncOperationType.CertExpiry,
					new Dictionary<AsyncOperationNotificationEmailType, string>
					{
						{
							AsyncOperationNotificationEmailType.CertExpiring,
							ServerStrings.NotificationEmailBodyCertExpiring
						},
						{
							AsyncOperationNotificationEmailType.CertExpired,
							ServerStrings.NotificationEmailBodyCertExpired
						}
					}
				},
				{
					AsyncOperationType.ExportPST,
					new Dictionary<AsyncOperationNotificationEmailType, string>
					{
						{
							AsyncOperationNotificationEmailType.Created,
							ServerStrings.NotificationEmailBodyExportPSTCreated
						},
						{
							AsyncOperationNotificationEmailType.Completed,
							ServerStrings.NotificationEmailBodyExportPSTCompleted
						},
						{
							AsyncOperationNotificationEmailType.Failed,
							ServerStrings.NotificationEmailBodyExportPSTFailed
						}
					}
				},
				{
					AsyncOperationType.ImportPST,
					new Dictionary<AsyncOperationNotificationEmailType, string>
					{
						{
							AsyncOperationNotificationEmailType.Created,
							ServerStrings.NotificationEmailBodyImportPSTCreated
						},
						{
							AsyncOperationNotificationEmailType.Completed,
							ServerStrings.NotificationEmailBodyImportPSTCompleted
						},
						{
							AsyncOperationNotificationEmailType.Failed,
							ServerStrings.NotificationEmailBodyImportPSTFailed
						}
					}
				}
			};
			Dictionary<string, Func<AsyncOperationNotificationEmail, string, string>> dictionary = new Dictionary<string, Func<AsyncOperationNotificationEmail, string, string>>();
			dictionary.Add("ExpireDate", (AsyncOperationNotificationEmail email, string key) => ExDateTime.FromFileTimeUtc(long.Parse(email.notification.GetExtendedAttributeValue(key))).ToShortDateString());
			dictionary.Add("StartedBy", delegate(AsyncOperationNotificationEmail email, string key)
			{
				if (email.notification.StartedBy != null)
				{
					return email.notification.StartedBy.ToString();
				}
				return string.Empty;
			});
			dictionary.Add("StartTime", (AsyncOperationNotificationEmail email, string key) => email.notification.StartTime.ToString());
			dictionary.Add("RunTime", delegate(AsyncOperationNotificationEmail email, string key)
			{
				if (email.notification.LastModified == null || email.notification.StartTime == null)
				{
					return string.Empty;
				}
				return email.notification.LastModified.Value.Subtract(email.notification.StartTime.Value).ToString();
			});
			dictionary.Add("DisplayName", (AsyncOperationNotificationEmail email, string key) => email.notification.DisplayName.ToString());
			dictionary.Add("EcpUrl", (AsyncOperationNotificationEmail email, string key) => email.GetEcpUrl());
			AsyncOperationNotificationEmail.bodyVariableGetters = dictionary;
			AsyncOperationNotificationEmail.bodyFormatRegex = new Regex("\\$_\\.(?<key>\\w+)", RegexOptions.Multiline | RegexOptions.Compiled);
		}

		public AsyncOperationNotificationEmail(AsyncOperationNotificationDataProvider provider, AsyncOperationNotification notification, bool forceSendCreatedMail)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (notification == null)
			{
				throw new ArgumentNullException("notification");
			}
			this.provider = provider;
			this.notification = notification;
			if (forceSendCreatedMail)
			{
				this.emailType = AsyncOperationNotificationEmailType.Created;
			}
			else
			{
				switch (notification.Status)
				{
				case AsyncOperationStatus.Completed:
					this.emailType = AsyncOperationNotificationEmailType.Completed;
					break;
				case AsyncOperationStatus.Failed:
					this.emailType = AsyncOperationNotificationEmailType.Failed;
					break;
				case AsyncOperationStatus.CertExpiring:
					this.emailType = AsyncOperationNotificationEmailType.CertExpiring;
					break;
				case AsyncOperationStatus.CertExpired:
					this.emailType = AsyncOperationNotificationEmailType.CertExpired;
					break;
				default:
					this.emailType = AsyncOperationNotificationEmailType.Created;
					break;
				}
			}
			this.emailMessage = new EmailMessage(this.provider.Service);
			this.emailMessage.Subject = this.GetSubject();
			this.emailMessage.Body = this.GetBody();
			this.AppendRecipients(notification.NotificationEmails);
		}

		public string Subject
		{
			get
			{
				return this.emailMessage.Subject;
			}
		}

		public MessageBody Body
		{
			get
			{
				return this.emailMessage.Body;
			}
		}

		public EmailAddressCollection ToRecipients
		{
			get
			{
				return this.emailMessage.ToRecipients;
			}
		}

		public AttachmentCollection Attachments
		{
			get
			{
				return this.emailMessage.Attachments;
			}
		}

		public void AppendRecipients(IEnumerable<ADRecipientOrAddress> recipients)
		{
			if (recipients != null)
			{
				foreach (ADRecipientOrAddress adrecipientOrAddress in recipients)
				{
					EmailAddress emailAddress = new EmailAddress(adrecipientOrAddress.DisplayName, adrecipientOrAddress.Address, adrecipientOrAddress.RoutingType);
					if (!this.emailMessage.ToRecipients.Contains(emailAddress))
					{
						this.emailMessage.ToRecipients.Add(emailAddress);
					}
				}
			}
		}

		public bool Send()
		{
			if (this.ToRecipients.Count == 0)
			{
				throw new InvalidOperationException("There is no Notification Email in the given notification object");
			}
			bool result = false;
			try
			{
				this.emailMessage.Send();
				result = true;
			}
			catch (ServiceRequestException)
			{
			}
			catch (ServiceResponseException)
			{
			}
			catch (WebException)
			{
			}
			return result;
		}

		private string GetSubject()
		{
			string text = string.Empty;
			switch (this.notification.Type)
			{
			case AsyncOperationType.ImportPST:
				text = ServerStrings.NotificationEmailSubjectImportPst;
				break;
			case AsyncOperationType.ExportPST:
				text = ServerStrings.NotificationEmailSubjectExportPst;
				break;
			case AsyncOperationType.Migration:
				text = ServerStrings.NotificationEmailSubjectMoveMailbox;
				break;
			}
			switch (this.emailType)
			{
			case AsyncOperationNotificationEmailType.Created:
				text = ServerStrings.NotificationEmailSubjectCreated(text);
				break;
			case AsyncOperationNotificationEmailType.Completed:
				text = ServerStrings.NotificationEmailSubjectCompleted(text);
				break;
			case AsyncOperationNotificationEmailType.Failed:
				text = ServerStrings.NotificationEmailSubjectFailed(text);
				break;
			case AsyncOperationNotificationEmailType.CertExpiring:
				text = ServerStrings.NotificationEmailSubjectCertExpiring;
				break;
			case AsyncOperationNotificationEmailType.CertExpired:
				text = ServerStrings.NotificationEmailSubjectCertExpired;
				break;
			}
			return text;
		}

		private MessageBody GetBody()
		{
			string text = string.Empty;
			Dictionary<AsyncOperationNotificationEmailType, string> dictionary;
			if (AsyncOperationNotificationEmail.bodyTable.TryGetValue(this.notification.Type, out dictionary) && dictionary.TryGetValue(this.emailType, out text))
			{
				text = AsyncOperationNotificationEmail.bodyFormatRegex.Replace(text, new MatchEvaluator(this.GetBodyVariable));
			}
			return "<html>\r\n            <head>\r\n            <style>\r\n            body\r\n            {\r\n                font-family: Tahoma;\r\n                background-color: #FFFFFF;\r\n                color: #000000; font-size:x-small;\r\n                width: 600px;\r\n            }\r\n            p\r\n            {\r\n                margin-left:0px ;\r\n                margin-bottom:8px\r\n            }\r\n            table\r\n            {\r\n                font-family: Tahoma;\r\n                background-color: #FFFFFF;\r\n                color: #000000;\r\n                font-size:x-small;\r\n                border:0px ;\r\n                text-align:left;\r\n                margin-left:20px\r\n            }\r\n            </style>\r\n            </head>\r\n            <body>\r\n            " + text + "\r\n            </body>\r\n            </html>";
		}

		private string GetBodyVariable(Match match)
		{
			string value = match.Groups["key"].Value;
			Func<AsyncOperationNotificationEmail, string, string> func;
			string result;
			LocalizedString value2;
			if (AsyncOperationNotificationEmail.bodyVariableGetters.TryGetValue(value, out func))
			{
				result = func(this, value);
			}
			else if (this.notification.TryGetExtendedAttributeValue(value, out value2))
			{
				result = value2;
			}
			else
			{
				result = match.Value;
			}
			return result;
		}

		private string GetEcpUrl()
		{
			if (AsyncOperationNotificationEmail.ecpUrl == null)
			{
				Uri uri = null;
				IExchangePrincipal mailbox = this.provider.Mailbox;
				try
				{
					if (AsyncOperationNotificationEmail.discoveryEcpExternalUrl == null)
					{
						AsyncOperationNotificationEmail.discoveryEcpExternalUrl = (Func<IExchangePrincipal, Uri>)Delegate.CreateDelegate(typeof(Func<IExchangePrincipal, Uri>), Type.GetType("Microsoft.Exchange.Data.ApplicationLogic.Cafe.FrontEndLocator, Microsoft.Exchange.Data.ApplicationLogic").GetMethod("GetFrontEndEcpUrl", BindingFlags.Static | BindingFlags.Public, null, new Type[]
						{
							typeof(IExchangePrincipal)
						}, null));
					}
					uri = AsyncOperationNotificationEmail.discoveryEcpExternalUrl(mailbox);
				}
				catch (Exception)
				{
				}
				if (uri != null && uri.IsAbsoluteUri)
				{
					AsyncOperationNotificationEmail.ecpUrl = uri.AbsoluteUri;
				}
			}
			return AsyncOperationNotificationEmail.ecpUrl ?? string.Empty;
		}

		public const string StartedByKey = "StartedBy";

		public const string StartTimeKey = "StartTime";

		public const string RunTimeKey = "RunTime";

		public const string DisplayNameKey = "DisplayName";

		public const string EcpUrlKey = "EcpUrl";

		public const string ExpireDateKey = "ExpireDate";

		private const string BodyBeginPart = "<html>\r\n            <head>\r\n            <style>\r\n            body\r\n            {\r\n                font-family: Tahoma;\r\n                background-color: #FFFFFF;\r\n                color: #000000; font-size:x-small;\r\n                width: 600px;\r\n            }\r\n            p\r\n            {\r\n                margin-left:0px ;\r\n                margin-bottom:8px\r\n            }\r\n            table\r\n            {\r\n                font-family: Tahoma;\r\n                background-color: #FFFFFF;\r\n                color: #000000;\r\n                font-size:x-small;\r\n                border:0px ;\r\n                text-align:left;\r\n                margin-left:20px\r\n            }\r\n            </style>\r\n            </head>\r\n            <body>\r\n            ";

		private const string BodyEndPart = "\r\n            </body>\r\n            </html>";

		private static Func<IExchangePrincipal, Uri> discoveryEcpExternalUrl;

		private static readonly Dictionary<AsyncOperationType, Dictionary<AsyncOperationNotificationEmailType, string>> bodyTable;

		private static readonly Dictionary<string, Func<AsyncOperationNotificationEmail, string, string>> bodyVariableGetters;

		private static readonly Regex bodyFormatRegex;

		private static string ecpUrl = null;

		private AsyncOperationNotificationDataProvider provider;

		private AsyncOperationNotification notification;

		private EmailMessage emailMessage;

		private AsyncOperationNotificationEmailType emailType;
	}
}
