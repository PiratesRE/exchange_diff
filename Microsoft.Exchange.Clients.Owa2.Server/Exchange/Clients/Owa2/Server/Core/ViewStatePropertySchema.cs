using System;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class ViewStatePropertySchema : UserConfigurationPropertySchemaBase
	{
		private ViewStatePropertySchema()
		{
		}

		internal static ViewStatePropertySchema Instance
		{
			get
			{
				ViewStatePropertySchema result;
				if ((result = ViewStatePropertySchema.instance) == null)
				{
					result = (ViewStatePropertySchema.instance = new ViewStatePropertySchema());
				}
				return result;
			}
		}

		internal override UserConfigurationPropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return ViewStatePropertySchema.propertyDefinitions;
			}
		}

		internal override UserConfigurationPropertyId PropertyDefinitionsBaseId
		{
			get
			{
				return UserConfigurationPropertyId.CalendarViewTypeNarrow;
			}
		}

		private static readonly UserConfigurationPropertyDefinition[] propertyDefinitions = new UserConfigurationPropertyDefinition[]
		{
			new UserConfigurationPropertyDefinition("CalendarViewTypeNarrow", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateCalendarViewTypeNarrow)),
			new UserConfigurationPropertyDefinition("CalendarViewTypeWide", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateCalendarViewType)),
			new UserConfigurationPropertyDefinition("CalendarViewTypeDesktop", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateCalendarViewType)),
			new UserConfigurationPropertyDefinition("CalendarSidePanelIsExpanded", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateCalendarSidePanelIsExpanded)),
			new UserConfigurationPropertyDefinition("FolderViewState", typeof(string[])),
			new UserConfigurationPropertyDefinition("SchedulingViewType", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSchedulingViewType)),
			new UserConfigurationPropertyDefinition("SchedulingLastUsedRoomListName", typeof(string)),
			new UserConfigurationPropertyDefinition("SchedulingLastUsedRoomListEmailAddress", typeof(string)),
			new UserConfigurationPropertyDefinition("SearchHistory", typeof(string[])),
			new UserConfigurationPropertyDefinition("PeopleHubDisplayOption", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidatePeopleHubDisplayOptionType)),
			new UserConfigurationPropertyDefinition("PeopleHubSortOption", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidatePeopleHubSortOptionType)),
			new UserConfigurationPropertyDefinition("CalendarSidePanelMonthPickerCount", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateCalendarSidePanelMonthPickerCount)),
			new UserConfigurationPropertyDefinition("SelectedCalendarsDesktop", typeof(string[])),
			new UserConfigurationPropertyDefinition("SelectedCalendarsTWide", typeof(string[])),
			new UserConfigurationPropertyDefinition("SelectedCalendarsTNarrow", typeof(string[])),
			new UserConfigurationPropertyDefinition("AttachmentsFilePickerViewTypeForMouse", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateAttachmentsFilePickerViewType)),
			new UserConfigurationPropertyDefinition("AttachmentsFilePickerViewTypeForTouch", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateAttachmentsFilePickerViewType)),
			new UserConfigurationPropertyDefinition("BookmarkedWeatherLocations", typeof(string[])),
			new UserConfigurationPropertyDefinition("CurrentWeatherLocationBookmarkIndex", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateCurrentWeatherLocationBookmarkIndex)),
			new UserConfigurationPropertyDefinition("TemperatureUnit", typeof(TemperatureUnit), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateTemperatureUnit)),
			new UserConfigurationPropertyDefinition("GlobalFolderViewState", typeof(string)),
			new UserConfigurationPropertyDefinition("CalendarAgendaViewIsExpandedMouse", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateCalendarAgendaViewIsExpandedMouse)),
			new UserConfigurationPropertyDefinition("CalendarAgendaViewIsExpandedTWide", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateCalendarAgendaViewIsExpandedTWide)),
			new UserConfigurationPropertyDefinition("AttachmentsFilePickerHideBanner", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateAttachmentsFilePickerHideBanner))
		};

		private static ViewStatePropertySchema instance;
	}
}
