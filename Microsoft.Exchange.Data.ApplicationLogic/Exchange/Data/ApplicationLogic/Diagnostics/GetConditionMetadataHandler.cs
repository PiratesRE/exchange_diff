using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	internal sealed class GetConditionMetadataHandler : ExchangeDiagnosableWrapper<GetConditionMetadataResult>
	{
		protected override string UsageText
		{
			get
			{
				return "This handler will return information about either a single registration or all registrations depending on what you pass into the argument.  To use this handler, the component or “method name” is GetConditionMetadata. \r\n                        Below are examples for using this diagnostics handler: ";
			}
		}

		protected override string UsageSample
		{
			get
			{
				return "Example 1: To get information about a single registration, use the argument: cookie=string\r\n                                    Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component GetConditionMetadata -Argument \"cookie=991eb6cf-24c1-4957-86bb-2082e4d261f6\"\r\n\r\n                                    Example 2: If you omit the argument to the GetConditionMetadata call, you will get data about all active and completed registrations by current user.\r\n                                    Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component GetConditionalMetadata\r\n\r\n                                    Example 3: To get the conditional registrations by all users use ShowAll Argument.\r\n                                    Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component GetConditionalMetada -Argument ShowAll";
			}
		}

		public static GetConditionMetadataHandler GetInstance()
		{
			if (GetConditionMetadataHandler.instance == null)
			{
				lock (GetConditionMetadataHandler.lockObject)
				{
					if (GetConditionMetadataHandler.instance == null)
					{
						GetConditionMetadataHandler.instance = new GetConditionMetadataHandler();
					}
				}
			}
			return GetConditionMetadataHandler.instance;
		}

		private GetConditionMetadataHandler()
		{
		}

		protected override string ComponentName
		{
			get
			{
				return "GetConditionMetadata";
			}
		}

		internal override GetConditionMetadataResult GetExchangeDiagnosticsInfoData(DiagnosableParameters argument)
		{
			string userIdentity = argument.UserIdentity.Replace("/", "-");
			if (string.IsNullOrEmpty(argument.Argument))
			{
				return this.GetAllMetadata(userIdentity);
			}
			if (string.Equals(argument.Argument, "showall", StringComparison.InvariantCultureIgnoreCase))
			{
				return this.GetAllMetadata("");
			}
			if (argument.Argument.Trim().ToLower().StartsWith("cookie="))
			{
				return this.GetMetaData(argument.Argument, userIdentity);
			}
			throw new ArgumentException("Expected argument to be empty for current user's metadata or \" ShowAll \" for ALL metdata, or be in format cookie={cookie guid} for single registration lookup.  Encountered: " + argument.Argument);
		}

		private GetConditionMetadataResult GetMetaData(string argument, string userIdentity)
		{
			int num = argument.IndexOf('=');
			string cookie = argument.Substring(num + 1);
			BaseConditionalRegistration reg;
			ConditionalRegistrationLog.ConditionalRegistrationHitMetadata hit;
			ConditionalRegistrationCache.Singleton.GetRegistrationMetadata(userIdentity, cookie, out reg, out hit);
			ActiveConditionalMetadataResult activeConditionalMetadataResult;
			ConditionalMetadataResult conditionalMetadataResult;
			this.FillCorrectResultType(reg, hit, out activeConditionalMetadataResult, out conditionalMetadataResult);
			GetConditionMetadataResult getConditionMetadataResult = new GetConditionMetadataResult();
			if (activeConditionalMetadataResult != null)
			{
				getConditionMetadataResult.ActiveConditions = new ActiveConditionalMetadataResult[]
				{
					activeConditionalMetadataResult
				};
			}
			if (conditionalMetadataResult != null)
			{
				getConditionMetadataResult.CompletedConditions = new ConditionalMetadataResult[]
				{
					conditionalMetadataResult
				};
			}
			return getConditionMetadataResult;
		}

		private GetConditionMetadataResult GetAllMetadata(string userIdentity = "")
		{
			GetConditionMetadataResult getConditionMetadataResult = new GetConditionMetadataResult();
			List<ActiveConditionalMetadataResult> list = new List<ActiveConditionalMetadataResult>();
			List<ConditionalMetadataResult> list2 = new List<ConditionalMetadataResult>();
			List<BaseConditionalRegistration> list3;
			List<ConditionalRegistrationLog.ConditionalRegistrationHitMetadata> list4;
			ConditionalRegistrationCache.Singleton.GetRegistrationMetadata(userIdentity, out list3, out list4);
			List<string> list5 = new List<string>();
			if (list4 != null)
			{
				foreach (ConditionalRegistrationLog.ConditionalRegistrationHitMetadata conditionalRegistrationHitMetadata in list4)
				{
					ConditionalRegistration reg = null;
					foreach (BaseConditionalRegistration baseConditionalRegistration in list3)
					{
						ConditionalRegistration conditionalRegistration = (ConditionalRegistration)baseConditionalRegistration;
						if (conditionalRegistration.Cookie == conditionalRegistrationHitMetadata.Cookie)
						{
							reg = conditionalRegistration;
							list5.Add(conditionalRegistration.Cookie);
							break;
						}
					}
					ActiveConditionalMetadataResult activeConditionalMetadataResult;
					ConditionalMetadataResult conditionalMetadataResult;
					this.FillCorrectResultType(reg, conditionalRegistrationHitMetadata, out activeConditionalMetadataResult, out conditionalMetadataResult);
					if (activeConditionalMetadataResult != null)
					{
						list.Add(activeConditionalMetadataResult);
					}
					else if (conditionalMetadataResult != null)
					{
						list2.Add(conditionalMetadataResult);
					}
				}
			}
			foreach (BaseConditionalRegistration baseConditionalRegistration2 in list3)
			{
				if (!list5.Contains(baseConditionalRegistration2.Cookie))
				{
					ActiveConditionalMetadataResult activeConditionalMetadataResult2;
					ConditionalMetadataResult conditionalMetadataResult2;
					this.FillCorrectResultType(baseConditionalRegistration2, null, out activeConditionalMetadataResult2, out conditionalMetadataResult2);
					activeConditionalMetadataResult2.Cookie = baseConditionalRegistration2.Cookie;
					list.Add(activeConditionalMetadataResult2);
				}
			}
			getConditionMetadataResult.ActiveConditions = list.ToArray();
			getConditionMetadataResult.CompletedConditions = list2.ToArray();
			return getConditionMetadataResult;
		}

		private void FillCorrectResultType(BaseConditionalRegistration reg, ConditionalRegistrationLog.ConditionalRegistrationHitMetadata hit, out ActiveConditionalMetadataResult active, out ConditionalMetadataResult expired)
		{
			if (reg != null)
			{
				expired = null;
				active = new ActiveConditionalMetadataResult();
				if (hit != null)
				{
					GetConditionMetadataHandler.FillHitData(active, hit);
				}
				active.Created = (DateTime)reg.Created;
				active.Cookie = reg.Cookie;
				active.Description = reg.Description;
				active.SelectClause = reg.OriginalPropertiesToFetch;
				active.WhereClause = reg.OriginalFilter;
				ConditionalRegistration conditionalRegistration = reg as ConditionalRegistration;
				if (conditionalRegistration != null)
				{
					active.MaxHits = conditionalRegistration.MaxHits;
					active.TimeRemaining = ((DateTime)reg.Created.Add(conditionalRegistration.TimeToLive) - DateTime.UtcNow).ToString();
					return;
				}
				active.MaxHits = int.MaxValue;
				active.TimeRemaining = "Persistent";
				return;
			}
			else
			{
				if (hit != null)
				{
					active = null;
					expired = new ConditionalMetadataResult();
					GetConditionMetadataHandler.FillHitData(expired, hit);
					return;
				}
				active = null;
				expired = null;
				return;
			}
		}

		private static void FillHitData(ConditionalMetadataResult result, ConditionalRegistrationLog.ConditionalRegistrationHitMetadata hit)
		{
			result.Cookie = hit.Cookie;
			if (hit == null || hit.HitFiles.Length <= 0)
			{
				result.Files = null;
				result.CurrentHits = 0;
				return;
			}
			FileInfo fileInfo = hit.HitFiles[0];
			XmlDocument xmlDocument = new XmlDocument();
			using (StreamReader streamReader = fileInfo.OpenText())
			{
				xmlDocument.LoadXml(streamReader.ReadToEnd());
			}
			result.Created = ((xmlDocument.SelectSingleNode("Registration/CreationDate") != null) ? DateTime.Parse(xmlDocument.SelectSingleNode("Registration/CreationDate").InnerText) : DateTime.MinValue);
			result.Description = ((xmlDocument.SelectSingleNode("Registration/Description") != null) ? xmlDocument.SelectSingleNode("Registration/Description").InnerText : string.Empty);
			result.WhereClause = ((xmlDocument.SelectSingleNode("Registration/OriginalFilter") != null) ? xmlDocument.SelectSingleNode("Registration/OriginalFilter").InnerText : string.Empty);
			result.SelectClause = ((xmlDocument.SelectSingleNode("Registration/Fetch") != null) ? xmlDocument.SelectSingleNode("Registration/Fetch").InnerText : string.Empty);
			result.CurrentHits = hit.HitFiles.Length;
			result.MaxHits = ((xmlDocument.SelectSingleNode("Registration/MaxHits") != null) ? int.Parse(xmlDocument.SelectSingleNode("Registration/MaxHits").InnerText) : 10);
			result.Files = new string[hit.HitFiles.Length];
			for (int i = 0; i < result.Files.Length; i++)
			{
				result.Files[i] = hit.HitFiles[i].Name;
			}
			result.CurrentHits = hit.HitFiles.Length;
		}

		private const string CookieArg = "cookie=";

		private const string ShowAllArg = "showall";

		private static GetConditionMetadataHandler instance;

		private static object lockObject = new object();
	}
}
