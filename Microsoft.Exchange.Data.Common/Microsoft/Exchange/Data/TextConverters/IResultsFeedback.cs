using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal interface IResultsFeedback
	{
		void Set(ConfigParameter parameterId, object val);
	}
}
