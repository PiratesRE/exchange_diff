using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class UMLanguage : IEquatable<UMLanguage>, IComparable, IComparable<UMLanguage>
	{
		public string DisplayName
		{
			get
			{
				return this.localizedDisplayName;
			}
		}

		public string EnglishName
		{
			get
			{
				return this.englishName;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int LCID
		{
			get
			{
				return this.lcid;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
		}

		public UMLanguage(int lcid)
		{
			this.lcid = lcid;
			this.SetCultureInfo(null);
		}

		public UMLanguage(CultureInfo culture)
		{
			this.lcid = culture.LCID;
			this.SetCultureInfo(culture);
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			this.SetCultureInfo(null);
		}

		private void SetCultureInfo(CultureInfo culture)
		{
			if (this.lcid == 22538)
			{
				this.SetLatinAmericanSpanish();
				return;
			}
			if (culture == null)
			{
				culture = new CultureInfo(this.lcid);
			}
			this.SetPropsFromCulture(culture);
		}

		public static UMLanguage Parse(string language)
		{
			if (string.IsNullOrEmpty(language))
			{
				throw new ArgumentNullException(language);
			}
			if (string.Compare(language, "es-419", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return new UMLanguage(22538);
			}
			CultureInfo cultureInfo = new CultureInfo(language);
			return new UMLanguage(cultureInfo);
		}

		public override string ToString()
		{
			return this.name;
		}

		private void SetLatinAmericanSpanish()
		{
			this.localizedDisplayName = DataStrings.LatAmSpanish;
			this.englishName = "Spanish (Latin America)";
			this.name = "es-419";
			this.culture = new CultureInfo("es-mx");
		}

		private void SetPropsFromCulture(CultureInfo culture)
		{
			this.culture = culture;
			this.localizedDisplayName = culture.DisplayName;
			this.englishName = culture.EnglishName;
			this.name = culture.Name;
		}

		public override int GetHashCode()
		{
			return this.lcid.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is UMLanguage))
			{
				return false;
			}
			UMLanguage umlanguage = (UMLanguage)obj;
			return this.lcid == umlanguage.LCID;
		}

		public int CompareTo(UMLanguage other)
		{
			return this.lcid.CompareTo(other.LCID);
		}

		public int CompareTo(object obj)
		{
			if (!(obj is UMLanguage))
			{
				return -1;
			}
			UMLanguage umlanguage = (UMLanguage)obj;
			return this.lcid.CompareTo(umlanguage.LCID);
		}

		public bool Equals(UMLanguage other)
		{
			return this.lcid == other.LCID;
		}

		private const string LatinAmericanSpanishRfcName = "es-419";

		private const string LatinAmericanSpanish_EnglishName = "Spanish (Latin America)";

		private const int LatinAmericanSpanishLcid = 22538;

		private int lcid;

		[NonSerialized]
		private string localizedDisplayName;

		[NonSerialized]
		private string englishName;

		[NonSerialized]
		private string name;

		[NonSerialized]
		private CultureInfo culture;

		internal static UMLanguage DefaultLanguage = new UMLanguage(1033);

		internal static UMLanguage[] Datacenterlanguages = new UMLanguage[]
		{
			UMLanguage.Parse("en-US"),
			UMLanguage.Parse("ca-ES"),
			UMLanguage.Parse("da-DK"),
			UMLanguage.Parse("de-DE"),
			UMLanguage.Parse("en-AU"),
			UMLanguage.Parse("en-CA"),
			UMLanguage.Parse("en-GB"),
			UMLanguage.Parse("en-IN"),
			UMLanguage.Parse("es-ES"),
			UMLanguage.Parse("es-MX"),
			UMLanguage.Parse("fi-FI"),
			UMLanguage.Parse("fr-FR"),
			UMLanguage.Parse("fr-CA"),
			UMLanguage.Parse("it-IT"),
			UMLanguage.Parse("ja-JP"),
			UMLanguage.Parse("ko-KR"),
			UMLanguage.Parse("nl-NL"),
			UMLanguage.Parse("nb-NO"),
			UMLanguage.Parse("pl-PL"),
			UMLanguage.Parse("pt-BR"),
			UMLanguage.Parse("pt-PT"),
			UMLanguage.Parse("ru-RU"),
			UMLanguage.Parse("sv-SE"),
			UMLanguage.Parse("zh-CN"),
			UMLanguage.Parse("zh-TW"),
			UMLanguage.Parse("zh-HK")
		};
	}
}
