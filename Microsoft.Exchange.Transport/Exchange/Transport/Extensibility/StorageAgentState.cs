using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Extensibility
{
	public class StorageAgentState : ICloneableInternal
	{
		internal Agent AssociatedAgent
		{
			get
			{
				return this.associatedAgent;
			}
			set
			{
				this.associatedAgent = value;
			}
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}

		private Agent associatedAgent;
	}
}
