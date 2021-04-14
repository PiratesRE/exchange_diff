using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.RbacTasks;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "Mailbox", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMailbox : RemoveMailboxBase<MailboxIdParameter>
	{
		[Parameter(Mandatory = false)]
		public new SwitchParameter ForReconciliation
		{
			get
			{
				return base.ForReconciliation;
			}
			set
			{
				base.ForReconciliation = value;
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADRecipient adrecipient = (ADRecipient)base.ResolveDataObject();
			if (MailboxTaskHelper.ExcludeArbitrationMailbox(adrecipient, base.Arbitration) || MailboxTaskHelper.ExcludePublicFolderMailbox(adrecipient, base.PublicFolder) || MailboxTaskHelper.ExcludeGroupMailbox(adrecipient, false) || MailboxTaskHelper.ExcludeMailboxPlan(adrecipient, false) || MailboxTaskHelper.ExcludeAuditLogMailbox(adrecipient, base.AuditLog))
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ExchangeErrorCategory.Client, this.Identity);
			}
			if (adrecipient != null && adrecipient.RecipientTypeDetails == RecipientTypeDetails.TeamMailbox)
			{
				TeamMailbox teamMailbox = TeamMailbox.FromDataObject((ADUser)adrecipient);
				teamMailbox.ClosedTime = new DateTime?(DateTime.UtcNow);
				this.removeTeamMailboxFromResolverCache = true;
				if (teamMailbox.SharePointUrl != null && base.ExchangeRunspaceConfig != null)
				{
					TeamMailboxHelper teamMailboxHelper = new TeamMailboxHelper(teamMailbox, base.ExchangeRunspaceConfig.ExecutingUser, base.ExchangeRunspaceConfig.ExecutingUserOrganizationId, (IRecipientSession)base.DataSession, new TeamMailboxGetDataObject<ADUser>(base.GetDataObject<ADUser>));
					try
					{
						teamMailboxHelper.LinkSharePointSite(null, false, false);
					}
					catch (RecipientTaskException ex)
					{
						this.WriteWarning(Strings.ErrorTeamMailFailedUnlinkSharePointSite(this.Identity.ToString(), teamMailbox.SharePointUrl.ToString(), ex.Message));
					}
				}
			}
			return adrecipient;
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			this.orgAdminHelper = new RoleAssignmentsGlobalConstraints(this.ConfigurationSession, base.TenantGlobalCatalogSession, new Task.ErrorLoggerDelegate(base.WriteError));
		}

		protected override void InternalValidate()
		{
			this.latencyContext = ProvisioningPerformanceHelper.StartLatencyDetection(this);
			base.InternalValidate();
			if (base.DataObject != null)
			{
				RemoveMailbox.CheckManagedGroups(base.DataObject, base.TenantGlobalCatalogSession, new Task.TaskWarningLoggingDelegate(this.WriteWarning));
				if (this.orgAdminHelper.ShouldPreventLastAdminRemoval(this, base.DataObject.OrganizationId) && this.orgAdminHelper.IsLastAdmin(base.DataObject))
				{
					base.WriteError(new CannotRemoveLastOrgAdminException(Strings.ErrorCannotRemoveLastOrgAdmin(base.DataObject.Identity.ToString())), ExchangeErrorCategory.Client, base.DataObject.Identity);
				}
				RemoveMailbox.CheckModeratedMailboxes(base.DataObject, base.TenantGlobalCatalogSession, new Task.ErrorLoggerDelegate(base.WriteError));
				if (base.DataObject.CatchAllRecipientBL.Count > 0)
				{
					string domain = string.Join(", ", (from r in base.DataObject.CatchAllRecipientBL
					select r.Name).ToArray<string>());
					base.WriteError(new CannotRemoveMailboxCatchAllRecipientException(domain), ExchangeErrorCategory.Client, base.DataObject.Identity);
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				base.InternalProcessRecord();
				if (this.removeTeamMailboxFromResolverCache)
				{
					TeamMailboxADUserResolver.RemoveIdIfExists(base.DataObject.Id);
				}
			}
			finally
			{
				ProvisioningPerformanceHelper.StopLatencyDetection(this.latencyContext);
			}
		}

		protected override bool ShouldSoftDeleteObject()
		{
			ADRecipient dataObject = base.DataObject;
			return dataObject != null && !(dataObject.OrganizationId == null) && dataObject.OrganizationId.ConfigurationUnit != null && !base.Disconnect && !base.Permanent && Globals.IsMicrosoftHostedOnly && SoftDeletedTaskHelper.IsSoftDeleteSupportedRecipientTypeDetail(dataObject.RecipientTypeDetails);
		}

		internal static void CheckManagedGroups(ADUser user, IConfigDataProvider session, Task.TaskWarningLoggingDelegate writeWarning)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADGroupSchema.MemberJoinRestriction, MemberUpdateType.ApprovalRequired),
				new ComparisonFilter(ComparisonOperator.Equal, ADGroupSchema.ManagedBy, user.Id)
			});
			IEnumerable<ADGroup> enumerable = session.FindPaged<ADGroup>(filter, null, true, null, 1);
			using (IEnumerator<ADGroup> enumerator = enumerable.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					writeWarning(Strings.WarningRemoveApprovalRequiredGroupOwners(user.Id.ToString()));
				}
			}
		}

		internal static void CheckModeratedMailboxes(ADUser user, IConfigDataProvider session, Task.ErrorLoggerDelegate writeError)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.ModerationEnabled, true),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.ModeratedBy, user.Id)
			});
			IEnumerable<ADUser> enumerable = session.FindPaged<ADUser>(filter, null, true, null, 1);
			using (IEnumerator<ADUser> enumerator = enumerable.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					writeError(new RecipientTaskException(Strings.ErrorRemoveModeratorMailbox(user.Name)), ExchangeErrorCategory.Client, user);
				}
			}
		}

		private LatencyDetectionContext latencyContext;

		private RoleAssignmentsGlobalConstraints orgAdminHelper;

		private bool removeTeamMailboxFromResolverCache;
	}
}
