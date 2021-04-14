using System;

namespace Microsoft.Exchange.Management.Analysis.Features
{
	internal abstract class Feature
	{
		public Feature(bool allowsMultiple, bool isInheritable)
		{
			this.AllowsMultiple = allowsMultiple;
			this.IsInheritable = isInheritable;
		}

		public bool AllowsMultiple { get; private set; }

		public bool IsInheritable { get; private set; }

		public override string ToString()
		{
			string text = "Feature";
			string text2 = base.GetType().Name;
			if (text2.EndsWith(text, StringComparison.OrdinalIgnoreCase))
			{
				text2 = text2.Remove(text2.Length - text.Length);
			}
			return text2;
		}
	}
}
