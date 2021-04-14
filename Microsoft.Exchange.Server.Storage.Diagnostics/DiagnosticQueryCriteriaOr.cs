using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class DiagnosticQueryCriteriaOr : DiagnosticQueryCriteria, IEquatable<DiagnosticQueryCriteriaOr>
	{
		private DiagnosticQueryCriteriaOr(DiagnosticQueryCriteria[] nestedCriteria)
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

		public static DiagnosticQueryCriteriaOr Create(params DiagnosticQueryCriteria[] nestedCriteria)
		{
			return new DiagnosticQueryCriteriaOr(nestedCriteria);
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

		public override string ToString()
		{
			if (this.description == null)
			{
				StringBuilder stringBuilder = new StringBuilder(200);
				foreach (DiagnosticQueryCriteria diagnosticQueryCriteria in this.nestedCriteria)
				{
					stringBuilder.AppendFormat("{0}{1}", (stringBuilder.Length > 0) ? ", " : string.Empty, diagnosticQueryCriteria.ToString());
					this.description = string.Format("or ({0})", stringBuilder.ToString());
				}
			}
			return this.description;
		}

		public override bool Equals(object obj)
		{
			DiagnosticQueryCriteriaOr diagnosticQueryCriteriaOr = obj as DiagnosticQueryCriteriaOr;
			return diagnosticQueryCriteriaOr != null && this.Equals(diagnosticQueryCriteriaOr);
		}

		public bool Equals(DiagnosticQueryCriteriaOr or)
		{
			if (or.NestedCriteria.Length == this.NestedCriteria.Length)
			{
				foreach (DiagnosticQueryCriteria diagnosticQueryCriteria in this.NestedCriteria)
				{
					bool flag = false;
					foreach (DiagnosticQueryCriteria obj in or.NestedCriteria)
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
				DiagnosticQueryCriteriaOr diagnosticQueryCriteriaOr = diagnosticQueryCriteria2 as DiagnosticQueryCriteriaOr;
				if (diagnosticQueryCriteriaOr != null)
				{
					list.AddRange(diagnosticQueryCriteriaOr.nestedCriteria);
				}
				else
				{
					list.Add(diagnosticQueryCriteria2);
				}
			}
			return new DiagnosticQueryCriteriaOr(list.ToArray());
		}

		private readonly DiagnosticQueryCriteria[] nestedCriteria;

		private string description;
	}
}
