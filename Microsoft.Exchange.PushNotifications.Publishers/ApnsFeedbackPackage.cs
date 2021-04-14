using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsFeedbackPackage : ApnsFeedbackFileBase
	{
		internal ApnsFeedbackPackage(ApnsFeedbackFileId identifier, ApnsFeedbackFileIO fileIO) : this(identifier, fileIO, ExTraceGlobals.PublisherManagerTracer)
		{
		}

		internal ApnsFeedbackPackage(ApnsFeedbackFileId identifier, ApnsFeedbackFileIO fileIO, ITracer tracer) : base(identifier, fileIO, tracer)
		{
		}

		public override bool IsLoaded
		{
			get
			{
				return this.HasLoadedMetadata && this.Feedback != null;
			}
		}

		public bool IsExtracted { get; private set; }

		public bool HasLoadedMetadata
		{
			get
			{
				return this.IsExtracted && this.Metadata != null && this.Metadata.IsLoaded;
			}
		}

		protected ApnsFeedbackMetadata Metadata { get; set; }

		protected Dictionary<string, ApnsFeedbackAppFile> Feedback { get; set; }

		public override ApnsFeedbackValidationResult ValidateNotification(ApnsNotification notification)
		{
			ArgumentValidator.ThrowIfNull("notification", notification);
			if (!this.IsLoaded)
			{
				base.Tracer.TraceDebug<ApnsNotification, ApnsFeedbackFileId>((long)this.GetHashCode(), "[ValidateNotification] Feedback package not loaded, defaulting to Uncertain for '{0}', '{1}'.", notification, base.Identifier);
				return ApnsFeedbackValidationResult.Uncertain;
			}
			ApnsFeedbackAppFile apnsFeedbackAppFile;
			if (this.Feedback.TryGetValue(notification.AppId, out apnsFeedbackAppFile))
			{
				return apnsFeedbackAppFile.ValidateNotification(notification);
			}
			base.Tracer.TraceDebug<ApnsNotification, string, ApnsFeedbackFileId>((long)this.GetHashCode(), "[ValidateNotification] Unable to find feedback for '{0}', '{1}', '{2}'.", notification, notification.AppId, base.Identifier);
			return ApnsFeedbackValidationResult.Uncertain;
		}

		public override void Remove()
		{
			Exception ex = null;
			base.Tracer.TraceDebug<ApnsFeedbackFileId>((long)this.GetHashCode(), "[Remove] Removing feedback package '{0}'", base.Identifier);
			try
			{
				if (this.IsExtracted)
				{
					base.FileIO.DeleteFolder(base.Identifier.GetPackageExtractionFolder());
				}
				else
				{
					base.FileIO.DeleteFile(base.Identifier.ToString());
				}
			}
			catch (UnauthorizedAccessException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				throw new ApnsFeedbackException(Strings.ApnsFeedbackPackageRemovalFailed(base.Identifier.ToString(), ex.Message), ex);
			}
		}

		public override void Load()
		{
			if (this.IsLoaded)
			{
				return;
			}
			if (!this.IsExtracted)
			{
				this.Extract();
			}
			if (!this.HasLoadedMetadata)
			{
				this.LoadMetadata();
			}
			this.LoadFeedback();
		}

		internal static ApnsFeedbackPackage CreateFromMetadata(ApnsFeedbackMetadata metadata)
		{
			ArgumentValidator.ThrowIfNull("metadata", metadata);
			return new ApnsFeedbackPackage(metadata.Identifier.GetPackageId(), metadata.FileIO, metadata.Tracer)
			{
				Metadata = metadata,
				IsExtracted = true
			};
		}

		internal static List<IApnsFeedbackFile> FindAll(string path, ApnsFeedbackFileIO fileIO, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("path", path);
			ArgumentValidator.ThrowIfNull("fileIO", fileIO);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			List<ApnsFeedbackPackage> list = ApnsFeedbackFileBase.FindFeedbackFiles<ApnsFeedbackPackage>(path, "*.zip", fileIO, (ApnsFeedbackFileId id) => new ApnsFeedbackPackage(id, fileIO), tracer);
			List<IApnsFeedbackFile> list2 = new List<IApnsFeedbackFile>(list.Count);
			foreach (ApnsFeedbackPackage item in list)
			{
				list2.Add(item);
			}
			List<ApnsFeedbackPackage> list3 = ApnsFeedbackFileBase.FindFeedbackFiles<ApnsFeedbackPackage>(path, "*.metadata", SearchOption.AllDirectories, fileIO, (ApnsFeedbackFileId id) => ApnsFeedbackPackage.CreateFromMetadata(new ApnsFeedbackMetadata(id, fileIO)), tracer);
			foreach (ApnsFeedbackPackage apnsFeedbackPackage in list3)
			{
				if (list2.Contains(apnsFeedbackPackage))
				{
					tracer.TraceWarning<ApnsFeedbackFileId>(0L, "[FindAll] Skipping extracted package '{0}' because we found also the compressed version", apnsFeedbackPackage.Identifier);
				}
				else
				{
					list2.Add(apnsFeedbackPackage);
				}
			}
			return list2;
		}

		private void Extract()
		{
			Exception ex = null;
			try
			{
				if (base.FileIO.Exists(base.Identifier.GetPackageExtractionFolder()))
				{
					base.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[Extract] Removing existing folder '{0}'", base.Identifier.GetPackageExtractionFolder());
					base.FileIO.DeleteFolder(base.Identifier.GetPackageExtractionFolder());
				}
				base.Tracer.TraceDebug<ApnsFeedbackFileId, string>((long)this.GetHashCode(), "[Extract] Extracting '{0}' on '{1}'", base.Identifier, base.Identifier.GetPackageExtractionFolder());
				base.FileIO.ExtractFileToDirectory(base.Identifier.ToString(), base.Identifier.GetPackageExtractionFolder());
				this.IsExtracted = true;
				base.Tracer.TraceDebug<ApnsFeedbackFileId>((long)this.GetHashCode(), "[Extract] Deleting '{0}'", base.Identifier);
				base.FileIO.DeleteFile(base.Identifier.ToString());
			}
			catch (UnauthorizedAccessException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				throw new ApnsFeedbackException(Strings.ApnsFeedbackPackageExtractionFailed(base.Identifier.ToString(), ex.Message), ex);
			}
		}

		private void LoadMetadata()
		{
			if (this.Metadata == null)
			{
				base.Tracer.TraceDebug<ApnsFeedbackFileId>((long)this.GetHashCode(), "[LoadMetadata] Loading metadata for '{0}'", base.Identifier);
				List<ApnsFeedbackMetadata> list = ApnsFeedbackFileBase.FindFeedbackFiles<ApnsFeedbackMetadata>(base.Identifier.GetPackageExtractionFolder(), "*.metadata", base.FileIO, (ApnsFeedbackFileId id) => new ApnsFeedbackMetadata(id, base.FileIO), base.Tracer);
				if (list.Count != 1)
				{
					if (list.Count > 1)
					{
						base.Tracer.TraceWarning(0L, string.Format("[FindInPackage] Found at least two metadata files for the same package: '{0}'; '{1}'", list[0], list[1]));
					}
					throw new ApnsFeedbackException(Strings.ApnsFeedbackPackageUnexpectedMetadataResult(base.Identifier.GetPackageExtractionFolder(), list.Count));
				}
				this.Metadata = list[0];
			}
			this.Metadata.Load();
		}

		private void LoadFeedback()
		{
			base.Tracer.TraceDebug<ApnsFeedbackFileId>((long)this.GetHashCode(), "[LoadMetadata] Loading feedback for '{0}''", base.Identifier);
			List<ApnsFeedbackAppFile> list = ApnsFeedbackFileBase.FindFeedbackFiles<ApnsFeedbackAppFile>(base.Identifier.GetPackageExtractionFolder(), base.Identifier.GetFeedbackFileSearchPattern(), base.FileIO, (ApnsFeedbackFileId id) => new ApnsFeedbackAppFile(id, base.FileIO), base.Tracer);
			if (list.Count <= 0)
			{
				throw new ApnsFeedbackException(Strings.ApnsFeedbackPackageFeedbackNotFound(base.Identifier.GetPackageExtractionFolder()));
			}
			Dictionary<string, ApnsFeedbackAppFile> dictionary = new Dictionary<string, ApnsFeedbackAppFile>();
			foreach (ApnsFeedbackAppFile apnsFeedbackAppFile in list)
			{
				apnsFeedbackAppFile.Load();
				dictionary.Add(apnsFeedbackAppFile.Identifier.AppId, apnsFeedbackAppFile);
			}
			this.Feedback = dictionary;
		}
	}
}
