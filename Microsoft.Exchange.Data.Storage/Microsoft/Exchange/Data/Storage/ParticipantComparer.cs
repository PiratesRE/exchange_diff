using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ParticipantComparer
	{
		public static IEqualityComparer<IParticipant> EmailAddress
		{
			get
			{
				return ParticipantComparer.EmailAddressEqualityComparerImpl.Default;
			}
		}

		public static IEqualityComparer<IParticipant> SmtpEmailAddress
		{
			get
			{
				return ParticipantComparer.SmtpEmailAddressEqualityComparerImpl.Default;
			}
		}

		public static IEqualityComparer<Participant> EmailAddressIgnoringRoutingType
		{
			get
			{
				return ParticipantComparer.EmailAddressIgnoringRoutingTypeEqualityComparerImpl.Default;
			}
		}

		public static IComparer<Participant> DisplayName
		{
			get
			{
				return ParticipantComparer.DisplayNameComparerImpl.Default;
			}
		}

		private class EmailAddressIgnoringRoutingTypeEqualityComparerImpl : IEqualityComparer<Participant>
		{
			public bool Equals(Participant x, Participant y)
			{
				return object.ReferenceEquals(x, y) || (x != null && y != null && x.EmailAddressEqualityComparer.Equals(x, y));
			}

			public int GetHashCode(Participant x)
			{
				if (!(x != null))
				{
					return 0;
				}
				return x.EmailAddressEqualityComparer.GetHashCode(x);
			}

			public static ParticipantComparer.EmailAddressIgnoringRoutingTypeEqualityComparerImpl Default = new ParticipantComparer.EmailAddressIgnoringRoutingTypeEqualityComparerImpl();
		}

		private class EmailAddressEqualityComparerImpl : IEqualityComparer<IParticipant>
		{
			public bool Equals(IParticipant x, IParticipant y)
			{
				return object.ReferenceEquals(x, y) || (x != null && y != null && x.RoutingType == y.RoutingType && x.EmailAddressEqualityComparer.Equals(x, y));
			}

			public int GetHashCode(IParticipant x)
			{
				if (x == null)
				{
					return 0;
				}
				return x.EmailAddressEqualityComparer.GetHashCode(x);
			}

			public static ParticipantComparer.EmailAddressEqualityComparerImpl Default = new ParticipantComparer.EmailAddressEqualityComparerImpl();
		}

		private class SmtpEmailAddressEqualityComparerImpl : IEqualityComparer<IParticipant>
		{
			public int GetHashCode(IParticipant x)
			{
				string smtpAddress = this.GetSmtpAddress(x);
				if (smtpAddress == null)
				{
					return 0;
				}
				return StringComparer.OrdinalIgnoreCase.GetHashCode(smtpAddress);
			}

			private string GetSmtpAddress(IParticipant x)
			{
				if (x == null)
				{
					return null;
				}
				if (x.SmtpEmailAddress != null)
				{
					return x.SmtpEmailAddress;
				}
				if (!(x.RoutingType == "SMTP"))
				{
					return null;
				}
				return x.EmailAddress;
			}

			public bool Equals(IParticipant x, IParticipant y)
			{
				return object.ReferenceEquals(x, y) || this.CompareSmtpAddress(x, y);
			}

			private bool CompareSmtpAddress(IParticipant x, IParticipant y)
			{
				string smtpAddress = this.GetSmtpAddress(x);
				string smtpAddress2 = this.GetSmtpAddress(y);
				return smtpAddress != null && smtpAddress2 != null && SmtpRoutingTypeDriver.SmtpAddressEqualityComparer.Equals(smtpAddress, smtpAddress2);
			}

			public static ParticipantComparer.SmtpEmailAddressEqualityComparerImpl Default = new ParticipantComparer.SmtpEmailAddressEqualityComparerImpl();
		}

		private class DisplayNameComparerImpl : IComparer<Participant>
		{
			public int Compare(Participant x, Participant y)
			{
				return StringComparer.OrdinalIgnoreCase.Compare(x.DisplayName, y.DisplayName);
			}

			public static ParticipantComparer.DisplayNameComparerImpl Default = new ParticipantComparer.DisplayNameComparerImpl();
		}
	}
}
