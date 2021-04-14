using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Microsoft.Exchange.Security.Authorization
{
	internal class PrincipalPermissionList : List<PrincipalPermissionPair>
	{
		public PrincipalPermissionList()
		{
		}

		public PrincipalPermissionList(int capacity) : base(capacity)
		{
		}

		public void Add(SecurityIdentifier sid, Permission permission)
		{
			base.Add(new PrincipalPermissionPair(sid, permission, AccessControlType.Allow));
		}

		public void AddDeny(SecurityIdentifier sid, Permission permission)
		{
			base.Add(new PrincipalPermissionPair(sid, permission, AccessControlType.Deny));
		}

		public RawSecurityDescriptor CreateExtendedRightsSecurityDescriptor(SecurityIdentifier owner, SecurityIdentifier group)
		{
			ActiveDirectorySecurity ads = new ActiveDirectorySecurity();
			this.ForEach(delegate(ActiveDirectoryAccessRule ace)
			{
				ads.AddAccessRule(ace);
			});
			ads.SetOwner(owner);
			ads.SetGroup(group);
			return new RawSecurityDescriptor(ads.GetSecurityDescriptorSddlForm(AccessControlSections.All));
		}

		public RawSecurityDescriptor AddExtendedRightsToSecurityDescriptor(RawSecurityDescriptor rsd)
		{
			ActiveDirectorySecurity ads = new ActiveDirectorySecurity();
			ads.SetSecurityDescriptorSddlForm(rsd.GetSddlForm(AccessControlSections.All));
			this.ForEach(delegate(ActiveDirectoryAccessRule ace)
			{
				ads.AddAccessRule(ace);
			});
			return new RawSecurityDescriptor(ads.GetSecurityDescriptorSddlForm(AccessControlSections.All));
		}

		public RawSecurityDescriptor RemoveExtendedRightsFromSecurityDescriptor(RawSecurityDescriptor rsd)
		{
			ActiveDirectorySecurity ads = new ActiveDirectorySecurity();
			ads.SetSecurityDescriptorSddlForm(rsd.GetSddlForm(AccessControlSections.All));
			this.ForEach(delegate(ActiveDirectoryAccessRule ace)
			{
				ads.RemoveAccessRule(ace);
			});
			return new RawSecurityDescriptor(ads.GetSecurityDescriptorSddlForm(AccessControlSections.All));
		}

		private void ForEach(Action<ActiveDirectoryAccessRule> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			foreach (PrincipalPermissionPair principalPermissionPair in this)
			{
				foreach (Guid objectType in WellKnownPermission.ToGuids(principalPermissionPair.Permission))
				{
					ActiveDirectoryAccessRule obj = new ActiveDirectoryAccessRule(principalPermissionPair.Principal, ActiveDirectoryRights.ExtendedRight, principalPermissionPair.AccessControlType, objectType, ActiveDirectorySecurityInheritance.All);
					action(obj);
				}
			}
		}
	}
}
