using System;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public abstract class DiagnosticQueryCriteria
	{
		protected int? HashCode
		{
			get
			{
				return this.hashCode;
			}
			set
			{
				this.hashCode = value;
			}
		}

		public abstract DiagnosticQueryCriteria Reduce();

		private int? hashCode;
	}
}
