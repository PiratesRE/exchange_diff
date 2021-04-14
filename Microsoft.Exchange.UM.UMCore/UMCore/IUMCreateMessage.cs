using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IUMCreateMessage
	{
		void PrepareUnProtectedMessage();

		void PrepareProtectedMessage();

		void PrepareNDRForFailureToGenerateProtectedMessage();
	}
}
