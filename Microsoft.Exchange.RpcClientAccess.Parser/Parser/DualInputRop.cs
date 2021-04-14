using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class DualInputRop : Rop
	{
		internal virtual byte SourceHandleTableIndex
		{
			get
			{
				return base.HandleTableIndex;
			}
		}

		internal byte DestinationHandleTableIndex
		{
			get
			{
				return this.destinationHandleTableIndex;
			}
		}

		internal sealed override void Execute(IConnectionInformation connection, IRopDriver ropDriver, ServerObjectHandleTable handleTable, ArraySegment<byte> outputBuffer)
		{
			IServerObject serverObject = null;
			IServerObject destinationServerObject = null;
			ErrorCode errorCode = ErrorCode.None;
			ServerObjectHandle handle = handleTable[(int)this.SourceHandleTableIndex];
			ServerObjectHandle handle2 = handleTable[(int)this.DestinationHandleTableIndex];
			if (ropDriver.TryGetServerObject(base.LogonIndex, handle, out serverObject, out errorCode))
			{
				this.ResolveString8Values(serverObject.String8Encoding);
				if (ropDriver.TryGetServerObject(base.LogonIndex, handle2, out destinationServerObject, out errorCode))
				{
					this.InternalExecute(serverObject, destinationServerObject, ropDriver.RopHandler, outputBuffer);
					base.Result.String8Encoding = serverObject.String8Encoding;
				}
				else if (ErrorCode.NullObject == errorCode)
				{
					errorCode = ErrorCode.DestinationNullObject;
				}
			}
			if (errorCode != ErrorCode.None)
			{
				IResultFactory defaultResultFactory = this.GetDefaultResultFactory(connection, outputBuffer);
				base.Result = defaultResultFactory.CreateStandardFailedResult(errorCode);
			}
			base.Result.SetServerObjectHandleIndex(base.HandleTableIndex);
			if (base.Result.String8Encoding == null)
			{
				base.Result.String8Encoding = connection.String8Encoding;
			}
		}

		protected sealed override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable, IParseLogonTracker logonTracker)
		{
			base.InternalParseInput(reader, serverObjectHandleTable, logonTracker);
			if (!logonTracker.ParseIsValidLogon(base.LogonIndex))
			{
				throw new BufferParseException("Invalid Logon");
			}
			this.InternalParseInput(reader, serverObjectHandleTable);
		}

		protected virtual void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			this.destinationHandleTableIndex = Rop.ReadHandleTableIndex(reader, serverObjectHandleTable);
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte(this.destinationHandleTableIndex);
		}

		protected void SetCommonInput(byte logonIndex, byte sourceHandleTableIndex, byte destinationHandleTableIndex)
		{
			base.SetCommonInput(logonIndex, sourceHandleTableIndex);
			this.destinationHandleTableIndex = destinationHandleTableIndex;
		}

		protected abstract void InternalExecute(IServerObject sourceServerObject, IServerObject destinationServerObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer);

		private byte destinationHandleTableIndex;
	}
}
