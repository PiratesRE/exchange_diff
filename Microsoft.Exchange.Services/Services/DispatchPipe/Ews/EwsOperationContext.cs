using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace Microsoft.Exchange.Services.DispatchPipe.Ews
{
	public class EwsOperationContext : EwsOperationContextBase
	{
		public static EwsOperationContext Create(Message requestMessage)
		{
			return new EwsOperationContext(requestMessage);
		}

		private EwsOperationContext(Message requestMessage)
		{
			this.requestMessage = requestMessage;
		}

		public override Message RequestMessage
		{
			get
			{
				return this.requestMessage;
			}
		}

		public override MessageProperties IncomingMessageProperties
		{
			get
			{
				if (this.requestMessage != null)
				{
					return this.requestMessage.Properties;
				}
				return null;
			}
		}

		public override MessageHeaders IncomingMessageHeaders
		{
			get
			{
				if (this.requestMessage != null)
				{
					return this.requestMessage.Headers;
				}
				return null;
			}
		}

		public override MessageVersion IncomingMessageVersion
		{
			get
			{
				if (this.requestMessage != null)
				{
					return this.requestMessage.Version;
				}
				return null;
			}
		}

		public override IEnumerable<SupportingTokenSpecification> SupportingTokens
		{
			get
			{
				MessageProperties incomingMessageProperties = this.IncomingMessageProperties;
				if (incomingMessageProperties != null && incomingMessageProperties.Security != null)
				{
					return new ReadOnlyCollection<SupportingTokenSpecification>(incomingMessageProperties.Security.IncomingSupportingTokens);
				}
				return null;
			}
		}

		public override MessageProperties OutgoingMessageProperties
		{
			get
			{
				if (this.outgoingMessageProperties == null)
				{
					this.outgoingMessageProperties = new MessageProperties();
				}
				return this.outgoingMessageProperties;
			}
		}

		public override Uri LocalAddressUri
		{
			get
			{
				return null;
			}
		}

		internal override OperationContext BackingOperationContext
		{
			get
			{
				return null;
			}
		}

		private MessageProperties outgoingMessageProperties;

		private Message requestMessage;
	}
}
