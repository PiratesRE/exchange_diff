using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Ceres.CoreServices.Services.Package;
using Microsoft.Ceres.NlpBase.Dictionaries;
using Microsoft.Ceres.NlpBase.DictionaryInterface;
using Microsoft.Ceres.NlpBase.DictionaryInterface.Resource;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Query
{
	internal class Completions
	{
		public Completions(ISearchServiceConfig config)
		{
			this.config = config;
		}

		public QueryCompletion TopNCompletions { get; set; }

		public bool ReloadTopN { get; private set; }

		internal int MaximumSuggestionsCount
		{
			get
			{
				return this.maximumSuggestionsCount;
			}
			set
			{
				this.maximumSuggestionsCount = value;
			}
		}

		internal Completions.CompletionsDictionaryState TopNState { get; set; }

		private static TopNManagementClient CompilationClient
		{
			get
			{
				if (Completions.compilationClient == null)
				{
					lock (Completions.StaticConstructionLockObject)
					{
						if (Completions.compilationClient == null)
						{
							Completions.compilationClient = new TopNManagementClient(SearchConfig.Instance);
						}
					}
				}
				return Completions.compilationClient;
			}
		}

		private QueryCompletion Spelling
		{
			get
			{
				if (Completions.spellingCompletions == null)
				{
					lock (Completions.StaticConstructionLockObject)
					{
						Thread.MemoryBarrier();
						if (Completions.spellingCompletions == null)
						{
							Completions.spellingCompletions = this.GetAssemblyBasedCompletionDictionary(this.config.SpellingMaximumEditDistance, this.config.SpellingMinimalSimilarity, this.config.SpellingExactPrefixLength, "Microsoft.System_Dictionaries_Spellcheck");
						}
					}
				}
				return Completions.spellingCompletions;
			}
		}

		private QueryCompletion Synonyms
		{
			get
			{
				if (Completions.synonymsCompletions == null)
				{
					lock (Completions.StaticConstructionLockObject)
					{
						Thread.MemoryBarrier();
						if (Completions.synonymsCompletions == null)
						{
							Completions.synonymsCompletions = this.GetAssemblyBasedCompletionDictionary(this.config.SynonymMaximumEditDistance, this.config.SynonymMinimalSimilarity, this.config.SynonymExactPrefixLength, "microsoft.synonymsdictionary");
						}
					}
				}
				return Completions.synonymsCompletions;
			}
		}

		private QueryCompletion Nicknames
		{
			get
			{
				if (Completions.nicknamesCompletions == null)
				{
					lock (Completions.StaticConstructionLockObject)
					{
						Thread.MemoryBarrier();
						if (Completions.nicknamesCompletions == null)
						{
							Completions.nicknamesCompletions = this.GetAssemblyBasedCompletionDictionary(this.config.NicknameMaximumEditDistance, this.config.NicknameMinimalSimilarity, this.config.NicknameExactPrefixLength, "microsoft.nicknamesdictionary");
						}
					}
				}
				return Completions.nicknamesCompletions;
			}
		}

		public void InitializeTopN(MailboxSession mailboxSession)
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			this.ReloadTopN = false;
			this.TopNState = Completions.CompletionsDictionaryState.Unknown;
			try
			{
				if (this.config.TopNEnabled)
				{
					using (UserConfiguration searchDictionaryItem = SearchDictionary.GetSearchDictionaryItem(mailboxSession, "Search.TopN"))
					{
						using (Stream stream = searchDictionaryItem.GetStream())
						{
							lock (this.topNlockObject)
							{
								using (Stream stream2 = SearchDictionary.InitializeFrom(stream, 1))
								{
									if (stream2 != null)
									{
										long length = stream2.Length;
										this.TopNCompletions = this.GetStreamBasedCompletionDictionary(this.config.TopNMaximumEditDistance, this.config.TopNMinimalSimilarity, this.config.TopNExactPrefixLength, "Search.TopN", stream2);
										if (ExDateTime.UtcNow.Subtract(searchDictionaryItem.LastModifiedTime) > this.config.TopNDictionaryAgeThreshold)
										{
											this.TopNState = Completions.CompletionsDictionaryState.Stale;
										}
										else
										{
											this.TopNState = Completions.CompletionsDictionaryState.Initialized;
										}
										list.Add(new KeyValuePair<string, object>("LastModified Time", searchDictionaryItem.LastModifiedTime));
										list.Add(new KeyValuePair<string, object>("Dictionary Size", length));
										list.Add(new KeyValuePair<string, object>("Max Edit Distance", this.config.TopNMaximumEditDistance));
										list.Add(new KeyValuePair<string, object>("Minimal Similarity", this.config.TopNMinimalSimilarity));
										list.Add(new KeyValuePair<string, object>("Exact Prefix Length", this.config.TopNExactPrefixLength));
									}
								}
							}
						}
					}
					if (this.TopNState == Completions.CompletionsDictionaryState.Stale || this.TopNState == Completions.CompletionsDictionaryState.Unknown)
					{
						list.Add(new KeyValuePair<string, object>("Compilation", "Requested"));
						Completions.CompilationClient.BeginExecuteFlow(mailboxSession.MdbGuid, mailboxSession.MailboxGuid, null, new AsyncCallback(this.TopNDictionaryCompilationCallback));
					}
				}
				else
				{
					this.TopNState = Completions.CompletionsDictionaryState.Disabled;
				}
			}
			catch (OutOfMemoryException)
			{
				throw;
			}
			catch (StackOverflowException)
			{
				throw;
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (Exception value)
			{
				list.Add(new KeyValuePair<string, object>("Failure", value));
				this.TopNState = Completions.CompletionsDictionaryState.Failed;
			}
			list.Add(new KeyValuePair<string, object>("Initialization state", this.TopNState));
			Interlocked.Exchange<List<KeyValuePair<string, object>>>(ref this.topNInitializationStatistics, list);
		}

		public List<KeyValuePair<string, object>> ReadAndResetTopNInitializationStatistics()
		{
			return Interlocked.Exchange<List<KeyValuePair<string, object>>>(ref this.topNInitializationStatistics, null);
		}

		public IDictionary<string, QuerySuggestion> GetSuggestions(string query, QuerySuggestionSources sources, string languageIdentifier)
		{
			Dictionary<string, QuerySuggestion> dictionary = new Dictionary<string, QuerySuggestion>();
			this.AddSuggestions(dictionary, query, sources, languageIdentifier);
			return dictionary;
		}

		public List<KeyValuePair<string, object>> AddSuggestions(IDictionary<string, QuerySuggestion> suggestions, string query, QuerySuggestionSources sources, string languageIdentifier)
		{
			double num = 0.8999;
			List<KeyValuePair<string, object>> list = null;
			if (this.config.TopNEnabled)
			{
				if (this.TopNState == Completions.CompletionsDictionaryState.Initialized || this.TopNState == Completions.CompletionsDictionaryState.Stale)
				{
					if ((sources & QuerySuggestionSources.TopN) != QuerySuggestionSources.None)
					{
						int count = suggestions.Count;
						this.AddSingleSourceSuggestions(suggestions, query, QuerySuggestionSources.TopN, languageIdentifier, ref num);
						if (list == null)
						{
							list = new List<KeyValuePair<string, object>>(1);
						}
						list.Add(new KeyValuePair<string, object>("TopNSuggestions", "Count: " + (suggestions.Count - count)));
					}
				}
				else
				{
					if (list == null)
					{
						list = new List<KeyValuePair<string, object>>(1);
					}
					list.Add(new KeyValuePair<string, object>("TopNSuggestions", "Skipped. Reason: " + this.TopNState));
				}
			}
			if (this.config.SpellingSuggestionsEnabled && (sources & QuerySuggestionSources.Spelling) != QuerySuggestionSources.None)
			{
				this.AddSingleSourceSuggestions(suggestions, query, QuerySuggestionSources.Spelling, languageIdentifier, ref num);
			}
			if (this.config.SynonymSuggestionsEnabled && (sources & QuerySuggestionSources.Synonyms) != QuerySuggestionSources.None)
			{
				this.AddSingleSourceSuggestions(suggestions, query, QuerySuggestionSources.Synonyms, languageIdentifier, ref num);
			}
			if (this.config.NicknameSuggestionsEnabled && (sources & QuerySuggestionSources.Nicknames) != QuerySuggestionSources.None)
			{
				this.AddSingleSourceSuggestions(suggestions, query, QuerySuggestionSources.Nicknames, languageIdentifier, ref num);
			}
			return list;
		}

		internal QueryCompletion GetStreamBasedCompletionDictionary(int maximumEditDistance, double minimalSimilarity, int exactPrefixLength, string dictionaryName, Stream stream)
		{
			this.InitializeStaticResourcesIfNeeded();
			ICompletion item = CompletionFactory.Instance.Get(1, dictionaryName, stream, maximumEditDistance, minimalSimilarity, exactPrefixLength, 180);
			return new QueryCompletion(new List<ICompletion>
			{
				item
			});
		}

		private void AddSingleSourceSuggestions(IDictionary<string, QuerySuggestion> suggestions, string query, QuerySuggestionSources source, string languageIdentifier, ref double rank)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"language",
					languageIdentifier
				}
			};
			string text = string.Empty;
			string text2 = string.Empty;
			IEnumerable<ICompletionResult> enumerable = null;
			char[] anyOf = Completions.DefaultExclusionChars;
			switch (source)
			{
			case QuerySuggestionSources.Spelling:
				enumerable = this.Spelling.GetCompletions(query, 0, dictionary, this.config.MaxCompletionTraversalCount, null, null, null);
				break;
			case QuerySuggestionSources.RecentSearches | QuerySuggestionSources.Spelling:
				break;
			case QuerySuggestionSources.Synonyms:
				text = "synonyms";
				enumerable = this.Synonyms.GetCompletions(query, 0, dictionary, this.config.MaxCompletionTraversalCount, null, null, null);
				break;
			default:
				if (source != QuerySuggestionSources.Nicknames)
				{
					if (source == QuerySuggestionSources.TopN)
					{
						if (this.config.FinalWordSuggestionsEnabled)
						{
							string text3 = query.Trim();
							int num = text3.LastIndexOf(' ') + 1;
							if (num > 0)
							{
								text2 = text3.Substring(0, num - 1);
								query = text3.Substring(num);
							}
						}
						anyOf = this.config.TopNExclusionCharacters;
						enumerable = this.TopNCompletions.GetCompletions(query, 0, dictionary, this.config.MaxCompletionTraversalCount, null, null, null);
					}
				}
				else
				{
					text = "nicknames";
					enumerable = this.Nicknames.GetCompletions(query, 0, dictionary, this.config.MaxCompletionTraversalCount, null, null, null);
				}
				break;
			}
			if (enumerable != null)
			{
				foreach (ICompletionResult completionResult in enumerable)
				{
					string text4 = completionResult.Query;
					if (text4.IndexOfAny(anyOf) == -1)
					{
						if (!string.IsNullOrEmpty(text2))
						{
							text4 = string.Format("{0} {1}", text2, text4);
						}
						if (!suggestions.Keys.Contains(text4))
						{
							QuerySuggestion value = new QuerySuggestion(text4, rank, source);
							suggestions.Add(text4, value);
							rank -= 0.0001;
						}
						foreach (IMatchResult matchResult in completionResult.Matches)
						{
							string text5;
							if (!string.IsNullOrEmpty(text))
							{
								text5 = matchResult.Attributes[text];
							}
							else
							{
								text5 = matchResult.MatchedItem;
							}
							if (!string.IsNullOrEmpty(text2))
							{
								text5 = string.Format("{0} {1}", text2, text5);
							}
							if (!suggestions.Keys.Contains(text5))
							{
								QuerySuggestion value2 = new QuerySuggestion(text5, rank, source);
								suggestions.Add(text5, value2);
								rank -= 0.0001;
								if (suggestions.Count >= this.MaximumSuggestionsCount)
								{
									break;
								}
							}
						}
						if (suggestions.Count >= this.MaximumSuggestionsCount)
						{
							break;
						}
					}
				}
			}
		}

		private QueryCompletion GetAssemblyBasedCompletionDictionary(int maximumEditDistance, double minimalSimilarity, int exactPrefixLength, string assemblyName)
		{
			this.InitializeStaticResourcesIfNeeded();
			AssemblyName assemblyName2 = new AssemblyName(assemblyName);
			ICompletion item = CompletionFactory.Instance.Get(1, assemblyName2, maximumEditDistance, minimalSimilarity, exactPrefixLength, 180);
			return new QueryCompletion(new List<ICompletion>
			{
				item
			});
		}

		private void InitializeStaticResourcesIfNeeded()
		{
			if (!Completions.staticResourcesInitialized)
			{
				lock (Completions.StaticConstructionLockObject)
				{
					Thread.MemoryBarrier();
					if (!Completions.staticResourcesInitialized)
					{
						string[] rootPath = new string[]
						{
							Path.Combine(ExchangeSetupContext.BinPath, "Search\\Ceres\\Resources\\Bundles")
						};
						IPackageManager packageManager = new DictionaryPackageManager(rootPath);
						NlpResourceManager.Instance.SetPackageManager(packageManager);
						Completions.staticResourcesInitialized = true;
					}
				}
			}
		}

		private void TopNDictionaryCompilationCallback(IAsyncResult asyncResult)
		{
			try
			{
				Completions.CompilationClient.EndExecuteFlow(asyncResult);
				LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (LazyAsyncResultWithTimeout)asyncResult;
				if (lazyAsyncResultWithTimeout.IsCanceled)
				{
					this.TopNState = Completions.CompletionsDictionaryState.InitializationCancelled;
				}
				else
				{
					this.ReloadTopN = true;
					this.TopNState = Completions.CompletionsDictionaryState.CompilationCompleted;
				}
			}
			catch (Exception)
			{
				this.TopNState = Completions.CompletionsDictionaryState.Failed;
			}
		}

		internal const double StartingSuggestionsRank = 0.8999;

		private const string NicknamesDictionaryAttribute = "nicknames";

		private const string SynonmsDictionaryAttribute = "synonyms";

		private const double SuggestionRankInterval = 0.0001;

		private static readonly char[] DefaultExclusionChars = new char[0];

		private static readonly object StaticConstructionLockObject = new object();

		private static QueryCompletion spellingCompletions;

		private static QueryCompletion synonymsCompletions;

		private static QueryCompletion nicknamesCompletions;

		private static bool staticResourcesInitialized;

		private static volatile TopNManagementClient compilationClient;

		private readonly object topNlockObject = new object();

		private readonly ISearchServiceConfig config;

		private int maximumSuggestionsCount = 100;

		private List<KeyValuePair<string, object>> topNInitializationStatistics;

		internal enum CompletionsDictionaryState
		{
			Unknown,
			Initialized,
			InitializationCancelled,
			CompilationCompleted,
			Failed,
			Stale,
			Disabled
		}
	}
}
