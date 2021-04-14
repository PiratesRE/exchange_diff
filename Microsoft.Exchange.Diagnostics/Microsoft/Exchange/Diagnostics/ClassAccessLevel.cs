using System;

namespace Microsoft.Exchange.Diagnostics
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public class ClassAccessLevel : Attribute
	{
		public ClassAccessLevel(AccessLevel accessLevel)
		{
			this.AccessLevel = accessLevel;
		}

		public AccessLevel AccessLevel { get; private set; }
	}
}
