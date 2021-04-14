using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal abstract class GlobalConfigurationBase<ConfigObjectT, SingletonWrapperT> where ConfigObjectT : ADConfigurationObject, new() where SingletonWrapperT : GlobalConfigurationBase<ConfigObjectT, SingletonWrapperT>, new()
	{
		public static event ADNotificationCallback ConfigurationObjectChanged;

		public static SingletonWrapperT Instance
		{
			get
			{
				return GlobalConfigurationBase<ConfigObjectT, SingletonWrapperT>.instance;
			}
		}

		public ConfigObjectT ConfigObject
		{
			get
			{
				return this.configObject;
			}
		}

		protected abstract string ConfigObjectName { get; }

		protected abstract string ReloadFailedString { get; }

		public static void Start()
		{
			SingletonWrapperT singletonWrapperT = Activator.CreateInstance<SingletonWrapperT>();
			GlobalConfigurationBase<ConfigObjectT, SingletonWrapperT>.instance = singletonWrapperT;
			ExTraceGlobals.ConfigurationTracer.TraceDebug<string>(0L, "Starting up monitoring of {0} configuration", singletonWrapperT.ConfigObjectName);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 105, "Start", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Configuration\\GlobalConfigurationBase.cs");
			GlobalConfigurationBase<ConfigObjectT, SingletonWrapperT>.cookie = ADNotificationAdapter.RegisterChangeNotification<ConfigObjectT>(singletonWrapperT.GetObjectId(tenantOrTopologyConfigurationSession), new ADNotificationCallback(GlobalConfigurationBase<ConfigObjectT, SingletonWrapperT>.Notify));
			ExTraceGlobals.ConfigurationTracer.TraceDebug<string>(0L, "Registered change notification for {0} configuration with AD. Startup successfull.", singletonWrapperT.ConfigObjectName);
			ADOperationResult adoperationResult;
			if (!singletonWrapperT.Load(out adoperationResult))
			{
				ExTraceGlobals.ConfigurationTracer.TraceError<string>(0L, "Could not load {0} configuration from AD. The service will be shut down", singletonWrapperT.ConfigObjectName);
				GlobalConfigurationBase<ConfigObjectT, SingletonWrapperT>.Stop();
				throw new TransportComponentLoadFailedException(singletonWrapperT.ReloadFailedString, adoperationResult.Exception);
			}
		}

		public static void Stop()
		{
			if (GlobalConfigurationBase<ConfigObjectT, SingletonWrapperT>.cookie != null)
			{
				ADNotificationAdapter.UnregisterChangeNotification(GlobalConfigurationBase<ConfigObjectT, SingletonWrapperT>.cookie);
			}
			ExTraceGlobals.ConfigurationTracer.TraceDebug<string>(0L, "Shut down {0} configuration monitoring", GlobalConfigurationBase<ConfigObjectT, SingletonWrapperT>.instance.ConfigObjectName);
		}

		protected abstract ADObjectId GetObjectId(IConfigurationSession session);

		protected virtual void HandleObjectLoaded()
		{
		}

		protected virtual bool HandleObjectNotFound()
		{
			return false;
		}

		private static void Notify(ADNotificationEventArgs args)
		{
			SingletonWrapperT singletonWrapperT = Activator.CreateInstance<SingletonWrapperT>();
			ExTraceGlobals.ConfigurationTracer.TraceDebug<string>(0L, "{0} change notification received", singletonWrapperT.ConfigObjectName);
			ADOperationResult adoperationResult;
			if (singletonWrapperT.Load(out adoperationResult))
			{
				GlobalConfigurationBase<ConfigObjectT, SingletonWrapperT>.instance = singletonWrapperT;
			}
			ADNotificationCallback configurationObjectChanged = GlobalConfigurationBase<ConfigObjectT, SingletonWrapperT>.ConfigurationObjectChanged;
			if (configurationObjectChanged != null)
			{
				Delegate[] invocationList = configurationObjectChanged.GetInvocationList();
				Delegate[] array = invocationList;
				for (int i = 0; i < array.Length; i++)
				{
					ADNotificationCallback handler = (ADNotificationCallback)array[i];
					adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						handler(args);
					});
					if (adoperationResult != ADOperationResult.Success)
					{
						ExTraceGlobals.ConfigurationTracer.TraceError<string, string, string>(0L, "An unhandled exception was raised by the {0} config. notification callback at {1}.{2}", singletonWrapperT.ConfigObjectName, handler.Method.DeclaringType.FullName, handler.Method.Name);
						GlobalConfigurationBase<ConfigObjectT, SingletonWrapperT>.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ComponentFailedTransportServerUpdate, null, new object[]
						{
							singletonWrapperT.ConfigObjectName
						});
					}
				}
			}
		}

		private bool Load(out ADOperationResult result)
		{
			ConfigObjectT[] objects = null;
			result = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 220, "Load", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Configuration\\GlobalConfigurationBase.cs");
				objects = tenantOrTopologyConfigurationSession.Find<ConfigObjectT>(this.GetObjectId(tenantOrTopologyConfigurationSession), QueryScope.Base, null, null, 1);
			});
			if (!result.Succeeded)
			{
				ExTraceGlobals.ConfigurationTracer.TraceError<string, string>((long)this.GetHashCode(), "Error reading {0} object, {1}", this.ConfigObjectName, (result.Exception == null) ? "no exception" : result.Exception.Message);
				return false;
			}
			if (objects != null && objects.Length == 1)
			{
				this.configObject = objects[0];
				this.HandleObjectLoaded();
				ExTraceGlobals.ConfigurationTracer.TraceDebug<string>((long)this.GetHashCode(), "{0} configuration object was read successfully.", this.ConfigObjectName);
				return true;
			}
			if (this.HandleObjectNotFound())
			{
				ExTraceGlobals.ConfigurationTracer.TraceDebug<string>((long)this.GetHashCode(), "{0} configuration object could not be read, but it is not critical.", this.ConfigObjectName);
				return true;
			}
			ExTraceGlobals.ConfigurationTracer.TraceError<string>((long)this.GetHashCode(), "{0} configuration object could not be read.", this.ConfigObjectName);
			return false;
		}

		private static ExEventLog eventLogger = new ExEventLog(new Guid("bad66201-f623-43e1-8fb4-bc5a8932c9f3"), TransportEventLog.GetEventSource());

		private static SingletonWrapperT instance;

		private static ADNotificationRequestCookie cookie;

		private ConfigObjectT configObject;
	}
}
