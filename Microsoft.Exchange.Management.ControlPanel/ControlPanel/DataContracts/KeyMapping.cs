using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel.DataContracts
{
	[DataContract]
	public class KeyMapping
	{
		public KeyMapping()
		{
		}

		public KeyMapping(KeyMapping taskKeyMapping)
		{
			this.Context = taskKeyMapping.Context;
			this.FindMeFirstNumber = taskKeyMapping.FindMeFirstNumber;
			this.FindMeFirstNumberDuration = taskKeyMapping.FindMeFirstNumberDuration;
			this.FindMeSecondNumber = taskKeyMapping.FindMeSecondNumber;
			this.FindMeSecondNumberDuration = taskKeyMapping.FindMeSecondNumberDuration;
			this.Key = taskKeyMapping.Key;
			this.KeyMappingType = taskKeyMapping.KeyMappingType.ToStringWithNull();
			this.TransferToGALContactLegacyDN = taskKeyMapping.TransferToGALContactLegacyDN;
			if (!string.IsNullOrEmpty(this.TransferToGALContactLegacyDN))
			{
				IEnumerable<ADRecipient> enumerable = RecipientObjectResolver.Instance.ResolveLegacyDNs(new string[]
				{
					this.TransferToGALContactLegacyDN
				});
				if (enumerable != null)
				{
					using (IEnumerator<ADRecipient> enumerator = enumerable.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							ADRecipient adrecipient = enumerator.Current;
							this.TransferToGALContactDisplayName = adrecipient.DisplayName;
						}
					}
				}
			}
			this.TransferToNumber = taskKeyMapping.TransferToNumber;
		}

		[DataMember]
		public string Context { get; set; }

		[DataMember]
		public string FindMeFirstNumber { get; set; }

		[DataMember]
		public int FindMeFirstNumberDuration { get; set; }

		[DataMember]
		public string FindMeSecondNumber { get; set; }

		[DataMember]
		public int FindMeSecondNumberDuration { get; set; }

		[DataMember]
		public int Key { get; set; }

		[DataMember]
		public string KeyMappingType { get; set; }

		[DataMember]
		public string TransferToGALContactLegacyDN { get; set; }

		[DataMember]
		public string TransferToNumber { get; set; }

		[DataMember]
		public string TransferToGALContactDisplayName { get; set; }

		public KeyMapping ToTaskObject()
		{
			string transferToGALContactLegacyDN = null;
			if (!string.IsNullOrEmpty(this.TransferToGALContactLegacyDN))
			{
				Guid guid;
				if (Guid.TryParse(this.TransferToGALContactLegacyDN, out guid))
				{
					ADObjectId adobjectId = ADObjectId.ParseDnOrGuid(this.TransferToGALContactLegacyDN);
					if (adobjectId == null)
					{
						goto IL_85;
					}
					IEnumerable<PeopleRecipientObject> enumerable = RecipientObjectResolver.Instance.ResolvePeople(new ADObjectId[]
					{
						adobjectId
					});
					if (enumerable == null)
					{
						goto IL_85;
					}
					using (IEnumerator<PeopleRecipientObject> enumerator = enumerable.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							PeopleRecipientObject peopleRecipientObject = enumerator.Current;
							transferToGALContactLegacyDN = peopleRecipientObject.LegacyExchangeDN;
						}
						goto IL_85;
					}
				}
				transferToGALContactLegacyDN = this.TransferToGALContactLegacyDN;
			}
			IL_85:
			return new KeyMapping((KeyMappingType)Enum.Parse(typeof(KeyMappingType), this.KeyMappingType), this.Key, this.Context, this.FindMeFirstNumber, this.FindMeFirstNumberDuration, this.FindMeSecondNumber, this.FindMeSecondNumberDuration, this.TransferToNumber, transferToGALContactLegacyDN);
		}
	}
}
