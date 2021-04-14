using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class ProcessBasicInfo
	{
		public ProcessBasicInfo(bool isInitializeWithCurrentProcess)
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				this.Initialize(currentProcess);
			}
		}

		public ProcessBasicInfo(int id)
		{
			using (Process processById = Process.GetProcessById(id))
			{
				this.Initialize(processById);
			}
		}

		public ProcessBasicInfo(Process process)
		{
			this.Initialize(process);
		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public DateTimeOffset StartTime { get; set; }

		internal void Initialize(Process process)
		{
			if (process != null)
			{
				this.Id = process.Id;
				this.Name = process.ProcessName;
				try
				{
					this.StartTime = process.StartTime;
				}
				catch
				{
					this.StartTime = DateTime.MinValue;
				}
			}
		}
	}
}
