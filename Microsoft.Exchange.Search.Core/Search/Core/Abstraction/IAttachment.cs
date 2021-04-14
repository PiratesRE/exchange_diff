using System;
using System.IO;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IAttachment : IDisposable
	{
		Stream GetContentStream();
	}
}
