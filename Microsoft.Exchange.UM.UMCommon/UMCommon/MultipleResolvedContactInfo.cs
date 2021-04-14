using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	internal class MultipleResolvedContactInfo : SimpleContactInfoBase
	{
		internal MultipleResolvedContactInfo(List<PersonalContactInfo> matches, PhoneNumber callerId, CultureInfo culture)
		{
			this.InitializeDisplayName(matches, callerId, culture);
		}

		internal override string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		internal override bool ResolvesToMultipleContacts
		{
			get
			{
				return true;
			}
		}

		internal string MultipleContactNames { get; private set; }

		private void InitializeDisplayName(List<PersonalContactInfo> matches, PhoneNumber callerId, CultureInfo culture)
		{
			this.displayName = callerId.ToDisplay;
			string firstContact = matches[0].DisplayName;
			string secondContact = matches[1].DisplayName;
			string number = MessageContentBuilder.FormatCallerIdWithAnchor(callerId, culture);
			this.MultipleContactNames = ((matches.Count == 2) ? Strings.MultipleResolvedContactDisplayWithTwoMatches(number, firstContact, secondContact).ToString(culture) : Strings.MultipleResolvedContactDisplayWithMoreThanTwoMatches(number, firstContact, secondContact).ToString(culture));
		}

		internal override LocalizedString GetVoicemailBodyDisplayLabel(PhoneNumber callerId, CultureInfo cultureInfo)
		{
			return Strings.VoiceMailBodyCallerMultipleResolved(this.MultipleContactNames);
		}

		internal override LocalizedString GetMissedCallBodyDisplayLabel(PhoneNumber callerId, CultureInfo cultureInfo)
		{
			return Strings.MissedCallBodyCallerMultipleResolved(this.MultipleContactNames);
		}

		internal override LocalizedString GetFaxBodyDisplayLabel(PhoneNumber callerId, CultureInfo cultureInfo)
		{
			return Strings.FaxBodyCallerMultipleResolved(this.MultipleContactNames);
		}

		private string displayName;
	}
}
