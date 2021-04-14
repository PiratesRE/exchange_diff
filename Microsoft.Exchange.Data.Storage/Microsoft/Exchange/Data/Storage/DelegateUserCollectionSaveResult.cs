using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DelegateUserCollectionSaveResult
	{
		internal DelegateUserCollectionSaveResult()
		{
		}

		internal DelegateUserCollectionSaveResult(Collection<KeyValuePair<DelegateSaveState, Exception>> problems)
		{
			this.problems = problems;
		}

		public ICollection<KeyValuePair<DelegateSaveState, Exception>> Problems
		{
			get
			{
				return this.problems;
			}
		}

		private Collection<KeyValuePair<DelegateSaveState, Exception>> problems;
	}
}
