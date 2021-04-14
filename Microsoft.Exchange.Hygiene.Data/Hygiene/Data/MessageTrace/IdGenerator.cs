using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class IdGenerator
	{
		public static Guid GenerateIdentifier(IdScope scope)
		{
			return CombGuidGenerator.NewGuid();
		}
	}
}
