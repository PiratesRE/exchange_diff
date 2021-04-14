using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal abstract class BackgroundJobBackendBase : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
