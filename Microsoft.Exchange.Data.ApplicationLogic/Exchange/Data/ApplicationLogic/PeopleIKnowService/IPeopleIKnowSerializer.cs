using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.PeopleIKnowService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IPeopleIKnowSerializer
	{
		string Serialize(PeopleIKnowGraph peopleIKnowGraph);

		PeopleIKnowGraph Deserialize(string serializedPeopleIKnow);
	}
}
