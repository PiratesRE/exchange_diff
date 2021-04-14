using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.MessageResubmission;
using Microsoft.Exchange.Transport.Pickup;
using Microsoft.Exchange.Transport.RecipientAPI;
using Microsoft.Exchange.Transport.RemoteDelivery;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Transport
{
	internal class ResourceManagerComponentsAdapter
	{
		public virtual RemoteDeliveryComponent RemoteDeliveryComponent
		{
			get
			{
				RemoteDeliveryComponent result;
				Components.TryGetRemoteDeliveryComponent(out result);
				return result;
			}
		}

		public virtual IsMemberOfResolverComponent<RoutingAddress> TransportIsMemberOfResolverComponent
		{
			get
			{
				IsMemberOfResolverComponent<RoutingAddress> result;
				Components.TryGetTransportIsMemberOfResolverComponent(out result);
				return result;
			}
		}

		public virtual bool IsActive
		{
			get
			{
				return Components.IsActive;
			}
		}

		public virtual bool IsPaused
		{
			get
			{
				return Components.IsPaused;
			}
		}

		public virtual bool IsBridgehead
		{
			get
			{
				return Components.IsBridgehead;
			}
		}

		public virtual object SyncRoot
		{
			get
			{
				return Components.SyncRoot;
			}
		}

		public virtual bool ShuttingDown
		{
			get
			{
				return Components.ShuttingDown;
			}
		}

		public virtual ISmtpInComponent SmtpInComponent
		{
			get
			{
				ISmtpInComponent result;
				Components.TryGetSmtpInComponent(out result);
				return result;
			}
		}

		public virtual IStartableTransportComponent Aggregator
		{
			get
			{
				IStartableTransportComponent result;
				Components.TryGetAggregator(out result);
				return result;
			}
		}

		public virtual EnhancedDns EnhancedDnsComponent
		{
			get
			{
				EnhancedDns result;
				Components.TryGetEnhancedDnsComponent(out result);
				return result;
			}
		}

		protected virtual IStartableTransportComponent PickupComponent
		{
			get
			{
				PickupComponent result;
				Components.TryGetPickupComponent(out result);
				return result;
			}
		}

		protected virtual IStartableTransportComponent StoreDriver
		{
			get
			{
				IStartableTransportComponent result;
				Components.TryGetStoreDriverLoaderComponent(out result);
				return result;
			}
		}

		protected virtual IStartableTransportComponent BootScanner
		{
			get
			{
				IStartableTransportComponent result;
				Components.TryGetBootScanner(out result);
				return result;
			}
		}

		protected virtual IStartableTransportComponent MessageResubmissionComponent
		{
			get
			{
				MessageResubmissionComponent result;
				Components.TryGetMessageResubmissionComponent(out result);
				return result;
			}
		}

		protected virtual IStartableTransportComponent ShadowRedundancyComponent
		{
			get
			{
				ShadowRedundancyComponent result;
				Components.TryGetShadowRedundancyComponent(out result);
				return result;
			}
		}

		public void AddDiagnosticInfo(XElement resourceManagerElement)
		{
			if (resourceManagerElement == null)
			{
				throw new ArgumentNullException("resourceManagerElement");
			}
			XElement xelement = new XElement("CurrentComponentStates");
			XElement xelement2 = new XElement("ResourcePressureRecomendedComponentStates");
			resourceManagerElement.Add(new object[]
			{
				new XElement("serviceActive", this.IsActive),
				new XElement("servicePaused", this.IsPaused),
				new XElement("serviceRole", this.IsBridgehead ? "Hub" : "Edge"),
				new XElement("shuttingDown", this.ShuttingDown),
				xelement2,
				xelement
			});
			foreach (object obj in Enum.GetValues(typeof(ComponentsState)))
			{
				ComponentsState componentsState = (ComponentsState)obj;
				if (ResourceManagerComponentsAdapter.IsSingleComponent(componentsState) && componentsState != ComponentsState.TransportServicePaused)
				{
					string diagnosticNameForComponentEnumFlag = ResourceManagerComponentsAdapter.GetDiagnosticNameForComponentEnumFlag(componentsState);
					xelement.Add(new XElement(diagnosticNameForComponentEnumFlag, ((int)this.currentComponentsState & (int)componentsState) == (int)componentsState));
					xelement2.Add(new XElement(diagnosticNameForComponentEnumFlag, ((int)this.requiredStateDueToBackPressure & (int)componentsState) == (int)componentsState));
				}
			}
		}

		public override string ToString()
		{
			ComponentsState componentsState = this.currentComponentsState;
			if (componentsState == ComponentsState.AllowAllComponents)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (ComponentsState componentsState2 = ComponentsState.AllowInboundMailSubmissionFromHubs; componentsState2 <= ComponentsState.AllowAllComponents; componentsState2 <<= 1)
			{
				if ((componentsState2 & ComponentsState.AllowAllComponents) != ComponentsState.None && (componentsState2 & componentsState) == ComponentsState.None)
				{
					ComponentsState componentsState3 = componentsState2;
					if (componentsState3 <= ComponentsState.AllowOutboundMailDeliveryToRemoteDomains)
					{
						if (componentsState3 <= ComponentsState.AllowInboundMailSubmissionFromPickupAndReplayDirectory)
						{
							if (componentsState3 < ComponentsState.AllowInboundMailSubmissionFromHubs)
							{
								goto IL_192;
							}
							switch ((int)(componentsState3 - ComponentsState.AllowInboundMailSubmissionFromHubs))
							{
							case 0:
								stringBuilder.AppendLine(Strings.InboundMailSubmissionFromHubsComponent);
								goto IL_1AD;
							case 1:
								stringBuilder.AppendLine(Strings.InboundMailSubmissionFromInternetComponent);
								goto IL_1AD;
							case 2:
								goto IL_192;
							case 3:
								stringBuilder.AppendLine(Strings.InboundMailSubmissionFromPickupDirectoryComponent);
								stringBuilder.AppendLine(Strings.InboundMailSubmissionFromReplayDirectoryComponent);
								goto IL_1AD;
							}
						}
						if (componentsState3 == ComponentsState.AllowInboundMailSubmissionFromMailbox)
						{
							stringBuilder.AppendLine(Strings.InboundMailSubmissionFromMailboxComponent);
							goto IL_1AD;
						}
						if (componentsState3 == ComponentsState.AllowOutboundMailDeliveryToRemoteDomains)
						{
							stringBuilder.AppendLine(Strings.OutboundMailDeliveryToRemoteDomainsComponent);
							goto IL_1AD;
						}
					}
					else if (componentsState3 <= ComponentsState.AllowContentAggregation)
					{
						if (componentsState3 == ComponentsState.AllowBootScannerRunning)
						{
							stringBuilder.AppendLine(Strings.BootScannerComponent);
							goto IL_1AD;
						}
						if (componentsState3 == ComponentsState.AllowContentAggregation)
						{
							if (this.Aggregator != null)
							{
								stringBuilder.AppendLine(Strings.ContentAggregationComponent);
								goto IL_1AD;
							}
							goto IL_1AD;
						}
					}
					else
					{
						if (componentsState3 == ComponentsState.AllowMessageRepositoryResubmission)
						{
							stringBuilder.AppendLine(Strings.MessageResubmissionComponentBanner);
							goto IL_1AD;
						}
						if (componentsState3 == ComponentsState.AllowShadowRedundancyResubmission)
						{
							stringBuilder.AppendLine(Strings.ShadowRedundancyComponentBanner);
							goto IL_1AD;
						}
					}
					IL_192:
					throw new ArgumentException("Unknown component with id = " + componentsState2.ToString());
				}
				IL_1AD:;
			}
			return stringBuilder.ToString();
		}

		internal bool UpdateComponentsState(ComponentsState requiredComponentState)
		{
			ComponentsState componentsState = requiredComponentState;
			if (this.IsPaused)
			{
				componentsState &= ~(ComponentsState.AllowInboundMailSubmissionFromHubs | ComponentsState.AllowInboundMailSubmissionFromInternet | ComponentsState.AllowInboundMailSubmissionFromPickupAndReplayDirectory | ComponentsState.AllowInboundMailSubmissionFromMailbox | ComponentsState.AllowContentAggregation);
				componentsState |= ComponentsState.TransportServicePaused;
			}
			ComponentsState componentsState2 = this.currentComponentsState ^ componentsState;
			if (componentsState2 == ComponentsState.None)
			{
				return false;
			}
			if (!this.ShuttingDown && this.IsActive)
			{
				lock (this.SyncRoot)
				{
					if (!this.ShuttingDown && this.IsActive)
					{
						this.requiredStateDueToBackPressure = requiredComponentState;
						return this.UpdateComponentsStateInternal(componentsState);
					}
				}
				return false;
			}
			return false;
		}

		private static bool IsSingleComponent(ComponentsState componentState)
		{
			return componentState != ComponentsState.None && (componentState & componentState - 1UL) == ComponentsState.None;
		}

		private static string GetDiagnosticNameForComponentEnumFlag(ComponentsState state)
		{
			if (state <= ComponentsState.AllowOutboundMailDeliveryToRemoteDomains)
			{
				if (state <= ComponentsState.AllowInboundMailSubmissionFromPickupAndReplayDirectory)
				{
					if (state < ComponentsState.AllowInboundMailSubmissionFromHubs)
					{
						goto IL_A0;
					}
					switch ((int)(state - ComponentsState.AllowInboundMailSubmissionFromHubs))
					{
					case 0:
						return "internalSmtpInEnabled";
					case 1:
						return "internetSmtpInEnabled";
					case 2:
						goto IL_A0;
					case 3:
						return "pickupEnabled";
					}
				}
				if (state == ComponentsState.AllowInboundMailSubmissionFromMailbox)
				{
					return "mailSubmissionEnabled";
				}
				if (state == ComponentsState.AllowOutboundMailDeliveryToRemoteDomains)
				{
					return "remoteDeliveryEnabled";
				}
			}
			else if (state <= ComponentsState.AllowContentAggregation)
			{
				if (state == ComponentsState.AllowBootScannerRunning)
				{
					return "bootScannerEnabled";
				}
				if (state == ComponentsState.AllowContentAggregation)
				{
					return "contentAggregationEnabled";
				}
			}
			else
			{
				if (state == ComponentsState.AllowMessageRepositoryResubmission)
				{
					return "messageRepositoryResubmissionEnabled";
				}
				if (state == ComponentsState.AllowShadowRedundancyResubmission)
				{
					return "shadowRedundancyResubmissionEnabled";
				}
			}
			IL_A0:
			return state.ToString();
		}

		private bool UpdateComponentsStateInternal(ComponentsState requiredComponentState)
		{
			ComponentsState componentsState = this.currentComponentsState ^ requiredComponentState;
			if (componentsState == ComponentsState.None)
			{
				return false;
			}
			this.Populate();
			ComponentsState componentsState2 = ComponentsState.AllowInboundMailSubmissionFromHubs | ComponentsState.AllowInboundMailSubmissionFromInternet | ComponentsState.TransportServicePaused;
			if ((componentsState & componentsState2) != ComponentsState.None)
			{
				ComponentsState componentsState3 = requiredComponentState & ComponentsState.AllowSmtpInComponentToRecvFromInternetAndHubs;
				if (componentsState3 == ComponentsState.AllowSmtpInComponentToRecvFromInternetAndHubs)
				{
					this.SmtpInComponent.Continue();
				}
				else if ((requiredComponentState & ComponentsState.TransportServicePaused) != ComponentsState.None)
				{
					this.SmtpInComponent.Pause();
				}
				else
				{
					bool rejectSubmits = (requiredComponentState & ComponentsState.AllowInboundMailSubmissionFromHubs) == ComponentsState.None;
					this.SmtpInComponent.Pause(rejectSubmits, SmtpResponse.InsufficientResource);
				}
				this.currentComponentsState = ((this.currentComponentsState & ~componentsState2) | (requiredComponentState & componentsState2));
				componentsState &= ~componentsState2;
			}
			ComponentsState componentsState4 = ComponentsState.AllowInboundMailSubmissionFromHubs;
			while (componentsState != ComponentsState.None && componentsState4 != ComponentsState.None)
			{
				if ((componentsState4 & componentsState) != ComponentsState.None)
				{
					IStartableTransportComponent startableTransportComponent;
					if (this.pausableComponents.TryGetValue(componentsState4, out startableTransportComponent))
					{
						if ((requiredComponentState & componentsState4) != ComponentsState.None)
						{
							startableTransportComponent.Continue();
							this.currentComponentsState |= componentsState4;
						}
						else
						{
							startableTransportComponent.Pause();
							this.currentComponentsState &= ~componentsState4;
						}
					}
					componentsState &= ~componentsState4;
				}
				componentsState4 <<= 1;
			}
			return true;
		}

		private void Populate()
		{
			if (!this.pausableComponentsLoaded)
			{
				this.pausableComponents[ComponentsState.AllowBootScannerRunning] = (this.BootScanner ?? new ResourceManagerComponentsAdapter.MockComponent());
				this.pausableComponents[ComponentsState.AllowContentAggregation] = (this.Aggregator ?? new ResourceManagerComponentsAdapter.MockComponent());
				this.pausableComponents[ComponentsState.AllowInboundMailSubmissionFromMailbox] = (this.StoreDriver ?? new ResourceManagerComponentsAdapter.MockComponent());
				this.pausableComponents[ComponentsState.AllowInboundMailSubmissionFromPickupAndReplayDirectory] = (this.PickupComponent ?? new ResourceManagerComponentsAdapter.MockComponent());
				this.pausableComponents[ComponentsState.AllowOutboundMailDeliveryToRemoteDomains] = (this.RemoteDeliveryComponent ?? new ResourceManagerComponentsAdapter.MockComponent());
				this.pausableComponents[ComponentsState.AllowMessageRepositoryResubmission] = (this.MessageResubmissionComponent ?? new ResourceManagerComponentsAdapter.MockComponent());
				this.pausableComponents[ComponentsState.AllowShadowRedundancyResubmission] = (this.ShadowRedundancyComponent ?? new ResourceManagerComponentsAdapter.MockComponent());
				this.pausableComponentsLoaded = true;
			}
		}

		private ComponentsState currentComponentsState;

		private ComponentsState requiredStateDueToBackPressure;

		private Dictionary<ComponentsState, IStartableTransportComponent> pausableComponents = new Dictionary<ComponentsState, IStartableTransportComponent>(7);

		private bool pausableComponentsLoaded;

		private class MockComponent : IStartableTransportComponent, ITransportComponent
		{
			public string CurrentState
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public void Start(bool initiallyPaused, ServiceState targetRunningState)
			{
				throw new NotImplementedException();
			}

			public void Stop()
			{
				throw new NotImplementedException();
			}

			public void Pause()
			{
			}

			public void Continue()
			{
			}

			public void Load()
			{
				throw new NotImplementedException();
			}

			public void Unload()
			{
				throw new NotImplementedException();
			}

			public string OnUnhandledException(Exception e)
			{
				throw new NotImplementedException();
			}
		}
	}
}
