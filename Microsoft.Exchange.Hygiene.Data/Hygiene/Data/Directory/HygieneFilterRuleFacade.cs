using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal abstract class HygieneFilterRuleFacade : ADObject
	{
		public bool Enabled
		{
			get
			{
				return (bool)this[HygieneFilterRuleFacadeSchema.Enabled];
			}
			set
			{
				this[HygieneFilterRuleFacadeSchema.Enabled] = value;
			}
		}

		public int Priority
		{
			get
			{
				return (int)this[HygieneFilterRuleFacadeSchema.Priority];
			}
			set
			{
				this[HygieneFilterRuleFacadeSchema.Priority] = value;
			}
		}

		public string Comments
		{
			get
			{
				return (string)this[HygieneFilterRuleFacadeSchema.Comments];
			}
			set
			{
				this[HygieneFilterRuleFacadeSchema.Comments] = value;
			}
		}

		public MultiValuedProperty<string> SentToMemberOf
		{
			get
			{
				return (MultiValuedProperty<string>)this[HygieneFilterRuleFacadeSchema.SentToMemberOf];
			}
			set
			{
				this[HygieneFilterRuleFacadeSchema.SentToMemberOf] = value;
			}
		}

		public MultiValuedProperty<string> SentTo
		{
			get
			{
				return (MultiValuedProperty<string>)this[HygieneFilterRuleFacadeSchema.SentTo];
			}
			set
			{
				this[HygieneFilterRuleFacadeSchema.SentTo] = value;
			}
		}

		public MultiValuedProperty<string> RecipientDomainIs
		{
			get
			{
				return (MultiValuedProperty<string>)this[HygieneFilterRuleFacadeSchema.RecipientDomainIs];
			}
			set
			{
				this[HygieneFilterRuleFacadeSchema.RecipientDomainIs] = value;
			}
		}

		public MultiValuedProperty<string> ExceptIfRecipientDomainIs
		{
			get
			{
				return (MultiValuedProperty<string>)this[HygieneFilterRuleFacadeSchema.ExceptIfRecipientDomainIs];
			}
			set
			{
				this[HygieneFilterRuleFacadeSchema.ExceptIfRecipientDomainIs] = value;
			}
		}

		public MultiValuedProperty<string> ExceptIfSentTo
		{
			get
			{
				return (MultiValuedProperty<string>)this[HygieneFilterRuleFacadeSchema.ExceptIfSentTo];
			}
			set
			{
				this[HygieneFilterRuleFacadeSchema.ExceptIfSentTo] = value;
			}
		}

		public MultiValuedProperty<string> ExceptIfSentToMemberOf
		{
			get
			{
				return (MultiValuedProperty<string>)this[HygieneFilterRuleFacadeSchema.ExceptIfSentToMemberOf];
			}
			set
			{
				this[HygieneFilterRuleFacadeSchema.ExceptIfSentToMemberOf] = value;
			}
		}
	}
}
