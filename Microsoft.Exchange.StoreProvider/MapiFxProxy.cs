using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiFxProxy : DisposeTrackableBase, IMapiFxProxy, IDisposeTrackable, IDisposable
	{
		public MapiFxProxy(IMapiFxCollector iCollector)
		{
			this.fxCollector = iCollector;
			bool isPrivateLogon = false;
			Guid objectType = this.fxCollector.GetObjectType();
			byte[] serverVersion = this.fxCollector.GetServerVersion();
			if (objectType == InterfaceIds.IMsgStoreGuid)
			{
				isPrivateLogon = this.fxCollector.IsPrivateLogon();
			}
			this.cachedObjectData = BinarySerializer.Serialize(delegate(BinarySerializer serializer)
			{
				serializer.Write(objectType);
				serializer.Write(serverVersion);
				if (objectType == InterfaceIds.IMsgStoreGuid)
				{
					serializer.Write(isPrivateLogon ? 1 : 0);
				}
			});
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				IDisposable disposable = this.fxCollector as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
				this.fxCollector = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiFxProxy>(this);
		}

		public void ProcessRequest(FxOpcodes opCode, byte[] request)
		{
			this.DoOperation(opCode, request);
		}

		public byte[] GetObjectData()
		{
			return this.cachedObjectData;
		}

		private void DoOperation(FxOpcodes opCode, byte[] request)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string>(46349, 36, (long)this.GetHashCode(), "MapiFxProxy.DoOperation: opCode={0}, request={1}", opCode.ToString(), TraceUtils.DumpArray(request));
			}
			switch (opCode)
			{
			case FxOpcodes.Config:
			{
				int flags = 0;
				int transferMethod = 0;
				BinaryDeserializer.Deserialize(request, delegate(BinaryDeserializer deserializer)
				{
					flags = deserializer.ReadInt();
					transferMethod = deserializer.ReadInt();
				});
				this.fxCollector.Config(flags, transferMethod);
				goto IL_2D1;
			}
			case FxOpcodes.TransferBuffer:
				this.fxCollector.TransferBuffer(request);
				goto IL_2D1;
			case FxOpcodes.IsInterfaceOk:
			{
				int transferMethod = 0;
				Guid refiid = Guid.Empty;
				int flags = 0;
				BinaryDeserializer.Deserialize(request, delegate(BinaryDeserializer deserializer)
				{
					transferMethod = deserializer.ReadInt();
					refiid = deserializer.ReadGuid();
					flags = deserializer.ReadInt();
				});
				this.fxCollector.IsInterfaceOk(transferMethod, refiid, flags);
				goto IL_2D1;
			}
			case FxOpcodes.TellPartnerVersion:
				this.fxCollector.TellPartnerVersion(request);
				goto IL_2D1;
			case FxOpcodes.StartMdbEventsImport:
				this.fxCollector.StartMdbEventsImport();
				goto IL_2D1;
			case FxOpcodes.FinishMdbEventsImport:
			{
				bool success = false;
				BinaryDeserializer.Deserialize(request, delegate(BinaryDeserializer deserializer)
				{
					success = (deserializer.ReadInt() != 0);
				});
				this.fxCollector.FinishMdbEventsImport(success);
				goto IL_2D1;
			}
			case FxOpcodes.AddMdbEvents:
				this.fxCollector.AddMdbEvents(request);
				goto IL_2D1;
			case FxOpcodes.SetWatermarks:
			{
				MDBEVENTWMRAW[] WMs = null;
				BinaryDeserializer.Deserialize(request, delegate(BinaryDeserializer deserializer)
				{
					WMs = deserializer.ReadArray<MDBEVENTWMRAW>((BinaryDeserializer innerDeserializer) => new MDBEVENTWMRAW
					{
						guidMailbox = innerDeserializer.ReadGuid(),
						guidConsumer = innerDeserializer.ReadGuid(),
						eventCounter = innerDeserializer.ReadUInt64()
					});
				});
				this.fxCollector.SetWatermarks(WMs);
				goto IL_2D1;
			}
			case FxOpcodes.SetReceiveFolder:
			{
				byte[] entryId = null;
				string messageClass = null;
				BinaryDeserializer.Deserialize(request, delegate(BinaryDeserializer deserializer)
				{
					entryId = deserializer.ReadBytes();
					messageClass = deserializer.ReadString();
				});
				this.fxCollector.SetReceiveFolder(entryId, messageClass);
				goto IL_2D1;
			}
			case FxOpcodes.SetPerUser:
			{
				MapiLtidNative ltid = default(MapiLtidNative);
				Guid guidReplica = Guid.Empty;
				int lib = 0;
				byte[] pb = null;
				bool fLast = false;
				BinaryDeserializer.Deserialize(request, delegate(BinaryDeserializer deserializer)
				{
					ltid.replGuid = deserializer.ReadGuid();
					ltid.globCount = deserializer.ReadBytes();
					guidReplica = deserializer.ReadGuid();
					lib = deserializer.ReadInt();
					pb = deserializer.ReadBytes();
					fLast = (deserializer.ReadInt() != 0);
				});
				this.fxCollector.SetPerUser(ltid, guidReplica, lib, pb, fLast);
				goto IL_2D1;
			}
			case FxOpcodes.SetProps:
			{
				PropValue[] pva = null;
				BinaryDeserializer.Deserialize(request, delegate(BinaryDeserializer deserializer)
				{
					pva = deserializer.ReadPropValues();
				});
				this.fxCollector.SetProps(pva);
				goto IL_2D1;
			}
			}
			MapiExceptionHelper.ThrowIfError("Invalid FxOpcode", -2147024809);
			IL_2D1:
			if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
			{
				ComponentTrace<MapiNetTags>.Trace(62733, 36, (long)this.GetHashCode(), "MapiFxProxy.DoOperation succeeded");
			}
		}

		private IMapiFxCollector fxCollector;

		private byte[] cachedObjectData;
	}
}
