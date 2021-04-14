using System;
using Microsoft.Exchange.Data.ContentTypes.Tnef;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal interface IMapiPropertyAccess
	{
		object GetProperty(TnefPropertyTag tag);

		object GetProperty(TnefNameTag nameId);

		void SetProperty(TnefPropertyTag tag, object value);

		void SetProperty(TnefNameTag nameId, object value);
	}
}
