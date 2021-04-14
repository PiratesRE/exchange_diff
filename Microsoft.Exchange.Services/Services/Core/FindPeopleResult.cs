using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class FindPeopleResult
	{
		private FindPeopleResult()
		{
		}

		public static FindPeopleResult CreateSearchResult(Persona[] personaList)
		{
			return new FindPeopleResult
			{
				PersonaList = personaList
			};
		}

		public static FindPeopleResult CreateMailboxBrowseResult(Persona[] personaList, int totalNumberOfPeopleInView, int firstLoadedRowIndex, int firstMatchingRowIndex)
		{
			return new FindPeopleResult
			{
				PersonaList = personaList,
				FirstLoadedRowIndex = firstLoadedRowIndex,
				FirstMatchingRowIndex = firstMatchingRowIndex,
				TotalNumberOfPeopleInView = totalNumberOfPeopleInView
			};
		}

		public static FindPeopleResult CreateMailFolderBrowseResult(Persona[] personaList)
		{
			return new FindPeopleResult
			{
				PersonaList = personaList
			};
		}

		public static FindPeopleResult CreateMailboxBrowseResult(Persona[] personaList, int totalNumberOfPeopleInView)
		{
			return new FindPeopleResult
			{
				PersonaList = personaList,
				TotalNumberOfPeopleInView = totalNumberOfPeopleInView
			};
		}

		public static FindPeopleResult CreateDirectoryBrowseResult(Persona[] personaList, int totalNumberOfPeopleInView, int firstLoadedRowIndex)
		{
			return new FindPeopleResult
			{
				PersonaList = personaList,
				TotalNumberOfPeopleInView = totalNumberOfPeopleInView,
				FirstLoadedRowIndex = firstLoadedRowIndex
			};
		}

		public Persona[] PersonaList { get; private set; }

		public int FirstLoadedRowIndex { get; private set; }

		public int TotalNumberOfPeopleInView { get; private set; }

		public int FirstMatchingRowIndex { get; private set; }
	}
}
