using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class UMLegacyMessageEncoderWithXmlDeclaration : MessageEncoderWithXmlDeclaration
	{
		public UMLegacyMessageEncoderWithXmlDeclaration(MessageEncoderWithXmlDeclarationFactory factory) : base(factory)
		{
		}

		protected override bool ShouldValidateRequest(string methodName, string methodNamespace, ExchangeVersion requestVersion, int requestSize)
		{
			return false;
		}

		protected override long MaxTraceRequestSize
		{
			get
			{
				return 0L;
			}
		}
	}
}
