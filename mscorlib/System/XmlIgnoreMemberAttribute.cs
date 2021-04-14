using System;

namespace System
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	internal sealed class XmlIgnoreMemberAttribute : Attribute
	{
	}
}
