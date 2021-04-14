using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Serializable]
	public sealed class MailboxDiagnosticLogs : ConfigurableObject
	{
		public MailboxDiagnosticLogs() : base(new SimplePropertyBag(SimpleProviderObjectSchema.Identity, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
			base.ResetChangeTracking();
		}

		public string MailboxLog
		{
			get
			{
				return (string)this[MailboxDiagnosticLogsSchema.MailboxLog];
			}
			set
			{
				this[MailboxDiagnosticLogsSchema.MailboxLog] = value;
			}
		}

		public string LogName
		{
			get
			{
				return (string)this[MailboxDiagnosticLogsSchema.LogName];
			}
			set
			{
				this[MailboxDiagnosticLogsSchema.LogName] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return (ObjectId)this[SimpleProviderObjectSchema.Identity];
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxDiagnosticLogs.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static MailboxDiagnosticLogsSchema schema = ObjectSchema.GetInstance<MailboxDiagnosticLogsSchema>();
	}
}
