using System;
using System.Net;
using System.Net.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.Export;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Task;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService
{
	internal static class Controller
	{
		public static SearchMailboxesResults SeachMailboxes(ISearchPolicy policy, SearchMailboxesInputs input)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, new object[]
			{
				"Controller.SeachMailboxes Input:",
				input,
				"IsLocal:",
				input.IsLocalCall,
				"SearchType:",
				input.SearchType
			});
			Recorder.Record record = policy.Recorder.Start("SearchMailboxes", TraceType.InfoTrace, true);
			ValidateSource.ValidateSourceContext taskContext = new ValidateSource.ValidateSourceContext
			{
				AllowedRecipientTypeDetails = SearchRecipient.RecipientTypeDetail,
				AllowedRecipientTypes = SearchRecipient.RecipientTypes,
				MinimumVersion = null,
				RequiredCmdlet = "New-MailboxSearch",
				RequiredCmdletParameters = "EstimateOnly"
			};
			MailboxInfoCreation.MailboxInfoCreationContext taskContext2 = new MailboxInfoCreation.MailboxInfoCreationContext
			{
				SuppressDuplicates = true,
				MaximumItems = (int)((input != null && input.SearchType == SearchType.Statistics) ? policy.ThrottlingSettings.DiscoveryMaxStatsSearchMailboxes : ((uint)policy.ExecutionSettings.DiscoveryMaxMailboxes))
			};
			ServerLookup.ServerLookupContext taskContext3 = new ServerLookup.ServerLookupContext();
			CompleteSearchMailbox.CompleteSearchMailboxContext taskContext4 = new CompleteSearchMailbox.CompleteSearchMailboxContext();
			Func<object, string> batchByDatabase = delegate(object item)
			{
				if (!((SearchSource)item).MailboxInfo.IsArchive)
				{
					return ((SearchSource)item).MailboxInfo.MdbGuid.ToString();
				}
				return ((SearchSource)item).MailboxInfo.ArchiveDatabase.ToString();
			};
			Func<object, string> batchKeyFactory = delegate(object item)
			{
				if (!((SearchSource)item).MailboxInfo.IsRemoteMailbox)
				{
					return batchByDatabase(item);
				}
				return "Remote";
			};
			Func<object, string> batchKeyFactory2 = (object item) => ((FanoutParameters)item).GroupId.Uri.ToString();
			Executor executor = new Executor(policy, typeof(InitializeSearchMailbox))
			{
				Concurrency = policy.ExecutionSettings.DiscoverySynchronousConcurrency
			};
			Executor executor2 = executor;
			if (input.IsLocalCall)
			{
				Recorder.Trace(2L, TraceType.InfoTrace, "Controller.SeachMailboxes LocalSearch");
				executor2 = executor2.Chain(new BatchedExecutor(policy, typeof(SearchDatabase))
				{
					Concurrency = policy.ExecutionSettings.DiscoveryLocalSearchConcurrency,
					BatchSize = (policy.ExecutionSettings.DiscoveryLocalSearchIsParallel ? 1U : policy.ExecutionSettings.DiscoveryLocalSearchBatch),
					BatchKeyFactory = batchByDatabase
				});
			}
			else
			{
				Recorder.Trace(2L, TraceType.InfoTrace, "Controller.SeachMailboxes Search");
				executor2 = executor2.Chain(new Executor(policy, typeof(DirectoryQueryFormatting))
				{
					Concurrency = policy.ExecutionSettings.DiscoverySynchronousConcurrency
				}).Chain(new Executor(policy, typeof(DirectoryLookup))
				{
					Concurrency = policy.ExecutionSettings.DiscoveryADLookupConcurrency
				}).Chain(new Executor(policy, typeof(ValidateSource))
				{
					Concurrency = policy.ExecutionSettings.DiscoverySynchronousConcurrency,
					TaskContext = taskContext
				}).Chain(new Executor(policy, typeof(MailboxInfoCreation))
				{
					Concurrency = policy.ExecutionSettings.DiscoverySynchronousConcurrency,
					TaskContext = taskContext2
				});
				if (input.SearchType == SearchType.ExpandSources)
				{
					Recorder.Trace(2L, TraceType.InfoTrace, "Controller.SeachMailboxes ExpandSources");
					executor2 = executor2.Chain(new BatchedExecutor(policy, typeof(CompleteSourceLookup))
					{
						Concurrency = policy.ExecutionSettings.DiscoverySynchronousConcurrency,
						BatchKeyFactory = BatchedExecutor.BatchByCount,
						BatchSize = policy.ThrottlingSettings.DiscoveryMaxPreviewSearchMailboxes,
						TaskContext = input
					});
				}
				else
				{
					executor2 = executor2.Chain(new BatchedExecutor(policy, typeof(ServerLookup))
					{
						Concurrency = policy.ExecutionSettings.DiscoveryServerLookupConcurrency,
						TaskContext = taskContext3,
						BatchSize = policy.ExecutionSettings.DiscoveryServerLookupBatch,
						BatchKeyFactory = batchKeyFactory
					}).Chain(new BatchedExecutor(policy, typeof(FanoutSearchMailboxes))
					{
						Concurrency = policy.ExecutionSettings.DiscoveryFanoutConcurrency,
						BatchSize = policy.ExecutionSettings.DiscoveryFanoutBatch,
						BatchKeyFactory = batchKeyFactory2
					});
				}
			}
			executor2 = executor2.Chain(new Executor(policy, typeof(CompleteSearchMailbox))
			{
				Concurrency = 1U,
				TaskContext = taskContext4
			});
			Recorder.Trace(2L, TraceType.InfoTrace, "Controller.SeachMailboxes Start");
			SearchMailboxesResults result;
			try
			{
				ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(ServerToServerEwsCallingContext.CertificateErrorHandler));
				SearchMailboxesResults searchMailboxesResults = executor.Process(input) as SearchMailboxesResults;
				Recorder.Trace(2L, TraceType.InfoTrace, "Controller.SeachMailboxes End");
				if (executor.Context.Failures.Count > 0)
				{
					Recorder.Trace(2L, TraceType.InfoTrace, "Controller.SeachMailboxes Failures:", executor.Context.Failures.Count);
					if (searchMailboxesResults == null)
					{
						searchMailboxesResults = new SearchMailboxesResults(null);
					}
					searchMailboxesResults.AddFailures(executor.Context.Failures);
				}
				policy.Recorder.End(record);
				result = searchMailboxesResults;
			}
			finally
			{
				ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Remove(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(ServerToServerEwsCallingContext.CertificateErrorHandler));
			}
			return result;
		}

		public static GetSearchableMailboxesResults GetSearchableMailboxes(ISearchPolicy policy, GetSearchableMailboxesInputs input)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "Controller.GetSearchableMailboxes Input:", input);
			Recorder.Record record = policy.Recorder.Start("GetSearchableMailboxes", TraceType.InfoTrace, true);
			Executor executor = new Executor(policy, typeof(InitializeGetSearchablebleMailbox))
			{
				Concurrency = policy.ExecutionSettings.DiscoverySynchronousConcurrency
			};
			Executor executor2 = executor;
			executor2 = executor2.Chain(new Executor(policy, typeof(DirectoryQueryFormatting))
			{
				Concurrency = policy.ExecutionSettings.DiscoverySynchronousConcurrency
			}).Chain(new Executor(policy, typeof(DirectoryLookup))
			{
				Concurrency = policy.ExecutionSettings.DiscoverySynchronousConcurrency
			}).Chain(new BatchedExecutor(policy, typeof(CompleteGetSearchableMailbox))
			{
				Concurrency = policy.ExecutionSettings.DiscoverySynchronousConcurrency,
				BatchSize = policy.ExecutionSettings.DiscoveryMaxAllowedExecutorItems,
				BatchKeyFactory = BatchedExecutor.BatchByCount
			});
			Recorder.Trace(2L, TraceType.InfoTrace, "Controller.GetSearchableMailboxes Start");
			GetSearchableMailboxesResults result = executor.Process(input) as GetSearchableMailboxesResults;
			policy.Recorder.End(record);
			return result;
		}
	}
}
