using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[AttributeUsage(AttributeTargets.Field)]
	internal sealed class DetectCodepageAttribute : Attribute
	{
	}
}
