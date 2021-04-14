using System;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class BoolAsYesNoListSource : ObjectListSource
	{
		public BoolAsYesNoListSource()
		{
			bool[] array = new bool[2];
			array[0] = true;
			base..ctor(array);
		}

		protected override string GetValueText(object objectValue)
		{
			if (!Convert.ToBoolean(objectValue))
			{
				return Strings.NoString.ToString();
			}
			return Strings.YesString.ToString();
		}
	}
}
