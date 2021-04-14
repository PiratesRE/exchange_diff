using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SearchItemDetailPrompt : VariablePrompt<ContactSearchItem>
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"searchItemDetail",
				base.Config.PromptName,
				string.Empty,
				base.SbLog.ToString()
			});
		}

		internal override string ToSsml()
		{
			return base.SbSsml.ToString();
		}

		protected override void InternalInitialize()
		{
			this.item = base.InitVal;
			if (this.item.IsPersonalContact && this.item.IsGroup)
			{
				this.AddGroupDetailPrompts();
			}
			else
			{
				this.AddContactDetailPrompts();
			}
			if (base.SbSsml.Length == 0)
			{
				this.AddNoDetailsPrompt();
				return;
			}
			this.InsertDetailIntroPromptToBeginning();
		}

		private void AddContactDetailPrompts()
		{
			if (this.item.HasBusinessAddress)
			{
				AddressPrompt paramPrompt = new AddressPrompt("businessAddress", base.Culture, this.item.BusinessAddress.ToString(base.Culture));
				this.AddStatementPrompt("tuiContactsBusinessAddress", base.Culture, paramPrompt);
			}
			if (this.item.HasHomeAddress)
			{
				AddressPrompt paramPrompt2 = new AddressPrompt("homeAddress", base.Culture, this.item.HomeAddress);
				this.AddStatementPrompt("tuiContactsHomeAddress", base.Culture, paramPrompt2);
			}
			if (this.item.HasOtherAddress)
			{
				AddressPrompt paramPrompt3 = new AddressPrompt("otherAddress", base.Culture, this.item.OtherAddress);
				this.AddStatementPrompt("tuiContactsOtherAddress", base.Culture, paramPrompt3);
			}
			if (this.item.HasMobileNumber)
			{
				TelephoneNumberPrompt paramPrompt4 = new TelephoneNumberPrompt("mobileNumber", base.Culture, this.item.MobilePhone);
				this.AddStatementPrompt("tuiContactsMobilePhoneNumber", base.Culture, paramPrompt4);
			}
			if (this.item.HasBusinessNumber)
			{
				TelephoneNumberPrompt paramPrompt5 = new TelephoneNumberPrompt("businessNumber", base.Culture, this.item.BusinessPhone);
				this.AddStatementPrompt("tuiContactsBusinessPhoneNumber", base.Culture, paramPrompt5);
			}
			if (this.item.HasHomeNumber)
			{
				TelephoneNumberPrompt paramPrompt6 = new TelephoneNumberPrompt("homeNumber", base.Culture, this.item.HomePhone);
				this.AddStatementPrompt("tuiContactsHomePhoneNumber", base.Culture, paramPrompt6);
			}
			if (this.item.HasAlias)
			{
				EmailPrompt paramPrompt7 = new EmailPrompt("alias", base.Culture, new EmailNormalizedText(this.item.Alias));
				this.AddStatementPrompt("tuiContactsEmailAlias", base.Culture, paramPrompt7);
			}
			if (this.item.HasEmail1)
			{
				EmailPrompt paramPrompt8 = new EmailPrompt("email1", base.Culture, new EmailNormalizedText(this.item.ContactEmailAddresses[0]));
				this.AddStatementPrompt("tuiContactsEmail1", base.Culture, paramPrompt8);
			}
			if (this.item.HasEmail2)
			{
				EmailPrompt paramPrompt9 = new EmailPrompt("email2", base.Culture, new EmailNormalizedText(this.item.ContactEmailAddresses[1]));
				this.AddStatementPrompt("tuiContactsEmail2", base.Culture, paramPrompt9);
			}
			if (this.item.HasEmail3)
			{
				EmailPrompt paramPrompt10 = new EmailPrompt("email3", base.Culture, new EmailNormalizedText(this.item.ContactEmailAddresses[2]));
				this.AddStatementPrompt("tuiContactsEmail3", base.Culture, paramPrompt10);
			}
		}

		private void AddGroupDetailPrompts()
		{
			this.AddStatementPrompt("tuiGroupDetails", base.Culture, null);
		}

		private void InsertDetailIntroPromptToBeginning()
		{
			SpokenNamePrompt spokenNamePrompt = new SpokenNamePrompt("userName", base.Culture, this.item.FullName);
			SingleStatementPrompt singleStatementPrompt = PromptUtils.CreateSingleStatementPrompt("tuiContactsDetailsIntro", base.Culture, new Prompt[]
			{
				spokenNamePrompt
			});
			base.SbSsml.Insert(0, singleStatementPrompt.ToSsml());
			base.SbLog.Insert(0, singleStatementPrompt.ToString());
		}

		private void AddNoDetailsPrompt()
		{
			SpokenNamePrompt paramPrompt = new SpokenNamePrompt("userName", base.Culture, this.item.FullName);
			this.AddStatementPrompt("tuiContactsNoDetails", base.Culture, paramPrompt);
		}

		private void AddStatementPrompt(string promptName, CultureInfo culture, Prompt paramPrompt)
		{
			SingleStatementPrompt p = PromptUtils.CreateSingleStatementPrompt(promptName, culture, new Prompt[]
			{
				paramPrompt
			});
			base.AddPrompt(p);
		}

		private const string GroupDetailsStatementPromptName = "tuiGroupDetails";

		private const string DetailsIntroStatementPromptName = "tuiContactsDetailsIntro";

		private const string NoDetailsStatementPromptName = "tuiContactsNoDetails";

		private const string UserNamePromptName = "userName";

		private const string BusinessAddressStatementPromptName = "tuiContactsBusinessAddress";

		private const string BusinessAddressPromptName = "businessAddress";

		private const string HomeAddressStatementPromptName = "tuiContactsHomeAddress";

		private const string HomeAddressPromptName = "homeAddress";

		private const string OtherAddressStatementPromptName = "tuiContactsOtherAddress";

		private const string OtherAddressPromptName = "otherAddress";

		private const string MobileNumberStatementPromptName = "tuiContactsMobilePhoneNumber";

		private const string MobileNumberPromptName = "mobileNumber";

		private const string BusinessNumberStatementPromptName = "tuiContactsBusinessPhoneNumber";

		private const string BusinessNumberPromptName = "businessNumber";

		private const string HomeNumberStatementPromptName = "tuiContactsHomePhoneNumber";

		private const string HomeNumberPromptName = "homeNumber";

		private const string Email1StatementPromptName = "tuiContactsEmail1";

		private const string Email1PromptName = "email1";

		private const string Email2StatementPromptName = "tuiContactsEmail2";

		private const string Email2PromptName = "email2";

		private const string Email3StatementPromptName = "tuiContactsEmail3";

		private const string Email3PromptName = "email3";

		private const string OfficeNumberStatementPromptName = "tuiContactsOfficePhoneNumber";

		private const string LocationStatementPromptName = "tuiContactsLocation";

		private const string EmailAliasStatementPromptName = "tuiContactsEmailAlias";

		private const string EmailAliasPromptName = "alias";

		private ContactSearchItem item;
	}
}
