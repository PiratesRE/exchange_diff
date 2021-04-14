using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IFeeder : IExecutable, IDiagnosable, IDisposable
	{
		FeederType FeederType { get; }
	}
}
