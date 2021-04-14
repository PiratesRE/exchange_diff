using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal interface IPregatherParticipants
	{
		void Pregather(StoreObject storeObject, List<Participant> participants);
	}
}
