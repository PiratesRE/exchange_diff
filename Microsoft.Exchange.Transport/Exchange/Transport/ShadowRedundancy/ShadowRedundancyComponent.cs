using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Storage.Messaging;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal sealed class ShadowRedundancyComponent : IStartableTransportComponent, ITransportComponent, IShadowRedundancyComponent, IDiagnosable
	{
		public ShadowRedundancyManager ShadowRedundancyManager
		{
			get
			{
				if (this.shadowRedundancyManager == null)
				{
					throw new InvalidOperationException("Attempt to retrieve ShadowRedundancyManager instance before ShadowRedundancyComponent is loaded.");
				}
				return this.shadowRedundancyManager;
			}
		}

		public void Load()
		{
			if (this.shadowRedundancyManager != null)
			{
				throw new InvalidOperationException("ShadowRedundancyComponent.Load() can only be called once.");
			}
			this.shadowRedundancyManager = new ShadowRedundancyManager(new ShadowRedundancyConfig(), new ShadowRedundancyPerformanceCounters(), new ShadowRedundancyEventLogger(), this.database);
			this.bootLoader.OnBootLoadCompleted += this.shadowRedundancyManager.NotifyBootLoaderDone;
		}

		public void Unload()
		{
			this.bootLoader.OnBootLoadCompleted -= this.shadowRedundancyManager.NotifyBootLoaderDone;
			this.shadowRedundancyManager.NotifyShuttingDown();
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void SetLoadTimeDependencies(IMessagingDatabase database, IBootLoader bootLoader)
		{
			if (database == null)
			{
				throw new ArgumentNullException("database");
			}
			if (bootLoader == null)
			{
				throw new ArgumentNullException("bootLoader");
			}
			this.database = database;
			this.bootLoader = bootLoader;
		}

		public string CurrentState
		{
			get
			{
				return null;
			}
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
			this.shadowRedundancyManager.Start(initiallyPaused, targetRunningState);
		}

		public void Stop()
		{
			this.shadowRedundancyManager.Stop();
		}

		public void Pause()
		{
			this.ShadowRedundancyManager.Pause();
		}

		public void Continue()
		{
			this.ShadowRedundancyManager.Continue();
		}

		IShadowRedundancyManagerFacade IShadowRedundancyComponent.ShadowRedundancyManager
		{
			get
			{
				return this.ShadowRedundancyManager;
			}
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "ShadowRedundancy";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			bool flag = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = flag || parameters.Argument.IndexOf("basic", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag3 = flag2 || parameters.Argument.IndexOf("config", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag4 = parameters.Argument.IndexOf("diversity", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag5 = (!flag3 && !flag4) || parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			string diagnosticComponentName = ((IDiagnosable)this).GetDiagnosticComponentName();
			XElement xelement = new XElement(diagnosticComponentName);
			if (flag5)
			{
				xelement.Add(new XElement("help", "Supported arguments: config, basic, verbose, diversity:" + QueueDiversity.UsageString));
			}
			if (flag3)
			{
				this.ShadowRedundancyManager.AddDiagnosticInfoTo(xelement, flag2, flag);
			}
			if (flag4)
			{
				string requestArgument = parameters.Argument.Substring(parameters.Argument.IndexOf("diversity", StringComparison.OrdinalIgnoreCase) + "diversity".Length);
				this.AddDiversityDiagnosticInfo(xelement, requestArgument);
			}
			return xelement;
		}

		private void AddDiversityDiagnosticInfo(XElement shadowRedundancyElement, string requestArgument)
		{
			QueueDiversity queueDiversity;
			string text;
			if (QueueDiversity.TryParse(requestArgument, false, out queueDiversity, out text))
			{
				if (queueDiversity.QueueId.Type == QueueType.Shadow)
				{
					List<ShadowMessageQueue> list = (queueDiversity.QueueId.RowId != 0L) ? this.ShadowRedundancyManager.FindByQueueIdentity(queueDiversity.QueueId) : null;
					if (list != null && list.Count > 0)
					{
						using (List<ShadowMessageQueue>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ShadowMessageQueue messageQueue = enumerator.Current;
								shadowRedundancyElement.Add(queueDiversity.GetDiagnosticInfo(messageQueue));
							}
							goto IL_B2;
						}
					}
					text = string.Format("Shadow Queues don't have Queue with ID '{0}'", queueDiversity.QueueId.RowId);
				}
				else
				{
					shadowRedundancyElement.Add(queueDiversity.GetComponentAdvice());
				}
			}
			IL_B2:
			if (!string.IsNullOrEmpty(text))
			{
				shadowRedundancyElement.Add(new XElement("Error", text));
			}
		}

		private ShadowRedundancyManager shadowRedundancyManager;

		private IMessagingDatabase database;

		private IBootLoader bootLoader;
	}
}
