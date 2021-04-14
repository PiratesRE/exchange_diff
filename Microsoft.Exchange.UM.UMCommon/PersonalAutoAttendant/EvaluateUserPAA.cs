using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class EvaluateUserPAA : DisposableBase, IPAAEvaluator, IDisposeTrackable, IDisposable
	{
		private EvaluateUserPAA(UMSubscriber callee, PhoneNumber callerid, string diversion, bool asynchronous)
		{
			if (callee == null)
			{
				throw new ArgumentNullException("callee");
			}
			if (diversion == null)
			{
				throw new ArgumentNullException("diversion");
			}
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._Callee, callee.DisplayName),
				PIIMessage.Create(PIIType._Caller, callerid),
				PIIMessage.Create(PIIType._PII, diversion)
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "EvaluateUserPAA(callee = _Callee callerid=\"_Caller\" diversion=\"_PII\"", new object[0]);
			this.callerId = callerid;
			this.diversion = diversion;
			this.asynchronous = asynchronous;
			this.callee = callee;
			this.callee.AddReference();
			if (this.asynchronous)
			{
				this.reader = new NonBlockingReader(new NonBlockingReader.Operation(this.EvaluateCallback), callee, PAAConstants.PAAEvaluationTimeout, null);
			}
		}

		public IDataLoader UserDataLoader
		{
			get
			{
				return this.dataLoader;
			}
			set
			{
				this.dataLoader = value;
			}
		}

		public IPAAStore PAAStorage
		{
			get
			{
				return this.paaStore;
			}
			set
			{
				this.paaStore = value;
			}
		}

		public string CallId
		{
			get
			{
				return this.callId;
			}
			set
			{
				this.callId = value;
			}
		}

		public Breadcrumbs Crumbs
		{
			get
			{
				return this.crumbs;
			}
			set
			{
				this.crumbs = value;
			}
		}

		public bool EvaluationTimedOut
		{
			get
			{
				return this.evaluationTimedOut;
			}
		}

		public bool SubscriberHasPAAConfigured
		{
			get
			{
				return this.haveAtleastOnePAA;
			}
		}

		public bool GetEffectivePAA(out PersonalAutoAttendant personalAutoAttendant)
		{
			base.CheckDisposed();
			personalAutoAttendant = null;
			this.haveAtleastOnePAA = false;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA::GetEffectivePAA()", new object[0]);
			if (this.asynchronous)
			{
				this.reader.StartAsyncOperation();
				if (!this.reader.WaitForCompletion())
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA::GetEffectivePAA() Timed out", new object[0]);
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_TimedOutEvaluatingPAA, null, new object[]
					{
						this.callee.MailAddress,
						this.callId,
						this.callee.ExchangePrincipal.MailboxInfo.Location.ServerFqdn,
						this.callee.ExchangePrincipal.MailboxInfo.MailboxDatabase.ToString()
					});
					this.evaluationTimedOut = true;
					personalAutoAttendant = null;
					return false;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA::GetEffectivePAA() Evaluation finished in allotted time", new object[0]);
			}
			else
			{
				this.EvaluateCallbackWorker(this.callee);
			}
			if (this.paa != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA::GetEffectivePAA() found PAA: GUID={0} Description=\"{1}\"", new object[]
				{
					this.paa.Identity,
					this.paa.Name
				});
				personalAutoAttendant = this.paa;
				return true;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA::GetEffectivePAA() did not find any PAA", new object[0]);
			return false;
		}

		internal static IPAAEvaluator Create(UMSubscriber callee, PhoneNumber callerid, string diversion)
		{
			return new EvaluateUserPAA(callee, callerid, diversion, true);
		}

		internal static IPAAEvaluator CreateSynchronous(UMSubscriber callee, PhoneNumber callerid, string diversion)
		{
			return new EvaluateUserPAA(callee, callerid, diversion, false);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<EvaluateUserPAA>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.reader != null)
				{
					this.reader.Dispose();
					this.reader = null;
				}
				if (this.callee != null)
				{
					this.callee.ReleaseReference();
				}
			}
		}

		private void EvaluateCallback(object state)
		{
			using (new CallId(this.callId))
			{
				this.EvaluateCallbackWorker(state);
			}
		}

		private void EvaluateCallbackWorker(object state)
		{
			UMSubscriber umsubscriber = (UMSubscriber)state;
			PIIMessage data = PIIMessage.Create(PIIType._Callee, umsubscriber.DisplayName);
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "EvaluateUserPAA:EvaluateCallback() subscriber = _Callee", new object[0]);
			bool flag = false;
			bool flag2 = false;
			try
			{
				if (this.paaStore == null)
				{
					this.paaStore = PAAStore.Create(umsubscriber);
					flag = true;
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA:EvaluateCallback() using Custom Plugin Storage of type: {0}", new object[]
					{
						this.paaStore.GetType()
					});
				}
				PAAStoreStatus paastoreStatus;
				IList<PersonalAutoAttendant> autoAttendants = this.paaStore.GetAutoAttendants(PAAValidationMode.Actions, out paastoreStatus);
				if (autoAttendants.Count == 0)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA:EvaluateCallback() no autoattendants in store. Returning.", new object[0]);
					return;
				}
				this.haveAtleastOnePAA = true;
				if (this.dataLoader == null)
				{
					this.dataLoader = new UserDataLoader(umsubscriber, this.callerId, this.diversion);
					flag2 = true;
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA:EvaluateCallback() using Custom User dataloader of type: {0}", new object[]
					{
						this.dataLoader.GetType()
					});
				}
				try
				{
					for (int i = 0; i < autoAttendants.Count; i++)
					{
						PersonalAutoAttendant personalAutoAttendant = autoAttendants[i];
						CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA:EvaluateCallback() Evaluating PAA {0} Enabled={1} Version={2} Compatible={3} Valid={4}", new object[]
						{
							personalAutoAttendant.Identity,
							personalAutoAttendant.Enabled,
							personalAutoAttendant.Version,
							personalAutoAttendant.IsCompatible,
							personalAutoAttendant.Valid
						});
						if (!personalAutoAttendant.IsCompatible)
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA:EvaluateCallback() PAA {0} Version = {1} Is not compatible. Aborting PAA evaluation", new object[]
							{
								personalAutoAttendant.Identity,
								personalAutoAttendant.Version
							});
							break;
						}
						if (!personalAutoAttendant.Enabled)
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA:EvaluateCallback() PAA {0} is disabled. No rule matching will be performed", new object[]
							{
								personalAutoAttendant.Identity
							});
						}
						else
						{
							PAARulesEvaluator paarulesEvaluator = PAARulesEvaluator.Create(personalAutoAttendant);
							LatencyDetectionContext latencyDetectionContext = PAAUtils.PAAEvaluationFactory.CreateContext(CommonConstants.ApplicationVersion, Microsoft.Exchange.UM.UMCommon.CallId.Id ?? string.Empty, new IPerformanceDataProvider[]
							{
								RpcDataProvider.Instance,
								PerformanceContext.Current
							});
							bool flag3 = paarulesEvaluator.Evaluate(this.dataLoader);
							TaskPerformanceData[] array = latencyDetectionContext.StopAndFinalizeCollection();
							TaskPerformanceData taskPerformanceData = array[0];
							PerformanceData end = taskPerformanceData.End;
							if (end != PerformanceData.Zero)
							{
								PerformanceData difference = taskPerformanceData.Difference;
								CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA:EvaluateCallback() Evaluating PAA {0} RPCRequests = {1}, RPCLatency = {2}", new object[]
								{
									personalAutoAttendant.Identity,
									difference.Count,
									difference.Milliseconds
								});
							}
							TaskPerformanceData taskPerformanceData2 = array[1];
							PerformanceData end2 = taskPerformanceData2.End;
							if (end2 != PerformanceData.Zero)
							{
								PerformanceData difference2 = taskPerformanceData2.Difference;
								CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA:EvaluateCallback() Evaluating PAA {0} ADRequests = {1}, ADLatency = {2}", new object[]
								{
									personalAutoAttendant.Identity,
									difference2.Count,
									difference2.Milliseconds
								});
							}
							if (flag3)
							{
								this.paa = personalAutoAttendant;
								break;
							}
							CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA:EvaluateCallback() PAA {0} Failed Evaluation", new object[]
							{
								personalAutoAttendant.Identity
							});
						}
					}
				}
				finally
				{
					if (flag2 && this.dataLoader != null)
					{
						this.dataLoader.Dispose();
					}
				}
			}
			catch (ObjectDisposedException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA:EvaluateCallback() Exception on Querying/Evaluating PAA", new object[0]);
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "Exception : {0}", new object[]
				{
					ex
				});
				throw;
			}
			catch (LocalizedException ex2)
			{
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA:EvaluateCallback() Exception on Querying/Evaluating PAA", new object[0]);
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "Exception : {0}", new object[]
				{
					ex2
				});
				throw;
			}
			catch (XmlException ex3)
			{
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA:EvaluateCallback() Exception on Querying/Evaluating PAA", new object[0]);
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "Exception : {0}", new object[]
				{
					ex3
				});
				throw;
			}
			finally
			{
				if (flag && this.paaStore != null)
				{
					this.paaStore.Dispose();
				}
			}
			if (this.paa != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA:EvaluateCallback() found PAA: GUID={0} Description=\"{1}\"", new object[]
				{
					this.paa.Identity,
					this.paa.Name
				});
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "EvaluateUserPAA:EvaluateCallback() No PAA Found", new object[0]);
		}

		private readonly bool asynchronous;

		private NonBlockingReader reader;

		private PersonalAutoAttendant paa;

		private PhoneNumber callerId;

		private string diversion;

		private IDataLoader dataLoader;

		private IPAAStore paaStore;

		private bool evaluationTimedOut;

		private bool haveAtleastOnePAA;

		private string callId;

		private Breadcrumbs crumbs;

		private UMSubscriber callee;
	}
}
