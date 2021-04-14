using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MailboxMergeSourceResource : MailboxMergeResource
	{
		private MailboxMergeSourceResource(Guid mailboxGuid) : base(mailboxGuid)
		{
		}

		public override int StaticCapacity
		{
			get
			{
				int config;
				using (base.ConfigContext.Activate())
				{
					config = ConfigBase<MRSConfigSchema>.GetConfig<int>("MaxActiveJobsPerSourceMailbox");
				}
				return config;
			}
		}

		public override string ResourceType
		{
			get
			{
				return "MailboxMergeSource";
			}
		}

		public static readonly ResourceCache<MailboxMergeSourceResource> Cache = new ResourceCache<MailboxMergeSourceResource>((Guid id) => new MailboxMergeSourceResource(id));
	}
}
