using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class CrashOnLIDException : Exception
	{
		public CrashOnLIDException(LID lid) : base(string.Format("Crash on LID: {0}", lid.Value))
		{
			this.lid = lid;
		}

		public CrashOnLIDException(LID lid, Exception innerException) : base(string.Format("Crash on LID: {0}", lid.Value), innerException)
		{
		}

		public LID LID
		{
			get
			{
				return this.lid;
			}
		}

		private LID lid;
	}
}
