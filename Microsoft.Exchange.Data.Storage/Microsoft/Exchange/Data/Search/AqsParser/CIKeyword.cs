using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search.AqsParser
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CIKeyword : Attribute
	{
	}
}
