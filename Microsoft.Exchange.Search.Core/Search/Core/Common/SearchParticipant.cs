using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class SearchParticipant
	{
		internal SearchParticipant()
		{
		}

		internal SearchParticipant(Participant participant)
		{
			this.SmtpAddress = participant.GetValueOrDefault<string>(ParticipantSchema.SmtpAddress);
			this.DisplayName = participant.DisplayName;
			this.SetRoutingType();
		}

		internal string SmtpAddress { get; private set; }

		internal string DisplayName { get; private set; }

		internal string RoutingType { get; private set; }

		public static string ToParticipantString(Participant participant)
		{
			if (participant == null)
			{
				return string.Empty;
			}
			string valueOrDefault = participant.GetValueOrDefault<string>(ParticipantSchema.SmtpAddress);
			string displayName = participant.DisplayName;
			return SearchParticipant.ToParticipantString(valueOrDefault, displayName);
		}

		public static string ToParticipantString(string smtpAddress, string displayName)
		{
			if (string.IsNullOrEmpty(smtpAddress) && string.IsNullOrEmpty(displayName))
			{
				return string.Empty;
			}
			return string.Format("{0} {1} {2}", smtpAddress, '|', displayName);
		}

		public static SearchParticipant FromParticipantString(string participantString)
		{
			if (string.IsNullOrEmpty(participantString))
			{
				return null;
			}
			string[] array = participantString.Split(SearchParticipant.Delimiters, 2);
			if (array.Length != 2)
			{
				return null;
			}
			SearchParticipant searchParticipant = new SearchParticipant();
			searchParticipant.SmtpAddress = (string.IsNullOrWhiteSpace(array[0]) ? null : array[0].Trim().ToLowerInvariant());
			searchParticipant.DisplayName = (string.IsNullOrWhiteSpace(array[1]) ? null : array[1].Trim());
			searchParticipant.SetRoutingType();
			return searchParticipant;
		}

		public override string ToString()
		{
			return SearchParticipant.ToParticipantString(this.SmtpAddress, this.DisplayName);
		}

		private void SetRoutingType()
		{
			this.RoutingType = (string.IsNullOrEmpty(this.SmtpAddress) ? null : "SMTP");
		}

		private const string SearchParticipantStringTemplate = "{0} {1} {2}";

		private const char Delimiter = '|';

		private static readonly char[] Delimiters = new char[]
		{
			'|'
		};
	}
}
