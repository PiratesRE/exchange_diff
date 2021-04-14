using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class DiagnosticQueryCriteriaAnd : DiagnosticQueryCriteria, IEquatable<DiagnosticQueryCriteriaAnd>
	{
		private DiagnosticQueryCriteriaAnd(DiagnosticQueryCriteria[] nestedCriteria)
		{
			this.nestedCriteria = nestedCriteria;
		}

		public DiagnosticQueryCriteria[] NestedCriteria
		{
			get
			{
				return this.nestedCriteria;
			}
		}

		public static DiagnosticQueryCriteriaAnd Create(params DiagnosticQueryCriteria[] nestedCriteria)
		{
			return new DiagnosticQueryCriteriaAnd(nestedCriteria);
		}

		public override string ToString()
		{
			if (this.description == null)
			{
				StringBuilder stringBuilder = new StringBuilder(200);
				foreach (DiagnosticQueryCriteria diagnosticQueryCriteria in this.nestedCriteria)
				{
					stringBuilder.AppendFormat("{0}{1}", (stringBuilder.Length > 0) ? ", " : string.Empty, diagnosticQueryCriteria.ToString());
				}
				this.description = string.Format("and ({0})", stringBuilder.ToString());
			}
			return this.description;
		}

		public override int GetHashCode()
		{
			if (base.HashCode == null)
			{
				int num = 0;
				foreach (DiagnosticQueryCriteria diagnosticQueryCriteria in this.nestedCriteria)
				{
					num ^= diagnosticQueryCriteria.GetHashCode();
				}
				base.HashCode = new int?(num);
			}
			return base.HashCode.Value;
		}

		public override bool Equals(object obj)
		{
			DiagnosticQueryCriteriaAnd diagnosticQueryCriteriaAnd = obj as DiagnosticQueryCriteriaAnd;
			return diagnosticQueryCriteriaAnd != null && this.Equals(diagnosticQueryCriteriaAnd);
		}

		public bool Equals(DiagnosticQueryCriteriaAnd and)
		{
			if (and.NestedCriteria.Length == this.NestedCriteria.Length)
			{
				foreach (DiagnosticQueryCriteria diagnosticQueryCriteria in this.NestedCriteria)
				{
					bool flag = false;
					foreach (DiagnosticQueryCriteria obj in and.NestedCriteria)
					{
						if (diagnosticQueryCriteria.Equals(obj))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override DiagnosticQueryCriteria Reduce()
		{
			List<DiagnosticQueryCriteria> list = new List<DiagnosticQueryCriteria>(this.nestedCriteria.Length);
			foreach (DiagnosticQueryCriteria diagnosticQueryCriteria in this.nestedCriteria)
			{
				DiagnosticQueryCriteria diagnosticQueryCriteria2 = diagnosticQueryCriteria.Reduce();
				DiagnosticQueryCriteriaAnd diagnosticQueryCriteriaAnd = diagnosticQueryCriteria2 as DiagnosticQueryCriteriaAnd;
				if (diagnosticQueryCriteriaAnd != null)
				{
					list.AddRange(diagnosticQueryCriteriaAnd.nestedCriteria);
				}
				else
				{
					list.Add(diagnosticQueryCriteria2);
				}
			}
			return new DiagnosticQueryCriteriaAnd(list.ToArray());
		}

		private readonly DiagnosticQueryCriteria[] nestedCriteria;

		private string description;
	}
}
