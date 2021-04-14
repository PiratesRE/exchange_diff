using System;
using System.Text;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	internal sealed class PeopleRelevanceConfig : Config
	{
		private PeopleRelevanceConfig()
		{
		}

		internal static PeopleRelevanceConfig Instance
		{
			get
			{
				return PeopleRelevanceConfig.peopleRelevanceConfigInstance;
			}
		}

		internal int RelevanceCategoriesCount { get; set; }

		internal int MaxRelevantRecipientsCount { get; set; }

		internal TimeSpan TimeWindowLength { get; private set; }

		internal int MaxContactUpdatesCount { get; set; }

		internal TimeSpan SleepTimeBeforeInferenceProcessingStarts { get; private set; }

		internal TimeSpan RecipientCacheValidationInterval { get; private set; }

		public override void Load()
		{
			this.RelevanceCategoriesCount = base.ReadInt("relevanceCategoriesCount", 10);
			this.MaxRelevantRecipientsCount = base.ReadInt("maxRelevantRecipientsCount", 200);
			this.TimeWindowLength = base.ReadTimeSpan("timeWindowLength", PeopleRelevanceConfig.DefaultTimeWindowLength);
			this.MaxContactUpdatesCount = base.ReadInt("maxContactUpdatesCount", 10);
			this.SleepTimeBeforeInferenceProcessingStarts = base.ReadTimeSpan("sleepTimeBeforeInferenceProcessingStarts", PeopleRelevanceConfig.DefaultSleepTimeBeforeInferenceProcessingStarts);
			this.RecipientCacheValidationInterval = base.ReadTimeSpan("recipientCacheValidationInterval", PeopleRelevanceConfig.DefaultRecipientCacheValidationInterval);
			this.Validate();
		}

		private void Validate()
		{
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder("Invalid PeopleRelevanceConfig parameters. ");
			if (this.RelevanceCategoriesCount < 1)
			{
				flag = false;
				stringBuilder.AppendFormat("RelevanceCategoriesCount {0}. ", this.RelevanceCategoriesCount.ToString());
			}
			if (this.MaxRelevantRecipientsCount < 1 || this.MaxRelevantRecipientsCount == 2147483647)
			{
				flag = false;
				stringBuilder.AppendFormat("MaxRelevantRecipientsCount {0}. ", this.MaxRelevantRecipientsCount.ToString());
			}
			if (this.TimeWindowLength <= TimeSpan.Zero)
			{
				flag = false;
				stringBuilder.AppendFormat("TimeWindowLength {0}. ", this.TimeWindowLength.ToString());
			}
			if (this.MaxContactUpdatesCount < 1)
			{
				flag = false;
				stringBuilder.AppendFormat("MaxContactUpdatesCount {0}. ", this.MaxContactUpdatesCount.ToString());
			}
			if (this.SleepTimeBeforeInferenceProcessingStarts < TimeSpan.Zero)
			{
				flag = false;
				stringBuilder.AppendFormat("SleepTimeBeforeInferenceProcessingStarts {0}. ", this.SleepTimeBeforeInferenceProcessingStarts.ToString());
			}
			if (this.RecipientCacheValidationInterval < TimeSpan.Zero)
			{
				flag = false;
				stringBuilder.AppendFormat("RecipientCacheValidationInterval {0}. ", this.RecipientCacheValidationInterval.ToString());
			}
			if (!flag)
			{
				throw new InvalidOperationException(stringBuilder.ToString());
			}
		}

		internal const int DefaultMaxContactUpdatesCount = 10;

		private const int DefaultRelevanceCategoriesCount = 10;

		private const int DefaultMaxRelevantRecipientsCount = 200;

		private static readonly TimeSpan DefaultTimeWindowLength = TimeSpan.FromHours(8.0);

		private static readonly TimeSpan DefaultSleepTimeBeforeInferenceProcessingStarts = TimeSpan.Zero;

		private static readonly TimeSpan DefaultRecipientCacheValidationInterval = TimeSpan.FromDays(3.0);

		private static PeopleRelevanceConfig peopleRelevanceConfigInstance = new PeopleRelevanceConfig();
	}
}
