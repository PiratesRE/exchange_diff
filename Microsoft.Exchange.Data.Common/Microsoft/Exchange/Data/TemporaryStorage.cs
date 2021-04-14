using System;
using System.IO;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data
{
	public static class TemporaryStorage
	{
		public static Stream Create()
		{
			TemporaryDataStorage temporaryDataStorage = new TemporaryDataStorage();
			Stream result = temporaryDataStorage.OpenWriteStream(false);
			temporaryDataStorage.Release();
			return result;
		}
	}
}
