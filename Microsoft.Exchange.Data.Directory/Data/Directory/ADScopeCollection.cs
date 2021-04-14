using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class ADScopeCollection : ReadOnlyCollection<ADScope>
	{
		internal ADScopeCollection(IList<ADScope> aDScopes) : base(aDScopes)
		{
			if (aDScopes == null)
			{
				throw new ArgumentNullException("adScopes");
			}
			if (aDScopes.Count == 0)
			{
				throw new ArgumentException("adScopes");
			}
		}

		private ADScopeCollection() : base(new ADScope[0])
		{
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < base.Count - 1; i++)
			{
				if (base[i] != null)
				{
					stringBuilder.Append(base[i].ToString());
					stringBuilder.Append(",");
				}
			}
			if (base.Count != 0 && base[base.Count - 1] != null)
			{
				stringBuilder.Append(base[base.Count - 1].ToString());
			}
			return stringBuilder.ToString();
		}

		internal static readonly ADScopeCollection Empty = new ADScopeCollection();

		internal static readonly ADScopeCollection[] EmptyArray = new ADScopeCollection[0];
	}
}
