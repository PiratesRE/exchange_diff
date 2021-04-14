using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class AddressBookViewState
	{
		internal static ReadingPanePosition DefaultReadingPanePosition
		{
			get
			{
				return ReadingPanePosition.Right;
			}
		}

		internal ReadingPanePosition ReadingPanePosition
		{
			get
			{
				return this.readingPanePosition;
			}
		}

		internal bool DefaultMultiLineSetting
		{
			get
			{
				return !this.isRoomView;
			}
		}

		internal bool IsMultiLine
		{
			get
			{
				return this.isMultiLine;
			}
		}

		internal bool FindBarOn
		{
			get
			{
				return true;
			}
		}

		internal static AddressBookViewState Load(UserContext userContext, bool isPicker, bool isRoomView)
		{
			PropertyDefinition[] propsToReturn = isPicker ? AddressBookViewState.pickerProperties : AddressBookViewState.browseProperties;
			PropertyDefinition propertyDefinition = isPicker ? ViewStateProperties.AddressBookPickerMultiLine : ViewStateProperties.AddressBookLookupMultiLine;
			AddressBookViewState addressBookViewState = new AddressBookViewState();
			addressBookViewState.isRoomView = isRoomView;
			if (userContext.IsWebPartRequest)
			{
				return addressBookViewState;
			}
			using (Folder folder = Folder.Bind(userContext.MailboxSession, DefaultFolderType.Root, propsToReturn))
			{
				addressBookViewState.isMultiLine = Utilities.GetFolderProperty<bool>(folder, propertyDefinition, !isRoomView);
				if (isPicker)
				{
					return addressBookViewState;
				}
				addressBookViewState.readingPanePosition = Utilities.GetFolderProperty<ReadingPanePosition>(folder, ViewStateProperties.AddressBookLookupReadingPanePosition, ReadingPanePosition.Right);
				if (!FolderViewStates.ValidateReadingPanePosition(addressBookViewState.readingPanePosition))
				{
					addressBookViewState.readingPanePosition = ReadingPanePosition.Right;
				}
			}
			return addressBookViewState;
		}

		public static void PersistMultiLineState(UserContext userContext, bool isMultiLine, bool isPicker)
		{
			if (!userContext.IsWebPartRequest)
			{
				PropertyDefinition propertyDefinition = isPicker ? ViewStateProperties.AddressBookPickerMultiLine : ViewStateProperties.AddressBookLookupMultiLine;
				Folder folder = Folder.Bind(userContext.MailboxSession, DefaultFolderType.Root);
				using (folder)
				{
					folder[propertyDefinition] = isMultiLine;
					folder.Save();
				}
			}
		}

		public static void PersistReadingPane(UserContext userContext, ReadingPanePosition readingPanePosition)
		{
			if (!userContext.IsWebPartRequest)
			{
				ExTraceGlobals.ContactsTracer.TraceDebug(0L, "AdderssBookViewState.PersistAddressBookReadingPane");
				Folder folder = Folder.Bind(userContext.MailboxSession, DefaultFolderType.Root);
				using (folder)
				{
					folder[ViewStateProperties.AddressBookLookupReadingPanePosition] = readingPanePosition;
					folder.Save();
				}
			}
		}

		private const bool FindBarOnValue = true;

		private const ReadingPanePosition ReadingPaneDefaultPosition = ReadingPanePosition.Right;

		internal const int ViewWidth = 365;

		internal const int ViewHeight = 250;

		private static readonly PropertyDefinition[] browseProperties = new PropertyDefinition[]
		{
			ViewStateProperties.AddressBookLookupMultiLine,
			ViewStateProperties.AddressBookLookupReadingPanePosition
		};

		private static readonly PropertyDefinition[] pickerProperties = new PropertyDefinition[]
		{
			ViewStateProperties.AddressBookPickerMultiLine
		};

		private bool isMultiLine = true;

		private ReadingPanePosition readingPanePosition = ReadingPanePosition.Right;

		private bool isRoomView;
	}
}
