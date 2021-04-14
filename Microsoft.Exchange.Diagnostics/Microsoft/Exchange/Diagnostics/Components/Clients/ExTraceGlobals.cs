using System;

namespace Microsoft.Exchange.Diagnostics.Components.Clients
{
	public static class ExTraceGlobals
	{
		public static Trace CoreTracer
		{
			get
			{
				if (ExTraceGlobals.coreTracer == null)
				{
					ExTraceGlobals.coreTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.coreTracer;
			}
		}

		public static Trace CoreCallTracer
		{
			get
			{
				if (ExTraceGlobals.coreCallTracer == null)
				{
					ExTraceGlobals.coreCallTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.coreCallTracer;
			}
		}

		public static Trace CoreDataTracer
		{
			get
			{
				if (ExTraceGlobals.coreDataTracer == null)
				{
					ExTraceGlobals.coreDataTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.coreDataTracer;
			}
		}

		public static Trace UserContextTracer
		{
			get
			{
				if (ExTraceGlobals.userContextTracer == null)
				{
					ExTraceGlobals.userContextTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.userContextTracer;
			}
		}

		public static Trace UserContextCallTracer
		{
			get
			{
				if (ExTraceGlobals.userContextCallTracer == null)
				{
					ExTraceGlobals.userContextCallTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.userContextCallTracer;
			}
		}

		public static Trace UserContextDataTracer
		{
			get
			{
				if (ExTraceGlobals.userContextDataTracer == null)
				{
					ExTraceGlobals.userContextDataTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.userContextDataTracer;
			}
		}

		public static Trace OehTracer
		{
			get
			{
				if (ExTraceGlobals.oehTracer == null)
				{
					ExTraceGlobals.oehTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.oehTracer;
			}
		}

		public static Trace OehCallTracer
		{
			get
			{
				if (ExTraceGlobals.oehCallTracer == null)
				{
					ExTraceGlobals.oehCallTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.oehCallTracer;
			}
		}

		public static Trace OehDataTracer
		{
			get
			{
				if (ExTraceGlobals.oehDataTracer == null)
				{
					ExTraceGlobals.oehDataTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.oehDataTracer;
			}
		}

		public static Trace ThemesTracer
		{
			get
			{
				if (ExTraceGlobals.themesTracer == null)
				{
					ExTraceGlobals.themesTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.themesTracer;
			}
		}

		public static Trace ThemesCallTracer
		{
			get
			{
				if (ExTraceGlobals.themesCallTracer == null)
				{
					ExTraceGlobals.themesCallTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.themesCallTracer;
			}
		}

		public static Trace ThemesDataTracer
		{
			get
			{
				if (ExTraceGlobals.themesDataTracer == null)
				{
					ExTraceGlobals.themesDataTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.themesDataTracer;
			}
		}

		public static Trace SmallIconTracer
		{
			get
			{
				if (ExTraceGlobals.smallIconTracer == null)
				{
					ExTraceGlobals.smallIconTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.smallIconTracer;
			}
		}

		public static Trace SmallIconCallTracer
		{
			get
			{
				if (ExTraceGlobals.smallIconCallTracer == null)
				{
					ExTraceGlobals.smallIconCallTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.smallIconCallTracer;
			}
		}

		public static Trace SmallIconDataTracer
		{
			get
			{
				if (ExTraceGlobals.smallIconDataTracer == null)
				{
					ExTraceGlobals.smallIconDataTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.smallIconDataTracer;
			}
		}

		public static Trace FormsRegistryCallTracer
		{
			get
			{
				if (ExTraceGlobals.formsRegistryCallTracer == null)
				{
					ExTraceGlobals.formsRegistryCallTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.formsRegistryCallTracer;
			}
		}

		public static Trace FormsRegistryDataTracer
		{
			get
			{
				if (ExTraceGlobals.formsRegistryDataTracer == null)
				{
					ExTraceGlobals.formsRegistryDataTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.formsRegistryDataTracer;
			}
		}

		public static Trace FormsRegistryTracer
		{
			get
			{
				if (ExTraceGlobals.formsRegistryTracer == null)
				{
					ExTraceGlobals.formsRegistryTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.formsRegistryTracer;
			}
		}

		public static Trace UserOptionsTracer
		{
			get
			{
				if (ExTraceGlobals.userOptionsTracer == null)
				{
					ExTraceGlobals.userOptionsTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.userOptionsTracer;
			}
		}

		public static Trace UserOptionsCallTracer
		{
			get
			{
				if (ExTraceGlobals.userOptionsCallTracer == null)
				{
					ExTraceGlobals.userOptionsCallTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.userOptionsCallTracer;
			}
		}

		public static Trace UserOptionsDataTracer
		{
			get
			{
				if (ExTraceGlobals.userOptionsDataTracer == null)
				{
					ExTraceGlobals.userOptionsDataTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.userOptionsDataTracer;
			}
		}

		public static Trace MailTracer
		{
			get
			{
				if (ExTraceGlobals.mailTracer == null)
				{
					ExTraceGlobals.mailTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.mailTracer;
			}
		}

		public static Trace MailCallTracer
		{
			get
			{
				if (ExTraceGlobals.mailCallTracer == null)
				{
					ExTraceGlobals.mailCallTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.mailCallTracer;
			}
		}

		public static Trace MailDataTracer
		{
			get
			{
				if (ExTraceGlobals.mailDataTracer == null)
				{
					ExTraceGlobals.mailDataTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.mailDataTracer;
			}
		}

		public static Trace CalendarTracer
		{
			get
			{
				if (ExTraceGlobals.calendarTracer == null)
				{
					ExTraceGlobals.calendarTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.calendarTracer;
			}
		}

		public static Trace CalendarCallTracer
		{
			get
			{
				if (ExTraceGlobals.calendarCallTracer == null)
				{
					ExTraceGlobals.calendarCallTracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.calendarCallTracer;
			}
		}

		public static Trace CalendarDataTracer
		{
			get
			{
				if (ExTraceGlobals.calendarDataTracer == null)
				{
					ExTraceGlobals.calendarDataTracer = new Trace(ExTraceGlobals.componentGuid, 26);
				}
				return ExTraceGlobals.calendarDataTracer;
			}
		}

		public static Trace ContactsTracer
		{
			get
			{
				if (ExTraceGlobals.contactsTracer == null)
				{
					ExTraceGlobals.contactsTracer = new Trace(ExTraceGlobals.componentGuid, 27);
				}
				return ExTraceGlobals.contactsTracer;
			}
		}

		public static Trace ContactsCallTracer
		{
			get
			{
				if (ExTraceGlobals.contactsCallTracer == null)
				{
					ExTraceGlobals.contactsCallTracer = new Trace(ExTraceGlobals.componentGuid, 28);
				}
				return ExTraceGlobals.contactsCallTracer;
			}
		}

		public static Trace ContactsDataTracer
		{
			get
			{
				if (ExTraceGlobals.contactsDataTracer == null)
				{
					ExTraceGlobals.contactsDataTracer = new Trace(ExTraceGlobals.componentGuid, 29);
				}
				return ExTraceGlobals.contactsDataTracer;
			}
		}

		public static Trace DocumentsTracer
		{
			get
			{
				if (ExTraceGlobals.documentsTracer == null)
				{
					ExTraceGlobals.documentsTracer = new Trace(ExTraceGlobals.componentGuid, 30);
				}
				return ExTraceGlobals.documentsTracer;
			}
		}

		public static Trace DocumentsCallTracer
		{
			get
			{
				if (ExTraceGlobals.documentsCallTracer == null)
				{
					ExTraceGlobals.documentsCallTracer = new Trace(ExTraceGlobals.componentGuid, 31);
				}
				return ExTraceGlobals.documentsCallTracer;
			}
		}

		public static Trace DocumentsDataTracer
		{
			get
			{
				if (ExTraceGlobals.documentsDataTracer == null)
				{
					ExTraceGlobals.documentsDataTracer = new Trace(ExTraceGlobals.componentGuid, 32);
				}
				return ExTraceGlobals.documentsDataTracer;
			}
		}

		public static Trace RequestTracer
		{
			get
			{
				if (ExTraceGlobals.requestTracer == null)
				{
					ExTraceGlobals.requestTracer = new Trace(ExTraceGlobals.componentGuid, 33);
				}
				return ExTraceGlobals.requestTracer;
			}
		}

		public static Trace PerformanceTracer
		{
			get
			{
				if (ExTraceGlobals.performanceTracer == null)
				{
					ExTraceGlobals.performanceTracer = new Trace(ExTraceGlobals.componentGuid, 34);
				}
				return ExTraceGlobals.performanceTracer;
			}
		}

		public static Trace DirectoryTracer
		{
			get
			{
				if (ExTraceGlobals.directoryTracer == null)
				{
					ExTraceGlobals.directoryTracer = new Trace(ExTraceGlobals.componentGuid, 35);
				}
				return ExTraceGlobals.directoryTracer;
			}
		}

		public static Trace DirectoryCallTracer
		{
			get
			{
				if (ExTraceGlobals.directoryCallTracer == null)
				{
					ExTraceGlobals.directoryCallTracer = new Trace(ExTraceGlobals.componentGuid, 36);
				}
				return ExTraceGlobals.directoryCallTracer;
			}
		}

		public static Trace ExceptionTracer
		{
			get
			{
				if (ExTraceGlobals.exceptionTracer == null)
				{
					ExTraceGlobals.exceptionTracer = new Trace(ExTraceGlobals.componentGuid, 37);
				}
				return ExTraceGlobals.exceptionTracer;
			}
		}

		public static Trace UnifiedMessagingTracer
		{
			get
			{
				if (ExTraceGlobals.unifiedMessagingTracer == null)
				{
					ExTraceGlobals.unifiedMessagingTracer = new Trace(ExTraceGlobals.componentGuid, 38);
				}
				return ExTraceGlobals.unifiedMessagingTracer;
			}
		}

		public static Trace TranscodingTracer
		{
			get
			{
				if (ExTraceGlobals.transcodingTracer == null)
				{
					ExTraceGlobals.transcodingTracer = new Trace(ExTraceGlobals.componentGuid, 39);
				}
				return ExTraceGlobals.transcodingTracer;
			}
		}

		public static Trace NotificationsCallTracer
		{
			get
			{
				if (ExTraceGlobals.notificationsCallTracer == null)
				{
					ExTraceGlobals.notificationsCallTracer = new Trace(ExTraceGlobals.componentGuid, 40);
				}
				return ExTraceGlobals.notificationsCallTracer;
			}
		}

		public static Trace SpellcheckCallTracer
		{
			get
			{
				if (ExTraceGlobals.spellcheckCallTracer == null)
				{
					ExTraceGlobals.spellcheckCallTracer = new Trace(ExTraceGlobals.componentGuid, 41);
				}
				return ExTraceGlobals.spellcheckCallTracer;
			}
		}

		public static Trace ProxyCallTracer
		{
			get
			{
				if (ExTraceGlobals.proxyCallTracer == null)
				{
					ExTraceGlobals.proxyCallTracer = new Trace(ExTraceGlobals.componentGuid, 42);
				}
				return ExTraceGlobals.proxyCallTracer;
			}
		}

		public static Trace ProxyTracer
		{
			get
			{
				if (ExTraceGlobals.proxyTracer == null)
				{
					ExTraceGlobals.proxyTracer = new Trace(ExTraceGlobals.componentGuid, 43);
				}
				return ExTraceGlobals.proxyTracer;
			}
		}

		public static Trace ProxyDataTracer
		{
			get
			{
				if (ExTraceGlobals.proxyDataTracer == null)
				{
					ExTraceGlobals.proxyDataTracer = new Trace(ExTraceGlobals.componentGuid, 44);
				}
				return ExTraceGlobals.proxyDataTracer;
			}
		}

		public static Trace ProxyRequestTracer
		{
			get
			{
				if (ExTraceGlobals.proxyRequestTracer == null)
				{
					ExTraceGlobals.proxyRequestTracer = new Trace(ExTraceGlobals.componentGuid, 45);
				}
				return ExTraceGlobals.proxyRequestTracer;
			}
		}

		public static Trace TasksTracer
		{
			get
			{
				if (ExTraceGlobals.tasksTracer == null)
				{
					ExTraceGlobals.tasksTracer = new Trace(ExTraceGlobals.componentGuid, 46);
				}
				return ExTraceGlobals.tasksTracer;
			}
		}

		public static Trace TasksCallTracer
		{
			get
			{
				if (ExTraceGlobals.tasksCallTracer == null)
				{
					ExTraceGlobals.tasksCallTracer = new Trace(ExTraceGlobals.componentGuid, 47);
				}
				return ExTraceGlobals.tasksCallTracer;
			}
		}

		public static Trace TasksDataTracer
		{
			get
			{
				if (ExTraceGlobals.tasksDataTracer == null)
				{
					ExTraceGlobals.tasksDataTracer = new Trace(ExTraceGlobals.componentGuid, 48);
				}
				return ExTraceGlobals.tasksDataTracer;
			}
		}

		public static Trace WebPartRequestTracer
		{
			get
			{
				if (ExTraceGlobals.webPartRequestTracer == null)
				{
					ExTraceGlobals.webPartRequestTracer = new Trace(ExTraceGlobals.componentGuid, 49);
				}
				return ExTraceGlobals.webPartRequestTracer;
			}
		}

		public static Trace ConfigurationManagerTracer
		{
			get
			{
				if (ExTraceGlobals.configurationManagerTracer == null)
				{
					ExTraceGlobals.configurationManagerTracer = new Trace(ExTraceGlobals.componentGuid, 50);
				}
				return ExTraceGlobals.configurationManagerTracer;
			}
		}

		public static Trace ChangePasswordTracer
		{
			get
			{
				if (ExTraceGlobals.changePasswordTracer == null)
				{
					ExTraceGlobals.changePasswordTracer = new Trace(ExTraceGlobals.componentGuid, 51);
				}
				return ExTraceGlobals.changePasswordTracer;
			}
		}

		public static Trace LiveIdAuthenticationModuleTracer
		{
			get
			{
				if (ExTraceGlobals.liveIdAuthenticationModuleTracer == null)
				{
					ExTraceGlobals.liveIdAuthenticationModuleTracer = new Trace(ExTraceGlobals.componentGuid, 52);
				}
				return ExTraceGlobals.liveIdAuthenticationModuleTracer;
			}
		}

		public static Trace PolicyConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.policyConfigurationTracer == null)
				{
					ExTraceGlobals.policyConfigurationTracer = new Trace(ExTraceGlobals.componentGuid, 53);
				}
				return ExTraceGlobals.policyConfigurationTracer;
			}
		}

		public static Trace CoBrandingTracer
		{
			get
			{
				if (ExTraceGlobals.coBrandingTracer == null)
				{
					ExTraceGlobals.coBrandingTracer = new Trace(ExTraceGlobals.componentGuid, 54);
				}
				return ExTraceGlobals.coBrandingTracer;
			}
		}

		public static Trace IrmTracer
		{
			get
			{
				if (ExTraceGlobals.irmTracer == null)
				{
					ExTraceGlobals.irmTracer = new Trace(ExTraceGlobals.componentGuid, 55);
				}
				return ExTraceGlobals.irmTracer;
			}
		}

		public static Trace InstantMessagingTracer
		{
			get
			{
				if (ExTraceGlobals.instantMessagingTracer == null)
				{
					ExTraceGlobals.instantMessagingTracer = new Trace(ExTraceGlobals.componentGuid, 56);
				}
				return ExTraceGlobals.instantMessagingTracer;
			}
		}

		public static Trace AttachmentHandlingTracer
		{
			get
			{
				if (ExTraceGlobals.attachmentHandlingTracer == null)
				{
					ExTraceGlobals.attachmentHandlingTracer = new Trace(ExTraceGlobals.componentGuid, 57);
				}
				return ExTraceGlobals.attachmentHandlingTracer;
			}
		}

		public static Trace SpeechRecognitionTracer
		{
			get
			{
				if (ExTraceGlobals.speechRecognitionTracer == null)
				{
					ExTraceGlobals.speechRecognitionTracer = new Trace(ExTraceGlobals.componentGuid, 58);
				}
				return ExTraceGlobals.speechRecognitionTracer;
			}
		}

		public static Trace AnonymousServiceCommandTracer
		{
			get
			{
				if (ExTraceGlobals.anonymousServiceCommandTracer == null)
				{
					ExTraceGlobals.anonymousServiceCommandTracer = new Trace(ExTraceGlobals.componentGuid, 59);
				}
				return ExTraceGlobals.anonymousServiceCommandTracer;
			}
		}

		public static Trace AppcacheManifestHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.appcacheManifestHandlerTracer == null)
				{
					ExTraceGlobals.appcacheManifestHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 60);
				}
				return ExTraceGlobals.appcacheManifestHandlerTracer;
			}
		}

		public static Trace DefaultPageHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.defaultPageHandlerTracer == null)
				{
					ExTraceGlobals.defaultPageHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 61);
				}
				return ExTraceGlobals.defaultPageHandlerTracer;
			}
		}

		public static Trace GetPersonaPhotoTracer
		{
			get
			{
				if (ExTraceGlobals.getPersonaPhotoTracer == null)
				{
					ExTraceGlobals.getPersonaPhotoTracer = new Trace(ExTraceGlobals.componentGuid, 62);
				}
				return ExTraceGlobals.getPersonaPhotoTracer;
			}
		}

		public static Trace MobileDevicePolicyTracer
		{
			get
			{
				if (ExTraceGlobals.mobileDevicePolicyTracer == null)
				{
					ExTraceGlobals.mobileDevicePolicyTracer = new Trace(ExTraceGlobals.componentGuid, 63);
				}
				return ExTraceGlobals.mobileDevicePolicyTracer;
			}
		}

		public static Trace GetPersonaOrganizationHierarchyTracer
		{
			get
			{
				if (ExTraceGlobals.getPersonaOrganizationHierarchyTracer == null)
				{
					ExTraceGlobals.getPersonaOrganizationHierarchyTracer = new Trace(ExTraceGlobals.componentGuid, 64);
				}
				return ExTraceGlobals.getPersonaOrganizationHierarchyTracer;
			}
		}

		public static Trace GetGroupTracer
		{
			get
			{
				if (ExTraceGlobals.getGroupTracer == null)
				{
					ExTraceGlobals.getGroupTracer = new Trace(ExTraceGlobals.componentGuid, 65);
				}
				return ExTraceGlobals.getGroupTracer;
			}
		}

		public static Trace ConnectedAccountsTracer
		{
			get
			{
				if (ExTraceGlobals.connectedAccountsTracer == null)
				{
					ExTraceGlobals.connectedAccountsTracer = new Trace(ExTraceGlobals.componentGuid, 66);
				}
				return ExTraceGlobals.connectedAccountsTracer;
			}
		}

		public static Trace AppWipeTracer
		{
			get
			{
				if (ExTraceGlobals.appWipeTracer == null)
				{
					ExTraceGlobals.appWipeTracer = new Trace(ExTraceGlobals.componentGuid, 67);
				}
				return ExTraceGlobals.appWipeTracer;
			}
		}

		public static Trace OnlineMeetingTracer
		{
			get
			{
				if (ExTraceGlobals.onlineMeetingTracer == null)
				{
					ExTraceGlobals.onlineMeetingTracer = new Trace(ExTraceGlobals.componentGuid, 68);
				}
				return ExTraceGlobals.onlineMeetingTracer;
			}
		}

		public static Trace InlineExploreTracer
		{
			get
			{
				if (ExTraceGlobals.inlineExploreTracer == null)
				{
					ExTraceGlobals.inlineExploreTracer = new Trace(ExTraceGlobals.componentGuid, 69);
				}
				return ExTraceGlobals.inlineExploreTracer;
			}
		}

		public static Trace RemoveFavoriteTracer
		{
			get
			{
				if (ExTraceGlobals.removeFavoriteTracer == null)
				{
					ExTraceGlobals.removeFavoriteTracer = new Trace(ExTraceGlobals.componentGuid, 70);
				}
				return ExTraceGlobals.removeFavoriteTracer;
			}
		}

		public static Trace CobaltTracer
		{
			get
			{
				if (ExTraceGlobals.cobaltTracer == null)
				{
					ExTraceGlobals.cobaltTracer = new Trace(ExTraceGlobals.componentGuid, 71);
				}
				return ExTraceGlobals.cobaltTracer;
			}
		}

		public static Trace PeopleIKnowNotificationsTracer
		{
			get
			{
				if (ExTraceGlobals.peopleIKnowNotificationsTracer == null)
				{
					ExTraceGlobals.peopleIKnowNotificationsTracer = new Trace(ExTraceGlobals.componentGuid, 72);
				}
				return ExTraceGlobals.peopleIKnowNotificationsTracer;
			}
		}

		public static Trace OwaPerTenantCacheTracer
		{
			get
			{
				if (ExTraceGlobals.owaPerTenantCacheTracer == null)
				{
					ExTraceGlobals.owaPerTenantCacheTracer = new Trace(ExTraceGlobals.componentGuid, 73);
				}
				return ExTraceGlobals.owaPerTenantCacheTracer;
			}
		}

		public static Trace AdfsAuthModuleTracer
		{
			get
			{
				if (ExTraceGlobals.adfsAuthModuleTracer == null)
				{
					ExTraceGlobals.adfsAuthModuleTracer = new Trace(ExTraceGlobals.componentGuid, 74);
				}
				return ExTraceGlobals.adfsAuthModuleTracer;
			}
		}

		public static Trace SessionDataHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.sessionDataHandlerTracer == null)
				{
					ExTraceGlobals.sessionDataHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 75);
				}
				return ExTraceGlobals.sessionDataHandlerTracer;
			}
		}

		public static Trace GetPersonaNotesTracer
		{
			get
			{
				if (ExTraceGlobals.getPersonaNotesTracer == null)
				{
					ExTraceGlobals.getPersonaNotesTracer = new Trace(ExTraceGlobals.componentGuid, 76);
				}
				return ExTraceGlobals.getPersonaNotesTracer;
			}
		}

		public static Trace CryptoTracer
		{
			get
			{
				if (ExTraceGlobals.cryptoTracer == null)
				{
					ExTraceGlobals.cryptoTracer = new Trace(ExTraceGlobals.componentGuid, 77);
				}
				return ExTraceGlobals.cryptoTracer;
			}
		}

		private static Guid componentGuid = new Guid("1758fd24-1153-4624-96f6-742b18fc0372");

		private static Trace coreTracer = null;

		private static Trace coreCallTracer = null;

		private static Trace coreDataTracer = null;

		private static Trace userContextTracer = null;

		private static Trace userContextCallTracer = null;

		private static Trace userContextDataTracer = null;

		private static Trace oehTracer = null;

		private static Trace oehCallTracer = null;

		private static Trace oehDataTracer = null;

		private static Trace themesTracer = null;

		private static Trace themesCallTracer = null;

		private static Trace themesDataTracer = null;

		private static Trace smallIconTracer = null;

		private static Trace smallIconCallTracer = null;

		private static Trace smallIconDataTracer = null;

		private static Trace formsRegistryCallTracer = null;

		private static Trace formsRegistryDataTracer = null;

		private static Trace formsRegistryTracer = null;

		private static Trace userOptionsTracer = null;

		private static Trace userOptionsCallTracer = null;

		private static Trace userOptionsDataTracer = null;

		private static Trace mailTracer = null;

		private static Trace mailCallTracer = null;

		private static Trace mailDataTracer = null;

		private static Trace calendarTracer = null;

		private static Trace calendarCallTracer = null;

		private static Trace calendarDataTracer = null;

		private static Trace contactsTracer = null;

		private static Trace contactsCallTracer = null;

		private static Trace contactsDataTracer = null;

		private static Trace documentsTracer = null;

		private static Trace documentsCallTracer = null;

		private static Trace documentsDataTracer = null;

		private static Trace requestTracer = null;

		private static Trace performanceTracer = null;

		private static Trace directoryTracer = null;

		private static Trace directoryCallTracer = null;

		private static Trace exceptionTracer = null;

		private static Trace unifiedMessagingTracer = null;

		private static Trace transcodingTracer = null;

		private static Trace notificationsCallTracer = null;

		private static Trace spellcheckCallTracer = null;

		private static Trace proxyCallTracer = null;

		private static Trace proxyTracer = null;

		private static Trace proxyDataTracer = null;

		private static Trace proxyRequestTracer = null;

		private static Trace tasksTracer = null;

		private static Trace tasksCallTracer = null;

		private static Trace tasksDataTracer = null;

		private static Trace webPartRequestTracer = null;

		private static Trace configurationManagerTracer = null;

		private static Trace changePasswordTracer = null;

		private static Trace liveIdAuthenticationModuleTracer = null;

		private static Trace policyConfigurationTracer = null;

		private static Trace coBrandingTracer = null;

		private static Trace irmTracer = null;

		private static Trace instantMessagingTracer = null;

		private static Trace attachmentHandlingTracer = null;

		private static Trace speechRecognitionTracer = null;

		private static Trace anonymousServiceCommandTracer = null;

		private static Trace appcacheManifestHandlerTracer = null;

		private static Trace defaultPageHandlerTracer = null;

		private static Trace getPersonaPhotoTracer = null;

		private static Trace mobileDevicePolicyTracer = null;

		private static Trace getPersonaOrganizationHierarchyTracer = null;

		private static Trace getGroupTracer = null;

		private static Trace connectedAccountsTracer = null;

		private static Trace appWipeTracer = null;

		private static Trace onlineMeetingTracer = null;

		private static Trace inlineExploreTracer = null;

		private static Trace removeFavoriteTracer = null;

		private static Trace cobaltTracer = null;

		private static Trace peopleIKnowNotificationsTracer = null;

		private static Trace owaPerTenantCacheTracer = null;

		private static Trace adfsAuthModuleTracer = null;

		private static Trace sessionDataHandlerTracer = null;

		private static Trace getPersonaNotesTracer = null;

		private static Trace cryptoTracer = null;
	}
}
