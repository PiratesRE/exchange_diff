using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class ADObjectFactory
	{
		internal static ADObject CreateAndInitializeObject<TResult>(ADPropertyBag propertyBag, IDirectorySession directorySession) where TResult : IConfigurable, new()
		{
			ArgumentValidator.ThrowIfNull("propertyBag", propertyBag);
			ADObject adobject = (ADObject)((object)((default(TResult) == null) ? Activator.CreateInstance<TResult>() : default(TResult)));
			adobject.m_Session = directorySession;
			adobject.propertyBag = propertyBag;
			adobject.Initialize();
			adobject.ResetChangeTracking(true);
			if (directorySession != null)
			{
				adobject.SetIsReadOnly(directorySession.ReadOnly);
			}
			return adobject;
		}

		internal static ADObject CreateAndInitializeRecipientObject<TRecipientObject>(ADPropertyBag propertyBag, ADRawEntry dummyObject, IRecipientSession recipientSession) where TRecipientObject : IConfigurable, new()
		{
			ArgumentValidator.ThrowIfNull("propertyBag", propertyBag);
			ArgumentValidator.ThrowIfNull("dummyObject", dummyObject);
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADObjectSchema.ObjectClass];
			ADObject adobject;
			if (dummyObject is OWAMiniRecipient)
			{
				adobject = new OWAMiniRecipient();
			}
			else if (dummyObject is ActiveSyncMiniRecipient)
			{
				adobject = new ActiveSyncMiniRecipient();
			}
			else if (dummyObject is StorageMiniRecipient)
			{
				adobject = new StorageMiniRecipient();
			}
			else if (dummyObject is TransportMiniRecipient)
			{
				adobject = new TransportMiniRecipient();
			}
			else if (dummyObject is LoadBalancingMiniRecipient)
			{
				adobject = new LoadBalancingMiniRecipient();
			}
			else if (dummyObject is MiniRecipientWithTokenGroups)
			{
				adobject = new MiniRecipientWithTokenGroups();
			}
			else if (dummyObject is FrontEndMiniRecipient)
			{
				adobject = new FrontEndMiniRecipient();
			}
			else if (dummyObject is MiniRecipient)
			{
				adobject = new MiniRecipient();
			}
			else if (dummyObject is RemovedMailbox)
			{
				adobject = new RemovedMailbox();
			}
			else if (dummyObject is DeletedRecipient)
			{
				adobject = new DeletedRecipient();
			}
			else if (multiValuedProperty.Contains(ADComputerRecipient.MostDerivedClass))
			{
				adobject = new ADComputerRecipient();
			}
			else if (multiValuedProperty.Contains(ADUser.MostDerivedClass))
			{
				adobject = new ADUser();
			}
			else if (multiValuedProperty.Contains(ADContact.MostDerivedClass))
			{
				adobject = new ADContact();
			}
			else if (multiValuedProperty.Contains(ADGroup.MostDerivedClass))
			{
				adobject = new ADGroup();
			}
			else if (multiValuedProperty.Contains(ADDynamicGroup.MostDerivedClass))
			{
				adobject = new ADDynamicGroup();
			}
			else if (multiValuedProperty.Contains(ADPublicFolder.MostDerivedClass))
			{
				adobject = new ADPublicFolder();
			}
			else if (multiValuedProperty.Contains(ADSystemAttendantMailbox.MostDerivedClass))
			{
				adobject = new ADSystemAttendantMailbox();
			}
			else if (multiValuedProperty.Contains(ADSystemMailbox.MostDerivedClass))
			{
				adobject = new ADSystemMailbox();
			}
			else if (multiValuedProperty.Contains(ADPublicDatabase.MostDerivedClass))
			{
				adobject = new ADPublicDatabase();
			}
			else
			{
				if (!multiValuedProperty.Contains(ADMicrosoftExchangeRecipient.MostDerivedClass))
				{
					string objectClass = string.Empty;
					foreach (string text in multiValuedProperty)
					{
						objectClass = text;
					}
					ObjectValidationError error = new ObjectValidationError(DirectoryStrings.UnsupportedObjectClass(objectClass), (ADObjectId)propertyBag[ADObjectSchema.Id], string.Empty);
					ADProviderPerf.UpdateProcessCounter(Counter.ProcessRateCriticalValidationFailures, UpdateType.Update, 1U);
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_VALIDATION_FAILED_FCO_MODE_RECIPIENT, ((ADObjectId)propertyBag[ADObjectSchema.Id]).ToString(), new object[]
					{
						((ADObjectId)propertyBag[ADObjectSchema.Id]).ToDNString()
					});
					throw new DataValidationException(error);
				}
				adobject = new ADMicrosoftExchangeRecipient();
			}
			adobject.m_Session = recipientSession;
			adobject.propertyBag = propertyBag;
			adobject.Initialize();
			adobject.ResetChangeTracking(true);
			if (recipientSession != null)
			{
				adobject.SetIsReadOnly(recipientSession.ReadOnly);
			}
			ExTraceGlobals.ADReadDetailsTracer.TraceDebug<string, RecipientType>((long)((recipientSession != null) ? recipientSession.GetHashCode() : 0), "ADRecipientObjectSession::CreateObject - Got {0} as {1}", adobject.DistinguishedName, (RecipientType)adobject[ADRecipientSchema.RecipientType]);
			return adobject;
		}

		internal static ADObject CreateAndInitializeConfigObject<TConfigObject>(ADPropertyBag propertyBag, ADRawEntry dummyObject, IDirectorySession directorySession) where TConfigObject : IConfigurable, new()
		{
			ArgumentValidator.ThrowIfNull("propertyBag", propertyBag);
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADObjectSchema.ObjectClass];
			ADObject adobject;
			if (dummyObject is MiniTopologyServer)
			{
				adobject = new MiniTopologyServer();
			}
			else if (dummyObject is MiniVirtualDirectory)
			{
				adobject = new MiniVirtualDirectory();
			}
			else if (dummyObject is MiniEmailTransport)
			{
				adobject = new MiniEmailTransport();
			}
			else if (dummyObject is MiniReceiveConnector)
			{
				adobject = new MiniReceiveConnector();
			}
			else if (multiValuedProperty.Contains(ADWebServicesVirtualDirectory.MostDerivedClass))
			{
				adobject = new ADWebServicesVirtualDirectory();
			}
			else if (multiValuedProperty.Contains(ADE12UMVirtualDirectory.MostDerivedClass))
			{
				adobject = new ADE12UMVirtualDirectory();
			}
			else if (multiValuedProperty.Contains("msExchMobileVirtualDirectory"))
			{
				adobject = new ADMobileVirtualDirectory();
			}
			else if (multiValuedProperty.Contains(ADOwaVirtualDirectory.MostDerivedClass))
			{
				adobject = new ADOwaVirtualDirectory();
			}
			else if (multiValuedProperty.Contains(ADRpcHttpVirtualDirectory.MostDerivedClass))
			{
				adobject = new ADRpcHttpVirtualDirectory();
			}
			else if (multiValuedProperty.Contains(ADMapiVirtualDirectory.MostDerivedClass))
			{
				adobject = new ADMapiVirtualDirectory();
			}
			else if (multiValuedProperty.Contains(ADAvailabilityForeignConnectorVirtualDirectory.MostDerivedClass))
			{
				adobject = new ADAvailabilityForeignConnectorVirtualDirectory();
			}
			else if (multiValuedProperty.Contains(ADOabVirtualDirectory.MostDerivedClass))
			{
				adobject = new ADOabVirtualDirectory();
			}
			else if (multiValuedProperty.Contains(ADEcpVirtualDirectory.MostDerivedClass))
			{
				adobject = new ADEcpVirtualDirectory();
			}
			else if (multiValuedProperty.Contains(Pop3AdConfiguration.MostDerivedClass))
			{
				adobject = new Pop3AdConfiguration();
			}
			else if (multiValuedProperty.Contains(Imap4AdConfiguration.MostDerivedClass))
			{
				adobject = new Imap4AdConfiguration();
			}
			else if (multiValuedProperty.Contains("mailGateway"))
			{
				if (multiValuedProperty.Contains(SmtpSendConnectorConfig.MostDerivedClass))
				{
					adobject = new SmtpSendConnectorConfig();
				}
				else if (multiValuedProperty.Contains(DeliveryAgentConnector.MostDerivedClass))
				{
					adobject = new DeliveryAgentConnector();
				}
				else if (propertyBag.Contains(ForeignConnectorSchema.DropDirectory))
				{
					adobject = new ForeignConnector();
				}
				else
				{
					adobject = new LegacyGatewayConnector();
				}
			}
			else if (multiValuedProperty.Contains("msExchEdgeSyncEhfConnector"))
			{
				adobject = new EdgeSyncEhfConnector();
			}
			else
			{
				if (!multiValuedProperty.Contains("msExchEdgeSyncMservConnector"))
				{
					return ADObjectFactory.CreateAndInitializeObject<TConfigObject>(propertyBag, directorySession);
				}
				adobject = new EdgeSyncMservConnector();
			}
			adobject.m_Session = directorySession;
			adobject.propertyBag = propertyBag;
			adobject.Initialize();
			adobject.ResetChangeTracking(true);
			if (directorySession != null)
			{
				adobject.SetIsReadOnly(directorySession.ReadOnly);
			}
			return adobject;
		}

		[Conditional("DEBUG")]
		private static void Dbg_CheckIsTypeAssignableFrom<TObject>(params Type[] types)
		{
			ArgumentValidator.ThrowIfNull("types", types);
			foreach (Type type in types)
			{
				if (typeof(TObject).Equals(type))
				{
					break;
				}
				if (typeof(TObject).IsSubclassOf(type))
				{
					break;
				}
			}
		}
	}
}
