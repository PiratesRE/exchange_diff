using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Office365.DataInsights.Uploader;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal abstract class ScopeMappingEndpoint : IDisposable
	{
		internal static bool IsDCEnvironment
		{
			get
			{
				return ScopeMappingEndpoint.isDCEnvironment;
			}
		}

		internal static bool IsSystemMonitoringEnvironment
		{
			get
			{
				return ScopeMappingEndpoint.isSystemMonitoringEnvironment;
			}
		}

		public ConcurrentDictionary<string, ScopeMapping> ScopeMappings { get; protected set; }

		public ConcurrentDictionary<string, SystemMonitoringMapping> SystemMonitoringMappings { get; protected set; }

		public ConcurrentDictionary<string, ScopeMapping> DefaultScopes { get; protected set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal abstract void InitializeScopeAndSystemMonitoringMappings();

		internal abstract void InitializeDefaultScopes();

		protected void InitializeScopeAndSystemMonitoringMappingsFromXml(XmlNode scope, ConcurrentDictionary<string, ScopeMapping> scopeMappings, ConcurrentDictionary<string, SystemMonitoringMapping> smMappings, ScopeMapping parentScopeMapping = null)
		{
			if (scope.Attributes["Name"] == null)
			{
				return;
			}
			if (scope.Attributes["Type"] == null)
			{
				return;
			}
			ScopeMapping scopeMapping = new ScopeMapping
			{
				ScopeName = scope.Attributes["Name"].Value,
				ScopeType = scope.Attributes["Type"].Value,
				SystemMonitoringInstances = ((parentScopeMapping != null) ? parentScopeMapping.SystemMonitoringInstances : null),
				Parent = parentScopeMapping
			};
			XmlNodeList xmlNodeList = scope.SelectNodes("SystemMonitoring/Instance");
			if (xmlNodeList != null && xmlNodeList.Count > 0)
			{
				scopeMapping.SystemMonitoringInstances = new List<SystemMonitoringMapping>();
				foreach (object obj in xmlNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string text = (!string.IsNullOrWhiteSpace(ScopeMappingEndpoint.systemMonitoringInstance)) ? ScopeMappingEndpoint.systemMonitoringInstance : xmlNode.Attributes["Name"].Value;
					SystemMonitoringMapping smMapping = null;
					if (!smMappings.TryGetValue(text, out smMapping))
					{
						smMapping = new SystemMonitoringMapping
						{
							InstanceName = text,
							Scopes = null,
							Uploader = this.GetBatchingUploader(text)
						};
						smMappings[text] = smMapping;
					}
					if (!scopeMapping.SystemMonitoringInstances.Exists((SystemMonitoringMapping i) => i.InstanceName.Equals(smMapping.InstanceName, StringComparison.InvariantCultureIgnoreCase)))
					{
						scopeMapping.SystemMonitoringInstances.Add(smMapping);
					}
				}
			}
			if (scope.Attributes["Exclude"] == null || !scope.Attributes["Exclude"].Value.Equals("True", StringComparison.InvariantCultureIgnoreCase))
			{
				scopeMappings[scopeMapping.ScopeName] = scopeMapping;
				if (scopeMapping.SystemMonitoringInstances != null)
				{
					scopeMapping.SystemMonitoringInstances.ForEach(delegate(SystemMonitoringMapping sm)
					{
						if (sm.Scopes == null)
						{
							sm.Scopes = new List<ScopeMapping>();
						}
						if (!sm.Scopes.Exists((ScopeMapping s) => s.ScopeName.Equals(scopeMapping.ScopeName, StringComparison.InvariantCultureIgnoreCase)))
						{
							sm.Scopes.Add(scopeMapping);
						}
					});
				}
			}
			foreach (object obj2 in scope.SelectNodes("Scope"))
			{
				XmlNode scope2 = (XmlNode)obj2;
				this.InitializeScopeAndSystemMonitoringMappingsFromXml(scope2, scopeMappings, smMappings, scopeMapping);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing && this.batchingUploaders != null)
				{
					foreach (KeyValuePair<string, BatchingUploader<ScopeNotificationRawData>> keyValuePair in this.batchingUploaders)
					{
						if (keyValuePair.Value != null)
						{
							keyValuePair.Value.Dispose();
						}
					}
				}
				this.disposed = true;
			}
		}

		private BatchingUploader<ScopeNotificationRawData> GetBatchingUploader(string uploadUri)
		{
			BatchingUploader<ScopeNotificationRawData> batchingUploader;
			if (!this.batchingUploaders.TryGetValue(uploadUri, out batchingUploader))
			{
				batchingUploader = new BatchingUploader<ScopeNotificationRawData>(this.encoder, uploadUri, 20000, TimeSpan.FromSeconds(10.0), 10, 1, 3, false, "", false, null, null, null, null, false);
				this.batchingUploaders[uploadUri] = batchingUploader;
			}
			return batchingUploader;
		}

		internal const int RecurrenceIntervalSeconds = 0;

		private static bool isDCEnvironment = string.Equals(Settings.Environment, "DC", StringComparison.InvariantCultureIgnoreCase);

		private static bool isSystemMonitoringEnvironment = string.Equals(Settings.Environment, "SM", StringComparison.InvariantCultureIgnoreCase);

		private static string systemMonitoringInstance = Settings.SystemMonitoringInstance;

		private DataContractSerializerEncoder<ScopeNotificationRawData> encoder = new DataContractSerializerEncoder<ScopeNotificationRawData>();

		private ConcurrentDictionary<string, BatchingUploader<ScopeNotificationRawData>> batchingUploaders = new ConcurrentDictionary<string, BatchingUploader<ScopeNotificationRawData>>(StringComparer.InvariantCultureIgnoreCase);

		private bool disposed;

		protected enum ServiceEnvironment
		{
			Prod,
			Sdf,
			Pdt,
			Gallatin,
			Test
		}
	}
}
