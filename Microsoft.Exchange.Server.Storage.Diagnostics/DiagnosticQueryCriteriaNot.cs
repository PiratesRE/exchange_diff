using System;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class DiagnosticQueryCriteriaNot : DiagnosticQueryCriteria, IEquatable<DiagnosticQueryCriteriaNot>
	{
		private DiagnosticQueryCriteriaNot(DiagnosticQueryCriteria nestedCriterion)
		{
			this.nestedCriterion = nestedCriterion;
		}

		public DiagnosticQueryCriteria NestedCriterion
		{
			get
			{
				return this.nestedCriterion;
			}
		}

		public static DiagnosticQueryCriteriaNot Create(DiagnosticQueryCriteria nestedCriterion)
		{
			return new DiagnosticQueryCriteriaNot(nestedCriterion);
		}

		public override string ToString()
		{
			return string.Format("not ({0})", this.NestedCriterion.ToString());
		}

		public override int GetHashCode()
		{
			if (base.HashCode == null)
			{
				base.HashCode = new int?(this.nestedCriterion.GetHashCode());
			}
			return base.HashCode.Value;
		}

		public override bool Equals(object obj)
		{
			DiagnosticQueryCriteriaNot diagnosticQueryCriteriaNot = obj as DiagnosticQueryCriteriaNot;
			return diagnosticQueryCriteriaNot != null && this.Equals(diagnosticQueryCriteriaNot);
		}

		public bool Equals(DiagnosticQueryCriteriaNot not)
		{
			return not.NestedCriterion.Equals(this.NestedCriterion);
		}

		public override DiagnosticQueryCriteria Reduce()
		{
			DiagnosticQueryCriteria diagnosticQueryCriteria = this.nestedCriterion.Reduce();
			DiagnosticQueryCriteriaNot diagnosticQueryCriteriaNot = diagnosticQueryCriteria as DiagnosticQueryCriteriaNot;
			if (diagnosticQueryCriteriaNot != null)
			{
				return diagnosticQueryCriteriaNot.nestedCriterion;
			}
			return new DiagnosticQueryCriteriaNot(diagnosticQueryCriteria);
		}

		private readonly DiagnosticQueryCriteria nestedCriterion;
	}
}
