using System;
using System.Collections.Generic;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class TrackingErrorCollection
	{
		internal TrackingErrorCollection()
		{
		}

		internal static TrackingErrorCollection Empty
		{
			get
			{
				return TrackingErrorCollection.empty;
			}
		}

		internal List<TrackingError> Errors
		{
			get
			{
				return this.errors;
			}
		}

		internal static bool IsNullOrEmpty(TrackingErrorCollection trackingErrorCollection)
		{
			return trackingErrorCollection == null || trackingErrorCollection == TrackingErrorCollection.Empty;
		}

		internal TrackingError Add(ErrorCode errorCode, string target, string data, string exception)
		{
			TrackingError trackingError = new TrackingError(errorCode, target, data, exception);
			this.errors.Add(trackingError);
			return trackingError;
		}

		internal void Add(TrackingErrorCollection errors)
		{
			if (!TrackingErrorCollection.IsNullOrEmpty(errors))
			{
				this.errors.AddRange(errors.errors);
			}
		}

		internal void ResetAllErrors()
		{
			this.Errors.Clear();
		}

		internal void ReadErrorsFromWSMessage(string[] diagnostics, ArrayOfTrackingPropertiesType[] errors)
		{
			if (this.TryReadErrorsFromWSMessage_Exchange2010(diagnostics))
			{
				return;
			}
			this.ReadErrorsFromWSMessage_Exchange2010SP1(errors);
		}

		private void ReadErrorsFromWSMessage_Exchange2010SP1(ArrayOfTrackingPropertiesType[] propertyBags)
		{
			if (propertyBags != null)
			{
				foreach (ArrayOfTrackingPropertiesType propertyBag in propertyBags)
				{
					TrackingError trackingError = TrackingError.CreateFromWSMessage(propertyBag);
					if (trackingError != null)
					{
						this.errors.Add(trackingError);
					}
				}
			}
		}

		private bool TryReadErrorsFromWSMessage_Exchange2010(string[] diagnostics)
		{
			if (diagnostics == null || diagnostics.Length < 1)
			{
				return false;
			}
			string text = diagnostics[0];
			int num = text.IndexOf("WebServiceError:", StringComparison.Ordinal);
			int num2 = num + "WebServiceError:".Length;
			if (num == -1)
			{
				return false;
			}
			if (num != -1 && num2 < text.Length)
			{
				char c = text[num2];
				if (c != 'C')
				{
					if (c == 'F')
					{
						this.errors.Add(new TrackingError(ErrorCode.GeneralFatalFailure, string.Empty, string.Empty, string.Empty));
						return true;
					}
					switch (c)
					{
					case 'R':
					case 'T':
						break;
					case 'S':
						return true;
					default:
						return true;
					}
				}
				this.errors.Add(new TrackingError(ErrorCode.GeneralTransientFailure, string.Empty, string.Empty, string.Empty));
			}
			return true;
		}

		internal const string ErrorLabel = "WebServiceError:";

		internal const char TransientErrorValue = 'T';

		internal const char FatalErrorValue = 'F';

		internal const char ConnectionErrorValue = 'C';

		internal const char ReadStatusErrorValue = 'R';

		private static TrackingErrorCollection empty = new TrackingErrorCollection();

		private List<TrackingError> errors = new List<TrackingError>(5);
	}
}
