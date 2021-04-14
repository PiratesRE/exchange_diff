using System;
using Microsoft.Exchange.Management.SystemManager;

namespace Microsoft.Exchange.Management.SnapIn.Esm.Toolbox
{
	public abstract class DataProvider
	{
		public DataList<Tool> Tools
		{
			get
			{
				return this.tools;
			}
		}

		public abstract void Query();

		private DataList<Tool> tools = new DataList<Tool>();
	}
}
