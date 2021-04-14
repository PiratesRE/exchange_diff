using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class SendCalendarSharingInviteStatistic
	{
		public SendCalendarSharingInviteStatistic()
		{
			this.requestedDetailLevelCountMap = new Dictionary<string, int>();
			this.delegateFailedInvites = new List<Tuple<Exception, CalendarSharingRecipient>>();
			this.otherFailedInvites = new List<Tuple<Exception, CalendarSharingRecipient>>();
		}

		public void IncreaseSucceededInvite(string detailLevel)
		{
			if (!this.requestedDetailLevelCountMap.ContainsKey(detailLevel))
			{
				this.requestedDetailLevelCountMap.Add(detailLevel, 1);
				return;
			}
			Dictionary<string, int> dictionary;
			(dictionary = this.requestedDetailLevelCountMap)[detailLevel] = dictionary[detailLevel] + 1;
		}

		public void LogFailedInvite(Exception ex, CalendarSharingRecipient recipient)
		{
			List<Tuple<Exception, CalendarSharingRecipient>> list;
			if (recipient.DetailLevelType == CalendarSharingDetailLevel.Delegate)
			{
				list = this.delegateFailedInvites;
			}
			else
			{
				list = this.otherFailedInvites;
			}
			list.Add(new Tuple<Exception, CalendarSharingRecipient>(ex, recipient));
		}

		public string GetSuccessString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.requestedDetailLevelCountMap.Count > 0)
			{
				stringBuilder.Append("Success-[");
				foreach (KeyValuePair<string, int> keyValuePair in this.requestedDetailLevelCountMap)
				{
					stringBuilder.AppendFormat("{0}-{1}|", keyValuePair.Key, keyValuePair.Value);
				}
				stringBuilder.Append("]");
			}
			return stringBuilder.ToString();
		}

		public string GetErrorString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.AppendErrors(stringBuilder, this.delegateFailedInvites, "DelErrs");
			this.AppendErrors(stringBuilder, this.otherFailedInvites, "Errs");
			return stringBuilder.ToString();
		}

		private void AppendErrors(StringBuilder sb, List<Tuple<Exception, CalendarSharingRecipient>> errorList, string errorTag)
		{
			if (errorList.Count > 0)
			{
				sb.AppendFormat("{0}-[", errorTag);
				HashSet<Type> hashSet = new HashSet<Type>();
				foreach (Tuple<Exception, CalendarSharingRecipient> tuple in this.delegateFailedInvites)
				{
					if (!hashSet.Contains(tuple.Item1.GetType()))
					{
						hashSet.Add(tuple.Item1.GetType());
						sb.AppendFormat("PRIP-{0}|EX-{1}:{2}|", tuple.Item2.EmailAddress.EmailAddress, tuple.Item1.GetType().Name, tuple.Item1.Message);
					}
				}
				sb.Append("]");
			}
		}

		private const string SuccessString = "Success-[";

		private const string EndBracket = "]";

		private const string KeyValuePairFormat = "{0}-{1}|";

		private const string DelegateErrors = "DelErrs";

		private const string Errors = "Errs";

		private const string ErrorKeyValuePairFormat = "PRIP-{0}|EX-{1}:{2}|";

		private const string ErrorTagFormat = "{0}-[";

		private Dictionary<string, int> requestedDetailLevelCountMap;

		private List<Tuple<Exception, CalendarSharingRecipient>> delegateFailedInvites;

		private List<Tuple<Exception, CalendarSharingRecipient>> otherFailedInvites;
	}
}
