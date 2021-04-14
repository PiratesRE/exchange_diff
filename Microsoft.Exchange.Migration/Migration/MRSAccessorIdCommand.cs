using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Migration
{
	internal class MRSAccessorIdCommand : MrsAccessorCommand
	{
		public MRSAccessorIdCommand(string name, ICollection<Type> ignoreExceptions, ICollection<Type> transientExceptions, object identity) : base(name, ignoreExceptions, transientExceptions)
		{
			base.Identity = identity;
		}
	}
}
