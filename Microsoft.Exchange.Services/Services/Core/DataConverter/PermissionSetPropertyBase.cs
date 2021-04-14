using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class PermissionSetPropertyBase<PermissionSerializationType> : ComplexPropertyBase, ISetCommand, ISetUpdateCommand, IDeleteUpdateCommand, IUpdateCommand, IPropertyCommand where PermissionSerializationType : BasePermissionType
	{
		public PermissionSetPropertyBase(CommandContext commandContext) : base(commandContext)
		{
		}

		void ISetCommand.Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			BaseFolderType serviceObject = commandSettings.ServiceObject as BaseFolderType;
			BasePermissionSetType permissionSet = this.GetPermissionSet(serviceObject);
			Folder folder = storeObject as Folder;
			this.SetPermissions(permissionSet, folder);
		}

		void ISetUpdateCommand.SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			this.SetUpdate(setPropertyUpdate, updateCommandSettings);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			Folder folder = updateCommandSettings.StoreObject as Folder;
			BaseFolderType serviceObject = setPropertyUpdate.ServiceObject as BaseFolderType;
			BasePermissionSetType permissionSet = this.GetPermissionSet(serviceObject);
			this.SetPermissions(permissionSet, folder);
		}

		public override void DeleteUpdate(DeletePropertyUpdate deletePropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			Folder folder = updateCommandSettings.StoreObject as Folder;
			PermissionRenderer.GetPermissionSet(folder).Clear();
		}

		protected static void CheckForDuplicatePermissions(PermissionInformationBase<PermissionSerializationType>[] permissionsInfo)
		{
			bool flag = false;
			List<List<BasePermissionType>> list = null;
			for (int i = 0; i < permissionsInfo.Length; i++)
			{
				List<BasePermissionType> list2 = null;
				bool flag2 = false;
				PermissionInformationBase<PermissionSerializationType> permissionInformationBase = permissionsInfo[i];
				if (permissionInformationBase.DistinguishedUserType != DistinguishedUserType.None)
				{
					for (int j = i + 1; j < permissionsInfo.Length; j++)
					{
						PermissionInformationBase<PermissionSerializationType> permissionInformationBase2 = permissionsInfo[j];
						if (permissionInformationBase2.DistinguishedUserType != DistinguishedUserType.None && permissionInformationBase2.DistinguishedUserType == permissionInformationBase.DistinguishedUserType)
						{
							flag = true;
							if (list == null)
							{
								list = new List<List<BasePermissionType>>();
							}
							if (list2 == null)
							{
								list2 = new List<BasePermissionType>();
								list.Add(list2);
							}
							if (!flag2)
							{
								list2.Add(permissionInformationBase.PermissionElement);
							}
							list2.Add(permissionInformationBase2.PermissionElement);
						}
					}
				}
				else
				{
					ADUser aduser = permissionInformationBase.Recipient as ADUser;
					ADGroup adgroup = permissionInformationBase.Recipient as ADGroup;
					ADContact adcontact = permissionInformationBase.Recipient as ADContact;
					SecurityIdentifier left = null;
					string externalUserIdentity = permissionInformationBase.ExternalUserIdentity;
					if (aduser != null)
					{
						left = aduser.Sid;
					}
					else if (adgroup != null)
					{
						left = adgroup.Sid;
					}
					else if (adcontact != null)
					{
						left = adcontact.MasterAccountSid;
					}
					for (int k = i + 1; k < permissionsInfo.Length; k++)
					{
						PermissionInformationBase<PermissionSerializationType> permissionInformationBase3 = permissionsInfo[k];
						if (permissionInformationBase3.DistinguishedUserType == DistinguishedUserType.None)
						{
							ADUser aduser2 = permissionInformationBase3.Recipient as ADUser;
							ADGroup adgroup2 = permissionInformationBase3.Recipient as ADGroup;
							ADContact adcontact2 = permissionInformationBase3.Recipient as ADContact;
							SecurityIdentifier securityIdentifier = null;
							string externalUserIdentity2 = permissionInformationBase3.ExternalUserIdentity;
							if (aduser2 != null)
							{
								securityIdentifier = aduser2.Sid;
							}
							else if (adgroup2 != null)
							{
								securityIdentifier = adgroup2.Sid;
							}
							else if (adcontact2 != null)
							{
								securityIdentifier = adcontact2.MasterAccountSid;
							}
							bool flag3 = false;
							if (externalUserIdentity != null && externalUserIdentity2 != null)
							{
								flag3 = StringComparer.OrdinalIgnoreCase.Equals(externalUserIdentity, externalUserIdentity2);
							}
							else if (left != null && securityIdentifier != null)
							{
								flag3 = (left == securityIdentifier);
							}
							if (flag3)
							{
								flag = true;
								if (list == null)
								{
									list = new List<List<BasePermissionType>>();
								}
								if (list2 == null)
								{
									list2 = new List<BasePermissionType>();
									list.Add(list2);
								}
								if (!flag2)
								{
									list2.Add(permissionInformationBase.PermissionElement);
								}
								list2.Add(permissionInformationBase3.PermissionElement);
							}
						}
					}
				}
			}
			if (flag)
			{
				throw new DuplicateUserIdsSpecifiedException(list);
			}
		}

		protected abstract void ConfirmFolderIsProperType(Folder folder);

		protected abstract PermissionInformationBase<PermissionSerializationType>[] ParsePermissions(BasePermissionSetType permissionSet, Folder folder);

		protected abstract BasePermissionSetType GetPermissionSet(BaseFolderType serviceObject);

		private void CleanFolderPermissions(Folder folder, PermissionInformationBase<PermissionSerializationType>[] newPermissions)
		{
			List<Permission> list = new List<Permission>();
			PermissionSet permissionSet = PermissionRenderer.GetPermissionSet(folder);
			if (permissionSet.DefaultPermission != null)
			{
				permissionSet.DefaultPermission.MemberRights = MemberRights.None;
			}
			if (permissionSet.AnonymousPermission != null)
			{
				permissionSet.AnonymousPermission.MemberRights = MemberRights.None;
			}
			foreach (Permission permission in permissionSet)
			{
				bool flag = false;
				if (permission.Principal.Type != PermissionSecurityPrincipal.SecurityPrincipalType.ADRecipientPrincipal)
				{
					list.Add(permission);
				}
				else
				{
					foreach (PermissionInformationBase<PermissionSerializationType> permissionInformationBase in newPermissions)
					{
						if (permissionInformationBase.Recipient != null && string.Equals(permission.Principal.ADRecipient.LegacyExchangeDN, permissionInformationBase.Recipient.LegacyExchangeDN, StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(permission);
					}
				}
			}
			foreach (Permission permission2 in list)
			{
				permissionSet.RemoveEntry(permission2.Principal);
			}
		}

		private void SetPermissions(BasePermissionSetType permissionSet, Folder folder)
		{
			this.ConfirmFolderIsProperType(folder);
			PermissionSet permissionSet2 = PermissionRenderer.GetPermissionSet(folder);
			ExternalUserCollection externalUserCollection = null;
			MailboxSession mailboxSession = folder.Session as MailboxSession;
			if (mailboxSession != null && mailboxSession.Capabilities.CanHaveExternalUsers)
			{
				externalUserCollection = mailboxSession.GetExternalUsers();
			}
			try
			{
				PermissionInformationBase<PermissionSerializationType>[] array = this.ParsePermissions(permissionSet, folder);
				PermissionSetPropertyBase<PermissionSerializationType>.CheckForDuplicatePermissions(array);
				this.CleanFolderPermissions(folder, array);
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetPermissionIntoPermissionSet(permissionSet2, externalUserCollection);
				}
				if (externalUserCollection != null)
				{
					externalUserCollection.Save();
				}
			}
			finally
			{
				if (externalUserCollection != null)
				{
					externalUserCollection.Dispose();
				}
			}
		}
	}
}
