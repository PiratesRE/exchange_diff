using System;
using System.ServiceModel.Channels;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	public class LegacyMessageEncoderBindingElement : MessageEncodingBindingElement
	{
		public LegacyMessageEncoderBindingElement()
		{
		}

		protected LegacyMessageEncoderBindingElement(MessageVersion version)
		{
			this.version = version;
		}

		public override MessageEncoderFactory CreateMessageEncoderFactory()
		{
			return new LegacyMessageEncoderFactory(this.version);
		}

		public override MessageVersion MessageVersion
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		public override BindingElement Clone()
		{
			return new LegacyMessageEncoderBindingElement(this.version);
		}

		public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
		{
			context.BindingParameters.Add(this);
			return base.BuildChannelFactory<TChannel>(context);
		}

		public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
		{
			context.BindingParameters.Add(this);
			return base.BuildChannelListener<TChannel>(context);
		}

		public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
		{
			return base.CanBuildChannelFactory<TChannel>(context);
		}

		public override bool CanBuildChannelListener<TChannel>(BindingContext context)
		{
			context.BindingParameters.Add(this);
			return base.CanBuildChannelListener<TChannel>(context);
		}

		public override T GetProperty<T>(BindingContext context)
		{
			return base.GetProperty<T>(context);
		}

		private MessageVersion version = MessageVersion.None;
	}
}
