using System;
using System.IO;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal interface IRelayable
	{
		void WriteTo(Stream stream);
	}
}
