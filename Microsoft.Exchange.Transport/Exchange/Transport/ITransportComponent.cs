using System;

namespace Microsoft.Exchange.Transport
{
	internal interface ITransportComponent
	{
		void Load();

		void Unload();

		string OnUnhandledException(Exception e);
	}
}
