using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDocument : IDocumentLibraryItem, IReadOnlyPropertyBag
	{
		long Size { get; }

		Stream GetDocument();
	}
}
