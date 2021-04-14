using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	internal class DefaultContactInfo : SimpleContactInfoBase
	{
		internal DefaultContactInfo()
		{
		}

		internal override ICollection<string> SanitizedPhoneNumbers
		{
			get
			{
				return this.sanitizedPhoneNumbers;
			}
		}

		internal override LocalizedString GetVoicemailBodyDisplayLabel(PhoneNumber callerId, CultureInfo cultureInfo)
		{
			return Strings.VoiceMailBodyCallerUnresolved(MessageContentBuilder.FormatCallerIdWithAnchor(callerId, cultureInfo));
		}

		internal override LocalizedString GetMissedCallBodyDisplayLabel(PhoneNumber callerId, CultureInfo cultureInfo)
		{
			return Strings.MissedCallBodyCallerUnresolved(MessageContentBuilder.FormatCallerIdWithAnchor(callerId, cultureInfo));
		}

		internal override LocalizedString GetFaxBodyDisplayLabel(PhoneNumber callerId, CultureInfo cultureInfo)
		{
			return Strings.FaxBodyCallerUnresolved(MessageContentBuilder.FormatCallerIdWithAnchor(callerId, cultureInfo));
		}

		private List<string> sanitizedPhoneNumbers = new List<string>();
	}
}
