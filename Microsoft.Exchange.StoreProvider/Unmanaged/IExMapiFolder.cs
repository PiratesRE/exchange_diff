using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExMapiFolder : IExMapiContainer, IExMapiProp, IExInterface, IDisposeTrackable, IDisposable
	{
		int CreateMessage(int ulFlags, out IExMapiMessage iMessage);

		int CopyMessages(SBinary[] sbinArray, IExMapiFolder iMAPIFolderDest, int ulFlags);

		int CopyMessages_External(SBinary[] sbinArray, IMAPIFolder iMAPIFolderDest, int ulFlags);

		int DeleteMessages(SBinary[] sbinArray, int ulFlags);

		int CreateFolder(int ulFolderType, string lpwszFolderName, string lpwszFolderComment, int ulFlags, out IExMapiFolder iMAPIFolderNew);

		int CopyFolder(int cbEntryId, byte[] lpEntryId, IExMapiFolder iMAPIFolderDest, string lpwszNewFolderName, int ulFlags);

		int CopyFolder_External(int cbEntryId, byte[] lpEntryId, IMAPIFolder iMAPIFolderDest, string lpwszNewFolderName, int ulFlags);

		int DeleteFolder(byte[] lpEntryId, int ulFlags);

		int SetReadFlags(SBinary[] sbinArray, int ulFlags);

		int GetMessageStatus(byte[] lpEntryId, int ulFlags, out MessageStatus pulMessageStatus);

		int SetMessageStatus(byte[] lpEntryId, MessageStatus ulNewStatus, MessageStatus ulNewStatusMask, out MessageStatus pulOldStatus);

		int EmptyFolder(int ulFlags);

		int IsContentAvailable(out bool isContentAvailable);

		int GetReplicaServers(out string[] servers, out uint numberOfCheapServers);

		int SetMessageFlags(int cbEntryId, byte[] lpEntryId, uint ulStatus, uint ulMask);

		int CopyMessagesEx(SBinary[] sbinArray, IExMapiFolder iMAPIFolderDest, int ulFlags, PropValue[] pva);

		int CopyMessagesEx_External(SBinary[] sbinArray, IMAPIFolder iMAPIFolderDest, int ulFlags, PropValue[] pva);

		int SetPropsConditional(Restriction lpRes, PropValue[] lpPropArray, out PropProblem[] lppProblems);

		int CopyMessagesEID(SBinary[] sbinArray, IExMapiFolder iMAPIFolderDest, int ulFlags, PropValue[] lpPropArray, out byte[][] lppEntryIds, out byte[][] lppChangeNumbers);

		int CopyMessagesEID_External(SBinary[] sbinArray, IMAPIFolder iMAPIFolderDest, int ulFlags, PropValue[] lpPropArray, out byte[][] lppEntryIds, out byte[][] lppChangeNumbers);

		int CreateFolderEx(int ulFolderType, string lpwszFolderName, string lpwszFolderComment, byte[] lpEntryId, int ulFlags, out IExMapiFolder iMAPIFolderNew);

		int HrSerializeSRestrictionEx(Restriction prest, out byte[] pbRest);

		int HrDeserializeSRestrictionEx(byte[] pbRest, out Restriction prest);

		int HrSerializeActionsEx(RuleAction[] pActions, out byte[] pbActions);

		int HrDeserializeActionsEx(byte[] pbActions, out RuleAction[] pActions);

		int SetPropsEx(bool trackChanges, ICollection<PropValue> lpPropArray, out PropProblem[] lppProblems);

		int DeletePropsEx(bool trackChanges, ICollection<PropTag> lpPropTags, out PropProblem[] lppProblems);
	}
}
