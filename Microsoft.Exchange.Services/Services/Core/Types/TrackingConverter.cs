using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.Tracking;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class TrackingConverter
	{
		internal static void Convert(FindMessageTrackingReportRequest request, ExchangeVersion version, out TrackingExtendedProperties extendedProperties)
		{
			if (version == ExchangeVersion.Exchange2010)
			{
				Options options = Options.GetOptions(request.DiagnosticsLevel);
				request.DiagnosticsLevel = (options.DiagnosticsEnabled ? VersionConverter.BasicDiagnostics : null);
				request.ServerHint = options.ServerHint;
				extendedProperties = new TrackingExtendedProperties(options.ExpandTree, options.SearchAsRecip, null, false, false);
				return;
			}
			extendedProperties = TrackingConverter.CreateFromTrackingPropertyArray(request.Properties);
		}

		internal static void Convert(FindMessageTrackingReportResponseMessage response, TrackingErrorCollection errors, ExchangeVersion version)
		{
			if (version == ExchangeVersion.Exchange2010)
			{
				response.ExecutedSearchScope = null;
				TrackingConverter.SetErrorsOnWSMessage_Exchange2010(errors, ref response.Diagnostics);
				response.Errors = null;
				response.Properties = null;
				return;
			}
			TrackingConverter.SetErrorsOnWSMessage_Exchange2010SP1(errors, out response.Errors);
		}

		internal static void SetErrorsOnWSMessage_Exchange2010SP1(TrackingErrorCollection errors, out ArrayOfTrackingPropertiesType[] errorsElement)
		{
			List<TrackingError> errors2 = errors.Errors;
			if (errors2.Count == 0)
			{
				ExTraceGlobals.WebServiceTracer.TraceDebug(0L, "No errors to set on Exchange2010_SP1 response");
				errorsElement = null;
				return;
			}
			errorsElement = new ArrayOfTrackingPropertiesType[errors2.Count];
			for (int i = 0; i < errorsElement.Length; i++)
			{
				ExTraceGlobals.WebServiceTracer.TraceDebug<string>(0L, "Copying error to Exchange2010_SP1 response: {0}", errors2[i].ToString());
				errorsElement[i] = TrackingConverter.GetErrorsPropertyBag_Exchange2010SP1(errors2[i]);
			}
		}

		internal static void SetErrorsOnWSMessage_Exchange2010(TrackingErrorCollection errors, ref string[] diagnostics)
		{
			StringBuilder stringBuilder = new StringBuilder(10);
			List<TrackingError> errors2 = errors.Errors;
			foreach (TrackingError trackingError in errors2)
			{
				ExTraceGlobals.WebServiceTracer.TraceDebug<string>(0L, "Copying error to Exchange2010_RTM response: {0}", trackingError.ToString());
				stringBuilder.Append("WebServiceError:");
				ErrorCode errorCode;
				if (!EnumValidator<ErrorCode>.TryParse(trackingError.ErrorCode, EnumParseOptions.Default, out errorCode))
				{
					stringBuilder.Append('T');
					ExTraceGlobals.WebServiceTracer.TraceDebug(0L, "Unrecognized error code (maybe from downstream E14SP1+), copied TransientError on response");
				}
				else
				{
					ErrorCodeInformationAttribute errorCodeInformationAttribute = null;
					if (!EnumAttributeInfo<ErrorCode, ErrorCodeInformationAttribute>.TryGetValue((int)errorCode, out errorCodeInformationAttribute))
					{
						throw new InvalidOperationException(string.Format("{0} not annotated with ErrorCodeInformationAttribute", errorCode));
					}
					if (!errorCodeInformationAttribute.IsTransientError)
					{
						stringBuilder.Append('F');
					}
					else
					{
						stringBuilder.Append('T');
					}
				}
			}
			if (stringBuilder.Length > 0)
			{
				ExTraceGlobals.WebServiceTracer.TraceDebug<StringBuilder>(0L, "Copying errors in legacy format: {0}", stringBuilder);
				string[] array = (diagnostics == null) ? new string[1] : new string[diagnostics.Length + 1];
				array[0] = stringBuilder.ToString();
				if (diagnostics != null)
				{
					diagnostics.CopyTo(array, 1);
				}
				diagnostics = array;
			}
		}

		internal static ArrayOfTrackingPropertiesType GetErrorsPropertyBag_Exchange2010SP1(TrackingError error)
		{
			TrackingPropertyType[] array = new TrackingPropertyType[error.Properties.Count];
			int num = 0;
			foreach (KeyValuePair<string, string> keyValuePair in error.Properties)
			{
				if (!string.IsNullOrEmpty(keyValuePair.Value))
				{
					array[num] = new TrackingPropertyType
					{
						Name = keyValuePair.Key,
						Value = keyValuePair.Value
					};
					num++;
				}
			}
			return new ArrayOfTrackingPropertiesType
			{
				Items = array
			};
		}

		internal static void Convert(FindMessageTrackingSearchResultType searchResult, ExchangeVersion version)
		{
			if (version == ExchangeVersion.Exchange2010)
			{
				searchResult.FirstHopServer = null;
				searchResult.Properties = null;
				searchResult.PurportedSender = new EmailAddressWrapper();
				searchResult.PurportedSender.EmailAddress = searchResult.Sender.EmailAddress;
			}
		}

		internal static void Convert(GetMessageTrackingReportRequest request, ExchangeVersion version, out TrackingExtendedProperties extendedProperties)
		{
			if (version == ExchangeVersion.Exchange2010)
			{
				extendedProperties = null;
				return;
			}
			extendedProperties = TrackingConverter.CreateFromTrackingPropertyArray(request.Properties);
		}

		internal static void Convert(GetMessageTrackingReportResponseMessage response, TrackingErrorCollection errors, ExchangeVersion version)
		{
			if (version == ExchangeVersion.Exchange2010)
			{
				response.Properties = null;
				response.Errors = null;
				if (response.MessageTrackingReport != null)
				{
					response.MessageTrackingReport.Properties = null;
					TrackingConverter.SetErrorsOnWSMessage_Exchange2010(errors, ref response.Diagnostics);
					return;
				}
			}
			else
			{
				TrackingConverter.SetErrorsOnWSMessage_Exchange2010SP1(errors, out response.Errors);
			}
		}

		internal static RecipientTrackingEvent Convert(RecipientTrackingEvent internalEvent, ExchangeVersion version)
		{
			RecipientTrackingEvent recipientTrackingEvent;
			if (version == ExchangeVersion.Exchange2010)
			{
				if (!TrackingConverter.IsRecipientEventSupportedForRtm(internalEvent))
				{
					return null;
				}
				recipientTrackingEvent = new RecipientTrackingEvent();
				recipientTrackingEvent.RootAddress = null;
				recipientTrackingEvent.Properties = null;
				string[] eventDataToTransmitForRtm = VersionConverter.GetEventDataToTransmitForRtm(internalEvent);
				recipientTrackingEvent.EventData = ((eventDataToTransmitForRtm.Length == 0) ? null : eventDataToTransmitForRtm);
			}
			else
			{
				recipientTrackingEvent = new RecipientTrackingEvent();
				recipientTrackingEvent.RootAddress = (string.IsNullOrEmpty(internalEvent.RootAddress) ? null : internalEvent.RootAddress);
				recipientTrackingEvent.EventData = internalEvent.EventData;
				recipientTrackingEvent.Properties = TrackingConverter.ConvertToTrackingPropertyArray(internalEvent.ExtendedProperties);
			}
			recipientTrackingEvent.Date = internalEvent.Date;
			recipientTrackingEvent.DeliveryStatus = Names<DeliveryStatus>.Map[(int)internalEvent.Status];
			if (version < ExchangeVersion.Exchange2012 && internalEvent.EventDescription == EventDescription.SubmittedCrossSite)
			{
				recipientTrackingEvent.EventDescription = Names<EventDescription>.Map[11];
			}
			else
			{
				recipientTrackingEvent.EventDescription = Names<EventDescription>.Map[(int)internalEvent.EventDescription];
			}
			recipientTrackingEvent.Recipient = new EmailAddressWrapper();
			recipientTrackingEvent.Recipient.EmailAddress = internalEvent.RecipientAddress.ToString();
			recipientTrackingEvent.Recipient.Name = internalEvent.RecipientDisplayName;
			recipientTrackingEvent.Server = internalEvent.Server;
			recipientTrackingEvent.InternalId = internalEvent.InternalMessageId.ToString();
			recipientTrackingEvent.UniquePathId = (string.IsNullOrEmpty(internalEvent.UniquePathId) ? null : internalEvent.UniquePathId);
			recipientTrackingEvent.HiddenRecipient = internalEvent.HiddenRecipient;
			recipientTrackingEvent.BccRecipient = internalEvent.BccRecipient;
			return recipientTrackingEvent;
		}

		internal static RecipientTrackingEvent[] Convert(IList<RecipientTrackingEvent> internalEvents, ExchangeVersion version)
		{
			bool flag = ExchangeVersion.Exchange2010 == version;
			if (flag)
			{
				List<RecipientTrackingEvent> list = new List<RecipientTrackingEvent>(internalEvents.Count);
				for (int i = 0; i < internalEvents.Count; i++)
				{
					RecipientTrackingEvent recipientTrackingEvent = TrackingConverter.Convert(internalEvents[i], version);
					if (recipientTrackingEvent != null)
					{
						list.Add(recipientTrackingEvent);
					}
				}
				return list.ToArray();
			}
			RecipientTrackingEvent[] array = new RecipientTrackingEvent[internalEvents.Count];
			for (int j = 0; j < internalEvents.Count; j++)
			{
				RecipientTrackingEvent recipientTrackingEvent2 = TrackingConverter.Convert(internalEvents[j], version);
				if (recipientTrackingEvent2 == null)
				{
					throw new InvalidOperationException("recipEvent should never be null here");
				}
				array[j] = recipientTrackingEvent2;
			}
			return array;
		}

		private static bool IsRecipientEventSupportedForRtm(RecipientTrackingEvent recipEvent)
		{
			return TrackingConverter.RtmSupportedDeliveryStatus.Contains(recipEvent.Status) && TrackingConverter.RtmSupportedEventDescription.Contains(recipEvent.EventDescription);
		}

		private static TrackingExtendedProperties CreateFromTrackingPropertyArray(TrackingPropertyType[] properties)
		{
			bool expandTree = false;
			bool searchAsRecip = false;
			TimeSpan? timeout = null;
			bool getAdditionalRecords = false;
			bool searchForModerationResult = false;
			string empty = string.Empty;
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			if (properties != null)
			{
				foreach (TrackingPropertyType trackingPropertyType in properties)
				{
					if ("ExpandTree".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
					{
						expandTree = true;
					}
					else if ("SearchAsRecip".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
					{
						searchAsRecip = true;
					}
					else if ("Timeout".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
					{
						timeout = Parse.ParseFromMilliseconds(trackingPropertyType.Value);
					}
					else if ("GetAdditionalRecords".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
					{
						getAdditionalRecords = true;
					}
					else if ("SearchForModerationResult".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
					{
						searchForModerationResult = true;
					}
					else if ("MessageTrackingReportId".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
					{
						empty = string.Empty;
					}
					else if ("ArbitrationMailboxAddress".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
					{
						empty2 = string.Empty;
					}
					else if ("InitiationMessageId".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
					{
						empty3 = string.Empty;
					}
				}
			}
			return new TrackingExtendedProperties(expandTree, searchAsRecip, timeout, getAdditionalRecords, empty, empty2, empty3, searchForModerationResult);
		}

		private static TrackingPropertyType[] ConvertToTrackingPropertyArray(TrackingExtendedProperties extendedProperties)
		{
			List<TrackingPropertyType> list = new List<TrackingPropertyType>(4);
			if (extendedProperties.ExpandTree)
			{
				list.Add(new TrackingPropertyType
				{
					Name = "ExpandTree"
				});
			}
			if (extendedProperties.SearchAsRecip)
			{
				list.Add(new TrackingPropertyType
				{
					Name = "SearchAsRecip"
				});
			}
			if (extendedProperties.Timeout != null)
			{
				list.Add(new TrackingPropertyType
				{
					Name = "Timeout",
					Value = extendedProperties.Timeout.Value.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)
				});
			}
			if (extendedProperties.GetAdditionalRecords)
			{
				list.Add(new TrackingPropertyType
				{
					Name = "GetAdditionalRecords"
				});
			}
			if (extendedProperties.SearchForModerationResult)
			{
				list.Add(new TrackingPropertyType
				{
					Name = "SearchForModerationResult"
				});
			}
			if (!string.IsNullOrEmpty(extendedProperties.MessageTrackingReportId))
			{
				list.Add(new TrackingPropertyType
				{
					Name = "MessageTrackingReportId",
					Value = extendedProperties.MessageTrackingReportId
				});
			}
			if (!string.IsNullOrEmpty(extendedProperties.ArbitrationMailboxAddress))
			{
				list.Add(new TrackingPropertyType
				{
					Name = "ArbitrationMailboxAddress",
					Value = extendedProperties.ArbitrationMailboxAddress
				});
			}
			if (!string.IsNullOrEmpty(extendedProperties.InitMessageId))
			{
				list.Add(new TrackingPropertyType
				{
					Name = "InitiationMessageId",
					Value = extendedProperties.InitMessageId
				});
			}
			return list.ToArray();
		}

		private static readonly HashSet<DeliveryStatus> RtmSupportedDeliveryStatus = new HashSet<DeliveryStatus>
		{
			DeliveryStatus.Delivered,
			DeliveryStatus.Pending,
			DeliveryStatus.Read,
			DeliveryStatus.Transferred,
			DeliveryStatus.Unsuccessful
		};

		private static readonly HashSet<EventDescription> RtmSupportedEventDescription = new HashSet<EventDescription>
		{
			EventDescription.Submitted,
			EventDescription.Resolved,
			EventDescription.Expanded,
			EventDescription.Delivered,
			EventDescription.MovedToFolderByInboxRule,
			EventDescription.RulesCc,
			EventDescription.FailedGeneral,
			EventDescription.FailedModeration,
			EventDescription.FailedTransportRules,
			EventDescription.SmtpSend,
			EventDescription.SmtpSendCrossSite,
			EventDescription.SmtpSendCrossForest,
			EventDescription.SmtpReceive,
			EventDescription.Forwarded,
			EventDescription.Pending,
			EventDescription.PendingModeration,
			EventDescription.ApprovedModeration,
			EventDescription.QueueRetry,
			EventDescription.QueueRetryNoRetryTime,
			EventDescription.MessageDefer,
			EventDescription.TransferredToForeignOrg,
			EventDescription.TransferredToLegacyExchangeServer,
			EventDescription.TransferredToPartnerOrg,
			EventDescription.DelayedAfterTransferToPartnerOrg,
			EventDescription.Read,
			EventDescription.NotRead,
			EventDescription.SubmittedCrossSite
		};
	}
}
