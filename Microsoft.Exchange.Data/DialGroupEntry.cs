using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class DialGroupEntry
	{
		public DialGroupEntry(string name, string mask, string number, string comment)
		{
			this.name = DialGroupEntryName.Parse(name);
			this.numberMask = DialGroupEntryNumberMask.Parse(mask);
			this.dialedNumber = DialGroupEntryDialedNumber.Parse(number);
			this.comment = DialGroupEntryComment.Parse(comment);
			this.Validate();
		}

		public DialGroupEntry(PSObject importedObject)
		{
			try
			{
				this.name = DialGroupEntryName.Parse((string)importedObject.Properties["Name"].Value);
				this.numberMask = DialGroupEntryNumberMask.Parse((string)importedObject.Properties["NumberMask"].Value);
				this.dialedNumber = DialGroupEntryDialedNumber.Parse((string)importedObject.Properties["DialedNumber"].Value);
				this.comment = DialGroupEntryComment.Parse((string)importedObject.Properties["Comment"].Value);
			}
			catch (NullReferenceException)
			{
				throw new FormatException(DataStrings.InvalidDialGroupEntryCsvFormat);
			}
			this.Validate();
		}

		private void Validate()
		{
			if (this.NumberMask.IndexOf('*') != -1 && this.DialedNumber.IndexOf('x') != -1)
			{
				throw new FormatException(DataStrings.InvalidDialledNumberFormatA);
			}
			if (string.CompareOrdinal(this.NumberMask, "*") == 0 && !DialGroupEntry.digitRegex.IsMatch(this.DialedNumber) && string.CompareOrdinal(this.DialedNumber, "*") != 0)
			{
				throw new FormatException(DataStrings.InvalidDialledNumberFormatB);
			}
			if (DialGroupEntry.digitRegex.IsMatch(this.NumberMask) && string.CompareOrdinal(this.DialedNumber, "*") == -1 && !DialGroupEntry.digitRegex.IsMatch(this.DialedNumber))
			{
				throw new FormatException(DataStrings.InvalidDialledNumberFormatC);
			}
			if (this.NumberMask.IndexOf('x') == -1 && this.DialedNumber.IndexOf('x') != -1)
			{
				throw new FormatException(DataStrings.InvalidDialledNumberFormatD);
			}
		}

		public string Name
		{
			get
			{
				return this.name.ToString();
			}
		}

		public string NumberMask
		{
			get
			{
				return this.numberMask.ToString();
			}
		}

		public string DialedNumber
		{
			get
			{
				return this.dialedNumber.ToString();
			}
		}

		public string Comment
		{
			get
			{
				return this.comment.ToString();
			}
		}

		public static DialGroupEntry Parse(string line)
		{
			if (string.IsNullOrEmpty(line))
			{
				return null;
			}
			line = line.Trim();
			string[] array = line.Split(new char[]
			{
				','
			});
			if (array.Length < 3 || array.Length > 4)
			{
				throw new FormatException(DataStrings.InvalidDialGroupEntryFormat);
			}
			return new DialGroupEntry(array[0], array[1], array[2], (array.Length > 3) ? array[3] : null);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(this.Comment))
			{
				stringBuilder.AppendFormat("{0},{1},{2},{3}", new object[]
				{
					this.Name,
					this.NumberMask,
					this.DialedNumber,
					this.Comment
				});
			}
			else
			{
				stringBuilder.AppendFormat("{0},{1},{2}", this.Name, this.NumberMask, this.DialedNumber);
			}
			return stringBuilder.ToString();
		}

		public static bool ValidateGroup(ICollection<DialGroupEntry> configuredGroups, ICollection<string> allowedGroups, bool inCountryGroup, out LocalizedString LocalizedError)
		{
			LocalizedError = LocalizedString.Empty;
			string group = inCountryGroup ? "ConfiguredInCountryOrRegionGroups" : "ConfiguredInternationalGroups";
			if (allowedGroups == null || allowedGroups.Count == 0)
			{
				return true;
			}
			if (allowedGroups.Count > 0 && (configuredGroups == null || configuredGroups.Count == 0))
			{
				LocalizedError = DataStrings.DialGroupNotSpecifiedOnDialPlan(group);
				return false;
			}
			foreach (string strA in allowedGroups)
			{
				bool flag = false;
				foreach (DialGroupEntry dialGroupEntry in configuredGroups)
				{
					if (string.Compare(strA, dialGroupEntry.Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					LocalizedError = DataStrings.DialGroupNotSpecifiedOnDialPlanB(strA, group);
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			DialGroupEntry dialGroupEntry = obj as DialGroupEntry;
			return dialGroupEntry != null && this.ToString().Equals(dialGroupEntry.ToString(), StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return this.ToString().ToLowerInvariant().GetHashCode();
		}

		private const char Wildcard = '*';

		private const char DigitPlaceholder = 'x';

		private DialGroupEntryName name;

		private DialGroupEntryNumberMask numberMask;

		private DialGroupEntryDialedNumber dialedNumber;

		private DialGroupEntryComment comment;

		private static Regex digitRegex = new Regex("^\\d+$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
	}
}
