using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ProvisioningAgent;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal class SinglePropertyLdapFilter : LdapFilter
	{
		private static bool IsCharStringSyntaxClass(DataSyntax syntax)
		{
			return syntax == DataSyntax.CaseSensitive || syntax == DataSyntax.Teletex || syntax == DataSyntax.Printable || syntax == DataSyntax.IA5 || syntax == DataSyntax.Numeric;
		}

		private static bool IsUnicodeStringSyntaxClass(DataSyntax syntax)
		{
			return syntax == DataSyntax.Unicode;
		}

		private static bool IsNumericSyntaxClass(DataSyntax syntax)
		{
			return syntax == DataSyntax.Integer || syntax == DataSyntax.Enumeration || syntax == DataSyntax.LargeInteger;
		}

		private static bool HasWildChar(string wsValue)
		{
			return wsValue.IndexOf('*') != -1;
		}

		private static bool IsPresenceCheck(string value)
		{
			return StringComparer.InvariantCultureIgnoreCase.Equals(value, "*");
		}

		private static bool StringToLong(string str, out int value)
		{
			if (int.TryParse(str, NumberStyles.AllowLeadingSign, null, out value))
			{
				return true;
			}
			uint num;
			if (uint.TryParse(str, NumberStyles.AllowLeadingSign, null, out num))
			{
				value = (int)num;
				return true;
			}
			return false;
		}

		private static bool StringToLongLong(string str, out long value)
		{
			return long.TryParse(str, NumberStyles.AllowLeadingSign, null, out value);
		}

		private static int CaseInsensitiveCompare(string left, string right)
		{
			return string.Compare(left, right, true, CultureInfo.InvariantCulture);
		}

		private static int CaseSensitiveCompare(string left, string right)
		{
			return string.Compare(left, right, false, CultureInfo.InvariantCulture);
		}

		private static int IntUnescapeHex(char char1, char char2)
		{
			if (LdapFilter.LdapEscapable(char1, char2))
			{
				return int.Parse(char1 + char2, NumberStyles.HexNumber);
			}
			return -1;
		}

		internal static string Unescape(StringBuilder wsValue)
		{
			for (int i = 0; i < wsValue.Length; i++)
			{
				if (wsValue[i] == '\\' && wsValue.Length - i > 2 && LdapFilter.LdapEscapable(wsValue[i + 1], wsValue[i + 2]))
				{
					wsValue[i] = (char)SinglePropertyLdapFilter.IntUnescapeHex(wsValue[i + 1], wsValue[i + 2]);
					wsValue.Remove(i + 1, 2);
				}
			}
			return wsValue.ToString();
		}

		private static string UnescapeToHex(StringBuilder wsValue)
		{
			int i = 0;
			while (i < wsValue.Length)
			{
				if (wsValue[i] == '\\')
				{
					if (wsValue.Length - i <= 2 || !LdapFilter.LdapEscapable(wsValue[i + 1], wsValue[i + 2]))
					{
						throw new LdapFilterException(Strings.LdapFilterErrorInvalidEscaping(wsValue.ToString()));
					}
					wsValue.Remove(i, 1);
					i += 2;
				}
				else
				{
					if (!char.IsLetterOrDigit(wsValue[i]))
					{
						throw new LdapFilterException(Strings.LdapFilterErrorInvalidEscaping(wsValue.ToString()));
					}
					string text = "0123456789ABCDEF";
					char c = wsValue[i];
					wsValue.Insert(i, "0");
					wsValue[i] = text[(int)(c >> 4 & '\u000f')];
					wsValue[i + 1] = text[(int)(c & '\u000f')];
					i += 2;
				}
			}
			return wsValue.ToString();
		}

		private static string[] SplitByWildCard(string wsValue)
		{
			List<string> list = new List<string>();
			int i = 0;
			int num = i;
			while (i < wsValue.Length)
			{
				char c = wsValue[i];
				if (c != '*')
				{
					if (c == '\\')
					{
						i++;
					}
				}
				else
				{
					if (num != i)
					{
						string text = wsValue.Substring(num, i - num);
						text = SinglePropertyLdapFilter.Unescape(new StringBuilder(text));
						list.Add(text);
					}
					list.Add("");
					num = i + 1;
				}
				i++;
			}
			if (num != i)
			{
				string text = wsValue.Substring(num, i - num);
				text = SinglePropertyLdapFilter.Unescape(new StringBuilder(text));
				list.Add(text);
			}
			return list.ToArray();
		}

		private string ValidateAndSetOperand(char wcOp, string wsAttrType)
		{
			string result = wsAttrType;
			char c = wcOp;
			switch (c)
			{
			case ':':
			{
				int num = wsAttrType.IndexOf(':');
				if (num != -1 && num < wsAttrType.Length - 1)
				{
					if (string.Equals(wsAttrType.Substring(num + 1, wsAttrType.Length - num - 1), SinglePropertyLdapFilter.LdapMatchingRuleBitAnd))
					{
						this.Operator = LdapComparisonOperator.BitAnd;
					}
					else
					{
						if (!string.Equals(wsAttrType.Substring(num + 1, wsAttrType.Length - num - 1), SinglePropertyLdapFilter.LdapMatchingRuleBitOr))
						{
							throw new LdapFilterException(Strings.LdapFilterErrorInvalidBitwiseOperand);
						}
						this.Operator = LdapComparisonOperator.BitOr;
					}
					return wsAttrType.Substring(0, num);
				}
				throw new LdapFilterException(Strings.LdapFilterErrorInvalidBitwiseOperand);
			}
			case ';':
				break;
			case '<':
				this.Operator = LdapComparisonOperator.LessOrEqual;
				return result;
			case '=':
				this.Operator = LdapComparisonOperator.Equal;
				return result;
			case '>':
				this.Operator = LdapComparisonOperator.GreaterOrEqual;
				return result;
			default:
				if (c != '~')
				{
				}
				break;
			}
			throw new LdapFilterException(Strings.LdapFilterErrorUnsupportedOperand(wcOp.ToString()));
		}

		private void ValidateAndSetAttributeValue(string wsValue, LdapFilterProvider provider)
		{
			if (wsValue.Length == 0)
			{
				throw new LdapFilterException(Strings.LdapFilterErrorNoAttributeValue(this.stringFilter));
			}
			wsValue = wsValue.TrimStart(new char[0]);
			if (wsValue.Length == 0)
			{
				throw new LdapFilterException(Strings.LdapFilterErrorTypeOnlySpaces(this.stringFilter));
			}
			if (this.Operator != LdapComparisonOperator.Equal && SinglePropertyLdapFilter.HasWildChar(wsValue))
			{
				throw new LdapFilterException(Strings.LdapFilterErrorInvalidWildCardGtLt);
			}
			this.m_wsValue = wsValue;
			if (!SinglePropertyLdapFilter.IsPresenceCheck(wsValue))
			{
				this.m_bCaseSensitive = false;
				if (this.Syntax == DataSyntax.CaseSensitive || this.Syntax == DataSyntax.Printable || this.Syntax == DataSyntax.IA5)
				{
					this.m_bCaseSensitive = true;
				}
				switch (this.Syntax)
				{
				case DataSyntax.UnDefined:
					throw new LdapFilterException(Strings.LdapFilterErrorUndefinedAttributeType(this.m_wsType));
				case DataSyntax.Boolean:
					if (SinglePropertyLdapFilter.CaseInsensitiveCompare(wsValue, "TRUE") != 0 && SinglePropertyLdapFilter.CaseInsensitiveCompare(wsValue, "FALSE") != 0)
					{
						throw new LdapFilterException(Strings.LdapFilterErrorInvalidBooleanValue);
					}
					this.m_value = wsValue;
					goto IL_2B2;
				case DataSyntax.Integer:
				case DataSyntax.Enumeration:
				{
					if (SinglePropertyLdapFilter.HasWildChar(wsValue))
					{
						throw new LdapFilterException(Strings.LdapFilterErrorInvalidWildCard(this.m_wsType));
					}
					int num;
					if (!SinglePropertyLdapFilter.StringToLong(wsValue, out num))
					{
						throw new LdapFilterException(Strings.LdapFilterErrorInvalidDecimal(wsValue));
					}
					this.m_value = num;
					goto IL_2B2;
				}
				case DataSyntax.Sid:
				case DataSyntax.Octet:
					if (SinglePropertyLdapFilter.HasWildChar(wsValue))
					{
						throw new LdapFilterException(Strings.LdapFilterErrorInvalidWildCard(this.m_wsType));
					}
					wsValue = SinglePropertyLdapFilter.UnescapeToHex(new StringBuilder(wsValue));
					this.m_value = wsValue;
					goto IL_2B2;
				case DataSyntax.ObjectIdentifier:
				case DataSyntax.DSDN:
					if (SinglePropertyLdapFilter.HasWildChar(wsValue))
					{
						throw new LdapFilterException(Strings.LdapFilterErrorInvalidWildCard(this.m_wsType));
					}
					wsValue = SinglePropertyLdapFilter.Unescape(new StringBuilder(wsValue));
					this.m_value = wsValue;
					goto IL_2B2;
				case DataSyntax.Numeric:
				case DataSyntax.Printable:
				case DataSyntax.Teletex:
				case DataSyntax.IA5:
				case DataSyntax.CaseSensitive:
				case DataSyntax.Unicode:
					if (SinglePropertyLdapFilter.HasWildChar(wsValue))
					{
						string[] value = SinglePropertyLdapFilter.SplitByWildCard(wsValue);
						this.m_value = value;
						goto IL_2B2;
					}
					wsValue = SinglePropertyLdapFilter.Unescape(new StringBuilder(wsValue));
					this.m_value = wsValue;
					goto IL_2B2;
				case DataSyntax.UTCTime:
				case DataSyntax.GeneralizedTime:
					throw new LdapFilterException(Strings.LdapFilterErrorUnsupportedAttrbuteType(this.m_wsType));
				case DataSyntax.LargeInteger:
				{
					if (SinglePropertyLdapFilter.HasWildChar(wsValue))
					{
						throw new LdapFilterException(Strings.LdapFilterErrorInvalidWildCard(this.m_wsType));
					}
					long num2;
					if (!SinglePropertyLdapFilter.StringToLongLong(wsValue, out num2))
					{
						throw new LdapFilterException(Strings.LdapFilterErrorInvalidDecimal(wsValue));
					}
					this.m_value = num2;
					goto IL_2B2;
				}
				case DataSyntax.NTSecDesc:
				case DataSyntax.ReplicaLink:
					throw new LdapFilterException(Strings.LdapFilterErrorUnsupportedAttrbuteType(this.m_wsType));
				case DataSyntax.AccessPoint:
				case DataSyntax.ORName:
				case DataSyntax.PresentationAddress:
					throw new LdapFilterException(Strings.LdapFilterErrorUnsupportedAttrbuteType(this.m_wsType));
				}
				throw new LdapFilterException(Strings.LdapFilterErrorUnsupportedAttrbuteType(this.m_wsType));
			}
			this.m_value = wsValue;
			IL_2B2:
			if (StringComparer.InvariantCultureIgnoreCase.Equals(this.m_wsType, "objectCategory") && !StringComparer.InvariantCultureIgnoreCase.Equals(this.m_value, "*"))
			{
				string objectCategory = provider.GetObjectCategory(this.m_wsValue);
				if (string.IsNullOrEmpty(objectCategory))
				{
					throw new LdapFilterException(Strings.LdapFilterErrorObjectCategoryNotFound(this.m_wsValue));
				}
				this.m_value = objectCategory;
			}
		}

		private void ValidateAndSetAttributeType(string wsAttr, LdapFilterProvider provider)
		{
			if (wsAttr.Length == 0)
			{
				throw new LdapFilterException(Strings.LdapFilterErrorNoAttributeType(this.stringFilter));
			}
			wsAttr = wsAttr.Trim();
			if (wsAttr.Length == 0)
			{
				throw new LdapFilterException(Strings.LdapFilterErrorTypeOnlySpaces(this.stringFilter));
			}
			int num = wsAttr.IndexOf(' ');
			if (num != -1)
			{
				throw new LdapFilterException(Strings.LdapFilterErrorSpaceMiddleType(wsAttr));
			}
			if (SinglePropertyLdapFilter.CaseInsensitiveCompare(wsAttr, "ANR") == 0)
			{
				throw new LdapFilterException(Strings.LdapFilterErrorAnrIsNotSupported);
			}
			SchemaAttributePresentationObject schemaAttributeObject = provider.GetSchemaAttributeObject(wsAttr);
			if (schemaAttributeObject == null)
			{
				throw new LdapFilterException(Strings.LdapFilterErrorUndefinedAttributeType(wsAttr));
			}
			this.Syntax = schemaAttributeObject.DataSyntax;
			this.m_wsType = wsAttr;
			switch (this.Operator)
			{
			case LdapComparisonOperator.GreaterOrEqual:
			case LdapComparisonOperator.LessOrEqual:
				if (!SinglePropertyLdapFilter.IsCharStringSyntaxClass(this.Syntax) && !SinglePropertyLdapFilter.IsUnicodeStringSyntaxClass(this.Syntax) && !SinglePropertyLdapFilter.IsNumericSyntaxClass(this.Syntax))
				{
					throw new LdapFilterException(Strings.LdapFilterErrorInvalidGtLtOperand);
				}
				break;
			case LdapComparisonOperator.BitAnd:
			case LdapComparisonOperator.BitOr:
				if (!SinglePropertyLdapFilter.IsNumericSyntaxClass(this.Syntax))
				{
					throw new LdapFilterException(Strings.LdapFilterErrorInvalidBitwiseOperand);
				}
				break;
			default:
				return;
			}
		}

		public new static LdapFilter Parse(string wsFilterCond, LdapFilterProvider provider)
		{
			SinglePropertyLdapFilter singlePropertyLdapFilter = new SinglePropertyLdapFilter();
			singlePropertyLdapFilter.stringFilter = wsFilterCond;
			int length = wsFilterCond.Length;
			int i = 0;
			int num = -1;
			int num2 = -1;
			bool flag = false;
			bool flag2 = false;
			int num3 = -1;
			int num4 = -1;
			bool flag3 = false;
			bool flag4 = false;
			int num5 = 0;
			bool flag5 = false;
			bool flag6 = false;
			char? c = null;
			char? c2 = null;
			int num6 = 0;
			while (i < length)
			{
				char c3 = wsFilterCond[i];
				if (c3 <= ')')
				{
					if (c3 == ' ')
					{
						i++;
						continue;
					}
					switch (c3)
					{
					case '(':
						if (c2 != '(')
						{
							char? c4 = c2;
							int? num7 = (c4 != null) ? new int?((int)c4.GetValueOrDefault()) : null;
							if (num7 != null)
							{
								flag5 = true;
								flag6 = true;
								if (!flag)
								{
									num = i;
									flag = true;
									break;
								}
								break;
							}
						}
						num5++;
						flag6 = false;
						break;
					case ')':
					{
						char? c5 = c2;
						int? num8 = (c5 != null) ? new int?((int)c5.GetValueOrDefault()) : null;
						if (num8 == null || !flag5)
						{
							if (!(c2 == '('))
							{
								char? c6 = c2;
								int? num9 = (c6 != null) ? new int?((int)c6.GetValueOrDefault()) : null;
								if (num9 != null)
								{
									if (!flag4 || flag3)
									{
										num4 = i;
										flag4 = true;
									}
									num5--;
									if (num5 < 0)
									{
										throw new LdapFilterException(Strings.LdapFilterErrorBracketMismatch);
									}
									flag6 = false;
									break;
								}
							}
							throw new LdapFilterException(Strings.LdapFilterErrorInvalidToken);
						}
						flag5 = false;
						flag6 = true;
						if (!flag)
						{
							num = i;
							flag = true;
						}
						break;
					}
					default:
						goto IL_38C;
					}
				}
				else
				{
					switch (c3)
					{
					case ':':
					case '<':
					case '>':
						break;
					case ';':
						goto IL_38C;
					case '=':
					{
						if (c2 == '(' || c2 == ')')
						{
							throw new LdapFilterException(Strings.LdapFilterErrorInvalidToken);
						}
						char? c7 = c;
						int? num10 = (c7 != null) ? new int?((int)c7.GetValueOrDefault()) : null;
						if (num10 != null)
						{
							goto IL_3BF;
						}
						c = new char?('=');
						num3 = i + 1;
						flag3 = true;
						if (flag)
						{
							num2 = i;
							flag2 = true;
							goto IL_3BF;
						}
						goto IL_3BF;
					}
					default:
						if (c3 != '\\')
						{
							if (c3 != '~')
							{
								goto IL_38C;
							}
						}
						else
						{
							if (c2 == ')')
							{
								throw new LdapFilterException(Strings.LdapFilterErrorInvalidToken);
							}
							if (length - i > 2 && LdapFilter.LdapEscapable(wsFilterCond[i + 1], wsFilterCond[i + 2]))
							{
								num6 = 2;
							}
							if (!flag)
							{
								num = i;
								flag = true;
								goto IL_3BF;
							}
							goto IL_3BF;
						}
						break;
					}
					if (c2 == '(' || c2 == ')')
					{
						throw new LdapFilterException(Strings.LdapFilterErrorInvalidToken);
					}
					if (wsFilterCond[i + 1] == '=')
					{
						char? c8 = c;
						int? num11 = (c8 != null) ? new int?((int)c8.GetValueOrDefault()) : null;
						if (num11 == null)
						{
							c = new char?(wsFilterCond[i]);
							if (flag)
							{
								num2 = i;
								flag2 = true;
							}
							i++;
							num3 = i + 1;
							flag3 = true;
						}
					}
				}
				IL_3BF:
				c2 = new char?(wsFilterCond[i]);
				i += num6 + 1;
				num6 = 0;
				continue;
				IL_38C:
				if (c2 == ')' && !flag6)
				{
					throw new LdapFilterException(Strings.LdapFilterErrorInvalidToken);
				}
				if (!flag)
				{
					num = i;
					flag = true;
					goto IL_3BF;
				}
				goto IL_3BF;
			}
			if (num5 != 0)
			{
				throw new LdapFilterException(Strings.LdapFilterErrorBracketMismatch);
			}
			char? c9 = c;
			int? num12 = (c9 != null) ? new int?((int)c9.GetValueOrDefault()) : null;
			if (num12 == null || !flag || !flag2)
			{
				throw new LdapFilterException(Strings.LdapFilterErrorNoValidComparison(wsFilterCond));
			}
			bool flag7 = false;
			if (wsFilterCond[num] == '!')
			{
				num++;
				while (num < wsFilterCond.Length && wsFilterCond[num] == ' ')
				{
					num++;
				}
				if (num == wsFilterCond.Length)
				{
					throw new LdapFilterException(Strings.LdapFilterErrorInvalidToken);
				}
				flag7 = true;
			}
			string text = wsFilterCond.Substring(num, num2 - num);
			text = singlePropertyLdapFilter.ValidateAndSetOperand(c.Value, text);
			singlePropertyLdapFilter.ValidateAndSetAttributeType(text, provider);
			if (!flag4)
			{
				num4 = wsFilterCond.Length;
			}
			string wsValue = wsFilterCond.Substring(num3, num4 - num3);
			singlePropertyLdapFilter.ValidateAndSetAttributeValue(wsValue, provider);
			wsFilterCond = singlePropertyLdapFilter.m_wsType;
			if (c != '=')
			{
				wsFilterCond += c;
			}
			wsFilterCond += '=';
			wsFilterCond += singlePropertyLdapFilter.m_wsValue;
			if (flag7)
			{
				return new NotLdapFilter(singlePropertyLdapFilter);
			}
			return singlePropertyLdapFilter;
		}

		private bool ConvertAndCompare(string wsValue)
		{
			bool result = false;
			switch (this.Syntax)
			{
			case DataSyntax.Boolean:
			case DataSyntax.Sid:
			case DataSyntax.Octet:
			case DataSyntax.ObjectIdentifier:
			case DataSyntax.Numeric:
			case DataSyntax.Printable:
			case DataSyntax.Teletex:
			case DataSyntax.IA5:
			case DataSyntax.CaseSensitive:
			case DataSyntax.Unicode:
			case DataSyntax.DSDN:
				result = this.EvaluateStringCond(wsValue);
				break;
			case DataSyntax.Integer:
			case DataSyntax.Enumeration:
			{
				int num = 0;
				if (SinglePropertyLdapFilter.StringToLong(wsValue, out num))
				{
					switch (this.Operator)
					{
					case LdapComparisonOperator.Equal:
						result = (num == (int)this.m_value);
						break;
					case LdapComparisonOperator.GreaterOrEqual:
						result = (num >= (int)this.m_value);
						break;
					case LdapComparisonOperator.LessOrEqual:
						result = (num <= (int)this.m_value);
						break;
					case LdapComparisonOperator.BitAnd:
						result = ((num & (int)this.m_value) == (int)this.m_value);
						break;
					case LdapComparisonOperator.BitOr:
						result = ((num & (int)this.m_value) != 0);
						break;
					}
				}
				break;
			}
			case DataSyntax.LargeInteger:
			{
				long num2 = 0L;
				if (SinglePropertyLdapFilter.StringToLongLong(wsValue, out num2))
				{
					switch (this.Operator)
					{
					case LdapComparisonOperator.Equal:
						result = (num2 == (long)this.m_value);
						break;
					case LdapComparisonOperator.GreaterOrEqual:
						result = (num2 >= (long)this.m_value);
						break;
					case LdapComparisonOperator.LessOrEqual:
						result = (num2 <= (long)this.m_value);
						break;
					case LdapComparisonOperator.BitAnd:
						result = ((num2 & (long)this.m_value) == (long)this.m_value);
						break;
					case LdapComparisonOperator.BitOr:
						result = ((num2 & (long)this.m_value) != 0L);
						break;
					}
				}
				break;
			}
			}
			return result;
		}

		public override bool Evaluate(object[] marshalledAttributes)
		{
			object[] array = null;
			foreach (object[] array2 in marshalledAttributes)
			{
				if (StringComparer.InvariantCultureIgnoreCase.Equals((string)array2[0], this.m_wsType))
				{
					array = array2;
					break;
				}
			}
			if (array == null || array.Length == 0)
			{
				return false;
			}
			if (SinglePropertyLdapFilter.IsPresenceCheck(this.m_wsValue))
			{
				return true;
			}
			if (SinglePropertyLdapFilter.HasWildChar(this.m_wsValue))
			{
				return LdapComparisonOperator.Equal == this.Operator && this.EvaluateWildChar(array);
			}
			for (int j = 1; j < array.Length; j++)
			{
				if (this.ConvertAndCompare((string)array[j]))
				{
					return true;
				}
			}
			return false;
		}

		private bool EvaluateWildChar(object[] AttrIn)
		{
			string[] array = this.m_value as string[];
			int num = array.Length;
			int i = 1;
			while (i < AttrIn.Length)
			{
				string text = (string)AttrIn[i];
				int num2 = 0;
				if (array[0].Length == 0)
				{
					goto IL_7A;
				}
				if (array[0].Length <= text.Length && string.Compare(array[0], text.Substring(0, array[0].Length), !this.m_bCaseSensitive, CultureInfo.InvariantCulture) == 0)
				{
					num2 += array[0].Length;
					goto IL_7A;
				}
				IL_16E:
				i++;
				continue;
				IL_7A:
				int num3 = 0;
				int j;
				for (j = 1; j < num; j++)
				{
					num3 += array[j].Length;
				}
				if (num3 > text.Length - num2)
				{
					goto IL_16E;
				}
				for (j = 1; j < num - 1; j++)
				{
					if (array[j].Length > 0)
					{
						int num4 = text.Length - num3;
						while (num2 <= num4 && string.Compare(array[j], text.Substring(num2, array[j].Length), !this.m_bCaseSensitive, CultureInfo.InvariantCulture) != 0)
						{
							num2++;
						}
						if (num2 > num4)
						{
							break;
						}
						num2 += array[j].Length;
						num3 -= array[j].Length;
					}
				}
				if (j != num - 1)
				{
					goto IL_16E;
				}
				if (num3 == 0)
				{
					return true;
				}
				if (num3 > text.Length - num2)
				{
					goto IL_16E;
				}
				num2 = text.Length - num3;
				if (string.Compare(array[num - 1], text.Substring(num2), !this.m_bCaseSensitive, CultureInfo.InvariantCulture) == 0)
				{
					return true;
				}
				goto IL_16E;
			}
			return false;
		}

		private bool EvaluateStringCond(string wsValue)
		{
			bool result = false;
			int num;
			if (this.m_bCaseSensitive)
			{
				num = SinglePropertyLdapFilter.CaseSensitiveCompare(wsValue, this.m_value as string);
			}
			else
			{
				num = SinglePropertyLdapFilter.CaseInsensitiveCompare(wsValue, this.m_value as string);
			}
			switch (this.Operator)
			{
			case LdapComparisonOperator.Equal:
				result = (num == 0);
				break;
			case LdapComparisonOperator.GreaterOrEqual:
				result = (num >= 0);
				break;
			case LdapComparisonOperator.LessOrEqual:
				result = (num <= 0);
				break;
			}
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			stringBuilder.Append(this.m_wsType);
			switch (this.Operator)
			{
			case LdapComparisonOperator.Equal:
				stringBuilder.Append("=");
				break;
			case LdapComparisonOperator.GreaterOrEqual:
				stringBuilder.Append(">=");
				break;
			case LdapComparisonOperator.LessOrEqual:
				stringBuilder.Append("<=");
				break;
			case LdapComparisonOperator.BitAnd:
				stringBuilder.Append(":");
				stringBuilder.Append(SinglePropertyLdapFilter.LdapMatchingRuleBitAnd);
				stringBuilder.Append(":=");
				break;
			case LdapComparisonOperator.BitOr:
				stringBuilder.Append(":");
				stringBuilder.Append(SinglePropertyLdapFilter.LdapMatchingRuleBitOr);
				stringBuilder.Append(":=");
				break;
			}
			stringBuilder.Append(this.m_wsValue);
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private static readonly string LdapMatchingRuleBitAnd = "1.2.840.113556.1.4.803";

		private static readonly string LdapMatchingRuleBitOr = "1.2.840.113556.1.4.804";

		private string stringFilter;

		private LdapComparisonOperator Operator;

		private bool m_bCaseSensitive;

		private string m_wsType;

		private DataSyntax Syntax;

		private string m_wsValue;

		private object m_value;
	}
}
