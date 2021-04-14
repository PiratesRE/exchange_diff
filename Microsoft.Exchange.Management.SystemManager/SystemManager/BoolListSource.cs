using System;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class BoolListSource : ObjectListSource
	{
		public BoolListSource()
		{
			bool[] array = new bool[2];
			array[0] = true;
			base..ctor(array);
		}

		protected override string GetValueText(object objectValue)
		{
			return Convert.ToBoolean(objectValue) ? Strings.TrueString : Strings.FalseString;
		}
	}
}
