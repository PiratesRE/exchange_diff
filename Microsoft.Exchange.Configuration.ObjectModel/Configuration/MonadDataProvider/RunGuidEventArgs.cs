using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class RunGuidEventArgs : HandledEventArgs
	{
		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public RunGuidEventArgs(Guid guid)
		{
			this.guid = guid;
		}

		private Guid guid;
	}
}
