using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ContentFilterPhrase : ConfigurableObject
	{
		public ContentFilterPhrase() : base(new SimpleProviderPropertyBag())
		{
			this.Phrase = string.Empty;
			this.Influence = Influence.GoodWord;
		}

		private ContentFilterPhrase(string phrase, Influence influence) : base(new SimpleProviderPropertyBag())
		{
			this.Phrase = phrase;
			this.Influence = influence;
		}

		public Influence Influence
		{
			get
			{
				return (Influence)this.propertyBag[ContentFilterPhraseSchema.Influence];
			}
			internal set
			{
				this.propertyBag[ContentFilterPhraseSchema.Influence] = value;
			}
		}

		public string Phrase
		{
			get
			{
				return (string)this.propertyBag[ContentFilterPhraseSchema.Phrase];
			}
			internal set
			{
				this.propertyBag[ContentFilterPhraseSchema.Phrase] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new ContentFilterPhraseIdentity(this.Phrase);
			}
		}

		public override string ToString()
		{
			return this.Phrase;
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			int num = 0;
			if (!string.IsNullOrEmpty(this.Phrase))
			{
				this.Phrase = this.Phrase.Trim();
				num = this.Phrase.Length;
			}
			if (!Enum.IsDefined(typeof(Influence), this.Influence))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.InvalidInfluence(this.Influence), null, this.Influence));
				return;
			}
			if (num < 1 || num > 256)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.InvalidPhrase((this.Influence == Influence.BadWord) ? "blocking" : "non-blocking", 256), null, this.Phrase));
			}
		}

		internal static ContentFilterPhrase Decode(string encoded)
		{
			int num = encoded.IndexOf(';');
			if (num < 0)
			{
				throw new FormatException("Encoded string is invalid: " + encoded);
			}
			int num2 = int.Parse(encoded.Substring(0, num), CultureInfo.InvariantCulture);
			if (!Enum.IsDefined(typeof(Influence), num2))
			{
				throw new FormatException("Encoded string is invalid: " + encoded);
			}
			return new ContentFilterPhrase(encoded.Substring(num + 1), (Influence)num2);
		}

		internal string Encode()
		{
			return ((int)this.Influence).ToString(CultureInfo.InvariantCulture) + ';' + this.Phrase;
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ContentFilterPhrase.schema;
			}
		}

		private const char Delimiter = ';';

		private const int MaxLength = 256;

		private static ObjectSchema schema = ObjectSchema.GetInstance<ContentFilterPhraseSchema>();
	}
}
