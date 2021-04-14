using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopRelease : Rop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.Release;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopRelease();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			throw new InvalidOperationException("Results are not supported for RopRelease");
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable, IParseLogonTracker logonTracker)
		{
			base.InternalParseInput(reader, serverObjectHandleTable, logonTracker);
			logonTracker.ParseRecordRelease(base.HandleTableIndex);
		}

		internal sealed override void Execute(IConnectionInformation connectionInfo, IRopDriver ropDriver, ServerObjectHandleTable handleTable, ArraySegment<byte> outputBuffer)
		{
			ServerObjectHandle handleToRelease = handleTable[(int)base.HandleTableIndex];
			ropDriver.ReleaseHandle(base.LogonIndex, handleToRelease);
			handleTable[(int)base.HandleTableIndex] = ServerObjectHandle.ReleasedObject;
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
		}

		private const RopId RopType = RopId.Release;
	}
}
