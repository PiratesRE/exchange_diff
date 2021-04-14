using System;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Inference.Mdb;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	[DataContract]
	[Serializable]
	internal sealed class InferenceRecipient : MdbRecipient, IInferenceRecipient, IMessageRecipient, IEquatable<IMessageRecipient>
	{
		internal InferenceRecipient(IMessageRecipient recipient) : base(recipient)
		{
			this.InitializeInternal();
		}

		[DataMember(Name = "TotalSentCount")]
		public long TotalSentCount { get; set; }

		[DataMember(Name = "FirstSentTime")]
		public DateTime FirstSentTime { get; set; }

		[DataMember(Name = "LastSentTime")]
		public DateTime LastSentTime { get; set; }

		[DataMember(Name = "RecipientRank")]
		public int RecipientRank
		{
			get
			{
				return this.recipientRank;
			}
			set
			{
				if (value < 1 || (value > PeopleRelevanceConfig.Instance.MaxRelevantRecipientsCount && value != 2147483647))
				{
					throw new InvalidOperationException(string.Format("Invalid recipient rank: {0}", value.ToString()));
				}
				this.recipientRank = value;
			}
		}

		[DataMember(Name = "RawRecipientWeight")]
		public double RawRecipientWeight { get; set; }

		public int RelevanceCategory
		{
			get
			{
				return InferenceRecipient.GetRelevanceCategoryForRecipient(this);
			}
		}

		[DataMember(Name = "RelevanceCategoryAtLastCapture")]
		public int RelevanceCategoryAtLastCapture { get; set; }

		[DataMember(Name = "LastUsedInTimeWindow")]
		public long LastUsedInTimeWindow { get; set; }

		[DataMember(Name = "CaptureFlag")]
		public int CaptureFlag { get; set; }

		[DataMember(Name = "HasUpdatedData")]
		public bool HasUpdatedData { get; set; }

		public override void UpdateFromRecipient(IMessageRecipient recipient)
		{
			if (!string.Equals(base.SipUri, recipient.SipUri, StringComparison.OrdinalIgnoreCase) || !string.Equals(base.Alias, recipient.Alias, StringComparison.OrdinalIgnoreCase) || base.RecipientDisplayType != recipient.RecipientDisplayType)
			{
				this.HasUpdatedData = true;
			}
			base.UpdateFromRecipient(recipient);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[S:{0},", base.SmtpAddress);
			stringBuilder.AppendFormat("R:{0},", this.RecipientRank);
			stringBuilder.AppendFormat("U:{0},", this.HasUpdatedData);
			stringBuilder.AppendFormat("F:{0}]", this.CaptureFlag);
			return stringBuilder.ToString();
		}

		internal static int GetRelevanceCategoryForRecipient(IInferenceRecipient recipient)
		{
			return InferenceRecipient.GetRelevanceCategoryForRank(recipient.RecipientRank);
		}

		internal static int GetRelevanceCategoryForRank(int relevanceRank)
		{
			if (relevanceRank == 2147483647)
			{
				return int.MaxValue;
			}
			return Math.Min(PeopleRelevanceConfig.Instance.RelevanceCategoriesCount, relevanceRank / (PeopleRelevanceConfig.Instance.MaxRelevantRecipientsCount / PeopleRelevanceConfig.Instance.RelevanceCategoriesCount) + 1);
		}

		private void InitializeInternal()
		{
			this.TotalSentCount = 0L;
			this.RecipientRank = int.MaxValue;
			this.RelevanceCategoryAtLastCapture = int.MaxValue;
		}

		public const int RecipientRankForIrrelevantEntries = 2147483647;

		public const int RelevanceCategoryForIrrelevantEntries = 2147483647;

		public const double RawRecipientWeightForIrrelevantEntries = 0.0;

		private int recipientRank;
	}
}
