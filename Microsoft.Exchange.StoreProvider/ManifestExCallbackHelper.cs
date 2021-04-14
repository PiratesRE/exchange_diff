using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ManifestExCallbackHelper : ContentsManifestCallbackHelperBase<IMapiManifestExCallback>, IExchangeManifestExCallback
	{
		public ManifestExCallbackHelper() : base(false)
		{
		}

		unsafe int IExchangeManifestExCallback.Change(ExchangeManifestCallbackChangeFlags flags, int cpvalHeader, SPropValue* ppvalHeader, int cpvalProps, SPropValue* ppvalProps)
		{
			return base.Change(flags, cpvalHeader, ppvalHeader, cpvalProps, ppvalProps);
		}

		int IExchangeManifestExCallback.Delete(ExchangeManifestCallbackDeleteFlags flags, int cbIdsetDeleted, IntPtr pbIdsetDeleted)
		{
			if (cbIdsetDeleted > 0)
			{
				byte[] idsetDeleted = new byte[cbIdsetDeleted];
				Marshal.Copy(pbIdsetDeleted, idsetDeleted, 0, cbIdsetDeleted);
				ExchangeManifestCallbackDeleteFlags localFlags = flags;
				base.Deletes.Enqueue((IMapiManifestExCallback callback) => callback.Delete(idsetDeleted, (localFlags & ExchangeManifestCallbackDeleteFlags.SoftDelete) == ExchangeManifestCallbackDeleteFlags.SoftDelete, (localFlags & ExchangeManifestCallbackDeleteFlags.Expiry) == ExchangeManifestCallbackDeleteFlags.Expiry));
			}
			return 0;
		}

		int IExchangeManifestExCallback.Read(ExchangeManifestCallbackReadFlags flags, int cbIdsetReadUnread, IntPtr pbIdsetReadUnread)
		{
			if (cbIdsetReadUnread > 0)
			{
				byte[] idsetReadUnread = new byte[cbIdsetReadUnread];
				Marshal.Copy(pbIdsetReadUnread, idsetReadUnread, 0, cbIdsetReadUnread);
				bool isRead = (flags & ExchangeManifestCallbackReadFlags.Read) == ExchangeManifestCallbackReadFlags.Read;
				base.Reads.Enqueue((IMapiManifestExCallback callback) => callback.ReadUnread(idsetReadUnread, isRead));
			}
			return 0;
		}

		protected override ManifestCallbackStatus DoChangeCallback(IMapiManifestExCallback callback, ManifestChangeType changeType, PropValue[] headerProps, PropValue[] messageProps)
		{
			return callback.Change(changeType == ManifestChangeType.Add, headerProps, messageProps);
		}
	}
}
