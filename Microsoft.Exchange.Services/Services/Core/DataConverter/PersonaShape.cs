using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class PersonaShape : Shape
	{
		static PersonaShape()
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			foreach (PropertyInformation propertyInformation in PersonaShape.defaultProperties)
			{
				list.AddRange(propertyInformation.GetPropertyDefinitions(null));
			}
			PersonaShape.DefaultPropertyDefinitions = list.ToArray();
		}

		private PersonaShape(List<PropertyInformation> defaultProperties) : base(Schema.Persona, PersonaSchema.GetSchema(), null, defaultProperties)
		{
		}

		internal static PersonaShape CreateShape()
		{
			return new PersonaShape(PersonaShape.defaultProperties);
		}

		private static readonly List<PropertyInformation> defaultProperties = new List<PropertyInformation>
		{
			PersonaSchema.PersonaId,
			PersonaSchema.ADObjectId,
			PersonaSchema.PersonaType,
			PersonaSchema.EmailAddress,
			PersonaSchema.CompanyName,
			PersonaSchema.DisplayName,
			PersonaSchema.DisplayNameFirstLast,
			PersonaSchema.DisplayNameLastFirst,
			PersonaSchema.GivenName,
			PersonaSchema.Surname,
			PersonaSchema.EmailAddresses,
			PersonaSchema.ImAddress,
			PersonaSchema.FileAs,
			PersonaSchema.HomeCity,
			PersonaSchema.WorkCity,
			PersonaSchema.CreationTime,
			PersonaSchema.IsFavorite
		};

		internal static readonly PropertyDefinition[] DefaultPropertyDefinitions;
	}
}
