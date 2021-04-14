using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class DbAccessServices
	{
		public static DataTable GetTableByType(Type t)
		{
			Type typeFromHandle = typeof(Database);
			return (DataTable)typeFromHandle.InvokeMember(t.Name, BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty, null, null, null, CultureInfo.InvariantCulture);
		}
	}
}
