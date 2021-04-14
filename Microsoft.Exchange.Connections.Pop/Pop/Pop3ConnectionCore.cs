using System;
using System.Collections.Generic;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class Pop3ConnectionCore
	{
		public static AsyncOperationResult<Pop3ResultData> DeleteEmails(Pop3ConnectionContext connectionContext, List<int> messageIds, AsyncCallback callback = null, object callbackState = null)
		{
			return Pop3Client.EndDeleteEmails(Pop3Client.BeginDeleteEmails(connectionContext.Client, messageIds, callback, callbackState, null));
		}

		public static AsyncOperationResult<Pop3ResultData> GetUniqueIds(Pop3ConnectionContext connectionContext, AsyncCallback callback = null, object callbackState = null)
		{
			return Pop3Client.EndGetUniqueIds(Pop3Client.BeginGetUniqueIds(connectionContext.Client, callback, callbackState, null));
		}

		public static AsyncOperationResult<Pop3ResultData> GetEmail(Pop3ConnectionContext connectionContext, int messageId, AsyncCallback callback = null, object callbackState = null)
		{
			return Pop3Client.EndGetEmail(Pop3Client.BeginGetEmail(connectionContext.Client, messageId, callback, callbackState, null));
		}

		public static AsyncOperationResult<Pop3ResultData> Quit(Pop3ConnectionContext connectionContext, AsyncCallback callback = null, object callbackState = null)
		{
			return Pop3Client.EndQuit(Pop3Client.BeginQuit(connectionContext.Client, callback, callbackState, null));
		}

		public static AsyncOperationResult<Pop3ResultData> VerifyAccount(Pop3ConnectionContext connectionContext, AsyncCallback callback = null, object callbackState = null)
		{
			return Pop3Client.EndVerifyAccount(Pop3Client.BeginVerifyAccount(connectionContext.Client, callback, callbackState, null));
		}
	}
}
