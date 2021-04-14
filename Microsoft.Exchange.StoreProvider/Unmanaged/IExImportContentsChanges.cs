using System;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExImportContentsChanges : IExInterface, IDisposeTrackable, IDisposable
	{
		int Config(IStream iStream, int ulFlags);

		int UpdateState(IStream iStream);

		unsafe int ImportMessageChange(int cpvalChanges, SPropValue* ppvalChanges, int ulFlags, out SafeExMapiMessageHandle iMessage);

		unsafe int ImportMessageDeletion(int ulFlags, _SBinaryArray* lpSrcEntryList);

		unsafe int ImportPerUserReadStateChange(int cElements, _ReadState* lpReadState);

		int ImportMessageMove(int cbSourceKeySrcFolder, byte[] pbSourceKeySrcFolder, int cbSourceKeySrcMessage, byte[] pbSourceKeySrcMessage, int cbPCLMessage, byte[] pbPCLMessage, int cbSourceKeyDestMessage, byte[] pbSourceKeyDestMessage, int cbChangeNumDestMessage, byte[] pbChangeNumDestMessage);
	}
}
