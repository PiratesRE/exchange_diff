using System;

namespace Microsoft.Exchange.EdgeSync.Validation.Mserv
{
	public sealed class MservRecipientRecord
	{
		public string Address
		{
			get
			{
				return this.address;
			}
		}

		public int PartnerId
		{
			get
			{
				return this.partnerId;
			}
		}

		internal MservRecipientRecord(string address, int partnerId)
		{
			this.address = address;
			this.partnerId = partnerId;
		}

		private string address;

		private int partnerId;
	}
}
