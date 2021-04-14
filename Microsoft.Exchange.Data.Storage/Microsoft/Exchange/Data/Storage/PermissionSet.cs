using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PermissionSet : IEnumerable<Permission>, IEnumerable
	{
		public PermissionSet(PermissionTable permissionTable)
		{
			this.permissionTable = permissionTable;
		}

		public Permission AddEntry(PermissionSecurityPrincipal securityPrincipal, PermissionLevel initialPermissionLevel)
		{
			if (!EnumValidator<PermissionLevel>.IsMemberOf(initialPermissionLevel, PermissionSet.ValidPermissionLevelForAddEntry))
			{
				throw new EnumOutOfRangeException("initialPermissionLevel", ServerStrings.BadEnumValue(typeof(PermissionLevel)));
			}
			Permission permission = this.PermissionTable.AddEntry(securityPrincipal, MemberRights.None);
			permission.PermissionLevel = initialPermissionLevel;
			return permission;
		}

		public Permission AddEntry(PermissionSecurityPrincipal securityPrincipal, MemberRights memberRights)
		{
			return this.PermissionTable.AddEntry(securityPrincipal, memberRights);
		}

		public void RemoveEntry(PermissionSecurityPrincipal securityPrincipal)
		{
			this.PermissionTable.RemoveEntry(securityPrincipal);
		}

		public void Clear()
		{
			this.PermissionTable.Clear();
		}

		public Permission GetEntry(PermissionSecurityPrincipal securityPrincipal)
		{
			return this.PermissionTable.GetEntry(securityPrincipal);
		}

		public Permission DefaultPermission
		{
			get
			{
				return this.PermissionTable.DefaultPermission;
			}
		}

		public void SetDefaultPermission(MemberRights memberRights)
		{
			EnumValidator<MemberRights>.ThrowIfInvalid(memberRights, "memberRights");
			if (this.PermissionTable.DefaultPermission != null)
			{
				this.PermissionTable.DefaultPermission.MemberRights = memberRights;
				return;
			}
			this.PermissionTable.DefaultPermission = this.CreatePermission(new PermissionSecurityPrincipal(PermissionSecurityPrincipal.SpecialPrincipalType.Default), memberRights, 0L);
			this.PermissionTable.DefaultPermission.MarkAsNew();
		}

		public Permission AnonymousPermission
		{
			get
			{
				return this.PermissionTable.AnonymousPermission;
			}
		}

		public void SetAnonymousPermission(MemberRights memberRights)
		{
			EnumValidator<MemberRights>.ThrowIfInvalid(memberRights, "memberRights");
			if (this.PermissionTable.AnonymousPermission != null)
			{
				this.PermissionTable.AnonymousPermission.MemberRights = memberRights;
				return;
			}
			this.PermissionTable.AnonymousPermission = this.CreatePermission(new PermissionSecurityPrincipal(PermissionSecurityPrincipal.SpecialPrincipalType.Anonymous), memberRights, -1L);
			this.PermissionTable.AnonymousPermission.MarkAsNew();
		}

		internal virtual Permission CreatePermission(PermissionSecurityPrincipal securityPrincipal, MemberRights memberRights)
		{
			return new Permission(securityPrincipal, memberRights);
		}

		internal virtual Permission CreatePermission(PermissionSecurityPrincipal securityPrincipal, MemberRights memberRights, long memberId)
		{
			return new Permission(securityPrincipal, memberRights, memberId);
		}

		internal virtual ModifyTableOptions ModifyTableOptions
		{
			get
			{
				return ModifyTableOptions.None;
			}
		}

		protected PermissionTable PermissionTable
		{
			get
			{
				return this.permissionTable;
			}
		}

		public IEnumerator<Permission> GetEnumerator()
		{
			return this.PermissionTable.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly PermissionTable permissionTable;

		private static readonly PermissionLevel[] ValidPermissionLevelForAddEntry = new PermissionLevel[]
		{
			PermissionLevel.None,
			PermissionLevel.Owner,
			PermissionLevel.PublishingEditor,
			PermissionLevel.Editor,
			PermissionLevel.PublishingAuthor,
			PermissionLevel.Author,
			PermissionLevel.NonEditingAuthor,
			PermissionLevel.Reviewer,
			PermissionLevel.Contributor
		};
	}
}
