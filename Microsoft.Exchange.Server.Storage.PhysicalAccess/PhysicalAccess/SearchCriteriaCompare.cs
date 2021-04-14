using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class SearchCriteriaCompare : SearchCriteria
	{
		protected SearchCriteriaCompare(Column lhs, SearchCriteriaCompare.SearchRelOp op, Column rhs)
		{
			this.lhs = lhs;
			this.relOp = op;
			this.rhs = rhs;
		}

		public Column Lhs
		{
			get
			{
				return this.lhs;
			}
		}

		public SearchCriteriaCompare.SearchRelOp RelOp
		{
			get
			{
				return this.relOp;
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
			bool flag = false;
			bool flag2 = (byte)(this.Lhs.ExtendedTypeCode & ExtendedTypeCode.MVFlag) == 16 && (this.Lhs.ExtendedTypeCode & (ExtendedTypeCode)239) == this.Rhs.ExtendedTypeCode;
			bool flag3 = (byte)(this.Rhs.ExtendedTypeCode & ExtendedTypeCode.MVFlag) == 16 && (this.Rhs.ExtendedTypeCode & (ExtendedTypeCode)239) == this.Lhs.ExtendedTypeCode;
			if (flag2 || flag3)
			{
				if (this.RelOp != SearchCriteriaCompare.SearchRelOp.Equal && this.RelOp != SearchCriteriaCompare.SearchRelOp.NotEqual)
				{
					throw new StoreException((LID)46056U, ErrorCodeValue.TooComplex, "One of comparison supports only Equal or NotEqual operators.");
				}
				Array array;
				object obj3;
				if (flag2)
				{
					array = (Array)obj;
					obj3 = obj2;
				}
				else
				{
					array = (Array)obj2;
					obj3 = obj;
				}
				if (obj3 is LargeValue)
				{
					throw new StoreException((LID)48768U, ErrorCodeValue.TooComplex, "One of comparison is LargeValue.");
				}
				int num = (array == null) ? 0 : array.Length;
				for (int i = 0; i < num; i++)
				{
					if (ValueHelper.ValuesCompare(array.GetValue(i), obj3, compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
					{
						flag = true;
						break;
					}
				}
				if (this.RelOp == SearchCriteriaCompare.SearchRelOp.NotEqual)
				{
					flag = !flag;
				}
			}
			else
			{
				if (obj is LargeValue || obj2 is LargeValue)
				{
					throw new StoreException((LID)49280U, ErrorCodeValue.TooComplex, "One of comparison is LargeValue.");
				}
				int num2 = ValueHelper.ValuesCompare(obj, obj2, compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
				switch (this.RelOp)
				{
				case SearchCriteriaCompare.SearchRelOp.Equal:
					flag = (num2 == 0);
					break;
				case SearchCriteriaCompare.SearchRelOp.NotEqual:
					flag = (num2 != 0);
					break;
				case SearchCriteriaCompare.SearchRelOp.LessThan:
					flag = (num2 < 0);
					break;
				case SearchCriteriaCompare.SearchRelOp.LessThanEqual:
					flag = (num2 <= 0);
					break;
				case SearchCriteriaCompare.SearchRelOp.GreaterThan:
					flag = (num2 > 0);
					break;
				case SearchCriteriaCompare.SearchRelOp.GreaterThanEqual:
					flag = (num2 >= 0);
					break;
				default:
					flag = false;
					break;
				}
			}
			return flag;
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

		protected override SearchCriteria InspectAndFixChildren(SearchCriteria.InspectAndFixCriteriaDelegate callback, CompareInfo compareInfo, bool simplifyNegation)
		{
			ConstantColumn col = this.lhs as ConstantColumn;
			if (!(col != null))
			{
				return this;
			}
			ConstantColumn col2 = this.rhs as ConstantColumn;
			if (!(col2 != null))
			{
				return Factory.CreateSearchCriteriaCompare(this.rhs, this.InvertSearchRelOp(), this.lhs);
			}
			if (!this.Evaluate(EmptyTwir.Instance, compareInfo))
			{
				return Factory.CreateSearchCriteriaFalse();
			}
			return Factory.CreateSearchCriteriaTrue();
		}

		internal SearchCriteriaCompare.SearchRelOp InvertSearchRelOp()
		{
			SearchCriteriaCompare.SearchRelOp result = this.relOp;
			switch (this.relOp)
			{
			case SearchCriteriaCompare.SearchRelOp.LessThan:
				result = SearchCriteriaCompare.SearchRelOp.GreaterThan;
				break;
			case SearchCriteriaCompare.SearchRelOp.LessThanEqual:
				result = SearchCriteriaCompare.SearchRelOp.GreaterThanEqual;
				break;
			case SearchCriteriaCompare.SearchRelOp.GreaterThan:
				result = SearchCriteriaCompare.SearchRelOp.LessThan;
				break;
			case SearchCriteriaCompare.SearchRelOp.GreaterThanEqual:
				result = SearchCriteriaCompare.SearchRelOp.LessThanEqual;
				break;
			}
			return result;
		}

		internal SearchCriteriaCompare.SearchRelOp NegateSearchRelOp()
		{
			SearchCriteriaCompare.SearchRelOp result = this.relOp;
			switch (this.relOp)
			{
			case SearchCriteriaCompare.SearchRelOp.Equal:
				result = SearchCriteriaCompare.SearchRelOp.NotEqual;
				break;
			case SearchCriteriaCompare.SearchRelOp.NotEqual:
				result = SearchCriteriaCompare.SearchRelOp.Equal;
				break;
			case SearchCriteriaCompare.SearchRelOp.LessThan:
				result = SearchCriteriaCompare.SearchRelOp.GreaterThanEqual;
				break;
			case SearchCriteriaCompare.SearchRelOp.LessThanEqual:
				result = SearchCriteriaCompare.SearchRelOp.GreaterThan;
				break;
			case SearchCriteriaCompare.SearchRelOp.GreaterThan:
				result = SearchCriteriaCompare.SearchRelOp.LessThanEqual;
				break;
			case SearchCriteriaCompare.SearchRelOp.GreaterThanEqual:
				result = SearchCriteriaCompare.SearchRelOp.LessThan;
				break;
			}
			return result;
		}

		internal override bool CanBeNegated
		{
			get
			{
				return true;
			}
		}

		internal override SearchCriteria Negate()
		{
			if (!this.CanBeNegated)
			{
				return base.Negate();
			}
			SearchCriteriaCompare.SearchRelOp searchRelOp = this.NegateSearchRelOp();
			if (searchRelOp == this.relOp)
			{
				return base.Negate();
			}
			return Factory.CreateSearchCriteriaCompare(this.lhs, searchRelOp, this.rhs);
		}

		public override void AppendToString(StringBuilder sb, StringFormatOptions formatOptions)
		{
			sb.Append("CMP(");
			this.lhs.AppendToString(sb, formatOptions);
			SearchCriteriaCompare.RelOpAsString(this.relOp, sb);
			this.rhs.AppendToString(sb, formatOptions);
			sb.Append(")");
		}

		internal static void RelOpAsString(SearchCriteriaCompare.SearchRelOp op, StringBuilder sb)
		{
			switch (op)
			{
			case SearchCriteriaCompare.SearchRelOp.Equal:
				sb.Append(" = ");
				return;
			case SearchCriteriaCompare.SearchRelOp.NotEqual:
				sb.Append(" <> ");
				return;
			case SearchCriteriaCompare.SearchRelOp.LessThan:
				sb.Append(" < ");
				return;
			case SearchCriteriaCompare.SearchRelOp.LessThanEqual:
				sb.Append(" <= ");
				return;
			case SearchCriteriaCompare.SearchRelOp.GreaterThan:
				sb.Append(" > ");
				return;
			case SearchCriteriaCompare.SearchRelOp.GreaterThanEqual:
				sb.Append(" >= ");
				return;
			default:
				return;
			}
		}

		protected override bool CriteriaEquivalent(SearchCriteria other)
		{
			SearchCriteriaCompare searchCriteriaCompare = other as SearchCriteriaCompare;
			return searchCriteriaCompare != null && this.lhs == searchCriteriaCompare.lhs && this.rhs == searchCriteriaCompare.rhs && this.relOp == searchCriteriaCompare.relOp;
		}

		private readonly Column lhs;

		private readonly SearchCriteriaCompare.SearchRelOp relOp;

		private readonly Column rhs;

		public enum SearchRelOp
		{
			Equal,
			NotEqual,
			LessThan,
			LessThanEqual,
			GreaterThan,
			GreaterThanEqual
		}
	}
}
