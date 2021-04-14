using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public sealed class DiagnosticQueryResults
	{
		private DiagnosticQueryResults(IList<string> names, IList<Type> types, IList<uint> widths, IList<object[]> values, bool truncated, bool interrupted)
		{
			this.names = names;
			this.types = types;
			this.widths = widths;
			this.values = values;
			this.truncated = truncated;
			this.interrupted = interrupted;
		}

		public IList<string> Names
		{
			get
			{
				return this.names;
			}
		}

		public IList<Type> Types
		{
			get
			{
				return this.types;
			}
		}

		public IList<uint> Widths
		{
			get
			{
				return this.widths;
			}
		}

		public IList<object[]> Values
		{
			get
			{
				return this.values;
			}
		}

		public bool IsTruncated
		{
			get
			{
				return this.truncated;
			}
		}

		public bool IsInterrupted
		{
			get
			{
				return this.interrupted;
			}
		}

		public static DiagnosticQueryResults Create(IList<string> names, IList<Type> types, IList<uint> widths, IList<object[]> values, bool truncated, bool interrupted)
		{
			return new DiagnosticQueryResults(names, types, widths, values, truncated, interrupted);
		}

		private readonly IList<string> names;

		private readonly IList<Type> types;

		private readonly IList<uint> widths;

		private readonly IList<object[]> values;

		private readonly bool truncated;

		private readonly bool interrupted;
	}
}
