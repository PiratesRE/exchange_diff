using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MapiIStream : IStream
	{
		internal MapiIStream(Stream ioStream)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(16))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string>(14674, 16, (long)this.GetHashCode(), "MapiIStream.MapiIStream: this={0}, ioStream={1}", TraceUtils.MakeHash(this), TraceUtils.MakeHash(ioStream));
			}
			this.ioStream = ioStream;
		}

		public void Clone(out IStream ppstm)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(16))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(9554, 16, (long)this.GetHashCode(), "MapiIStream.Clone results: ppstm={0}", TraceUtils.MakeHash(this));
			}
			ppstm = this;
		}

		public void Commit(int grfCommitFlags)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(16))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(13650, 16, (long)this.GetHashCode(), "MapiIStream.Commit params: grfCommitFlags=0x{0}", grfCommitFlags.ToString("x"));
			}
			this.ioStream.Flush();
		}

		public unsafe void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(16))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string>(11602, 16, (long)this.GetHashCode(), "MapiIStream.CopyTo params: pstm={0}, cb={1}", TraceUtils.MakeHash(pstm), cb.ToString());
			}
			byte[] array = new byte[cb];
			int* ptr = (int*)((void*)pcbRead);
			int num = this.ioStream.Read(array, 0, (int)cb);
			pstm.Write(array, (int)cb, pcbWritten);
			if (ptr != null)
			{
				*ptr = num;
			}
			if (ComponentTrace<MapiNetTags>.CheckEnabled(16))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(15698, 16, (long)this.GetHashCode(), "MapiIStream.CopyTo results: cbRead={0}", num.ToString());
			}
		}

		public void LockRegion(long libOffset, long cb, int dwLockType)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(16))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string, string>(8786, 16, (long)this.GetHashCode(), "MapiIStream.LockRegion params: libOffset={0}, cb={1}, dwLockType={2}", libOffset.ToString(), cb.ToString(), dwLockType.ToString());
			}
			throw MapiExceptionHelper.NotSupportedException("MapiIStream.LockRegion not supported");
		}

		public unsafe void Read(byte[] pv, int cb, IntPtr pcbRead)
		{
			int* ptr = (int*)((void*)pcbRead);
			int num = this.ioStream.Read(pv, 0, cb);
			if (ptr != null)
			{
				*ptr = num;
			}
			if (ComponentTrace<MapiNetTags>.CheckEnabled(16))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string>(12882, 16, (long)this.GetHashCode(), "MapiIStream.Read: cb={0}, cbRead={1}", cb.ToString(), num.ToString());
			}
		}

		public void Revert()
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(16))
			{
				ComponentTrace<MapiNetTags>.Trace(10834, 16, (long)this.GetHashCode(), "MapiIStream.Revert");
			}
		}

		public unsafe void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
		{
			long* ptr = (long*)((void*)plibNewPosition);
			switch (dwOrigin)
			{
			case 0:
				this.ioStream.Position = dlibMove;
				break;
			case 1:
			{
				long position = this.ioStream.Position;
				this.ioStream.Position = position + dlibMove;
				break;
			}
			case 2:
			{
				long length = this.ioStream.Length;
				this.ioStream.Position = length + dlibMove;
				break;
			}
			default:
				throw MapiExceptionHelper.ArgumentException("dwOrigin", "Invalid dwOrigin for MapiIStream.Seek");
			}
			if (ptr != null)
			{
				*ptr = this.ioStream.Position;
			}
			if (ComponentTrace<MapiNetTags>.CheckEnabled(16))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string, string>(14930, 16, (long)this.GetHashCode(), "MapiIStream.Seek: dlibMove={0}, dwOrigin={1}, newPosition={2}", dlibMove.ToString(), dwOrigin.ToString(), this.ioStream.Position.ToString());
			}
		}

		public void SetSize(long libNewSize)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(16))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(9810, 16, (long)this.GetHashCode(), "MapiIStream.SetSize params: libNewSize={0}", libNewSize.ToString());
			}
			this.ioStream.SetLength(libNewSize);
		}

		public void Stat(out STATSTG pstatstg, int grfStatFlag)
		{
			if (grfStatFlag != 1)
			{
				throw MapiExceptionHelper.NotSupportedException("MapiIStream.Stat does not support return STATSTG.pwcsName");
			}
			STATSTG statstg = default(STATSTG);
			statstg.type = 2;
			statstg.cbSize = this.ioStream.Length;
			if (ComponentTrace<MapiNetTags>.CheckEnabled(16))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string>(13906, 16, (long)this.GetHashCode(), "MapiIStream.Stat: grfStatFlag=0x{0}, ioStream.Length={1}", grfStatFlag.ToString("x"), this.ioStream.Length.ToString());
			}
			pstatstg = statstg;
		}

		public void UnlockRegion(long libOffset, long cb, int dwLockType)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(16))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string, string>(11858, 16, (long)this.GetHashCode(), "MapiIStream.UnlockRegion params: libOffset={0}, cb={1}, dwLockType={2}", libOffset.ToString(), cb.ToString(), dwLockType.ToString());
			}
			throw MapiExceptionHelper.NotSupportedException("MapiIStream.LockRegion not supported");
		}

		public unsafe void Write(byte[] pv, int cb, IntPtr pcbWritten)
		{
			int* ptr = (int*)((void*)pcbWritten);
			this.ioStream.Write(pv, 0, cb);
			if (ptr != null)
			{
				*ptr = cb;
			}
			if (ComponentTrace<MapiNetTags>.CheckEnabled(16))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string>(15954, 16, (long)this.GetHashCode(), "MapiIStream.Write: cb={0}, cbWritten={1}", cb.ToString(), cb.ToString());
			}
		}

		private Stream ioStream;
	}
}
