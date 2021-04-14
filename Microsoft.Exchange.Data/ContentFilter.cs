using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal abstract class ContentFilter : SinglePropertyFilter
	{
		public ContentFilter(PropertyDefinition property, MatchOptions matchOptions, MatchFlags matchFlags) : base(property)
		{
			this.matchOptions = matchOptions;
			this.matchFlags = matchFlags;
		}

		public MatchOptions MatchOptions
		{
			get
			{
				return this.matchOptions;
			}
		}

		public MatchFlags MatchFlags
		{
			get
			{
				return this.matchFlags;
			}
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(");
			sb.Append(this.matchOptions.ToString());
			sb.Append(" ");
			sb.Append(this.matchFlags.ToString());
			sb.Append("(");
			sb.Append(base.Property.Name);
			sb.Append(")=");
			sb.Append(this.StringValue);
			sb.Append(")");
		}

		protected abstract string StringValue { get; }

		private readonly MatchFlags matchFlags;

		private readonly MatchOptions matchOptions;
	}
}
