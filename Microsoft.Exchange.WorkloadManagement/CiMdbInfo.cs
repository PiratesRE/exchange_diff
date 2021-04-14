using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class CiMdbInfo
	{
		public CiMdbInfo(IEnumerable<CiMdbCopyInfo> info)
		{
			this.Copies = info;
		}

		public IEnumerable<CiMdbCopyInfo> Copies { get; private set; }

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = string.Join<CiMdbCopyInfo>(";", this.Copies);
			}
			return this.toString;
		}

		private string toString;
	}
}
