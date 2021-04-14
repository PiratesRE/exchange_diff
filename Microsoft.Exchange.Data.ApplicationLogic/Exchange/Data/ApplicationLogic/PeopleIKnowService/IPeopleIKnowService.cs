using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.PeopleIKnowService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPeopleIKnowService
	{
		string GetSerializedPeopleIKnowGraph(IMailboxSession mailboxSession, IXSOFactory xsoFactory);

		IComparer<string> GetRelevancyComparer(string serializedPeopleIKnow);
	}
}
