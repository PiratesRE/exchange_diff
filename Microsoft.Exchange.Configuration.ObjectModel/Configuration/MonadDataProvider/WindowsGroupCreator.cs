using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class WindowsGroupCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"Notes",
				"Members"
			};
		}
	}
}
