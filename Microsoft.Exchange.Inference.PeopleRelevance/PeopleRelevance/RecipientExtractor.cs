using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Inference;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RecipientExtractor : BaseComponent
	{
		internal RecipientExtractor()
		{
			this.DiagnosticsSession.ComponentName = "RecipientExtractor";
			this.DiagnosticsSession.Tracer = ExTraceGlobals.RecipientExtractorTracer;
		}

		public override string Description
		{
			get
			{
				return "The RecipientExtractor extracts recipient related properties.";
			}
		}

		public override string Name
		{
			get
			{
				return "RecipientExtractor";
			}
		}

		protected override void InternalProcessDocument(DocumentContext data)
		{
			this.DiagnosticsSession.TraceDebug<IIdentity>("Processing document - {0}", data.Document.Identity);
			object obj;
			IDictionary<string, IInferenceRecipient> dictionary;
			if (data.Document.TryGetProperty(PeopleRelevanceSchema.ContactList, out obj))
			{
				dictionary = (obj as IDictionary<string, IInferenceRecipient>);
			}
			else
			{
				dictionary = new Dictionary<string, IInferenceRecipient>();
			}
			ExDateTime property = data.Document.GetProperty<ExDateTime>(PeopleRelevanceSchema.SentTime);
			bool isReply = false;
			if (data.Document.TryGetProperty(PeopleRelevanceSchema.IsReply, out obj))
			{
				isReply = (bool)obj;
			}
			long num;
			if (!data.Document.TryGetProperty(PeopleRelevanceSchema.CurrentTimeWindowNumber, out obj))
			{
				num = 1L;
				data.Document.SetProperty(PeopleRelevanceSchema.CurrentTimeWindowStartTime, property);
				data.Document.SetProperty(PeopleRelevanceSchema.CurrentTimeWindowNumber, num);
			}
			else
			{
				num = (long)obj;
				TimeSpan t = property - data.Document.GetProperty<ExDateTime>(PeopleRelevanceSchema.CurrentTimeWindowStartTime);
				if (t >= RecipientExtractor.TimeWindowLength)
				{
					num += 1L;
					data.Document.SetProperty(PeopleRelevanceSchema.CurrentTimeWindowStartTime, property);
					data.Document.SetProperty(PeopleRelevanceSchema.CurrentTimeWindowNumber, num);
				}
			}
			data.Document.TryGetProperty(PeopleRelevanceSchema.RecipientsTo, out obj);
			if (obj != null)
			{
				IList<IMessageRecipient> recipients = (IList<IMessageRecipient>)obj;
				this.ProcessRecipients(recipients, dictionary, property, isReply, num);
			}
			data.Document.TryGetProperty(PeopleRelevanceSchema.RecipientsCc, out obj);
			if (obj != null)
			{
				IList<IMessageRecipient> recipients2 = (IList<IMessageRecipient>)obj;
				this.ProcessRecipients(recipients2, dictionary, property, isReply, num);
			}
			data.Document.SetProperty(PeopleRelevanceSchema.ContactList, dictionary);
		}

		private void ProcessRecipients(IList<IMessageRecipient> recipients, IDictionary<string, IInferenceRecipient> contactList, ExDateTime sentTime, bool isReply, long currentTimeWindowNumber)
		{
			foreach (IMessageRecipient recipient in recipients)
			{
				IInferenceRecipient inferenceRecipient = new InferenceRecipient(recipient);
				string key = inferenceRecipient.SmtpAddress.ToLower(CultureInfo.InvariantCulture);
				if (!contactList.ContainsKey(key))
				{
					contactList.Add(key, inferenceRecipient);
				}
				else
				{
					contactList[key].UpdateFromRecipient(inferenceRecipient);
					inferenceRecipient = contactList[key];
				}
				if (inferenceRecipient.TotalSentCount == 0L)
				{
					int num = isReply ? 2 : 6;
					inferenceRecipient.FirstSentTime = sentTime.UniversalTime;
					inferenceRecipient.RawRecipientWeight = (double)(num - 1);
					inferenceRecipient.RecipientRank = int.MaxValue;
				}
				inferenceRecipient.TotalSentCount += 1L;
				inferenceRecipient.LastSentTime = sentTime.UniversalTime;
				inferenceRecipient.LastUsedInTimeWindow = currentTimeWindowNumber;
				inferenceRecipient.RawRecipientWeight += 1.0;
			}
		}

		public const int InitialRecipientWeight = 6;

		public const int InitialRecipientWeightForReplies = 2;

		private const string ComponentDescription = "The RecipientExtractor extracts recipient related properties.";

		private const string ComponentName = "RecipientExtractor";

		public static readonly TimeSpan TimeWindowLength = TimeSpan.FromHours(8.0);
	}
}
