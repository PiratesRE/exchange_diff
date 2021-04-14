using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Clutter
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IClutterOverrideManager : ICollection<SmtpAddress>, IEnumerable<SmtpAddress>, IEnumerable, IDisposable
	{
		void Save();
	}
}
