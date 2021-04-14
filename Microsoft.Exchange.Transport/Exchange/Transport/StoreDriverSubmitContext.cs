using System;

namespace Microsoft.Exchange.Transport
{
	internal class StoreDriverSubmitContext : PoisonContext
	{
		public StoreDriverSubmitContext(string poisonId) : base(MessageProcessingSource.StoreDriverSubmit)
		{
			this.poisonId = poisonId;
		}

		public override string ToString()
		{
			return this.poisonId;
		}

		public override int GetHashCode()
		{
			return this.poisonId.GetHashCode();
		}

		private string poisonId;
	}
}
