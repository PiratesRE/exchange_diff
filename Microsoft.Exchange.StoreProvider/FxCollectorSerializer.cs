using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FxCollectorSerializer : IMapiFxCollector
	{
		public FxCollectorSerializer(IMapiFxProxy fxProxy)
		{
			this.fxProxy = fxProxy;
			byte[] objectData = this.fxProxy.GetObjectData();
			BinaryDeserializer.Deserialize(objectData, delegate(BinaryDeserializer deserializer)
			{
				this.cachedObjectType = deserializer.ReadGuid();
				this.cachedServerVersion = deserializer.ReadBytes();
				if (this.cachedObjectType == InterfaceIds.IMsgStoreGuid)
				{
					this.cachedIsPrivateLogon = (deserializer.ReadInt() != 0);
					return;
				}
				this.cachedIsPrivateLogon = false;
			});
		}

		private void DoOperation(FxOpcodes opCode, byte[] request)
		{
			this.fxProxy.ProcessRequest(opCode, request);
		}

		public Guid GetObjectType()
		{
			return this.cachedObjectType;
		}

		public byte[] GetServerVersion()
		{
			return this.cachedServerVersion;
		}

		public bool IsPrivateLogon()
		{
			return this.cachedIsPrivateLogon;
		}

		public void Config(int flags, int transferMethod)
		{
			byte[] request = BinarySerializer.Serialize(delegate(BinarySerializer serializer)
			{
				serializer.Write(flags);
				serializer.Write(transferMethod);
			});
			this.DoOperation(FxOpcodes.Config, request);
		}

		public void TransferBuffer(byte[] data)
		{
			this.DoOperation(FxOpcodes.TransferBuffer, data);
		}

		public void IsInterfaceOk(int transferMethod, Guid refiid, int flags)
		{
			byte[] request = BinarySerializer.Serialize(delegate(BinarySerializer serializer)
			{
				serializer.Write(transferMethod);
				serializer.Write(refiid);
				serializer.Write(flags);
			});
			this.DoOperation(FxOpcodes.IsInterfaceOk, request);
		}

		public void TellPartnerVersion(byte[] versionData)
		{
			this.DoOperation(FxOpcodes.TellPartnerVersion, versionData);
		}

		public void StartMdbEventsImport()
		{
			this.DoOperation(FxOpcodes.StartMdbEventsImport, null);
		}

		public void FinishMdbEventsImport(bool success)
		{
			byte[] request = BinarySerializer.Serialize(delegate(BinarySerializer serializer)
			{
				serializer.Write(success ? 1 : 0);
			});
			this.DoOperation(FxOpcodes.FinishMdbEventsImport, request);
		}

		public void AddMdbEvents(byte[] request)
		{
			this.DoOperation(FxOpcodes.AddMdbEvents, request);
		}

		public void SetWatermarks(MDBEVENTWMRAW[] WMs)
		{
			byte[] request = BinarySerializer.Serialize(delegate(BinarySerializer serializer)
			{
				serializer.Write(WMs.Length);
				foreach (MDBEVENTWMRAW mdbeventwmraw in WMs)
				{
					serializer.Write(mdbeventwmraw.guidMailbox);
					serializer.Write(mdbeventwmraw.guidConsumer);
					serializer.Write(mdbeventwmraw.eventCounter);
				}
			});
			this.DoOperation(FxOpcodes.SetWatermarks, request);
		}

		public void SetReceiveFolder(byte[] entryId, string messageClass)
		{
			byte[] request = BinarySerializer.Serialize(delegate(BinarySerializer serializer)
			{
				serializer.Write(entryId);
				serializer.Write(messageClass);
			});
			this.DoOperation(FxOpcodes.SetReceiveFolder, request);
		}

		public void SetPerUser(MapiLtidNative ltid, Guid guidReplica, int lib, byte[] pb, bool fLast)
		{
			byte[] request = BinarySerializer.Serialize(delegate(BinarySerializer serializer)
			{
				serializer.Write(ltid.replGuid);
				serializer.Write(ltid.globCount);
				serializer.Write(guidReplica);
				serializer.Write(lib);
				serializer.Write(pb);
				serializer.Write(fLast ? 1 : 0);
			});
			this.DoOperation(FxOpcodes.SetPerUser, request);
		}

		public void SetProps(PropValue[] pva)
		{
			byte[] request = BinarySerializer.Serialize(delegate(BinarySerializer serializer)
			{
				serializer.Write(pva);
			});
			this.DoOperation(FxOpcodes.SetProps, request);
		}

		protected IMapiFxProxy fxProxy;

		protected Guid cachedObjectType;

		protected byte[] cachedServerVersion;

		protected bool cachedIsPrivateLogon;
	}
}
