using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.ApplicationLogic.PeopleIKnowService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ModernGroupMemberComparerByRelevanceScore : IComparer<ModernGroupMemberType>
	{
		public ModernGroupMemberComparerByRelevanceScore(string serializedPeopleIKnowGraph, ITracer tracer)
		{
			IPeopleIKnowSerializerFactory serializerFactory = new PeopleIKnowSerializerFactory();
			IPeopleIKnowServiceFactory peopleIKnowServiceFactory = new PeopleIKnowServiceFactory(serializerFactory);
			IPeopleIKnowService peopleIKnowService = peopleIKnowServiceFactory.CreatePeopleIKnowService(tracer);
			this.personaComparerByRelevanceScore = peopleIKnowService.GetRelevancyComparer(serializedPeopleIKnowGraph);
		}

		public int Compare(ModernGroupMemberType member1, ModernGroupMemberType member2)
		{
			bool flag = false;
			bool flag2 = false;
			if (member1 == null || member1.Persona == null || member1.Persona.EmailAddress == null)
			{
				flag = true;
			}
			if (member2 == null || member2.Persona == null || member2.Persona.EmailAddress == null)
			{
				flag2 = true;
			}
			if (flag && flag2)
			{
				return 0;
			}
			if (flag)
			{
				return 1;
			}
			if (flag2)
			{
				return -1;
			}
			return this.personaComparerByRelevanceScore.Compare(member1.Persona.EmailAddress.EmailAddress, member2.Persona.EmailAddress.EmailAddress);
		}

		private readonly IComparer<string> personaComparerByRelevanceScore;
	}
}
