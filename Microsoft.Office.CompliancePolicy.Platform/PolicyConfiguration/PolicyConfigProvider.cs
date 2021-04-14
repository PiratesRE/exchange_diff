using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	public abstract class PolicyConfigProvider : IDisposable
	{
		public PolicyConfigProvider() : this(null)
		{
		}

		public PolicyConfigProvider(ExecutionLog logProvider)
		{
			this.logProvider = logProvider;
		}

		public event PolicyConfigChangeEventHandler PolicyConfigChanged;

		public void LogOneEntry(string tenantId, ExecutionLog.EventType eventType, string objectType, string contextData, Exception exception)
		{
			if (this.logProvider != null)
			{
				this.logProvider.LogOneEntry("PolicyConfigProvider", tenantId, this.GetHashCode().ToString(), eventType, objectType, contextData, exception, new KeyValuePair<string, object>[0]);
			}
		}

		public void LogOneEntry(ExecutionLog.EventType eventType, string contextData, Exception exception)
		{
			this.LogOneEntry(string.Empty, eventType, string.Empty, contextData, exception);
		}

		public string GetLocalOrganizationId()
		{
			this.CheckDispose();
			string organizationId = null;
			this.WrapKnownException(delegate
			{
				organizationId = this.InternalGetLocalOrganizationId();
			});
			return organizationId;
		}

		public T FindByIdentity<T>(Guid identity) where T : PolicyConfigBase
		{
			this.CheckDispose();
			T config = default(T);
			this.WrapKnownException(delegate
			{
				config = this.InternalFindByIdentity<T>(identity);
				if (config != null)
				{
					config.ResetChangeTracking();
				}
			});
			return config;
		}

		public IEnumerable<T> FindByName<T>(string name) where T : PolicyConfigBase
		{
			this.CheckDispose();
			IEnumerable<T> configs = null;
			this.WrapKnownException(delegate
			{
				configs = this.InternalFindByName<T>(name);
				this.ResetChangeTrackingForCollection<T>(configs);
			});
			return configs;
		}

		public IEnumerable<T> FindByPolicyDefinitionConfigId<T>(Guid policyDefinitionConfigId) where T : PolicyConfigBase
		{
			this.CheckDispose();
			IEnumerable<T> configs = null;
			this.WrapKnownException(delegate
			{
				configs = this.InternalFindByPolicyDefinitionConfigId<T>(policyDefinitionConfigId);
				this.ResetChangeTrackingForCollection<T>(configs);
			});
			return configs;
		}

		public IEnumerable<PolicyBindingConfig> FindPolicyBindingConfigsByScopes(IEnumerable<string> scopes)
		{
			this.CheckDispose();
			IEnumerable<PolicyBindingConfig> bindingConfigs = null;
			this.WrapKnownException(delegate
			{
				bindingConfigs = this.InternalFindPolicyBindingConfigsByScopes(scopes);
				this.ResetChangeTrackingForCollection<PolicyBindingConfig>(bindingConfigs);
			});
			return bindingConfigs;
		}

		public PolicyAssociationConfig FindPolicyAssociationConfigByScope(string scope)
		{
			this.CheckDispose();
			PolicyAssociationConfig associationConfig = null;
			this.WrapKnownException(delegate
			{
				associationConfig = this.InternalFindPolicyAssociationConfigByScope(scope);
				if (associationConfig != null)
				{
					associationConfig.ResetChangeTracking();
				}
			});
			return associationConfig;
		}

		public void Save(PolicyConfigBase instance)
		{
			this.CheckDispose();
			ArgumentValidator.ThrowIfNull("instance", instance);
			this.WrapKnownException(delegate
			{
				string name = instance.GetType().Name;
				object wholeProperty = Utility.GetWholeProperty(instance, "Scenario");
				string text = (wholeProperty != null) ? string.Format(CultureInfo.InvariantCulture, "Scenario: {0}.", new object[]
				{
					wholeProperty.ToString()
				}) : string.Empty;
				this.LogOneEntry(string.Empty, ExecutionLog.EventType.Information, name, string.Format(CultureInfo.InvariantCulture, "Begin Save: {0}. {1}", new object[]
				{
					instance.Identity,
					text
				}), null);
				this.InternalSave(instance);
				this.LogOneEntry(string.Empty, ExecutionLog.EventType.Information, name, string.Format(CultureInfo.InvariantCulture, "Begin Save Change Event: {0}. {1}", new object[]
				{
					instance.Identity,
					text
				}), null);
				this.OnPolicyConfigChanged(new PolicyConfigChangeEventArgs(this, instance, ChangeType.Update));
				instance.ResetChangeTracking();
				this.LogOneEntry(string.Empty, ExecutionLog.EventType.Information, name, string.Format(CultureInfo.InvariantCulture, "End Save: {0}. {1}", new object[]
				{
					instance.Identity,
					text
				}), null);
			});
		}

		public void Delete(PolicyConfigBase instance)
		{
			this.CheckDispose();
			ArgumentValidator.ThrowIfNull("instance", instance);
			this.WrapKnownException(delegate
			{
				string name = instance.GetType().Name;
				object wholeProperty = Utility.GetWholeProperty(instance, "Scenario");
				string text = (wholeProperty != null) ? string.Format(CultureInfo.InvariantCulture, "Scenario: {0}.", new object[]
				{
					wholeProperty.ToString()
				}) : string.Empty;
				this.LogOneEntry(string.Empty, ExecutionLog.EventType.Information, name, string.Format(CultureInfo.InvariantCulture, "Begin Delete: {0}. {1}", new object[]
				{
					instance.Identity,
					text
				}), null);
				this.InternalDelete(instance);
				instance.MarkAsDeleted();
				this.LogOneEntry(string.Empty, ExecutionLog.EventType.Information, name, string.Format(CultureInfo.InvariantCulture, "End Delete: {0}. {1}", new object[]
				{
					instance.Identity,
					text
				}), null);
			});
		}

		public void PublishStatus(IEnumerable<UnifiedPolicyStatus> statusUpdateNotifications)
		{
			ArgumentValidator.ThrowIfCollectionNullOrEmpty<UnifiedPolicyStatus>("statusUpdateNotifications", statusUpdateNotifications);
			this.CheckDispose();
			this.WrapKnownException(delegate
			{
				this.InternalPublishStatus(statusUpdateNotifications);
			});
		}

		public T NewBlankConfigInstance<T>() where T : PolicyConfigBase
		{
			this.CheckDispose();
			T config = default(T);
			this.WrapKnownException(delegate
			{
				config = this.InternalNewBlankConfigInstance<T>();
			});
			return config;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed && disposing)
			{
				this.isDisposed = true;
			}
		}

		protected void CheckDispose()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
		}

		protected abstract string InternalGetLocalOrganizationId();

		protected abstract T InternalFindByIdentity<T>(Guid identity) where T : PolicyConfigBase;

		protected abstract IEnumerable<T> InternalFindByName<T>(string name) where T : PolicyConfigBase;

		protected virtual IEnumerable<T> InternalFindByPolicyDefinitionConfigId<T>(Guid policyDefinitionConfigId) where T : PolicyConfigBase
		{
			if (!typeof(PolicyBindingSetConfig).IsAssignableFrom(typeof(T)))
			{
				throw new InvalidOperationException();
			}
			IEnumerable<PolicyBindingConfig> enumerable = this.FindByPolicyDefinitionConfigId<PolicyBindingConfig>(policyDefinitionConfigId);
			if (enumerable == null || !enumerable.Any<PolicyBindingConfig>())
			{
				return null;
			}
			PolicyBindingSetConfig policyBindingSetConfig = this.NewBlankConfigInstance<PolicyBindingSetConfig>();
			policyBindingSetConfig.Identity = Guid.NewGuid();
			policyBindingSetConfig.PolicyDefinitionConfigId = policyDefinitionConfigId;
			policyBindingSetConfig.AppliedScopes = enumerable;
			policyBindingSetConfig.Version = PolicyVersion.Empty;
			policyBindingSetConfig.ResetChangeTracking();
			return new T[]
			{
				policyBindingSetConfig as T
			};
		}

		protected abstract IEnumerable<PolicyBindingConfig> InternalFindPolicyBindingConfigsByScopes(IEnumerable<string> scopes);

		protected abstract PolicyAssociationConfig InternalFindPolicyAssociationConfigByScope(string scope);

		protected virtual void InternalSave(PolicyConfigBase instance)
		{
			if (instance is PolicyBindingSetConfig)
			{
				PolicyBindingSetConfig policyBindingSetConfig = instance as PolicyBindingSetConfig;
				if (policyBindingSetConfig.AppliedScopes != null)
				{
					using (IEnumerator<PolicyBindingConfig> enumerator = policyBindingSetConfig.AppliedScopes.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							PolicyBindingConfig policyBindingConfig = enumerator.Current;
							if (policyBindingConfig.ObjectState == ChangeType.Delete)
							{
								this.InternalDelete(policyBindingConfig);
							}
							else if (policyBindingConfig.ObjectState != ChangeType.None)
							{
								this.InternalSave(policyBindingConfig);
							}
						}
						return;
					}
					goto IL_61;
				}
				return;
			}
			IL_61:
			throw new InvalidOperationException();
		}

		protected virtual void InternalDelete(PolicyConfigBase instance)
		{
			if (instance is PolicyBindingSetConfig)
			{
				PolicyBindingSetConfig policyBindingSetConfig = instance as PolicyBindingSetConfig;
				if (policyBindingSetConfig.AppliedScopes != null)
				{
					using (IEnumerator<PolicyBindingConfig> enumerator = policyBindingSetConfig.AppliedScopes.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							PolicyBindingConfig instance2 = enumerator.Current;
							this.InternalDelete(instance2);
						}
						return;
					}
					goto IL_47;
				}
				return;
			}
			IL_47:
			throw new InvalidOperationException();
		}

		protected abstract void InternalPublishStatus(IEnumerable<UnifiedPolicyStatus> statusUpdateNotifications);

		protected virtual T InternalNewBlankConfigInstance<T>() where T : PolicyConfigBase
		{
			if (typeof(T).Equals(typeof(PolicyDefinitionConfig)))
			{
				return new PolicyDefinitionConfig() as T;
			}
			if (typeof(T).Equals(typeof(PolicyRuleConfig)))
			{
				return new PolicyRuleConfig() as T;
			}
			if (typeof(T).Equals(typeof(PolicyBindingConfig)))
			{
				return new PolicyBindingConfig() as T;
			}
			if (typeof(T).Equals(typeof(PolicyBindingSetConfig)))
			{
				return new PolicyBindingSetConfig() as T;
			}
			if (typeof(T).Equals(typeof(PolicyAssociationConfig)))
			{
				return new PolicyAssociationConfig() as T;
			}
			throw new InvalidOperationException();
		}

		protected void WrapKnownException(Action dowork)
		{
			try
			{
				dowork();
			}
			catch (Exception ex)
			{
				if (this.IsTransientException(ex))
				{
					bool flag = this.IsPerObjectException(ex);
					this.LogOneEntry(ExecutionLog.EventType.Warning, string.Format("Known transient exception, per object:{0}.", flag), ex);
					throw new PolicyConfigProviderTransientException(ex.Message, ex, flag, SyncAgentErrorCode.Generic);
				}
				if (this.IsPermanentException(ex))
				{
					bool flag2 = this.IsPerObjectException(ex);
					this.LogOneEntry(ExecutionLog.EventType.Warning, string.Format("Known permanent exception, per object:{0}.", flag2), ex);
					throw new PolicyConfigProviderPermanentException(ex.Message, ex, flag2, SyncAgentErrorCode.Generic);
				}
				this.LogOneEntry(ExecutionLog.EventType.Error, "Unhandled Exception", ex);
				throw;
			}
		}

		protected virtual bool IsPerObjectException(Exception exception)
		{
			return false;
		}

		protected virtual bool IsTransientException(Exception exception)
		{
			return false;
		}

		protected virtual bool IsPermanentException(Exception exception)
		{
			return false;
		}

		protected void OnPolicyConfigChanged(PolicyConfigChangeEventArgs e)
		{
			if (this.PolicyConfigChanged != null)
			{
				this.PolicyConfigChanged(this, e);
			}
		}

		private void ResetChangeTrackingForCollection<T>(IEnumerable<T> objects) where T : PolicyConfigBase
		{
			if (objects != null)
			{
				foreach (T t in objects)
				{
					t.ResetChangeTracking();
				}
			}
		}

		private readonly ExecutionLog logProvider;

		private bool isDisposed;
	}
}
