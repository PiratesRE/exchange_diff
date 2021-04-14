using System;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class DiagnosticQueryCriteriaCompare : DiagnosticQueryCriteria, IEquatable<DiagnosticQueryCriteriaCompare>
	{
		private DiagnosticQueryCriteriaCompare(string columnName, DiagnosticQueryOperator queryOperator, string value)
		{
			this.columnName = columnName;
			this.queryOperator = queryOperator;
			this.value = value;
		}

		public string ColumnName
		{
			get
			{
				return this.columnName;
			}
		}

		public DiagnosticQueryOperator QueryOperator
		{
			get
			{
				return this.queryOperator;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public static DiagnosticQueryCriteriaCompare Create(string columnName, DiagnosticQueryOperator queryOperator, string value)
		{
			return new DiagnosticQueryCriteriaCompare(columnName, queryOperator, value);
		}

		public override string ToString()
		{
			double num;
			string format = (double.TryParse(this.Value, out num) || this.Value == null) ? "{0} {1} {2}" : "{0} {1} \"{2}\"";
			return string.Format(format, this.ColumnName, this.QueryOperator, this.Value ?? "NULL");
		}

		public override int GetHashCode()
		{
			if (base.HashCode == null)
			{
				if (this.Value != null)
				{
					base.HashCode = new int?(this.ColumnName.GetHashCode() ^ this.QueryOperator.GetHashCode() ^ this.Value.GetHashCode());
				}
				else
				{
					base.HashCode = new int?(this.ColumnName.GetHashCode() ^ this.QueryOperator.GetHashCode());
				}
			}
			return base.HashCode.Value;
		}

		public override bool Equals(object obj)
		{
			DiagnosticQueryCriteriaCompare diagnosticQueryCriteriaCompare = obj as DiagnosticQueryCriteriaCompare;
			return diagnosticQueryCriteriaCompare != null && this.Equals(diagnosticQueryCriteriaCompare);
		}

		public bool Equals(DiagnosticQueryCriteriaCompare compare)
		{
			return compare.ColumnName.Equals(this.ColumnName) && compare.QueryOperator.Equals(this.QueryOperator) && ((compare.Value == null && this.Value == null) || (compare.Value != null && this.Value != null && compare.Value.Equals(this.Value)));
		}

		public override DiagnosticQueryCriteria Reduce()
		{
			return this;
		}

		private readonly string columnName;

		private readonly DiagnosticQueryOperator queryOperator;

		private readonly string value;
	}
}
