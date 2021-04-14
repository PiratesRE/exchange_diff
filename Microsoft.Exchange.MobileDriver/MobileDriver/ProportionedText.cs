using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class ProportionedText
	{
		public ProportionedText(string text) : this(text, false)
		{
		}

		public ProportionedText(string text, bool isOptional) : this(text, string.IsNullOrEmpty(text) ? 0 : text.Length, string.IsNullOrEmpty(text) ? 0 : text.Length, 0)
		{
			this.IsOptional = isOptional;
		}

		public ProportionedText(string text, int lengthReservedAtLeast, int lengthKeptAtMost, int weight)
		{
			if (0 > lengthReservedAtLeast)
			{
				throw new ArgumentOutOfRangeException("lengthReservedAtLeast");
			}
			if (0 > lengthKeptAtMost)
			{
				throw new ArgumentOutOfRangeException("lengthKeptAtMost");
			}
			if (0 > weight)
			{
				throw new ArgumentOutOfRangeException("weight");
			}
			if (lengthReservedAtLeast > lengthKeptAtMost)
			{
				throw new ArgumentOutOfRangeException("lengthReservedAtLeast");
			}
			this.Text = (text ?? string.Empty);
			this.LengthReservedAtLeast = Math.Min(this.Text.Length, lengthReservedAtLeast);
			this.LengthKeptAtMost = Math.Min(this.Text.Length, lengthKeptAtMost);
			this.Weight = weight;
			this.IsOptional = false;
			if (this.LengthReservedAtLeast == this.Text.Length)
			{
				this.Type = ProportionedText.ProportionedTextType.OriginalCopy;
				return;
			}
			if (this.LengthReservedAtLeast == this.LengthKeptAtMost)
			{
				this.Type = ProportionedText.ProportionedTextType.FixedTruncation;
				this.Text = this.Text.Substring(0, this.LengthKeptAtMost);
				return;
			}
			this.Type = ProportionedText.ProportionedTextType.WeightedProportion;
		}

		private ProportionedText(string text, ProportionedText.ProportionedTextType type) : this(text, false)
		{
			this.Type = type;
		}

		public ProportionedText.ProportionedTextType Type { get; private set; }

		public int Weight { get; private set; }

		public int LengthReservedAtLeast { get; private set; }

		public int LengthKeptAtMost { get; private set; }

		public string Text { get; private set; }

		public bool IsOptional { get; private set; }

		public ProportionedText.PresentationText ToPresentationText(int lengthKept, int groupId)
		{
			if (ProportionedText.ProportionedTextType.WeightedProportion != this.Type)
			{
				return new ProportionedText.PresentationText(this.Text, ProportionedText.ProportionedTextType.FixedTruncation == this.Type, groupId);
			}
			lengthKept = Math.Max(lengthKept, this.LengthReservedAtLeast);
			lengthKept = Math.Min(lengthKept, this.LengthKeptAtMost);
			return new ProportionedText.PresentationText(this.Text.Substring(0, lengthKept), lengthKept < this.Text.Length, groupId);
		}

		public ProportionedText.PresentationText ToPresentationText(int totalWeight, int capacityForWeightedText, int groupId)
		{
			if (ProportionedText.ProportionedTextType.WeightedProportion != this.Type)
			{
				return new ProportionedText.PresentationText(this.Text, ProportionedText.ProportionedTextType.FixedTruncation == this.Type, groupId);
			}
			if (0 > totalWeight || this.Weight > totalWeight)
			{
				throw new ArgumentOutOfRangeException("totalWeight");
			}
			if (0 > capacityForWeightedText)
			{
				throw new ArgumentOutOfRangeException("capacityForWeightedText");
			}
			if (totalWeight == 0)
			{
				return new ProportionedText.PresentationText(string.Empty, false, groupId);
			}
			return this.ToPresentationText(capacityForWeightedText * this.Weight / totalWeight, groupId);
		}

		public override string ToString()
		{
			return this.Text;
		}

		public static readonly ProportionedText Delimiter = new ProportionedText(" ", ProportionedText.ProportionedTextType.Delimiter);

		internal enum ProportionedTextType
		{
			OriginalCopy,
			FixedTruncation,
			WeightedProportion,
			Delimiter
		}

		public class PresentationText
		{
			public PresentationText(string text, bool needEllipsisTrail, int groupId)
			{
				this.Text = text;
				this.NeedEllipsisTrail = needEllipsisTrail;
				this.GroupId = groupId;
			}

			public string Text { get; private set; }

			public bool NeedEllipsisTrail { get; private set; }

			public int GroupId { get; private set; }

			public override string ToString()
			{
				return this.ToString(CodingScheme.Unicode);
			}

			public string ToString(CodingScheme codingScheme)
			{
				if (codingScheme == CodingScheme.Neutral)
				{
					throw new ArgumentOutOfRangeException("codingScheme");
				}
				if (!this.NeedEllipsisTrail)
				{
					return this.Text;
				}
				return new EllipsisTrailer(codingScheme).Trail(this.Text);
			}
		}

		public class PresentationContent
		{
			public bool IsGrouped { get; private set; }

			public PresentationContent(IList<ProportionedText.PresentationText> presentationText)
			{
				if (presentationText == null)
				{
					throw new ArgumentNullException("texts");
				}
				this.PresentationTexts = presentationText;
				this.IsGrouped = false;
				for (int i = 0; i < presentationText.Count; i++)
				{
					if (presentationText[i].GroupId > 0)
					{
						this.IsGrouped = true;
						return;
					}
				}
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder(this.PresentationTexts.Count);
				int num = 0;
				int num2 = 0;
				while (this.PresentationTexts.Count > num2)
				{
					if (this.PresentationTexts[num2].GroupId != num)
					{
						num = this.PresentationTexts[num2].GroupId;
					}
					else
					{
						stringBuilder.Append(this.PresentationTexts[num2].Text);
					}
					num2++;
				}
				return stringBuilder.ToString();
			}

			public IList<ProportionedText.PresentationText> PresentationTexts;
		}
	}
}
