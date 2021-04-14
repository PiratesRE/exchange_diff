using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UpdateGroupMailboxViaXSO : UpdateGroupMailboxBase
	{
		public UpdateGroupMailboxViaXSO(IRecipientSession adSession, ADUser group, ExchangePrincipal groupMailboxPrincipal, ADUser executingUser, GroupMailboxConfigurationActionType forceActionMask, UserMailboxLocator[] addedMembers, UserMailboxLocator[] removedMembers, int? permissionsVersion) : base(group, executingUser, forceActionMask, permissionsVersion)
		{
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			this.addedMembers = addedMembers;
			this.removedMembers = removedMembers;
			this.adSession = adSession;
			this.groupMailboxPrincipal = groupMailboxPrincipal;
		}

		public override void Execute()
		{
			using (MailboxSession mailboxSession = ConfigureGroupMailbox.CreateMailboxSessionForConfiguration(this.groupMailboxPrincipal, this.group.OriginatingServer))
			{
				this.ConfigureGroupMailboxIfRequired(mailboxSession);
				string arg = GroupMailboxContext.EnsureGroupIsInDirectoryCache("UpdateGroupMailboxViaXSO.Execute", this.adSession, this.group);
				UpdateGroupMailboxViaXSO.Tracer.TraceDebug<string>((long)this.GetHashCode(), "{0}", arg);
				this.WriteMembersToGroupIfRequired(mailboxSession);
				this.SetPermissionsVersionIfRequired(mailboxSession);
			}
		}

		private void ConfigureGroupMailboxIfRequired(MailboxSession mailboxSession)
		{
			if (this.group.IsGroupMailboxConfigured && this.forceActionMask == (GroupMailboxConfigurationActionType)0)
			{
				return;
			}
			ConfigureGroupMailbox configureGroupMailbox = new ConfigureGroupMailbox(this.adSession, this.group, this.executingUser, mailboxSession);
			try
			{
				GroupMailboxConfigurationReport groupMailboxConfigurationReport = configureGroupMailbox.Execute((GroupMailboxConfigurationAction)this.forceActionMask);
				foreach (KeyValuePair<GroupMailboxConfigurationAction, LatencyStatistics> keyValuePair in groupMailboxConfigurationReport.ConfigurationActionLatencyStatistics)
				{
					LatencyStatistics value = keyValuePair.Value;
					this.AppendAggregatedOperationStatisticsToCmdletLog(keyValuePair.Key, "AD", value.ADLatency);
					this.AppendAggregatedOperationStatisticsToCmdletLog(keyValuePair.Key, "Rpc", value.RpcLatency);
					this.AppendGenericLatencyToCmdletLog(keyValuePair.Key, (long)value.ElapsedTime.TotalMilliseconds);
				}
			}
			catch (LocalizedException ex)
			{
				UpdateGroupMailboxViaXSO.Tracer.TraceError<string>((long)this.GetHashCode(), "UpdateGroupMailboxViaXSO.ConfigureGroupMailboxIfRequired - Caught LocalizedException: {0}", ex.Message);
				base.Error = ex.LocalizedString;
			}
		}

		private void WriteMembersToGroupIfRequired(MailboxSession mailboxSession)
		{
			if (this.addedMembers == null && this.removedMembers == null)
			{
				return;
			}
			GroupMailboxAccessLayer.Execute("UpdateGroupMailboxViaXSO.WriteMembersToGroupIfRequired", this.adSession, mailboxSession, delegate(GroupMailboxAccessLayer accessLayer)
			{
				GroupMailboxLocator group = GroupMailboxLocator.Instantiate(this.adSession, this.group);
				accessLayer.SetMembershipState(this.executingUser, this.addedMembers, this.removedMembers, group);
			});
		}

		private void SetPermissionsVersionIfRequired(MailboxSession mailboxSession)
		{
			if (this.permissionsVersion != null)
			{
				mailboxSession.Mailbox[MailboxSchema.GroupMailboxPermissionsVersion] = this.permissionsVersion.Value;
				mailboxSession.Mailbox.Save();
			}
		}

		private void AppendAggregatedOperationStatisticsToCmdletLog(object key, string metric, AggregatedOperationStatistics? stats)
		{
			if (stats == null)
			{
				return;
			}
			this.AppendGenericLatencyToCmdletLog(string.Format("{0}.{1}C", key, metric), stats.Value.Count);
			this.AppendGenericLatencyToCmdletLog(string.Format("{0}.{1}", key, metric), (long)stats.Value.TotalMilliseconds);
		}

		private void AppendGenericLatencyToCmdletLog(object key, long value)
		{
			CmdletLogger.SafeAppendColumn(RpsCommonMetadata.GenericLatency, key.ToString(), value.ToString(NumberFormatInfo.InvariantInfo));
		}

		private static readonly Trace Tracer = ExTraceGlobals.CmdletsTracer;

		private readonly IRecipientSession adSession;

		private readonly ExchangePrincipal groupMailboxPrincipal;

		private readonly UserMailboxLocator[] addedMembers;

		private readonly UserMailboxLocator[] removedMembers;
	}
}
