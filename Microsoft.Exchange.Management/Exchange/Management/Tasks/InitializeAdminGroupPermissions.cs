using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("initialize", "AdminGroupPermissions", SupportsShouldProcess = true)]
	public sealed class InitializeAdminGroupPermissions : SetupTaskBase
	{
		[Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false)]
		public string AdministrativeGroup
		{
			get
			{
				return (string)base.Fields["AdministrativeGroup"];
			}
			set
			{
				base.Fields["AdministrativeGroup"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			QueryFilter filter = null;
			if (base.Fields.IsModified("AdministrativeGroup"))
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, this.AdministrativeGroup);
			}
			IEnumerable<AdministrativeGroup> enumerable = this.configurationSession.FindPaged<AdministrativeGroup>(null, QueryScope.SubTree, filter, null, 0);
			IEnumerator<AdministrativeGroup> enumerator = enumerable.GetEnumerator();
			if (enumerator == null || !enumerator.MoveNext())
			{
				if (base.Fields.IsModified("AdministrativeGroup"))
				{
					base.WriteError(new AdminGroupNotFoundException(this.AdministrativeGroup), ErrorCategory.ObjectNotFound, null);
				}
				else
				{
					base.WriteError(new AdminGroupsNotFoundException(), ErrorCategory.ObjectNotFound, null);
				}
			}
			SecurityIdentifier sid = this.exs.Sid;
			SecurityIdentifier securityIdentifier = new SecurityIdentifier("AU");
			SecurityIdentifier identity = new SecurityIdentifier("SY");
			Guid schemaClassGuid = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "msExchExchangeServer");
			List<ActiveDirectoryAccessRule> list = new List<ActiveDirectoryAccessRule>();
			list.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.RecipientUpdateExtendedRightGuid, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid));
			list.Add(new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.RecipientUpdateExtendedRightGuid, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid));
			GenericAce[] aces = new GenericAce[]
			{
				new ObjectAce(AceFlags.None, AceQualifier.AccessAllowed, 256, securityIdentifier, ObjectAceFlags.ObjectAceTypePresent, WellKnownGuid.CreatePublicFolderExtendedRightGuid, Guid.Empty, false, null)
			};
			do
			{
				AdministrativeGroup administrativeGroup = enumerator.Current;
				base.LogReadObject(administrativeGroup);
				if (base.ShouldProcess(administrativeGroup.DistinguishedName, Strings.InfoProcessAction(this.eoa.Sid.ToString()), null))
				{
					DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, administrativeGroup, list.ToArray());
				}
				DirectoryCommon.SetAclOnAlternateProperty(administrativeGroup, aces, AdministrativeGroupSchema.PublicFolderDefaultAdminAcl);
				if (base.ShouldProcess(administrativeGroup.DistinguishedName, Strings.InfoProcessAction(securityIdentifier.ToString()), null))
				{
					this.configurationSession.Save(administrativeGroup);
				}
				PublicFolderTree[] array = this.configurationSession.Find<PublicFolderTree>(administrativeGroup.Id, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, PublicFolderTreeSchema.PublicFolderTreeType, PublicFolderTreeType.Mapi), null, 0);
				if (array.Length == 0)
				{
					base.WriteVerbose(Strings.InfoCouldNotFindMAPITLHInAdminGroup(administrativeGroup.AdminDisplayName));
				}
				else
				{
					PublicFolderTree publicFolderTree = array[0];
					base.LogReadObject(publicFolderTree);
					DirectoryCommon.SetAclOnAlternateProperty(publicFolderTree, aces, AdministrativeGroupSchema.PublicFolderDefaultAdminAcl);
					if (base.ShouldProcess(publicFolderTree.DistinguishedName, Strings.InfoProcessAction(securityIdentifier.ToString()), null))
					{
						this.configurationSession.Save(publicFolderTree);
					}
				}
				ActiveDirectoryAccessRule ace = new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.ReceiveAsExtendedRightGuid, ActiveDirectorySecurityInheritance.All);
				this.SetAceByObjectClass<ServersContainer>(administrativeGroup.Id, ace);
			}
			while (enumerator.MoveNext());
			TaskLogger.LogExit();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.exs = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.ExSWkGuid);
			if (this.exs == null)
			{
				base.ThrowTerminatingError(new ExSGroupNotFoundException(WellKnownGuid.ExSWkGuid), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(this.exs);
			this.eoa = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.EoaWkGuid);
			if (this.eoa == null)
			{
				base.ThrowTerminatingError(new ExMailboxAdminSGroupNotFoundException(WellKnownGuid.EoaWkGuid), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(this.eoa);
			TaskLogger.LogExit();
		}

		private void SetAceByObjectClass<T>(ADObjectId root, ActiveDirectoryAccessRule ace) where T : ADConfigurationObject, new()
		{
			T[] array = this.configurationSession.Find<T>(root, QueryScope.SubTree, null, null, 0);
			foreach (T t in array)
			{
				if (base.ShouldProcess(t.DistinguishedName, Strings.InfoProcessAction(ace.IdentityReference.ToString()), null))
				{
					DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, t, new ActiveDirectoryAccessRule[]
					{
						ace
					});
				}
			}
		}

		private ADGroup exs;

		private ADGroup eoa;
	}
}
