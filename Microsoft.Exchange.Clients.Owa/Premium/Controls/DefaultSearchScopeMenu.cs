using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class DefaultSearchScopeMenu : ContextMenu
	{
		internal static DefaultSearchScopeMenu Create(UserContext userContext, OutlookModule outlookModule, string archiveName)
		{
			return new DefaultSearchScopeMenu(userContext, outlookModule, archiveName);
		}

		private DefaultSearchScopeMenu(UserContext userContext, OutlookModule outlookModule, string archiveName) : base("divDftScpMnu", userContext, true)
		{
			this.outlookModule = outlookModule;
			this.archiveName = archiveName;
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			SearchScope searchScope = this.userContext.UserOptions.GetSearchScope(this.outlookModule);
			base.RenderMenuItem(output, 1749416719, (searchScope == SearchScope.SelectedFolder) ? ThemeFileId.Search : ThemeFileId.None, "divFS" + DefaultSearchScopeMenu.selectedFolder, "d" + DefaultSearchScopeMenu.selectedFolder, false);
			base.RenderMenuItem(output, -1578460849, (searchScope == SearchScope.SelectedAndSubfolders) ? ThemeFileId.Search : ThemeFileId.None, "divFS" + DefaultSearchScopeMenu.selectedAndSubFolders, "d" + DefaultSearchScopeMenu.selectedAndSubFolders, false);
			if (this.outlookModule == OutlookModule.Tasks || this.outlookModule == OutlookModule.Contacts)
			{
				Strings.IDs displayString = (this.outlookModule == OutlookModule.Tasks) ? -464657744 : -1237143503;
				base.RenderMenuItem(output, displayString, (searchScope == SearchScope.AllItemsInModule) ? ThemeFileId.Search : ThemeFileId.None, "divFS" + DefaultSearchScopeMenu.allItemsInModule, "d" + DefaultSearchScopeMenu.allItemsInModule, false);
				return;
			}
			string text;
			if (!string.IsNullOrEmpty(this.archiveName))
			{
				text = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(-1597765325), new object[]
				{
					this.archiveName
				});
			}
			else
			{
				text = LocalizedStrings.GetNonEncoded(591328129);
			}
			base.RenderMenuItem(output, text, (searchScope == SearchScope.AllFoldersAndItems) ? ThemeFileId.Search : ThemeFileId.None, "divFS" + DefaultSearchScopeMenu.allFolders, "d" + DefaultSearchScopeMenu.allFolders, false, null, null);
		}

		private static readonly string selectedFolder = 0.ToString(CultureInfo.InvariantCulture);

		private static readonly string selectedAndSubFolders = 1.ToString(CultureInfo.InvariantCulture);

		private static readonly string allItemsInModule = 2.ToString(CultureInfo.InvariantCulture);

		private static readonly string allFolders = 3.ToString(CultureInfo.InvariantCulture);

		private OutlookModule outlookModule;

		private string archiveName;
	}
}
