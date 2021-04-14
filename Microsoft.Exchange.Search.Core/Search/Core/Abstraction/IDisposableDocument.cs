using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IDisposableDocument : IDocument, IPropertyBag, IReadOnlyPropertyBag, IDisposeTrackable, IDisposable
	{
	}
}
