using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class RecipientWithAdUserIdParameter<TRecipientIdParameter> : RecipientIdParameter where TRecipientIdParameter : RecipientIdParameter, new()
	{
		static RecipientWithAdUserIdParameter()
		{
			TRecipientIdParameter trecipientIdParameter = Activator.CreateInstance<TRecipientIdParameter>();
			RecipientWithAdUserIdParameter<TRecipientIdParameter>.AllowedRecipientTypes = new RecipientType[trecipientIdParameter.RecipientTypes.Length + 1];
			trecipientIdParameter.RecipientTypes.CopyTo(RecipientWithAdUserIdParameter<TRecipientIdParameter>.AllowedRecipientTypes, 0);
			RecipientWithAdUserIdParameter<TRecipientIdParameter>.AllowedRecipientTypes[RecipientWithAdUserIdParameter<TRecipientIdParameter>.AllowedRecipientTypes.Length - 1] = RecipientType.User;
		}

		public RecipientWithAdUserIdParameter(string identity) : base(identity)
		{
		}

		public RecipientWithAdUserIdParameter()
		{
		}

		public RecipientWithAdUserIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public RecipientWithAdUserIdParameter(ADPresentationObject recipient) : base(recipient)
		{
		}

		public RecipientWithAdUserIdParameter(ReducedRecipient recipient) : base(recipient.Id)
		{
		}

		public RecipientWithAdUserIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return RecipientWithAdUserIdParameter<TRecipientIdParameter>.AllowedRecipientTypes;
			}
		}

		public new static RecipientWithAdUserIdParameter<TRecipientIdParameter> Parse(string identity)
		{
			return new RecipientWithAdUserIdParameter<TRecipientIdParameter>(identity);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes;
	}
}
