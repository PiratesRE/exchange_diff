using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class AttachmentDataProviderManager
	{
		public bool UseMockAttachmentDataProvider
		{
			get
			{
				return this.useMockAttachmentDataProvider;
			}
		}

		public AttachmentDataProviderManager()
		{
			this.isOneDriveProProviderAvailable = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.OneDriveProProviderAvailable.Enabled;
			this.lockObject = new object();
			this.useMockAttachmentDataProvider = AttachmentDataProviderManager.IsMockDataProviderEnabled();
		}

		public static bool IsMockDataProviderEnabled()
		{
			return new BoolAppSettingsEntry("UseMockAttachmentDataProvider", false, ExTraceGlobals.AttachmentHandlingTracer).Value;
		}

		public AttachmentDataProvider GetDefaultUploadDataProvider(CallContext callContext)
		{
			if (this.defaultUploadDataProvider == null)
			{
				this.EnsureAttachmentDataProviders(callContext);
				this.defaultUploadDataProvider = this.dataProviders.Values.FirstOrDefault((AttachmentDataProvider x) => x.GetType() == typeof(OneDriveProAttachmentDataProvider));
			}
			return this.defaultUploadDataProvider;
		}

		public AttachmentDataProvider[] GetProviders(CallContext callContext)
		{
			return this.GetProviders(callContext, null);
		}

		public AttachmentDataProvider[] GetProviders(CallContext callContext, GetAttachmentDataProvidersRequest request)
		{
			this.EnsureAttachmentDataProviders(callContext);
			Dictionary<string, AttachmentDataProvider>.ValueCollection values = this.dataProviders.Values;
			foreach (AttachmentDataProvider attachmentDataProvider in values)
			{
				attachmentDataProvider.PostInitialize(request);
			}
			return values.ToArray<AttachmentDataProvider>();
		}

		public AttachmentDataProvider GetProvider(CallContext callContext, string id)
		{
			this.EnsureAttachmentDataProviders(callContext);
			if (this.dataProviders.ContainsKey(id))
			{
				return this.dataProviders[id];
			}
			ExTraceGlobals.DocumentsTracer.TraceDebug<string>((long)this.GetHashCode(), "Provider with id {0} was not found, getting the default upload provider", id);
			UserContext userContext = UserContextManager.GetUserContext(callContext.HttpContext, callContext.EffectiveCaller, true);
			OwaServerTraceLogger.AppendToLog(new TraceLogEvent("ADPM.GP", userContext, "GetProvider", string.Format("Provider with id {0} was not found", id)));
			return this.GetDefaultUploadDataProvider(callContext);
		}

		public AttachmentDataProvider AddProvider(CallContext callContext, AttachmentDataProvider attachmentDataProvider)
		{
			this.EnsureAttachmentDataProviders(callContext);
			attachmentDataProvider.Id = Guid.NewGuid().ToString();
			this.AddProviderInternal(callContext, new PolymorphicConfiguration<AttachmentDataProvider>(), attachmentDataProvider);
			return attachmentDataProvider;
		}

		public void RemoveProvider(string id)
		{
			throw new NotImplementedException();
		}

		public GetAttachmentDataProviderItemsResponse GetRecentItems(CallContext callContext)
		{
			GetAttachmentDataProviderItemsResponse getAttachmentDataProviderItemsResponse = new GetAttachmentDataProviderItemsResponse();
			try
			{
				this.EnsureAttachmentDataProviders(callContext);
				List<AttachmentDataProviderItem> list = new List<AttachmentDataProviderItem>();
				foreach (AttachmentDataProvider attachmentDataProvider in this.dataProviders.Values)
				{
					AttachmentDataProviderItem[] recentItems = attachmentDataProvider.GetRecentItems();
					if (recentItems != null)
					{
						list.AddRange(recentItems);
					}
				}
				getAttachmentDataProviderItemsResponse.Items = list.ToArray();
				getAttachmentDataProviderItemsResponse.TotalItemCount = getAttachmentDataProviderItemsResponse.Items.Count<AttachmentDataProviderItem>();
				getAttachmentDataProviderItemsResponse.ResultCode = AttachmentResultCode.Success;
			}
			catch (Exception)
			{
				getAttachmentDataProviderItemsResponse.ResultCode = AttachmentResultCode.GenericFailure;
			}
			return getAttachmentDataProviderItemsResponse;
		}

		public GetAttachmentDataProviderItemsResponse GetGroups(CallContext callContext, MailboxSession mailboxSession)
		{
			AttachmentDataProvider attachmentDataProvider = this.GetDefaultUploadDataProvider(callContext);
			GetAttachmentDataProviderItemsResponse getAttachmentDataProviderItemsResponse;
			if (attachmentDataProvider is OneDriveProAttachmentDataProvider)
			{
				OneDriveProAttachmentDataProvider oneDriveProAttachmentDataProvider = (OneDriveProAttachmentDataProvider)attachmentDataProvider;
				getAttachmentDataProviderItemsResponse = oneDriveProAttachmentDataProvider.GetGroups(mailboxSession);
			}
			else
			{
				getAttachmentDataProviderItemsResponse = new GetAttachmentDataProviderItemsResponse();
				getAttachmentDataProviderItemsResponse.Items = new AttachmentDataProviderItem[0];
				getAttachmentDataProviderItemsResponse.TotalItemCount = 0;
				getAttachmentDataProviderItemsResponse.ResultCode = AttachmentResultCode.GenericFailure;
			}
			return getAttachmentDataProviderItemsResponse;
		}

		private void EnsureAttachmentDataProviders(CallContext callContext)
		{
			if (this.dataProviders == null || this.dataProviders.Count == 0)
			{
				lock (this.lockObject)
				{
					if (this.dataProviders == null || this.dataProviders.Count == 0)
					{
						UserContext userContext = UserContextManager.GetUserContext(callContext.HttpContext, callContext.EffectiveCaller, true);
						PolymorphicConfiguration<AttachmentDataProvider> polymorphicConfiguration = null;
						if (!userContext.IsGroupUserContext)
						{
							polymorphicConfiguration = new PolymorphicConfiguration<AttachmentDataProvider>();
							try
							{
								polymorphicConfiguration.Load(callContext);
							}
							catch (TypeLoadException)
							{
							}
							this.dataProviders = polymorphicConfiguration.Entries.ToDictionary((AttachmentDataProvider x) => x.Id);
						}
						else
						{
							this.dataProviders = new Dictionary<string, AttachmentDataProvider>();
						}
						this.EnsureOneDriveProDataProvider(callContext, polymorphicConfiguration, userContext);
						IEnumerable<AttachmentDataProvider> enumerable = from x in this.dataProviders.Values
						where x is IAttachmentDataProviderChanged
						select x;
						foreach (AttachmentDataProvider attachmentDataProvider in enumerable)
						{
							((IAttachmentDataProviderChanged)attachmentDataProvider).AttachmentDataProviderChanged += this.ProviderChanged;
						}
					}
				}
			}
		}

		private void EnsureOneDriveProDataProvider(CallContext callContext, PolymorphicConfiguration<AttachmentDataProvider> attachmentDataProvidersConfig, UserContext userContext)
		{
			if (!this.useMockAttachmentDataProvider)
			{
				if (!this.isOneDriveProProviderAvailable)
				{
					return;
				}
				if (!userContext.IsBposUser)
				{
					return;
				}
			}
			if (!this.dataProviders.Values.Any((AttachmentDataProvider x) => x.GetType() == typeof(OneDriveProAttachmentDataProvider)))
			{
				lock (this.lockObject)
				{
					if (!this.dataProviders.Values.Any((AttachmentDataProvider x) => x.GetType() == typeof(OneDriveProAttachmentDataProvider)))
					{
						OneDriveProAttachmentDataProvider oneDriveProAttachmentDataProvider = OneDriveProAttachmentDataProvider.CreateFromBpos(userContext, callContext, this.useMockAttachmentDataProvider);
						if (oneDriveProAttachmentDataProvider != null)
						{
							this.AddProviderInternal(callContext, attachmentDataProvidersConfig, oneDriveProAttachmentDataProvider);
						}
					}
				}
			}
		}

		private void AddProviderInternal(CallContext callContext, PolymorphicConfiguration<AttachmentDataProvider> attachmentDataProvidersConfig, AttachmentDataProvider provider)
		{
			lock (this.lockObject)
			{
				this.dataProviders[provider.Id] = provider;
				if (attachmentDataProvidersConfig != null)
				{
					attachmentDataProvidersConfig.Entries.Add(provider);
					attachmentDataProvidersConfig.Save(callContext);
				}
			}
		}

		private void ProviderChanged(AttachmentDataProvider provider, AttachmentDataProviderChangedEventArgs args)
		{
			if (this.dataProviders.ContainsKey(provider.Id))
			{
				lock (this.lockObject)
				{
					if (this.dataProviders.ContainsKey(provider.Id))
					{
						PolymorphicConfiguration<AttachmentDataProvider> polymorphicConfiguration = new PolymorphicConfiguration<AttachmentDataProvider>();
						foreach (AttachmentDataProvider item in this.dataProviders.Values)
						{
							polymorphicConfiguration.Entries.Add(item);
							polymorphicConfiguration.Save(args.MailboxSession);
						}
					}
				}
			}
		}

		private readonly bool isOneDriveProProviderAvailable;

		private Dictionary<string, AttachmentDataProvider> dataProviders;

		private object lockObject;

		private AttachmentDataProvider defaultUploadDataProvider;

		private readonly bool useMockAttachmentDataProvider;
	}
}
