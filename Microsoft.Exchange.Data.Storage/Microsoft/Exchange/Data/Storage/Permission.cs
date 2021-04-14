using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Permission
	{
		internal Permission(PermissionSecurityPrincipal securityPrincipal, MemberRights memberRights, long memberId) : this(securityPrincipal, memberRights, PermissionOrigin.Read)
		{
			this.memberId = new long?(memberId);
		}

		public Permission(PermissionSecurityPrincipal securityPrincipal, MemberRights memberRights) : this(securityPrincipal, memberRights, PermissionOrigin.New)
		{
			this.ValidateMemberRights(memberRights);
		}

		private Permission(PermissionSecurityPrincipal securityPrincipal, MemberRights memberRights, PermissionOrigin origin)
		{
			this.securityPrincipal = securityPrincipal;
			this.initialMemberRights = memberRights;
			this.memberRights = memberRights;
			this.origin = origin;
		}

		public PermissionSecurityPrincipal Principal
		{
			get
			{
				return this.securityPrincipal;
			}
		}

		public void CopyFrom(Permission source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			this.memberRights = source.memberRights;
		}

		public PermissionLevel PermissionLevel
		{
			get
			{
				return Permission.GetPermissionLevel(this.memberRights);
			}
			set
			{
				this.memberRights = (Permission.GetMemberRights(value) | (this.memberRights & (MemberRights.FreeBusySimple | MemberRights.FreeBusyDetailed)));
			}
		}

		public bool CanCreateItems
		{
			get
			{
				return this.GetMemberRight(MemberRights.Create);
			}
			set
			{
				this.SetMemberRight(MemberRights.Create, value);
			}
		}

		public bool CanReadItems
		{
			get
			{
				return this.GetMemberRight(MemberRights.ReadAny);
			}
			set
			{
				this.SetMemberRight(MemberRights.ReadAny, value);
			}
		}

		public bool CanCreateSubfolders
		{
			get
			{
				return this.GetMemberRight(MemberRights.CreateSubfolder);
			}
			set
			{
				this.SetMemberRight(MemberRights.CreateSubfolder, value);
			}
		}

		public bool IsFolderOwner
		{
			get
			{
				return this.GetMemberRight(MemberRights.Owner);
			}
			set
			{
				this.SetMemberRight(MemberRights.Owner, value);
			}
		}

		public bool IsFolderContact
		{
			get
			{
				return this.GetMemberRight(MemberRights.Contact);
			}
			set
			{
				this.SetMemberRight(MemberRights.Contact, value);
			}
		}

		public bool IsFolderVisible
		{
			get
			{
				return this.GetMemberRight(MemberRights.Visible);
			}
			set
			{
				this.SetMemberRight(MemberRights.Visible, value);
			}
		}

		public ItemPermissionScope EditItems
		{
			get
			{
				return this.GetPermissionScope(MemberRights.EditAny, MemberRights.EditOwned);
			}
			set
			{
				this.SetPermissionScope(MemberRights.EditAny, MemberRights.EditOwned, value);
			}
		}

		public ItemPermissionScope DeleteItems
		{
			get
			{
				return this.GetPermissionScope(MemberRights.DeleteAny, MemberRights.DeleteOwned);
			}
			set
			{
				this.SetPermissionScope(MemberRights.DeleteAny, MemberRights.DeleteOwned, value);
			}
		}

		public MemberRights MemberRights
		{
			get
			{
				return this.memberRights;
			}
			set
			{
				this.ValidateMemberRights(value);
				this.memberRights = value;
			}
		}

		public static PermissionLevel GetPermissionLevel(MemberRights memberRights)
		{
			MemberRights memberRights2 = memberRights & ~(MemberRights.FreeBusySimple | MemberRights.FreeBusyDetailed);
			MemberRights memberRights3 = memberRights2;
			if (memberRights3 <= (MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.Visible))
			{
				if (memberRights3 <= MemberRights.Contact)
				{
					if (memberRights3 != MemberRights.None && memberRights3 != MemberRights.Contact)
					{
						return PermissionLevel.Custom;
					}
				}
				else
				{
					switch (memberRights3)
					{
					case MemberRights.Visible:
						break;
					case MemberRights.ReadAny | MemberRights.Visible:
						return PermissionLevel.Reviewer;
					case MemberRights.Create | MemberRights.Visible:
						return PermissionLevel.Contributor;
					default:
						if (memberRights3 == (MemberRights.ReadAny | MemberRights.Create | MemberRights.DeleteOwned | MemberRights.Visible))
						{
							return PermissionLevel.NonEditingAuthor;
						}
						if (memberRights3 != (MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.Visible))
						{
							return PermissionLevel.Custom;
						}
						return PermissionLevel.Author;
					}
				}
			}
			else
			{
				if (memberRights3 > (MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.EditAny | MemberRights.DeleteAny | MemberRights.CreateSubfolder | MemberRights.Visible))
				{
					if (memberRights3 != (MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.EditAny | MemberRights.DeleteAny | MemberRights.CreateSubfolder | MemberRights.Owner | MemberRights.Visible))
					{
						if (memberRights3 == (MemberRights.Contact | MemberRights.Visible))
						{
							goto IL_8E;
						}
						if (memberRights3 != (MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.EditAny | MemberRights.DeleteAny | MemberRights.CreateSubfolder | MemberRights.Owner | MemberRights.Contact | MemberRights.Visible))
						{
							return PermissionLevel.Custom;
						}
					}
					return PermissionLevel.Owner;
				}
				if (memberRights3 == (MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.EditAny | MemberRights.DeleteAny | MemberRights.Visible))
				{
					return PermissionLevel.Editor;
				}
				if (memberRights3 == (MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.CreateSubfolder | MemberRights.Visible))
				{
					return PermissionLevel.PublishingAuthor;
				}
				if (memberRights3 != (MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.EditAny | MemberRights.DeleteAny | MemberRights.CreateSubfolder | MemberRights.Visible))
				{
					return PermissionLevel.Custom;
				}
				return PermissionLevel.PublishingEditor;
			}
			IL_8E:
			if (memberRights == memberRights2)
			{
				return PermissionLevel.None;
			}
			return PermissionLevel.Custom;
		}

		public static MemberRights GetMemberRights(PermissionLevel permissionLevel)
		{
			switch (permissionLevel)
			{
			case PermissionLevel.None:
				return MemberRights.None;
			case PermissionLevel.Owner:
				return MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.EditAny | MemberRights.DeleteAny | MemberRights.CreateSubfolder | MemberRights.Owner | MemberRights.Contact | MemberRights.Visible;
			case PermissionLevel.PublishingEditor:
				return MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.EditAny | MemberRights.DeleteAny | MemberRights.CreateSubfolder | MemberRights.Visible;
			case PermissionLevel.Editor:
				return MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.EditAny | MemberRights.DeleteAny | MemberRights.Visible;
			case PermissionLevel.PublishingAuthor:
				return MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.CreateSubfolder | MemberRights.Visible;
			case PermissionLevel.Author:
				return MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.Visible;
			case PermissionLevel.NonEditingAuthor:
				return MemberRights.ReadAny | MemberRights.Create | MemberRights.DeleteOwned | MemberRights.Visible;
			case PermissionLevel.Reviewer:
				return MemberRights.ReadAny | MemberRights.Visible;
			case PermissionLevel.Contributor:
				return MemberRights.Create | MemberRights.Visible;
			default:
				throw new EnumOutOfRangeException("PermissionLevel", ServerStrings.BadEnumValue(typeof(PermissionLevel)));
			}
		}

		internal PermissionOrigin Origin
		{
			get
			{
				return this.origin;
			}
		}

		internal bool IsDirty
		{
			get
			{
				return this.origin == PermissionOrigin.New || this.memberRights != this.initialMemberRights;
			}
		}

		internal long MemberId
		{
			get
			{
				return this.memberId.Value;
			}
		}

		internal void MarkAsNew()
		{
			this.origin = PermissionOrigin.New;
		}

		protected virtual void ValidateMemberRights(MemberRights memberRights)
		{
			MemberRights memberRights2 = memberRights & ~Permission.ValidRegularFolderMemberRights;
			if (memberRights2 != MemberRights.None)
			{
				throw new EnumOutOfRangeException("memberRights");
			}
		}

		private bool GetMemberRight(MemberRights memberRight)
		{
			return (this.memberRights & memberRight) == memberRight;
		}

		private void SetMemberRight(MemberRights memberRight, bool value)
		{
			if (value)
			{
				this.memberRights |= memberRight;
				return;
			}
			this.memberRights &= ~memberRight;
		}

		private ItemPermissionScope GetPermissionScope(MemberRights anyScope, MemberRights ownerScope)
		{
			if ((this.memberRights & anyScope) == anyScope)
			{
				return ItemPermissionScope.AllItems;
			}
			if ((this.memberRights & ownerScope) == ownerScope)
			{
				return ItemPermissionScope.OwnedItems;
			}
			return ItemPermissionScope.None;
		}

		private void SetPermissionScope(MemberRights anyScope, MemberRights ownerScope, ItemPermissionScope value)
		{
			EnumValidator.ThrowIfInvalid<ItemPermissionScope>(value, "value");
			switch (value)
			{
			case ItemPermissionScope.None:
				this.memberRights &= ~(anyScope | ownerScope);
				return;
			case ItemPermissionScope.OwnedItems:
				this.memberRights |= ownerScope;
				this.memberRights &= ~anyScope;
				return;
			case ItemPermissionScope.AllItems:
				this.memberRights |= (anyScope | ownerScope);
				return;
			default:
				return;
			}
		}

		private const MemberRights PermissionLevelNone = MemberRights.None;

		private const MemberRights PermissionLevelNoneOption1 = MemberRights.Contact;

		private const MemberRights PermissionLevelNoneOption2 = MemberRights.Visible;

		private const MemberRights PermissionLevelNoneOption3 = MemberRights.Contact | MemberRights.Visible;

		private const MemberRights OwnerRightsOption1 = MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.EditAny | MemberRights.DeleteAny | MemberRights.CreateSubfolder | MemberRights.Owner | MemberRights.Visible;

		private const MemberRights OwnerRights = MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.EditAny | MemberRights.DeleteAny | MemberRights.CreateSubfolder | MemberRights.Owner | MemberRights.Contact | MemberRights.Visible;

		private const MemberRights PublishingEditorRights = MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.EditAny | MemberRights.DeleteAny | MemberRights.CreateSubfolder | MemberRights.Visible;

		private const MemberRights EditorRights = MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.EditAny | MemberRights.DeleteAny | MemberRights.Visible;

		private const MemberRights PublishingAuthorRights = MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.CreateSubfolder | MemberRights.Visible;

		private const MemberRights AuthorRights = MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.Visible;

		private const MemberRights NonEditingAuthorRights = MemberRights.ReadAny | MemberRights.Create | MemberRights.DeleteOwned | MemberRights.Visible;

		private const MemberRights ReviewerRights = MemberRights.ReadAny | MemberRights.Visible;

		private const MemberRights ContributorRights = MemberRights.Create | MemberRights.Visible;

		private const MemberRights FreeBusyRights = MemberRights.FreeBusySimple | MemberRights.FreeBusyDetailed;

		private readonly PermissionSecurityPrincipal securityPrincipal;

		private MemberRights memberRights;

		private readonly long? memberId = null;

		private readonly MemberRights initialMemberRights;

		private PermissionOrigin origin;

		private static readonly MemberRights ValidRegularFolderMemberRights = MemberRights.ReadAny | MemberRights.Create | MemberRights.EditOwned | MemberRights.DeleteOwned | MemberRights.EditAny | MemberRights.DeleteAny | MemberRights.CreateSubfolder | MemberRights.Owner | MemberRights.Contact | MemberRights.Visible;
	}
}
