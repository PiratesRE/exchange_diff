using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal sealed class PrimarySmtpAddressProperty : SmartPropertyDefinition
	{
		internal PrimarySmtpAddressProperty() : base("PrimarySmtpAddressProperty", typeof(string), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, PrimarySmtpAddressProperty.dependantProps)
		{
		}

		private static PropertyDependency[] GetDependencies(ContactEmailSlotParticipantProperty[] sourceProps)
		{
			List<PropertyDependency> list = new List<PropertyDependency>();
			foreach (ContactEmailSlotParticipantProperty contactEmailSlotParticipantProperty in sourceProps)
			{
				if (contactEmailSlotParticipantProperty == null)
				{
					throw new InvalidProgramException("Initialization of sourceProps failed due to an ordering issue.");
				}
				foreach (PropertyDependency propertyDependency in contactEmailSlotParticipantProperty.Dependencies)
				{
					if ((propertyDependency.Type & PropertyDependencyType.NeedForRead) == PropertyDependencyType.NeedForRead)
					{
						list.Add(new PropertyDependency(propertyDependency.Property, PropertyDependencyType.NeedForRead));
					}
				}
			}
			return list.ToArray();
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			foreach (ContactEmailSlotParticipantProperty propertyDefinition in PrimarySmtpAddressProperty.sourceProps)
			{
				Participant participant = propertyBag.GetValue(propertyDefinition) as Participant;
				if (participant != null && participant.RoutingType == "EX")
				{
					string text = participant.TryGetProperty(ParticipantSchema.SmtpAddress) as string;
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
				}
			}
			return new PropertyError(this, PropertyErrorCode.NotFound);
		}

		private static ContactEmailSlotParticipantProperty[] sourceProps = new ContactEmailSlotParticipantProperty[]
		{
			InternalSchema.ContactEmail1,
			InternalSchema.ContactEmail2,
			InternalSchema.ContactEmail3
		};

		private static PropertyDependency[] dependantProps = PrimarySmtpAddressProperty.GetDependencies(PrimarySmtpAddressProperty.sourceProps);
	}
}
