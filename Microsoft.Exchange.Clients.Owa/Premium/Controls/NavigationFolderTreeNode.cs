using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class NavigationFolderTreeNode : FolderTreeNode
	{
		internal NavigationFolderTreeNode(UserContext userContext, NavigationNodeFolder nodeFolder) : base(userContext, OwaStoreObjectId.CreateFromNavigationNodeFolder(userContext, nodeFolder), nodeFolder.Subject, nodeFolder.GetFolderClass(), nodeFolder.IsFlagSet(NavigationNodeFlags.TodoFolder) ? DefaultFolderType.ToDoSearch : DefaultFolderType.None)
		{
			if (nodeFolder.IsNew)
			{
				throw new ArgumentException("Should not use newly created node");
			}
			this.navigationNodeFolder = nodeFolder;
			this.elcPolicyFolderId = null;
		}

		internal NavigationFolderTreeNode(UserContext userContext, NavigationNodeFolder nodeFolder, StoreObjectId elcPolicyFolderId, object[] values, Dictionary<PropertyDefinition, int> propertyMap) : base(userContext, OwaStoreObjectId.CreateFromNavigationNodeFolder(userContext, nodeFolder).GetSession(userContext), values, propertyMap)
		{
			if (nodeFolder.IsNew)
			{
				throw new ArgumentException("Should not use newly created node");
			}
			this.navigationNodeFolder = nodeFolder;
			this.elcPolicyFolderId = elcPolicyFolderId;
		}

		internal NavigationFolderTreeNode(UserContext userContext, NavigationNodeFolder nodeFolder, Folder folder) : base(userContext, folder)
		{
			if (nodeFolder.IsNew)
			{
				throw new ArgumentException("Should not use newly created node");
			}
			this.navigationNodeFolder = nodeFolder;
			this.elcPolicyFolderId = null;
		}

		public override string Id
		{
			get
			{
				return "f" + this.navigationNodeFolder.NavigationNodeId.ObjectId.ToString();
			}
		}

		internal override string DisplayName
		{
			get
			{
				if (base.FolderType == DefaultFolderType.ToDoSearch)
				{
					return LocalizedStrings.GetNonEncoded(-1954334922);
				}
				string text = (this.navigationNodeFolder.NavigationNodeGroupSection == NavigationNodeGroupSection.First) ? base.DisplayName : this.FolderName;
				if (this.navigationNodeFolder.NavigationNodeType == NavigationNodeType.NormalFolder && !StringComparer.OrdinalIgnoreCase.Equals(this.navigationNodeFolder.MailboxLegacyDN, base.UserContext.MailboxOwnerLegacyDN))
				{
					return string.Format(LocalizedStrings.GetNonEncoded(-83764036), text, Utilities.GetMailboxOwnerDisplayName((MailboxSession)base.FolderId.GetSession(base.UserContext)));
				}
				return text;
			}
		}

		protected override void RenderAdditionalProperties(TextWriter writer)
		{
			writer.Write(" _fid=\"");
			Utilities.HtmlEncode(base.FolderId.ToBase64String(), writer);
			writer.Write("\"");
			if (this.navigationNodeFolder.NavigationNodeGroupSection == NavigationNodeGroupSection.Calendar && CalendarColorManager.IsColorIndexValid(this.navigationNodeFolder.NavigationNodeCalendarColor))
			{
				writer.Write(" _iSavedColor=");
				writer.Write(CalendarColorManager.GetClientColorIndex(this.navigationNodeFolder.NavigationNodeCalendarColor));
			}
			if (this.navigationNodeFolder.IsFilteredView)
			{
				writer.Write(" _fltr=1");
			}
			if (this.navigationNodeFolder.NavigationNodeGroupSection == NavigationNodeGroupSection.First && this.elcPolicyFolderId != null)
			{
				writer.Write(" _sPlcyFId=\"");
				Utilities.HtmlEncode(this.elcPolicyFolderId.ToBase64String(), writer);
				writer.Write("\"");
			}
			base.RenderAdditionalProperties(writer);
		}

		internal override void RenderNodeBody(TextWriter writer)
		{
			if (this.navigationNodeFolder.NavigationNodeGroupSection == NavigationNodeGroupSection.Calendar)
			{
				base.UserContext.RenderThemeImage(writer, base.Selected ? ThemeFileId.CheckChecked : ThemeFileId.CheckUnchecked, null, new object[]
				{
					"id=\"imgCk\""
				});
			}
			base.RenderNodeBody(writer);
		}

		internal override bool HasChildren
		{
			get
			{
				return false;
			}
		}

		internal override bool Selectable
		{
			get
			{
				return true;
			}
		}

		protected override FolderTreeNode.ContentCountDisplayType ContentCountDisplay
		{
			get
			{
				if (this.navigationNodeFolder.NavigationNodeGroupSection != NavigationNodeGroupSection.First)
				{
					return FolderTreeNode.ContentCountDisplayType.None;
				}
				return base.ContentCountDisplay;
			}
		}

		protected override void RenderIcon(TextWriter writer, params string[] extraAttributes)
		{
			if (this.navigationNodeFolder.IsFilteredView)
			{
				base.UserContext.RenderThemeImage(writer, ThemeFileId.FavoritesFilter, null, extraAttributes);
				return;
			}
			base.RenderIcon(writer, extraAttributes);
		}

		private readonly StoreObjectId elcPolicyFolderId;

		private readonly NavigationNodeFolder navigationNodeFolder;
	}
}
