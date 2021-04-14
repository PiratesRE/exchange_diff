using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class FormattedADObjectIdCollection
	{
		internal FormattedADObjectIdCollection(IEnumerable<ADObjectId> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			this.objs = collection;
		}

		public IEnumerable<ADObjectId> AssignmentChains
		{
			get
			{
				return this.objs;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (ADObjectId adobjectId in this.objs)
			{
				if (!flag)
				{
					stringBuilder.Append('\\');
				}
				else
				{
					flag = false;
				}
				stringBuilder.Append(adobjectId.Name);
			}
			return stringBuilder.ToString();
		}

		private IEnumerable<ADObjectId> objs;
	}
}
