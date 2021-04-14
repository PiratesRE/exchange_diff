using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal abstract class FullSyncConfiguration : SyncConfiguration
	{
		private protected static int ObjectsPerPageLimit { protected get; private set; }

		private protected static int LinksPerPageLimit { protected get; private set; }

		public static int InitialLinkReadSize { get; private set; }

		protected int ReturnedLinkCount { get; set; }

		protected int ReturnedObjectCount { get; set; }

		public IFullSyncPageToken FullSyncPageToken { get; private set; }

		static FullSyncConfiguration()
		{
			FullSyncConfiguration.InitializeConfigurableSettings();
			FullSyncConfiguration.InitialLinkMetadataRangedProperty = RangedPropertyHelper.CreateRangedProperty(ADRecipientSchema.LinkMetadata, new IntRange(0, FullSyncConfiguration.InitialLinkReadSize - 1));
			List<PropertyDefinition> list = new List<PropertyDefinition>(SyncSchema.Instance.AllBackSyncBaseProperties.Cast<PropertyDefinition>());
			list.AddRange(SyncObject.BackSyncProperties.Cast<PropertyDefinition>());
			list.AddRange(SyncSchema.Instance.AllBackSyncShadowBaseProperties.Cast<PropertyDefinition>());
			FullSyncConfiguration.backSyncBaseProperties = list.ToArray();
			list.Add(FullSyncConfiguration.InitialLinkMetadataRangedProperty);
			list.Add(ADRecipientSchema.UsnChanged);
			FullSyncConfiguration.backSyncBasePropertiesPlusLinks = list.ToArray();
		}

		internal static void InitializeConfigurableSettings()
		{
			FullSyncConfiguration.ObjectsPerPageLimit = SyncConfiguration.GetConfigurationValue<int>("ObjectsPerPageLimit", 200);
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "FullSyncConfiguration.InitializeConfigurableSettings FullSyncConfiguration.ObjectsPerPageLimit = {0}", FullSyncConfiguration.ObjectsPerPageLimit);
			FullSyncConfiguration.LinksPerPageLimit = SyncConfiguration.GetConfigurationValue<int>("LinksPerPageLimit", 2000);
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "FullSyncConfiguration.InitializeConfigurableSettings FullSyncConfiguration.LinksPerPageLimit = {0}", FullSyncConfiguration.LinksPerPageLimit);
			FullSyncConfiguration.InitialLinkReadSize = SyncConfiguration.GetConfigurationValue<int>("InitialLinkReadSize", 100);
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "FullSyncConfiguration.InitializeConfigurableSettings FullSyncConfiguration.InitialLinkReadSize = {0}", FullSyncConfiguration.InitialLinkReadSize);
		}

		public virtual DirectoryObjectError[] GetReportedErrors()
		{
			return null;
		}

		public FullSyncConfiguration(IFullSyncPageToken pageToken, Guid invocationId, OutputResultDelegate writeResult, ISyncEventLogger eventLogger, IExcludedObjectReporter excludedObjectReporter, IFullSyncPageToken originalToken) : base(invocationId, writeResult, eventLogger, excludedObjectReporter)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "New FullSyncConfiguration");
			this.FullSyncPageToken = pageToken;
			this.originalToken = originalToken;
		}

		public override bool MoreData
		{
			get
			{
				return this.FullSyncPageToken.MoreData;
			}
		}

		public override byte[] GetResultCookie()
		{
			return this.FullSyncPageToken.ToByteArray();
		}

		public override Exception HandleException(Exception e)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "FullSyncConfiguration HandleException");
			DateTime utcNow = DateTime.UtcNow;
			if (e is ADExternalException || (base.IsSubsequentFailedAttempt() && base.IsFailoverTimeoutExceeded(utcNow)))
			{
				ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "FullSyncConfiguration throw BackSyncDataSourceUnavailableException for exception {0}", e.ToString());
				return new BackSyncDataSourceUnavailableException(e);
			}
			if (base.IsTransientException(e))
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Handle transient exception");
				this.originalToken.PrepareForFailover();
				base.UpdateSyncCookieErrorObjectsAndFailureCounts(this.originalToken);
				this.ReturnErrorPageToken(utcNow, this.originalToken);
				return new BackSyncDataSourceTransientException(e);
			}
			return e;
		}

		protected static bool NotAllLinksRetrieved(MultiValuedProperty<LinkMetadata> linkMetadata)
		{
			bool flag = linkMetadata.ValueRange != null && !linkMetadata.ValueRange.Equals(RangedPropertyHelper.AllValuesRange);
			ExTraceGlobals.BackSyncTracer.TraceDebug<bool>((long)SyncConfiguration.TraceId, "NotAllLinksRetrieved = {0}", flag);
			return flag;
		}

		protected static PropertyDefinition[] GetPropertyDefinitions(bool includeLinks)
		{
			if (!includeLinks)
			{
				return FullSyncConfiguration.backSyncBaseProperties;
			}
			return FullSyncConfiguration.backSyncBasePropertiesPlusLinks;
		}

		protected override DateTime GetLastReadFailureStartTime()
		{
			return this.FullSyncPageToken.LastReadFailureStartTime;
		}

		protected override DateTime GetSyncSequenceStartTime()
		{
			return this.FullSyncPageToken.SequenceStartTimestamp;
		}

		protected override bool IsDCFailoverResilienceEnabled()
		{
			return SyncConfiguration.EnableDCFailoverResilienceForTenantFullSync();
		}

		protected void ReturnErrorPageToken(DateTime now, IFullSyncPageToken pageToken)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "FullSyncConfiguration ReturnErrorPageToken");
			DateTime dateTime = base.IsSubsequentFailedAttempt() ? this.GetLastReadFailureStartTime() : now;
			ExTraceGlobals.BackSyncTracer.TraceDebug<DateTime>((long)SyncConfiguration.TraceId, "FullSyncConfiguration lastReadFailureStartTime {0}", dateTime);
			this.originalToken.Timestamp = now;
			this.originalToken.LastReadFailureStartTime = dateTime;
			byte[] array = pageToken.ToByteArray();
			base.WriteResult(array, SyncObject.CreateGetDirectoryObjectsResponse(new List<SyncObject>(), true, array, this.GetReportedErrors(), null));
		}

		protected virtual bool ShouldReadMoreData()
		{
			return this.ReturnedObjectCount < FullSyncConfiguration.ObjectsPerPageLimit && this.ReturnedLinkCount < FullSyncConfiguration.LinksPerPageLimit;
		}

		private const string ObjectsPerPageLimitValueName = "ObjectsPerPageLimit";

		private const string LinksPerPageLimitValueName = "LinksPerPageLimit";

		private const string InitialLinkReadSizeValueName = "InitialLinkReadSize";

		internal const int DefaultInitialLinkReadSize = 100;

		private const int DefaultObjectsPerPageLimit = 200;

		private const int DefaultLinksPerPageLimit = 2000;

		private static readonly PropertyDefinition[] backSyncBaseProperties;

		private static readonly PropertyDefinition[] backSyncBasePropertiesPlusLinks;

		protected static readonly ADPropertyDefinition InitialLinkMetadataRangedProperty;

		private IFullSyncPageToken originalToken;
	}
}
