using System;
using System.IO;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public interface IListView
	{
		int TotalCount { get; }

		void Render(TextWriter writer);

		void RenderForCompactWebPart(TextWriter writer);
	}
}
