using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public abstract class MailUserIdParameterBase : RecipientIdParameter
	{
		public MailUserIdParameterBase(string identity) : base(identity)
		{
		}

		public MailUserIdParameterBase()
		{
		}

		public MailUserIdParameterBase(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public MailUserIdParameterBase(MailUser user) : base(user.Id)
		{
		}

		public MailUserIdParameterBase(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return MailUserIdParameterBase.AllowedRecipientTypes;
			}
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.MailUser
		};
	}
}
