using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	[Serializable]
	public class CredentialRecords
	{
		public CredentialRecords()
		{
			this.credentialRecords = new MultiValuedProperty<CredentialRecord>();
		}

		public MultiValuedProperty<CredentialRecord> Records
		{
			get
			{
				return this.credentialRecords;
			}
			set
			{
				this.credentialRecords = value;
			}
		}

		public override string ToString()
		{
			return "Number of credentials " + this.Records.Count;
		}

		private MultiValuedProperty<CredentialRecord> credentialRecords;
	}
}
