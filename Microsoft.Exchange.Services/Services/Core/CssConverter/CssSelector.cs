using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.CssConverter
{
	public class CssSelector
	{
		public CssSelector()
		{
			this.Tag = string.Empty;
			this.Id = string.Empty;
			this.PseudoClass = string.Empty;
			this.Classes = new List<string>();
		}

		public CssSelector(string selectorString) : this()
		{
			this.selectorString = selectorString;
			this.Parse(selectorString);
		}

		public string Tag { get; private set; }

		public string Id { get; private set; }

		public string PseudoClass { get; private set; }

		public IList<string> Classes { get; private set; }

		public bool IsDirective
		{
			get
			{
				return this.selectorString.StartsWith("@");
			}
		}

		public bool ContainsUnsafePseudoClasses
		{
			get
			{
				return !string.IsNullOrEmpty(this.PseudoClass) && !CssSelector.SafePseudoClasses.Contains(this.PseudoClass);
			}
		}

		public override string ToString()
		{
			return this.selectorString;
		}

		public void PrependClass(string className)
		{
			this.selectorString = string.Format(".{0} {1}", className, this.selectorString);
		}

		private void Parse(string selectorString)
		{
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			for (int i = 0; i < selectorString.Length; i++)
			{
				char c = selectorString[i];
				if (c != '#')
				{
					if (c != '.')
					{
						if (c == ':')
						{
							num3 = i;
						}
					}
					else if (num2 == -1)
					{
						num2 = i;
					}
				}
				else
				{
					num = i;
				}
			}
			int[] array = new int[]
			{
				num,
				num2,
				num3,
				selectorString.Length
			};
			Array.Sort<int>(array);
			int num4 = -1;
			int num5 = 0;
			while (num5 + 1 < array.Length)
			{
				if (array[num5] != -1)
				{
					if (num4 == -1)
					{
						num4 = array[num5];
					}
					int length = array[num5 + 1] - array[num5];
					string text = selectorString.Substring(array[num5], length);
					if (!string.IsNullOrEmpty(text))
					{
						if (array[num5] == num)
						{
							this.Id = text.Substring(1);
						}
						else if (array[num5] == num2)
						{
							this.Classes = CssParser.SplitToList(text, CssParser.ClassDelimiter, int.MaxValue);
						}
						else if (array[num5] == num3)
						{
							this.PseudoClass = text.Substring(1);
						}
					}
				}
				num5++;
			}
			if (num4 == -1)
			{
				num4 = selectorString.Length;
			}
			this.Tag = selectorString.Substring(0, num4).ToLowerInvariant();
		}

		private static readonly HashSet<string> SafePseudoClasses = new HashSet<string>
		{
			"after",
			"before",
			"empty",
			"first-child",
			"first-letter",
			"first-line",
			"first-of-type",
			"last-child",
			"last-of-type",
			"nth-child",
			"nth-last-child",
			"nth-last-of-type",
			"nth-of-type",
			"only-child",
			"only-of-type"
		};

		private string selectorString;
	}
}
