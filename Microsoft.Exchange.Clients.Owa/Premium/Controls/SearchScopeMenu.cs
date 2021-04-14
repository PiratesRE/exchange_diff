using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class SearchScopeMenu : ContextMenu
	{
		internal SearchScopeMenu(UserContext userContext, OutlookModule outlookModule, string archiveName) : base("divScpMnu", userContext, false)
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
			output.Write("<div class=\"sttc sttcs bld\" nowrap>");
			output.Write(LocalizedStrings.GetHtmlEncoded(656259478));
			output.Write("</div>");
			base.RenderMenuItem(output, 1749416719, SearchScopeMenu.selectedFolder, false);
			base.RenderMenuItem(output, -1578460849, SearchScopeMenu.selectedAndSubFolders, !this.userContext.MailboxSession.Mailbox.IsContentIndexingEnabled);
			if (this.outlookModule == OutlookModule.Tasks || this.outlookModule == OutlookModule.Contacts)
			{
				Strings.IDs displayString = (this.outlookModule == OutlookModule.Tasks) ? -464657744 : -1237143503;
				base.RenderMenuItem(output, displayString, SearchScopeMenu.allItemsInModule, !this.userContext.MailboxSession.Mailbox.IsContentIndexingEnabled);
			}
			else
			{
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
				base.RenderMenuItem(output, text, ThemeFileId.None, null, SearchScopeMenu.allFolders, !this.userContext.MailboxSession.Mailbox.IsContentIndexingEnabled, null, null);
			}
			if (this.userContext.MailboxSession.Mailbox.IsContentIndexingEnabled)
			{
				base.RenderMenuItem(output, 285187440, ThemeFileId.None, null, null, false, null, null, DefaultSearchScopeMenu.Create(this.userContext, this.outlookModule, this.archiveName));
			}
		}

		private static readonly string selectedFolder = 0.ToString(CultureInfo.InvariantCulture);

		private static readonly string selectedAndSubFolders = 1.ToString(CultureInfo.InvariantCulture);

		private static readonly string allItemsInModule = 2.ToString(CultureInfo.InvariantCulture);

		private static readonly string allFolders = 3.ToString(CultureInfo.InvariantCulture);

		private OutlookModule outlookModule;

		private string archiveName;
	}
}
