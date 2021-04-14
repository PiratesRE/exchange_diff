using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DisambiguatedName
	{
		public DisambiguatedName(string name, string disambiguationText, DisambiguationFieldEnum disambiguationField)
		{
			this.name = name;
			this.disambiguationText = disambiguationText;
			this.disambiguationField = disambiguationField;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string DisambiguationText
		{
			get
			{
				return this.disambiguationText;
			}
		}

		public DisambiguationFieldEnum DisambiguationField
		{
			get
			{
				return this.disambiguationField;
			}
		}

		private string name;

		private string disambiguationText;

		private DisambiguationFieldEnum disambiguationField = DisambiguationFieldEnum.None;
	}
}
