using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class SearchCriteriaText : SearchCriteria
	{
		protected SearchCriteriaText(Column lhs, SearchCriteriaText.SearchTextFullness fullnessFlags, SearchCriteriaText.SearchTextFuzzyLevel fuzzynessFlags, Column rhs)
		{
			this.lhs = lhs;
			this.fullnessFlags = fullnessFlags;
			this.fuzzynessFlags = fuzzynessFlags;
			this.rhs = rhs;
		}

		public Column Lhs
		{
			get
			{
				return this.lhs;
			}
		}

		public SearchCriteriaText.SearchTextFullness FullnessFlags
		{
			get
			{
				return this.fullnessFlags;
			}
		}

		public SearchCriteriaText.SearchTextFuzzyLevel FuzzynessFlags
		{
			get
			{
				return this.fuzzynessFlags;
			}
		}

		public Column Rhs
		{
			get
			{
				return this.rhs;
			}
		}

		public override bool Evaluate(ITWIR twir, CompareInfo compareInfo)
		{
			object obj = this.Lhs.Evaluate(twir);
			object obj2 = this.Rhs.Evaluate(twir);
			bool flag = (byte)(this.Lhs.ExtendedTypeCode & ExtendedTypeCode.MVFlag) == 16 && (this.Lhs.ExtendedTypeCode & (ExtendedTypeCode)239) == this.Rhs.ExtendedTypeCode;
			bool flag2 = (byte)(this.Rhs.ExtendedTypeCode & ExtendedTypeCode.MVFlag) == 16 && (this.Rhs.ExtendedTypeCode & (ExtendedTypeCode)239) == this.Lhs.ExtendedTypeCode;
			if (flag || flag2)
			{
				Array array;
				if (flag)
				{
					array = (Array)obj;
				}
				else
				{
					array = (Array)obj2;
				}
				int num = (array == null) ? 0 : array.Length;
				for (int i = 0; i < num; i++)
				{
					bool flag3;
					if (flag)
					{
						flag3 = this.EvaluateHelper((string)array.GetValue(i), (string)obj2, compareInfo);
					}
					else
					{
						flag3 = this.EvaluateHelper((string)obj, (string)array.GetValue(i), compareInfo);
					}
					if (flag3)
					{
						return true;
					}
				}
				return false;
			}
			return this.EvaluateHelper(obj as string, obj2 as string, compareInfo);
		}

		private bool EvaluateHelper(string lhsString, string rhsString, CompareInfo compareInfo)
		{
			SearchCriteriaText.SearchTextFullness searchTextFullness = this.FullnessFlags & ~(SearchCriteriaText.SearchTextFullness.PrefixOnAnyWord | SearchCriteriaText.SearchTextFullness.PhraseMatch);
			if (searchTextFullness == SearchCriteriaText.SearchTextFullness.FullString)
			{
				if (compareInfo == null)
				{
					return string.Compare(lhsString, rhsString, (this.FuzzynessFlags != (SearchCriteriaText.SearchTextFuzzyLevel)0) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0;
				}
				return compareInfo.Compare(lhsString, rhsString, (this.FuzzynessFlags != (SearchCriteriaText.SearchTextFuzzyLevel)0) ? CompareOptions.IgnoreCase : CompareOptions.None) == 0;
			}
			else
			{
				if (lhsString == null || rhsString == null)
				{
					return rhsString == null;
				}
				if (searchTextFullness == SearchCriteriaText.SearchTextFullness.Prefix)
				{
					if (compareInfo == null)
					{
						return lhsString.StartsWith(rhsString, (this.FuzzynessFlags != (SearchCriteriaText.SearchTextFuzzyLevel)0) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
					}
					return compareInfo.IsPrefix(lhsString, rhsString, (this.FuzzynessFlags != (SearchCriteriaText.SearchTextFuzzyLevel)0) ? CompareOptions.IgnoreCase : CompareOptions.None);
				}
				else
				{
					if ((ushort)(searchTextFullness & SearchCriteriaText.SearchTextFullness.SubString) != 1)
					{
						return false;
					}
					if (compareInfo == null)
					{
						return -1 != lhsString.IndexOf(rhsString, (this.FuzzynessFlags != (SearchCriteriaText.SearchTextFuzzyLevel)0) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
					}
					return -1 != compareInfo.IndexOf(lhsString, rhsString, (this.FuzzynessFlags != (SearchCriteriaText.SearchTextFuzzyLevel)0) ? CompareOptions.IgnoreCase : CompareOptions.None);
				}
			}
		}

		public override void EnumerateColumns(Action<Column, object> callback, object state, bool explodeCompositeColumns)
		{
			if (this.lhs != null)
			{
				if (explodeCompositeColumns)
				{
					this.lhs.EnumerateColumns(callback, state);
				}
				else
				{
					callback(this.lhs, state);
				}
			}
			if (this.rhs != null)
			{
				if (explodeCompositeColumns)
				{
					this.rhs.EnumerateColumns(callback, state);
					return;
				}
				callback(this.rhs, state);
			}
		}

		public override void AppendToString(StringBuilder sb, StringFormatOptions formatOptions)
		{
			sb.Append("TEXT(");
			this.lhs.AppendToString(sb, formatOptions);
			sb.Append(", ");
			this.rhs.AppendToString(sb, formatOptions);
			sb.Append(", ");
			SearchCriteriaText.FullnessFlagsAsString(this.fullnessFlags, sb);
			sb.Append(", ");
			SearchCriteriaText.FuzzynessFlagsAsString(this.fuzzynessFlags, sb);
			sb.Append(")");
		}

		internal static void FullnessFlagsAsString(SearchCriteriaText.SearchTextFullness fullnessFlags, StringBuilder sb)
		{
			sb.Append(fullnessFlags.ToString());
		}

		internal static void FuzzynessFlagsAsString(SearchCriteriaText.SearchTextFuzzyLevel fuzzynessFlags, StringBuilder sb)
		{
			sb.Append(fuzzynessFlags.ToString());
		}

		protected override bool CriteriaEquivalent(SearchCriteria other)
		{
			SearchCriteriaText searchCriteriaText = other as SearchCriteriaText;
			return searchCriteriaText != null && this.lhs == searchCriteriaText.lhs && this.fullnessFlags == searchCriteriaText.fullnessFlags && this.fuzzynessFlags == searchCriteriaText.fuzzynessFlags && this.rhs == searchCriteriaText.rhs;
		}

		private readonly Column lhs;

		private readonly SearchCriteriaText.SearchTextFullness fullnessFlags;

		private readonly SearchCriteriaText.SearchTextFuzzyLevel fuzzynessFlags;

		private readonly Column rhs;

		public enum SearchTextFuzzyLevel : ushort
		{
			IgnoreCase = 1,
			IgnoreNonSpace,
			Loose = 4
		}

		[Flags]
		public enum SearchTextFullness : ushort
		{
			FullString = 0,
			SubString = 1,
			Prefix = 2,
			PrefixOnAnyWord = 16,
			PhraseMatch = 32
		}
	}
}
