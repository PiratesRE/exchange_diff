using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidCspForCryptoModeInImportedTrustedPublishingDomainException : LocalizedException
	{
		public InvalidCspForCryptoModeInImportedTrustedPublishingDomainException(string cspName, int cryptoMode) : base(Strings.InvalidCspForCryptoModeInImportedTrustedPublishingDomain(cspName, cryptoMode))
		{
			this.cspName = cspName;
			this.cryptoMode = cryptoMode;
		}

		public InvalidCspForCryptoModeInImportedTrustedPublishingDomainException(string cspName, int cryptoMode, Exception innerException) : base(Strings.InvalidCspForCryptoModeInImportedTrustedPublishingDomain(cspName, cryptoMode), innerException)
		{
			this.cspName = cspName;
			this.cryptoMode = cryptoMode;
		}

		protected InvalidCspForCryptoModeInImportedTrustedPublishingDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.cspName = (string)info.GetValue("cspName", typeof(string));
			this.cryptoMode = (int)info.GetValue("cryptoMode", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("cspName", this.cspName);
			info.AddValue("cryptoMode", this.cryptoMode);
		}

		public string CspName
		{
			get
			{
				return this.cspName;
			}
		}

		public int CryptoMode
		{
			get
			{
				return this.cryptoMode;
			}
		}

		private readonly string cspName;

		private readonly int cryptoMode;
	}
}
