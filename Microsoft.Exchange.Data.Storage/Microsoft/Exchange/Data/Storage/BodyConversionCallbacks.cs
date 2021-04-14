using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct BodyConversionCallbacks
	{
		public HtmlCallbackBase HtmlCallback;

		public RtfCallbackBase RtfCallback;
	}
}
