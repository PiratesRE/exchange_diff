using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class InputOutputRop : Rop
	{
		internal byte ReturnHandleTableIndex
		{
			get
			{
				return this.returnObjectHandleIndex;
			}
		}

		internal sealed override void Execute(IConnectionInformation connection, IRopDriver ropDriver, ServerObjectHandleTable handleTable, ArraySegment<byte> outputBuffer)
		{
			IServerObject serverObject = null;
			ErrorCode errorCode = ErrorCode.None;
			bool flag = false;
			ServerObjectHandle value = ServerObjectHandle.None;
			ServerObjectMap serverObjectMap;
			if (ropDriver.TryGetServerObjectMap(base.LogonIndex, out serverObjectMap, out errorCode))
			{
				ServerObjectHandle handle = handleTable[(int)base.HandleTableIndex];
				if (serverObjectMap.TryGetValue(handle, out serverObject, out errorCode))
				{
					flag = true;
					this.ResolveString8Values(serverObject.String8Encoding);
					try
					{
						this.PeekReturnServerObjectHandleValue = serverObjectMap.NextHandleValue;
						this.InternalExecute(serverObject, ropDriver.RopHandler, outputBuffer);
						if (base.Result.ReturnObject != null)
						{
							base.Result.String8Encoding = base.Result.ReturnObject.String8Encoding;
						}
						value = base.Result.GetServerObjectHandle(serverObjectMap);
						uint handleValue = this.PeekReturnServerObjectHandleValue.HandleValue;
						uint handleValue2 = value.HandleValue;
					}
					finally
					{
						if (base.Result != null && base.Result.ReturnObject != null)
						{
							ropDriver.RopHandler.Release(base.Result.ReturnObject);
							base.Result.ReturnObject = null;
						}
					}
				}
			}
			if (!flag)
			{
				IResultFactory defaultResultFactory = this.GetDefaultResultFactory(connection, outputBuffer);
				base.Result = defaultResultFactory.CreateStandardFailedResult(errorCode);
			}
			handleTable[(int)this.ReturnHandleTableIndex] = value;
			base.Result.SetServerObjectHandleIndex(this.ReturnHandleTableIndex);
			if (base.Result.String8Encoding == null)
			{
				base.Result.String8Encoding = connection.String8Encoding;
			}
		}

		private protected ServerObjectHandle PeekReturnServerObjectHandleValue { protected get; private set; }

		protected sealed override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable, IParseLogonTracker logonTracker)
		{
			base.InternalParseInput(reader, serverObjectHandleTable, logonTracker);
			if (!logonTracker.ParseIsValidLogon(base.LogonIndex))
			{
				throw new BufferParseException("Invalid Logon");
			}
			this.InternalParseInput(reader, serverObjectHandleTable);
			logonTracker.ParseRecordInputOutput(base.HandleTableIndex);
		}

		protected virtual void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			this.returnObjectHandleIndex = Rop.ReadHandleTableIndex(reader, serverObjectHandleTable);
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte(this.returnObjectHandleIndex);
		}

		protected void SetCommonInput(byte logonIndex, byte inputHandleTableIndex, byte outputHandleTableIndex)
		{
			base.SetCommonInput(logonIndex, inputHandleTableIndex);
			this.returnObjectHandleIndex = outputHandleTableIndex;
		}

		protected abstract void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer);

		private byte returnObjectHandleIndex;
	}
}
