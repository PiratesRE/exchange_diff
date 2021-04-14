using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IParticipant : IEquatable<IParticipant>
	{
		IEqualityComparer<IParticipant> EmailAddressEqualityComparer { get; }

		string DisplayName { get; }

		string OriginalDisplayName { get; }

		string EmailAddress { get; }

		string SmtpEmailAddress { get; }

		string RoutingType { get; }

		ParticipantOrigin Origin { get; }

		string SipUri { get; }

		bool Submitted { get; set; }

		ICollection<PropertyDefinition> LoadedProperties { get; }

		T GetValueOrDefault<T>(PropertyDefinition propertyDefinition);
	}
}
