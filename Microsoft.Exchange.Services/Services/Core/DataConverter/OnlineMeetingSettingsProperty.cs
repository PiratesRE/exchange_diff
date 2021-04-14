using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class OnlineMeetingSettingsProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, IPropertyCommand
	{
		private OnlineMeetingSettingsProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static OnlineMeetingSettingsProperty CreateCommand(CommandContext commandContext)
		{
			return new OnlineMeetingSettingsProperty(commandContext);
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			OnlineMeetingSettingsType value = null;
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
			{
				CalendarItemBase calendarItemBase = storeObject as CalendarItemBase;
				if (calendarItemBase != null && PropertyCommand.StorePropertyExists(calendarItemBase, CalendarItemBaseSchema.UCMeetingSetting))
				{
					object propertyValueFromStoreObject = PropertyCommand.GetPropertyValueFromStoreObject(calendarItemBase, CalendarItemBaseSchema.UCMeetingSetting);
					LobbyBypass lobbyBypass;
					OnlineMeetingAccessLevel accessLevel;
					Presenters presenters;
					if (propertyValueFromStoreObject != null && OnlineMeetingSettingsProperty.TryGetOnlineMeetingSettings((string)propertyValueFromStoreObject, out lobbyBypass, out accessLevel, out presenters))
					{
						value = new OnlineMeetingSettingsType
						{
							LobbyBypass = lobbyBypass,
							AccessLevel = accessLevel,
							Presenters = presenters
						};
					}
				}
			}
			serviceObject[CalendarItemSchema.OnlineMeetingSettings] = value;
		}

		public void ToXml()
		{
			throw new InvalidOperationException("OnlineMeetingSettingsProperty.ToXml should not be called.");
		}

		internal static bool TryGetOnlineMeetingSettings(string ucMeetingSettingsXml, out LobbyBypass lobbyBypass, out OnlineMeetingAccessLevel accessLevel, out Presenters presenters)
		{
			lobbyBypass = LobbyBypass.Disabled;
			accessLevel = OnlineMeetingAccessLevel.Locked;
			presenters = Presenters.Disabled;
			try
			{
				MeetingSetting meetingSetting = (MeetingSetting)OutlookAddinAdapter.Deserialize(ucMeetingSettingsXml, typeof(MeetingSetting));
				if (meetingSetting == null)
				{
					ExTraceGlobals.OnlineMeetingTracer.TraceError(0L, "[OnlineMeetingSettingsProperty.TryGetOnlineMeetingSettings] Deserialized meeting setting is null");
					return false;
				}
				return OnlineMeetingSettingsProperty.GetPresenters(meetingSetting.AutoPromote, out presenters) && OnlineMeetingSettingsProperty.GetAccessLevel(meetingSetting.Permissions, out accessLevel) && OnlineMeetingSettingsProperty.GetLobbyBypass(meetingSetting.Audio, out lobbyBypass);
			}
			catch (InvalidOperationException)
			{
				ExTraceGlobals.OnlineMeetingTracer.TraceError<string>(0L, "[OnlineMeetingSettingsProperty.TryGetOnlineMeetingSettings] UCMeetingSettings was not valid XML for determining online meeting settings {0}", ucMeetingSettingsXml);
			}
			catch (ArgumentNullException)
			{
				ExTraceGlobals.OnlineMeetingTracer.TraceError(0L, "[OnlineMeetingSettingsProperty.TryGetOnlineMeetingSettings] UCMeetingSettings was null");
			}
			return false;
		}

		internal static bool GetPresenters(AutoPromote autoPromote, out Presenters presenters)
		{
			presenters = Presenters.Disabled;
			if (autoPromote == null)
			{
				ExTraceGlobals.OnlineMeetingTracer.TraceError(0L, "[OnlineMeetingSettingsProperty.GetPresenters] Settings/AutoPromote node not found");
				return false;
			}
			switch (autoPromote.Value)
			{
			case AutoPromoteEnum.None:
				presenters = Presenters.Disabled;
				break;
			case AutoPromoteEnum.Company:
				presenters = Presenters.Internal;
				break;
			case AutoPromoteEnum.Everyone:
				presenters = Presenters.Everyone;
				break;
			default:
				ExTraceGlobals.OnlineMeetingTracer.TraceError<AutoPromoteEnum>(0L, "[OnlineMeetingSettingsProperty.GetPresenters] Invalid value for AutoPromote.Value attribute found: '{0}'", autoPromote.Value);
				return false;
			}
			return true;
		}

		internal static bool GetAccessLevel(Permissions permissions, out OnlineMeetingAccessLevel accessLevel)
		{
			accessLevel = OnlineMeetingAccessLevel.Locked;
			if (permissions == null)
			{
				ExTraceGlobals.OnlineMeetingTracer.TraceError(0L, "[OnlineMeetingSettingsProperty.GetAccessLevel] Settings/Permissions node not found");
				return false;
			}
			switch (permissions.AdmissionType)
			{
			case AdmissionType.ucLocked:
				accessLevel = OnlineMeetingAccessLevel.Locked;
				break;
			case AdmissionType.ucClosedAuthenticated:
				accessLevel = OnlineMeetingAccessLevel.Invited;
				break;
			case AdmissionType.ucOpenAuthenticated:
				accessLevel = OnlineMeetingAccessLevel.Internal;
				break;
			case AdmissionType.ucAnonymous:
				accessLevel = OnlineMeetingAccessLevel.Everyone;
				break;
			default:
				ExTraceGlobals.OnlineMeetingTracer.TraceError<AdmissionType>(0L, "[OnlineMeetingSettingsProperty.GetAccessLevel] Invalid value for meetingSetting.Permissions.AdmissionType: '{0}'", permissions.AdmissionType);
				return false;
			}
			return true;
		}

		internal static bool GetLobbyBypass(Audio audio, out LobbyBypass lobbyBypass)
		{
			lobbyBypass = LobbyBypass.Disabled;
			if (audio == null || audio.CaaAudio == null)
			{
				ExTraceGlobals.OnlineMeetingTracer.TraceError(0L, "[OnlineMeetingSettingsProperty.GetLobbyBypass] Settings/Audio/CAA/BypassLobby node not found");
				return false;
			}
			lobbyBypass = (audio.CaaAudio.BypassLobby ? LobbyBypass.EnabledForGatewayParticipants : LobbyBypass.Disabled);
			return true;
		}
	}
}
