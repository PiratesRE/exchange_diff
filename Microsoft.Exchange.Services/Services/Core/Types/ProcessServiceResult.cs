using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal delegate void ProcessServiceResult<TValue>(ServiceResult<TValue> result);
}
