using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IPersistableDocumentAdapter : IDocumentAdapter
	{
		void Save();

		void Save(bool reaload);
	}
}
