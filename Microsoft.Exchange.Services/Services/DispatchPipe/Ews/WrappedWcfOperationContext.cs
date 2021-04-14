using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace Microsoft.Exchange.Services.DispatchPipe.Ews
{
	public class WrappedWcfOperationContext : EwsOperationContextBase
	{
		public WrappedWcfOperationContext(OperationContext operationContext)
		{
			this.operationContext = operationContext;
		}

		public override Message RequestMessage
		{
			get
			{
				if (this.operationContext.RequestContext != null)
				{
					return this.operationContext.RequestContext.RequestMessage;
				}
				return null;
			}
		}

		public override MessageProperties IncomingMessageProperties
		{
			get
			{
				return this.operationContext.IncomingMessageProperties;
			}
		}

		public override MessageHeaders IncomingMessageHeaders
		{
			get
			{
				return this.operationContext.IncomingMessageHeaders;
			}
		}

		public override MessageVersion IncomingMessageVersion
		{
			get
			{
				return this.operationContext.IncomingMessageVersion;
			}
		}

		public override IEnumerable<SupportingTokenSpecification> SupportingTokens
		{
			get
			{
				return this.operationContext.SupportingTokens;
			}
		}

		public override MessageProperties OutgoingMessageProperties
		{
			get
			{
				return this.operationContext.OutgoingMessageProperties;
			}
		}

		public override Uri LocalAddressUri
		{
			get
			{
				return this.operationContext.Channel.LocalAddress.Uri;
			}
		}

		internal override OperationContext BackingOperationContext
		{
			get
			{
				return this.operationContext;
			}
		}

		private OperationContext operationContext;
	}
}
