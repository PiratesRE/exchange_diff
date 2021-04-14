using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Win32;

namespace Microsoft.Exchange.Provisioning
{
	internal static class ProvisioningLayer
	{
		public static bool Disabled
		{
			get
			{
				return ProvisioningLayer.disabled || ProvisioningLayer.IsEdge() || !ProvisioningLayer.InDomain();
			}
			set
			{
				ProvisioningLayer.disabled = value;
			}
		}

		public static void RefreshProvisioningBroker(Task task)
		{
			using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, "ProvisioningLayerLatency", "RefreshProvisioningBroker", LoggerHelper.CmdletPerfMonitors))
			{
				ExchangePropertyContainer.RefreshProvisioningBroker(task.SessionState);
			}
		}

		public static ProvisioningHandler[] GetProvisioningHandlers(Task task)
		{
			ProvisioningHandler[] provisioningHandlersImpl;
			using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, "ProvisioningLayerLatency", "GetProvisioningHandlers", LoggerHelper.CmdletPerfMonitors))
			{
				provisioningHandlersImpl = ProvisioningLayer.GetProvisioningHandlersImpl(task);
			}
			return provisioningHandlersImpl;
		}

		private static ProvisioningHandler[] GetProvisioningHandlersImpl(Task task)
		{
			if (ProvisioningLayer.Disabled)
			{
				return null;
			}
			ProvisioningBroker provisioningBroker = ExchangePropertyContainer.GetProvisioningBroker(task.SessionState);
			if (provisioningBroker.InitializationException != null && !task.CurrentTaskContext.InvocationInfo.CommandName.StartsWith("Get-"))
			{
				string commandName;
				if ((commandName = task.CurrentTaskContext.InvocationInfo.CommandName) == null || (!(commandName == "Set-CmdletExtensionAgent") && !(commandName == "Remove-CmdletExtensionAgent") && !(commandName == "Disable-CmdletExtensionAgent")))
				{
					ProvisioningBrokerException ex = new ProvisioningBrokerException(Strings.ProvisioningBrokerInitializationFailed(provisioningBroker.InitializationException.Message), provisioningBroker.InitializationException);
					TaskLogger.LogError(ex);
					throw ex;
				}
				task.WriteWarning(provisioningBroker.InitializationException.Message);
			}
			return provisioningBroker.GetProvisioningHandlers(task);
		}

		public static void SetLogMessageDelegate(Task task)
		{
			using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, "ProvisioningLayerLatency", "SetLogMessageDelegate", LoggerHelper.CmdletPerfMonitors))
			{
				ProvisioningLayer.SetLogMessageDelegateImpl(task);
			}
		}

		private static void SetLogMessageDelegateImpl(Task task)
		{
			if (ProvisioningLayer.Disabled || !task.IsProvisioningLayerAvailable)
			{
				return;
			}
			LogMessageDelegate logMessage = delegate(string message)
			{
				task.WriteVerbose(new LocalizedString(message));
			};
			WriteErrorDelegate writeError = delegate(LocalizedException ex, ExchangeErrorCategory category)
			{
				task.WriteError(ex, category, task.CurrentObjectIndex);
			};
			for (int i = 0; i < task.ProvisioningHandlers.Length; i++)
			{
				task.ProvisioningHandlers[i].LogMessage = logMessage;
				task.ProvisioningHandlers[i].WriteError = writeError;
			}
		}

		public static void SetUserScope(Task task)
		{
			using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, "ProvisioningLayerLatency", "SetUserScope", LoggerHelper.CmdletPerfMonitors))
			{
				ProvisioningLayer.SetUserScopeImpl(task);
			}
		}

		private static void SetUserScopeImpl(Task task)
		{
			if (ProvisioningLayer.Disabled || !task.IsProvisioningLayerAvailable)
			{
				return;
			}
			UserScopeFlags userScopeFlags = UserScopeFlags.None;
			if (task.ExchangeRunspaceConfig == null)
			{
				userScopeFlags |= UserScopeFlags.Local;
			}
			string userId = null;
			ADObjectId adobjectId;
			if (task.ExchangeRunspaceConfig != null)
			{
				userId = task.ExchangeRunspaceConfig.IdentityName;
			}
			else if (task.TryGetExecutingUserId(out adobjectId))
			{
				userId = adobjectId.ToString();
			}
			UserScope userScope = new UserScope(userId, task.ExecutingUserOrganizationId, task.CurrentOrganizationId, userScopeFlags, task.ScopeSet);
			for (int i = 0; i < task.ProvisioningHandlers.Length; i++)
			{
				task.ProvisioningHandlers[i].UserScope = userScope;
			}
		}

		public static void SetUserSpecifiedParameters(Task task, PropertyBag userSpecifiedParameters)
		{
			using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, "ProvisioningLayerLatency", "SetUserSpecifiedParameters", LoggerHelper.CmdletPerfMonitors))
			{
				if (!ProvisioningLayer.Disabled && task.IsProvisioningLayerAvailable)
				{
					for (int i = 0; i < task.ProvisioningHandlers.Length; i++)
					{
						task.ProvisioningHandlers[i].UserSpecifiedParameters = userSpecifiedParameters;
					}
				}
			}
		}

		public static void SetProvisioningCache(Task task, ProvisioningCache cache)
		{
			using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, "ProvisioningLayerLatency", "SetProvisioningCache", LoggerHelper.CmdletPerfMonitors))
			{
				if (!ProvisioningLayer.Disabled && task.IsProvisioningLayerAvailable)
				{
					for (int i = 0; i < task.ProvisioningHandlers.Length; i++)
					{
						task.ProvisioningHandlers[i].ProvisioningCache = cache;
					}
				}
			}
		}

		public static void ProvisionDefaultProperties(Task task, IConfigurable temporaryObject, IConfigurable dataObject, bool checkProvisioningLayerAvailability)
		{
			using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, "ProvisioningLayerLatency", "ProvisionDefaultProperties", LoggerHelper.CmdletPerfMonitors))
			{
				ProvisioningLayer.ProvisionDefaultPropertiesImpl(task, temporaryObject, dataObject, checkProvisioningLayerAvailability);
			}
		}

		private static void ProvisionDefaultPropertiesImpl(Task task, IConfigurable temporaryObject, IConfigurable dataObject, bool checkProvisioningLayerAvailability)
		{
			if (checkProvisioningLayerAvailability && (ProvisioningLayer.Disabled || !task.IsProvisioningLayerAvailable))
			{
				return;
			}
			ADObject adobject = dataObject as ADObject;
			HashSet<PropertyDefinition> hashSet = null;
			for (int i = 0; i < task.ProvisioningHandlers.Length; i++)
			{
				IConfigurable configurable = null;
				using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, task.ProvisioningHandlers[i].AgentName, "ProvisionDefaultProperties", LoggerHelper.CmdletPerfMonitors))
				{
					configurable = task.ProvisioningHandlers[i].ProvisionDefaultProperties(temporaryObject);
				}
				task.WriteVerbose(Strings.ProvisionDefaultProperties(i));
				if (configurable != null)
				{
					if (hashSet == null)
					{
						hashSet = ProvisioningLayer.LoadPrefilledProperties(dataObject as ADObject);
					}
					ADObject adobject2 = configurable as ADObject;
					if (adobject2 != null && adobject != null)
					{
						foreach (object obj in adobject2.propertyBag.Keys)
						{
							PropertyDefinition propertyDefinition = (PropertyDefinition)obj;
							ProviderPropertyDefinition providerPropertyDefinition = propertyDefinition as ProviderPropertyDefinition;
							if (providerPropertyDefinition != null && !hashSet.Contains(providerPropertyDefinition))
							{
								if (ProvisioningLayer.IsPropertyPrefilled(adobject.propertyBag, providerPropertyDefinition))
								{
									throw new ProvisioningException(Strings.PropertyIsAlreadyProvisioned(propertyDefinition.Name, i));
								}
								if (task.IsVerboseOn)
								{
									task.WriteVerbose(Strings.PropertyProvisioned(i, propertyDefinition.Name, (adobject2[propertyDefinition] ?? "<null>").ToString()));
								}
							}
						}
					}
					dataObject.CopyChangesFrom(configurable);
					temporaryObject.CopyChangesFrom(configurable);
				}
			}
		}

		private static bool IsPropertyPrefilled(PropertyBag propertyBag, ProviderPropertyDefinition property)
		{
			return propertyBag.Contains(property) && propertyBag[property] != property.DefaultValue;
		}

		private static HashSet<PropertyDefinition> LoadPrefilledProperties(IConfigurable inObject)
		{
			ADObject adobject = inObject as ADObject;
			if (adobject == null)
			{
				return new HashSet<PropertyDefinition>();
			}
			return new HashSet<PropertyDefinition>(adobject.propertyBag.Keys.Cast<PropertyDefinition>());
		}

		public static bool UpdateAffectedIConfigurable(Task task, IConfigurable writeableIConfigurable, bool checkProvisioningLayerAvailability)
		{
			bool result;
			using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, "ProvisioningLayerLatency", "UpdateAffectedIConfigurable", LoggerHelper.CmdletPerfMonitors))
			{
				if (checkProvisioningLayerAvailability && (ProvisioningLayer.Disabled || !task.IsProvisioningLayerAvailable))
				{
					result = false;
				}
				else
				{
					bool flag = false;
					for (int i = 0; i < task.ProvisioningHandlers.Length; i++)
					{
						using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, task.ProvisioningHandlers[i].AgentName, "UpdateAffectedIConfigurable", LoggerHelper.CmdletPerfMonitors))
						{
							flag |= task.ProvisioningHandlers[i].UpdateAffectedIConfigurable(writeableIConfigurable);
						}
					}
					result = flag;
				}
			}
			return result;
		}

		public static bool PreInternalProcessRecord(Task task, IConfigurable writeableIConfigurable, bool checkProvisioningLayerAvailability)
		{
			bool result;
			using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, "ProvisioningLayerLatency", "PreInternalProcessRecord", LoggerHelper.CmdletPerfMonitors))
			{
				if (checkProvisioningLayerAvailability && (ProvisioningLayer.Disabled || !task.IsProvisioningLayerAvailable))
				{
					result = false;
				}
				else
				{
					bool flag = false;
					for (int i = 0; i < task.ProvisioningHandlers.Length; i++)
					{
						using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, task.ProvisioningHandlers[i].AgentName, "PreInternalProcessRecord", LoggerHelper.CmdletPerfMonitors))
						{
							flag |= task.ProvisioningHandlers[i].PreInternalProcessRecord(writeableIConfigurable);
							task.WriteVerbose(Strings.ProvisioningPreInternalProcessRecord(i, flag));
						}
					}
					result = flag;
				}
			}
			return result;
		}

		public static ProvisioningValidationError[] Validate(Task task, IConfigurable readOnlyIConfigurable)
		{
			ProvisioningValidationError[] result;
			using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, "ProvisioningLayerLatency", "Validate", LoggerHelper.CmdletPerfMonitors))
			{
				result = ProvisioningLayer.ValidateImpl(task, readOnlyIConfigurable);
			}
			return result;
		}

		public static ProvisioningValidationError[] ValidateImpl(Task task, IConfigurable readOnlyIConfigurable)
		{
			if (ProvisioningLayer.Disabled || !task.IsProvisioningLayerAvailable)
			{
				return null;
			}
			List<ProvisioningValidationError> list = new List<ProvisioningValidationError>();
			for (int i = 0; i < task.ProvisioningHandlers.Length; i++)
			{
				using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, task.ProvisioningHandlers[i].AgentName, "Validate", LoggerHelper.CmdletPerfMonitors))
				{
					ProvisioningValidationError[] array = task.ProvisioningHandlers[i].Validate(readOnlyIConfigurable);
					if (array != null && array.Length > 0)
					{
						for (int j = 0; j < array.Length; j++)
						{
							array[j].AgentName = task.ProvisioningHandlers[i].AgentName;
						}
						list.AddRange(array);
						if (task.IsVerboseOn)
						{
							task.WriteVerbose(TaskVerboseStringHelper.GetProvisioningValidationErrors(array, i));
						}
					}
				}
			}
			return list.ToArray();
		}

		public static ProvisioningValidationError[] ValidateUserScope(Task task)
		{
			ProvisioningValidationError[] result;
			using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, "ProvisioningLayerLatency", "ValidateUserScope", LoggerHelper.CmdletPerfMonitors))
			{
				result = ProvisioningLayer.ValidateUserScopeImpl(task);
			}
			return result;
		}

		private static ProvisioningValidationError[] ValidateUserScopeImpl(Task task)
		{
			if (ProvisioningLayer.Disabled || !task.IsProvisioningLayerAvailable)
			{
				return null;
			}
			List<ProvisioningValidationError> list = new List<ProvisioningValidationError>();
			for (int i = 0; i < task.ProvisioningHandlers.Length; i++)
			{
				using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, task.ProvisioningHandlers[i].AgentName, "ValidateUserScope", LoggerHelper.CmdletPerfMonitors))
				{
					ProvisioningValidationError[] array = task.ProvisioningHandlers[i].ValidateUserScope();
					if (array != null && array.Length > 0)
					{
						for (int j = 0; j < array.Length; j++)
						{
							array[j].AgentName = task.ProvisioningHandlers[i].AgentName;
						}
						list.AddRange(array);
						if (task.IsVerboseOn)
						{
							task.WriteVerbose(TaskVerboseStringHelper.GetProvisioningValidationErrors(array, i));
						}
					}
				}
			}
			return list.ToArray();
		}

		public static void OnComplete(Task task, bool succeeded, Exception exception)
		{
			using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, "ProvisioningLayerLatency", "OnComplete", LoggerHelper.CmdletPerfMonitors))
			{
				ProvisioningLayer.OnCompleteImpl(task, succeeded, exception);
			}
		}

		private static void OnCompleteImpl(Task task, bool succeeded, Exception exception)
		{
			Dictionary<int, Exception> dictionary = null;
			if (ProvisioningLayer.Disabled || !task.IsProvisioningLayerAvailable)
			{
				return;
			}
			for (int i = 0; i < task.ProvisioningHandlers.Length; i++)
			{
				try
				{
					using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, task.ProvisioningHandlers[i].AgentName, "OnComplete", LoggerHelper.CmdletPerfMonitors))
					{
						task.ProvisioningHandlers[i].OnComplete(succeeded, exception);
					}
				}
				catch (Exception ex)
				{
					if (ProvisioningLayer.IsUnsafeException(ex))
					{
						throw;
					}
					if (dictionary == null)
					{
						dictionary = new Dictionary<int, Exception>();
					}
					dictionary.Add(i, ex);
				}
			}
			if (dictionary != null)
			{
				foreach (int num in dictionary.Keys)
				{
					task.WriteWarning(Strings.HandlerThronwExceptionInOnComplete(num, dictionary[num].ToString()));
				}
			}
		}

		public static void EndProcessing(Task task)
		{
			using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, "ProvisioningLayerLatency", "EndProcessiong", LoggerHelper.CmdletPerfMonitors))
			{
				ProvisioningLayer.EndProcessingImpl(task);
			}
		}

		private static void EndProcessingImpl(Task task)
		{
			Dictionary<int, Exception> dictionary = null;
			if (ProvisioningLayer.Disabled || !task.IsProvisioningLayerAvailable)
			{
				return;
			}
			for (int i = 0; i < task.ProvisioningHandlers.Length; i++)
			{
				using (new CmdletMonitoredScope(task.CurrentTaskContext.UniqueId, task.ProvisioningHandlers[i].AgentName, "Dispose", LoggerHelper.CmdletPerfMonitors))
				{
					try
					{
						IDisposable disposable = task.ProvisioningHandlers[i] as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
					catch (Exception ex)
					{
						if (ProvisioningLayer.IsUnsafeException(ex))
						{
							throw;
						}
						if (dictionary == null)
						{
							dictionary = new Dictionary<int, Exception>();
						}
						dictionary.Add(i, ex);
					}
				}
			}
			if (dictionary != null)
			{
				foreach (int key in dictionary.Keys)
				{
					task.WriteWarning(dictionary[key].ToString());
				}
			}
		}

		private static bool IsEdge()
		{
			if (ADSession.IsBoundToAdam)
			{
				return true;
			}
			string keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole";
			string text = (string)Registry.GetValue(keyName, "ConfiguredVersion", null);
			return text != null;
		}

		private static bool InDomain()
		{
			bool result;
			try
			{
				NativeHelpers.GetDomainName();
				result = true;
			}
			catch (CannotGetDomainInfoException)
			{
				result = false;
			}
			return result;
		}

		private static bool IsUnsafeException(Exception e)
		{
			return e is StackOverflowException || e is OutOfMemoryException || e is ThreadAbortException;
		}

		private static bool disabled;
	}
}
