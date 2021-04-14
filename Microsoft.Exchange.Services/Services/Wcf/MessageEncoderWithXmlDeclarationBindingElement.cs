using System;
using System.ServiceModel.Channels;

namespace Microsoft.Exchange.Services.Wcf
{
	public class MessageEncoderWithXmlDeclarationBindingElement : MessageEncodingBindingElement
	{
		public MessageEncoderWithXmlDeclarationBindingElement(MessageVersion version)
		{
			this.version = version;
		}

		public MessageEncoderWithXmlDeclarationBindingElement(MessageEncoderWithXmlDeclarationBindingElement other)
		{
			this.version = other.version;
		}

		public override MessageEncoderFactory CreateMessageEncoderFactory()
		{
			return new MessageEncoderWithXmlDeclarationFactory(this);
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
			return new MessageEncoderWithXmlDeclarationBindingElement(this);
		}

		public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
		{
			context.BindingParameters.Add(this);
			return context.BuildInnerChannelFactory<TChannel>();
		}

		public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
		{
			return context.CanBuildInnerChannelFactory<TChannel>();
		}

		public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
		{
			context.BindingParameters.Add(this);
			if (Global.UseBufferRequestChannelListener.Member && typeof(TChannel) == typeof(IReplyChannel))
			{
				BufferRequestChannelListener bufferRequestChannelListener = new BufferRequestChannelListener(context.BuildInnerChannelListener<IReplyChannel>());
				return (IChannelListener<TChannel>)bufferRequestChannelListener;
			}
			return context.BuildInnerChannelListener<TChannel>();
		}

		public override bool CanBuildChannelListener<TChannel>(BindingContext context)
		{
			context.BindingParameters.Add(this);
			return context.CanBuildInnerChannelListener<TChannel>();
		}

		private MessageVersion version;
	}
}
