using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class PermissionRendererBase<PermissionType, PermissionSetSerializationType, PermissionSerializationType> where PermissionType : Permission where PermissionSetSerializationType : BasePermissionSetType where PermissionSerializationType : BasePermissionType
	{
		protected abstract string GetPermissionsArrayElementName();

		protected abstract string GetPermissionElementName();

		protected abstract PermissionType GetDefaultPermission();

		protected abstract PermissionType GetAnonymousPermission();

		protected abstract IEnumerator<PermissionType> GetPermissionEnumerator();

		protected abstract void RenderByTypePermissionDetails(PermissionSerializationType permissionElement, PermissionType permission);

		protected PermissionRendererBase()
		{
			this.serviceProperty = this.CreatePermissionSetElement();
		}

		protected abstract PermissionSetSerializationType CreatePermissionSetElement();

		private void RenderDefaultPermissionIfSet(List<PermissionSerializationType> permissions)
		{
			PermissionType defaultPermission = this.GetDefaultPermission();
			if (defaultPermission == null)
			{
				return;
			}
			permissions.Add(this.RenderDistinguishedUserPermission(DistinguishedUserType.Default, defaultPermission));
		}

		private void RenderAnonymousPermissionIfSet(List<PermissionSerializationType> permissions)
		{
			PermissionType anonymousPermission = this.GetAnonymousPermission();
			if (anonymousPermission == null)
			{
				return;
			}
			permissions.Add(this.RenderDistinguishedUserPermission(DistinguishedUserType.Anonymous, anonymousPermission));
		}

		private void RenderIteratorPermissions(List<PermissionSerializationType> permissions)
		{
			IEnumerator<PermissionType> permissionEnumerator = this.GetPermissionEnumerator();
			while (permissionEnumerator.MoveNext())
			{
				PermissionType permissionType = permissionEnumerator.Current;
				if (permissionType.Principal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.ADRecipientPrincipal)
				{
					permissions.Add(this.RenderUserPermission(permissionEnumerator.Current));
				}
			}
		}

		private string[] GetUnknownEntries()
		{
			List<string> list = new List<string>(1);
			IEnumerator<PermissionType> permissionEnumerator = this.GetPermissionEnumerator();
			while (permissionEnumerator.MoveNext())
			{
				PermissionType permissionType = permissionEnumerator.Current;
				if (permissionType.Principal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.UnknownPrincipal)
				{
					list.Add(permissionType.Principal.UnknownPrincipalMemberName);
				}
				else if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010) && permissionType.Principal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal)
				{
					list.Add(permissionType.Principal.ExternalUser.Name);
				}
			}
			return list.ToArray();
		}

		private void RenderUnknownEntries()
		{
			string[] unknownEntries = this.GetUnknownEntries();
			if (unknownEntries != null && unknownEntries.Length != 0)
			{
				this.SetUnknownEntriesOnSerializationObject(this.serviceProperty, unknownEntries);
			}
		}

		internal PermissionSetSerializationType Render()
		{
			List<PermissionSerializationType> list = new List<PermissionSerializationType>();
			this.RenderDefaultPermissionIfSet(list);
			this.RenderAnonymousPermissionIfSet(list);
			this.RenderIteratorPermissions(list);
			this.RenderExternalPermissions(list);
			this.RenderUnknownEntries();
			this.SetPermissionsOnSerializationObject(this.serviceProperty, list);
			return this.serviceProperty;
		}

		private void RenderDistinguishedUserId(PermissionSerializationType permissionElement, DistinguishedUserType distinguishedUserType)
		{
			if (distinguishedUserType == DistinguishedUserType.Default)
			{
				permissionElement.UserId.DistinguishedUser = DistinguishedUserType.Default;
				return;
			}
			permissionElement.UserId.DistinguishedUser = DistinguishedUserType.Anonymous;
		}

		private void RenderUserId(PermissionSerializationType permissionElement, ADRecipient recipient)
		{
			ADUser aduser = recipient as ADUser;
			ADGroup adgroup = recipient as ADGroup;
			ADContact adcontact = recipient as ADContact;
			if (aduser == null && adgroup == null && adcontact == null)
			{
				ExTraceGlobals.FolderAlgorithmTracer.TraceDebug<SmtpAddress, string, string>(0L, "[PermissionSetProperty::RenderUserId] User with primary SMTP address {0}/display name {1} was not an ADUser, ADGroup, or ADContact - was of type {2}", recipient.PrimarySmtpAddress, recipient.DisplayName, recipient.GetType().FullName);
			}
			if (aduser != null)
			{
				permissionElement.UserId.Sid = aduser.Sid.ToString();
			}
			else if (adgroup != null)
			{
				permissionElement.UserId.Sid = adgroup.Sid.ToString();
			}
			permissionElement.UserId.PrimarySmtpAddress = recipient.PrimarySmtpAddress.ToString();
			permissionElement.UserId.DisplayName = recipient.DisplayName;
		}

		private PermissionSerializationType RenderDistinguishedUserPermission(DistinguishedUserType distinguishedUserType, PermissionType permission)
		{
			PermissionSerializationType permissionSerializationType = this.CreatePermissionElement();
			this.RenderDistinguishedUserId(permissionSerializationType, distinguishedUserType);
			this.RenderBasePermissionDetails(permissionSerializationType, permission);
			this.RenderByTypePermissionDetails(permissionSerializationType, permission);
			return permissionSerializationType;
		}

		private PermissionSerializationType RenderUserPermission(PermissionType permission)
		{
			PermissionSerializationType permissionSerializationType = this.CreatePermissionElement();
			this.RenderUserId(permissionSerializationType, permission.Principal.ADRecipient);
			this.RenderBasePermissionDetails(permissionSerializationType, permission);
			this.RenderByTypePermissionDetails(permissionSerializationType, permission);
			return permissionSerializationType;
		}

		private PermissionActionType CreatePermissionAction(ItemPermissionScope permissionScope)
		{
			if (ItemPermissionScope.AllItems == permissionScope)
			{
				return PermissionActionType.All;
			}
			if (ItemPermissionScope.OwnedItems == permissionScope)
			{
				return PermissionActionType.Owned;
			}
			return PermissionActionType.None;
		}

		private void RenderBasePermissionDetails(PermissionSerializationType permissionElement, PermissionType permission)
		{
			permissionElement.CanCreateItems = permission.CanCreateItems;
			permissionElement.CanCreateItemsSpecified = true;
			permissionElement.CanCreateSubFolders = permission.CanCreateSubfolders;
			permissionElement.CanCreateSubFoldersSpecified = true;
			permissionElement.IsFolderOwner = permission.IsFolderOwner;
			permissionElement.IsFolderOwnerSpecified = true;
			permissionElement.IsFolderVisible = permission.IsFolderVisible;
			permissionElement.IsFolderVisibleSpecified = true;
			permissionElement.IsFolderContact = permission.IsFolderContact;
			permissionElement.IsFolderContactSpecified = true;
			permissionElement.EditItems = this.CreatePermissionAction(permission.EditItems);
			permissionElement.EditItemsSpecified = true;
			permissionElement.DeleteItems = this.CreatePermissionAction(permission.DeleteItems);
			permissionElement.DeleteItemsSpecified = true;
		}

		private void RenderExternalPermissions(List<PermissionSerializationType> permissions)
		{
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
			{
				IEnumerator<PermissionType> permissionEnumerator = this.GetPermissionEnumerator();
				while (permissionEnumerator.MoveNext())
				{
					PermissionType permissionType = permissionEnumerator.Current;
					if (permissionType.Principal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal)
					{
						permissions.Add(this.RenderExternalUserPermission(permissionEnumerator.Current));
					}
				}
			}
		}

		protected PermissionSerializationType RenderExternalUserPermission(PermissionType permission)
		{
			PermissionSerializationType permissionSerializationType = this.CreatePermissionElement();
			this.RenderExternalUserId(permissionSerializationType, permission.Principal.ExternalUser.SmtpAddress.ToString());
			this.RenderBasePermissionDetails(permissionSerializationType, permission);
			this.RenderByTypePermissionDetails(permissionSerializationType, permission);
			return permissionSerializationType;
		}

		private void RenderExternalUserId(PermissionSerializationType permissionElement, string externalUserIdentity)
		{
			permissionElement.UserId.ExternalUserIdentity = externalUserIdentity;
		}

		protected abstract PermissionSerializationType CreatePermissionElement();

		protected abstract void SetPermissionsOnSerializationObject(PermissionSetSerializationType serviceProperty, List<PermissionSerializationType> renderedPermissions);

		protected abstract void SetUnknownEntriesOnSerializationObject(PermissionSetSerializationType serviceProperty, string[] entries);

		private PermissionSetSerializationType serviceProperty;
	}
}
