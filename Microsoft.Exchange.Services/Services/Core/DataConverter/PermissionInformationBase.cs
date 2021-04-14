using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class PermissionInformationBase<PermissionSerializationType> where PermissionSerializationType : BasePermissionType
	{
		internal PermissionInformationBase(PermissionSerializationType permissionSerializationObject)
		{
			this.PermissionElement = permissionSerializationObject;
		}

		internal PermissionInformationBase()
		{
			this.PermissionElement = this.CreateDefaultBasePermissionType();
		}

		protected abstract PermissionSerializationType CreateDefaultBasePermissionType();

		protected abstract PermissionLevel GetPermissionLevelToSet();

		internal abstract bool IsNonCustomPermissionLevelSet();

		protected abstract void SetByTypePermissionFieldsOntoPermission(Permission permission);

		internal virtual bool DoAnyNonPermissionLevelFieldsHaveValue()
		{
			return this.CanReadItems != null || this.CanCreateItems != null || this.CanCreateSubFolders != null || this.IsFolderOwner != null || this.IsFolderVisible != null || this.IsFolderContact != null || this.EditItems != null || this.DeleteItems != null;
		}

		internal PermissionSerializationType PermissionElement { get; private set; }

		internal Action StoredErrorTrace { get; set; }

		internal DistinguishedUserType DistinguishedUserType
		{
			get
			{
				PermissionSerializationType permissionElement = this.PermissionElement;
				return permissionElement.UserId.DistinguishedUser;
			}
			set
			{
				PermissionSerializationType permissionElement = this.PermissionElement;
				permissionElement.UserId.DistinguishedUser = value;
			}
		}

		internal ADRecipient Recipient
		{
			get
			{
				if (this.recipient == null)
				{
					this.recipient = this.CreateADRecipientFromSerializationClass();
				}
				return this.recipient;
			}
			set
			{
				this.recipient = value;
				this.UpdateSerializationClassFromADRecipient(this.recipient);
			}
		}

		internal string ExternalUserIdentity
		{
			get
			{
				PermissionSerializationType permissionElement = this.PermissionElement;
				return permissionElement.UserId.ExternalUserIdentity;
			}
			set
			{
				PermissionSerializationType permissionElement = this.PermissionElement;
				permissionElement.UserId.ExternalUserIdentity = value;
			}
		}

		internal abstract bool? CanReadItems { get; set; }

		internal bool? CanCreateItems
		{
			get
			{
				this.EnsurePermissionElementIsNotNull();
				PermissionSerializationType permissionElement = this.PermissionElement;
				if (permissionElement.CanCreateItemsSpecified)
				{
					PermissionSerializationType permissionElement2 = this.PermissionElement;
					return new bool?(permissionElement2.CanCreateItems);
				}
				return null;
			}
			set
			{
				this.EnsurePermissionElementIsNotNull();
				if (value != null)
				{
					PermissionSerializationType permissionElement = this.PermissionElement;
					permissionElement.CanCreateItems = value.Value;
					PermissionSerializationType permissionElement2 = this.PermissionElement;
					permissionElement2.CanCreateItemsSpecified = true;
					return;
				}
				PermissionSerializationType permissionElement3 = this.PermissionElement;
				permissionElement3.CanCreateItemsSpecified = false;
			}
		}

		internal bool? CanCreateSubFolders
		{
			get
			{
				this.EnsurePermissionElementIsNotNull();
				PermissionSerializationType permissionElement = this.PermissionElement;
				if (permissionElement.CanCreateSubFoldersSpecified)
				{
					PermissionSerializationType permissionElement2 = this.PermissionElement;
					return new bool?(permissionElement2.CanCreateSubFolders);
				}
				return null;
			}
			set
			{
				this.EnsurePermissionElementIsNotNull();
				if (value != null)
				{
					PermissionSerializationType permissionElement = this.PermissionElement;
					permissionElement.CanCreateSubFolders = value.Value;
					PermissionSerializationType permissionElement2 = this.PermissionElement;
					permissionElement2.CanCreateSubFoldersSpecified = true;
					return;
				}
				PermissionSerializationType permissionElement3 = this.PermissionElement;
				permissionElement3.CanCreateSubFoldersSpecified = false;
			}
		}

		internal bool? IsFolderOwner
		{
			get
			{
				this.EnsurePermissionElementIsNotNull();
				PermissionSerializationType permissionElement = this.PermissionElement;
				if (permissionElement.IsFolderOwnerSpecified)
				{
					PermissionSerializationType permissionElement2 = this.PermissionElement;
					return new bool?(permissionElement2.IsFolderOwner);
				}
				return null;
			}
			set
			{
				this.EnsurePermissionElementIsNotNull();
				if (value != null)
				{
					PermissionSerializationType permissionElement = this.PermissionElement;
					permissionElement.IsFolderOwner = value.Value;
					PermissionSerializationType permissionElement2 = this.PermissionElement;
					permissionElement2.IsFolderOwnerSpecified = true;
					return;
				}
				PermissionSerializationType permissionElement3 = this.PermissionElement;
				permissionElement3.IsFolderOwnerSpecified = false;
			}
		}

		internal bool? IsFolderVisible
		{
			get
			{
				this.EnsurePermissionElementIsNotNull();
				PermissionSerializationType permissionElement = this.PermissionElement;
				if (permissionElement.IsFolderVisibleSpecified)
				{
					PermissionSerializationType permissionElement2 = this.PermissionElement;
					return new bool?(permissionElement2.IsFolderVisible);
				}
				return null;
			}
			set
			{
				this.EnsurePermissionElementIsNotNull();
				if (value != null)
				{
					PermissionSerializationType permissionElement = this.PermissionElement;
					permissionElement.IsFolderVisible = value.Value;
					PermissionSerializationType permissionElement2 = this.PermissionElement;
					permissionElement2.IsFolderVisibleSpecified = true;
					return;
				}
				PermissionSerializationType permissionElement3 = this.PermissionElement;
				permissionElement3.IsFolderVisibleSpecified = false;
			}
		}

		internal bool? IsFolderContact
		{
			get
			{
				this.EnsurePermissionElementIsNotNull();
				PermissionSerializationType permissionElement = this.PermissionElement;
				if (permissionElement.IsFolderContactSpecified)
				{
					PermissionSerializationType permissionElement2 = this.PermissionElement;
					return new bool?(permissionElement2.IsFolderContact);
				}
				return null;
			}
			set
			{
				this.EnsurePermissionElementIsNotNull();
				if (value != null)
				{
					PermissionSerializationType permissionElement = this.PermissionElement;
					permissionElement.IsFolderContact = value.Value;
					PermissionSerializationType permissionElement2 = this.PermissionElement;
					permissionElement2.IsFolderContactSpecified = true;
					return;
				}
				PermissionSerializationType permissionElement3 = this.PermissionElement;
				permissionElement3.IsFolderContactSpecified = false;
			}
		}

		internal ItemPermissionScope? EditItems
		{
			get
			{
				this.EnsurePermissionElementIsNotNull();
				PermissionSerializationType permissionElement = this.PermissionElement;
				if (permissionElement.EditItemsSpecified)
				{
					PermissionSerializationType permissionElement2 = this.PermissionElement;
					return this.ConvertToItemPermissionScope(permissionElement2.EditItems);
				}
				return null;
			}
			set
			{
				this.EnsurePermissionElementIsNotNull();
				if (value != null)
				{
					PermissionSerializationType permissionElement = this.PermissionElement;
					permissionElement.EditItems = this.ConvertToItemPermissionActionType(value.Value).Value;
					PermissionSerializationType permissionElement2 = this.PermissionElement;
					permissionElement2.EditItemsSpecified = true;
					return;
				}
				PermissionSerializationType permissionElement3 = this.PermissionElement;
				permissionElement3.EditItemsSpecified = false;
			}
		}

		internal ItemPermissionScope? DeleteItems
		{
			get
			{
				this.EnsurePermissionElementIsNotNull();
				PermissionSerializationType permissionElement = this.PermissionElement;
				if (permissionElement.DeleteItemsSpecified)
				{
					PermissionSerializationType permissionElement2 = this.PermissionElement;
					return this.ConvertToItemPermissionScope(permissionElement2.DeleteItems);
				}
				return null;
			}
			set
			{
				this.EnsurePermissionElementIsNotNull();
				if (value != null)
				{
					PermissionSerializationType permissionElement = this.PermissionElement;
					permissionElement.DeleteItems = this.ConvertToItemPermissionActionType(value.Value).Value;
					PermissionSerializationType permissionElement2 = this.PermissionElement;
					permissionElement2.DeleteItemsSpecified = true;
					return;
				}
				PermissionSerializationType permissionElement3 = this.PermissionElement;
				permissionElement3.DeleteItemsSpecified = false;
			}
		}

		private void SetCommonPermissionFieldsOntoPermission(Permission permission)
		{
			if (this.CanReadItems != null)
			{
				permission.CanReadItems = this.CanReadItems.Value;
			}
			if (this.CanCreateItems != null)
			{
				permission.CanCreateItems = this.CanCreateItems.Value;
			}
			if (this.CanCreateSubFolders != null)
			{
				permission.CanCreateSubfolders = this.CanCreateSubFolders.Value;
			}
			if (this.IsFolderOwner != null)
			{
				permission.IsFolderOwner = this.IsFolderOwner.Value;
			}
			if (this.IsFolderVisible != null)
			{
				permission.IsFolderVisible = this.IsFolderVisible.Value;
			}
			if (this.IsFolderContact != null)
			{
				permission.IsFolderContact = this.IsFolderContact.Value;
			}
			if (this.EditItems != null)
			{
				permission.EditItems = this.EditItems.Value;
			}
			if (this.DeleteItems != null)
			{
				permission.DeleteItems = this.DeleteItems.Value;
			}
		}

		internal void SetPermissionIntoPermissionSet(PermissionSet permissionSet, ExternalUserCollection externalUserCollection)
		{
			Permission permission = null;
			PermissionLevel permissionLevel = this.GetPermissionLevelToSet();
			MemberRights? memberRights = null;
			if (permissionLevel == PermissionLevel.Custom)
			{
				permissionLevel = PermissionLevel.None;
			}
			if (this.DistinguishedUserType != DistinguishedUserType.None)
			{
				if (this.DistinguishedUserType == DistinguishedUserType.Anonymous)
				{
					permission = permissionSet.AnonymousPermission;
				}
				else
				{
					permission = permissionSet.DefaultPermission;
				}
				if (permission == null)
				{
					string arg;
					if (this.DistinguishedUserType == DistinguishedUserType.Anonymous)
					{
						arg = "Anonymous";
					}
					else
					{
						arg = "Default";
					}
					ExTraceGlobals.FolderAlgorithmTracer.TraceDebug<string>(0L, "[PermissionInformationBase::SetPermissionIntoPermissionSet] Distinguished user permission {0} is null, throwing InvalidUserInfoException", arg);
					throw new InvalidUserInfoException(this.PermissionElement);
				}
				permission.PermissionLevel = permissionLevel;
			}
			else
			{
				if (this.Recipient != null)
				{
					try
					{
						PermissionSecurityPrincipal securityPrincipal = new PermissionSecurityPrincipal(this.Recipient);
						permission = permissionSet.GetEntry(securityPrincipal);
						if (permission != null)
						{
							memberRights = new MemberRights?(permission.MemberRights);
							permission.MemberRights = MemberRights.None;
							permission.PermissionLevel = permissionLevel;
						}
						else
						{
							if (this.StoredErrorTrace != null)
							{
								this.StoredErrorTrace();
								throw new InvalidUserInfoException(this.PermissionElement);
							}
							permission = permissionSet.AddEntry(new PermissionSecurityPrincipal(this.Recipient), permissionLevel);
						}
						goto IL_1B0;
					}
					catch (ArgumentException arg2)
					{
						ExTraceGlobals.FolderAlgorithmTracer.TraceDebug<ArgumentException>(0L, "[PermissionInformationBase::SetPermissionIntoPermissionSet] Our permission is not valid per XSO - exception {0}", arg2);
						throw new InvalidUserInfoException(this.PermissionElement);
					}
				}
				if (this.ExternalUserIdentity != null)
				{
					if (externalUserCollection == null)
					{
						ExTraceGlobals.FolderAlgorithmTracer.TraceDebug(0L, "[PermissionInformationBase::SetPermissionIntoPermissionSet] ExternalUserCollection is null, throwing InvalidUserInfoException");
						throw new InvalidUserInfoException(this.PermissionElement);
					}
					SmtpAddress smtpAddress = new SmtpAddress(this.ExternalUserIdentity);
					ExternalUser externalUser = externalUserCollection.FindExternalUser(smtpAddress);
					if (externalUser == null)
					{
						externalUser = externalUserCollection.AddFederatedUser(smtpAddress);
					}
					try
					{
						permission = permissionSet.AddEntry(new PermissionSecurityPrincipal(externalUser), permissionLevel);
					}
					catch (ArgumentException arg3)
					{
						ExTraceGlobals.FolderAlgorithmTracer.TraceDebug<ArgumentException>(0L, "[PermissionInformationBase::SetPermissionIntoPermissionSet] Our permission is not valid per XSO - exception {0}", arg3);
						throw new InvalidUserInfoException(this.PermissionElement);
					}
				}
			}
			IL_1B0:
			this.SetCommonPermissionFieldsOntoPermission(permission);
			this.SetByTypePermissionFieldsOntoPermission(permission);
			if (memberRights != null && permission.MemberRights != memberRights.Value && this.StoredErrorTrace != null)
			{
				this.StoredErrorTrace();
				throw new InvalidUserInfoException(this.PermissionElement);
			}
		}

		private ADRecipient CreateADRecipientFromSerializationClass()
		{
			ADRecipient recipient = null;
			IRecipientSession adRecipientSession = CallContext.Current.ADRecipientSessionContext.GetADRecipientSession();
			PermissionSerializationType permissionElement = this.PermissionElement;
			if (!string.IsNullOrEmpty(permissionElement.UserId.Sid))
			{
				try
				{
					RequestDetailsLogger.Current.TrackLatency(ServiceLatencyMetadata.RecipientLookupLatency, delegate()
					{
						PermissionSerializationType permissionElement7 = this.PermissionElement;
						if (!Directory.TryFindRecipient(new SecurityIdentifier(permissionElement7.UserId.Sid), adRecipientSession, out recipient))
						{
							throw new InvalidUserInfoException(this.PermissionElement);
						}
					});
				}
				catch (ArgumentException innerException)
				{
					throw new InvalidUserInfoException(this.PermissionElement, innerException);
				}
				PermissionSerializationType permissionElement2 = this.PermissionElement;
				if (!string.IsNullOrEmpty(permissionElement2.UserId.PrimarySmtpAddress))
				{
					string a = recipient.PrimarySmtpAddress.ToString();
					PermissionSerializationType permissionElement3 = this.PermissionElement;
					if (!string.Equals(a, permissionElement3.UserId.PrimarySmtpAddress, StringComparison.OrdinalIgnoreCase))
					{
						throw new InvalidUserInfoException(this.PermissionElement);
					}
				}
			}
			else
			{
				PermissionSerializationType permissionElement4 = this.PermissionElement;
				if (!string.IsNullOrEmpty(permissionElement4.UserId.PrimarySmtpAddress))
				{
					try
					{
						RequestDetailsLogger.Current.TrackLatency(ServiceLatencyMetadata.RecipientLookupLatency, delegate()
						{
							PermissionSerializationType permissionElement7 = this.PermissionElement;
							if (!Directory.TryFindRecipient(permissionElement7.UserId.PrimarySmtpAddress, adRecipientSession, out recipient))
							{
								throw new InvalidUserInfoException(this.PermissionElement);
							}
						});
					}
					catch (ArgumentException innerException2)
					{
						throw new InvalidUserInfoException(this.PermissionElement, innerException2);
					}
				}
			}
			PermissionSerializationType permissionElement5 = this.PermissionElement;
			if (!string.IsNullOrEmpty(permissionElement5.UserId.DisplayName))
			{
				string displayName = recipient.DisplayName;
				PermissionSerializationType permissionElement6 = this.PermissionElement;
				if (!string.Equals(displayName, permissionElement6.UserId.DisplayName, StringComparison.OrdinalIgnoreCase))
				{
					throw new InvalidUserInfoException(this.PermissionElement);
				}
			}
			return recipient;
		}

		private void UpdateSerializationClassFromADRecipient(ADRecipient recipient)
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
				PermissionSerializationType permissionElement = this.PermissionElement;
				permissionElement.UserId.Sid = aduser.Sid.ToString();
			}
			else if (adgroup != null)
			{
				PermissionSerializationType permissionElement2 = this.PermissionElement;
				permissionElement2.UserId.Sid = adgroup.Sid.ToString();
			}
			PermissionSerializationType permissionElement3 = this.PermissionElement;
			permissionElement3.UserId.PrimarySmtpAddress = recipient.PrimarySmtpAddress.ToString();
			PermissionSerializationType permissionElement4 = this.PermissionElement;
			permissionElement4.UserId.DisplayName = recipient.DisplayName;
		}

		protected void EnsurePermissionElementIsNotNull()
		{
			if (this.PermissionElement == null)
			{
				this.PermissionElement = this.CreateDefaultBasePermissionType();
			}
		}

		private PermissionActionType? ConvertToItemPermissionActionType(ItemPermissionScope itemPermissionScope)
		{
			return new PermissionActionType?(PermissionInformationBase<PermissionSerializationType>.permissionTypeDictionary[itemPermissionScope]);
		}

		private ItemPermissionScope? ConvertToItemPermissionScope(PermissionActionType permissionActionType)
		{
			foreach (KeyValuePair<ItemPermissionScope, PermissionActionType> keyValuePair in PermissionInformationBase<PermissionSerializationType>.permissionTypeDictionary)
			{
				if (keyValuePair.Value == permissionActionType)
				{
					return new ItemPermissionScope?(keyValuePair.Key);
				}
			}
			return null;
		}

		private static Dictionary<ItemPermissionScope, PermissionActionType> permissionTypeDictionary = new Dictionary<ItemPermissionScope, PermissionActionType>
		{
			{
				ItemPermissionScope.AllItems,
				PermissionActionType.All
			},
			{
				ItemPermissionScope.None,
				PermissionActionType.None
			},
			{
				ItemPermissionScope.OwnedItems,
				PermissionActionType.Owned
			}
		};

		private ADRecipient recipient;
	}
}
