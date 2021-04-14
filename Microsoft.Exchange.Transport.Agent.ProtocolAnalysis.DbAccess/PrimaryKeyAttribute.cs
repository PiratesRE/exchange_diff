using System;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal sealed class PrimaryKeyAttribute : Attribute
	{
	}
}
