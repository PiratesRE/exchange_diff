using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSynchronizationOpenAdvisor : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SynchronizationOpenAdvisor;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSynchronizationOpenAdvisor();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulSynchronizationOpenAdvisorResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSynchronizationOpenAdvisor.resultFactory;
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SynchronizationOpenAdvisor(serverObject, RopSynchronizationOpenAdvisor.resultFactory);
		}

		private const RopId RopType = RopId.SynchronizationOpenAdvisor;

		private static SynchronizationOpenAdvisorResultFactory resultFactory = new SynchronizationOpenAdvisorResultFactory();
	}
}
