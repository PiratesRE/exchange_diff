using System;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	internal class MapiHttpDispatchedCallResult
	{
		public MapiHttpDispatchedCallResult(uint statusCode, Exception exception)
		{
			this.statusCode = statusCode;
			if (this.statusCode != 0U)
			{
				Util.ThrowOnNullArgument(exception, "exception");
				this.exception = exception;
				return;
			}
			if (exception != null)
			{
				throw new ArgumentException("Exception must be null if the status code is 0 (success).", "exception");
			}
		}

		public uint StatusCode
		{
			get
			{
				return this.statusCode;
			}
		}

		public MapiHttpResponse CreateResponse(Func<ArraySegment<byte>> bufferAcquisitionDelegate, Func<MapiHttpResponse> successfulResponseDelegate)
		{
			if (this.statusCode == 0U)
			{
				return successfulResponseDelegate();
			}
			return new MapiHttpFailureResponse(this.statusCode, this.SerializeExceptionIntoAuxOut(this.exception, bufferAcquisitionDelegate()));
		}

		private ArraySegment<byte> SerializeExceptionIntoAuxOut(Exception exception, ArraySegment<byte> auxOut)
		{
			ExceptionTraceAuxiliaryBlock outputBlock = new ExceptionTraceAuxiliaryBlock(0U, exception.ToString());
			AuxiliaryData emptyAuxiliaryData = AuxiliaryData.GetEmptyAuxiliaryData();
			emptyAuxiliaryData.AppendOutput(outputBlock);
			return emptyAuxiliaryData.Serialize(auxOut);
		}

		private readonly uint statusCode;

		private readonly Exception exception;
	}
}
