using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Speech.Recognition;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaRecognizedFeature : IUMRecognizedFeature
	{
		private UcmaRecognizedFeature(UcmaRecognizedFeature.SemanticValueBase semanticValue, int firstWordOffset, ICollection<UcmaReplacementText> replacementWordUnits)
		{
			this.name = semanticValue.Name;
			this.value = semanticValue.Value;
			this.firstWordIndex = semanticValue.FirstWordIndex;
			this.countOfWords = semanticValue.CountOfWords;
			List<UcmaReplacementText> list = new List<UcmaReplacementText>(1);
			List<UcmaReplacementText> list2 = new List<UcmaReplacementText>(10);
			foreach (UcmaReplacementText ucmaReplacementText in replacementWordUnits)
			{
				if (ucmaReplacementText.FirstWordIndex < this.firstWordIndex && this.firstWordIndex < ucmaReplacementText.FirstWordIndex + ucmaReplacementText.CountOfWords)
				{
					this.countOfWords += this.firstWordIndex - ucmaReplacementText.FirstWordIndex;
					this.firstWordIndex = ucmaReplacementText.FirstWordIndex;
				}
				if (ucmaReplacementText.FirstWordIndex < this.firstWordIndex + this.countOfWords && this.firstWordIndex + this.countOfWords < ucmaReplacementText.FirstWordIndex + ucmaReplacementText.CountOfWords)
				{
					this.countOfWords += ucmaReplacementText.FirstWordIndex + ucmaReplacementText.CountOfWords - (this.firstWordIndex + this.countOfWords);
				}
				if (this.firstWordIndex <= ucmaReplacementText.FirstWordIndex)
				{
					if (ucmaReplacementText.FirstWordIndex + ucmaReplacementText.CountOfWords > this.firstWordIndex + this.countOfWords)
					{
						break;
					}
					list.Add(ucmaReplacementText);
				}
				else
				{
					list2.Add(ucmaReplacementText);
				}
			}
			foreach (UcmaReplacementText ucmaReplacementText2 in list)
			{
				this.countOfWords -= ucmaReplacementText2.CountOfWords - 1;
			}
			foreach (UcmaReplacementText ucmaReplacementText3 in list2)
			{
				this.firstWordIndex -= ucmaReplacementText3.CountOfWords - 1;
			}
			this.firstWordIndex += firstWordOffset;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public int FirstWordIndex
		{
			get
			{
				return this.firstWordIndex;
			}
		}

		public int CountOfWords
		{
			get
			{
				return this.countOfWords;
			}
		}

		internal static bool TryCreate(KeyValuePair<string, SemanticValue> fragment, int firstWordOffset, ICollection<UcmaReplacementText> replacementWordUnits, out UcmaRecognizedFeature newFeature)
		{
			UcmaRecognizedFeature.SemanticValueBase semanticValue;
			if (UcmaRecognizedFeature.SemanticValueBase.TryCreate(fragment.Value, out semanticValue))
			{
				newFeature = new UcmaRecognizedFeature(semanticValue, firstWordOffset, replacementWordUnits);
			}
			else
			{
				newFeature = null;
			}
			return newFeature != null;
		}

		public static string ParseName(SemanticValue semanticValue)
		{
			return UcmaRecognizedFeature.GetAttributeString(semanticValue, "name");
		}

		public static int ParseFirstWordIndex(SemanticValue semanticValue)
		{
			string attributeString = UcmaRecognizedFeature.GetAttributeString(semanticValue, "FirstWordIndex");
			return int.Parse(attributeString, NumberFormatInfo.InvariantInfo);
		}

		public static int ParseWordCount(SemanticValue semanticValue)
		{
			string attributeString = UcmaRecognizedFeature.GetAttributeString(semanticValue, "CountOfWords");
			return int.Parse(attributeString, NumberFormatInfo.InvariantInfo);
		}

		public static string ParsePhoneNumberSemanticValue(SemanticValue semanticValue)
		{
			return UcmaRecognizedFeature.PhoneNumberSemanticValue.ParseValue(semanticValue);
		}

		private static string GetAttributeString(SemanticValue semanticValue, string attributeName)
		{
			SemanticValue semanticValue2 = semanticValue["_attributes"];
			return (string)semanticValue2[attributeName].Value;
		}

		private readonly string name;

		private readonly int firstWordIndex;

		private readonly string value;

		private readonly int countOfWords;

		private class SemanticValueBase
		{
			protected SemanticValueBase(SemanticValue semanticValue)
			{
				this.firstWordIndex = UcmaRecognizedFeature.ParseFirstWordIndex(semanticValue);
				this.countOfWords = UcmaRecognizedFeature.ParseWordCount(semanticValue);
			}

			internal int FirstWordIndex
			{
				get
				{
					return this.firstWordIndex;
				}
			}

			internal int CountOfWords
			{
				get
				{
					return this.countOfWords;
				}
			}

			protected internal string Name
			{
				get
				{
					return this.name;
				}
				protected set
				{
					this.name = value;
				}
			}

			protected internal string Value
			{
				get
				{
					return this.value;
				}
				protected set
				{
					this.value = value;
				}
			}

			internal static bool TryCreate(SemanticValue semanticValue, out UcmaRecognizedFeature.SemanticValueBase newSemanticValue)
			{
				newSemanticValue = null;
				string a;
				if ((a = UcmaRecognizedFeature.ParseName(semanticValue)) != null)
				{
					if (!(a == "PhoneNumber"))
					{
						if (!(a == "PersonName"))
						{
							return false;
						}
						newSemanticValue = new UcmaRecognizedFeature.PersonNameSemanticValue(semanticValue);
					}
					else
					{
						newSemanticValue = new UcmaRecognizedFeature.PhoneNumberSemanticValue(semanticValue);
					}
					return null != newSemanticValue;
				}
				return false;
			}

			private readonly int firstWordIndex;

			private readonly int countOfWords;

			private string name = string.Empty;

			private string value = string.Empty;
		}

		private class PhoneNumberSemanticValue : UcmaRecognizedFeature.SemanticValueBase
		{
			internal PhoneNumberSemanticValue(SemanticValue semanticValue) : base(semanticValue)
			{
				base.Name = "PhoneNumber";
				base.Value = UcmaRecognizedFeature.PhoneNumberSemanticValue.ParseValue(semanticValue);
			}

			internal static string ParseValue(SemanticValue semanticValue)
			{
				string result = string.Empty;
				if (semanticValue.ContainsKey("PhoneNumber"))
				{
					result = (string)semanticValue["PhoneNumber"].Value;
				}
				else
				{
					string str = string.Empty;
					string str2 = string.Empty;
					string str3 = string.Empty;
					if (semanticValue.ContainsKey("AreaCode"))
					{
						str = (string)semanticValue["AreaCode"].Value;
					}
					if (semanticValue.ContainsKey("LocalNumber"))
					{
						str2 = (string)semanticValue["LocalNumber"].Value;
					}
					if (semanticValue.ContainsKey("Extension"))
					{
						str3 = (string)semanticValue["Extension"].Value;
					}
					result = str + str2 + str3;
				}
				return result;
			}
		}

		private class DateSemanticValue : UcmaRecognizedFeature.SemanticValueBase
		{
			private DateSemanticValue(SemanticValue semanticValue, ExDateTime date) : base(semanticValue)
			{
				base.Name = "Date";
				base.Value = date.ToString("d", DateTimeFormatInfo.InvariantInfo);
			}

			internal new static bool TryCreate(SemanticValue semanticValue, out UcmaRecognizedFeature.SemanticValueBase dateSemantic)
			{
				dateSemantic = null;
				bool flag = bool.Parse((string)semanticValue["IsValidDate"].Value);
				if (flag)
				{
					int day = int.Parse((string)semanticValue["Day"].Value, NumberFormatInfo.InvariantInfo);
					int month = int.Parse((string)semanticValue["Month"].Value, NumberFormatInfo.InvariantInfo);
					int year = int.Parse((string)semanticValue["Year"].Value, NumberFormatInfo.InvariantInfo);
					ExDateTime date = new ExDateTime(ExTimeZone.UtcTimeZone, year, month, day);
					dateSemantic = new UcmaRecognizedFeature.DateSemanticValue(semanticValue, date);
				}
				return null != dateSemantic;
			}
		}

		private class TimeSemanticValue : UcmaRecognizedFeature.SemanticValueBase
		{
			internal TimeSemanticValue(SemanticValue semanticValue) : base(semanticValue)
			{
				base.Name = "Time";
				int hour = int.Parse((string)semanticValue["Hour"].Value, NumberFormatInfo.InvariantInfo);
				int minute = int.Parse((string)semanticValue["Minute"].Value, NumberFormatInfo.InvariantInfo);
				ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, ExDateTime.UtcNow.Year, ExDateTime.UtcNow.Month, ExDateTime.UtcNow.Day, hour, minute, 0);
				base.Value = exDateTime.ToString("t", DateTimeFormatInfo.InvariantInfo);
			}
		}

		private class PersonNameSemanticValue : UcmaRecognizedFeature.SemanticValueBase
		{
			internal PersonNameSemanticValue(SemanticValue semanticValue) : base(semanticValue)
			{
				if (semanticValue.ContainsKey("Mailbox"))
				{
					base.Name = "Mailbox";
					base.Value = (string)semanticValue["Mailbox"].Value;
					return;
				}
				if (semanticValue.ContainsKey("Contact"))
				{
					base.Name = "Contact";
					base.Value = (string)semanticValue["Contact"].Value;
					return;
				}
				base.Name = "PersonName";
				SemanticValue semanticValue2 = semanticValue["_attributes"];
				base.Value = (string)semanticValue2["text"].Value;
			}
		}
	}
}
