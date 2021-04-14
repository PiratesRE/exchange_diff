using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Cluster.Replay
{
	[Serializable]
	public class DagConfigurationHelper
	{
		public int Version
		{
			get
			{
				return this.m_version;
			}
			set
			{
				this.m_version = value;
			}
		}

		public int ServersPerDag
		{
			get
			{
				return this.m_serversPerDag;
			}
			set
			{
				this.m_serversPerDag = value;
			}
		}

		public int DatabasesPerServer
		{
			get
			{
				return this.m_databasesPerServer;
			}
			set
			{
				this.m_databasesPerServer = value;
			}
		}

		public int DatabasesPerVolume
		{
			get
			{
				return this.m_databasesPerVolume;
			}
			set
			{
				this.m_databasesPerVolume = value;
			}
		}

		public int CopiesPerDatabase
		{
			get
			{
				return this.m_copiesPerDatabase;
			}
			set
			{
				this.m_copiesPerDatabase = value;
			}
		}

		public int MinCopiesPerDatabaseForMonitoring
		{
			get
			{
				return this.m_minCopiesPerDatabaseForMonitoring;
			}
			set
			{
				this.m_minCopiesPerDatabaseForMonitoring = value;
			}
		}

		public DagConfigurationHelper()
		{
			this.m_version = 1;
			this.m_serversPerDag = 0;
			this.m_databasesPerServer = 0;
			this.m_databasesPerVolume = 0;
			this.m_copiesPerDatabase = 0;
			this.m_minCopiesPerDatabaseForMonitoring = 1;
		}

		public DagConfigurationHelper(int serversPerDag, int databasesPerServer, int databasesPerVolume, int copiesPerDatabase, int minCopiesPerDatabaseForMonitoring)
		{
			this.m_version = 1;
			this.m_serversPerDag = serversPerDag;
			this.m_databasesPerServer = databasesPerServer;
			this.m_databasesPerVolume = databasesPerVolume;
			this.m_copiesPerDatabase = copiesPerDatabase;
			this.m_minCopiesPerDatabaseForMonitoring = minCopiesPerDatabaseForMonitoring;
		}

		public string Serialize()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(DagConfigurationHelper));
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				xmlSerializer.Serialize(stringWriter, this);
				this.m_serializedForm = stringWriter.ToString();
			}
			return this.m_serializedForm;
		}

		public static DagConfigurationHelper Deserialize(string configXML)
		{
			DagConfigurationHelper dagConfigurationHelper = null;
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(DagConfigurationHelper));
			DagConfigurationHelper result;
			using (StringReader stringReader = new StringReader(configXML))
			{
				try
				{
					object obj = xmlSerializer.Deserialize(stringReader);
					dagConfigurationHelper = (obj as DagConfigurationHelper);
					if (dagConfigurationHelper == null)
					{
						ExTraceGlobals.CmdletsTracer.TraceError<string, string>(0L, "Deserialized object {0} was not compatible with expected type {1}.", (obj != null) ? obj.GetType().Name : "(null)", typeof(DagConfigurationHelper).Name);
					}
				}
				catch (InvalidOperationException ex)
				{
					ExTraceGlobals.CmdletsTracer.TraceDebug<string, string>(0L, "Deserialization of object {0} failed:\n{1}", typeof(DagConfigurationHelper).Name, ex.ToString());
				}
				if (dagConfigurationHelper == null)
				{
					throw new FailedToDeserializeDagConfigurationXMLString(configXML, typeof(DagConfigurationHelper).Name);
				}
				result = dagConfigurationHelper;
			}
			return result;
		}

		internal static DatabaseAvailabilityGroupConfiguration ReadDagConfig(ADObjectId dagConfigId, IConfigDataProvider configSession)
		{
			DatabaseAvailabilityGroupConfiguration result = null;
			if (!ADObjectId.IsNullOrEmpty(dagConfigId))
			{
				result = (DatabaseAvailabilityGroupConfiguration)configSession.Read<DatabaseAvailabilityGroupConfiguration>(dagConfigId);
			}
			return result;
		}

		internal static DatabaseAvailabilityGroupConfiguration DagConfigIdParameterToDagConfig(DatabaseAvailabilityGroupConfigurationIdParameter dagConfigIdParam, IConfigDataProvider configSession)
		{
			IEnumerable<DatabaseAvailabilityGroupConfiguration> objects = dagConfigIdParam.GetObjects<DatabaseAvailabilityGroupConfiguration>(null, configSession);
			DatabaseAvailabilityGroupConfiguration result;
			using (IEnumerator<DatabaseAvailabilityGroupConfiguration> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new ManagementObjectNotFoundException(Strings.ErrorDagNotFound(dagConfigIdParam.ToString()));
				}
				DatabaseAvailabilityGroupConfiguration databaseAvailabilityGroupConfiguration = enumerator.Current;
				if (enumerator.MoveNext())
				{
					throw new ManagementObjectAmbiguousException(Strings.ErrorDagNotUnique(dagConfigIdParam.ToString()));
				}
				result = databaseAvailabilityGroupConfiguration;
			}
			return result;
		}

		[NonSerialized]
		public const int ConfigVersion = 1;

		private int m_version;

		private int m_serversPerDag;

		private int m_databasesPerServer;

		private int m_databasesPerVolume;

		private int m_copiesPerDatabase;

		private int m_minCopiesPerDatabaseForMonitoring;

		private string m_serializedForm;
	}
}
