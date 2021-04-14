using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class PermissionRenderer : PermissionRendererBase<Permission, PermissionSetType, Microsoft.Exchange.Services.Core.Types.PermissionType>
	{
		internal PermissionRenderer(Folder folder)
		{
			this.permissionSet = PermissionRenderer.GetPermissionSet(folder);
		}

		protected override Permission GetDefaultPermission()
		{
			return this.permissionSet.DefaultPermission;
		}

		protected override Permission GetAnonymousPermission()
		{
			return this.permissionSet.AnonymousPermission;
		}

		protected override IEnumerator<Permission> GetPermissionEnumerator()
		{
			return this.permissionSet.GetEnumerator();
		}

		protected override string GetPermissionsArrayElementName()
		{
			return "Permissions";
		}

		protected override string GetPermissionElementName()
		{
			return "Permission";
		}

		private PermissionLevelType CreatePermissionLevel(PermissionLevel permissionLevel)
		{
			switch (permissionLevel)
			{
			case PermissionLevel.None:
				return PermissionLevelType.None;
			case PermissionLevel.Owner:
				return PermissionLevelType.Owner;
			case PermissionLevel.PublishingEditor:
				return PermissionLevelType.PublishingEditor;
			case PermissionLevel.Editor:
				return PermissionLevelType.Editor;
			case PermissionLevel.PublishingAuthor:
				return PermissionLevelType.PublishingAuthor;
			case PermissionLevel.Author:
				return PermissionLevelType.Author;
			case PermissionLevel.NonEditingAuthor:
				return PermissionLevelType.NoneditingAuthor;
			case PermissionLevel.Reviewer:
				return PermissionLevelType.Reviewer;
			case PermissionLevel.Contributor:
				return PermissionLevelType.Contributor;
			default:
				return PermissionLevelType.Custom;
			}
		}

		private PermissionReadAccess CreatePermissionReadAccess(bool canReadItems)
		{
			if (!canReadItems)
			{
				return PermissionReadAccess.None;
			}
			return PermissionReadAccess.FullDetails;
		}

		protected override void RenderByTypePermissionDetails(Microsoft.Exchange.Services.Core.Types.PermissionType permissionElement, Permission permission)
		{
			permissionElement.ReadItems = new PermissionReadAccess?(this.CreatePermissionReadAccess(permission.CanReadItems));
			permissionElement.PermissionLevel = this.CreatePermissionLevel(permission.PermissionLevel);
		}

		internal static PermissionSet GetPermissionSet(Folder folder)
		{
			PermissionSet result;
			try
			{
				FaultInjection.GenerateFault((FaultInjection.LIDs)3024497981U);
				result = folder.GetPermissionSet();
			}
			catch (StoragePermanentException ex)
			{
				if (ex.InnerException is MapiExceptionAmbiguousAlias)
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceError<StoragePermanentException>(0L, "Error occurred when fetching permission set for folder. Exception '{0}'.", ex);
					throw new ObjectCorruptException(ex, false);
				}
				throw;
			}
			return result;
		}

		protected override Microsoft.Exchange.Services.Core.Types.PermissionType CreatePermissionElement()
		{
			return new Microsoft.Exchange.Services.Core.Types.PermissionType();
		}

		protected override void SetPermissionsOnSerializationObject(PermissionSetType serviceProperty, List<Microsoft.Exchange.Services.Core.Types.PermissionType> renderedPermissions)
		{
			serviceProperty.Permissions = renderedPermissions.ToArray();
		}

		protected override void SetUnknownEntriesOnSerializationObject(PermissionSetType serviceProperty, string[] entries)
		{
			serviceProperty.UnknownEntries = entries;
		}

		protected override PermissionSetType CreatePermissionSetElement()
		{
			return new PermissionSetType();
		}

		private PermissionSet permissionSet;
	}
}
