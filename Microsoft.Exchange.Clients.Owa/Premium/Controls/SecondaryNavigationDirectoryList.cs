using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class SecondaryNavigationDirectoryList : SecondaryNavigationList
	{
		public static SecondaryNavigationDirectoryList CreateCondensedDirectoryList(UserContext userContext, bool isRoomPicker)
		{
			SecondaryNavigationDirectoryList secondaryNavigationDirectoryList = new SecondaryNavigationDirectoryList(userContext);
			secondaryNavigationDirectoryList.AddEntry(userContext.GlobalAddressListInfo.DisplayName, userContext.GlobalAddressListInfo.ToBase64String(), !isRoomPicker, false);
			if (DirectoryAssistance.IsRoomsAddressListAvailable(userContext) && userContext.AllRoomsAddressBookInfo != null && !userContext.AllRoomsAddressBookInfo.IsEmpty)
			{
				secondaryNavigationDirectoryList.AddEntry(userContext.AllRoomsAddressBookInfo.DisplayName, userContext.AllRoomsAddressBookInfo.ToBase64String(), isRoomPicker, true);
			}
			return secondaryNavigationDirectoryList;
		}

		public static SecondaryNavigationDirectoryList CreateExtendedDirectoryList(UserContext userContext)
		{
			SecondaryNavigationDirectoryList secondaryNavigationDirectoryList = new SecondaryNavigationDirectoryList(userContext);
			AddressBookBase[] allAddressBooks = DirectoryAssistance.GetAllAddressBooks(userContext);
			for (int i = 0; i < allAddressBooks.Length; i++)
			{
				if (!string.Equals(allAddressBooks[i].Base64Guid, userContext.GlobalAddressListInfo.ToBase64String(), StringComparison.Ordinal) && (userContext.AllRoomsAddressBookInfo == null || !string.Equals(allAddressBooks[i].Base64Guid, userContext.AllRoomsAddressBookInfo.ToBase64String(), StringComparison.Ordinal)))
				{
					secondaryNavigationDirectoryList.AddEntry(allAddressBooks[i].DisplayName, allAddressBooks[i].Base64Guid, false, false);
				}
			}
			return secondaryNavigationDirectoryList;
		}

		private SecondaryNavigationDirectoryList(UserContext userContext) : base("divDirLst")
		{
			this.userContext = userContext;
		}

		private static void RenderMoreOrLess(TextWriter output, UserContext userContext, bool moreOrLess, ThemeFileId image)
		{
			output.Write("<div id=\"");
			output.Write(moreOrLess ? "divABMore" : "divABLess");
			output.Write("\" class=\"abMoreLessWrap\" _fMrLs=\"1\"");
			if (!moreOrLess)
			{
				output.Write(" style=\"display:none\"");
			}
			output.Write("><span class=\"abMoreLess\">");
			output.Write(LocalizedStrings.GetHtmlEncoded(moreOrLess ? 1132752106 : -584522130));
			output.Write("</span>&nbsp;");
			userContext.RenderThemeImage(output, image, "abMoreLessImg", new object[0]);
			output.Write("</div>");
		}

		private void AddEntry(string displayString, string containerId, bool isSelected, bool isRoom)
		{
			SecondaryNavigationDirectoryList.DirectoryListEntryInfo item;
			item.IsSelected = isSelected;
			item.IsRoom = isRoom;
			item.DisplayString = displayString;
			item.ContainerId = containerId;
			this.entries.Add(item);
		}

		protected override int Count
		{
			get
			{
				return this.entries.Count;
			}
		}

		protected override void RenderEntryOnClickHandler(TextWriter output, int entryIndex)
		{
			Utilities.HtmlEncode("onClkABFld(\"", output);
			Utilities.HtmlEncode(Utilities.JavascriptEncode(this.entries[entryIndex].ContainerId), output);
			Utilities.HtmlEncode("\",\"", output);
			Utilities.HtmlEncode(this.entries[entryIndex].IsRoom ? "Rooms" : "Recipients", output);
			Utilities.HtmlEncode("\")", output);
		}

		protected override void RenderEntryAttributes(TextWriter output, int entryIndex)
		{
			if (this.entries[entryIndex].IsSelected)
			{
				output.Write("_sel=\"1\"");
			}
		}

		protected override void RenderEntryIcon(TextWriter output, int entryIndex)
		{
			this.userContext.RenderThemeImage(output, ThemeFileId.AddressBook, "snlADImg", new object[0]);
		}

		protected override string GetEntryText(int entryIndex)
		{
			return this.entries[entryIndex].DisplayString;
		}

		protected override void RenderFooter(TextWriter output)
		{
			if (this.userContext.IsFeatureEnabled(Feature.AddressLists) && this.userContext.GlobalAddressListInfo.Origin == GlobalAddressListInfo.GalOrigin.DefaultGlobalAddressList)
			{
				SecondaryNavigationDirectoryList.RenderMoreOrLess(output, this.userContext, true, ThemeFileId.Expand);
				SecondaryNavigationDirectoryList.RenderMoreOrLess(output, this.userContext, false, ThemeFileId.Collapse);
				output.Write("<div id=\"divAllAL\" style=\"display:none\"></div>");
			}
		}

		private UserContext userContext;

		private List<SecondaryNavigationDirectoryList.DirectoryListEntryInfo> entries = new List<SecondaryNavigationDirectoryList.DirectoryListEntryInfo>();

		private struct DirectoryListEntryInfo
		{
			public string DisplayString;

			public string ContainerId;

			public bool IsSelected;

			public bool IsRoom;
		}
	}
}
