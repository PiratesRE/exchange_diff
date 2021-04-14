using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public abstract class MailEnabledOrgPerson : MailEnabledRecipient
	{
		protected MailEnabledOrgPerson()
		{
		}

		protected MailEnabledOrgPerson(ADObject dataObject) : base(dataObject)
		{
		}

		public MultiValuedProperty<string> Extensions
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailEnabledOrgPersonSchema.Extensions];
			}
		}

		public bool HasPicture
		{
			get
			{
				return (bool)this[MailEnabledOrgPersonSchema.HasPicture];
			}
		}

		public bool HasSpokenName
		{
			get
			{
				return (bool)this[MailEnabledOrgPersonSchema.HasSpokenName];
			}
		}
	}
}
