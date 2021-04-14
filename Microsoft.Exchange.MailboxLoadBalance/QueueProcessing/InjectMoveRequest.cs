using System;
using System.Collections.Generic;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	internal class InjectMoveRequest : CmdletExecutionRequest<MoveRequestStatistics>
	{
		public InjectMoveRequest(string batchName, DirectoryMailbox mailbox, ILogger logger, bool protect, RequestPriority requestPriority, ADObjectId mailboxObjectId, DirectoryIdentity targetDatabaseIdentity, ILoadBalanceSettings settings, CmdletExecutionPool cmdletPool) : base("New-MoveRequest", cmdletPool, logger)
		{
			this.settings = settings;
			this.TargetDatabase = targetDatabaseIdentity;
			this.BatchName = batchName;
			this.Mailbox = mailbox;
			this.Protect = protect;
			AnchorUtil.ThrowOnNullArgument(mailbox, "mailbox");
			base.Command.AddParameter("Identity", mailboxObjectId);
			base.Command.AddParameter("WorkloadType", RequestWorkloadType.LoadBalancing);
			if (!string.IsNullOrWhiteSpace(batchName))
			{
				base.Command.AddParameter("BatchName", batchName);
			}
			if (targetDatabaseIdentity != null)
			{
				DatabaseIdParameter value = new DatabaseIdParameter(targetDatabaseIdentity.ADObjectId);
				if (mailbox.IsArchiveOnly)
				{
					base.Command.AddParameter("ArchiveTargetDatabase", value);
					base.Command.AddParameter("ArchiveOnly");
				}
				else
				{
					base.Command.AddParameter("TargetDatabase", value);
					base.Command.AddParameter("PrimaryOnly");
				}
			}
			base.Command.AddParameter("CompletedRequestAgeLimit", 0);
			base.Command.AddParameter("Priority", requestPriority);
			if (protect)
			{
				base.Command.AddParameter("Protect");
			}
		}

		public override IEnumerable<ResourceKey> Resources
		{
			get
			{
				return new ADResourceKey[]
				{
					ADResourceKey.Key
				};
			}
		}

		private string BatchName { get; set; }

		private DirectoryMailbox Mailbox { get; set; }

		private bool Protect { get; set; }

		private DirectoryIdentity TargetDatabase { get; set; }

		public override RequestDiagnosticData GetDiagnosticData(bool verbose)
		{
			InjectMoveRequestDiagnosticData injectMoveRequestDiagnosticData = (InjectMoveRequestDiagnosticData)base.GetDiagnosticData(verbose);
			injectMoveRequestDiagnosticData.ArchiveOnly = this.Mailbox.IsArchiveOnly;
			injectMoveRequestDiagnosticData.BatchName = this.BatchName;
			injectMoveRequestDiagnosticData.Mailbox = this.Mailbox;
			injectMoveRequestDiagnosticData.Protect = this.Protect;
			injectMoveRequestDiagnosticData.TargetDatabase = this.TargetDatabase;
			injectMoveRequestDiagnosticData.TargetDatabaseName = this.TargetDatabase.Name;
			injectMoveRequestDiagnosticData.MovedMailboxGuid = this.Mailbox.Guid;
			injectMoveRequestDiagnosticData.SourceDatabaseName = this.Mailbox.GetDatabaseForMailbox().Name;
			return injectMoveRequestDiagnosticData;
		}

		protected override RequestDiagnosticData CreateDiagnosticData()
		{
			return new InjectMoveRequestDiagnosticData();
		}

		protected override void ProcessRequest()
		{
			if (this.settings.DontCreateMoveRequests)
			{
				base.Command.AddParameter("WhatIf");
			}
			base.ProcessRequest();
		}

		private readonly ILoadBalanceSettings settings;
	}
}
