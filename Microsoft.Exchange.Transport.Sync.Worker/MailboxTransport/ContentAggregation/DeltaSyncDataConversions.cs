using System;
using System.Text;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class DeltaSyncDataConversions
	{
		internal static string EncodeEmailAddress(string nativeEmailAddress)
		{
			if (nativeEmailAddress == null)
			{
				return null;
			}
			if (!DeltaSyncDataConversions.IsProcessingRequired(nativeEmailAddress))
			{
				return nativeEmailAddress;
			}
			Participant participant = null;
			if (!Participant.TryParse(nativeEmailAddress, out participant))
			{
				return nativeEmailAddress;
			}
			string displayName = participant.DisplayName;
			if (displayName == null)
			{
				return nativeEmailAddress;
			}
			Participant participant2 = new Participant(MimeInternalHelpers.Rfc2047Encode(displayName, Encoding.UTF8), participant.EmailAddress, "SMTP");
			return participant2.ToString(AddressFormat.Rfc822Smtp);
		}

		internal static string DecodeEmailAddress(string encodedEmailAddress)
		{
			if (encodedEmailAddress == null)
			{
				return null;
			}
			if (!DeltaSyncDataConversions.IsProcessingRequired(encodedEmailAddress))
			{
				return encodedEmailAddress;
			}
			Participant participant = null;
			if (!Participant.TryParse(encodedEmailAddress, out participant))
			{
				return encodedEmailAddress;
			}
			string displayName = participant.DisplayName;
			if (displayName == null)
			{
				return encodedEmailAddress;
			}
			Participant participant2 = new Participant(MimeInternalHelpers.Rfc2047Decode(displayName), participant.EmailAddress, "SMTP");
			return participant2.ToString(AddressFormat.Rfc822Smtp);
		}

		private static bool IsProcessingRequired(string emailAddress)
		{
			return emailAddress.EndsWith(">") && emailAddress.StartsWith("\"");
		}
	}
}
