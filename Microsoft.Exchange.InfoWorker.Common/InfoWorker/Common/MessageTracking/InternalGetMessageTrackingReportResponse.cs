using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class InternalGetMessageTrackingReportResponse
	{
		internal Microsoft.Exchange.SoapWebClient.EWS.GetMessageTrackingReportResponseMessageType Response { get; private set; }

		internal List<RecipientTrackingEvent> RecipientTrackingEvents { get; private set; }

		internal static InternalGetMessageTrackingReportResponse Create(string domain, Microsoft.Exchange.SoapWebClient.EWS.GetMessageTrackingReportResponseMessageType response)
		{
			if (!InternalGetMessageTrackingReportResponse.CheckValidAndFixupIfNeeded(response))
			{
				return null;
			}
			return new InternalGetMessageTrackingReportResponse(domain, response);
		}

		internal static InternalGetMessageTrackingReportResponse Create(string domain, Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.GetMessageTrackingReportResponseMessageType response)
		{
			if (!InternalGetMessageTrackingReportResponse.CheckValidAndFixupIfNeeded(response))
			{
				return null;
			}
			return new InternalGetMessageTrackingReportResponse(domain, response);
		}

		private static bool CheckValidAndFixupIfNeeded(Microsoft.Exchange.SoapWebClient.EWS.GetMessageTrackingReportResponseMessageType response)
		{
			if (response == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError(0, "Empty/Invalid response for GetMessageTrackingReport", new object[0]);
				return false;
			}
			if (response.MessageTrackingReport == null)
			{
				if (response.ResponseClass == Microsoft.Exchange.SoapWebClient.EWS.ResponseClassType.Success && response.Errors == null && response.Errors.Length == 0)
				{
					TraceWrapper.SearchLibraryTracer.TraceError(0, "Empty/Invalid response for GetMessageTrackingReport is only permitted if there were errors", new object[0]);
					return false;
				}
				TraceWrapper.SearchLibraryTracer.TraceError(0, "Fixing up error response by inserting empty MessageTrackingReportType", new object[0]);
				response.MessageTrackingReport = new Microsoft.Exchange.SoapWebClient.EWS.MessageTrackingReportType();
			}
			return true;
		}

		private static bool CheckValidAndFixupIfNeeded(Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.GetMessageTrackingReportResponseMessageType response)
		{
			if (response == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError(0, "Empty/Invalid response for Proxy.GetMessageTrackingReport", new object[0]);
				return false;
			}
			if (response.MessageTrackingReport == null)
			{
				if (response.ResponseClass == Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.ResponseClassType.Success && response.Errors == null && response.Errors.Length == 0)
				{
					TraceWrapper.SearchLibraryTracer.TraceError(0, "Empty/Invalid response for Proxy.GetMessageTrackingReport is only permitted if there were errors", new object[0]);
					return false;
				}
				TraceWrapper.SearchLibraryTracer.TraceError(0, "Fixing up error response by inserting empty Proxy.MessageTrackingReportType", new object[0]);
				response.MessageTrackingReport = new Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.MessageTrackingReportType();
			}
			return true;
		}

		private InternalGetMessageTrackingReportResponse()
		{
		}

		private InternalGetMessageTrackingReportResponse(string domain, Microsoft.Exchange.SoapWebClient.EWS.GetMessageTrackingReportResponseMessageType response)
		{
			this.Response = response;
			this.RecipientTrackingEvents = InternalGetMessageTrackingReportResponse.CreateEventList<Microsoft.Exchange.SoapWebClient.EWS.RecipientTrackingEventType>(domain, response.MessageTrackingReport.RecipientTrackingEvents, InternalGetMessageTrackingReportResponse.ewsConversionDelegate);
			response.MessageTrackingReport.RecipientTrackingEvents = null;
		}

		private InternalGetMessageTrackingReportResponse(string domain, Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.GetMessageTrackingReportResponseMessageType dispatcherResponse)
		{
			this.Response = new Microsoft.Exchange.SoapWebClient.EWS.GetMessageTrackingReportResponseMessageType();
			this.Response.Diagnostics = dispatcherResponse.Diagnostics;
			Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.MessageTrackingReportType messageTrackingReport = dispatcherResponse.MessageTrackingReport;
			Microsoft.Exchange.SoapWebClient.EWS.MessageTrackingReportType messageTrackingReportType = new Microsoft.Exchange.SoapWebClient.EWS.MessageTrackingReportType();
			messageTrackingReportType.OriginalRecipients = MessageConverter.CopyEmailAddressArray(messageTrackingReport.OriginalRecipients);
			messageTrackingReportType.PurportedSender = MessageConverter.CopyEmailAddress(messageTrackingReport.PurportedSender);
			messageTrackingReportType.Sender = MessageConverter.CopyEmailAddress(messageTrackingReport.Sender);
			messageTrackingReportType.Subject = messageTrackingReport.Subject;
			messageTrackingReportType.SubmitTime = messageTrackingReport.SubmitTime;
			messageTrackingReportType.SubmitTimeSpecified = messageTrackingReport.SubmitTimeSpecified;
			messageTrackingReportType.Properties = MessageConverter.CopyTrackingProperties(dispatcherResponse.Properties);
			this.Response.MessageTrackingReport = messageTrackingReportType;
			this.Response.Properties = MessageConverter.CopyTrackingProperties(dispatcherResponse.Properties);
			this.Response.Errors = MessageConverter.CopyErrors(dispatcherResponse.Errors);
			Microsoft.Exchange.SoapWebClient.EWS.ResponseCodeType responseCode;
			if (EnumValidator<Microsoft.Exchange.SoapWebClient.EWS.ResponseCodeType>.TryParse(dispatcherResponse.ResponseCode, EnumParseOptions.Default, out responseCode))
			{
				this.Response.ResponseCode = responseCode;
			}
			else
			{
				TraceWrapper.SearchLibraryTracer.TraceError<string>(0, "{0} cannot be converted to a valid ResponseCodeType, mapping to ErrorMessageTrackingPermanentError", dispatcherResponse.ResponseCode);
				this.Response.ResponseCode = Microsoft.Exchange.SoapWebClient.EWS.ResponseCodeType.ErrorMessageTrackingPermanentError;
			}
			this.Response.ResponseClass = EnumConverter<Microsoft.Exchange.SoapWebClient.EWS.ResponseClassType, Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.ResponseClassType>.Convert(dispatcherResponse.ResponseClass);
			this.Response.MessageText = dispatcherResponse.MessageText;
			messageTrackingReportType.RecipientTrackingEvents = null;
			this.RecipientTrackingEvents = InternalGetMessageTrackingReportResponse.CreateEventList<Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.RecipientTrackingEventType>(domain, messageTrackingReport.RecipientTrackingEvents, InternalGetMessageTrackingReportResponse.dispatcherConversionDelegate);
		}

		private static List<RecipientTrackingEvent> CreateEventList<T>(string domain, T[] wsRecipientTrackingEvents, InternalGetMessageTrackingReportResponse.RecipientEventConversionDelegate<T> conversionMethod)
		{
			if (wsRecipientTrackingEvents == null)
			{
				return new List<RecipientTrackingEvent>(0);
			}
			List<RecipientTrackingEvent> list = new List<RecipientTrackingEvent>(wsRecipientTrackingEvents.Length);
			for (int i = 0; i < wsRecipientTrackingEvents.Length; i++)
			{
				RecipientTrackingEvent recipientTrackingEvent = conversionMethod(domain, wsRecipientTrackingEvents[i]);
				if (recipientTrackingEvent != null)
				{
					list.Add(recipientTrackingEvent);
				}
			}
			return list;
		}

		private static InternalGetMessageTrackingReportResponse.RecipientEventConversionDelegate<Microsoft.Exchange.SoapWebClient.EWS.RecipientTrackingEventType> ewsConversionDelegate = new InternalGetMessageTrackingReportResponse.RecipientEventConversionDelegate<Microsoft.Exchange.SoapWebClient.EWS.RecipientTrackingEventType>(RecipientTrackingEvent.Create);

		private static InternalGetMessageTrackingReportResponse.RecipientEventConversionDelegate<Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.RecipientTrackingEventType> dispatcherConversionDelegate = new InternalGetMessageTrackingReportResponse.RecipientEventConversionDelegate<Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.RecipientTrackingEventType>(RecipientTrackingEvent.Create);

		public delegate RecipientTrackingEvent RecipientEventConversionDelegate<T>(string domain, T wsEvent);
	}
}
