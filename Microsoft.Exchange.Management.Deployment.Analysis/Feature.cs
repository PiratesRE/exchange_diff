using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public abstract class Feature
	{
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
