using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Transport
{
	internal class QueueDiversity
	{
		private QueueDiversity()
		{
		}

		public QueueIdentity QueueId
		{
			get
			{
				return this.queueIdentity;
			}
			private set
			{
				this.queueIdentity = value;
			}
		}

		public int TopCount
		{
			get
			{
				return this.topCount;
			}
			private set
			{
				this.topCount = value;
			}
		}

		public bool IncludeDeferred
		{
			get
			{
				return this.includeDeferred;
			}
			private set
			{
				this.includeDeferred = value;
			}
		}

		public int MaxDiversity
		{
			get
			{
				return this.maxDiversity;
			}
			private set
			{
				this.maxDiversity = value;
			}
		}

		public static bool TryParse(string requestArgument, bool implicitShadow, out QueueDiversity queueDiversity, out string parseError)
		{
			QueueIdentity queueIdentity = QueueIdentity.SubmissionQueueIdentity;
			int num = 3;
			int num2 = 10000;
			bool flag = true;
			queueDiversity = null;
			if (!string.IsNullOrEmpty(requestArgument))
			{
				Match match = QueueDiversity.RegexParser.Match(requestArgument);
				if (!match.Success || match.Value != requestArgument)
				{
					parseError = string.Format("Invalid argument format or options detected in '{0}'.", requestArgument);
					return false;
				}
				Group group = match.Groups["queueId"];
				if (group.Success && !string.IsNullOrEmpty(group.Value))
				{
					queueIdentity = QueueIdentity.Parse(group.Value, implicitShadow);
					if (!queueIdentity.IsLocal)
					{
						parseError = string.Format("Invalid use of server name in the QueueID: {0}. Please use -Server parameter to specify server name.", queueIdentity.Server);
						return false;
					}
					if (queueIdentity.Type == QueueType.Poison)
					{
						parseError = "Queue Diversity does not support retrieving information from Poison Queue at this point.";
						return false;
					}
				}
				Group group2 = match.Groups["top"];
				if (group2.Success && !int.TryParse(group2.Value, out num))
				{
					parseError = string.Format("Invalid option topCount '{0}' specified.", group2.Value);
					return false;
				}
				Group group3 = match.Groups["max"];
				if (group3.Success && !string.IsNullOrEmpty(group3.Value) && !int.TryParse(group3.Value, out num2))
				{
					parseError = string.Format("Invalid option maxDiversity '{0}' specified.", group3.Value);
					return false;
				}
				Group group4 = match.Groups["skipDeferred"];
				if (group4.Success && !string.IsNullOrEmpty(group4.Value))
				{
					flag = false;
				}
			}
			parseError = string.Empty;
			queueDiversity = new QueueDiversity
			{
				IncludeDeferred = flag,
				QueueId = queueIdentity,
				TopCount = num,
				MaxDiversity = num2
			};
			return true;
		}

		public XElement GetDiagnosticInfo(MessageQueue messageQueue)
		{
			XElement xelement = new XElement("QueueDiversity");
			if (this.QueueId.Type != QueueType.Delivery && this.QueueId.Type != QueueType.Submission && this.QueueId.Type != QueueType.Unreachable)
			{
				xelement.Add(new XElement("Mismatch", string.Format("Type Mismatch usage between message queue and QueueIdentity. Current QueueId is {0}", this.QueueId)));
			}
			else if (messageQueue != null && messageQueue.TotalCount > 0)
			{
				this.AddQueueInfo(xelement, messageQueue.TotalCount, messageQueue.DeferredCount);
				this.ReportDiversity(xelement, this.GetDiversityInformation<MessageQueue>(messageQueue));
			}
			else
			{
				this.AddQueueInfo(xelement, 0, 0);
			}
			return xelement;
		}

		public XElement GetDiagnosticInfo(ShadowMessageQueue messageQueue)
		{
			XElement xelement = new XElement("QueueDiversity");
			if (this.QueueId.Type != QueueType.Shadow)
			{
				xelement.Add(new XElement("Mismatch", string.Format("Type Mismatch usage between message queue and QueueIdentity. Current QueueId is {0}", this.QueueId)));
			}
			else if (messageQueue != null && messageQueue.Count > 0)
			{
				this.AddQueueInfo(xelement, messageQueue.Count, 0);
				this.ReportDiversity(xelement, this.GetDiversityInformation<ShadowMessageQueue>(messageQueue));
			}
			else
			{
				this.AddQueueInfo(xelement, 0, 0);
			}
			return xelement;
		}

		public XElement GetComponentAdvice()
		{
			string componentName = QueueDiversity.GetComponentName(this.QueueId.Type);
			string content;
			if (!string.IsNullOrEmpty(componentName))
			{
				content = string.Format("Please use '-component {0}' parameter to retrieve diversity information of QueueId:'{1}'", QueueDiversity.GetComponentName(this.QueueId.Type), this.QueueId.ToString());
			}
			else
			{
				content = string.Format("Queue Diversity can't handle queue identity of QueueId:'{0}'.", this.QueueId.ToString());
			}
			return new XElement("ComponentError", content);
		}

		private static IList<KeyValuePair<string, int>> TakeTop(ICollection<KeyValuePair<string, int>> items, int topCount)
		{
			if (items.Count <= 0)
			{
				return null;
			}
			QueueDiversity.ReverseCompare reverseCompare = new QueueDiversity.ReverseCompare();
			SortedList<KeyValuePair<string, int>, int> sortedList = new SortedList<KeyValuePair<string, int>, int>(reverseCompare);
			foreach (KeyValuePair<string, int> keyValuePair in items)
			{
				if (sortedList.Count < topCount)
				{
					sortedList.Add(keyValuePair, keyValuePair.Value);
				}
				else if (reverseCompare.Compare(keyValuePair, sortedList.Keys[topCount - 1]) < 0)
				{
					sortedList.Add(keyValuePair, keyValuePair.Value);
					sortedList.RemoveAt(topCount);
				}
			}
			return sortedList.Keys;
		}

		private static XElement GetDiagnosticInfo(IEnumerable<KeyValuePair<string, int>> items, string rootName, string itemName, bool resultCompacted)
		{
			if (items == null)
			{
				return null;
			}
			XElement xelement = new XElement(rootName);
			if (resultCompacted)
			{
				string content = string.Format("Count may not be accurate. Use option '{0}:' to set a larger memory allowance for more accurate result.", "max");
				xelement.Add(new XElement("Warning", content));
			}
			foreach (KeyValuePair<string, int> keyValuePair in items)
			{
				XElement xelement2 = new XElement(itemName);
				xelement2.SetAttributeValue("Id", keyValuePair.Key);
				xelement2.SetAttributeValue("Count", keyValuePair.Value);
				xelement.Add(xelement2);
			}
			return xelement;
		}

		private static string GetComponentName(QueueType queueType)
		{
			switch (queueType)
			{
			case QueueType.Delivery:
			case QueueType.Unreachable:
				return "RemoteDelivery";
			case QueueType.Submission:
				return "Categorizer";
			case QueueType.Shadow:
				return "ShadowRedundancy";
			}
			return null;
		}

		private void AddOrUpdatePropertyCount(IDictionary<string, int> items, string property, ref bool compacted)
		{
			if (items.ContainsKey(property))
			{
				items[property]++;
				return;
			}
			if (items.Count == this.MaxDiversity)
			{
				int smallestCount = items.Min((KeyValuePair<string, int> item) => item.Value);
				IList<KeyValuePair<string, int>> list = (from item in items
				where item.Value == smallestCount
				select item).ToList<KeyValuePair<string, int>>();
				foreach (KeyValuePair<string, int> item2 in list)
				{
					items.Remove(item2);
				}
				compacted = true;
			}
			items.Add(property, 1);
		}

		private void AddOrUpdateRecipientCount(IDictionary<string, int> items, IReadOnlyMailItem mailItem, ref bool compacted)
		{
			IEnumerable<MailRecipient> all = mailItem.Recipients.All;
			foreach (MailRecipient mailRecipient in all)
			{
				this.AddOrUpdatePropertyCount(items, mailRecipient.ToString(), ref compacted);
			}
		}

		private void AddQueueInfo(XElement diversity, int totalCount, int deferredCount)
		{
			XElement xelement = new XElement("Queue");
			xelement.SetAttributeValue("Id", this.QueueId.ToString());
			xelement.SetAttributeValue("IncludeDeferred", this.IncludeDeferred.ToString());
			xelement.SetAttributeValue("TotalCount", totalCount);
			xelement.SetAttributeValue("DeferredCount", deferredCount);
			diversity.Add(xelement);
		}

		private void ReportDiversity(XElement diversity, IList<KeyValuePair<string, int>>[] topRepeatingTypes)
		{
			diversity.Add(QueueDiversity.GetDiagnosticInfo(topRepeatingTypes[0], "Organizations", "Organization", this.orgsCompacted));
			diversity.Add(QueueDiversity.GetDiagnosticInfo(topRepeatingTypes[1], "Senders", "Sender", this.sendersCompacted));
			diversity.Add(QueueDiversity.GetDiagnosticInfo(topRepeatingTypes[2], "Recipients", "Recipient", this.recipientsCompacted));
		}

		private IList<KeyValuePair<string, int>>[] GetDiversityInformation<T>(T queue) where T : IQueueVisitor
		{
			Dictionary<string, int>[] groups = new Dictionary<string, int>[]
			{
				new Dictionary<string, int>(),
				new Dictionary<string, int>(),
				new Dictionary<string, int>()
			};
			this.orgsCompacted = false;
			this.sendersCompacted = false;
			this.recipientsCompacted = false;
			queue.ForEach(delegate(IQueueItem mailItem)
			{
				this.AddOrUpdatePropertyCount(groups[0], ((IReadOnlyMailItem)mailItem).ExternalOrganizationId.ToString(), ref this.orgsCompacted);
				this.AddOrUpdatePropertyCount(groups[1], (string)((IReadOnlyMailItem)mailItem).From, ref this.sendersCompacted);
				this.AddOrUpdateRecipientCount(groups[2], (IReadOnlyMailItem)mailItem, ref this.recipientsCompacted);
			}, this.IncludeDeferred);
			return new IList<KeyValuePair<string, int>>[]
			{
				QueueDiversity.TakeTop(groups[0], this.TopCount),
				QueueDiversity.TakeTop(groups[1], this.TopCount),
				QueueDiversity.TakeTop(groups[2], this.TopCount)
			};
		}

		public const string DiversityArgumentName = "diversity";

		private const string SkipDeferredOptionName = "skipDeferred";

		private const string QueueIdOptionName = "queueId";

		private const string TopCountOptionName = "top";

		private const string MaxDiversityOptionName = "max";

		private const int DefaultTopCount = 3;

		private const int DefaultMaxDiversity = 10000;

		public static readonly string UsageString = string.Format("[queueId][/[[{0}:]{{topCount}},][{1}:{{maxDiversity}},][{2}]]", "top", "max", "skipDeferred");

		private static readonly string RegexQueueIdPart = string.Format("\\:*\\s*(?<{0}>[^/\\s]*)", "queueId");

		private static readonly string RegexMaxDiversityPart = string.Format("({0}\\:(?<{0}>\\d+))", "max");

		private static readonly string RegexTopCountPart = string.Format("(({0}\\:)*(?<{0}>\\d+))", "top");

		private static readonly string RegexSkipDeferredPart = string.Format("(?<{0}>{0})", "skipDeferred");

		private static readonly string RegexString = string.Format("\\s*{0}\\s*(/{{1}}\\s*(({1}|{2}|{3})[,\\s]*){{0,3}})?", new object[]
		{
			QueueDiversity.RegexQueueIdPart,
			QueueDiversity.RegexMaxDiversityPart,
			QueueDiversity.RegexTopCountPart,
			QueueDiversity.RegexSkipDeferredPart
		});

		private static readonly Regex RegexParser = new Regex(QueueDiversity.RegexString, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		private bool includeDeferred = true;

		private QueueIdentity queueIdentity = QueueIdentity.SubmissionQueueIdentity;

		private int topCount = 3;

		private int maxDiversity = 10000;

		private bool orgsCompacted;

		private bool sendersCompacted;

		private bool recipientsCompacted;

		private enum GroupBy
		{
			Tenants,
			Senders,
			Recipients
		}

		private class ReverseCompare : IComparer<KeyValuePair<string, int>>
		{
			public int Compare(KeyValuePair<string, int> x, KeyValuePair<string, int> y)
			{
				if (x.Value != y.Value)
				{
					return y.Value - x.Value;
				}
				return string.Compare(y.Key, x.Key, StringComparison.OrdinalIgnoreCase);
			}
		}
	}
}
