using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Speech.Recognition;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaRecognizedWordUnit : IUMRecognizedWord
	{
		internal UcmaRecognizedWordUnit(RecognizedWordUnit recognizedWordUnit, TimeSpan transcriptionResultAudioPosition)
		{
			this.audioPosition = transcriptionResultAudioPosition + recognizedWordUnit.AudioPosition;
			this.audioDuration = recognizedWordUnit.AudioDuration;
			this.text = recognizedWordUnit.Text;
			this.displayAttributes = recognizedWordUnit.DisplayAttributes;
			this.confidence = recognizedWordUnit.Confidence;
		}

		internal UcmaRecognizedWordUnit(UcmaReplacementText replacementText, ReadOnlyCollection<RecognizedWordUnit> recognizedWords, TimeSpan transcriptionResultAudioPosition)
		{
			float num = 0f;
			int firstWordIndex = replacementText.FirstWordIndex;
			RecognizedWordUnit recognizedWordUnit = recognizedWords[firstWordIndex];
			RecognizedWordUnit recognizedWordUnit2 = recognizedWords[firstWordIndex + replacementText.CountOfWords - 1];
			this.audioPosition = recognizedWordUnit.AudioPosition + transcriptionResultAudioPosition;
			this.audioDuration = recognizedWordUnit2.AudioPosition + recognizedWordUnit2.AudioDuration - recognizedWordUnit.AudioPosition;
			for (int i = 0; i < replacementText.CountOfWords; i++)
			{
				RecognizedWordUnit recognizedWordUnit3 = recognizedWords[firstWordIndex++];
				num += recognizedWordUnit3.Confidence;
			}
			this.confidence = ((replacementText.CountOfWords > 0) ? (num / (float)replacementText.CountOfWords) : 0f);
			this.text = replacementText.Text;
			this.displayAttributes = replacementText.DisplayAttributes;
		}

		public float Confidence
		{
			get
			{
				return this.confidence;
			}
		}

		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		public UMDisplayAttributes DisplayAttributes
		{
			get
			{
				return this.displayAttributes;
			}
			set
			{
				this.displayAttributes = value;
			}
		}

		public TimeSpan AudioPosition
		{
			get
			{
				return this.audioPosition;
			}
		}

		public TimeSpan AudioDuration
		{
			get
			{
				return this.audioDuration;
			}
		}

		private readonly TimeSpan audioPosition;

		private readonly TimeSpan audioDuration;

		private readonly float confidence;

		private string text;

		private DisplayAttributes displayAttributes;
	}
}
