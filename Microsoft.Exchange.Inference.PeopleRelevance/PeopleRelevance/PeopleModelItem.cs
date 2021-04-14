using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Exchange.Inference.Common;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	[DataContract(Name = "PeopleModelData")]
	[KnownType(typeof(InferenceRecipient))]
	[Serializable]
	internal class PeopleModelItem : InferenceBaseModelItem
	{
		public PeopleModelItem()
		{
			base.Version = PeopleModelItem.CurrentVersion;
			this.LastProcessedMessageSentTime = DateTime.MinValue;
			this.IsDefaultModel = true;
			this.ContactList = new List<IInferenceRecipient>();
			this.CurrentTimeWindowStartTime = DateTime.MinValue;
			this.LastRecipientCacheValidationTime = DateTime.MinValue;
		}

		[DataMember(Name = "ContactList")]
		public List<IInferenceRecipient> ContactList { get; set; }

		[DataMember]
		public DateTime LastProcessedMessageSentTime { get; set; }

		[DataMember]
		public DateTime CurrentTimeWindowStartTime { get; set; }

		[DataMember]
		public long CurrentTimeWindowNumber { get; set; }

		[DataMember]
		public DateTime LastRecipientCacheValidationTime { get; set; }

		[DataMember]
		internal bool IsDefaultModel { get; set; }

		public IDictionary<string, IInferenceRecipient> CreateContactDictionary()
		{
			IDictionary<string, IInferenceRecipient> dictionary = new Dictionary<string, IInferenceRecipient>();
			foreach (IInferenceRecipient inferenceRecipient in this.ContactList)
			{
				string text = inferenceRecipient.SmtpAddress;
				if (!string.IsNullOrEmpty(text))
				{
					text = text.ToLower(CultureInfo.InvariantCulture);
					dictionary.Add(text, inferenceRecipient);
				}
			}
			return dictionary;
		}

		public const string ModelDataFAIName = "Inference.PeopleModel";

		public static readonly Version MinimumSupportedVersion = new Version(1, 5);

		public static readonly Version CurrentVersion = new Version(1, 5);
	}
}
