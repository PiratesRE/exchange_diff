using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSetSynchronizationNotificationGuid : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SetSynchronizationNotificationGuid;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSetSynchronizationNotificationGuid();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, Guid notificationGuid)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.notificationGuid = notificationGuid;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteGuid(this.notificationGuid);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSetSynchronizationNotificationGuid.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.notificationGuid = reader.ReadGuid();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SetSynchronizationNotificationGuid(serverObject, this.notificationGuid, RopSetSynchronizationNotificationGuid.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" NotificationGuid=").Append(this.notificationGuid);
		}

		private const RopId RopType = RopId.SetSynchronizationNotificationGuid;

		private static SetSynchronizationNotificationGuidResultFactory resultFactory = new SetSynchronizationNotificationGuidResultFactory();

		private Guid notificationGuid;
	}
}
