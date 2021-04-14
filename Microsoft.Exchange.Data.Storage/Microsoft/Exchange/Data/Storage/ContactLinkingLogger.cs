using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ContactLinkingLogger : StorageLoggerBase
	{
		public ContactLinkingLogger(string activityName, MailboxInfoForLinking mailboxInfo) : this(ContactLinkingLogger.Logger.Member, activityName, mailboxInfo)
		{
		}

		internal ContactLinkingLogger(IExtensibleLogger logger, string activityName, MailboxInfoForLinking mailboxInfo) : base(logger)
		{
			ArgumentValidator.ThrowIfNull("activityName", activityName);
			ArgumentValidator.ThrowIfNull("mailboxInfo", mailboxInfo);
			this.mailboxInfo = mailboxInfo;
			this.activityName = activityName;
		}

		protected override string TenantName
		{
			get
			{
				return this.mailboxInfo.TenantName;
			}
		}

		protected override Guid MailboxGuid
		{
			get
			{
				return this.mailboxInfo.MailboxGuid;
			}
		}

		protected override void AppendEventData(ICollection<KeyValuePair<string, object>> eventData)
		{
			base.AppendEventData(eventData);
			eventData.Add(new KeyValuePair<string, object>(ContactLinkingLogger.ActivityNameFieldName, this.activityName));
		}

		internal static readonly string ActivityNameFieldName = "ActivityName";

		private static readonly LazyMember<ExtensibleLogger> Logger = new LazyMember<ExtensibleLogger>(() => new ExtensibleLogger(ContactLinkingLogConfiguration.Default));

		private readonly MailboxInfoForLinking mailboxInfo;

		private readonly string activityName;
	}
}
