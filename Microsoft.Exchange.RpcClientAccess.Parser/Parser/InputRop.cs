using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class InputRop : Rop
	{
		internal virtual byte InputHandleTableIndex
		{
			get
			{
				return base.HandleTableIndex;
			}
		}

		internal sealed override void Execute(IConnectionInformation connection, IRopDriver ropDriver, ServerObjectHandleTable handleTable, ArraySegment<byte> outputBuffer)
		{
			IServerObject serverObject = null;
			ErrorCode errorCode = ErrorCode.None;
			ServerObjectHandle handle = handleTable[(int)this.InputHandleTableIndex];
			if (ropDriver.TryGetServerObject(base.LogonIndex, handle, out serverObject, out errorCode))
			{
				this.ResolveString8Values(serverObject.String8Encoding);
				this.InternalExecute(serverObject, ropDriver.RopHandler, outputBuffer);
				base.Result.String8Encoding = serverObject.String8Encoding;
			}
			else
			{
				IResultFactory defaultResultFactory = this.GetDefaultResultFactory(connection, outputBuffer);
				base.Result = defaultResultFactory.CreateStandardFailedResult(errorCode);
				base.Result.String8Encoding = connection.String8Encoding;
			}
			base.Result.SetServerObjectHandleIndex(base.HandleTableIndex);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable, IParseLogonTracker logonTracker)
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
		}

		protected abstract void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer);
	}
}
