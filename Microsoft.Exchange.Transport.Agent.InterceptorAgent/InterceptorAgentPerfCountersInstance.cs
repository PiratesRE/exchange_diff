using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal sealed class InterceptorAgentPerfCountersInstance : PerformanceCounterInstance
	{
		internal InterceptorAgentPerfCountersInstance(string instanceName, InterceptorAgentPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Interceptor Agent")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.MessagesPermanentlyRejected = new ExPerformanceCounter(base.CategoryName, "Messages Permanently Rejected", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesPermanentlyRejected);
				this.MessagesPermanentlyRejectedRate = new ExPerformanceCounter(base.CategoryName, "Messages Permanently Rejected Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesPermanentlyRejectedRate);
				this.MessagesTransientlyRejected = new ExPerformanceCounter(base.CategoryName, "Messages Transiently Rejected", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesTransientlyRejected);
				this.MessagesTransientlyRejectedRate = new ExPerformanceCounter(base.CategoryName, "Messages Transiently Rejected Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesTransientlyRejectedRate);
				this.MessagesDropped = new ExPerformanceCounter(base.CategoryName, "Messages Dropped", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDropped);
				this.MessagesDroppedRate = new ExPerformanceCounter(base.CategoryName, "Messages Dropped Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDroppedRate);
				this.MessagesDeferred = new ExPerformanceCounter(base.CategoryName, "Messages Deferred", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDeferred);
				this.MessagesDeferredRate = new ExPerformanceCounter(base.CategoryName, "Messages Deferred Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDeferredRate);
				this.MessagesDelayed = new ExPerformanceCounter(base.CategoryName, "Messages Delayed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDelayed);
				this.MessagesDelayedRate = new ExPerformanceCounter(base.CategoryName, "Messages Delayed Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDelayedRate);
				this.MessagesArchived = new ExPerformanceCounter(base.CategoryName, "Messages Archived", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesArchived);
				this.MessagesArchivedRate = new ExPerformanceCounter(base.CategoryName, "Messages Archived Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesArchivedRate);
				this.MessageHeadersArchived = new ExPerformanceCounter(base.CategoryName, "Message Headers Archived", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessageHeadersArchived);
				this.MessageHeadersArchivedRate = new ExPerformanceCounter(base.CategoryName, "Message Headers Archived Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessageHeadersArchivedRate);
				this.MatchedOnMailFromMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnMailFrom Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnMailFromMessages);
				this.MatchedOnMailFromMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnMailFrom Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnMailFromMessagesRate);
				this.EvaluatedOnMailFromMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnMailFrom Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnMailFromMessages);
				this.EvaluatedOnMailFromMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnMailFrom Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnMailFromMessagesRate);
				this.MatchedOnRcptToMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnRcptTo Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnRcptToMessages);
				this.MatchedOnRcptToMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnRcptTo Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnRcptToMessagesRate);
				this.EvaluatedOnRcptToMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnRcptTo Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnRcptToMessages);
				this.EvaluatedOnRcptToMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnRcptTo Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnRcptToMessagesRate);
				this.MatchedOnEndOfHeadersMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnEndOfHeaders Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnEndOfHeadersMessages);
				this.MatchedOnEndOfHeadersMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnEndOfHeaders Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnEndOfHeadersMessagesRate);
				this.EvaluatedOnEndOfHeadersMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnEndOfHeaders Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnEndOfHeadersMessages);
				this.EvaluatedOnEndOfHeadersMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnEndOfHeaders Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnEndOfHeadersMessagesRate);
				this.MatchedOnEndOfDataMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnEndOfData Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnEndOfDataMessages);
				this.MatchedOnEndOfDataMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnEndOfData Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnEndOfDataMessagesRate);
				this.EvaluatedOnEndOfDataMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnEndOfData Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnEndOfDataMessages);
				this.EvaluatedOnEndOfDataMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnEndOfData Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnEndOfDataMessagesRate);
				this.MatchedOnSubmittedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnSubmittedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnSubmittedMessageMessages);
				this.MatchedOnSubmittedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnSubmittedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnSubmittedMessageMessagesRate);
				this.EvaluatedOnSubmittedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnSubmittedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnSubmittedMessageMessages);
				this.EvaluatedOnSubmittedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnSubmittedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnSubmittedMessageMessagesRate);
				this.MatchedOnResolvedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnResolvedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnResolvedMessageMessages);
				this.MatchedOnResolvedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnResolvedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnResolvedMessageMessagesRate);
				this.EvaluatedOnResolvedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnResolvedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnResolvedMessageMessages);
				this.EvaluatedOnResolvedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnResolvedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnResolvedMessageMessagesRate);
				this.MatchedOnRoutedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnRoutedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnRoutedMessageMessages);
				this.MatchedOnRoutedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnRoutedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnRoutedMessageMessagesRate);
				this.EvaluatedOnRoutedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnRoutedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnRoutedMessageMessages);
				this.EvaluatedOnRoutedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnRoutedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnRoutedMessageMessagesRate);
				this.MatchedOnCategorizedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnCategorizedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnCategorizedMessageMessages);
				this.MatchedOnCategorizedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnCategorizedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnCategorizedMessageMessagesRate);
				this.EvaluatedOnCategorizedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnCategorizedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnCategorizedMessageMessages);
				this.EvaluatedOnCategorizedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnCategorizedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnCategorizedMessageMessagesRate);
				this.MatchedOnInitMsgMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnInitMsg Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnInitMsgMessages);
				this.MatchedOnInitMsgMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnInitMsg Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnInitMsgMessagesRate);
				this.EvaluatedOnInitMsgMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnInitMsg Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnInitMsgMessages);
				this.EvaluatedOnInitMsgMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnInitMsg Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnInitMsgMessagesRate);
				this.MatchedOnPromotedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnPromotedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnPromotedMessageMessages);
				this.MatchedOnPromotedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnPromotedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnPromotedMessageMessagesRate);
				this.EvaluatedOnPromotedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnPromotedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnPromotedMessageMessages);
				this.EvaluatedOnPromotedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnPromotedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnPromotedMessageMessagesRate);
				this.MatchedOnCreatedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnCreatedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnCreatedMessageMessages);
				this.MatchedOnCreatedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnCreatedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnCreatedMessageMessagesRate);
				this.EvaluatedOnCreatedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnCreatedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnCreatedMessageMessages);
				this.EvaluatedOnCreatedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnCreatedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnCreatedMessageMessagesRate);
				this.MatchedOnDemotedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnDemotedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnDemotedMessageMessages);
				this.MatchedOnDemotedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnDemotedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnDemotedMessageMessagesRate);
				this.EvaluatedOnDemotedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnDemotedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnDemotedMessageMessages);
				this.EvaluatedOnDemotedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnDemotedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnDemotedMessageMessagesRate);
				this.MatchedOnLoadedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnLoadedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnLoadedMessageMessages);
				this.MatchedOnLoadedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnLoadedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnLoadedMessageMessagesRate);
				this.EvaluatedOnLoadedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnLoadedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnLoadedMessageMessages);
				this.EvaluatedOnLoadedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnLoadedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnLoadedMessageMessagesRate);
				long num = this.MessagesPermanentlyRejected.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal InterceptorAgentPerfCountersInstance(string instanceName) : base(instanceName, "MSExchange Interceptor Agent")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.MessagesPermanentlyRejected = new ExPerformanceCounter(base.CategoryName, "Messages Permanently Rejected", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesPermanentlyRejected);
				this.MessagesPermanentlyRejectedRate = new ExPerformanceCounter(base.CategoryName, "Messages Permanently Rejected Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesPermanentlyRejectedRate);
				this.MessagesTransientlyRejected = new ExPerformanceCounter(base.CategoryName, "Messages Transiently Rejected", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesTransientlyRejected);
				this.MessagesTransientlyRejectedRate = new ExPerformanceCounter(base.CategoryName, "Messages Transiently Rejected Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesTransientlyRejectedRate);
				this.MessagesDropped = new ExPerformanceCounter(base.CategoryName, "Messages Dropped", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDropped);
				this.MessagesDroppedRate = new ExPerformanceCounter(base.CategoryName, "Messages Dropped Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDroppedRate);
				this.MessagesDeferred = new ExPerformanceCounter(base.CategoryName, "Messages Deferred", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDeferred);
				this.MessagesDeferredRate = new ExPerformanceCounter(base.CategoryName, "Messages Deferred Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDeferredRate);
				this.MessagesDelayed = new ExPerformanceCounter(base.CategoryName, "Messages Delayed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDelayed);
				this.MessagesDelayedRate = new ExPerformanceCounter(base.CategoryName, "Messages Delayed Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDelayedRate);
				this.MessagesArchived = new ExPerformanceCounter(base.CategoryName, "Messages Archived", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesArchived);
				this.MessagesArchivedRate = new ExPerformanceCounter(base.CategoryName, "Messages Archived Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesArchivedRate);
				this.MessageHeadersArchived = new ExPerformanceCounter(base.CategoryName, "Message Headers Archived", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessageHeadersArchived);
				this.MessageHeadersArchivedRate = new ExPerformanceCounter(base.CategoryName, "Message Headers Archived Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessageHeadersArchivedRate);
				this.MatchedOnMailFromMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnMailFrom Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnMailFromMessages);
				this.MatchedOnMailFromMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnMailFrom Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnMailFromMessagesRate);
				this.EvaluatedOnMailFromMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnMailFrom Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnMailFromMessages);
				this.EvaluatedOnMailFromMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnMailFrom Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnMailFromMessagesRate);
				this.MatchedOnRcptToMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnRcptTo Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnRcptToMessages);
				this.MatchedOnRcptToMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnRcptTo Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnRcptToMessagesRate);
				this.EvaluatedOnRcptToMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnRcptTo Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnRcptToMessages);
				this.EvaluatedOnRcptToMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnRcptTo Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnRcptToMessagesRate);
				this.MatchedOnEndOfHeadersMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnEndOfHeaders Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnEndOfHeadersMessages);
				this.MatchedOnEndOfHeadersMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnEndOfHeaders Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnEndOfHeadersMessagesRate);
				this.EvaluatedOnEndOfHeadersMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnEndOfHeaders Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnEndOfHeadersMessages);
				this.EvaluatedOnEndOfHeadersMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnEndOfHeaders Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnEndOfHeadersMessagesRate);
				this.MatchedOnEndOfDataMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnEndOfData Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnEndOfDataMessages);
				this.MatchedOnEndOfDataMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnEndOfData Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnEndOfDataMessagesRate);
				this.EvaluatedOnEndOfDataMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnEndOfData Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnEndOfDataMessages);
				this.EvaluatedOnEndOfDataMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnEndOfData Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnEndOfDataMessagesRate);
				this.MatchedOnSubmittedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnSubmittedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnSubmittedMessageMessages);
				this.MatchedOnSubmittedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnSubmittedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnSubmittedMessageMessagesRate);
				this.EvaluatedOnSubmittedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnSubmittedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnSubmittedMessageMessages);
				this.EvaluatedOnSubmittedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnSubmittedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnSubmittedMessageMessagesRate);
				this.MatchedOnResolvedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnResolvedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnResolvedMessageMessages);
				this.MatchedOnResolvedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnResolvedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnResolvedMessageMessagesRate);
				this.EvaluatedOnResolvedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnResolvedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnResolvedMessageMessages);
				this.EvaluatedOnResolvedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnResolvedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnResolvedMessageMessagesRate);
				this.MatchedOnRoutedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnRoutedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnRoutedMessageMessages);
				this.MatchedOnRoutedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnRoutedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnRoutedMessageMessagesRate);
				this.EvaluatedOnRoutedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnRoutedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnRoutedMessageMessages);
				this.EvaluatedOnRoutedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnRoutedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnRoutedMessageMessagesRate);
				this.MatchedOnCategorizedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnCategorizedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnCategorizedMessageMessages);
				this.MatchedOnCategorizedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnCategorizedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnCategorizedMessageMessagesRate);
				this.EvaluatedOnCategorizedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnCategorizedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnCategorizedMessageMessages);
				this.EvaluatedOnCategorizedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnCategorizedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnCategorizedMessageMessagesRate);
				this.MatchedOnInitMsgMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnInitMsg Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnInitMsgMessages);
				this.MatchedOnInitMsgMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnInitMsg Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnInitMsgMessagesRate);
				this.EvaluatedOnInitMsgMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnInitMsg Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnInitMsgMessages);
				this.EvaluatedOnInitMsgMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnInitMsg Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnInitMsgMessagesRate);
				this.MatchedOnPromotedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnPromotedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnPromotedMessageMessages);
				this.MatchedOnPromotedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnPromotedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnPromotedMessageMessagesRate);
				this.EvaluatedOnPromotedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnPromotedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnPromotedMessageMessages);
				this.EvaluatedOnPromotedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnPromotedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnPromotedMessageMessagesRate);
				this.MatchedOnCreatedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnCreatedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnCreatedMessageMessages);
				this.MatchedOnCreatedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnCreatedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnCreatedMessageMessagesRate);
				this.EvaluatedOnCreatedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnCreatedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnCreatedMessageMessages);
				this.EvaluatedOnCreatedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnCreatedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnCreatedMessageMessagesRate);
				this.MatchedOnDemotedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnDemotedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnDemotedMessageMessages);
				this.MatchedOnDemotedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnDemotedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnDemotedMessageMessagesRate);
				this.EvaluatedOnDemotedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnDemotedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnDemotedMessageMessages);
				this.EvaluatedOnDemotedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnDemotedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnDemotedMessageMessagesRate);
				this.MatchedOnLoadedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Matched OnLoadedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnLoadedMessageMessages);
				this.MatchedOnLoadedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Matched OnLoadedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MatchedOnLoadedMessageMessagesRate);
				this.EvaluatedOnLoadedMessageMessages = new ExPerformanceCounter(base.CategoryName, "Evaluated OnLoadedMessage Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnLoadedMessageMessages);
				this.EvaluatedOnLoadedMessageMessagesRate = new ExPerformanceCounter(base.CategoryName, "Evaluated OnLoadedMessage Messages Rate", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EvaluatedOnLoadedMessageMessagesRate);
				long num = this.MessagesPermanentlyRejected.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter MessagesPermanentlyRejected;

		public readonly ExPerformanceCounter MessagesPermanentlyRejectedRate;

		public readonly ExPerformanceCounter MessagesTransientlyRejected;

		public readonly ExPerformanceCounter MessagesTransientlyRejectedRate;

		public readonly ExPerformanceCounter MessagesDropped;

		public readonly ExPerformanceCounter MessagesDroppedRate;

		public readonly ExPerformanceCounter MessagesDeferred;

		public readonly ExPerformanceCounter MessagesDeferredRate;

		public readonly ExPerformanceCounter MessagesDelayed;

		public readonly ExPerformanceCounter MessagesDelayedRate;

		public readonly ExPerformanceCounter MessagesArchived;

		public readonly ExPerformanceCounter MessagesArchivedRate;

		public readonly ExPerformanceCounter MessageHeadersArchived;

		public readonly ExPerformanceCounter MessageHeadersArchivedRate;

		public readonly ExPerformanceCounter MatchedOnMailFromMessages;

		public readonly ExPerformanceCounter MatchedOnMailFromMessagesRate;

		public readonly ExPerformanceCounter EvaluatedOnMailFromMessages;

		public readonly ExPerformanceCounter EvaluatedOnMailFromMessagesRate;

		public readonly ExPerformanceCounter MatchedOnRcptToMessages;

		public readonly ExPerformanceCounter MatchedOnRcptToMessagesRate;

		public readonly ExPerformanceCounter EvaluatedOnRcptToMessages;

		public readonly ExPerformanceCounter EvaluatedOnRcptToMessagesRate;

		public readonly ExPerformanceCounter MatchedOnEndOfHeadersMessages;

		public readonly ExPerformanceCounter MatchedOnEndOfHeadersMessagesRate;

		public readonly ExPerformanceCounter EvaluatedOnEndOfHeadersMessages;

		public readonly ExPerformanceCounter EvaluatedOnEndOfHeadersMessagesRate;

		public readonly ExPerformanceCounter MatchedOnEndOfDataMessages;

		public readonly ExPerformanceCounter MatchedOnEndOfDataMessagesRate;

		public readonly ExPerformanceCounter EvaluatedOnEndOfDataMessages;

		public readonly ExPerformanceCounter EvaluatedOnEndOfDataMessagesRate;

		public readonly ExPerformanceCounter MatchedOnSubmittedMessageMessages;

		public readonly ExPerformanceCounter MatchedOnSubmittedMessageMessagesRate;

		public readonly ExPerformanceCounter EvaluatedOnSubmittedMessageMessages;

		public readonly ExPerformanceCounter EvaluatedOnSubmittedMessageMessagesRate;

		public readonly ExPerformanceCounter MatchedOnResolvedMessageMessages;

		public readonly ExPerformanceCounter MatchedOnResolvedMessageMessagesRate;

		public readonly ExPerformanceCounter EvaluatedOnResolvedMessageMessages;

		public readonly ExPerformanceCounter EvaluatedOnResolvedMessageMessagesRate;

		public readonly ExPerformanceCounter MatchedOnRoutedMessageMessages;

		public readonly ExPerformanceCounter MatchedOnRoutedMessageMessagesRate;

		public readonly ExPerformanceCounter EvaluatedOnRoutedMessageMessages;

		public readonly ExPerformanceCounter EvaluatedOnRoutedMessageMessagesRate;

		public readonly ExPerformanceCounter MatchedOnCategorizedMessageMessages;

		public readonly ExPerformanceCounter MatchedOnCategorizedMessageMessagesRate;

		public readonly ExPerformanceCounter EvaluatedOnCategorizedMessageMessages;

		public readonly ExPerformanceCounter EvaluatedOnCategorizedMessageMessagesRate;

		public readonly ExPerformanceCounter MatchedOnInitMsgMessages;

		public readonly ExPerformanceCounter MatchedOnInitMsgMessagesRate;

		public readonly ExPerformanceCounter EvaluatedOnInitMsgMessages;

		public readonly ExPerformanceCounter EvaluatedOnInitMsgMessagesRate;

		public readonly ExPerformanceCounter MatchedOnPromotedMessageMessages;

		public readonly ExPerformanceCounter MatchedOnPromotedMessageMessagesRate;

		public readonly ExPerformanceCounter EvaluatedOnPromotedMessageMessages;

		public readonly ExPerformanceCounter EvaluatedOnPromotedMessageMessagesRate;

		public readonly ExPerformanceCounter MatchedOnCreatedMessageMessages;

		public readonly ExPerformanceCounter MatchedOnCreatedMessageMessagesRate;

		public readonly ExPerformanceCounter EvaluatedOnCreatedMessageMessages;

		public readonly ExPerformanceCounter EvaluatedOnCreatedMessageMessagesRate;

		public readonly ExPerformanceCounter MatchedOnDemotedMessageMessages;

		public readonly ExPerformanceCounter MatchedOnDemotedMessageMessagesRate;

		public readonly ExPerformanceCounter EvaluatedOnDemotedMessageMessages;

		public readonly ExPerformanceCounter EvaluatedOnDemotedMessageMessagesRate;

		public readonly ExPerformanceCounter MatchedOnLoadedMessageMessages;

		public readonly ExPerformanceCounter MatchedOnLoadedMessageMessagesRate;

		public readonly ExPerformanceCounter EvaluatedOnLoadedMessageMessages;

		public readonly ExPerformanceCounter EvaluatedOnLoadedMessageMessagesRate;
	}
}
