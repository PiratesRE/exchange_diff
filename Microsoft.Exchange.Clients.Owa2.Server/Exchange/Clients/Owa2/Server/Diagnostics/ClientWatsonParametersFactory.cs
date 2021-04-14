using System;
using System.Collections.Concurrent;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal class ClientWatsonParametersFactory
	{
		public static ClientWatsonParameters GetInstance(string owaVersion)
		{
			if (!ClientWatsonParametersFactory.clientWatsonParametersCollection.ContainsKey(owaVersion))
			{
				ClientWatsonParameters value = new ClientWatsonParameters(owaVersion);
				ClientWatsonParametersFactory.clientWatsonParametersCollection.TryAdd(owaVersion, value);
			}
			return ClientWatsonParametersFactory.clientWatsonParametersCollection[owaVersion];
		}

		public static void Shutdown()
		{
			foreach (string key in ClientWatsonParametersFactory.clientWatsonParametersCollection.Keys)
			{
				ClientWatsonParameters clientWatsonParameters;
				if (ClientWatsonParametersFactory.clientWatsonParametersCollection.ContainsKey(key) && ClientWatsonParametersFactory.clientWatsonParametersCollection.TryGetValue(key, out clientWatsonParameters) && clientWatsonParameters != null)
				{
					clientWatsonParameters.Dispose();
				}
			}
			ClientWatsonParametersFactory.clientWatsonParametersCollection.Clear();
		}

		private static ConcurrentDictionary<string, ClientWatsonParameters> clientWatsonParametersCollection = new ConcurrentDictionary<string, ClientWatsonParameters>();
	}
}
