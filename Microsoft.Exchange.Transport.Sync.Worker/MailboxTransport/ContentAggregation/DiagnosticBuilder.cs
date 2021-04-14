using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DiagnosticBuilder
	{
		public void UpdateSubscriptionDiagnosticMessage(ISyncWorkerData subscription, SyncLogSession syncLogSession, Exception exception)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("SubscriptionDiagnostics", subscription.Diagnostics);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			string diagnostics = string.Empty;
			if (exception != null)
			{
				string blackBoxText = syncLogSession.GetBlackBoxText();
				diagnostics = this.AppendDiagnosticMessage(subscription.Diagnostics, blackBoxText);
			}
			subscription.Diagnostics = diagnostics;
		}

		private string AppendDiagnosticMessage(string oldmessage, string newmessage)
		{
			StringBuilder stringBuilder = new StringBuilder(oldmessage, oldmessage.Length + newmessage.Length);
			if (stringBuilder.Length != 0)
			{
				stringBuilder.Append(Environment.NewLine);
			}
			stringBuilder.Append(newmessage);
			if (stringBuilder.Length > DiagnosticBuilder.MaximumDiagnosticSize)
			{
				int length = stringBuilder.Length - DiagnosticBuilder.MaximumDiagnosticSize + DiagnosticBuilder.MoreCharString.Length;
				stringBuilder.Remove(0, length);
				stringBuilder.Insert(0, DiagnosticBuilder.MoreCharString);
			}
			return stringBuilder.ToString();
		}

		internal static readonly int MaximumDiagnosticSize = 8192;

		internal static readonly string MoreCharString = "...";
	}
}
