using System;

namespace Microsoft.Forefront.Reporting.Common
{
	internal abstract class QueryField
	{
		protected internal QueryField(QueryGroupField parent, int startPos, int endPos)
		{
			this.Parent = parent;
			this.StartPosition = startPos;
			this.EndPosition = endPos;
			if (parent != null)
			{
				this.QueryString = parent.QueryString;
				this.Level = parent.Level + 1;
				this.Compiler = parent.Compiler;
				return;
			}
			this.Level = 1;
		}

		internal string QueryString { get; set; }

		internal int StartPosition { get; set; }

		internal int EndPosition { get; set; }

		internal int Level { get; set; }

		internal bool HasOptionalCriterion { get; set; }

		internal QueryGroupField Parent { get; set; }

		internal QueryCompiler Compiler { get; set; }

		internal abstract string Compile();

		internal string GetField()
		{
			return this.QueryString.Substring(this.StartPosition, this.EndPosition - this.StartPosition);
		}
	}
}
