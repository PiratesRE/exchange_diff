using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Permission;
using Microsoft.Exchange.Management.DDIService;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class DistributionGroupServiceBase : RecipientDataSourceService
	{
		protected PowerShellResults<T> GetDistributionGroup<T>(Identity identity) where T : DistributionGroup
		{
			PowerShellResults<T> @object = base.GetObject<T>("Get-DistributionGroup", identity);
			if (@object.SucceededWithValue)
			{
				PowerShellResults<WindowsGroup> powerShellResults = @object.MergeErrors<WindowsGroup>(base.GetObject<WindowsGroup>("Get-Group", identity));
				if (powerShellResults.SucceededWithValue)
				{
					T value = @object.Value;
					value.WindowsGroup = powerShellResults.Value;
				}
				if (RbacPrincipal.Current.IsInRole("Enterprise+Get-ADPermission?Identity"))
				{
					PowerShellResults<ADAcePresentationObject> powerShellResults2 = @object.MergeErrors<ADAcePresentationObject>(base.GetObject<ADAcePresentationObject>(new PSCommand().AddCommand("Get-ADPermission").AddParameter("Identity", identity)));
					if (powerShellResults2.Output.Length > 0)
					{
						T value2 = @object.Value;
						value2.SendAsPermissionsEnterprise = MailboxPropertiesHelper.FindRecipientsWithSendAsPermissionEnt(powerShellResults2.Output, null);
					}
				}
				else if (RbacPrincipal.Current.IsInRole("MultiTenant+Get-RecipientPermission?Identity"))
				{
					PowerShellResults<RecipientPermission> powerShellResults3 = @object.MergeErrors<RecipientPermission>(base.GetObject<RecipientPermission>(new PSCommand().AddCommand("Get-RecipientPermission").AddParameter("Identity", identity)));
					if (powerShellResults3.Output.Length > 0)
					{
						T value3 = @object.Value;
						value3.SendAsPermissionsCloud = MailboxPropertiesHelper.FindRecipientsWithSendAsPermissionCloud(powerShellResults3.Output);
					}
				}
			}
			return @object;
		}

		protected PowerShellResults<DistributionGroupRow> SetDistributionGroup<T, S, U>(Identity identity, T properties) where T : SetDistributionGroupBase<S, U> where S : SetGroupBase, new() where U : UpdateDistributionGroupMemberBase, new()
		{
			PowerShellResults<DistributionGroupRow> powerShellResults = new PowerShellResults<DistributionGroupRow>();
			identity.FaultIfNull();
			properties.FaultIfNull();
			powerShellResults.MergeErrors<WindowsGroup>(base.SetObject<WindowsGroup, SetGroupBase>("Set-Group", identity, properties.SetGroup));
			if (powerShellResults.Failed)
			{
				return powerShellResults;
			}
			U updateDistributionGroupMember = properties.UpdateDistributionGroupMember;
			if (updateDistributionGroupMember.Members != null)
			{
				U updateDistributionGroupMember2 = properties.UpdateDistributionGroupMember;
				updateDistributionGroupMember2.IgnoreNullOrEmpty = false;
				powerShellResults.MergeErrors(base.Invoke(new PSCommand().AddCommand("Update-DistributionGroupMember"), new Identity[]
				{
					identity
				}, properties.UpdateDistributionGroupMember));
				if (powerShellResults.Failed)
				{
					return powerShellResults;
				}
			}
			if (RbacPrincipal.Current.IsInRole("Enterprise"))
			{
				if (properties.SendAsPermissionsEnterprise != null && !properties.SendAsPermissionsEnterprise.Added.IsNullOrEmpty())
				{
					powerShellResults.MergeErrors(this.UpdateEnterpriseSendAsPermissions("Add-ADPermission", properties.SendAsPermissionsEnterprise.Added, new AddSendAsPermission
					{
						Identity = identity.RawIdentity
					}));
					if (powerShellResults.Failed)
					{
						return powerShellResults;
					}
				}
				if (properties.SendAsPermissionsEnterprise != null && !properties.SendAsPermissionsEnterprise.Removed.IsNullOrEmpty())
				{
					powerShellResults.MergeErrors(this.UpdateEnterpriseSendAsPermissions("Remove-ADPermission", properties.SendAsPermissionsEnterprise.Removed, new RemoveSendAsPermission
					{
						Identity = identity.RawIdentity
					}));
					if (powerShellResults.Failed)
					{
						return powerShellResults;
					}
				}
			}
			else if (RbacPrincipal.Current.IsInRole("MultiTenant"))
			{
				if (properties.SendAsPermissionsCloud != null && !properties.SendAsPermissionsCloud.Added.IsNullOrEmpty())
				{
					powerShellResults.MergeErrors(this.UpdateCloudSendAsPermissions("Add-RecipientPermission", properties.SendAsPermissionsCloud.Added, new AddCloudSendAsPermission
					{
						Identity = identity.RawIdentity
					}));
					if (powerShellResults.Failed)
					{
						return powerShellResults;
					}
				}
				if (properties.SendAsPermissionsCloud != null && !properties.SendAsPermissionsCloud.Removed.IsNullOrEmpty())
				{
					powerShellResults.MergeErrors(this.UpdateCloudSendAsPermissions("Remove-RecipientPermission", properties.SendAsPermissionsCloud.Removed, new AddCloudSendAsPermission
					{
						Identity = identity.RawIdentity
					}));
					if (powerShellResults.Failed)
					{
						return powerShellResults;
					}
				}
			}
			powerShellResults.MergeAll(base.SetObject<DistributionGroup, T, DistributionGroupRow>("Set-DistributionGroup", identity, properties));
			return powerShellResults;
		}

		private PowerShellResults UpdateEnterpriseSendAsPermissions(string cmdlet, Identity[] trustees, UpdateSendAsPermission param)
		{
			PowerShellResults powerShellResults = new PowerShellResults();
			foreach (Identity identity in trustees)
			{
				param.User = identity.RawIdentity;
				PSCommand psCommand = new PSCommand().AddCommand(cmdlet).AddParameters(param);
				powerShellResults.MergeErrors(base.Invoke(psCommand));
				if (powerShellResults.Failed)
				{
					break;
				}
			}
			return powerShellResults;
		}

		private PowerShellResults UpdateCloudSendAsPermissions(string cmdlet, Identity[] trustees, UpdateCloudSendAsPermission param)
		{
			PowerShellResults powerShellResults = new PowerShellResults();
			foreach (Identity identity in trustees)
			{
				param.Trustee = identity.RawIdentity;
				PSCommand psCommand = new PSCommand().AddCommand(cmdlet).AddParameters(param);
				powerShellResults.MergeErrors(base.Invoke(psCommand));
				if (powerShellResults.Failed)
				{
					break;
				}
			}
			return powerShellResults;
		}

		public static Identity GetGroupIdentityForTranslation(Identity[] identities)
		{
			if (identities.Length != 1)
			{
				return Identity.FromExecutingUserId();
			}
			return identities[0];
		}
	}
}
