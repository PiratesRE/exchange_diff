using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.PeopleIKnowService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPeopleIKnowServiceFactory
	{
		IPeopleIKnowService CreatePeopleIKnowService(ITracer tracer);
	}
}
