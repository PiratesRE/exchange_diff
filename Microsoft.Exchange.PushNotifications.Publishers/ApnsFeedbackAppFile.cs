using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsFeedbackAppFile : ApnsFeedbackFileBase
	{
		internal ApnsFeedbackAppFile(ApnsFeedbackFileId identifier, ApnsFeedbackFileIO fileIO) : this(identifier, fileIO, ExTraceGlobals.PublisherManagerTracer)
		{
		}

		internal ApnsFeedbackAppFile(ApnsFeedbackFileId identifier, ApnsFeedbackFileIO fileIO, ITracer tracer) : base(identifier, fileIO, tracer)
		{
		}

		public override bool IsLoaded
		{
			get
			{
				return this.Feedback != null;
			}
		}

		private Dictionary<string, ApnsFeedbackResponse> Feedback { get; set; }

		public override ApnsFeedbackValidationResult ValidateNotification(ApnsNotification notification)
		{
			ArgumentValidator.ThrowIfNull("notification", notification);
			if (!this.IsLoaded)
			{
				base.Tracer.TraceDebug<ApnsNotification, ApnsFeedbackFileId>((long)this.GetHashCode(), "[ValidateNotification] Feedback app file not loaded, defaulting to Uncertain for {0}, {1}.", notification, base.Identifier);
				return ApnsFeedbackValidationResult.Uncertain;
			}
			ApnsFeedbackResponse apnsFeedbackResponse;
			if (this.Feedback.TryGetValue(notification.DeviceToken, out apnsFeedbackResponse))
			{
				if (apnsFeedbackResponse.TimeStamp > (ExDateTime)notification.LastSubscriptionUpdate)
				{
					return ApnsFeedbackValidationResult.Expired;
				}
				base.Tracer.TraceDebug((long)this.GetHashCode(), "[ValidateNotification] '{0}' is valid because its last subscription update '{1}' is higher than the timestamp '{3}' for feedback '{4}'", new object[]
				{
					notification,
					notification.LastSubscriptionUpdate,
					apnsFeedbackResponse.TimeStamp,
					base.Identifier
				});
			}
			else
			{
				base.Tracer.TraceDebug<ApnsNotification, ApnsFeedbackFileId>((long)this.GetHashCode(), "[ValidateNotification] '{0}' is valid because its device token is not part of the feedback '{1}'", notification, base.Identifier);
			}
			return ApnsFeedbackValidationResult.Valid;
		}

		public override void Load()
		{
			if (this.IsLoaded)
			{
				return;
			}
			Dictionary<string, ApnsFeedbackResponse> dictionary = new Dictionary<string, ApnsFeedbackResponse>();
			Exception ex = null;
			base.Tracer.TraceDebug<ApnsFeedbackFileId>((long)this.GetHashCode(), "[Load] Loading APNs Feedback Responses from '{0}'", base.Identifier);
			try
			{
				using (StreamReader fileReader = base.FileIO.GetFileReader(base.Identifier.ToString()))
				{
					string text;
					while ((text = fileReader.ReadLine()) != null)
					{
						if (!string.IsNullOrEmpty(text))
						{
							base.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[Load] Parsing APNs Feedback Response '{0}'", text);
							ApnsFeedbackResponse apnsFeedbackResponse = ApnsFeedbackResponse.FromFeedbackFileEntry(text);
							dictionary.Add(apnsFeedbackResponse.Token, apnsFeedbackResponse);
						}
						else
						{
							base.Tracer.TraceWarning((long)this.GetHashCode(), "[Load] Unexpected empty line");
						}
					}
					base.Tracer.TraceDebug((long)this.GetHashCode(), "[Load] Done loading.");
				}
				this.Feedback = dictionary;
			}
			catch (SecurityException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				throw new ApnsFeedbackException(Strings.ApnsFeedbackFileLoadError(base.Identifier.ToString(), ex.Message), ex);
			}
		}
	}
}
