using System;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DxStore;
using Microsoft.Exchange.DxStore.Common;

namespace Microsoft.Exchange.DxStore.Server
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
	public class DxStoreAccess : IDxStoreAccess
	{
		public DxStoreAccess(DxStoreInstance instance)
		{
			this.instance = instance;
			this.UpdateTimeout = instance.GroupConfig.Settings.PaxosUpdateTimeout;
		}

		public TimeSpan UpdateTimeout { get; set; }

		public WriteResult CreateKey(string baseKeyName, string subKeyName, WriteOptions writeOptions)
		{
			DxStoreCommand.CreateKey command = new DxStoreCommand.CreateKey
			{
				KeyName = baseKeyName,
				SubkeyName = subKeyName
			};
			return this.ExecuteCommand(command, writeOptions, this.UpdateTimeout);
		}

		public DxStoreAccessReply.CheckKey CheckKey(DxStoreAccessRequest.CheckKey request)
		{
			return this.RunServerOperationAndConvertToFaultException<DxStoreAccessRequest.CheckKey, DxStoreAccessReply.CheckKey>(request, (DxStoreAccessRequest.CheckKey req) => this.CheckKeyInternal(req));
		}

		public DxStoreAccessReply.DeleteKey DeleteKey(DxStoreAccessRequest.DeleteKey request)
		{
			return this.RunServerOperationAndConvertToFaultException<DxStoreAccessRequest.DeleteKey, DxStoreAccessReply.DeleteKey>(request, (DxStoreAccessRequest.DeleteKey req) => this.DeleteKeyInternal(req));
		}

		public DxStoreAccessReply.SetProperty SetProperty(DxStoreAccessRequest.SetProperty request)
		{
			return this.RunServerOperationAndConvertToFaultException<DxStoreAccessRequest.SetProperty, DxStoreAccessReply.SetProperty>(request, (DxStoreAccessRequest.SetProperty req) => this.SetPropertyInternal(req));
		}

		public DxStoreAccessReply.DeleteProperty DeleteProperty(DxStoreAccessRequest.DeleteProperty request)
		{
			return this.RunServerOperationAndConvertToFaultException<DxStoreAccessRequest.DeleteProperty, DxStoreAccessReply.DeleteProperty>(request, (DxStoreAccessRequest.DeleteProperty req) => this.DeletePropertyInternal(req));
		}

		public DxStoreAccessReply.ExecuteBatch ExecuteBatch(DxStoreAccessRequest.ExecuteBatch request)
		{
			return this.RunServerOperationAndConvertToFaultException<DxStoreAccessRequest.ExecuteBatch, DxStoreAccessReply.ExecuteBatch>(request, (DxStoreAccessRequest.ExecuteBatch req) => this.ExecuteBatchInternal(req));
		}

		public DxStoreAccessReply.GetProperty GetProperty(DxStoreAccessRequest.GetProperty request)
		{
			return this.RunServerOperationAndConvertToFaultException<DxStoreAccessRequest.GetProperty, DxStoreAccessReply.GetProperty>(request, (DxStoreAccessRequest.GetProperty req) => this.GetPropertyInternal(req));
		}

		public DxStoreAccessReply.GetAllProperties GetAllProperties(DxStoreAccessRequest.GetAllProperties request)
		{
			return this.RunServerOperationAndConvertToFaultException<DxStoreAccessRequest.GetAllProperties, DxStoreAccessReply.GetAllProperties>(request, (DxStoreAccessRequest.GetAllProperties req) => this.GetAllPropertiesInternal(req));
		}

		public DxStoreAccessReply.GetPropertyNames GetPropertyNames(DxStoreAccessRequest.GetPropertyNames request)
		{
			return this.RunServerOperationAndConvertToFaultException<DxStoreAccessRequest.GetPropertyNames, DxStoreAccessReply.GetPropertyNames>(request, (DxStoreAccessRequest.GetPropertyNames req) => this.GetPropertyNamesInternal(req));
		}

		public DxStoreAccessReply.GetSubkeyNames GetSubkeyNames(DxStoreAccessRequest.GetSubkeyNames request)
		{
			return this.RunServerOperationAndConvertToFaultException<DxStoreAccessRequest.GetSubkeyNames, DxStoreAccessReply.GetSubkeyNames>(request, (DxStoreAccessRequest.GetSubkeyNames req) => this.GetSubkeyNamesInternal(req));
		}

		public TRep RunServerOperationAndConvertToFaultException<TReq, TRep>(TReq request, Func<TReq, TRep> action) where TReq : DxStoreAccessRequest where TRep : DxStoreAccessReply
		{
			TRep trep = default(TRep);
			try
			{
				if (ExTraceGlobals.AccessEntryPointTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					string arg = Utils.SerializeObjectToJsonString<TReq>(request, false, true) ?? "<serialization error>";
					ExTraceGlobals.AccessEntryPointTracer.TraceDebug<string, string, string>((long)this.instance.IdentityHash, "{0}: Entering Request: {1} {2}", this.instance.Identity, typeof(TReq).Name, arg);
				}
				trep = action(request);
			}
			catch (Exception ex)
			{
				if (!this.instance.GroupConfig.Settings.IsUseHttpTransportForClientCommunication)
				{
					DxStoreServerFault dxStoreServerFault = WcfUtils.ConvertExceptionToDxStoreFault(ex);
					if (ExTraceGlobals.AccessEntryPointTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						string text = Utils.SerializeObjectToJsonString<DxStoreServerFault>(dxStoreServerFault, false, true) ?? "<serialization error>";
						ExTraceGlobals.AccessEntryPointTracer.TraceError((long)this.instance.IdentityHash, "{0}: Leaving (Failed) - Request: {1} - Fault: {2} - Exception: {3}", new object[]
						{
							this.instance.Identity,
							typeof(TReq).Name,
							text,
							ex
						});
					}
					throw new FaultException<DxStoreServerFault>(dxStoreServerFault);
				}
				ExTraceGlobals.AccessEntryPointTracer.TraceError<string, string, Exception>((long)this.instance.IdentityHash, "{0}: Leaving (Failed) - Request: {1} - Exception: {2}", this.instance.Identity, typeof(TReq).Name, ex);
				throw;
			}
			if (ExTraceGlobals.AccessEntryPointTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string text2 = (trep != null) ? (Utils.SerializeObjectToJsonString<TRep>(trep, false, true) ?? "<serialization error>") : "<null>";
				ExTraceGlobals.AccessEntryPointTracer.TraceDebug((long)this.instance.IdentityHash, "{0}: Leaving (Success) - Request: {1} Reply: {2} {3}", new object[]
				{
					this.instance.Identity,
					typeof(TReq).Name,
					typeof(TRep).Name,
					text2
				});
			}
			return trep;
		}

		private DxStoreAccessReply.CheckKey CheckKeyInternal(DxStoreAccessRequest.CheckKey request)
		{
			bool isStale = !this.EnsureInstanceReadyAndNotStale(request.ReadOptions);
			DxStoreAccessReply.CheckKey checkKey = this.CreateReply<DxStoreAccessReply.CheckKey>();
			string keyName = Utils.CombinePathNullSafe(request.FullKeyName, request.SubkeyName);
			checkKey.ReadResult = new ReadResult
			{
				IsStale = isStale
			};
			checkKey.IsExist = this.instance.LocalDataStore.IsKeyExist(keyName);
			if (!checkKey.IsExist && request.IsCreateIfNotExist)
			{
				checkKey.WriteResult = this.CreateKey(request.FullKeyName, request.SubkeyName, request.WriteOptions);
				checkKey.IsCreated = true;
				checkKey.IsExist = true;
			}
			return this.FinishRequest<DxStoreAccessReply.CheckKey>(checkKey);
		}

		private DxStoreAccessReply.DeleteKey DeleteKeyInternal(DxStoreAccessRequest.DeleteKey request)
		{
			bool isStale = !this.EnsureInstanceReadyAndNotStale(request.ReadOptions);
			DxStoreAccessReply.DeleteKey deleteKey = this.CreateReply<DxStoreAccessReply.DeleteKey>();
			string keyName = Utils.CombinePathNullSafe(request.FullKeyName, request.SubkeyName);
			deleteKey.ReadResult = new ReadResult
			{
				IsStale = isStale
			};
			deleteKey.IsExist = this.instance.LocalDataStore.IsKeyExist(keyName);
			if (deleteKey.IsExist)
			{
				DxStoreCommand.DeleteKey command = new DxStoreCommand.DeleteKey
				{
					KeyName = request.FullKeyName,
					SubkeyName = request.SubkeyName
				};
				deleteKey.WriteResult = this.ExecuteCommand(command, request.WriteOptions, this.UpdateTimeout);
			}
			return this.FinishRequest<DxStoreAccessReply.DeleteKey>(deleteKey);
		}

		private DxStoreAccessReply.SetProperty SetPropertyInternal(DxStoreAccessRequest.SetProperty request)
		{
			this.VerifyInstanceReady();
			DxStoreAccessReply.SetProperty setProperty = this.CreateReply<DxStoreAccessReply.SetProperty>();
			DxStoreCommand.SetProperty command = new DxStoreCommand.SetProperty
			{
				KeyName = request.FullKeyName,
				Name = request.Name,
				Value = request.Value
			};
			setProperty.WriteResult = this.ExecuteCommand(command, request.WriteOptions, this.UpdateTimeout);
			return this.FinishRequest<DxStoreAccessReply.SetProperty>(setProperty);
		}

		private DxStoreAccessReply.DeleteProperty DeletePropertyInternal(DxStoreAccessRequest.DeleteProperty request)
		{
			bool isStale = !this.EnsureInstanceReadyAndNotStale(request.ReadOptions);
			DxStoreAccessReply.DeleteProperty deleteProperty = this.CreateReply<DxStoreAccessReply.DeleteProperty>();
			deleteProperty.ReadResult = new ReadResult
			{
				IsStale = isStale
			};
			deleteProperty.IsExist = this.instance.LocalDataStore.IsPropertyExist(request.FullKeyName, request.Name);
			if (deleteProperty.IsExist)
			{
				DxStoreCommand.DeleteProperty command = new DxStoreCommand.DeleteProperty
				{
					KeyName = request.FullKeyName,
					Name = request.Name
				};
				deleteProperty.WriteResult = this.ExecuteCommand(command, request.WriteOptions, this.UpdateTimeout);
			}
			return this.FinishRequest<DxStoreAccessReply.DeleteProperty>(deleteProperty);
		}

		private DxStoreAccessReply.ExecuteBatch ExecuteBatchInternal(DxStoreAccessRequest.ExecuteBatch request)
		{
			this.VerifyInstanceReady();
			DxStoreAccessReply.ExecuteBatch executeBatch = this.CreateReply<DxStoreAccessReply.ExecuteBatch>();
			DxStoreCommand.ExecuteBatch command = new DxStoreCommand.ExecuteBatch
			{
				KeyName = request.FullKeyName,
				Commands = request.Commands
			};
			executeBatch.WriteResult = this.ExecuteCommand(command, request.WriteOptions, this.UpdateTimeout);
			return this.FinishRequest<DxStoreAccessReply.ExecuteBatch>(executeBatch);
		}

		private DxStoreAccessReply.GetProperty GetPropertyInternal(DxStoreAccessRequest.GetProperty request)
		{
			bool isStale = !this.EnsureInstanceReadyAndNotStale(request.ReadOptions);
			DxStoreAccessReply.GetProperty getProperty = this.CreateReply<DxStoreAccessReply.GetProperty>();
			getProperty.ReadResult = new ReadResult
			{
				IsStale = isStale
			};
			getProperty.Value = this.instance.LocalDataStore.GetProperty(request.FullKeyName, request.Name);
			return this.FinishRequest<DxStoreAccessReply.GetProperty>(getProperty);
		}

		private DxStoreAccessReply.GetAllProperties GetAllPropertiesInternal(DxStoreAccessRequest.GetAllProperties request)
		{
			bool isStale = !this.EnsureInstanceReadyAndNotStale(request.ReadOptions);
			DxStoreAccessReply.GetAllProperties getAllProperties = this.CreateReply<DxStoreAccessReply.GetAllProperties>();
			getAllProperties.ReadResult = new ReadResult
			{
				IsStale = isStale
			};
			getAllProperties.Values = this.instance.LocalDataStore.GetAllProperties(request.FullKeyName);
			return this.FinishRequest<DxStoreAccessReply.GetAllProperties>(getAllProperties);
		}

		private DxStoreAccessReply.GetPropertyNames GetPropertyNamesInternal(DxStoreAccessRequest.GetPropertyNames request)
		{
			bool isStale = !this.EnsureInstanceReadyAndNotStale(request.ReadOptions);
			DxStoreAccessReply.GetPropertyNames getPropertyNames = this.CreateReply<DxStoreAccessReply.GetPropertyNames>();
			getPropertyNames.ReadResult = new ReadResult
			{
				IsStale = isStale
			};
			getPropertyNames.Infos = this.instance.LocalDataStore.EnumPropertyNames(request.FullKeyName);
			return this.FinishRequest<DxStoreAccessReply.GetPropertyNames>(getPropertyNames);
		}

		private DxStoreAccessReply.GetSubkeyNames GetSubkeyNamesInternal(DxStoreAccessRequest.GetSubkeyNames request)
		{
			bool isStale = !this.EnsureInstanceReadyAndNotStale(request.ReadOptions);
			DxStoreAccessReply.GetSubkeyNames getSubkeyNames = this.CreateReply<DxStoreAccessReply.GetSubkeyNames>();
			getSubkeyNames.ReadResult = new ReadResult
			{
				IsStale = isStale
			};
			getSubkeyNames.Keys = this.instance.LocalDataStore.EnumSubkeyNames(request.FullKeyName);
			return this.FinishRequest<DxStoreAccessReply.GetSubkeyNames>(getSubkeyNames);
		}

		private T CreateReply<T>() where T : DxStoreAccessReply, new()
		{
			T result = Activator.CreateInstance<T>();
			result.Initialize(this.instance.GroupConfig.Self);
			return result;
		}

		private WriteResult EnsureTestUpdateIsSuccessful(DxStoreCommand command, WriteOptions options, TimeSpan timeout)
		{
			DxStoreCommand.DummyCommand dummyCommand = new DxStoreCommand.DummyCommand
			{
				OriginalDbCommandId = command.CommandId
			};
			dummyCommand.Initialize(this.instance.GroupConfig.Self, options);
			WriteResult writeResult = this.ExecuteCommandWithAck(dummyCommand, options, true, timeout);
			if (!writeResult.IsConstraintPassed)
			{
				throw new DxStoreCommandConstraintFailedException("TestUpdate");
			}
			return writeResult;
		}

		private WriteResult ExecuteCommand(DxStoreCommand command, WriteOptions options, TimeSpan timeout)
		{
			WriteResult writeResult = null;
			command.Initialize(this.instance.GroupConfig.Self, options);
			if (options != null && (options.IsPerformTestUpdate || options.IsWaitRequired()))
			{
				if (options.IsPerformTestUpdate)
				{
					this.EnsureTestUpdateIsSuccessful(command, options, timeout);
				}
				writeResult = this.ExecuteCommandWithAck(command, options, true, timeout);
				if (!writeResult.IsConstraintPassed)
				{
					this.instance.EventLogger.Log(DxEventSeverity.Warning, 0, "{0}: Failed to satisfy constraint in the second attempt. Updates will eventually catch up", new object[]
					{
						this.instance.GroupConfig.Identity
					});
				}
			}
			else
			{
				this.instance.StateMachine.ReplicateCommand(command, new TimeSpan?(timeout));
			}
			return writeResult;
		}

		private WriteResult ExecuteCommandWithAck(DxStoreCommand command, WriteOptions options, bool isThrowOnException, TimeSpan timeout)
		{
			LocalCommitAcknowledger commitAcknowledger = this.instance.CommitAcknowledger;
			WriteResult writeResult = new WriteResult();
			Guid commandId = command.CommandId;
			try
			{
				commitAcknowledger.AddCommand(command, options);
				this.instance.StateMachine.ReplicateCommand(command, new TimeSpan?(timeout));
				if (commitAcknowledger.WaitForExecution(commandId, timeout))
				{
					writeResult.IsConstraintPassed = true;
				}
			}
			finally
			{
				writeResult.Responses = commitAcknowledger.RemoveCommand(commandId);
			}
			return writeResult;
		}

		private bool EnsureInstanceReadyAndNotStale(ReadOptions readOptions)
		{
			this.VerifyInstanceReady();
			return this.EnsureDatabaseNotStale(readOptions);
		}

		private void VerifyInstanceReady()
		{
			this.instance.EnsureInstanceIsReady();
		}

		private bool EnsureDatabaseNotStale(ReadOptions readOptions)
		{
			if (this.instance.HealthChecker.IsStoreReady())
			{
				return true;
			}
			if (readOptions != null && readOptions.IsAllowStale)
			{
				return false;
			}
			throw new DxStoreInstanceStaleStoreException();
		}

		private void InitializeCommand(DxStoreCommand command, WriteOptions options)
		{
			command.TimeInitiated = DateTimeOffset.Now;
			command.Initiator = this.instance.GroupConfig.Self;
			if (options != null)
			{
				command.IsNotifyInitiator = options.IsWaitRequired();
			}
		}

		private T FinishRequest<T>(T reply) where T : DxStoreAccessReply
		{
			reply.MostRecentUpdateNumber = this.instance.LocalDataStore.LastInstanceExecuted;
			reply.Duration = DateTimeOffset.Now - reply.TimeReceived;
			return reply;
		}

		private readonly DxStoreInstance instance;
	}
}
