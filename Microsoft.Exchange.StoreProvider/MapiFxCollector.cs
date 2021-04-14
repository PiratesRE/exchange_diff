using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiFxCollector : MapiUnk, IMapiFxCollector
	{
		internal MapiFxCollector(IExFastTransferEx iExchangeFastTransferEx, object externalIExchangeFastTransferEx, MapiStore mapiStore) : base(iExchangeFastTransferEx, externalIExchangeFastTransferEx, mapiStore)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(40253, 36, (long)this.GetHashCode(), "MapiFxCollector.MapiFxCollector: this={0}", TraceUtils.MakeHash(this));
			}
			this.iExchangeFastTransferEx = iExchangeFastTransferEx;
			this.externalIExchangeFastTransferEx = (IExchangeFastTransferEx)externalIExchangeFastTransferEx;
		}

		protected override void MapiInternalDispose()
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(56637, 36, (long)this.GetHashCode(), "MapiFxCollector.InternalDispose: this={0}", TraceUtils.MakeHash(this));
			}
			this.iExchangeFastTransferEx = null;
			this.externalIExchangeFastTransferEx = null;
			base.MapiInternalDispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiFxCollector>(this);
		}

		public void Config(int flags, int transferMethod)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace<string, int>(44349, 36, (long)this.GetHashCode(), "MapiFxCollector.Config params: flags=0x{0}, transferMethod={1}", flags.ToString("x"), transferMethod);
				}
				int num;
				if (base.IsExternal)
				{
					num = this.ExternalIExchangeFastTransferEx.Config(flags, transferMethod);
				}
				else
				{
					num = this.iExchangeFastTransferEx.Config(flags, transferMethod);
				}
				if (num != 0)
				{
					base.ThrowIfError("IExchangeFastTransferEx.Config failed", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void TransferBuffer(byte[] buffer)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(36157, 36, (long)this.GetHashCode(), "MapiFxCollector.TransferBuffer params: buf={0}", TraceUtils.DumpArray(buffer));
				}
				int arg;
				int num;
				if (base.IsExternal)
				{
					num = this.ExternalIExchangeFastTransferEx.TransferBuffer(buffer.Length, buffer, out arg);
				}
				else
				{
					num = this.iExchangeFastTransferEx.TransferBuffer(buffer.Length, buffer, out arg);
				}
				if (num != 0)
				{
					base.ThrowIfError("IExchangeFastTransferEx.TransferBuffer failed", num);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace<int>(52541, 36, (long)this.GetHashCode(), "MapiFxCollector.TransferBuffer succeeded: bytesTransfered={0}", arg);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void IsInterfaceOk(int transferMethod, Guid refiid, int flags)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string, string>(46397, 36, (long)this.GetHashCode(), "MapiFxCollector.IsInterfaceOk params: transferMethod={0}, refiid={1}, flags=0x{2}", transferMethod.ToString(), refiid.ToString(), flags.ToString("x"));
				}
				bool flag;
				if (base.IsExternal)
				{
					flag = this.ExternalIExchangeFastTransferEx.IsInterfaceOk(transferMethod, ref refiid, IntPtr.Zero, flags);
				}
				else
				{
					flag = (this.iExchangeFastTransferEx.IsInterfaceOk(transferMethod, ref refiid, IntPtr.Zero, flags) != 0);
				}
				if (!flag)
				{
					MapiExceptionHelper.ThrowIfError("IExchangeFastTransferEx.IsInterfaceOk returned false", -2147221246);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public unsafe byte[] GetServerVersion()
		{
			base.CheckDisposed();
			base.LockStore();
			byte[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace(52941, 36, (long)this.GetHashCode(), "MapiFxCollector.GetServerVersion");
				}
				try
				{
					fixed (byte* ptr = new byte[50])
					{
						int num;
						int serverVersion;
						if (base.IsExternal)
						{
							serverVersion = this.ExternalIExchangeFastTransferEx.GetServerVersion(50, ptr, out num);
						}
						else
						{
							serverVersion = this.iExchangeFastTransferEx.GetServerVersion(50, ptr, out num);
						}
						if (serverVersion != 0)
						{
							base.ThrowIfError("IExchangeFastTransferEx.GetServerVersion failed", serverVersion);
						}
						byte[] array = new byte[num];
						Marshal.Copy((IntPtr)((void*)ptr), array, 0, num);
						if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
						{
							ComponentTrace<MapiNetTags>.Trace<string>(46797, 36, (long)this.GetHashCode(), "MapiFxCollector.GetServerVersion returned {0}", TraceUtils.DumpBytes(array));
						}
						result = array;
					}
				}
				finally
				{
					byte* ptr = null;
				}
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public unsafe void TellPartnerVersion(byte[] versionData)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(63181, 36, (long)this.GetHashCode(), "MapiFxCollector.TellPartnerVersion: partnerVersionData={0}", TraceUtils.DumpBytes(versionData));
				}
				int num = versionData.Length;
				try
				{
					fixed (byte* ptr = new byte[num])
					{
						Marshal.Copy(versionData, 0, (IntPtr)((void*)ptr), num);
						int num2;
						if (base.IsExternal)
						{
							num2 = this.ExternalIExchangeFastTransferEx.TellPartnerVersion(num, ptr);
						}
						else
						{
							num2 = this.iExchangeFastTransferEx.TellPartnerVersion(num, ptr);
						}
						if (num2 != 0)
						{
							base.ThrowIfError("IExchangeFastTransferEx.TellPartnerVersion failed", num2);
						}
					}
				}
				finally
				{
					byte* ptr = null;
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public Guid GetObjectType()
		{
			base.CheckDisposed();
			base.LockStore();
			Guid result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace(56013, 36, (long)this.GetHashCode(), "MapiFxCollector.GetObjectType");
				}
				Guid guid;
				int objectType;
				if (base.IsExternal)
				{
					objectType = this.ExternalIExchangeFastTransferEx.GetObjectType(out guid);
				}
				else
				{
					objectType = this.iExchangeFastTransferEx.GetObjectType(out guid);
				}
				if (objectType != 0)
				{
					base.ThrowIfError("IExchangeFastTransferEx.GetObjectType failed", objectType);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(39629, 36, (long)this.GetHashCode(), "MapiFxCollector.GetObjectType returned {0}", guid.ToString());
				}
				result = guid;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public bool IsPrivateLogon()
		{
			base.CheckDisposed();
			base.LockStore();
			bool result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace(41677, 36, (long)this.GetHashCode(), "MapiFxCollector.IsPrivateLogon");
				}
				bool flag;
				if (base.IsExternal)
				{
					flag = this.ExternalIExchangeFastTransferEx.IsPrivateLogon();
				}
				else
				{
					flag = (this.iExchangeFastTransferEx.IsPrivateLogon() != 0);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(48397, 36, (long)this.GetHashCode(), "MapiFxCollector.IsPrivateLogon returned {0}", flag.ToString());
				}
				result = flag;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public void StartMdbEventsImport()
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace(64781, 36, (long)this.GetHashCode(), "MapiFxCollector.StartMdbEventsImport");
				}
				int num;
				if (base.IsExternal)
				{
					num = this.ExternalIExchangeFastTransferEx.StartMdbEventsImport();
				}
				else
				{
					num = this.iExchangeFastTransferEx.StartMdbEventsImport();
				}
				if (num != 0)
				{
					base.ThrowIfError("IExchangeFastTransferEx.StartMdbEventsImport failed", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void FinishMdbEventsImport(bool success)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace<bool>(40205, 36, (long)this.GetHashCode(), "MapiFxCollector.FinishMdbEventsImport({0})", success);
				}
				int num;
				if (base.IsExternal)
				{
					num = this.ExternalIExchangeFastTransferEx.FinishMdbEventsImport(success);
				}
				else
				{
					num = this.iExchangeFastTransferEx.FinishMdbEventsImport(success);
				}
				if (num != 0)
				{
					base.ThrowIfError("IExchangeFastTransferEx.FinishMdbEventsImport failed", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void AddMdbEvents(byte[] request)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(59085, 36, (long)this.GetHashCode(), "MapiFxCollector.AddMdbEvents request={0}", TraceUtils.DumpArray(request));
				}
				int num;
				if (base.IsExternal)
				{
					num = this.ExternalIExchangeFastTransferEx.AddMdbEvents(request.Length, request);
				}
				else
				{
					num = this.iExchangeFastTransferEx.AddMdbEvents(request.Length, request);
				}
				if (num != 0)
				{
					base.ThrowIfError("IExchangeFastTransferEx.AddMdbEvents failed", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void SetWatermarks(MDBEVENTWMRAW[] wmData)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(56589, 36, (long)this.GetHashCode(), "MapiFxCollector.SetWatermarks: wmData={0}", TraceUtils.DumpArray(wmData));
				}
				int num;
				if (base.IsExternal)
				{
					num = this.ExternalIExchangeFastTransferEx.SetWatermarks(wmData.Length, wmData);
				}
				else
				{
					num = this.iExchangeFastTransferEx.SetWatermarks(wmData.Length, wmData);
				}
				if (num != 0)
				{
					base.ThrowIfError("IExchangeFastTransferEx.SetWatermarks failed", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void SetReceiveFolder(byte[] entryId, string messageClass)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(44301, 36, (long)this.GetHashCode(), "MapiFxCollector.SetReceiveFolder: entryId={0}, msgClass={1}", TraceUtils.DumpEntryId(entryId), messageClass);
				}
				int num;
				if (base.IsExternal)
				{
					num = this.ExternalIExchangeFastTransferEx.SetReceiveFolder(entryId.Length, entryId, messageClass);
				}
				else
				{
					num = this.iExchangeFastTransferEx.SetReceiveFolder(entryId.Length, entryId, messageClass);
				}
				if (num != 0)
				{
					base.ThrowIfError("IExchangeFastTransferEx.SetReceiveFolder failed", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public unsafe void SetPerUser(MapiLtidNative ltid, Guid guidReplica, int lib, byte[] pb, bool fLast)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace(60685, 36, (long)this.GetHashCode(), "MapiFxCollector.SetPerUser: ltid={0}, guidReplica={1}, lib={2}, pb={3}, fLast={4}", new object[]
					{
						TraceUtils.MakeHash(ltid),
						guidReplica.ToString(),
						lib.ToString(),
						TraceUtils.DumpArray(pb),
						fLast.ToString()
					});
				}
				Guid* guidReplica2 = (guidReplica == Guid.Empty) ? null : (&guidReplica);
				int num;
				if (base.IsExternal)
				{
					num = this.ExternalIExchangeFastTransferEx.SetPerUser(ref ltid, guidReplica2, lib, pb, pb.Length, fLast);
				}
				else
				{
					num = this.iExchangeFastTransferEx.SetPerUser(ref ltid, guidReplica2, lib, pb, pb.Length, fLast);
				}
				if (num != 0)
				{
					base.ThrowIfError("IExchangeFastTransferEx.SetPerUser failed", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public unsafe void SetProps(PropValue[] pva)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(36))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(34509, 36, (long)this.GetHashCode(), "MapiFxCollector.SetProps, props={0}", TraceUtils.DumpPropValsArray(pva));
				}
				int num = 0;
				foreach (PropValue propValue in pva)
				{
					num += propValue.GetBytesToMarshal();
				}
				try
				{
					fixed (byte* ptr = new byte[num])
					{
						PropValue.MarshalToNative(pva, ptr);
						int num2;
						if (base.IsExternal)
						{
							num2 = this.ExternalIExchangeFastTransferEx.SetProps(pva.Length, (SPropValue*)ptr);
						}
						else
						{
							num2 = this.iExchangeFastTransferEx.SetProps(pva.Length, (SPropValue*)ptr);
						}
						if (num2 != 0)
						{
							base.ThrowIfError("IExchangeFastTransferEx.SetProps failed", num2);
						}
					}
				}
				finally
				{
					byte* ptr = null;
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		protected SafeExFastTransferExHandle IExchangeFastTransferEx
		{
			get
			{
				return (SafeExFastTransferExHandle)this.iExchangeFastTransferEx;
			}
		}

		protected IExchangeFastTransferEx ExternalIExchangeFastTransferEx
		{
			get
			{
				return this.externalIExchangeFastTransferEx;
			}
		}

		private IExFastTransferEx iExchangeFastTransferEx;

		private IExchangeFastTransferEx externalIExchangeFastTransferEx;
	}
}
