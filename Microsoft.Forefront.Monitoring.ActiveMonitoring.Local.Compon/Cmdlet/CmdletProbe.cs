using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Xml.Serialization;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Cmdlet
{
	public class CmdletProbe : ProbeWorkItem
	{
		private CmdletProbe.CmdletsWorkflowDefinition WorkflowDefinition
		{
			get
			{
				if (this.cmdletsWorkflowDefinition == null)
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(CmdletProbe.CmdletsWorkflowDefinition));
					this.cmdletsWorkflowDefinition = (CmdletProbe.CmdletsWorkflowDefinition)xmlSerializer.Deserialize(new StringReader(base.Definition.ExtensionAttributes));
				}
				return this.cmdletsWorkflowDefinition;
			}
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			Runspace runspace = null;
			try
			{
				this.WriteResult("CmdletProbe started. ", new object[0]);
				RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
				this.LoadSnapins(runspaceConfiguration);
				runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
				runspace.Open();
				this.ExecuteCmdlets(runspace, cancellationToken);
				this.WriteResult("CmdletProbe finished.", new object[0]);
			}
			catch (Exception ex)
			{
				this.WriteResult("Exception: {0} ", new object[]
				{
					ex
				});
				throw;
			}
			finally
			{
				if (runspace != null)
				{
					runspace.Close();
				}
			}
		}

		private void ExecuteCmdlets(Runspace runspace, CancellationToken cancellationToken)
		{
			Pipeline pipeline = null;
			CmdletProbe.CmdletWorkItem[] array = this.WorkflowDefinition.Cmdlets ?? new CmdletProbe.CmdletWorkItem[0];
			if (array.Length > 0 && array.Last<CmdletProbe.CmdletWorkItem>().IsPiped)
			{
				throw new InvalidDataException(string.Format("The last cmdlet {0} must not have have isPiped as true", array.LastOrDefault<CmdletProbe.CmdletWorkItem>()));
			}
			foreach (CmdletProbe.CmdletWorkItem cmdletWorkItem in array)
			{
				this.WriteResult("Executing cmdlet: {0}", new object[]
				{
					cmdletWorkItem
				});
				if (cancellationToken.IsCancellationRequested)
				{
					this.WriteResult("Cancellation Requested", new object[0]);
					break;
				}
				Command command = new Command(cmdletWorkItem.Name);
				foreach (CmdletProbe.CmdletParameter cmdletParameter in cmdletWorkItem.Parameters ?? new CmdletProbe.CmdletParameter[0])
				{
					command.Parameters.Add(cmdletParameter.Name, cmdletParameter.Value);
				}
				if (pipeline == null)
				{
					pipeline = runspace.CreatePipeline();
				}
				pipeline.Commands.Add(command);
				if (!cmdletWorkItem.IsPiped)
				{
					pipeline.Invoke();
					PipelineReader<object> error = pipeline.Error;
					pipeline = null;
					if (error.Count > 0)
					{
						throw new InvalidOperationException(string.Format("Cmdlet returned Error {0}", string.Join<object>(", ", error.ReadToEnd())));
					}
				}
			}
		}

		private void LoadSnapins(RunspaceConfiguration rsConfig)
		{
			CmdletProbe.CmdletSnapin[] array = this.WorkflowDefinition.Snapins ?? new CmdletProbe.CmdletSnapin[0];
			foreach (CmdletProbe.CmdletSnapin cmdletSnapin in array)
			{
				PSSnapInException ex;
				rsConfig.AddPSSnapIn(cmdletSnapin.Name, out ex);
				if (ex != null)
				{
					throw ex;
				}
			}
			this.WriteResult("Loaded the following Snapins: {0}", new object[]
			{
				string.Join<CmdletProbe.CmdletSnapin>(", ", array)
			});
		}

		private void WriteResult(string format, params object[] objs)
		{
			ProbeResult result = base.Result;
			result.ExecutionContext += string.Format(format, objs);
			ProbeResult result2 = base.Result;
			result2.ExecutionContext += "|";
		}

		private CmdletProbe.CmdletsWorkflowDefinition cmdletsWorkflowDefinition;

		public class CmdletsWorkflowDefinition
		{
			[XmlArrayItem("Snapin")]
			public CmdletProbe.CmdletSnapin[] Snapins { get; set; }

			[XmlArrayItem("Cmdlet")]
			public CmdletProbe.CmdletWorkItem[] Cmdlets { get; set; }
		}

		public class CmdletWorkItem
		{
			[XmlAttribute]
			public string Name { get; set; }

			[XmlArrayItem("Parameter")]
			public CmdletProbe.CmdletParameter[] Parameters { get; set; }

			[XmlAttribute]
			[DefaultValue(false)]
			public bool IsPiped { get; set; }

			[XmlIgnore]
			public List<object> Outputs
			{
				get
				{
					List<object> result;
					if ((result = this.outputs) == null)
					{
						result = (this.outputs = new List<object>());
					}
					return result;
				}
			}

			public override string ToString()
			{
				return string.Format("{0} {1}", this.Name, (this.Parameters != null) ? string.Join<CmdletProbe.CmdletParameter>(", ", this.Parameters) : string.Empty);
			}

			private List<object> outputs;
		}

		public class CmdletParameter
		{
			[XmlAttribute]
			public string Name { get; set; }

			[XmlAttribute]
			public string Value { get; set; }

			public override string ToString()
			{
				if (this.Name == null)
				{
					return this.Value;
				}
				return this.Name + " : " + this.Value;
			}
		}

		public class CmdletSnapin
		{
			[XmlAttribute]
			public string Name { get; set; }

			public override string ToString()
			{
				return this.Name;
			}
		}
	}
}
