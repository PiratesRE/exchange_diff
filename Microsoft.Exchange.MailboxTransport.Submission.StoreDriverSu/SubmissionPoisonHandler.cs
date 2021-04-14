using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class SubmissionPoisonHandler : ITransportComponent
	{
		public SubmissionPoisonHandler(TimeSpan poisonEntryExpiryWindow, int maxPoisonEntries, QuarantineHandler quarantineHandler, ICrashRepository crashRepository, IStoreDriverTracer storeDriverTracer)
		{
			ArgumentValidator.ThrowIfZeroOrNegative("maxPoisonEntries", maxPoisonEntries);
			ArgumentValidator.ThrowIfNull("quarantineHandler", quarantineHandler);
			ArgumentValidator.ThrowIfNull("crashRepository", crashRepository);
			ArgumentValidator.ThrowIfNull("storeDriverTracer", storeDriverTracer);
			this.poisonEntryExpiryWindow = poisonEntryExpiryWindow;
			this.maxPoisonEntries = maxPoisonEntries;
			this.quarantineHandler = quarantineHandler;
			this.crashRepository = crashRepository;
			this.storeDriverTracer = storeDriverTracer;
		}

		public virtual int PoisonThreshold
		{
			get
			{
				return Components.Configuration.LocalServer.TransportServer.PoisonThreshold;
			}
		}

		public bool Loaded
		{
			get
			{
				return this.loaded;
			}
			protected set
			{
				this.loaded = value;
			}
		}

		public Dictionary<Guid, Dictionary<long, ResourceEventCounterCrashInfo>> SubmissionPoisonDataStore
		{
			get
			{
				return this.submissionPoisonDataStore;
			}
			protected set
			{
				this.submissionPoisonDataStore = value;
			}
		}

		public void Load()
		{
			try
			{
				foreach (Guid guid in this.crashRepository.GetAllResourceIDs())
				{
					Dictionary<long, ResourceEventCounterCrashInfo> value;
					SortedSet<DateTime> sortedSet;
					if (!this.crashRepository.GetResourceCrashInfoData(guid, this.poisonEntryExpiryWindow, out value, out sortedSet))
					{
						this.crashRepository.PurgeResourceData(guid);
					}
					else
					{
						this.submissionPoisonDataStore.Add(guid, value);
						if (sortedSet != null && sortedSet.Count > 0)
						{
							this.quarantineHandler.CheckAndQuarantine(guid, sortedSet);
						}
					}
				}
			}
			catch (CrashRepositoryAccessException ex)
			{
				throw new TransportComponentLoadFailedException(ex.ErrorDescription, ex);
			}
			this.loaded = true;
		}

		public void Unload()
		{
			this.resourceProtector.EnterWriteLock();
			try
			{
				if (this.loaded)
				{
					this.submissionPoisonDataStore.Clear();
					this.loaded = false;
				}
			}
			finally
			{
				this.resourceProtector.ExitWriteLock();
			}
		}

		public string OnUnhandledException(Exception e)
		{
			if (e != null)
			{
				return "Unhandled Exception encountered: " + e.ToString();
			}
			return "Unhandled Exception encountered";
		}

		public bool VerifyPoisonMessage(SubmissionPoisonContext submissionPoisonContext)
		{
			int num = 0;
			this.resourceProtector.EnterReadLock();
			try
			{
				if (!this.loaded)
				{
					return false;
				}
				ArgumentValidator.ThrowIfNull("submissionPoisonContext", submissionPoisonContext);
				ArgumentValidator.ThrowIfEmpty("submissionPoisonContext.ResourceGuid", submissionPoisonContext.ResourceGuid);
				if (this.submissionPoisonDataStore.ContainsKey(submissionPoisonContext.ResourceGuid) && this.submissionPoisonDataStore[submissionPoisonContext.ResourceGuid].ContainsKey(submissionPoisonContext.MapiEventCounter))
				{
					foreach (DateTime dateTime in this.submissionPoisonDataStore[submissionPoisonContext.ResourceGuid][submissionPoisonContext.MapiEventCounter].CrashTimes)
					{
						if (!StoreDriverUtils.CheckIfDateTimeExceedsThreshold(dateTime, DateTime.UtcNow, this.poisonEntryExpiryWindow))
						{
							num++;
						}
					}
					if (num >= this.PoisonThreshold)
					{
						return true;
					}
				}
			}
			finally
			{
				this.resourceProtector.ExitReadLock();
			}
			return false;
		}

		public bool VerifyPoisonNdrSent(SubmissionPoisonContext submissionPoisonContext)
		{
			this.resourceProtector.EnterReadLock();
			try
			{
				if (!this.loaded)
				{
					return false;
				}
				ArgumentValidator.ThrowIfNull("submissionPoisonContext", submissionPoisonContext);
				ArgumentValidator.ThrowIfEmpty("submissionPoisonContext.ResourceGuid", submissionPoisonContext.ResourceGuid);
				if (this.submissionPoisonDataStore.ContainsKey(submissionPoisonContext.ResourceGuid) && this.submissionPoisonDataStore[submissionPoisonContext.ResourceGuid].ContainsKey(submissionPoisonContext.MapiEventCounter))
				{
					return this.submissionPoisonDataStore[submissionPoisonContext.ResourceGuid][submissionPoisonContext.MapiEventCounter].IsPoisonNdrSent;
				}
			}
			finally
			{
				this.resourceProtector.ExitReadLock();
			}
			return false;
		}

		public void SavePoisonContext(SubmissionPoisonContext submissionPoisonContext)
		{
			this.resourceProtector.EnterReadLock();
			try
			{
				if (!this.loaded)
				{
					return;
				}
				if (submissionPoisonContext == null || submissionPoisonContext.ResourceGuid == Guid.Empty)
				{
					this.storeDriverTracer.GeneralTracer.TracePass(this.storeDriverTracer.MessageProbeActivityId, 0L, "Poison context information cannot be store on the crashing thread. Exiting...");
					return;
				}
			}
			finally
			{
				this.resourceProtector.ExitReadLock();
			}
			this.resourceProtector.EnterWriteLock();
			try
			{
				ResourceEventCounterCrashInfo resourceEventCounterCrashInfo;
				if (this.submissionPoisonDataStore.ContainsKey(submissionPoisonContext.ResourceGuid) && this.submissionPoisonDataStore[submissionPoisonContext.ResourceGuid].ContainsKey(submissionPoisonContext.MapiEventCounter))
				{
					resourceEventCounterCrashInfo = this.submissionPoisonDataStore[submissionPoisonContext.ResourceGuid][submissionPoisonContext.MapiEventCounter];
				}
				else
				{
					resourceEventCounterCrashInfo = new ResourceEventCounterCrashInfo(new SortedSet<DateTime>(), false);
				}
				resourceEventCounterCrashInfo.CrashTimes.Add(DateTime.UtcNow);
				try
				{
					this.crashRepository.PersistCrashInfo(submissionPoisonContext.ResourceGuid, submissionPoisonContext.MapiEventCounter, resourceEventCounterCrashInfo, this.maxPoisonEntries);
				}
				catch (CrashRepositoryAccessException)
				{
				}
			}
			finally
			{
				this.resourceProtector.ExitWriteLock();
			}
		}

		public void UpdatePoisonNdrSentToTrue(SubmissionPoisonContext submissionPoisonContext)
		{
			this.resourceProtector.EnterReadLock();
			try
			{
				if (!this.loaded)
				{
					return;
				}
				ArgumentValidator.ThrowIfNull("submissionPoisonContext", submissionPoisonContext);
				ArgumentValidator.ThrowIfEmpty("submissionPoisonContext.ResourceGuid", submissionPoisonContext.ResourceGuid);
			}
			finally
			{
				this.resourceProtector.ExitReadLock();
			}
			this.resourceProtector.EnterWriteLock();
			try
			{
				if (this.submissionPoisonDataStore.ContainsKey(submissionPoisonContext.ResourceGuid) && this.submissionPoisonDataStore[submissionPoisonContext.ResourceGuid].ContainsKey(submissionPoisonContext.MapiEventCounter))
				{
					ResourceEventCounterCrashInfo resourceEventCounterCrashInfo = this.submissionPoisonDataStore[submissionPoisonContext.ResourceGuid][submissionPoisonContext.MapiEventCounter];
					resourceEventCounterCrashInfo.IsPoisonNdrSent = true;
					this.crashRepository.PersistCrashInfo(submissionPoisonContext.ResourceGuid, submissionPoisonContext.MapiEventCounter, resourceEventCounterCrashInfo, this.maxPoisonEntries);
				}
			}
			finally
			{
				this.resourceProtector.ExitWriteLock();
			}
		}

		public SmtpResponse GetPoisonHandledSmtpResponse(long mapiEventCounterID)
		{
			return new SmtpResponse("554", "5.6.0", new string[]
			{
				string.Format(SubmissionPoisonHandler.poisonNdrGenerationFailureSmtpResponseText, mapiEventCounterID)
			});
		}

		private static string poisonNdrGenerationFailureSmtpResponseText = "An unexpected error was encountered during submission of this mail. Error reference number = {0}.";

		private readonly int maxPoisonEntries;

		private readonly TimeSpan poisonEntryExpiryWindow;

		private readonly QuarantineHandler quarantineHandler;

		private readonly ICrashRepository crashRepository;

		private readonly IStoreDriverTracer storeDriverTracer;

		private readonly ReaderWriterLockSlim resourceProtector = new ReaderWriterLockSlim();

		private Dictionary<Guid, Dictionary<long, ResourceEventCounterCrashInfo>> submissionPoisonDataStore = new Dictionary<Guid, Dictionary<long, ResourceEventCounterCrashInfo>>();

		private bool loaded;
	}
}
