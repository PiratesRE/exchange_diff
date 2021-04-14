using System;

namespace Microsoft.Exchange.Server.Storage.FullTextIndex
{
	public static class Globals
	{
		public static void Initialize()
		{
			FullTextIndexSchema.Initialize();
		}

		public static void Terminate()
		{
			FullTextIndexSchema.Terminate();
		}
	}
}
