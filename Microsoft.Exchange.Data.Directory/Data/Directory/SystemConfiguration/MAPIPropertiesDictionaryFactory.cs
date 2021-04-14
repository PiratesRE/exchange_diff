using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class MAPIPropertiesDictionaryFactory
	{
		internal static MAPIPropertiesDictionary GetPropertiesDictionary()
		{
			if (MAPIPropertiesDictionaryFactory.propertiesDictionary == null)
			{
				lock (MAPIPropertiesDictionaryFactory.objLock)
				{
					if (MAPIPropertiesDictionaryFactory.propertiesDictionary == null)
					{
						MAPIPropertiesDictionaryFactory.propertiesDictionary = new MAPIPropertiesDictionary();
					}
				}
			}
			return MAPIPropertiesDictionaryFactory.propertiesDictionary;
		}

		private static object objLock = new object();

		private static volatile MAPIPropertiesDictionary propertiesDictionary;
	}
}
