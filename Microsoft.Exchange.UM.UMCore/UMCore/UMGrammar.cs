using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UMGrammar : UMGrammarBase
	{
		internal UMGrammar(string path, string ruleName, CultureInfo culture) : base(path, culture)
		{
			this.ruleName = ruleName;
		}

		internal UMGrammar(string path, string ruleName, CultureInfo culture, Uri baseUri, bool deleteFileAfterUse) : this(path, ruleName, culture)
		{
			this.baseUri = baseUri;
			this.deleteFileAfterUse = deleteFileAfterUse;
		}

		public Uri BaseUri
		{
			get
			{
				return this.baseUri;
			}
		}

		public bool DeleteFileAfterUse
		{
			get
			{
				return this.deleteFileAfterUse;
			}
		}

		internal string RuleName
		{
			get
			{
				return this.ruleName;
			}
		}

		internal string Script { get; set; }

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.RuleName.GetHashCode();
		}

		internal override bool Equals(UMGrammarBase umGrammarBase)
		{
			UMGrammar umgrammar = umGrammarBase as UMGrammar;
			return umgrammar != null && string.Equals(base.Path, umgrammar.Path, StringComparison.OrdinalIgnoreCase) && string.Equals(this.RuleName, umgrammar.RuleName, StringComparison.OrdinalIgnoreCase);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Path: {0}, Rule: {1}, BaseURI: '{2}'", new object[]
			{
				base.Path,
				this.RuleName,
				(this.BaseUri != null) ? this.BaseUri.ToString() : "<null>"
			});
		}

		private readonly bool deleteFileAfterUse;

		private string ruleName;

		private Uri baseUri;
	}
}
