using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class RecipientWithAdUserGroupIdParameter<TRecipientIdParameter> : RecipientIdParameter where TRecipientIdParameter : RecipientIdParameter, new()
	{
		static RecipientWithAdUserGroupIdParameter()
		{
			TRecipientIdParameter trecipientIdParameter = Activator.CreateInstance<TRecipientIdParameter>();
			RecipientWithAdUserGroupIdParameter<TRecipientIdParameter>.AllowedRecipientTypes = new RecipientType[trecipientIdParameter.RecipientTypes.Length + 2];
			trecipientIdParameter.RecipientTypes.CopyTo(RecipientWithAdUserGroupIdParameter<TRecipientIdParameter>.AllowedRecipientTypes, 0);
			RecipientWithAdUserGroupIdParameter<TRecipientIdParameter>.AllowedRecipientTypes[RecipientWithAdUserGroupIdParameter<TRecipientIdParameter>.AllowedRecipientTypes.Length - 2] = RecipientType.Group;
			RecipientWithAdUserGroupIdParameter<TRecipientIdParameter>.AllowedRecipientTypes[RecipientWithAdUserGroupIdParameter<TRecipientIdParameter>.AllowedRecipientTypes.Length - 1] = RecipientType.User;
		}

		public RecipientWithAdUserGroupIdParameter(string identity) : base(identity)
		{
		}

		public RecipientWithAdUserGroupIdParameter()
		{
		}

		public RecipientWithAdUserGroupIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public RecipientWithAdUserGroupIdParameter(ADPresentationObject recipient) : base(recipient)
		{
		}

		public RecipientWithAdUserGroupIdParameter(ReducedRecipient recipient) : base(recipient.Id)
		{
		}

		public RecipientWithAdUserGroupIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return RecipientWithAdUserGroupIdParameter<TRecipientIdParameter>.AllowedRecipientTypes;
			}
		}

		public new static RecipientWithAdUserGroupIdParameter<TRecipientIdParameter> Parse(string identity)
		{
			return new RecipientWithAdUserGroupIdParameter<TRecipientIdParameter>(identity);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes;
	}
}
