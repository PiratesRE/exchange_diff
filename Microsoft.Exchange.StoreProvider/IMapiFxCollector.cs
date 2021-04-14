using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMapiFxCollector
	{
		Guid GetObjectType();

		byte[] GetServerVersion();

		bool IsPrivateLogon();

		void Config(int ulFlags, int ulTransferMethod);

		void TransferBuffer(byte[] data);

		void IsInterfaceOk(int ulTransferMethod, Guid refiid, int ulFlags);

		void TellPartnerVersion(byte[] versionData);

		void StartMdbEventsImport();

		void FinishMdbEventsImport(bool success);

		void AddMdbEvents(byte[] request);

		void SetWatermarks(MDBEVENTWMRAW[] WMs);

		void SetReceiveFolder(byte[] entryId, string messageClass);

		void SetPerUser(MapiLtidNative ltid, Guid pguidReplica, int lib, byte[] pb, bool fLast);

		void SetProps(PropValue[] pva);
	}
}
