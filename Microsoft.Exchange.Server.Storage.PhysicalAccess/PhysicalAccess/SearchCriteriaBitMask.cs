using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class SearchCriteriaBitMask : SearchCriteria
	{
		protected SearchCriteriaBitMask(Column lhs, Column rhs, SearchCriteriaBitMask.SearchBitMaskOp op)
		{
			this.lhs = lhs;
			this.op = op;
			this.rhs = rhs;
		}

		public Column Lhs
		{
			get
			{
				return this.lhs;
			}
		}

		public SearchCriteriaBitMask.SearchBitMaskOp Op
		{
			get
			{
				return this.op;
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
			object o = this.Lhs.Evaluate(twir);
			object o2 = this.Rhs.Evaluate(twir);
			long num = SearchCriteriaBitMask.ConvertToInt64(o);
			long num2 = SearchCriteriaBitMask.ConvertToInt64(o2);
			bool result;
			switch (this.Op)
			{
			case SearchCriteriaBitMask.SearchBitMaskOp.EqualToZero:
				result = ((num & num2) == 0L);
				break;
			case SearchCriteriaBitMask.SearchBitMaskOp.NotEqualToZero:
				result = ((num & num2) != 0L);
				break;
			default:
				result = false;
				break;
			}
			return result;
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

		internal override bool CanBeNegated
		{
			get
			{
				return true;
			}
		}

		internal static void BitMaskOpAsString(SearchCriteriaBitMask.SearchBitMaskOp op, StringBuilder sb)
		{
			switch (op)
			{
			case SearchCriteriaBitMask.SearchBitMaskOp.EqualToZero:
				sb.Append(" = 0");
				return;
			case SearchCriteriaBitMask.SearchBitMaskOp.NotEqualToZero:
				sb.Append(" <> 0");
				return;
			default:
				return;
			}
		}

		internal override SearchCriteria Negate()
		{
			if (!this.CanBeNegated)
			{
				return base.Negate();
			}
			SearchCriteriaBitMask.SearchBitMaskOp searchBitMaskOp;
			switch (this.op)
			{
			case SearchCriteriaBitMask.SearchBitMaskOp.EqualToZero:
				searchBitMaskOp = SearchCriteriaBitMask.SearchBitMaskOp.NotEqualToZero;
				break;
			case SearchCriteriaBitMask.SearchBitMaskOp.NotEqualToZero:
				searchBitMaskOp = SearchCriteriaBitMask.SearchBitMaskOp.EqualToZero;
				break;
			default:
				return base.Negate();
			}
			return Factory.CreateSearchCriteriaBitMask(this.lhs, this.rhs, searchBitMaskOp);
		}

		public override void AppendToString(StringBuilder sb, StringFormatOptions formatOptions)
		{
			sb.Append("BITMASK(");
			this.lhs.AppendToString(sb, formatOptions);
			sb.Append(" & ");
			this.rhs.AppendToString(sb, formatOptions);
			SearchCriteriaBitMask.BitMaskOpAsString(this.op, sb);
			sb.Append(")");
		}

		private static long ConvertToInt64(object o)
		{
			if (o == null)
			{
				return 0L;
			}
			if (o is int)
			{
				return (long)((ulong)((int)o));
			}
			if (o is short)
			{
				return (long)((ulong)((ushort)((short)o)));
			}
			return (long)o;
		}

		protected override bool CriteriaEquivalent(SearchCriteria other)
		{
			SearchCriteriaBitMask searchCriteriaBitMask = other as SearchCriteriaBitMask;
			return searchCriteriaBitMask != null && this.lhs == searchCriteriaBitMask.lhs && this.op == searchCriteriaBitMask.op && this.rhs == searchCriteriaBitMask.rhs;
		}

		private readonly Column lhs;

		private readonly SearchCriteriaBitMask.SearchBitMaskOp op;

		private readonly Column rhs;

		public enum SearchBitMaskOp
		{
			EqualToZero,
			NotEqualToZero
		}
	}
}
