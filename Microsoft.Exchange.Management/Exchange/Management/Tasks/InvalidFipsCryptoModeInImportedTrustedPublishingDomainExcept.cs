using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidFipsCryptoModeInImportedTrustedPublishingDomainException : LocalizedException
	{
		public InvalidFipsCryptoModeInImportedTrustedPublishingDomainException(int cryptoMode) : base(Strings.invalidFipsCryptoModeInImportedTrustedPublishingDomain(cryptoMode))
		{
			this.cryptoMode = cryptoMode;
		}

		public InvalidFipsCryptoModeInImportedTrustedPublishingDomainException(int cryptoMode, Exception innerException) : base(Strings.invalidFipsCryptoModeInImportedTrustedPublishingDomain(cryptoMode), innerException)
		{
			this.cryptoMode = cryptoMode;
		}

		protected InvalidFipsCryptoModeInImportedTrustedPublishingDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.cryptoMode = (int)info.GetValue("cryptoMode", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("cryptoMode", this.cryptoMode);
		}

		public int CryptoMode
		{
			get
			{
				return this.cryptoMode;
			}
		}

		private readonly int cryptoMode;
	}
}
