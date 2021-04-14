using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.DocumentLibrary;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class DocumentViewArrangeByMenu : ArrangeByMenu
	{
		internal DocumentViewArrangeByMenu(UriFlags libraryType)
		{
			this.libraryType = libraryType;
		}

		protected override void RenderMenuItems(TextWriter output, UserContext userContext)
		{
			UriFlags uriFlags = this.libraryType;
			switch (uriFlags)
			{
			case UriFlags.Sharepoint:
				ArrangeByMenu.RenderMenuItem(output, -1966747349, null, ColumnId.SharepointDocumentLibraryDisplayName);
				ArrangeByMenu.RenderMenuItem(output, 873740972, null, ColumnId.SharepointDocumentLibraryDescription);
				ArrangeByMenu.RenderMenuItem(output, 1554779067, null, ColumnId.SharepointDocumentLibraryItemCount);
				ArrangeByMenu.RenderMenuItem(output, 869905365, null, ColumnId.SharepointDocumentLibraryLastModified);
				return;
			case UriFlags.Unc:
				ArrangeByMenu.RenderMenuItem(output, -1966747349, null, ColumnId.UncDocumentDisplayName);
				ArrangeByMenu.RenderMenuItem(output, 869905365, null, ColumnId.UncDocumentLastModified);
				return;
			case UriFlags.Sharepoint | UriFlags.Unc:
			case UriFlags.DocumentLibrary:
				return;
			case UriFlags.SharepointDocumentLibrary:
				goto IL_81;
			case UriFlags.UncDocumentLibrary:
				break;
			default:
				switch (uriFlags)
				{
				case UriFlags.SharepointFolder:
					goto IL_81;
				case UriFlags.UncFolder:
					break;
				default:
					return;
				}
				break;
			}
			ArrangeByMenu.RenderMenuItem(output, -1966747349, null, ColumnId.UncDocumentDisplayName);
			ArrangeByMenu.RenderMenuItem(output, 869905365, null, ColumnId.UncDocumentLastModified);
			ArrangeByMenu.RenderMenuItem(output, -837446919, null, ColumnId.UncDocumentFileSize);
			return;
			IL_81:
			ArrangeByMenu.RenderMenuItem(output, -1966747349, null, ColumnId.SharepointDocumentDisplayName);
			ArrangeByMenu.RenderMenuItem(output, 869905365, null, ColumnId.SharepointDocumentLastModified);
			ArrangeByMenu.RenderMenuItem(output, 1276881056, null, ColumnId.SharepointDocumentModifiedBy);
			ArrangeByMenu.RenderMenuItem(output, -580782680, null, ColumnId.SharepointDocumentCheckedOutTo);
			ArrangeByMenu.RenderMenuItem(output, -837446919, null, ColumnId.SharepointDocumentFileSize);
		}

		private UriFlags libraryType;
	}
}
