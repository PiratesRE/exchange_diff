using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Inference.Common;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	[DataContract(Name = "MaskedPeopleModelItem")]
	[KnownType(typeof(InferenceRecipient))]
	[Serializable]
	internal class MaskedPeopleModelItem : InferenceBaseModelItem
	{
		public MaskedPeopleModelItem()
		{
			base.Version = PeopleModelItem.CurrentVersion;
			this.ContactList = new List<MaskedRecipient>(10);
		}

		[DataMember]
		public List<MaskedRecipient> ContactList { get; set; }

		public IDictionary<string, MaskedRecipient> CreateDictionary()
		{
			return this.ContactList.ToDictionary((MaskedRecipient x) => x.EmailAddress, StringComparer.OrdinalIgnoreCase);
		}

		public const string ModelDataFAIName = "Inference.MaskedPeopleModel";

		public static readonly Version MinimumSupportedVersion = new Version(1, 0);

		public static readonly Version CurrentVersion = new Version(1, 0);
	}
}
