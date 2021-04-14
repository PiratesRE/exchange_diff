using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class UMGrammarBase
	{
		protected UMGrammarBase(string path, CultureInfo culture)
		{
			this.path = path;
			this.culture = culture;
		}

		internal string Path
		{
			get
			{
				return this.path;
			}
		}

		internal CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
		}

		internal string ManifestFileGrammarNode
		{
			get
			{
				Uri uri = new Uri("file:///" + this.Path);
				return string.Format(CultureInfo.InvariantCulture, "<resource src=\"{0}\"/>", new object[]
				{
					uri.ToString()
				});
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			UMGrammarBase umgrammarBase = obj as UMGrammarBase;
			return umgrammarBase != null && this.Equals(umgrammarBase);
		}

		public override int GetHashCode()
		{
			return this.Path.GetHashCode();
		}

		internal abstract bool Equals(UMGrammarBase umGrammarBase);

		private string path;

		private CultureInfo culture;
	}
}
