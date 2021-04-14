using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopRegisterNotification : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.RegisterNotification;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopRegisterNotification();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte notificationHandleIndex, NotificationFlags flags, NotificationEventFlags eventFlags, bool wantGlobalScope, StoreId folderId, StoreId messageId)
		{
			if ((byte)(eventFlags & NotificationEventFlags.RowFound) == 4)
			{
				throw new ArgumentException("RowFound is unsupported", "eventFlags");
			}
			base.SetCommonInput(logonIndex, handleTableIndex, notificationHandleIndex);
			this.flags = flags;
			this.eventFlags = eventFlags;
			this.wantGlobalScope = wantGlobalScope;
			this.folderId = folderId;
			this.messageId = messageId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt16((ushort)this.flags);
			if ((ushort)(this.flags & NotificationFlags.Extended) == 1024)
			{
				writer.WriteByte((byte)this.eventFlags);
				if ((byte)(this.eventFlags & NotificationEventFlags.RowFound) == 4)
				{
					throw new InvalidOperationException("Cannot serialize NotificationEventFlags.RowFound");
				}
			}
			writer.WriteBool(this.wantGlobalScope);
			if (!this.wantGlobalScope)
			{
				this.folderId.Serialize(writer);
				this.messageId.Serialize(writer);
			}
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulRegisterNotificationResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new RegisterNotificationResultFactory(base.PeekReturnServerObjectHandleValue);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.flags = (NotificationFlags)reader.ReadUInt16();
			if ((ushort)(this.flags & NotificationFlags.Extended) == 1024)
			{
				this.eventFlags = (NotificationEventFlags)reader.ReadByte();
				if ((byte)(this.eventFlags & NotificationEventFlags.RowFound) == 4)
				{
					reader.ReadUInt16();
					int num = (int)(reader.ReadByte() + reader.ReadByte());
					for (int i = 0; i < num; i++)
					{
						reader.ReadPropertyTag();
					}
				}
			}
			this.wantGlobalScope = reader.ReadBool();
			if (this.wantGlobalScope)
			{
				this.folderId = default(StoreId);
				this.messageId = default(StoreId);
				return;
			}
			this.folderId = StoreId.Parse(reader);
			this.messageId = StoreId.Parse(reader);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			RegisterNotificationResultFactory resultFactory = new RegisterNotificationResultFactory(base.PeekReturnServerObjectHandleValue);
			this.result = ropHandler.RegisterNotification(serverObject, this.flags, this.eventFlags, this.wantGlobalScope, this.folderId, this.messageId, resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.flags);
			stringBuilder.Append(" EventFlags=").Append(this.eventFlags);
			stringBuilder.Append(" Global=").Append(this.wantGlobalScope);
			stringBuilder.Append(" FID=").Append(this.folderId.ToString());
			stringBuilder.Append(" MID=").Append(this.messageId.ToString());
		}

		private const RopId RopType = RopId.RegisterNotification;

		private NotificationFlags flags;

		private NotificationEventFlags eventFlags;

		private bool wantGlobalScope;

		private StoreId folderId;

		private StoreId messageId;
	}
}
