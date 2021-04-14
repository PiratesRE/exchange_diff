using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExFastTransferEx : IExInterface, IDisposeTrackable, IDisposable
	{
		int Config(int ulFlags, int ulTransferMethod);

		int TransferBuffer(int cbData, byte[] data, out int cbProcessed);

		int IsInterfaceOk(int ulTransferMethod, ref Guid refiid, IntPtr lpPropTagArray, int ulFlags);

		int GetObjectType(out Guid iid);

		int GetLastLowLevelError(out int lowLevelError);

		unsafe int GetServerVersion(int cbBufferSize, byte* pBuffer, out int cbBuffer);

		unsafe int TellPartnerVersion(int cbBuffer, byte* pBuffer);

		int IsPrivateLogon();

		int StartMdbEventsImport();

		int FinishMdbEventsImport(bool bSuccess);

		int SetWatermarks(int cWMs, MDBEVENTWMRAW[] WMs);

		int AddMdbEvents(int cbRequest, byte[] pbRequest);

		int SetReceiveFolder(int cbEntryId, byte[] entryId, string messageClass);

		unsafe int SetPerUser(ref MapiLtidNative pltid, Guid* guidReplica, int lib, byte[] pb, int cb, bool fLast);

		unsafe int SetProps(int cValues, SPropValue* lpPropArray);
	}
}
