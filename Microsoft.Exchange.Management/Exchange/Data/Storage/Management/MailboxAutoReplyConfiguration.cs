using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MailboxAutoReplyConfiguration : XsoMailboxConfigurationObject
	{
		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return MailboxAutoReplyConfiguration.schema;
			}
		}

		[Parameter(Mandatory = false)]
		public OofState AutoReplyState
		{
			get
			{
				return (OofState)this[MailboxAutoReplyConfigurationSchema.AutoReplyState];
			}
			set
			{
				this[MailboxAutoReplyConfigurationSchema.AutoReplyState] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime EndTime
		{
			get
			{
				return (DateTime)this[MailboxAutoReplyConfigurationSchema.EndTime];
			}
			set
			{
				this[MailboxAutoReplyConfigurationSchema.EndTime] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExternalAudience ExternalAudience
		{
			get
			{
				return (ExternalAudience)this[MailboxAutoReplyConfigurationSchema.ExternalAudience];
			}
			set
			{
				this[MailboxAutoReplyConfigurationSchema.ExternalAudience] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ExternalMessage
		{
			get
			{
				return (string)this[MailboxAutoReplyConfigurationSchema.ExternalMessage];
			}
			set
			{
				this[MailboxAutoReplyConfigurationSchema.ExternalMessage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string InternalMessage
		{
			get
			{
				return (string)this[MailboxAutoReplyConfigurationSchema.InternalMessage];
			}
			set
			{
				this[MailboxAutoReplyConfigurationSchema.InternalMessage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime StartTime
		{
			get
			{
				return (DateTime)this[MailboxAutoReplyConfigurationSchema.StartTime];
			}
			set
			{
				this[MailboxAutoReplyConfigurationSchema.StartTime] = value;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.AutoReplyState == OofState.Scheduled)
			{
				if (this.StartTime.ToUniversalTime() >= this.EndTime.ToUniversalTime())
				{
					errors.Add(new PropertyValidationError(Strings.ErrorEndTimeSmallerThanStartTime, MailboxAutoReplyConfigurationSchema.EndTime, this.EndTime));
				}
				if (this.EndTime.ToUniversalTime() <= DateTime.UtcNow)
				{
					errors.Add(new PropertyValidationError(Strings.ErrorEndTimeSmallerThanNow, MailboxAutoReplyConfigurationSchema.EndTime, this.EndTime));
				}
			}
		}

		internal const int MaxAutoReplySize = 128000;

		private static MailboxAutoReplyConfigurationSchema schema = ObjectSchema.GetInstance<MailboxAutoReplyConfigurationSchema>();
	}
}
