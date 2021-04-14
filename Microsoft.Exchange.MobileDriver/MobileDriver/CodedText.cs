using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class CodedText
	{
		public CodedText(CodingScheme codingScheme, string text, IList<int> radixCountMap)
		{
			if (codingScheme == CodingScheme.Neutral)
			{
				throw new ArgumentOutOfRangeException("codingScheme");
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException("text");
			}
			if (radixCountMap == null || text.Length != radixCountMap.Count)
			{
				throw new ArgumentOutOfRangeException("radixCountMap");
			}
			this.CodingScheme = codingScheme;
			this.Text = text;
			this.RadixCountMap = radixCountMap;
		}

		public bool CanBeCodedEntirely
		{
			get
			{
				if (this.canBeCodedEntirely == null)
				{
					this.canBeCodedEntirely = new bool?(true);
					using (IEnumerator<int> enumerator = this.RadixCountMap.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current == 0)
							{
								this.canBeCodedEntirely = new bool?(false);
								break;
							}
						}
					}
				}
				return this.canBeCodedEntirely.Value;
			}
		}

		public CodingScheme CodingScheme { get; private set; }

		public string Text { get; set; }

		private IList<int> RadixCountMap { get; set; }

		public override string ToString()
		{
			return this.Text;
		}

		public int GetRadixCount(int index)
		{
			if (0 > index || this.Text.Length <= index)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return this.RadixCountMap[index];
		}

		public void ReplaceUncodableCharacters(char fallbackCharacter)
		{
			if (this.CanBeCodedEntirely)
			{
				return;
			}
			if (CodingSchemeInfo.GetCodingSchemeInfo(this.CodingScheme).Coder.GetCodedRadixCount(fallbackCharacter) == 0)
			{
				throw new ArgumentOutOfRangeException("fallbackCharacter");
			}
			StringBuilder stringBuilder = new StringBuilder(this.Text.Length);
			int num = 0;
			while (this.RadixCountMap.Count > num)
			{
				stringBuilder.Append((0 < this.RadixCountMap[num]) ? this.Text[num] : fallbackCharacter);
				num++;
			}
			this.Text = stringBuilder.ToString();
		}

		public const char DefaultFallbackCharacter = '?';

		private bool? canBeCodedEntirely;
	}
}
