using System;
using System.ServiceModel.Channels;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	public class LegacyMessageEncoderFactory : MessageEncoderFactory
	{
		public LegacyMessageEncoderFactory(MessageVersion version)
		{
			this.version = version;
			this.encoder = new LegacyMessageEncoder(this.version);
		}

		public override MessageEncoder Encoder
		{
			get
			{
				return this.encoder;
			}
		}

		public override MessageVersion MessageVersion
		{
			get
			{
				return this.version;
			}
		}

		private MessageVersion version;

		private MessageEncoder encoder;
	}
}
