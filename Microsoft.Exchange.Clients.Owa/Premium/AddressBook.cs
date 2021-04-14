using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class AddressBook : NavigationHost, IRegistryOnlyForm
	{
		public bool IsRoomPicker
		{
			get
			{
				return this.picker == AddressBook.pickers[3];
			}
		}

		public RecipientBlockType RecipientBlockType
		{
			get
			{
				if (this.picker == AddressBook.pickers[8] || this.picker == AddressBook.pickers[9] || this.picker == AddressBook.pickers[12])
				{
					return RecipientBlockType.DL;
				}
				if (this.picker == AddressBook.pickers[14])
				{
					return RecipientBlockType.PDL;
				}
				return RecipientBlockType.None;
			}
		}

		public bool IsPersonalAutoAttendantPicker
		{
			get
			{
				return this.picker == AddressBook.pickers[8] || this.picker == AddressBook.pickers[9];
			}
		}

		public bool IsPicker
		{
			get
			{
				return this.picker != null;
			}
		}

		protected bool IsMobileNumberPicker
		{
			get
			{
				return this.picker == AddressBook.pickers[12] || this.picker == AddressBook.pickers[11];
			}
		}

		protected bool IsSingleRecipientWell
		{
			get
			{
				return this.picker == AddressBook.pickers[7] || this.picker == AddressBook.pickers[9] || this.picker == AddressBook.pickers[14] || this.picker == AddressBook.pickers[6] || this.picker == AddressBook.pickers[13];
			}
		}

		protected bool ShouldRenderContactsInSecondaryNavigation
		{
			get
			{
				return this.picker != AddressBook.pickers[7] && this.picker != AddressBook.pickers[9] && this.picker != AddressBook.pickers[15];
			}
		}

		protected AddressBookRecipientPicker Picker
		{
			get
			{
				return this.picker;
			}
		}

		protected override NavigationModule SelectNavagationModule()
		{
			return NavigationModule.AddressBook;
		}

		protected override void OnInit(EventArgs e)
		{
			ExTraceGlobals.ContactsCallTracer.TraceDebug(0L, "AddressBook.OnInit");
			this.DeterminePicker();
			if (this.IsPicker)
			{
				this.lastModuleMappingAction = (this.IsMobileNumberPicker ? "PickMobile" : "Pick");
			}
			base.OnInit(e);
		}

		private void DeterminePicker()
		{
			string action = base.OwaContext.FormsRegistryContext.Action;
			if (action != null)
			{
				object obj = AddressBook.actionParser.Parse(action);
				this.picker = ((obj != null) ? AddressBook.pickers[(int)obj] : null);
			}
		}

		protected void RenderJavascriptEncodedContactsFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.ContactsFolderId.ToBase64String(), base.Response.Output);
		}

		private const string PickAction = "Pick";

		private const string PickMobileAction = "PickMobile";

		private const int BaseWidth = 990;

		protected const int NavigationPaneWidth = 184;

		protected const int VlvMinWidth = 325;

		public const int PickerDialogWidth = 990;

		public const int BrowseWindowWidth = 990;

		public const int WindowHeight = 641;

		private static readonly AddressBookRecipientPicker[] pickers = new AddressBookRecipientPicker[]
		{
			AddressBookRecipientPicker.Recipients,
			AddressBookRecipientPicker.Attendees,
			AddressBookRecipientPicker.DistributionListMember,
			AddressBookRecipientPicker.Rooms,
			AddressBookRecipientPicker.FromRecipients,
			AddressBookRecipientPicker.ToRecipients,
			AddressBookRecipientPicker.SendFromRecipients,
			AddressBookRecipientPicker.SelectOtherMailboxRecipient,
			AddressBookRecipientPicker.PersonalAutoAttendantCallers,
			AddressBookRecipientPicker.PersonalAutoAttendantOneCaller,
			AddressBookRecipientPicker.ChatParticipants,
			AddressBookRecipientPicker.ToMobileNumberOrDL,
			AddressBookRecipientPicker.ToMobileNumber,
			AddressBookRecipientPicker.AddBuddy,
			AddressBookRecipientPicker.Filter,
			AddressBookRecipientPicker.UsersAndGroups
		};

		private static readonly FastEnumParser actionParser = new FastEnumParser(typeof(AddressBook.Action), true);

		private AddressBookRecipientPicker picker;

		private enum Action
		{
			PickRecipients,
			PickAttendees,
			PickMembers,
			PickRooms,
			PickFrom,
			PickTo,
			PickSendFrom,
			PickSelectMailbox,
			PickPAACallers,
			PickPAAOneCaller,
			PickParticipants,
			PickMobileOrDL,
			PickMobile,
			PickBuddy,
			PickFilter,
			PickUsersAndGroups
		}
	}
}
