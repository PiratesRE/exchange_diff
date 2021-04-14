using System;
using System.IO;
using System.Reflection;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal static class Constants
	{
		public static readonly string MExRuntimeLocation = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
	}
}
