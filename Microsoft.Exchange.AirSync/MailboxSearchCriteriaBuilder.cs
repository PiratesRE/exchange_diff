using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class MailboxSearchCriteriaBuilder
	{
		public MailboxSearchCriteriaBuilder(CultureInfo cultureInfo)
		{
			this.ExcludedFolders = new HashSet<StoreObjectId>();
			this.cultureInfo = cultureInfo;
		}

		public List<string> FolderScope
		{
			get
			{
				return this.folderScope;
			}
		}

		public Dictionary<string, MailboxSearchCriteriaBuilder.SchemaCacheItem> SchemaCache
		{
			get
			{
				return this.schemaCache;
			}
		}

		public HashSet<StoreObjectId> ExcludedFolders { get; private set; }

		public Conversation Conversation { get; private set; }

		public QueryFilter ParseTopLevelClassAndFolders(XmlNode queryNode, bool contentIndexingEnabled, IAirSyncVersionFactory versionFactory, IAirSyncContext context)
		{
			this.Clear();
			if (queryNode.ChildNodes.Count != 1)
			{
				throw new SearchFilterTooComplexException
				{
					ErrorStringForProtocolLogger = "SearchTooComplexError1"
				};
			}
			XmlNode xmlNode = queryNode.ChildNodes[0];
			if ("Search:" != xmlNode.NamespaceURI || "And" != xmlNode.Name)
			{
				throw new SearchFilterTooComplexException
				{
					ErrorStringForProtocolLogger = "SearchTooComplexError1"
				};
			}
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<XmlNode> list3 = new List<XmlNode>();
			this.contentIndexingEnabled = contentIndexingEnabled;
			foreach (object obj in xmlNode.ChildNodes)
			{
				XmlNode xmlNode2 = (XmlNode)obj;
				if ("AirSync:" == xmlNode2.NamespaceURI && "CollectionId" == xmlNode2.Name)
				{
					if (xmlNode2.ChildNodes.Count != 1 || xmlNode2.FirstChild.NodeType != XmlNodeType.Text)
					{
						throw new SearchProtocolErrorException
						{
							ErrorStringForProtocolLogger = "SearchProtocolError1"
						};
					}
					list2.Add(xmlNode2.InnerText);
				}
				else if ("AirSync:" == xmlNode2.NamespaceURI && "Class" == xmlNode2.Name)
				{
					if (xmlNode2.ChildNodes.Count != 1 || xmlNode2.FirstChild.NodeType != XmlNodeType.Text)
					{
						throw new SearchProtocolErrorException
						{
							ErrorStringForProtocolLogger = "SearchProtocolError2"
						};
					}
					list.Add(xmlNode2.InnerText);
				}
				else
				{
					list3.Add(xmlNode2);
				}
			}
			if (list3.Count < 1)
			{
				throw new SearchProtocolErrorException
				{
					ErrorStringForProtocolLogger = "SearchProtocolError3"
				};
			}
			this.folderScope = list2;
			this.airSyncClasses = list;
			this.schemaCache = new Dictionary<string, MailboxSearchCriteriaBuilder.SchemaCacheItem>();
			List<QueryFilter> list4 = new List<QueryFilter>();
			new List<QueryFilter>();
			if (list.Count == 0)
			{
				list.Add("Email");
				list.Add("Calendar");
				list.Add("Contacts");
				list.Add("Tasks");
				if (context.Request.Version >= 140)
				{
					list.Add("Notes");
					list.Add("SMS");
				}
			}
			foreach (string text in list)
			{
				string key;
				switch (key = text)
				{
				case "Email":
					this.schemaCache["Email"] = new MailboxSearchCriteriaBuilder.SchemaCacheItem(versionFactory.CreateEmailSchema(null), versionFactory.CreateMissingPropertyStrategy(null));
					continue;
				case "Calendar":
					this.schemaCache["Calendar"] = new MailboxSearchCriteriaBuilder.SchemaCacheItem(versionFactory.CreateCalendarSchema(), versionFactory.CreateMissingPropertyStrategy(null));
					continue;
				case "Contacts":
					this.schemaCache["Contacts"] = new MailboxSearchCriteriaBuilder.SchemaCacheItem(versionFactory.CreateContactsSchema(), versionFactory.CreateMissingPropertyStrategy(null));
					continue;
				case "Tasks":
					this.schemaCache["Tasks"] = new MailboxSearchCriteriaBuilder.SchemaCacheItem(versionFactory.CreateTasksSchema(), versionFactory.CreateMissingPropertyStrategy(null));
					continue;
				case "Notes":
					if (context.Request.Version < 140)
					{
						throw new SearchProtocolErrorException
						{
							ErrorStringForProtocolLogger = "SearchProtocolError4"
						};
					}
					this.schemaCache["Notes"] = new MailboxSearchCriteriaBuilder.SchemaCacheItem(versionFactory.CreateNotesSchema(), versionFactory.CreateMissingPropertyStrategy(null));
					continue;
				case "SMS":
					if (context.Request.Version < 140)
					{
						throw new SearchProtocolErrorException
						{
							ErrorStringForProtocolLogger = "SearchProtocolError5"
						};
					}
					this.schemaCache["SMS"] = new MailboxSearchCriteriaBuilder.SchemaCacheItem(versionFactory.CreateSmsSchema(), versionFactory.CreateMissingPropertyStrategy(null));
					continue;
				}
				throw new SearchProtocolErrorException
				{
					ErrorStringForProtocolLogger = "SearchProtocolError6"
				};
			}
			foreach (XmlNode node in list3)
			{
				QueryFilter queryFilter = this.ParseSearchNamespace(node);
				if (queryFilter == null)
				{
					return null;
				}
				list4.Add(queryFilter);
			}
			return MailboxSearchCriteriaBuilder.ConstructAndOrOperator(list4, "And");
		}

		public bool DoesMatchCriteria(StoreObjectId parentFolderId, StoreObjectId storeObjectId)
		{
			IConversationTreeNode conversationTreeNode;
			return !this.ExcludedFolders.Contains(parentFolderId) && (this.Conversation == null || this.Conversation.ConversationTree.TryGetConversationTreeNode(storeObjectId, out conversationTreeNode));
		}

		private static QueryFilter ConstructAndOrOperator(List<QueryFilter> childFilters, string op)
		{
			if (op != null)
			{
				QueryFilter result;
				if (!(op == "And"))
				{
					if (!(op == "Or"))
					{
						goto IL_3F;
					}
					result = new OrFilter(childFilters.ToArray());
				}
				else
				{
					result = new AndFilter(childFilters.ToArray());
				}
				return result;
			}
			IL_3F:
			throw new InvalidOperationException("Unexpected bad operator: " + op);
		}

		private static QueryFilter ParseLtGtOperator(XmlNode parentNode)
		{
			if (2 != parentNode.ChildNodes.Count)
			{
				throw new SearchProtocolErrorException
				{
					ErrorStringForProtocolLogger = "SearchProtocolError7"
				};
			}
			XmlNode xmlNode = parentNode.ChildNodes[0];
			if ("Email:" != xmlNode.NamespaceURI || "DateReceived" != xmlNode.Name)
			{
				throw new SearchFilterTooComplexException
				{
					ErrorStringForProtocolLogger = "SearchTooComplexError3"
				};
			}
			xmlNode = parentNode.ChildNodes[1];
			if ("Search:" != xmlNode.NamespaceURI || "Value" != xmlNode.Name)
			{
				throw new SearchProtocolErrorException
				{
					ErrorStringForProtocolLogger = "SearchProtocolError8"
				};
			}
			if (xmlNode.ChildNodes.Count > 1)
			{
				throw new SearchProtocolErrorException
				{
					ErrorStringForProtocolLogger = "SearchProtocolError9"
				};
			}
			AirSyncUtcDateTimeProperty airSyncUtcDateTimeProperty = new AirSyncUtcDateTimeProperty("Search:", "Value", AirSyncDateFormat.Punctuate, false);
			airSyncUtcDateTimeProperty.Bind(xmlNode);
			ExDateTime dateTime;
			try
			{
				dateTime = airSyncUtcDateTimeProperty.DateTime;
			}
			catch (AirSyncPermanentException ex)
			{
				throw new SearchProtocolErrorException
				{
					ErrorStringForProtocolLogger = "SearchProtocolError10:" + ex.ErrorStringForProtocolLogger
				};
			}
			string name;
			if ((name = parentNode.Name) != null)
			{
				QueryFilter result;
				if (!(name == "LessThan"))
				{
					if (!(name == "GreaterThan"))
					{
						goto IL_16F;
					}
					result = new ComparisonFilter(ComparisonOperator.GreaterThan, ItemSchema.ReceivedTime, dateTime);
				}
				else
				{
					result = new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.ReceivedTime, dateTime);
				}
				return result;
			}
			IL_16F:
			throw new InvalidOperationException("Unexpected operator in Search request: " + parentNode.Name);
		}

		private void Clear()
		{
			this.schemaCache = null;
			this.airSyncClasses = null;
			this.folderScope = null;
			this.contentIndexingEnabled = false;
		}

		private QueryFilter ParseFreeText(XmlNode node)
		{
			SearchFilterGenerator searchFilterGenerator = new SearchFilterGenerator();
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			foreach (string text in this.airSyncClasses)
			{
				string a;
				if ((a = text) != null)
				{
					if (!(a == "Tasks"))
					{
						if (!(a == "Email") && !(a == "SMS"))
						{
							if (!(a == "Calendar"))
							{
								if (!(a == "Contacts"))
								{
									if (a == "Notes")
									{
										dictionary["IPF.StickyNote"] = false;
									}
								}
								else
								{
									dictionary["IPF.Contact"] = false;
								}
							}
							else
							{
								dictionary["IPF.Appointment"] = false;
							}
						}
						else
						{
							dictionary["IPF.Note"] = false;
						}
					}
					else
					{
						dictionary["IPF.Task"] = false;
					}
				}
			}
			return searchFilterGenerator.Execute(node.InnerText, this.contentIndexingEnabled, dictionary, SearchScope.AllFoldersAndItems, this.cultureInfo);
		}

		private QueryFilter ParseConversationId(XmlNode node)
		{
			if (this.Conversation != null)
			{
				throw new SearchFilterTooComplexException
				{
					ErrorStringForProtocolLogger = "SearchTooComplexError4"
				};
			}
			AirSyncByteArrayProperty airSyncByteArrayProperty = new AirSyncByteArrayProperty("Search:", "ConversationId", false);
			airSyncByteArrayProperty.Bind(node);
			byte[] byteArrayData = airSyncByteArrayProperty.ByteArrayData;
			if (byteArrayData == null)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
				{
					ErrorStringForProtocolLogger = "BadConversationIdInSearch"
				};
			}
			ConversationId conversationId;
			try
			{
				conversationId = ConversationId.Create(byteArrayData);
			}
			catch (CorruptDataException innerException)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, innerException, false)
				{
					ErrorStringForProtocolLogger = "BadConversationIdInSearch2"
				};
			}
			Conversation conversation;
			Command.CurrentCommand.GetOrCreateConversation(conversationId, false, out conversation);
			this.Conversation = conversation;
			return new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.ConversationTopic, this.Conversation.Topic);
		}

		private QueryFilter ParseContains(XmlNode parentNode)
		{
			if (parentNode.ChildNodes.Count != 2)
			{
				throw new SearchProtocolErrorException
				{
					ErrorStringForProtocolLogger = "SearchProtocolError7"
				};
			}
			XmlNode xmlNode = parentNode.ChildNodes[0];
			XmlNode xmlNode2 = parentNode.ChildNodes[1];
			if ("Search:" != xmlNode2.NamespaceURI || "Value" != xmlNode2.Name)
			{
				throw new SearchProtocolErrorException
				{
					ErrorStringForProtocolLogger = "SearchProtocolError8"
				};
			}
			string innerText = xmlNode2.InnerText;
			string name;
			if ((name = xmlNode.Name) != null)
			{
				QueryFilter result;
				if (!(name == "From"))
				{
					if (!(name == "CategoryId"))
					{
						goto IL_FE;
					}
					int num;
					if (!int.TryParse(innerText, NumberStyles.None, CultureInfo.InvariantCulture, out num) || num % 2 != 1)
					{
						throw new SearchProtocolErrorException
						{
							ErrorStringForProtocolLogger = "SearchProtocolError11"
						};
					}
					string text = AirSyncUtility.ConvertSytemCategoryIdToKeywordsFormat(num);
					result = new TextFilter(ItemSchema.Categories, text, MatchOptions.SubString, MatchFlags.Default);
				}
				else
				{
					result = new TextFilter(ItemSchema.SentRepresentingEmailAddress, innerText, MatchOptions.SubString, MatchFlags.Loose);
				}
				return result;
			}
			IL_FE:
			throw new SearchProtocolErrorException
			{
				ErrorStringForProtocolLogger = "BadNode(" + xmlNode.Name + ")InSearch"
			};
		}

		private QueryFilter ParseDoesNotContain(XmlNode parentNode)
		{
			return new NotFilter(this.ParseContains(parentNode));
		}

		private QueryFilter ParseOrOp(XmlNode parentNode)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			foreach (object obj in parentNode.ChildNodes)
			{
				XmlNode node = (XmlNode)obj;
				QueryFilter item = this.ParseSearchNamespace(node);
				list.Add(item);
			}
			return MailboxSearchCriteriaBuilder.ConstructAndOrOperator(list, "Or");
		}

		private QueryFilter ParseSearchNamespace(XmlNode node)
		{
			if ("Search:" != node.NamespaceURI)
			{
				throw new SearchProtocolErrorException
				{
					ErrorStringForProtocolLogger = "SearchProtocolError11"
				};
			}
			string name;
			switch (name = node.Name)
			{
			case "FreeText":
				return this.ParseFreeText(node);
			case "LessThan":
			case "GreaterThan":
				return MailboxSearchCriteriaBuilder.ParseLtGtOperator(node);
			case "ConversationId":
				return this.ParseConversationId(node);
			case "Contains":
				return this.ParseContains(node);
			case "DoesNotContain":
				return this.ParseDoesNotContain(node);
			case "Or":
				return this.ParseOrOp(node);
			}
			throw new SearchProtocolErrorException
			{
				ErrorStringForProtocolLogger = "BadNode(" + node.Name + ")InSearch"
			};
		}

		private const string BadConversationIdInSearch = "BadConversationIdInSearch";

		private const string BadConversationIdInSearch2 = "BadConversationIdInSearch2";

		private const string SearchProtocolError1 = "SearchProtocolError1";

		private const string SearchProtocolError2 = "SearchProtocolError2";

		private const string SearchProtocolError3 = "SearchProtocolError3";

		private const string SearchProtocolError4 = "SearchProtocolError4";

		private const string SearchProtocolError5 = "SearchProtocolError5";

		private const string SearchProtocolError6 = "SearchProtocolError6";

		private const string SearchProtocolError7 = "SearchProtocolError7";

		private const string SearchProtocolError8 = "SearchProtocolError8";

		private const string SearchProtocolError9 = "SearchProtocolError9";

		private const string SearchProtocolError10 = "SearchProtocolError10";

		private const string SearchProtocolError11 = "SearchProtocolError11";

		private const string SearchProtocolError12 = "SearchProtocolError12";

		private const string SearchTooComplexError1 = "SearchTooComplexError1";

		private const string SearchTooComplexError3 = "SearchTooComplexError3";

		private const string SearchTooComplexError4 = "SearchTooComplexError4";

		private List<string> airSyncClasses;

		private bool contentIndexingEnabled;

		private List<string> folderScope;

		private Dictionary<string, MailboxSearchCriteriaBuilder.SchemaCacheItem> schemaCache;

		private CultureInfo cultureInfo;

		internal class SchemaCacheItem
		{
			public SchemaCacheItem(AirSyncSchemaState schemaState, IAirSyncMissingPropertyStrategy missingPropertyStrategy)
			{
				this.schemaState = schemaState;
				AirSyncEntitySchemaState airSyncEntitySchemaState = this.schemaState as AirSyncEntitySchemaState;
				AirSyncXsoSchemaState airSyncXsoSchemaState = this.schemaState as AirSyncXsoSchemaState;
				if (airSyncEntitySchemaState != null)
				{
					this.mailboxDataObject = airSyncEntitySchemaState.GetEntityDataObject();
				}
				else
				{
					if (airSyncXsoSchemaState == null)
					{
						throw new ApplicationException(string.Format("SchemaState of type {0} is not supported", schemaState.GetType().FullName));
					}
					this.mailboxDataObject = airSyncXsoSchemaState.GetXsoDataObject();
				}
				this.airSyncDataObject = this.schemaState.GetAirSyncDataObject(new Hashtable(), missingPropertyStrategy);
			}

			public AirSyncDataObject AirSyncDataObject
			{
				get
				{
					return this.airSyncDataObject;
				}
			}

			public IServerDataObject MailboxDataObject
			{
				get
				{
					return this.mailboxDataObject;
				}
			}

			public AirSyncSchemaState SchemaState
			{
				get
				{
					return this.schemaState;
				}
			}

			private AirSyncDataObject airSyncDataObject;

			private IServerDataObject mailboxDataObject;

			private AirSyncSchemaState schemaState;
		}
	}
}
