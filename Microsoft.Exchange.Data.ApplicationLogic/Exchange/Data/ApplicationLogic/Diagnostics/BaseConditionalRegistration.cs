using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	internal abstract class BaseConditionalRegistration
	{
		public static bool Initialized { get; private set; }

		public static Dictionary<string, string> PropertyGroups { get; set; }

		public static Func<int> GetHitCountForCookie { get; set; }

		public static void Initialize(string protocolName, ObjectSchema fetchSchema, ObjectSchema querySchema, Dictionary<string, string> customPropertyGroups = null)
		{
			if (BaseConditionalRegistration.Initialized)
			{
				return;
			}
			BaseConditionalRegistration.FetchSchema = fetchSchema;
			BaseConditionalRegistration.QuerySchema = querySchema;
			ConditionalRegistrationLog.ProtocolName = protocolName;
			if (BaseConditionalRegistration.PropertyGroups == null)
			{
				BaseConditionalRegistration.PropertyGroups = new Dictionary<string, string>();
			}
			BaseConditionalRegistration.PropertyGroups.Add("user", BaseConditionalRegistration.GetConfigurationValue("UserPropertyGroup") ?? "SmtpAddress,DisplayName,TenantName,WindowsLiveId,MailboxServer,MailboxDatabase,MailboxServerVersion,IsMonitoringUser");
			BaseConditionalRegistration.PropertyGroups.Add("wlm", BaseConditionalRegistration.GetConfigurationValue("WlmPropertyGroup") ?? "IsOverBudgetAtStart,IsOverBudgetAtEnd,BudgetBalanceStart,BudgetBalanceEnd,BudgetDelay,BudgetUsed,BudgetLockedOut,BudgetLockedUntil");
			BaseConditionalRegistration.PropertyGroups.Add("policy", BaseConditionalRegistration.GetConfigurationValue("PolicyPropertyGroup") ?? "ThrottlingPolicyName,MaxConcurrency,MaxBurst,RechargeRate,CutoffBalance,ThrottlingPolicyScope,ConcurrencyStart,ConcurrencyEnd");
			BaseConditionalRegistration.PropertyGroups.Add("error", BaseConditionalRegistration.GetConfigurationValue("ErrorPropertyGroup") ?? "Exception");
			if (customPropertyGroups != null)
			{
				foreach (string key in customPropertyGroups.Keys)
				{
					if (!BaseConditionalRegistration.PropertyGroups.ContainsKey(key))
					{
						BaseConditionalRegistration.PropertyGroups.Add(key, customPropertyGroups[key]);
					}
				}
			}
			BaseConditionalRegistration.Initialized = true;
			RegisterConditionHandler.GetInstance().HydratePersistentHandlers();
			RegisterConditionHandler.GetInstance().HydrateNonPersistentRegistrations();
		}

		protected static void ParseArgument(string argument, out string selectClause, out string whereClause)
		{
			string text = argument.Trim();
			string text2 = text.ToLower();
			int num = text2.IndexOf("select ");
			int num2 = text2.IndexOf(" where ");
			int num3 = text2.IndexOf(" options ");
			if (num != 0)
			{
				throw new ArgumentException("Conditional must start with 'select'.");
			}
			if (num2 <= "select ".Length)
			{
				throw new ArgumentException("Conditional must contains a 'where' clause which should come after 'select'.");
			}
			selectClause = text.Substring("select ".Length, num2 - "select ".Length);
			whereClause = text.Substring(num2 + " where ".Length, ((num3 == -1) ? text.Length : num3) - (num2 + " where ".Length));
		}

		protected static string GetRightHand(string expression)
		{
			int num = expression.IndexOf('=');
			if (num == -1)
			{
				throw new ArgumentException("Expression should have an equal sign.  Expression: " + expression);
			}
			return expression.Substring(num + 1).Trim();
		}

		protected BaseConditionalRegistration(string cookie, string user, string propertiesToFetch, string whereClause)
		{
			this.User = user;
			this.OriginalFilter = whereClause;
			this.OriginalPropertiesToFetch = propertiesToFetch;
			this.PropertiesToFetch = BaseConditionalRegistration.ParsePropertiesToFetch(propertiesToFetch);
			this.QueryFilter = BaseConditionalRegistration.ParseWhereClause(whereClause);
			this.Created = (ExDateTime)TimeProvider.UtcNow;
			this.Cookie = cookie;
			if (BaseConditionalRegistration.GetHitCountForCookie == null)
			{
				ConditionalRegistrationLog.ConditionalRegistrationHitMetadata hitsForCookie = ConditionalRegistrationLog.GetHitsForCookie(user, cookie);
				if (hitsForCookie != null)
				{
					this.hits = hitsForCookie.HitFiles.Length;
					return;
				}
			}
			else
			{
				this.hits = BaseConditionalRegistration.GetHitCountForCookie();
			}
		}

		public static ObjectSchema FetchSchema { get; set; }

		public static ObjectSchema QuerySchema { get; set; }

		public Action<BaseConditionalRegistration, RemoveReason> OnExpired { get; set; }

		public int CurrentHits
		{
			get
			{
				return this.hits;
			}
		}

		public DateTime LastHitUtc { get; private set; }

		public string User { get; private set; }

		public string OriginalFilter { get; private set; }

		public string OriginalPropertiesToFetch { get; private set; }

		public ExDateTime Created { get; private set; }

		public string Cookie { get; private set; }

		public PropertyDefinition[] PropertiesToFetch { get; private set; }

		public QueryFilter QueryFilter { get; private set; }

		public abstract string Description { get; protected set; }

		public virtual bool ShouldEvaluate
		{
			get
			{
				return true;
			}
		}

		public ConditionalResults Evaluate(IReadOnlyPropertyBag propertyBag)
		{
			if (this.ShouldEvaluate && OpathFilterEvaluator.FilterMatches(this.QueryFilter, propertyBag))
			{
				Interlocked.Increment(ref this.hits);
				this.LastHitUtc = TimeProvider.UtcNow;
				return this.FetchData(propertyBag);
			}
			return null;
		}

		private ConditionalResults FetchData(IReadOnlyPropertyBag propertyBag)
		{
			Dictionary<PropertyDefinition, object> dictionary = new Dictionary<PropertyDefinition, object>();
			foreach (PropertyDefinition propertyDefinition in this.PropertiesToFetch)
			{
				object obj = propertyBag[propertyDefinition];
				if (obj != null)
				{
					dictionary[propertyDefinition] = obj;
				}
			}
			return new ConditionalResults(this, RegistrationResult.Success, dictionary);
		}

		internal static PropertyDefinition[] ParsePropertiesToFetch(string propertiesToFetch)
		{
			if (string.IsNullOrEmpty(propertiesToFetch))
			{
				throw new ArgumentException("propertiesToFetch cannot be null or empty.");
			}
			string[] array = BaseConditionalRegistration.ExpandPropertyGroups(propertiesToFetch);
			if (array.Length > 100)
			{
				throw new ArgumentException(string.Format("A maximum of {0} properties can be requested", 100));
			}
			PropertyDefinition[] array2 = new PropertyDefinition[array.Length];
			int num = 0;
			foreach (string text in array)
			{
				PropertyDefinition propertyDefinition = BaseConditionalRegistration.FetchSchema[text.Trim()];
				if (propertyDefinition == null)
				{
					throw new ArgumentException("Undefined property in properties to fetch: " + text);
				}
				array2[num++] = propertyDefinition;
			}
			return array2;
		}

		internal static QueryFilter ParseWhereClause(string whereClause)
		{
			QueryParser queryParser = new QueryParser(whereClause, BaseConditionalRegistration.QuerySchema, QueryParser.Capabilities.All, null, new QueryParser.ConvertValueFromStringDelegate(QueryParserUtils.ConvertValueFromString));
			return queryParser.ParseTree;
		}

		internal static string GetConfigurationValue(string key)
		{
			return ConfigurationManager.AppSettings[key];
		}

		private static string[] ExpandPropertyGroups(string properties)
		{
			List<string> list = new List<string>();
			foreach (string text in properties.Split(new char[]
			{
				','
			}))
			{
				if (BaseConditionalRegistration.IsPropertyGroup(text))
				{
					list.AddRange(BaseConditionalRegistration.GetPropertiesForPropertyGroup(text.ToLower().Trim(new char[]
					{
						'[',
						']'
					})).Split(new char[]
					{
						','
					}));
				}
				else
				{
					list.Add(text);
				}
			}
			return list.ToArray();
		}

		private static bool IsPropertyGroup(string property)
		{
			return property.StartsWith("[") && property.EndsWith("]");
		}

		private static string GetPropertiesForPropertyGroup(string propertyGroup)
		{
			if (BaseConditionalRegistration.PropertyGroups.ContainsKey(propertyGroup))
			{
				return BaseConditionalRegistration.PropertyGroups[propertyGroup];
			}
			return propertyGroup;
		}

		private const string SelectKeywordWithSpaces = "select ";

		private const string WhereKeywordWithSpaces = " where ";

		protected const string OptionsKeywordWithSpaces = " options ";

		public const string UserPropertyGroup = "SmtpAddress,DisplayName,TenantName,WindowsLiveId,MailboxServer,MailboxDatabase,MailboxServerVersion,IsMonitoringUser";

		public const string WlmPropertyGroup = "IsOverBudgetAtStart,IsOverBudgetAtEnd,BudgetBalanceStart,BudgetBalanceEnd,BudgetDelay,BudgetUsed,BudgetLockedOut,BudgetLockedUntil";

		public const string PolicyPropertyGroup = "ThrottlingPolicyName,MaxConcurrency,MaxBurst,RechargeRate,CutoffBalance,ThrottlingPolicyScope,ConcurrencyStart,ConcurrencyEnd";

		private const string ErrorPropertyGroup = "Exception";

		private const int MaxPropertiesToFetch = 100;

		private int hits;
	}
}
