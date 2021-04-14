using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants
{
	internal class OnlineDiagnostics : ExchangeDiagnosableWrapper<QueryResponse>
	{
		public static OnlineDiagnostics Instance
		{
			get
			{
				return OnlineDiagnostics.instance;
			}
		}

		protected override string ComponentName
		{
			get
			{
				return "EBA";
			}
		}

		private OnlineDiagnostics()
		{
			this.dispatcher = new Dictionary<string, QueryTemplate>(StringComparer.OrdinalIgnoreCase);
			this.dispatcher.Add("DatabaseManager", new QueryTemplate(new Func<object[], QueryFilter, List<QueryableObject>>(this.QueryDatabaseManager), new SimpleProviderPropertyDefinition[0]));
			this.dispatcher.Add("AssistantType", new QueryTemplate(new Func<object[], QueryFilter, List<QueryableObject>>(this.QueryAssistantType), new SimpleProviderPropertyDefinition[0]));
			this.dispatcher.Add("OnlineDatabase", new QueryTemplate(new Func<object[], QueryFilter, List<QueryableObject>>(this.QueryOnlineDatabase), new SimpleProviderPropertyDefinition[]
			{
				QueryableObjectSchema.DatabaseGuid
			}));
			this.dispatcher.Add("EventController", new QueryTemplate(new Func<object[], QueryFilter, List<QueryableObject>>(this.QueryEventController), new SimpleProviderPropertyDefinition[]
			{
				QueryableObjectSchema.DatabaseGuid
			}));
			this.dispatcher.Add("MailboxDispatcher", new QueryTemplate(new Func<object[], QueryFilter, List<QueryableObject>>(this.QueryMailboxDispatcher), new SimpleProviderPropertyDefinition[]
			{
				QueryableObjectSchema.DatabaseGuid,
				QueryableObjectSchema.MailboxGuid
			}));
			this.dispatcher.Add("EventDispatcher", new QueryTemplate(new Func<object[], QueryFilter, List<QueryableObject>>(this.QueryEventDispatcher), new SimpleProviderPropertyDefinition[]
			{
				QueryableObjectSchema.DatabaseGuid,
				QueryableObjectSchema.MailboxGuid,
				QueryableObjectSchema.AssistantGuid
			}));
		}

		public void RegisterDatabaseManager(DatabaseManager databaseManager)
		{
			if (!this.registered && databaseManager != null)
			{
				this.databaseManager = databaseManager;
				ExchangeDiagnosticsHelper.RegisterDiagnosticsComponents(this);
				this.registered = true;
			}
		}

		internal override QueryResponse GetExchangeDiagnosticsInfoData(DiagnosableParameters arguments)
		{
			QueryResponse result;
			try
			{
				if (this.databaseManager == null)
				{
					result = QueryResponse.CreateError("DatabaseManager is not start yet, or not registered for online diagnostics");
				}
				else if (string.IsNullOrEmpty(arguments.Argument))
				{
					StringBuilder sb = new StringBuilder();
					this.dispatcher.Keys.ToList<string>().ForEach(delegate(string x)
					{
						sb.AppendFormat(" {0}", x);
					});
					result = QueryResponse.CreateError("Please specify argument paramter, supported objectClass:" + sb.ToString());
				}
				else
				{
					QueryParser queryParser = new QueryParser(arguments.Argument, ObjectSchema.GetInstance<QueryableObjectSchema>(), QueryParser.Capabilities.All, null, new QueryParser.ConvertValueFromStringDelegate(QueryParserUtils.ConvertValueFromString));
					QueryFilter filter = queryParser.ParseTree;
					filter = QueryFilter.SimplifyFilter(filter);
					result = this.ExecuteQuery(filter);
				}
			}
			catch (ArgumentException ex)
			{
				result = QueryResponse.CreateError(ex.ToString());
			}
			return result;
		}

		protected override string UsageSample
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				this.dispatcher.Keys.ToList<string>().ForEach(delegate(string x)
				{
					sb.AppendFormat("\r\n{0}", x);
				});
				return " Example 1: Get DatabaseManager\r\n                        Get-ExchangeDiagnosticInfo -Process MSExchangeMailboxAssistants -Component EBA -Argument \"ObjectClass -eq 'DatabaseManager\";\r\n\r\n                        Example 2: Get a particular onlineDatabase\r\n                        Get-ExchangeDiagnosticInfo -Process MSExchangeMailboxAssistants -Component EBA -Argument \"ObjectClass -eq 'OnlineDatabase' -and DatabaseGuid -eq 'ad55da35-71e1-477c-80f3-6353cc1dfb4e'\";\r\n\r\n                        Example 3: Get a particular EventDispatcher\r\n                        Get-ExchangeDiagnosticInfo -Process MSExchangeMailboxAssistants -Component EBA -Argument \"ObjectClass -eq 'EventDispatcher' -and DatabaseGuid -eq 'ad55da35-71e1-477c-80f3-6353cc1dfb4e' -and MailboxGuid -eq '55548e2a-7867-4291-a539-cee04febfeff'-and AssistantGuid -eq '15166323-526d-4bbf-b9b0-4ef6226acc03'\";\r\n\r\n                        Supported ObjectClass:\r\n                        " + sb.ToString();
			}
		}

		private QueryResponse ExecuteQuery(QueryFilter filter)
		{
			string text = null;
			if (!this.ExtractIdentityPropertyFromFilter<string>(QueryableObjectSchema.ObjectClass, ref filter, out text))
			{
				return QueryResponse.CreateError("Unable to extract object class from filter");
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "DatabaseManager";
			}
			QueryTemplate queryTemplate = null;
			if (!this.dispatcher.TryGetValue(text, out queryTemplate) || queryTemplate == null)
			{
				StringBuilder sb = new StringBuilder();
				this.dispatcher.Keys.ToList<string>().ForEach(delegate(string x)
				{
					sb.AppendFormat(" {0}", x);
				});
				return QueryResponse.CreateError("Unsupported objectClass, please use one of the supported objectClass:" + sb.ToString());
			}
			List<object> list = null;
			if (queryTemplate.Parameters != null)
			{
				list = new List<object>(queryTemplate.Parameters.Length);
				for (int i = 0; i < queryTemplate.Parameters.Length; i++)
				{
					SimpleProviderPropertyDefinition simpleProviderPropertyDefinition = queryTemplate.Parameters[i];
					object item = null;
					if (!this.ExtractIdentityPropertyFromFilter<object>(simpleProviderPropertyDefinition, ref filter, out item))
					{
						if (i < queryTemplate.Parameters.Length - 1)
						{
							return QueryResponse.CreateError("Missing required property \"" + simpleProviderPropertyDefinition.Name + "\"");
						}
						item = null;
					}
					list.Add(item);
				}
			}
			List<QueryableObject> source = queryTemplate.Executor((list != null) ? list.ToArray() : null, filter);
			List<Dictionary<string, object>> obj = (from x in source
			select x.ToDictionary()).ToList<Dictionary<string, object>>();
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return new QueryResponse(text, javaScriptSerializer.Serialize(obj));
		}

		private List<QueryableObject> QueryDatabaseManager(object[] parameters, QueryFilter filter)
		{
			QueryableDatabaseManager queryableDatabaseManager = new QueryableDatabaseManager();
			OnlineDiagnostics.Instance.databaseManager.ExportToQueryableObject(queryableDatabaseManager);
			List<QueryableObject> list = new List<QueryableObject>(1);
			if (filter == null || OpathFilterEvaluator.FilterMatches(filter, queryableDatabaseManager))
			{
				list.Add(queryableDatabaseManager);
			}
			return list;
		}

		private List<QueryableObject> QueryAssistantType(object[] parameters, QueryFilter filter)
		{
			List<QueryableObject> list = new List<QueryableObject>(OnlineDiagnostics.Instance.databaseManager.AssistantTypes.Length);
			foreach (AssistantType assistantType in OnlineDiagnostics.Instance.databaseManager.AssistantTypes)
			{
				QueryableEventBasedAssistantType queryableEventBasedAssistantType = new QueryableEventBasedAssistantType();
				assistantType.ExportToQueryableObject(queryableEventBasedAssistantType);
				if (filter == null || OpathFilterEvaluator.FilterMatches(filter, queryableEventBasedAssistantType))
				{
					list.Add(queryableEventBasedAssistantType);
				}
			}
			return list;
		}

		private List<QueryableObject> QueryOnlineDatabase(object[] parameters, QueryFilter filter)
		{
			IList<OnlineDatabase> onlineDatabases = OnlineDiagnostics.Instance.databaseManager.GetOnlineDatabases((Guid?)parameters[0]);
			List<QueryableObject> list = new List<QueryableObject>(onlineDatabases.Count);
			foreach (OnlineDatabase onlineDatabase in onlineDatabases)
			{
				QueryableOnlineDatabase queryableOnlineDatabase = new QueryableOnlineDatabase();
				onlineDatabase.ExportToQueryableObject(queryableOnlineDatabase);
				if (filter == null || OpathFilterEvaluator.FilterMatches(filter, queryableOnlineDatabase))
				{
					list.Add(queryableOnlineDatabase);
				}
			}
			return list;
		}

		private List<QueryableObject> QueryMailboxDispatcher(object[] parameters, QueryFilter filter)
		{
			List<QueryableObject> list = new List<QueryableObject>();
			IList<OnlineDatabase> onlineDatabases = OnlineDiagnostics.Instance.databaseManager.GetOnlineDatabases((Guid?)parameters[0]);
			if (onlineDatabases == null || onlineDatabases.Count != 1)
			{
				throw new ArgumentException("Could not find the database specified by DatabaseGuid", "DatabaseGuid");
			}
			IList<MailboxDispatcher> mailboxDispatcher = ((EventControllerPrivate)onlineDatabases[0].EventController).GetMailboxDispatcher((Guid?)parameters[1]);
			foreach (MailboxDispatcher mailboxDispatcher2 in mailboxDispatcher)
			{
				QueryableMailboxDispatcher queryableMailboxDispatcher = new QueryableMailboxDispatcher();
				mailboxDispatcher2.ExportToQueryableObject(queryableMailboxDispatcher);
				if (filter == null || OpathFilterEvaluator.FilterMatches(filter, queryableMailboxDispatcher))
				{
					list.Add(queryableMailboxDispatcher);
				}
			}
			return list;
		}

		private List<QueryableObject> QueryEventController(object[] parameters, QueryFilter filter)
		{
			IList<OnlineDatabase> onlineDatabases = OnlineDiagnostics.Instance.databaseManager.GetOnlineDatabases((Guid?)parameters[0]);
			List<QueryableObject> list = new List<QueryableObject>(onlineDatabases.Count);
			foreach (OnlineDatabase onlineDatabase in onlineDatabases)
			{
				if (onlineDatabase != null && onlineDatabase.EventController != null)
				{
					QueryableEventController queryableEventController = new QueryableEventController();
					onlineDatabase.EventController.ExportToQueryableObject(queryableEventController);
					if (filter == null || OpathFilterEvaluator.FilterMatches(filter, queryableEventController))
					{
						list.Add(queryableEventController);
					}
				}
			}
			return list;
		}

		private List<QueryableObject> QueryEventDispatcher(object[] parameters, QueryFilter filter)
		{
			List<QueryableObject> list = new List<QueryableObject>(50);
			IList<OnlineDatabase> onlineDatabases = OnlineDiagnostics.Instance.databaseManager.GetOnlineDatabases((Guid?)parameters[0]);
			if (onlineDatabases == null || onlineDatabases.Count != 1)
			{
				throw new ArgumentException("Could not find the database specified by DatabaseGuid", "DatabaseGuid");
			}
			EventControllerPrivate eventControllerPrivate = (EventControllerPrivate)onlineDatabases[0].EventController;
			IList<MailboxDispatcher> mailboxDispatcher = eventControllerPrivate.GetMailboxDispatcher((Guid?)parameters[1]);
			if (mailboxDispatcher == null)
			{
				return list;
			}
			MailboxDispatcher mailboxDispatcher2 = mailboxDispatcher[0];
			foreach (EventDispatcherPrivate eventDispatcherPrivate in mailboxDispatcher2.GetEventDispatcher((Guid?)parameters[2]))
			{
				QueryableEventDispatcher queryableEventDispatcher = new QueryableEventDispatcher();
				eventDispatcherPrivate.ExportToQueryableObject(queryableEventDispatcher);
				if (filter == null || OpathFilterEvaluator.FilterMatches(filter, queryableEventDispatcher))
				{
					list.Add(queryableEventDispatcher);
				}
			}
			return list;
		}

		private bool ExtractIdentityPropertyFromFilter<T>(SimpleProviderPropertyDefinition property, ref QueryFilter filter, out T propertyValue)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			AndFilter andFilter = filter as AndFilter;
			Func<QueryFilter, bool> func = delegate(QueryFilter f)
			{
				ComparisonFilter comparisonFilter2 = f as ComparisonFilter;
				return comparisonFilter2 != null && comparisonFilter2.Property == property && comparisonFilter2.ComparisonOperator == ComparisonOperator.Equal;
			};
			if (func(comparisonFilter))
			{
				propertyValue = (T)((object)comparisonFilter.PropertyValue);
				filter = null;
				return true;
			}
			if (andFilter != null)
			{
				comparisonFilter = (ComparisonFilter)andFilter.Filters.FirstOrDefault(func);
				if (comparisonFilter != null)
				{
					propertyValue = (T)((object)comparisonFilter.PropertyValue);
					filter = QueryFilter.AndTogether(andFilter.Filters.SkipWhile(func).ToArray<QueryFilter>());
					filter = QueryFilter.SimplifyFilter(filter);
					return true;
				}
			}
			propertyValue = default(T);
			return false;
		}

		private const string DiagnosticsComponentName = "EBA";

		private static OnlineDiagnostics instance = new OnlineDiagnostics();

		private Dictionary<string, QueryTemplate> dispatcher;

		private DatabaseManager databaseManager;

		private bool registered;
	}
}
