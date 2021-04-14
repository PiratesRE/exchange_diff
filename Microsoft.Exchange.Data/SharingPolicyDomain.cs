using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class SharingPolicyDomain
	{
		public string Domain { get; private set; }

		public SharingPolicyAction Actions { get; private set; }

		public SharingPolicyDomain(string domain, SharingPolicyAction actions)
		{
			domain = (StringComparer.OrdinalIgnoreCase.Equals(domain, "Anonymous") ? "Anonymous" : domain);
			SharingPolicyDomain.Validate(domain, actions);
			this.Domain = domain;
			this.Actions = actions;
		}

		public static SharingPolicyDomain Parse(string input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			string[] array = input.Split(new char[]
			{
				':'
			});
			if (array == null || array.Length != 2)
			{
				throw new StrongTypeFormatException(DataStrings.SharingPolicyDomainInvalid(input), string.Empty);
			}
			string domain = array[0].Trim();
			SharingPolicyAction actions;
			try
			{
				actions = (SharingPolicyAction)Enum.Parse(typeof(SharingPolicyAction), array[1], true);
			}
			catch (ArgumentException)
			{
				throw new StrongTypeFormatException(DataStrings.SharingPolicyDomainInvalidAction(array[1]), string.Empty);
			}
			return new SharingPolicyDomain(domain, actions);
		}

		public override bool Equals(object otherObject)
		{
			SharingPolicyDomain sharingPolicyDomain = otherObject as SharingPolicyDomain;
			return sharingPolicyDomain != null && this.Actions == sharingPolicyDomain.Actions && StringComparer.OrdinalIgnoreCase.Equals(this.Domain, sharingPolicyDomain.Domain);
		}

		public override int GetHashCode()
		{
			return this.Domain.GetHashCode();
		}

		public override string ToString()
		{
			return this.Domain + ":" + this.Actions.ToString();
		}

		private static void Validate(string domain, SharingPolicyAction actions)
		{
			if (!SmtpAddress.IsValidDomain(domain) && domain != "*" && domain != "Anonymous")
			{
				throw new StrongTypeFormatException(DataStrings.SharingPolicyDomainInvalidDomain(domain), "Domain");
			}
			if (domain == "Anonymous" && (actions & SharingPolicyAction.ContactsSharing) != (SharingPolicyAction)0)
			{
				throw new StrongTypeFormatException(DataStrings.SharingPolicyDomainInvalidActionForAnonymous(actions.ToString()), "Actions");
			}
			if (domain != "Anonymous" && ((actions & SharingPolicyAction.MeetingFullDetails) == SharingPolicyAction.MeetingFullDetails || (actions & SharingPolicyAction.MeetingLimitedDetails) == SharingPolicyAction.MeetingLimitedDetails || (actions & SharingPolicyAction.MeetingFullDetailsWithAttendees) == SharingPolicyAction.MeetingFullDetailsWithAttendees))
			{
				throw new StrongTypeFormatException(DataStrings.SharingPolicyDomainInvalidActionForDomain(actions.ToString()), "Actions");
			}
		}

		internal const string Asterisk = "*";

		internal const string Anonymous = "Anonymous";
	}
}
