using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public static class ResultPaneProfileLoader
	{
		public static ObjectPickerProfileLoader Loader
		{
			get
			{
				return ResultPaneProfileLoader.loader;
			}
		}

		private static readonly ObjectPickerProfileLoader loader = new ObjectPickerProfileLoader(1);
	}
}
