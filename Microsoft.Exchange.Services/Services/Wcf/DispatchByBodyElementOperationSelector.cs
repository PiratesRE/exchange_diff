using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class DispatchByBodyElementOperationSelector : IDispatchOperationSelector
	{
		public string SelectOperation(ref Message message)
		{
			DispatchByBodyElementOperationSelector.CheckWcfDelayedException(message);
			DateTime utcNow = DateTime.UtcNow;
			if (message.IsEmpty)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender, ExchangeVersion.Exchange2007);
			}
			if (message.Properties.ContainsKey("ReadMsgEnd"))
			{
				object obj = message.Properties["ReadMsgEnd"];
				if (obj is DateTime)
				{
					DateTime d = (DateTime)obj;
					double totalMilliseconds = (utcNow - d).TotalMilliseconds;
					message.Properties["WcfLatency"] = totalMilliseconds;
				}
			}
			ExchangeVersion currentExchangeVersion = (ExchangeVersion)message.Properties["WS_ServerVersionKey"];
			object obj2;
			if (message.Properties.TryGetValue("DelayedException", out obj2))
			{
				SchemaValidationException ex = obj2 as SchemaValidationException;
				if (ex != null)
				{
					throw FaultExceptionUtilities.DealWithSchemaViolation(ex, message);
				}
				throw (Exception)obj2;
			}
			else
			{
				string key = (string)message.Properties["MethodName"];
				string text = (string)message.Properties["MethodNamespace"];
				if (!"http://schemas.microsoft.com/exchange/services/2006/messages".Equals(text, StringComparison.OrdinalIgnoreCase))
				{
					throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender, currentExchangeVersion);
				}
				MessageHeaderProcessor value = DispatchByBodyElementOperationSelector.headerProcessorMap.Member.GetValue(key);
				message.Properties["MessageHeaderProcessor"] = value;
				message.Properties["ConnectionCostType"] = DispatchByBodyElementOperationSelector.connectionCostTypeMap.Member.GetValue(key);
				WebMethodEntry webMethodEntry = null;
				if (!WebMethodMetadata.Entries.TryGetValue(key, out webMethodEntry))
				{
					throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender, currentExchangeVersion);
				}
				message.Properties["WebMethodEntry"] = webMethodEntry;
				message.Headers.Action = string.Format("{0}/{1}", text, webMethodEntry.Name);
				return webMethodEntry.Name;
			}
		}

		internal static void CheckWcfDelayedException(Message message)
		{
			if (message == null)
			{
				return;
			}
			if (message.Properties == null)
			{
				return;
			}
			object obj = null;
			if (message.Properties.TryGetValue("WS_WcfDelayedExceptionKey", out obj))
			{
				Exception ex = obj as Exception;
				if (ex != null)
				{
					LocalizedException ex2 = ex as LocalizedException;
					if (ex2 != null)
					{
						throw FaultExceptionUtilities.CreateFault(ex2, FaultParty.Sender);
					}
					if (ex is XmlException)
					{
						XmlException ex3 = ex as XmlException;
						SchemaValidationException exception = new SchemaValidationException(ex3, ex3.LineNumber, ex3.LinePosition, ex3.Message);
						throw FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
					}
					throw ex;
				}
			}
		}

		public const string SelectOperationLatencyKey = "SelectOperation";

		private static LazyMember<DictionaryWithDefault<string, MessageHeaderProcessor>> headerProcessorMap = new LazyMember<DictionaryWithDefault<string, MessageHeaderProcessor>>(delegate()
		{
			DictionaryWithDefault<string, MessageHeaderProcessor> dictionaryWithDefault = new DictionaryWithDefault<string, MessageHeaderProcessor>(MessageHeaderProcessor.GetInstance());
			dictionaryWithDefault.Add("GetUserAvailabilityRequest", GetUserAvailabilityMessageHeaderProcessor.GetInstance());
			return dictionaryWithDefault;
		});

		private static LazyMember<DictionaryWithDefault<string, CostType>> connectionCostTypeMap = new LazyMember<DictionaryWithDefault<string, CostType>>(delegate()
		{
			DictionaryWithDefault<string, CostType> dictionaryWithDefault = new DictionaryWithDefault<string, CostType>(CostType.Connection);
			dictionaryWithDefault.Add("GetStreamingEvents", CostType.HangingConnection);
			return dictionaryWithDefault;
		});
	}
}
