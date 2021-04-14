using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.EdgeSync.Common.Internal
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(39187813U, "NoSubscriptions");
			Strings.stringIDs.Add(137354313U, "CertErrorDuringConnect");
			Strings.stringIDs.Add(1462089994U, "SeeEventLogForSyncFailures");
			Strings.stringIDs.Add(836513619U, "CannotGetAdminGroupsContainer");
			Strings.stringIDs.Add(380956288U, "ServersEnumerate");
			Strings.stringIDs.Add(468239512U, "CouldNotSaveHub");
			Strings.stringIDs.Add(1997022922U, "CannotGetOrgContainer");
			Strings.stringIDs.Add(2249628033U, "CannotGetLocalSite");
			Strings.stringIDs.Add(2143632979U, "UnableToLoadSD");
			Strings.stringIDs.Add(1724752590U, "EdgeNotFoundInDNS");
			Strings.stringIDs.Add(2846323109U, "LocalHubNotFound");
		}

		public static LocalizedString NoSubscriptions
		{
			get
			{
				return new LocalizedString("NoSubscriptions", "ExAFFA7E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeSyncNormal(string service)
		{
			return new LocalizedString("EdgeSyncNormal", "Ex622C6F", false, true, Strings.ResourceManager, new object[]
			{
				service
			});
		}

		public static LocalizedString CertErrorDuringConnect
		{
			get
			{
				return new LocalizedString("CertErrorDuringConnect", "Ex3A7E44", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeSyncServiceConfigNotFoundException(string site, string dn)
		{
			return new LocalizedString("EdgeSyncServiceConfigNotFoundException", "", false, false, Strings.ResourceManager, new object[]
			{
				site,
				dn
			});
		}

		public static LocalizedString ProviderNull(string name)
		{
			return new LocalizedString("ProviderNull", "Ex5F9541", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString LoadedADAMObjectCount(int objectCount)
		{
			return new LocalizedString("LoadedADAMObjectCount", "Ex750831", false, true, Strings.ResourceManager, new object[]
			{
				objectCount
			});
		}

		public static LocalizedString EdgeSyncNotConfigured(string service)
		{
			return new LocalizedString("EdgeSyncNotConfigured", "Ex0C9239", false, true, Strings.ResourceManager, new object[]
			{
				service
			});
		}

		public static LocalizedString CouldNotCreateProvider(string name)
		{
			return new LocalizedString("CouldNotCreateProvider", "Ex5D9A4C", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString SeeEventLogForSyncFailures
		{
			get
			{
				return new LocalizedString("SeeEventLogForSyncFailures", "ExE31A64", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetAdminGroupsContainer
		{
			get
			{
				return new LocalizedString("CannotGetAdminGroupsContainer", "ExE6C23A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeSyncFailedUrgent(string service, string taskName)
		{
			return new LocalizedString("EdgeSyncFailedUrgent", "", false, false, Strings.ResourceManager, new object[]
			{
				service,
				taskName
			});
		}

		public static LocalizedString ServersEnumerate
		{
			get
			{
				return new LocalizedString("ServersEnumerate", "ExAA8622", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CouldNotSaveHub
		{
			get
			{
				return new LocalizedString("CouldNotSaveHub", "Ex5E8A8B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetOrgContainer
		{
			get
			{
				return new LocalizedString("CannotGetOrgContainer", "ExFC412D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeSyncFailed(string service, string taskName)
		{
			return new LocalizedString("EdgeSyncFailed", "Ex383E75", false, true, Strings.ResourceManager, new object[]
			{
				service,
				taskName
			});
		}

		public static LocalizedString LoadedADObjectCount(int objectCount)
		{
			return new LocalizedString("LoadedADObjectCount", "ExFC8325", false, true, Strings.ResourceManager, new object[]
			{
				objectCount
			});
		}

		public static LocalizedString CannotGetLocalSite
		{
			get
			{
				return new LocalizedString("CannotGetLocalSite", "Ex3C890F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LoadingADAMComparisonList(string objectType, string serverName)
		{
			return new LocalizedString("LoadingADAMComparisonList", "ExA08E15", false, true, Strings.ResourceManager, new object[]
			{
				objectType,
				serverName
			});
		}

		public static LocalizedString TargetEdgeNotFound(string server)
		{
			return new LocalizedString("TargetEdgeNotFound", "Ex5553EC", false, true, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString UnableToLoadSD
		{
			get
			{
				return new LocalizedString("UnableToLoadSD", "ExCDE63F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidProviderPath(string name, string path)
		{
			return new LocalizedString("InvalidProviderPath", "Ex55004B", false, true, Strings.ResourceManager, new object[]
			{
				name,
				path
			});
		}

		public static LocalizedString EdgeNotFoundInDNS
		{
			get
			{
				return new LocalizedString("EdgeNotFoundInDNS", "Ex2F4B16", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeSyncAbnormal(string service, string taskName)
		{
			return new LocalizedString("EdgeSyncAbnormal", "Ex511308", false, true, Strings.ResourceManager, new object[]
			{
				service,
				taskName
			});
		}

		public static LocalizedString NoCredentialsFound(string serverName)
		{
			return new LocalizedString("NoCredentialsFound", "Ex81B410", false, true, Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString LocalHubNotFound
		{
			get
			{
				return new LocalizedString("LocalHubNotFound", "ExE2E5D2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeSyncInconclusive(string service, string taskName)
		{
			return new LocalizedString("EdgeSyncInconclusive", "ExFC6C9A", false, true, Strings.ResourceManager, new object[]
			{
				service,
				taskName
			});
		}

		public static LocalizedString NoSiteAttributeFound(string servername)
		{
			return new LocalizedString("NoSiteAttributeFound", "Ex12C90A", false, true, Strings.ResourceManager, new object[]
			{
				servername
			});
		}

		public static LocalizedString CannotLoadDefaultCertificate(string server)
		{
			return new LocalizedString("CannotLoadDefaultCertificate", "Ex2B8ADD", false, true, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString LoadingADComparisonList(string objectType)
		{
			return new LocalizedString("LoadingADComparisonList", "Ex70819E", false, true, Strings.ResourceManager, new object[]
			{
				objectType
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(11);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.EdgeSync.Common.Internal.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			NoSubscriptions = 39187813U,
			CertErrorDuringConnect = 137354313U,
			SeeEventLogForSyncFailures = 1462089994U,
			CannotGetAdminGroupsContainer = 836513619U,
			ServersEnumerate = 380956288U,
			CouldNotSaveHub = 468239512U,
			CannotGetOrgContainer = 1997022922U,
			CannotGetLocalSite = 2249628033U,
			UnableToLoadSD = 2143632979U,
			EdgeNotFoundInDNS = 1724752590U,
			LocalHubNotFound = 2846323109U
		}

		private enum ParamIDs
		{
			EdgeSyncNormal,
			EdgeSyncServiceConfigNotFoundException,
			ProviderNull,
			LoadedADAMObjectCount,
			EdgeSyncNotConfigured,
			CouldNotCreateProvider,
			EdgeSyncFailedUrgent,
			EdgeSyncFailed,
			LoadedADObjectCount,
			LoadingADAMComparisonList,
			TargetEdgeNotFound,
			InvalidProviderPath,
			EdgeSyncAbnormal,
			NoCredentialsFound,
			EdgeSyncInconclusive,
			NoSiteAttributeFound,
			CannotLoadDefaultCertificate,
			LoadingADComparisonList
		}
	}
}
