using System;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public abstract class DiagnosticQueryStringFormatter : DiagnosticQueryFormatter<StringBuilder>
	{
		protected DiagnosticQueryStringFormatter(DiagnosticQueryResults results) : base(results)
		{
			this.builder = new StringBuilder(4096);
		}

		protected StringBuilder Builder
		{
			get
			{
				return this.builder;
			}
		}

		public override StringBuilder FormatResults()
		{
			this.WriteHeader();
			this.WriteValues();
			return this.builder;
		}

		protected abstract void WriteHeader();

		protected abstract void WriteValues();

		private readonly StringBuilder builder;
	}
}
