using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MailboxMergeTargetResource : MailboxMergeResource
	{
		private MailboxMergeTargetResource(Guid mailboxGuid) : base(mailboxGuid)
		{
		}

		public override int StaticCapacity
		{
			get
			{
				int config;
				using (base.ConfigContext.Activate())
				{
					config = ConfigBase<MRSConfigSchema>.GetConfig<int>("MaxActiveJobsPerTargetMailbox");
				}
				return config;
			}
		}

		public override string ResourceType
		{
			get
			{
				return "MailboxMergeTarget";
			}
		}

		public static readonly ResourceCache<MailboxMergeTargetResource> Cache = new ResourceCache<MailboxMergeTargetResource>((Guid id) => new MailboxMergeTargetResource(id));
	}
}
