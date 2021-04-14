using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.FullTextIndex
{
	internal static class FqlQueryGenerator
	{
		public static string ToFqlString(this SearchCriteria value, CultureInfo culture)
		{
			return value.ToFqlString(FqlQueryGenerator.Options.Default, culture);
		}

		public static string ToFqlString(this SearchCriteria value, FqlQueryGenerator.Options options, CultureInfo culture)
		{
			return value.ToFql(options, culture).Value;
		}

		public static FqlQuery ToFql(this SearchCriteria value, FqlQueryGenerator.Options options, CultureInfo culture)
		{
			FqlQueryGenerator.FqlQueryBuilder fqlQueryBuilder = new FqlQueryGenerator.FqlQueryBuilder();
			return fqlQueryBuilder.ToFql(value, options, culture);
		}

		[Conditional("DEBUG")]
		internal static void AssertCanProcess(SearchCriteria value, bool looseCheck)
		{
			bool flag;
			FullTextIndexSchema.Current.GetCriteriaFullTextFlavor(value, Guid.Empty, looseCheck, out flag);
		}

		internal static bool IsValidFqlRangeType(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
			case TypeCode.DateTime:
				return true;
			default:
				return false;
			}
		}

		private const string FalseCondition = "and(string(\"abc\", not(string(\"abc\"))";

		[Flags]
		public enum Options
		{
			Default = 0,
			LooseCheck = 1
		}

		private class FqlQueryBuilder
		{
			public FqlQueryBuilder()
			{
				this.termReplacementMapping = new Dictionary<string, string>();
			}

			public FqlQuery ToFql(SearchCriteria value, FqlQueryGenerator.Options options, CultureInfo culture)
			{
				FqlQuery fqlQuery = new FqlQuery();
				this.AppendToFqlString(value, fqlQuery, options, culture);
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<string, string> keyValuePair in this.termReplacementMapping)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(',');
					}
					stringBuilder.AppendFormat("{0}={1}", keyValuePair.Value, keyValuePair.Key.Length);
				}
				fqlQuery.TermLength = stringBuilder.ToString();
				return fqlQuery;
			}

			private static void SeparateNotCriteria(ICollection<SearchCriteria> source, out List<SearchCriteria> notCriteriaList, out List<SearchCriteria> rest)
			{
				notCriteriaList = new List<SearchCriteria>(source.Count);
				rest = new List<SearchCriteria>(source.Count);
				foreach (SearchCriteria searchCriteria in source)
				{
					SearchCriteriaNot searchCriteriaNot = searchCriteria as SearchCriteriaNot;
					if (searchCriteriaNot != null)
					{
						notCriteriaList.Add(searchCriteriaNot.Criteria);
					}
					else
					{
						rest.Add(searchCriteria);
					}
				}
			}

			private static void WrapIntoCompositeOperator(FqlQuery targetFqlString, FqlQuery sourceFqlString, int numTerms, string op)
			{
				if (numTerms > 1)
				{
					targetFqlString.Append(op);
					targetFqlString.Append("(");
					targetFqlString.Append(sourceFqlString);
					targetFqlString.Append(")");
					return;
				}
				targetFqlString.Append(sourceFqlString);
			}

			private static FullTextIndexSchema.FullTextIndexInfo GetFastPropertyInfo(Column column)
			{
				FullTextIndexSchema.FullTextIndexInfo result = null;
				if (!FullTextIndexSchema.Current.IsColumnInFullTextIndex(column, Guid.Empty, out result))
				{
					return null;
				}
				return result;
			}

			private static string SanitizeSearchString(string input)
			{
				StringBuilder stringBuilder = new StringBuilder(input.Length);
				foreach (char c in input)
				{
					char c2 = c;
					if (c2 != '"')
					{
						if (c2 == '\\')
						{
							stringBuilder.Append("\\\\");
						}
						else
						{
							stringBuilder.Append(c);
						}
					}
					else
					{
						stringBuilder.Append("\\\"");
					}
				}
				return stringBuilder.ToString();
			}

			private static bool CheckValueEmptyString(object value)
			{
				string text = value as string;
				return text != null && string.IsNullOrEmpty(text);
			}

			private void AppendToFqlString(SearchCriteria value, FqlQuery fqlQuery, FqlQueryGenerator.Options options, CultureInfo culture)
			{
				SearchCriteriaText searchCriteriaText = value as SearchCriteriaText;
				if (searchCriteriaText != null)
				{
					FqlQueryGenerator.FqlQueryBuilder.FqlTerm fqlTerm = this.ToFqlTerm(searchCriteriaText, options, culture);
					if (fqlTerm != null)
					{
						fqlTerm.Append(fqlQuery);
						return;
					}
					fqlQuery.Append("and(string(\"abc\", not(string(\"abc\"))");
					return;
				}
				else
				{
					SearchCriteriaCompare searchCriteriaCompare = value as SearchCriteriaCompare;
					if (searchCriteriaCompare != null)
					{
						FqlQueryGenerator.FqlQueryBuilder.FqlTerm fqlTerm2 = this.ToFqlTerm(searchCriteriaCompare, options, culture);
						if (fqlTerm2 != null)
						{
							fqlTerm2.Append(fqlQuery);
							return;
						}
						fqlQuery.Append("and(string(\"abc\", not(string(\"abc\"))");
						return;
					}
					else
					{
						SearchCriteriaOr searchCriteriaOr = value as SearchCriteriaOr;
						if (searchCriteriaOr != null)
						{
							this.AppendToFqlString(searchCriteriaOr, fqlQuery, options, culture);
							return;
						}
						SearchCriteriaAnd searchCriteriaAnd = value as SearchCriteriaAnd;
						if (searchCriteriaAnd != null)
						{
							this.AppendToFqlString(searchCriteriaAnd, fqlQuery, options, culture);
							return;
						}
						SearchCriteriaNot searchCriteriaNot = value as SearchCriteriaNot;
						if (searchCriteriaNot != null)
						{
							this.AppendToFqlString(searchCriteriaNot, fqlQuery, options, culture);
							return;
						}
						SearchCriteriaNear searchCriteriaNear = value as SearchCriteriaNear;
						if (searchCriteriaNear != null)
						{
							this.AppendToFqlString(searchCriteriaNear, fqlQuery, options, culture);
							return;
						}
						fqlQuery.Append("and(string(\"abc\", not(string(\"abc\"))");
						return;
					}
				}
			}

			private void AppendToFqlString(SearchCriteriaAnd value, FqlQuery fqlQuery, FqlQueryGenerator.Options options, CultureInfo culture)
			{
				if (value.NestedCriteria.Length == 0)
				{
					fqlQuery.Append("and(string(\"abc\", not(string(\"abc\"))");
					return;
				}
				List<SearchCriteria> list;
				List<SearchCriteria> list2;
				FqlQueryGenerator.FqlQueryBuilder.SeparateNotCriteria(value.NestedCriteria, out list, out list2);
				if (list2.Count == 0)
				{
					FqlQuery fqlQuery2 = new FqlQuery();
					fqlQuery.Append("not(");
					FqlQueryGenerator.FqlQueryBuilder.WrapIntoCompositeOperator(fqlQuery, fqlQuery2, this.AppendTermsForAndOr(fqlQuery2, list, options, culture, false), "or");
					fqlQuery.Append(")");
					return;
				}
				if (list.Count > 0)
				{
					fqlQuery.Append("andnot(");
				}
				FqlQuery fqlQuery3 = new FqlQuery();
				FqlQueryGenerator.FqlQueryBuilder.WrapIntoCompositeOperator(fqlQuery, fqlQuery3, this.AppendTermsForAndOr(fqlQuery3, list2, options, culture, true), "and");
				if (list.Count > 0)
				{
					fqlQuery.Append(", ");
					FqlQuery fqlQuery4 = new FqlQuery();
					FqlQueryGenerator.FqlQueryBuilder.WrapIntoCompositeOperator(fqlQuery, fqlQuery4, this.AppendTermsForAndOr(fqlQuery4, list, options, culture, false), "or");
					fqlQuery.Append(")");
				}
			}

			private void AppendToFqlString(SearchCriteriaOr value, FqlQuery fqlQuery, FqlQueryGenerator.Options options, CultureInfo culture)
			{
				if (value.NestedCriteria.Length == 0)
				{
					fqlQuery.Append("and(string(\"abc\", not(string(\"abc\"))");
					return;
				}
				List<SearchCriteria> list;
				List<SearchCriteria> list2;
				FqlQueryGenerator.FqlQueryBuilder.SeparateNotCriteria(value.NestedCriteria, out list, out list2);
				int num = 0;
				FqlQuery fqlQuery2 = new FqlQuery();
				if (list.Count > 0)
				{
					fqlQuery2.Append("not(");
					FqlQuery fqlQuery3 = new FqlQuery();
					FqlQueryGenerator.FqlQueryBuilder.WrapIntoCompositeOperator(fqlQuery2, fqlQuery3, this.AppendTermsForAndOr(fqlQuery3, list, options, culture, true), "and");
					fqlQuery2.Append(")");
					num++;
				}
				if (list2.Count > 0)
				{
					if (num > 0)
					{
						fqlQuery2.Append(", ");
					}
					num += this.AppendTermsForAndOr(fqlQuery2, list2, options, culture, false);
				}
				FqlQueryGenerator.FqlQueryBuilder.WrapIntoCompositeOperator(fqlQuery, fqlQuery2, num, "or");
			}

			private int AppendTermsForAndOr(FqlQuery fqlQuery, IEnumerable<SearchCriteria> nestedCriteria, FqlQueryGenerator.Options options, CultureInfo culture, bool isAnd)
			{
				int num = 0;
				Dictionary<string, HashSet<FqlQueryGenerator.FqlQueryBuilder.FqlTerm>> dictionary = new Dictionary<string, HashSet<FqlQueryGenerator.FqlQueryBuilder.FqlTerm>>();
				foreach (SearchCriteria searchCriteria in nestedCriteria)
				{
					FqlQueryGenerator.FqlQueryBuilder.FqlTerm fqlTerm = null;
					SearchCriteriaText searchCriteriaText = searchCriteria as SearchCriteriaText;
					if (searchCriteriaText != null)
					{
						fqlTerm = this.ToFqlTerm(searchCriteriaText, options, culture);
					}
					else
					{
						SearchCriteriaCompare searchCriteriaCompare = searchCriteria as SearchCriteriaCompare;
						if (searchCriteriaCompare != null)
						{
							fqlTerm = this.ToFqlTerm(searchCriteriaCompare, options, culture);
						}
					}
					if (fqlTerm != null)
					{
						HashSet<FqlQueryGenerator.FqlQueryBuilder.FqlTerm> hashSet = null;
						if (!dictionary.TryGetValue(fqlTerm.FqlConstraint.Value, out hashSet))
						{
							hashSet = new HashSet<FqlQueryGenerator.FqlQueryBuilder.FqlTerm>();
							dictionary.Add(fqlTerm.FqlConstraint.Value, hashSet);
						}
						hashSet.Add(fqlTerm);
					}
					else
					{
						if (num > 0)
						{
							fqlQuery.Append(", ");
						}
						this.AppendToFqlString(searchCriteria, fqlQuery, options, culture);
						num++;
					}
				}
				foreach (HashSet<FqlQueryGenerator.FqlQueryBuilder.FqlTerm> collection in dictionary.Values)
				{
					List<FqlQueryGenerator.FqlQueryBuilder.FqlTerm> list = new List<FqlQueryGenerator.FqlQueryBuilder.FqlTerm>(collection);
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] != null)
						{
							for (int j = 0; j < list.Count; j++)
							{
								if (i != j && list[j] != null && ((isAnd && list[i].IsStricterOrEquivalentTo(list[j])) || (!isAnd && list[j].IsStricterOrEquivalentTo(list[i]))))
								{
									list[j] = null;
								}
							}
						}
					}
					foreach (FqlQueryGenerator.FqlQueryBuilder.FqlTerm fqlTerm2 in list)
					{
						if (fqlTerm2 != null)
						{
							if (num > 0)
							{
								fqlQuery.Append(", ");
							}
							fqlTerm2.Append(fqlQuery);
							num++;
						}
					}
				}
				return num;
			}

			private void AppendToFqlString(SearchCriteriaNot value, FqlQuery fqlQuery, FqlQueryGenerator.Options options, CultureInfo culture)
			{
				if (value.Criteria == null)
				{
					fqlQuery.Append("and(string(\"abc\", not(string(\"abc\"))");
					return;
				}
				fqlQuery.Append("not(");
				this.AppendToFqlString(value.Criteria, fqlQuery, options, culture);
				fqlQuery.Append(')');
			}

			private void AppendToFqlString(SearchCriteriaNear value, FqlQuery fqlString, FqlQueryGenerator.Options options, CultureInfo culture)
			{
				if (value.Criteria == null || value.Criteria.NestedCriteria.Length == 0)
				{
					fqlString.Append("and(string(\"abc\", not(string(\"abc\"))");
					return;
				}
				fqlString.Append(value.Ordered ? "onear(" : "near(");
				foreach (SearchCriteria value2 in value.Criteria.NestedCriteria)
				{
					this.AppendToFqlString(value2, fqlString, options, culture);
					fqlString.Append(", ");
				}
				fqlString.Append("N=");
				fqlString.Append(value.Distance.ToString());
				fqlString.Append(")");
			}

			private FqlQueryGenerator.FqlQueryBuilder.FqlTerm ToFqlTerm(SearchCriteriaCompare value, FqlQueryGenerator.Options options, CultureInfo culture)
			{
				ConstantColumn constantColumn = value.Rhs as ConstantColumn;
				if (!(constantColumn != null) || constantColumn.Value == null)
				{
					return null;
				}
				FullTextIndexSchema.FullTextIndexInfo fastPropertyInfo = FqlQueryGenerator.FqlQueryBuilder.GetFastPropertyInfo(value.Lhs);
				if (FqlQueryGenerator.FqlQueryBuilder.CheckValueEmptyString(constantColumn.Value))
				{
					return null;
				}
				FqlQuery fqlQuery = new FqlQuery();
				if (FqlQueryGenerator.IsValidFqlRangeType(constantColumn.Type))
				{
					if (fastPropertyInfo.Definition == null || !fastPropertyInfo.Definition.Queryable)
					{
						return null;
					}
					fqlQuery.Append("range(");
					switch (value.RelOp)
					{
					case SearchCriteriaCompare.SearchRelOp.Equal:
					case SearchCriteriaCompare.SearchRelOp.NotEqual:
					{
						FqlQuery fqlString = this.ValueToFqlString(constantColumn.Value, SearchCriteriaText.SearchTextFullness.PhraseMatch, options, culture);
						fqlQuery.Append(fqlString);
						fqlQuery.Append(", ");
						fqlQuery.Append(fqlString);
						fqlQuery.Append(", from=ge, to=le");
						break;
					}
					case SearchCriteriaCompare.SearchRelOp.LessThan:
						fqlQuery.Append("min, ");
						fqlQuery.Append(this.ValueToFqlString(constantColumn.Value, SearchCriteriaText.SearchTextFullness.PhraseMatch, options, culture));
						fqlQuery.Append(", to=lt");
						break;
					case SearchCriteriaCompare.SearchRelOp.LessThanEqual:
						fqlQuery.Append("min, ");
						fqlQuery.Append(this.ValueToFqlString(constantColumn.Value, SearchCriteriaText.SearchTextFullness.PhraseMatch, options, culture));
						fqlQuery.Append(", to=le");
						break;
					case SearchCriteriaCompare.SearchRelOp.GreaterThan:
						fqlQuery.Append(this.ValueToFqlString(constantColumn.Value, SearchCriteriaText.SearchTextFullness.PhraseMatch, options, culture));
						fqlQuery.Append(", max, from=gt");
						break;
					case SearchCriteriaCompare.SearchRelOp.GreaterThanEqual:
						fqlQuery.Append(this.ValueToFqlString(constantColumn.Value, SearchCriteriaText.SearchTextFullness.PhraseMatch, options, culture));
						fqlQuery.Append(", max, from=ge");
						break;
					}
					fqlQuery.Append(')');
				}
				else
				{
					if (value.RelOp != SearchCriteriaCompare.SearchRelOp.Equal && value.RelOp != SearchCriteriaCompare.SearchRelOp.NotEqual)
					{
						return null;
					}
					if (fastPropertyInfo.Definition != null && fastPropertyInfo.Definition == FastIndexSystemSchema.FolderId.Definition)
					{
						byte[] array = constantColumn.Value as byte[];
						if (array != null && array.Length == 26)
						{
							fqlQuery.Append("string(\"");
							string text = HexConverter.ByteArrayToHexString(array, 0, 24);
							fqlQuery.AppendValue(text, this.GetTermReplacements(text));
							fqlQuery.Append("\")");
						}
						else
						{
							fqlQuery.Append(this.ValueToFqlString(constantColumn.Value, SearchCriteriaText.SearchTextFullness.PhraseMatch, options, culture));
						}
					}
					else
					{
						fqlQuery.Append(this.ValueToFqlString(constantColumn.Value, SearchCriteriaText.SearchTextFullness.PhraseMatch, options, culture));
					}
				}
				return new FqlQueryGenerator.FqlQueryBuilder.FqlTerm(fastPropertyInfo, fqlQuery, value.RelOp == SearchCriteriaCompare.SearchRelOp.NotEqual);
			}

			private FqlQueryGenerator.FqlQueryBuilder.FqlTerm ToFqlTerm(SearchCriteriaText value, FqlQueryGenerator.Options options, CultureInfo culture)
			{
				ConstantColumn constantColumn = value.Rhs as ConstantColumn;
				if (!(constantColumn != null) || constantColumn.Value == null)
				{
					return null;
				}
				FullTextIndexSchema.FullTextIndexInfo fastPropertyInfo = FqlQueryGenerator.FqlQueryBuilder.GetFastPropertyInfo(value.Lhs);
				if (FqlQueryGenerator.FqlQueryBuilder.CheckValueEmptyString(constantColumn.Value))
				{
					return null;
				}
				return new FqlQueryGenerator.FqlQueryBuilder.FqlTerm(fastPropertyInfo, this.ValueToFqlString(constantColumn.Value, value.FullnessFlags, options, culture), false);
			}

			private string GetTermReplacements(string term)
			{
				string text;
				if (!this.termReplacementMapping.TryGetValue(term, out text))
				{
					if (term.IndexOf("31febf7b418e44878df6e5623e37c828", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						text = term;
					}
					else
					{
						text = string.Format("term{0}", this.termReplacementMapping.Count + 1);
						this.termReplacementMapping[term] = text;
					}
				}
				return text;
			}

			private FqlQuery ValueToFqlString(object value, SearchCriteriaText.SearchTextFullness flags, FqlQueryGenerator.Options options, CultureInfo culture)
			{
				FqlQuery fqlQuery = new FqlQuery();
				string text;
				switch (Type.GetTypeCode(value.GetType()))
				{
				case TypeCode.Boolean:
					fqlQuery.Append("string(");
					text = (((bool)value) ? 1 : 0).GetAsString<int>();
					fqlQuery.AppendValue(text, this.GetTermReplacements(text));
					fqlQuery.Append(")");
					return fqlQuery;
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
					fqlQuery.Append("int(");
					text = value.GetAsString<object>();
					fqlQuery.AppendValue(text, this.GetTermReplacements(text));
					goto IL_26A;
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					fqlQuery.Append("float(");
					text = value.GetAsString<object>();
					fqlQuery.AppendValue(text, this.GetTermReplacements(text));
					goto IL_26A;
				case TypeCode.DateTime:
					fqlQuery.Append("datetime(");
					text = ((DateTime)value).ToString("s");
					fqlQuery.AppendValue(text, this.GetTermReplacements(text));
					fqlQuery.Append('Z');
					goto IL_26A;
				case TypeCode.String:
					fqlQuery.Append("string(\"");
					text = FqlQueryGenerator.FqlQueryBuilder.SanitizeSearchString((string)value);
					fqlQuery.AppendValue(text, this.GetTermReplacements(text));
					if ((ushort)(flags & SearchCriteriaText.SearchTextFullness.PhraseMatch) == 32)
					{
						if (!(culture.TwoLetterISOLanguageName == "zh") && !(culture.TwoLetterISOLanguageName == "ja") && !(culture.TwoLetterISOLanguageName == "ko") && !(culture.TwoLetterISOLanguageName == "th") && !(culture.TwoLetterISOLanguageName == "km") && !(culture.TwoLetterISOLanguageName == "lo") && !(culture.TwoLetterISOLanguageName == "my"))
						{
							fqlQuery.Append("\"");
							goto IL_26A;
						}
						fqlQuery.Append((((string)value).Length == 1) ? "*\", mode=\"and\"" : "\"");
						goto IL_26A;
					}
					else
					{
						if ((ushort)(flags & SearchCriteriaText.SearchTextFullness.SubString) == 1 || (ushort)(flags & SearchCriteriaText.SearchTextFullness.Prefix) == 2 || (ushort)(flags & SearchCriteriaText.SearchTextFullness.PrefixOnAnyWord) == 16)
						{
							fqlQuery.Append("*\", mode=\"and\"");
							goto IL_26A;
						}
						fqlQuery.Append("\", mode=\"and\"");
						goto IL_26A;
					}
					break;
				}
				fqlQuery.Append("string(\"");
				text = FqlQueryGenerator.FqlQueryBuilder.SanitizeSearchString(value.GetAsString<object>());
				fqlQuery.AppendValue(text, this.GetTermReplacements(text));
				fqlQuery.Append("\", wildcard=off");
				IL_26A:
				fqlQuery.Append(')');
				return fqlQuery;
			}

			private Dictionary<string, string> termReplacementMapping;

			private class FqlTerm : IEquatable<FqlQueryGenerator.FqlQueryBuilder.FqlTerm>
			{
				public FqlTerm(FullTextIndexSchema.FullTextIndexInfo scope, FqlQuery constraint, bool needsNegation)
				{
					this.scope = scope;
					this.constraint = constraint;
					this.needsNegation = needsNegation;
				}

				public FqlQuery FqlConstraint
				{
					get
					{
						return this.constraint;
					}
				}

				public void Append(FqlQuery fqlString)
				{
					if (this.needsNegation)
					{
						fqlString.Append("not(");
					}
					if (this.scope.Definition != null && this.scope.Definition.Queryable)
					{
						fqlString.Append(this.scope.FastPropertyName);
						fqlString.Append(":");
					}
					fqlString.Append(this.FqlConstraint);
					if (this.needsNegation)
					{
						fqlString.Append(")");
					}
				}

				public bool IsStricterOrEquivalentTo(FqlQueryGenerator.FqlQueryBuilder.FqlTerm term)
				{
					if (this.needsNegation != term.needsNegation || this.FqlConstraint.Value != term.FqlConstraint.Value)
					{
						return false;
					}
					FqlQueryGenerator.FqlQueryBuilder.FqlTerm fqlTerm = this.needsNegation ? term : this;
					FqlQueryGenerator.FqlQueryBuilder.FqlTerm fqlTerm2 = this.needsNegation ? this : term;
					return fqlTerm.scope.Definition == fqlTerm2.scope.Definition || ((fqlTerm.scope.Definition == null || !fqlTerm.scope.Definition.Queryable) && (fqlTerm2.scope.Definition == null || !fqlTerm2.scope.Definition.Queryable)) || (fqlTerm.scope.Definition != null && fqlTerm.scope.Definition.Searchable && (fqlTerm2.scope.Definition == null || !fqlTerm2.scope.Definition.Queryable));
				}

				public override int GetHashCode()
				{
					int num = this.FqlConstraint.Value.GetHashCode();
					string text = (this.scope.Definition == null) ? string.Empty : this.scope.FastPropertyName;
					num ^= text.GetHashCode();
					return num ^ (this.needsNegation ? 1 : 0);
				}

				public bool Equals(FqlQueryGenerator.FqlQueryBuilder.FqlTerm other)
				{
					return this.scope.Definition == other.scope.Definition && this.FqlConstraint.Value == other.FqlConstraint.Value && this.needsNegation == other.needsNegation;
				}

				private readonly FullTextIndexSchema.FullTextIndexInfo scope;

				private readonly FqlQuery constraint;

				private readonly bool needsNegation;
			}
		}
	}
}
