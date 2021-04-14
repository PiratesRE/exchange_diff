using System;

namespace Microsoft.Exchange.Data.Transport.Storage
{
	internal class StorageEventBindings
	{
		public const string EventOnLoadedMessage = "OnLoadedMessage";

		public static readonly string[] All = new string[]
		{
			"OnLoadedMessage"
		};
	}
}
