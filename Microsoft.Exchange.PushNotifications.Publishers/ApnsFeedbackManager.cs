using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsFeedbackManager : IApnsFeedbackProvider
	{
		public ApnsFeedbackManager(ApnsFeedbackManagerSettings settings = null) : this(settings ?? new ApnsFeedbackManagerSettings(), ApnsFeedbackScheduler.DefaultScheduler, ApnsFeedbackFileIO.DefaultFileIO, ExTraceGlobals.PublisherManagerTracer)
		{
		}

		protected ApnsFeedbackManager(ApnsFeedbackManagerSettings settings, ApnsFeedbackScheduler scheduler, ApnsFeedbackFileIO fileIO, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("settings", settings);
			ArgumentValidator.ThrowIfNegative("settings.UpdateIntervalInMilliseconds", settings.UpdateIntervalInMilliseconds);
			ArgumentValidator.ThrowIfNull("scheduler", scheduler);
			ArgumentValidator.ThrowIfNull("fileIO", fileIO);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.Settings = settings;
			this.Scheduler = scheduler;
			this.FileIO = fileIO;
			this.Tracer = tracer;
			this.CurrentFeedback = new Dictionary<ApnsFeedbackFileId, IApnsFeedbackFile>();
			this.ExpirationThreshold = TimeSpan.FromMilliseconds((double)this.Settings.ExpirationThresholdInMilliseconds);
			this.Scheduler.ScheduleOnce(new Action(this.UpdateFeedback), 10);
		}

		private protected Dictionary<ApnsFeedbackFileId, IApnsFeedbackFile> CurrentFeedback { protected get; private set; }

		private ApnsFeedbackManagerSettings Settings { get; set; }

		private TimeSpan ExpirationThreshold { get; set; }

		private ApnsFeedbackScheduler Scheduler { get; set; }

		private ApnsFeedbackFileIO FileIO { get; set; }

		private ITracer Tracer { get; set; }

		public ApnsFeedbackValidationResult ValidateNotification(ApnsNotification notification)
		{
			ArgumentValidator.ThrowIfNull("notification", notification);
			Dictionary<ApnsFeedbackFileId, IApnsFeedbackFile> currentFeedback = this.CurrentFeedback;
			if (currentFeedback.Count == 0)
			{
				this.Tracer.TraceDebug<ApnsNotification>((long)this.GetHashCode(), "[ValidateNotification] Feedback is not available, defaulting to Uncertain for '{0}'", notification);
				return ApnsFeedbackValidationResult.Uncertain;
			}
			ApnsFeedbackValidationResult apnsFeedbackValidationResult = ApnsFeedbackValidationResult.Valid;
			foreach (IApnsFeedbackFile apnsFeedbackFile in currentFeedback.Values)
			{
				if (!apnsFeedbackFile.HasExpired(this.ExpirationThreshold))
				{
					ApnsFeedbackValidationResult apnsFeedbackValidationResult2 = apnsFeedbackFile.ValidateNotification(notification);
					if (apnsFeedbackValidationResult2 == ApnsFeedbackValidationResult.Expired)
					{
						return apnsFeedbackValidationResult2;
					}
					if (apnsFeedbackValidationResult != apnsFeedbackValidationResult2 && apnsFeedbackValidationResult == ApnsFeedbackValidationResult.Valid)
					{
						apnsFeedbackValidationResult = ApnsFeedbackValidationResult.Uncertain;
					}
				}
			}
			return apnsFeedbackValidationResult;
		}

		protected void UpdateFeedback()
		{
			try
			{
				this.Tracer.TraceDebug((long)this.GetHashCode(), "[UpdateFeedback] Updating feedback");
				PushNotificationsCrimsonEvents.ApnsFeedbackManagerUpdating.Log<string>(string.Empty);
				if (!this.FileIO.Exists(ApnsFeedbackManager.FeedbackPath))
				{
					this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[UpdateFeedback] Feedback folder not found: '{0}'.", ApnsFeedbackManager.FeedbackPath);
					this.CurrentFeedback = new Dictionary<ApnsFeedbackFileId, IApnsFeedbackFile>();
				}
				else
				{
					this.CurrentFeedback = this.BuildFeedbackFromFilesystem();
				}
			}
			catch (Exception ex)
			{
				string text = ex.ToTraceString();
				this.Tracer.TraceError<string>((long)this.GetHashCode(), "[UpdateFeedback] An error occurred trying to update the feedback: {0}", text);
				PushNotificationsCrimsonEvents.ApnsFeedbackManagerUpdateError.Log<string>(text);
				this.CurrentFeedback = this.ExpireInMemoryFeedback();
				if (!(ex is ApnsFeedbackException))
				{
					throw;
				}
			}
			finally
			{
				ExDateTime arg = ExDateTime.UtcNow.AddMilliseconds((double)this.Settings.UpdateIntervalInMilliseconds);
				this.Tracer.TraceDebug<ExDateTime>((long)this.GetHashCode(), "[UpdateFeedback] Scheduling next feedback update for '{0}'.", arg);
				PushNotificationsCrimsonEvents.ApnsFeedbackManagerUpdateDone.Log<string>(arg.ToString());
				this.Scheduler.ScheduleOnce(new Action(this.UpdateFeedback), this.Settings.UpdateIntervalInMilliseconds);
			}
		}

		private Dictionary<ApnsFeedbackFileId, IApnsFeedbackFile> BuildFeedbackFromFilesystem()
		{
			Dictionary<ApnsFeedbackFileId, IApnsFeedbackFile> dictionary = new Dictionary<ApnsFeedbackFileId, IApnsFeedbackFile>();
			this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[UpdateFeedback] Searching for all feedback packages under: '{0}'.", ApnsFeedbackManager.FeedbackPath);
			List<IApnsFeedbackFile> list = ApnsFeedbackPackage.FindAll(ApnsFeedbackManager.FeedbackPath, this.FileIO, this.Tracer);
			foreach (IApnsFeedbackFile apnsFeedbackFile in list)
			{
				try
				{
					if (apnsFeedbackFile.HasExpired(this.ExpirationThreshold))
					{
						this.Tracer.TraceDebug<ApnsFeedbackFileId>((long)this.GetHashCode(), "[UpdateFeedback] Removing package: '{0}'.", apnsFeedbackFile.Identifier);
						apnsFeedbackFile.Remove();
					}
					else
					{
						IApnsFeedbackFile apnsFeedbackFile2;
						if (!this.CurrentFeedback.TryGetValue(apnsFeedbackFile.Identifier, out apnsFeedbackFile2))
						{
							apnsFeedbackFile2 = apnsFeedbackFile;
							apnsFeedbackFile2.Load();
						}
						this.Tracer.TraceDebug<ApnsFeedbackFileId>((long)this.GetHashCode(), "[UpdateFeedback] Adding package: '{0}'.", apnsFeedbackFile2.Identifier);
						dictionary.Add(apnsFeedbackFile2.Identifier, apnsFeedbackFile2);
					}
				}
				catch (ApnsFeedbackException exception)
				{
					string text = exception.ToTraceString();
					this.Tracer.TraceError<ApnsFeedbackFileId, string>((long)this.GetHashCode(), "[UpdateFeedback] An error occurred trying to update the feedback for '{0}': {1}", apnsFeedbackFile.Identifier, text);
					PushNotificationsCrimsonEvents.ApnsFeedbackManagerPackageError.Log<ApnsFeedbackFileId, string>(apnsFeedbackFile.Identifier, text);
				}
			}
			return dictionary;
		}

		private Dictionary<ApnsFeedbackFileId, IApnsFeedbackFile> ExpireInMemoryFeedback()
		{
			this.Tracer.TraceDebug((long)this.GetHashCode(), "[UpdateFeedback] Removing all expired packages from the in-memory collection.");
			Dictionary<ApnsFeedbackFileId, IApnsFeedbackFile> dictionary = new Dictionary<ApnsFeedbackFileId, IApnsFeedbackFile>();
			foreach (IApnsFeedbackFile apnsFeedbackFile in this.CurrentFeedback.Values)
			{
				if (apnsFeedbackFile.HasExpired(this.ExpirationThreshold))
				{
					this.Tracer.TraceDebug<ApnsFeedbackFileId>((long)this.GetHashCode(), "[UpdateFeedback] Removing expired package '{0}'.", apnsFeedbackFile.Identifier);
				}
				else
				{
					this.Tracer.TraceDebug<ApnsFeedbackFileId>((long)this.GetHashCode(), "[UpdateFeedback] Keeping current package '{0}'.", apnsFeedbackFile.Identifier);
					dictionary.Add(apnsFeedbackFile.Identifier, apnsFeedbackFile);
				}
			}
			return dictionary;
		}

		private const string FeedbackRelativePath = "ClientAccess\\PushNotifications\\Feedback";

		internal static readonly string FeedbackPath = Path.Combine(ExchangeSetupContext.InstallPath, "ClientAccess\\PushNotifications\\Feedback");
	}
}
