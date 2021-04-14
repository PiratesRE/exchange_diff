using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Speech.Recognition;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaRecognitionPhrase : UcmaRecognitionPhraseBase
	{
		internal UcmaRecognitionPhrase(RecognizedPhrase recognitionPhrase)
		{
			ValidateArgument.NotNull(recognitionPhrase, "recognitionPhrase");
			this.recognitionPhrase = recognitionPhrase;
		}

		public override float Confidence
		{
			get
			{
				return this.recognitionPhrase.Confidence;
			}
		}

		public override string Text
		{
			get
			{
				return this.recognitionPhrase.Text;
			}
		}

		public override int HomophoneGroupId
		{
			get
			{
				return this.recognitionPhrase.HomophoneGroupId;
			}
		}

		public override object this[string key]
		{
			get
			{
				if (this.recognitionPhrase.Semantics.ContainsKey(key))
				{
					return this.recognitionPhrase.Semantics[key].Value ?? string.Empty;
				}
				return null;
			}
		}

		protected override ReadOnlyCollection<RecognizedWordUnit> WordUnits
		{
			get
			{
				return this.recognitionPhrase.Words;
			}
		}

		public override string ToSml()
		{
			if (this.recognitionPhrase == null)
			{
				return string.Empty;
			}
			return this.recognitionPhrase.ConstructSmlFromSemantics().CreateNavigator().OuterXml;
		}

		private RecognizedPhrase recognitionPhrase;
	}
}
