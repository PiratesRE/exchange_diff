using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class OwaViewStateConfiguration : UserConfigurationBaseType
	{
		public OwaViewStateConfiguration() : base(OwaViewStateConfiguration.configurationName)
		{
		}

		[DataMember]
		public TemperatureUnit TemperatureUnit
		{
			get
			{
				return (TemperatureUnit)base[UserConfigurationPropertyId.TemperatureUnit];
			}
			set
			{
				base[UserConfigurationPropertyId.TemperatureUnit] = value;
			}
		}

		[DataMember]
		public string[] BookmarkedWeatherLocations
		{
			get
			{
				return (string[])base[UserConfigurationPropertyId.BookmarkedWeatherLocations];
			}
			set
			{
				base[UserConfigurationPropertyId.BookmarkedWeatherLocations] = value;
			}
		}

		[DataMember]
		public int CalendarViewTypeNarrow
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.CalendarViewTypeNarrow];
			}
			set
			{
				base[UserConfigurationPropertyId.CalendarViewTypeNarrow] = value;
			}
		}

		[DataMember]
		public int CalendarViewTypeWide
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.CalendarViewTypeWide];
			}
			set
			{
				base[UserConfigurationPropertyId.CalendarViewTypeWide] = value;
			}
		}

		[DataMember]
		public int CalendarViewTypeDesktop
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.CalendarViewTypeDesktop];
			}
			set
			{
				base[UserConfigurationPropertyId.CalendarViewTypeDesktop] = value;
			}
		}

		[DataMember]
		public bool CalendarSidePanelIsExpanded
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.CalendarSidePanelIsExpanded];
			}
			set
			{
				base[UserConfigurationPropertyId.CalendarSidePanelIsExpanded] = value;
			}
		}

		[DataMember]
		public int CalendarSidePanelMonthPickerCount
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.CalendarSidePanelMonthPickerCount];
			}
			set
			{
				base[UserConfigurationPropertyId.CalendarSidePanelMonthPickerCount] = value;
			}
		}

		[DataMember]
		public int CurrentWeatherLocationBookmarkIndex
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.CurrentWeatherLocationBookmarkIndex];
			}
			set
			{
				base[UserConfigurationPropertyId.CurrentWeatherLocationBookmarkIndex] = value;
			}
		}

		[DataMember]
		public string[] SelectedCalendarsDesktop
		{
			get
			{
				return (string[])base[UserConfigurationPropertyId.SelectedCalendarsDesktop];
			}
			set
			{
				base[UserConfigurationPropertyId.SelectedCalendarsDesktop] = value;
			}
		}

		[DataMember]
		public string[] SelectedCalendarsTWide
		{
			get
			{
				return (string[])base[UserConfigurationPropertyId.SelectedCalendarsTWide];
			}
			set
			{
				base[UserConfigurationPropertyId.SelectedCalendarsTWide] = value;
			}
		}

		[DataMember]
		public string[] SelectedCalendarsTNarrow
		{
			get
			{
				return (string[])base[UserConfigurationPropertyId.SelectedCalendarsTNarrow];
			}
			set
			{
				base[UserConfigurationPropertyId.SelectedCalendarsTNarrow] = value;
			}
		}

		[DataMember]
		public string[] FolderViewState
		{
			get
			{
				return (string[])base[UserConfigurationPropertyId.FolderViewState];
			}
			set
			{
				base[UserConfigurationPropertyId.FolderViewState] = value;
			}
		}

		[DataMember]
		public string GlobalFolderViewState
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.GlobalFolderViewState];
			}
			set
			{
				base[UserConfigurationPropertyId.GlobalFolderViewState] = value;
			}
		}

		[DataMember]
		public int SchedulingViewType
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.SchedulingViewType];
			}
			set
			{
				base[UserConfigurationPropertyId.SchedulingViewType] = value;
			}
		}

		[DataMember]
		public string SchedulingLastUsedRoomListName
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.SchedulingLastUsedRoomListName];
			}
			set
			{
				base[UserConfigurationPropertyId.SchedulingLastUsedRoomListName] = value;
			}
		}

		[DataMember]
		public string SchedulingLastUsedRoomListEmailAddress
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.SchedulingLastUsedRoomListEmailAddress];
			}
			set
			{
				base[UserConfigurationPropertyId.SchedulingLastUsedRoomListEmailAddress] = value;
			}
		}

		[DataMember]
		public string[] SearchHistory
		{
			get
			{
				return (string[])base[UserConfigurationPropertyId.SearchHistory];
			}
			set
			{
				base[UserConfigurationPropertyId.SearchHistory] = value;
			}
		}

		[DataMember]
		public int PeopleHubDisplayOption
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.PeopleHubDisplayOption];
			}
			set
			{
				base[UserConfigurationPropertyId.PeopleHubDisplayOption] = value;
			}
		}

		[DataMember]
		public int PeopleHubSortOption
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.PeopleHubSortOption];
			}
			set
			{
				base[UserConfigurationPropertyId.PeopleHubSortOption] = value;
			}
		}

		[DataMember]
		public int AttachmentsFilePickerViewTypeForMouse
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.AttachmentsFilePickerViewTypeForMouse];
			}
			set
			{
				base[UserConfigurationPropertyId.AttachmentsFilePickerViewTypeForMouse] = value;
			}
		}

		[DataMember]
		public int AttachmentsFilePickerViewTypeForTouch
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.AttachmentsFilePickerViewTypeForTouch];
			}
			set
			{
				base[UserConfigurationPropertyId.AttachmentsFilePickerViewTypeForTouch] = value;
			}
		}

		[DataMember]
		public bool AttachmentsFilePickerHideBanner
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.AttachmentsFilePickerHideBanner];
			}
			set
			{
				base[UserConfigurationPropertyId.AttachmentsFilePickerHideBanner] = value;
			}
		}

		[DataMember]
		public bool CalendarAgendaViewIsExpandedMouse
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.CalendarAgendaViewIsExpandedMouse];
			}
			set
			{
				base[UserConfigurationPropertyId.CalendarAgendaViewIsExpandedMouse] = value;
			}
		}

		[DataMember]
		public bool CalendarAgendaViewIsExpandedTWide
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.CalendarAgendaViewIsExpandedTWide];
			}
			set
			{
				base[UserConfigurationPropertyId.CalendarAgendaViewIsExpandedTWide] = value;
			}
		}

		internal override UserConfigurationPropertySchemaBase Schema
		{
			get
			{
				return ViewStatePropertySchema.Instance;
			}
		}

		internal void LoadAll(MailboxSession session)
		{
			IList<UserConfigurationPropertyDefinition> properties = new List<UserConfigurationPropertyDefinition>(base.OptionProperties.Keys);
			base.Load(session, properties, true);
		}

		private static string configurationName = "OWA.ViewStateConfiguration";
	}
}
