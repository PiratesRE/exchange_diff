using System;
using System.IO;
using System.Net;
using System.Security;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.CalendarDiagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CalendarRemoteItem : CalendarInstance
	{
		internal CalendarRemoteItem(ExchangePrincipal remotePrincipal, MeetingValidatorEwsBinding binding) : base(remotePrincipal)
		{
			this.calendarConverter = new CalendarItemConverter();
			this.binding = binding;
		}

		internal override CalendarProcessingFlags? GetCalendarConfig()
		{
			CalendarProcessingFlags value = (CalendarProcessingFlags)CalendarConfigurationSchema.AutomateProcessing.DefaultValue;
			GetUserConfigurationType getUserConfigurationType = new GetUserConfigurationType();
			getUserConfigurationType.UserConfigurationProperties = UserConfigurationPropertyType.Dictionary;
			getUserConfigurationType.UserConfigurationName = new UserConfigurationNameType
			{
				Name = "Calendar",
				Item = new DistinguishedFolderIdType
				{
					Id = DistinguishedFolderIdNameType.calendar
				}
			};
			try
			{
				GetUserConfigurationResponseType userConfiguration = this.binding.GetUserConfiguration(getUserConfigurationType);
				bool flag;
				UserConfigurationType userConfigurationType = this.HandleGetUserConfigurationResponse(userConfiguration, out flag);
				if (flag)
				{
					return new CalendarProcessingFlags?(value);
				}
				if (userConfigurationType == null || userConfigurationType.Dictionary == null)
				{
					Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "GetUserConfiguration returned NULL userConfigurationType.");
					return null;
				}
				foreach (UserConfigurationDictionaryEntryType userConfigurationDictionaryEntryType in userConfigurationType.Dictionary)
				{
					if (userConfigurationDictionaryEntryType.DictionaryKey != null && userConfigurationDictionaryEntryType.DictionaryKey.Value != null && userConfigurationDictionaryEntryType.DictionaryKey.Value.Length > 0 && string.Compare(userConfigurationDictionaryEntryType.DictionaryKey.Value[0], CalendarConfigurationSchema.AutomateProcessing.Name, StringComparison.OrdinalIgnoreCase) == 0 && userConfigurationDictionaryEntryType.DictionaryValue != null && userConfigurationDictionaryEntryType.DictionaryValue.Value != null && userConfigurationDictionaryEntryType.DictionaryValue.Value.Length > 0)
					{
						CalendarProcessingFlags value2 = (CalendarProcessingFlags)Enum.Parse(typeof(CalendarProcessingFlags), userConfigurationDictionaryEntryType.DictionaryValue.Value[0]);
						return new CalendarProcessingFlags?(value2);
					}
				}
			}
			catch (ProtocolViolationException exception)
			{
				this.HandleRemoteException(exception);
			}
			catch (SecurityException exception2)
			{
				this.HandleRemoteException(exception2);
			}
			catch (ArgumentException exception3)
			{
				this.HandleRemoteException(exception3);
			}
			catch (InvalidOperationException exception4)
			{
				this.HandleRemoteException(exception4);
			}
			catch (NotSupportedException exception5)
			{
				this.HandleRemoteException(exception5);
			}
			catch (XmlException exception6)
			{
				this.HandleRemoteException(exception6);
			}
			catch (XPathException exception7)
			{
				this.HandleRemoteException(exception7);
			}
			catch (SoapException exception8)
			{
				this.HandleRemoteException(exception8);
			}
			catch (IOException exception9)
			{
				this.HandleRemoteException(exception9);
			}
			return new CalendarProcessingFlags?(value);
		}

		internal override Inconsistency GetInconsistency(CalendarValidationContext context, string fullDescription)
		{
			Inconsistency inconsistency = null;
			if (base.LoadInconsistency != null)
			{
				return base.LoadInconsistency;
			}
			bool isCleanGlobalObjectId = context.BaseItem.GlobalObjectId.IsCleanGlobalObjectId;
			try
			{
				ClientIntentFlags? missingItemIntent = this.GetMissingItemIntent(context, isCleanGlobalObjectId);
				if (context.OppositeRole == RoleType.Attendee)
				{
					inconsistency = (isCleanGlobalObjectId ? this.GetAttendeeMissingItemInconsistency(context, fullDescription, missingItemIntent, ClientIntentFlags.RespondedDecline, ClientIntentFlags.DeletedWithNoResponse) : this.GetAttendeeMissingItemInconsistency(context, fullDescription, missingItemIntent, ClientIntentFlags.RespondedExceptionDecline, ClientIntentFlags.DeletedExceptionWithNoResponse));
				}
				else
				{
					inconsistency = MissingItemInconsistency.CreateOrganizerMissingItemInstance(fullDescription, context);
					if (RumFactory.Instance.Policy.RepairMode == CalendarRepairType.ValidateOnly)
					{
						inconsistency.Intent = missingItemIntent;
					}
				}
			}
			catch (ProtocolViolationException exception)
			{
				this.HandleRemoteException(exception);
			}
			catch (SecurityException exception2)
			{
				this.HandleRemoteException(exception2);
			}
			catch (ArgumentException exception3)
			{
				this.HandleRemoteException(exception3);
			}
			catch (InvalidOperationException exception4)
			{
				this.HandleRemoteException(exception4);
			}
			catch (NotSupportedException exception5)
			{
				this.HandleRemoteException(exception5);
			}
			catch (XmlException exception6)
			{
				this.HandleRemoteException(exception6);
			}
			catch (XPathException exception7)
			{
				this.HandleRemoteException(exception7);
			}
			catch (SoapException exception8)
			{
				this.HandleRemoteException(exception8);
			}
			catch (IOException exception9)
			{
				this.HandleRemoteException(exception9);
			}
			return inconsistency;
		}

		internal override bool WouldTryToRepairIfMissing(CalendarValidationContext context, out MeetingInquiryAction predictedAction)
		{
			predictedAction = MeetingInquiryAction.DeletedVersionNotFound;
			if (this.calendarIntent != null)
			{
				predictedAction = this.calendarConverter.ConvertToMeetingInquiryAction(this.calendarIntent.PredictedAction);
				return this.calendarIntent.WouldRepair;
			}
			return false;
		}

		internal override ClientIntentFlags? GetLocationIntent(CalendarValidationContext context, GlobalObjectId globalObjectId, string organizerLocation, string attendeeLocation)
		{
			GetClientIntentType getClientIntentType = new GetClientIntentType();
			getClientIntentType.GlobalObjectId = Convert.ToBase64String(globalObjectId.Bytes);
			getClientIntentType.StateDefinition = new NonEmptyStateDefinitionType
			{
				Item = new LocationBasedStateDefinitionType
				{
					OrganizerLocation = organizerLocation,
					AttendeeLocation = attendeeLocation
				}
			};
			GetClientIntentResponseMessageType clientIntent = this.binding.GetClientIntent(getClientIntentType);
			this.calendarIntent = this.HandleGetClientIntentResponse(clientIntent);
			if (this.calendarIntent == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "GetClientIntent returned NULL.");
				return null;
			}
			return new ClientIntentFlags?((ClientIntentFlags)this.calendarIntent.Intent);
		}

		private ClientIntentFlags? GetMissingItemIntent(CalendarValidationContext context, bool isNotOccurrence)
		{
			GetClientIntentType getClientIntentType = new GetClientIntentType();
			getClientIntentType.GlobalObjectId = Convert.ToBase64String(context.BaseItem.GlobalObjectId.Bytes);
			if (isNotOccurrence)
			{
				getClientIntentType.StateDefinition = new NonEmptyStateDefinitionType
				{
					Item = new DeleteFromFolderStateDefinitionType()
				};
			}
			else
			{
				getClientIntentType.StateDefinition = new NonEmptyStateDefinitionType
				{
					Item = new DeletedOccurrenceStateDefinitionType
					{
						OccurrenceDate = context.BaseItem.GlobalObjectId.Date.UniversalTime,
						IsOccurrencePresent = false
					}
				};
			}
			GetClientIntentResponseMessageType clientIntent = this.binding.GetClientIntent(getClientIntentType);
			this.calendarIntent = this.HandleGetClientIntentResponse(clientIntent);
			if (this.calendarIntent == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "GetClientIntent returned NULL.");
				return null;
			}
			return new ClientIntentFlags?((ClientIntentFlags)this.calendarIntent.Intent);
		}

		private Inconsistency GetAttendeeMissingItemInconsistency(CalendarValidationContext context, string fullDescription, ClientIntentFlags? inconsistencyIntent, ClientIntentFlags declineIntent, ClientIntentFlags deleteIntent)
		{
			Inconsistency inconsistency = null;
			if (ClientIntentQuery.CheckDesiredClientIntent(inconsistencyIntent, new ClientIntentFlags[]
			{
				declineIntent
			}))
			{
				inconsistency = ResponseInconsistency.CreateInstance(fullDescription, ResponseType.Decline, context.Attendee.ResponseType, ExDateTime.MinValue, context.Attendee.ReplyTime, context);
				inconsistency.Intent = inconsistencyIntent;
			}
			else if (ClientIntentQuery.CheckDesiredClientIntent(inconsistencyIntent, new ClientIntentFlags[]
			{
				deleteIntent
			}))
			{
				inconsistency = null;
			}
			else if (this.calendarIntent != null && this.calendarIntent.ItemVersion > 0)
			{
				inconsistency = MissingItemInconsistency.CreateAttendeeMissingItemInstance(fullDescription, inconsistencyIntent, new int?(this.calendarIntent.ItemVersion), context);
				inconsistency.Intent = inconsistencyIntent;
			}
			return inconsistency;
		}

		private ResponseMessageType HandleBaseResponseMessage(BaseResponseMessageType response)
		{
			if (response.ResponseMessages == null || response.ResponseMessages.Items == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "Web request returned NULL ResponseMessages.");
				return null;
			}
			ResponseMessageType responseMessageType = response.ResponseMessages.Items[0];
			if (responseMessageType == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "Web request returned NULL ResponseMessageType.");
				return null;
			}
			return responseMessageType;
		}

		private UserConfigurationType HandleGetUserConfigurationResponse(BaseResponseMessageType response, out bool itemNotFound)
		{
			itemNotFound = false;
			ResponseMessageType responseMessageType = this.HandleBaseResponseMessage(response);
			if (responseMessageType == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "GetUserConfiguration returned NULL responseType.");
				return null;
			}
			if (responseMessageType.ResponseCode == ResponseCodeType.ErrorItemNotFound)
			{
				itemNotFound = true;
				return null;
			}
			if (responseMessageType.ResponseCode != ResponseCodeType.NoError)
			{
				Globals.ConsistencyChecksTracer.TraceDebug<ResponseCodeType>((long)this.GetHashCode(), "Web request returned ResponseCodeType {0}.", responseMessageType.ResponseCode);
				return null;
			}
			GetUserConfigurationResponseMessageType getUserConfigurationResponseMessageType = responseMessageType as GetUserConfigurationResponseMessageType;
			if (getUserConfigurationResponseMessageType == null || getUserConfigurationResponseMessageType.UserConfiguration == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "GetUserConfiguration web request returned NULL UserConfiguration.");
				return null;
			}
			return getUserConfigurationResponseMessageType.UserConfiguration;
		}

		private ClientIntentType HandleGetClientIntentResponse(ResponseMessageType responseType)
		{
			if (responseType == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "GetClientIntent returned NULL responseType.");
				return null;
			}
			if (responseType.ResponseCode != ResponseCodeType.NoError)
			{
				Globals.ConsistencyChecksTracer.TraceDebug<ResponseCodeType>((long)this.GetHashCode(), "Web request returned ResponseCodeType {0}.", responseType.ResponseCode);
				return null;
			}
			GetClientIntentResponseMessageType getClientIntentResponseMessageType = responseType as GetClientIntentResponseMessageType;
			if (getClientIntentResponseMessageType == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "GetClientIntent web request returned NULL GetClientIntentResponseMessageType.");
				return null;
			}
			return getClientIntentResponseMessageType.ClientIntent;
		}

		private void HandleRemoteException(Exception exception)
		{
			Globals.ConsistencyChecksTracer.TraceError<Exception, SmtpAddress>((long)this.GetHashCode(), "{0}: Could not access remote server to open mailbox {1}.", exception, base.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress);
			Globals.CalendarRepairLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_ErrorAccessingRemoteMailbox, base.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), new object[]
			{
				base.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(),
				exception
			});
		}

		private MeetingValidatorEwsBinding binding;

		private CalendarItemConverter calendarConverter;

		private ClientIntentType calendarIntent;
	}
}
