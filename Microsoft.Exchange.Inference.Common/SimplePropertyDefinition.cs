using System;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Common
{
	internal sealed class SimplePropertyDefinition : PropertyDefinition
	{
		internal SimplePropertyDefinition(string name, Type type, PropertyFlag flags) : base(name, type, flags)
		{
		}

		internal SimplePropertyDefinition(string name, Type type) : base(name, type, PropertyFlag.None)
		{
		}
	}
}
