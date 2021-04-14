using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Ceres.ContentEngine.Admin.FlowService;
using Microsoft.Ceres.CoreServices.Tools.Management.Client;
using Microsoft.Ceres.SearchCore.Admin.Model;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Fast
{
	internal class FlowManager : FastManagementClient, IFlowManager
	{
		internal FlowManager(ISearchServiceConfig config)
		{
			base.DiagnosticsSession.ComponentName = "FlowManager";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.IndexManagementTracer;
			this.config = config;
		}

		public static IFlowManager Instance
		{
			get
			{
				if (Interlocked.CompareExchange<Hookable<IFlowManager>>(ref FlowManager.hookableInstance, null, null) == null)
				{
					lock (FlowManager.staticLockObject)
					{
						if (FlowManager.hookableInstance == null)
						{
							Hookable<IFlowManager> hookable = Hookable<IFlowManager>.Create(true, new FlowManager(new FlightingSearchConfig()));
							Thread.MemoryBarrier();
							FlowManager.hookableInstance = hookable;
						}
					}
				}
				return FlowManager.hookableInstance.Value;
			}
		}

		protected override int ManagementPortOffset
		{
			get
			{
				return 3;
			}
		}

		protected virtual IFlowServiceManagementAgent CtsFlowService
		{
			get
			{
				return this.ctsFlowService;
			}
		}

		protected virtual IFlowServiceManagementAgent ImsFlowService
		{
			get
			{
				return this.imsFlowService;
			}
		}

		public void RemoveFlowsForIndexSystem(string indexSystemName)
		{
			ICollection<string> flowNamesForIndexSystem = this.GetFlowNamesForIndexSystem(indexSystemName);
			this.RemoveFlows(flowNamesForIndexSystem);
		}

		public IEnumerable<string> GetFlows()
		{
			return this.PerformFastOperation<IList<string>>(() => this.CtsFlowService.GetFlows(), "GetFlows");
		}

		public string GetFlow(string flowName)
		{
			return this.PerformFastOperation<string>(() => this.CtsFlowService.GetFlow(flowName), "GetFlow");
		}

		public XElement GetFlowDiagnostics()
		{
			XElement xelement = new XElement("Flows");
			foreach (string content in this.GetFlows())
			{
				xelement.Add(new XElement("Flow", content));
			}
			return xelement;
		}

		public void EnsureQueryFlows(string indexSystemName)
		{
			ICollection<string> flowNamesForIndexSystem = this.GetFlowNamesForIndexSystem(indexSystemName);
			ICollection<FlowDescriptor> expectedFlowsForIndexSystem = this.GetExpectedFlowsForIndexSystem(indexSystemName);
			bool flag = false;
			if (flowNamesForIndexSystem.Count != expectedFlowsForIndexSystem.Count)
			{
				flag = true;
			}
			else
			{
				foreach (FlowDescriptor flowDescriptor in expectedFlowsForIndexSystem)
				{
					if (!flowNamesForIndexSystem.Contains(flowDescriptor.DisplayName))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return;
			}
			this.RemoveFlows(flowNamesForIndexSystem);
			foreach (FlowDescriptor flowDescriptor2 in expectedFlowsForIndexSystem)
			{
				this.AddIMSFlow(indexSystemName, flowDescriptor2.DisplayName, flowDescriptor2.Template);
			}
		}

		public void EnsureIndexingFlow()
		{
			bool flag = false;
			FlowDescriptor indexingFlowDescriptor = FlowDescriptor.GetIndexingFlowDescriptor(this.config);
			foreach (string text in this.GetFlows())
			{
				bool flag2;
				if (indexingFlowDescriptor.MatchFlowName(text, ref flag2))
				{
					if (flag2)
					{
						this.RemoveCTSFlow(text);
					}
					else
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				this.AddCTSFlow(indexingFlowDescriptor.DisplayName, indexingFlowDescriptor.Template);
			}
		}

		public void EnsureTransportFlow()
		{
			List<string> list = new List<string>(this.GetFlows());
			foreach (FlowDescriptor flowDescriptor in FlowDescriptor.GetTransportFlowDescriptors())
			{
				bool flag = false;
				foreach (string text in list)
				{
					bool flag2;
					if (flowDescriptor.MatchFlowName(text, ref flag2))
					{
						if (flag2)
						{
							this.RemoveCTSFlow(text);
						}
						else
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					this.AddCTSFlow(flowDescriptor.DisplayName, flowDescriptor.Template);
				}
			}
		}

		public ICollection<FlowDescriptor> GetExpectedFlowsForIndexSystem(string indexSystemName)
		{
			return new List<FlowDescriptor>(2)
			{
				FlowDescriptor.GetImsFlowDescriptor(this.config, indexSystemName),
				FlowDescriptor.GetImsInternalFlowDescriptor(this.config, indexSystemName)
			};
		}

		public void AddCtsFlow(string flowName, string flowXML)
		{
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "AddCTSFlow {0}", new object[]
			{
				flowName
			});
			flowXML = flowXML.Replace("[FlowNamePlaceHolder]", flowName);
			base.PerformFastOperation(delegate()
			{
				this.CtsFlowService.PutFlow(flowName, flowXML);
			}, "AddCtsFlow");
		}

		public bool RemoveCTSFlow(string flowName)
		{
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "RemoveCTSFlow {0}", new object[]
			{
				flowName
			});
			bool result;
			try
			{
				base.PerformFastOperation(delegate()
				{
					this.CtsFlowService.DeleteFlow(flowName);
				}, "RemoveCTSFlow");
				result = true;
			}
			catch (Exception arg)
			{
				base.DiagnosticsSession.TraceError<Exception, string>("Caught exception {0} when trying to RemoveFlow {1}", arg, flowName);
				result = false;
			}
			return result;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FlowManager>(this);
		}

		internal static IDisposable SetInstanceTestHook(IFlowManager mockFlowManager)
		{
			if (FlowManager.hookableInstance == null)
			{
				IFlowManager instance = FlowManager.Instance;
			}
			return FlowManager.hookableInstance.SetTestHook(mockFlowManager);
		}

		protected override void InternalConnectManagementAgents(WcfManagementClient client)
		{
			this.ctsFlowService = client.GetManagementAgent<IFlowServiceManagementAgent>("ContentTransformation/FlowService");
			this.imsFlowService = client.GetManagementAgent<IFlowServiceManagementAgent>("InteractionEngine/FlowService");
		}

		private void RemoveIMSFlow(string flowName)
		{
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "RemoveIMSFlow {0}", new object[]
			{
				flowName
			});
			try
			{
				base.PerformFastOperation(delegate()
				{
					this.ImsFlowService.DeleteFlow(flowName);
				}, "RemoveIMSFlow");
			}
			catch (Exception ex)
			{
				base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Caught exception {0} when trying to RemoveFlow {1}", new object[]
				{
					ex,
					flowName
				});
			}
		}

		private void AddCTSFlow(string flowName, string resourceXmlName)
		{
			string flowXmlFromResource = this.GetFlowXmlFromResource(resourceXmlName);
			this.ConfigureCtsFlowWithWordbreakerFieldList(ref flowXmlFromResource);
			this.AddCtsFlow(flowName, flowXmlFromResource);
		}

		private void AddIMSFlow(string indexSystemName, string flowName, string resourceXmlName)
		{
			string text = this.GetFlowXmlFromResource(resourceXmlName);
			text = text.Replace("[IndexSystemNamePlaceHolder]", indexSystemName);
			text = text.Replace("[FlowNamePlaceHolder]", flowName);
			this.AddIMSFlow(flowName, text);
		}

		private void AddIMSFlow(string flowName, string flowXml)
		{
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "AddIMSFlow {0}", new object[]
			{
				flowName
			});
			base.PerformFastOperation(delegate()
			{
				this.ImsFlowService.PutFlow(flowName, flowXml);
			}, "AddIMSFlow");
		}

		private string GetFlowXmlFromResource(string resourceXmlName)
		{
			string result;
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceXmlName))
			{
				using (TextReader textReader = new StreamReader(manifestResourceStream, Encoding.UTF8))
				{
					result = textReader.ReadToEnd();
				}
			}
			return result;
		}

		private void ConfigureCtsFlowWithWordbreakerFieldList(ref string flowXml)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			stringBuilder.AppendFormat("&quot;{0}&quot;", "tempbody");
			foreach (IndexSystemField indexSystemField in FastIndexSystemSchema.GetIndexSystemSchema(0).Fields)
			{
				if (!indexSystemField.NoWordBreaker && indexSystemField.Type == 1)
				{
					stringBuilder.Append(',');
					stringBuilder.AppendFormat("&quot;{0}&quot;", indexSystemField.Name);
				}
			}
			stringBuilder.Append(']');
			flowXml = flowXml.Replace("[WordBreakerPropertyList]", stringBuilder.ToString());
		}

		private ISet<string> GetFlowNamesForIndexSystem(string indexSystemName)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			string value = indexSystemName;
			if (indexSystemName.Length >= 36)
			{
				string text = indexSystemName.Substring(0, 36);
				Guid guid;
				if (Guid.TryParse(text, out guid))
				{
					value = text;
				}
			}
			foreach (string text2 in this.GetFlows())
			{
				if (text2.StartsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					hashSet.Add(text2);
				}
			}
			return hashSet;
		}

		private void RemoveFlows(IEnumerable<string> flows)
		{
			foreach (string flowName in flows)
			{
				this.RemoveCTSFlow(flowName);
			}
		}

		private const string FlowTemplateIndexSystemNamePlaceholder = "[IndexSystemNamePlaceHolder]";

		private const string FlowTemplateFlowNamePlaceholder = "[FlowNamePlaceHolder]";

		private const string FieldsToWordBreakPlaceHolder = "[WordBreakerPropertyList]";

		private const string PropertyFieldFormat = "&quot;{0}&quot;";

		private static Hookable<IFlowManager> hookableInstance;

		private static object staticLockObject = new object();

		private readonly ISearchServiceConfig config;

		private volatile IFlowServiceManagementAgent ctsFlowService;

		private volatile IFlowServiceManagementAgent imsFlowService;
	}
}
