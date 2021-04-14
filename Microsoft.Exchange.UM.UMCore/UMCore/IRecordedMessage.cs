using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IRecordedMessage
	{
		void DoSubmit(Importance importance, bool markPrivate, Stack<Participant> recips);

		void DoSubmit(Importance importance);

		void DoPostSubmit();
	}
}
