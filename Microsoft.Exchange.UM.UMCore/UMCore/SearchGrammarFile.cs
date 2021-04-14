using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class SearchGrammarFile
	{
		protected SearchGrammarFile(CultureInfo culture) : this(culture, false)
		{
		}

		protected SearchGrammarFile(CultureInfo culture, bool compiled)
		{
			this.culture = culture;
			this.compiled = compiled;
		}

		public virtual Uri BaseUri
		{
			get
			{
				return null;
			}
		}

		internal CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
			set
			{
				this.culture = value;
			}
		}

		internal bool Compiled
		{
			get
			{
				return this.compiled;
			}
			set
			{
				this.compiled = value;
			}
		}

		internal abstract string FilePath { get; }

		internal abstract bool HasEntries { get; }

		protected string Extension
		{
			get
			{
				if (!this.compiled)
				{
					return ".grxml";
				}
				return ".cfg";
			}
		}

		private CultureInfo culture;

		private bool compiled;
	}
}
