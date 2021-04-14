using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Optics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class MailboxLoggerBase : StorageLoggerBase
	{
		protected MailboxLoggerBase(Guid mailboxGuid, OrganizationId organizationId, IExtensibleLogger logger) : base(logger)
		{
			if (organizationId != null && organizationId.OrganizationalUnit != null)
			{
				this.tenantName = organizationId.OrganizationalUnit.ToString();
			}
			this.mailboxGuid = mailboxGuid;
		}

		protected override string TenantName
		{
			get
			{
				return this.tenantName;
			}
		}

		protected override Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		protected override void AppendEventData(ICollection<KeyValuePair<string, object>> eventData)
		{
			base.AppendEventData(eventData);
			eventData.Add(MailboxLoggerBase.ApplicationIdKeyValuePair.Value);
			eventData.Add(MailboxLoggerBase.ApplicationVersionKeyValuePair.Value);
		}

		private const string ApplicationIdName = "ApplicationId";

		private const string ApplicationVersionName = "ApplicationVersion";

		private static readonly Lazy<KeyValuePair<string, object>> ApplicationIdKeyValuePair = new Lazy<KeyValuePair<string, object>>(() => new KeyValuePair<string, object>("ApplicationId", ApplicationName.Current.Name));

		private static readonly Lazy<KeyValuePair<string, object>> ApplicationVersionKeyValuePair = new Lazy<KeyValuePair<string, object>>(() => new KeyValuePair<string, object>("ApplicationVersion", "15.00.1497.012"));

		private readonly string tenantName;

		private readonly Guid mailboxGuid;
	}
}
