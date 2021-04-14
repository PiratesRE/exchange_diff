using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.VariantConfiguration.Reflection;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SettingOverrideSync : IDiagnosable
	{
		private SettingOverrideSync()
		{
		}

		public event EventHandler<RefreshCompletedEventArgs> RefreshCompleted;

		public event EventHandler<RefreshFailedEventArgs> RefreshFailed;

		public static SettingOverrideSync Instance
		{
			get
			{
				return SettingOverrideSync.instance;
			}
		}

		public void Start(bool refreshNow = true)
		{
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).VariantConfig.SettingOverrideSync.Enabled && this.cache == null)
			{
				lock (this.instanceLock)
				{
					if (this.cache == null)
					{
						this.session = DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 341, "Start", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationSettings\\SettingOverrideSync.cs");
						this.cache = new ADObjectCache<SettingOverride, ConfigurationSettingsException>(new Func<SettingOverride[], SettingOverride[]>(this.Load), null);
						VariantConfiguration.UpdateCommitted += this.OnUpdateCommitted;
						using (ManualResetEvent waitForUpdate = new ManualResetEvent(false))
						{
							EventHandler<RefreshCompletedEventArgs> value = delegate(object sender, RefreshCompletedEventArgs args)
							{
								try
								{
									waitForUpdate.Set();
								}
								catch (ObjectDisposedException)
								{
								}
							};
							EventHandler<RefreshFailedEventArgs> value2 = delegate(object sender, RefreshFailedEventArgs args)
							{
								try
								{
									waitForUpdate.Set();
								}
								catch (ObjectDisposedException)
								{
								}
							};
							this.RefreshCompleted += value;
							this.RefreshFailed += value2;
							this.cache.Initialize(SettingOverrideSync.instance.RefreshInterval, refreshNow);
							VariantConfigurationOverride[] overrides = VariantConfiguration.Overrides;
							if (overrides != null && overrides.Length > 0 && refreshNow)
							{
								waitForUpdate.WaitOne(SettingOverrideSync.WaitForUpdateTimeout);
							}
							this.RefreshCompleted -= value;
							this.RefreshFailed -= value2;
						}
					}
				}
			}
		}

		public void Stop()
		{
			lock (this.instanceLock)
			{
				if (this.cache != null && this.cache.IsInitialized)
				{
					VariantConfiguration.UpdateCommitted -= this.OnUpdateCommitted;
					this.cache.Dispose();
					this.cache = null;
				}
			}
		}

		public void Refresh()
		{
			ADObjectCache<SettingOverride, ConfigurationSettingsException> adobjectCache = null;
			lock (this.instanceLock)
			{
				if (this.cache != null && this.cache.IsInitialized)
				{
					adobjectCache = this.cache;
				}
			}
			if (adobjectCache != null)
			{
				adobjectCache.Refresh(null);
				return;
			}
			throw new InvalidOperationException("Setting override sync is not started.");
		}

		public string GetDiagnosticComponentName()
		{
			return typeof(VariantConfiguration).Name;
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			SettingOverrideSync.SettingOverrideDiagnosableArgument settingOverrideDiagnosableArgument = new SettingOverrideSync.SettingOverrideDiagnosableArgument(parameters);
			XElement xelement = new XElement(this.GetDiagnosticComponentName());
			if (settingOverrideDiagnosableArgument.HasArgument("refresh"))
			{
				this.RefreshAndWait();
			}
			if (settingOverrideDiagnosableArgument.HasArgument("errors"))
			{
				xelement.Add(this.GetErrorsDiagnosticInfo());
			}
			else if (settingOverrideDiagnosableArgument.HasArgument("overrides"))
			{
				xelement.Add(this.GetOverridesDiagnosticInfo());
			}
			else if (settingOverrideDiagnosableArgument.HasArgument("refresh"))
			{
				xelement.Add(this.GetOverridesDiagnosticInfo());
			}
			else if (settingOverrideDiagnosableArgument.HasArgument("config"))
			{
				string content = null;
				VariantConfigurationSnapshot diagnosticInfoSnapshot = this.GetDiagnosticInfoSnapshot(settingOverrideDiagnosableArgument, out content);
				if (diagnosticInfoSnapshot != null)
				{
					xelement.Add(this.GetConstraintDiagnosticInfo(diagnosticInfoSnapshot));
					xelement.Add(this.GetConfigurationDiagnosticInfo(diagnosticInfoSnapshot));
				}
				else
				{
					xelement.Add(new XElement("Error", content));
				}
			}
			else
			{
				xelement.Add(new XElement("Help", "Allowed arguments: " + string.Join(", ", settingOverrideDiagnosableArgument.ArgumentSchema.Keys) + "."));
			}
			return xelement;
		}

		private void RefreshAndWait()
		{
			using (ManualResetEvent waitForUpdate = new ManualResetEvent(false))
			{
				EventHandler<RefreshCompletedEventArgs> value = delegate(object sender, RefreshCompletedEventArgs args)
				{
					try
					{
						waitForUpdate.Set();
					}
					catch (ObjectDisposedException)
					{
					}
				};
				EventHandler<RefreshFailedEventArgs> value2 = delegate(object sender, RefreshFailedEventArgs args)
				{
					try
					{
						waitForUpdate.Set();
					}
					catch (ObjectDisposedException)
					{
					}
				};
				this.RefreshCompleted += value;
				this.RefreshFailed += value2;
				this.Refresh();
				waitForUpdate.WaitOne(SettingOverrideSync.WaitForUpdateTimeout);
			}
		}

		private VariantConfigurationSnapshot GetDiagnosticInfoSnapshot(SettingOverrideSync.SettingOverrideDiagnosableArgument argument, out string error)
		{
			IConstraintProvider constraintProvider = MachineSettingsContext.Local;
			if (argument.HasArgument("user"))
			{
				string userId = argument.GetArgument<string>("user");
				if (!string.IsNullOrWhiteSpace(userId))
				{
					string orgId = null;
					if (argument.HasArgument("org"))
					{
						orgId = argument.GetArgument<string>("org");
					}
					if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled && string.IsNullOrEmpty(orgId))
					{
						error = "Org is required.";
						return null;
					}
					ADUser user = null;
					ADNotificationAdapter.TryRunADOperation(delegate()
					{
						user = new SettingOverrideSync.UserResolver(userId, orgId).Resolve();
					}, 3);
					if (user == null)
					{
						error = "User not found.";
						return null;
					}
					constraintProvider = user.GetContext(null);
				}
			}
			error = null;
			return VariantConfiguration.GetSnapshot(constraintProvider, null, null);
		}

		private XElement GetErrorsDiagnosticInfo()
		{
			XElement xelement = new XElement("Errors");
			lock (this.errors)
			{
				foreach (SettingOverrideSync.DiagnosticsError diagnosticsError in this.errors)
				{
					xelement.Add(diagnosticsError.GetDiagnosticInfo());
				}
			}
			return xelement;
		}

		private XElement GetOverridesDiagnosticInfo()
		{
			XElement xelement = new XElement("Overrides", new XAttribute("Updated", (this.lastUpdate != null) ? this.lastUpdate.ToString() : string.Empty));
			IEnumerable<SettingOverrideSync.SettingOverrideDiagnosticInfo> enumerable = this.overridesInfo;
			if (enumerable != null)
			{
				foreach (SettingOverrideSync.SettingOverrideDiagnosticInfo settingOverrideDiagnosticInfo in enumerable)
				{
					xelement.Add(settingOverrideDiagnosticInfo.GetDiagnosticInfo());
				}
			}
			return xelement;
		}

		private XElement GetConstraintDiagnosticInfo(VariantConfigurationSnapshot snapshot)
		{
			XElement xelement = new XElement("Constraints");
			foreach (KeyValuePair<string, string> keyValuePair in snapshot.Constraints)
			{
				XElement content = new XElement("Constraint", new object[]
				{
					keyValuePair.Value,
					new XAttribute("Name", keyValuePair.Key)
				});
				xelement.Add(content);
			}
			return xelement;
		}

		private XElement GetConfigurationDiagnosticInfo(VariantConfigurationSnapshot snapshot)
		{
			XElement xelement = new XElement("Configuration");
			bool enabled = snapshot.VariantConfig.InternalAccess.Enabled;
			foreach (string text in VariantConfiguration.Settings.GetComponents(enabled))
			{
				XElement xelement2 = new XElement(text);
				xelement.Add(xelement2);
				VariantConfigurationComponent variantConfigurationComponent = VariantConfiguration.Settings[text];
				foreach (string text2 in variantConfigurationComponent.GetSections(enabled))
				{
					XElement xelement3 = new XElement(text2);
					xelement2.Add(xelement3);
					ISettings @object = snapshot.GetObject<ISettings>(Path.GetFileName(variantConfigurationComponent.FileName), text2);
					foreach (Type type in new List<Type>(variantConfigurationComponent[text2].Type.GetInterfaces())
					{
						variantConfigurationComponent[text2].Type
					})
					{
						foreach (PropertyInfo propertyInfo in type.GetProperties())
						{
							if (!propertyInfo.Name.Equals("Name"))
							{
								XElement content = new XElement(propertyInfo.Name, this.GetConfigurationDiagnosticValueString(propertyInfo.GetValue(@object)));
								xelement3.Add(content);
							}
						}
					}
				}
			}
			return xelement;
		}

		private string GetConfigurationDiagnosticValueString(object value)
		{
			if (value == null)
			{
				return string.Empty;
			}
			if (value is IEnumerable<object>)
			{
				return string.Join<object>(",", (IEnumerable<object>)value);
			}
			return value.ToString();
		}

		private void OnUpdateCommitted(object sender, UpdateCommittedEventArgs args)
		{
			this.lastUpdate = new ExDateTime?(ExDateTime.UtcNow);
			EventHandler<RefreshCompletedEventArgs> refreshCompleted = this.RefreshCompleted;
			if (refreshCompleted != null)
			{
				refreshCompleted(this, new RefreshCompletedEventArgs(true, VariantConfiguration.Overrides));
			}
		}

		private TimeSpan RefreshInterval
		{
			get
			{
				TimeSpan result;
				try
				{
					result = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).VariantConfig.SettingOverrideSync.RefreshInterval;
				}
				catch (KeyNotFoundException)
				{
					result = SettingOverrideSync.DefaultRefreshInterval;
				}
				return result;
			}
		}

		private SettingOverride[] Load(SettingOverride[] existingValue)
		{
			try
			{
				IEnumerable<SettingOverrideSync.SettingOverrideDiagnosticInfo> source = this.ReadOverrides();
				VariantConfigurationOverride[] overrides = (from o in source
				where o.Status == SettingOverrideSync.OverrideStatus.Accepted
				select o.Override.GetVariantConfigurationOverride()).ToArray<VariantConfigurationOverride>();
				this.overridesInfo = source;
				if (VariantConfiguration.SetOverrides(overrides))
				{
					if (this.cache.RefreshInterval != this.RefreshInterval)
					{
						this.cache.SetRefreshInterval(this.RefreshInterval);
					}
				}
				else
				{
					EventHandler<RefreshCompletedEventArgs> refreshCompleted = this.RefreshCompleted;
					if (refreshCompleted != null)
					{
						refreshCompleted(this, new RefreshCompletedEventArgs(false, VariantConfiguration.Overrides));
					}
				}
				this.HandleLoadSuccess(overrides);
			}
			catch (ConfigurationSettingsADConfigDriverException e)
			{
				this.HandleLoadException(e);
				throw;
			}
			return null;
		}

		private IEnumerable<SettingOverrideSync.SettingOverrideDiagnosticInfo> ReadOverrides()
		{
			ADOperationResult adoperationResult = null;
			List<SettingOverride> list = new List<SettingOverride>();
			List<SettingOverrideSync.SettingOverrideDiagnosticInfo> list2 = new List<SettingOverrideSync.SettingOverrideDiagnosticInfo>();
			try
			{
				bool[] array = new bool[]
				{
					default(bool),
					true
				};
				for (int i = 0; i < array.Length; i++)
				{
					SettingOverrideSync.<>c__DisplayClass1a CS$<>8__locals1 = new SettingOverrideSync.<>c__DisplayClass1a();
					CS$<>8__locals1.isFlight = array[i];
					SettingOverride[] adOverridesSubset = null;
					adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						adOverridesSubset = this.session.Find<SettingOverride>(SettingOverride.GetContainerId(CS$<>8__locals1.isFlight), QueryScope.OneLevel, null, null, int.MaxValue);
					}, 3);
					if (adOverridesSubset != null)
					{
						list.AddRange(adOverridesSubset);
					}
				}
			}
			catch (LocalizedException innerException)
			{
				throw new ConfigurationSettingsADConfigDriverException(innerException);
			}
			catch (InvalidOperationException innerException2)
			{
				throw new ConfigurationSettingsADConfigDriverException(innerException2);
			}
			if (!adoperationResult.Succeeded)
			{
				throw new ConfigurationSettingsADNotificationException(adoperationResult.Exception);
			}
			list.Sort(new Comparison<SettingOverride>(SettingOverrideSync.Compare));
			List<SettingOverrideException> list3 = new List<SettingOverrideException>();
			foreach (SettingOverride settingOverride in list)
			{
				if (!settingOverride.Applies)
				{
					list2.Add(new SettingOverrideSync.SettingOverrideDiagnosticInfo(settingOverride, SettingOverrideSync.OverrideStatus.NotApplicable, null));
				}
				else
				{
					try
					{
						SettingOverride.Validate(settingOverride.GetVariantConfigurationOverride(), true);
					}
					catch (SettingOverrideException ex)
					{
						list3.Add(ex);
						list2.Add(new SettingOverrideSync.SettingOverrideDiagnosticInfo(settingOverride, SettingOverrideSync.OverrideStatus.Invalid, ex));
						continue;
					}
					list2.Add(new SettingOverrideSync.SettingOverrideDiagnosticInfo(settingOverride, SettingOverrideSync.OverrideStatus.Accepted, null));
				}
			}
			this.HandleSettingOverrideException(list3);
			return list2;
		}

		private static int Compare(SettingOverride override1, SettingOverride override2)
		{
			if (override1.WhenCreated == null && override2.WhenCreated != null)
			{
				return 1;
			}
			if (override1.WhenCreated != null && override2.WhenCreated == null)
			{
				return -1;
			}
			if (override1.WhenCreated != null && override2.WhenCreated != null)
			{
				if (override1.WhenCreated < override2.WhenCreated)
				{
					return 1;
				}
				if (override1.WhenCreated > override2.WhenCreated)
				{
					return -1;
				}
			}
			return string.Compare(override1.Id.Name, override2.Id.Name, true);
		}

		private void HandleLoadException(Exception e)
		{
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_ConfigurationSettingsLoadError, base.GetType().Name, new object[]
			{
				e.ToString()
			});
			lock (this.errors)
			{
				while (this.errors.Count >= 50)
				{
					this.errors.Dequeue();
				}
				this.errors.Enqueue(new SettingOverrideSync.DiagnosticsError(e));
			}
			EventHandler<RefreshFailedEventArgs> refreshFailed = this.RefreshFailed;
			if (refreshFailed != null)
			{
				refreshFailed(this, new RefreshFailedEventArgs(e));
			}
		}

		private void HandleLoadSuccess(VariantConfigurationOverride[] overrides)
		{
			lock (this.errors)
			{
				this.errors.Clear();
			}
		}

		private void HandleSettingOverrideException(ICollection<SettingOverrideException> exceptions)
		{
			if (exceptions != null && exceptions.Count > 0)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_SettingOverrideValidationError, base.GetType().Name, new object[]
				{
					string.Join<SettingOverrideException>(Environment.NewLine, exceptions)
				});
			}
		}

		private const int MaxErrorHistory = 50;

		private static readonly TimeSpan DefaultRefreshInterval = TimeSpan.FromMinutes(15.0);

		private static readonly TimeSpan WaitForUpdateTimeout = TimeSpan.FromSeconds(5.0);

		private readonly object instanceLock = new object();

		private static SettingOverrideSync instance = new SettingOverrideSync();

		private ADObjectCache<SettingOverride, ConfigurationSettingsException> cache;

		private ExDateTime? lastUpdate = null;

		private IConfigurationSession session;

		private Queue<SettingOverrideSync.DiagnosticsError> errors = new Queue<SettingOverrideSync.DiagnosticsError>();

		private IEnumerable<SettingOverrideSync.SettingOverrideDiagnosticInfo> overridesInfo;

		private enum OverrideStatus
		{
			NotApplicable,
			Invalid,
			Accepted
		}

		private sealed class SettingOverrideDiagnosticInfo
		{
			public SettingOverrideDiagnosticInfo(SettingOverride o, SettingOverrideSync.OverrideStatus status, SettingOverrideException e)
			{
				this.Override = o;
				this.Status = status;
				this.ValidationException = e;
			}

			public SettingOverride Override { get; private set; }

			public SettingOverrideSync.OverrideStatus Status { get; private set; }

			public SettingOverrideException ValidationException { get; private set; }

			private string Message
			{
				get
				{
					switch (this.Status)
					{
					case SettingOverrideSync.OverrideStatus.NotApplicable:
						return string.Concat(new object[]
						{
							"Either this server version '",
							SettingOverrideXml.CurrentVersion,
							"' or its name '",
							Environment.MachineName,
							"' does not match criteria defined in the override."
						});
					case SettingOverrideSync.OverrideStatus.Invalid:
						if (this.ValidationException == null)
						{
							return "Unknown error during validation.";
						}
						return this.ValidationException.Message;
					case SettingOverrideSync.OverrideStatus.Accepted:
						return "This override has been accepted as applicable to this server.";
					default:
						return string.Empty;
					}
				}
			}

			public XElement GetDiagnosticInfo()
			{
				XElement xelement = new XElement((!string.IsNullOrEmpty(this.Override.FlightName)) ? "FlightOverride" : "SettingOverride");
				this.Add(xelement, "Name", this.Override.Name);
				this.Add(xelement, "Reason", this.Override.Reason);
				this.Add(xelement, "ModifiedBy", this.Override.ModifiedBy);
				this.Add(xelement, "FlightName", this.Override.FlightName);
				this.Add(xelement, "ComponentName", this.Override.ComponentName);
				this.Add(xelement, "SectionName", this.Override.SectionName);
				this.Add(xelement, "Status", this.Status.ToString());
				this.Add(xelement, "Message", this.Message);
				XElement xelement2 = new XElement("Parameters");
				xelement.Add(xelement2);
				foreach (string value in this.Override.Parameters)
				{
					this.Add(xelement2, "Parameter", value);
				}
				return xelement;
			}

			private void Add(XElement parent, string element, string value)
			{
				if (value != null)
				{
					parent.Add(new XElement(element, value));
				}
			}

			private void Add(XElement parent, string element, bool value)
			{
				this.Add(parent, element, value ? bool.TrueString : bool.FalseString);
			}
		}

		private class DiagnosticsError
		{
			public DiagnosticsError(Exception e)
			{
				if (e == null)
				{
					throw new ArgumentNullException("e");
				}
				this.Exception = e;
				this.RaisedAt = DateTime.UtcNow;
			}

			public Exception Exception { get; private set; }

			public DateTime RaisedAt { get; private set; }

			public XElement GetDiagnosticInfo()
			{
				return new XElement("Error", new object[]
				{
					new XAttribute("RaisedAt", this.RaisedAt),
					new XElement("Exception", this.Exception)
				});
			}
		}

		private sealed class SettingOverrideDiagnosableArgument : DiagnosableArgument
		{
			public SettingOverrideDiagnosableArgument(DiagnosableParameters parameters)
			{
				base.Initialize(parameters);
			}

			public new Dictionary<string, Type> ArgumentSchema
			{
				get
				{
					return base.ArgumentSchema;
				}
			}

			protected override void InitializeSchema(Dictionary<string, Type> schema)
			{
				schema["errors"] = typeof(bool);
				schema["overrides"] = typeof(bool);
				schema["refresh"] = typeof(bool);
				schema["config"] = typeof(bool);
				schema["user"] = typeof(string);
				schema["org"] = typeof(string);
			}

			public const string ErrorHistory = "errors";

			public const string CurrentOverrides = "overrides";

			public const string Refresh = "refresh";

			public const string Configuration = "config";

			public const string User = "user";

			public const string Organization = "org";
		}

		private class UserResolver
		{
			public UserResolver(string userId, string orgId)
			{
				this.userId = userId;
				ADSessionSettings sessionSettings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(orgId);
				this.session = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 1127, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationSettings\\SettingOverrideSync.cs");
			}

			public ADUser Resolve()
			{
				Func<ADUser>[] array = new Func<ADUser>[]
				{
					new Func<ADUser>(this.TryAccountName),
					new Func<ADUser>(this.TryObjectId),
					new Func<ADUser>(this.TryExternalDirectoryId),
					new Func<ADUser>(this.TryGuid),
					new Func<ADUser>(this.TryLegacyDN),
					new Func<ADUser>(this.TryProxyAddress),
					new Func<ADUser>(this.TrySid)
				};
				foreach (Func<ADUser> func in array)
				{
					ADUser aduser = func();
					if (aduser != null)
					{
						return aduser;
					}
				}
				return null;
			}

			private ADUser TryAccountName()
			{
				IEnumerable<ADUser> enumerable = this.session.FindByAccountName<ADUser>(null, this.userId, null, null);
				if (enumerable == null)
				{
					return null;
				}
				return enumerable.FirstOrDefault<ADUser>();
			}

			private ADUser TryObjectId()
			{
				ADObjectId adObjectId = null;
				if (ADObjectId.TryParseDnOrGuid(this.userId, out adObjectId))
				{
					return this.session.FindADUserByObjectId(adObjectId);
				}
				return null;
			}

			private ADUser TryExternalDirectoryId()
			{
				return this.session.FindADUserByExternalDirectoryObjectId(this.userId);
			}

			private ADUser TryGuid()
			{
				Guid guid;
				if (!Guid.TryParse(this.userId, out guid))
				{
					return null;
				}
				ADUser aduser = this.ConvertRecipientToUser(this.session.FindByExchangeGuidIncludingAlternate(guid));
				if (aduser == null)
				{
					aduser = this.ConvertRecipientToUser(this.session.FindByExchangeObjectId(guid));
				}
				if (aduser == null)
				{
					aduser = this.ConvertRecipientToUser(this.session.FindByObjectGuid(guid));
				}
				return aduser;
			}

			private ADUser TryLegacyDN()
			{
				return this.ConvertRecipientToUser(this.session.FindByLegacyExchangeDN(this.userId));
			}

			private ADUser TryProxyAddress()
			{
				ProxyAddress proxyAddress;
				if (ProxyAddress.TryParse(this.userId, out proxyAddress))
				{
					return this.ConvertRecipientToUser(this.session.FindByProxyAddress(proxyAddress));
				}
				return null;
			}

			private ADUser TrySid()
			{
				try
				{
					SecurityIdentifier sId = new SecurityIdentifier(this.userId);
					return this.ConvertRecipientToUser(this.session.FindBySid(sId));
				}
				catch
				{
				}
				return null;
			}

			private ADUser ConvertRecipientToUser(IEnumerable<ADRecipient> recipient)
			{
				if (recipient == null)
				{
					return null;
				}
				return this.ConvertRecipientToUser(recipient.FirstOrDefault<ADRecipient>());
			}

			private ADUser ConvertRecipientToUser(ADRecipient recipient)
			{
				if (recipient == null)
				{
					return null;
				}
				return this.session.FindADUserByObjectId(recipient.Id);
			}

			private string userId;

			private IRecipientSession session;
		}
	}
}
