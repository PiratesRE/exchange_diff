using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	[DataContract]
	[Serializable]
	internal sealed class MaskedRecipient
	{
		public MaskedRecipient(string emailAddress)
		{
			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new ArgumentException("emailAddress");
			}
			this.EmailAddress = emailAddress;
			this.LastMaskedFromAutoCompleteTimeUtc = DateTime.UtcNow;
		}

		[DataMember]
		public string EmailAddress { get; set; }

		[DataMember]
		public DateTime LastMaskedFromAutoCompleteTimeUtc { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[Email:{0},", this.EmailAddress);
			stringBuilder.AppendFormat("LastDeletedUtc:{0}]", this.LastMaskedFromAutoCompleteTimeUtc);
			return stringBuilder.ToString();
		}
	}
}
