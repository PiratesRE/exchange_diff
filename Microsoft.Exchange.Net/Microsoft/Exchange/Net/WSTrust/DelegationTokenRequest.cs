using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Net.WSTrust
{
	[Serializable]
	internal sealed class DelegationTokenRequest
	{
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.Append("FederatedIdentity=");
			stringBuilder.AppendLine(this.FederatedIdentity.ToString());
			stringBuilder.Append("EmailAddress=");
			stringBuilder.AppendLine(this.EmailAddress.ToString());
			if (!string.IsNullOrEmpty(this.Policy))
			{
				stringBuilder.Append("Policy=");
				stringBuilder.AppendLine(this.Policy);
			}
			stringBuilder.Append("Target=");
			stringBuilder.AppendLine(this.Target.ToString());
			stringBuilder.Append("Offer=");
			stringBuilder.AppendLine(this.Offer.ToString());
			if (this.EmailAddresses != null && this.EmailAddresses.Count != 0)
			{
				stringBuilder.Append("EmailAddresses=");
				bool flag = true;
				foreach (string value in this.EmailAddresses)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(value);
				}
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		public FederatedIdentity FederatedIdentity;

		public string EmailAddress;

		public string Policy;

		public TokenTarget Target;

		public Offer Offer;

		public List<string> EmailAddresses;
	}
}
