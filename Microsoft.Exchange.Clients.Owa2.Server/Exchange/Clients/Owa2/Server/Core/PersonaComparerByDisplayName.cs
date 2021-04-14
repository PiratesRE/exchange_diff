using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class PersonaComparerByDisplayName : IComparer<Persona>
	{
		public PersonaComparerByDisplayName(CultureInfo culture)
		{
			this.culture = culture;
		}

		public int Compare(Persona persona1, Persona persona2)
		{
			return string.Compare(persona1.DisplayName, persona2.DisplayName, this.culture, CompareOptions.None);
		}

		private readonly CultureInfo culture;
	}
}
