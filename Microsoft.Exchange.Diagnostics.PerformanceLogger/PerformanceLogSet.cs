using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics.Service.Common;
using PlaLibrary;
using TaskScheduler;

namespace Microsoft.Exchange.Diagnostics.PerformanceLogger
{
	public class PerformanceLogSet
	{
		public PerformanceLogSet(string performanceLogXmlFile, string counterSetName) : this(performanceLogXmlFile, counterSetName, null, new PerformanceLogSet.PerformanceLogFormat?(PerformanceLogSet.PerformanceLogFormat.CSV), new PerformanceLogSet.FileNameFormatPattern?(PerformanceLogSet.FileNameFormatPattern.NNNNNN), TimeSpan.FromSeconds(15.0), TimeSpan.FromMinutes(5.0))
		{
		}

		public PerformanceLogSet(string performanceLogXmlFile, string counterSetName, string performanceLogPath) : this(performanceLogXmlFile, counterSetName, performanceLogPath, new PerformanceLogSet.PerformanceLogFormat?(PerformanceLogSet.PerformanceLogFormat.CSV))
		{
		}

		public PerformanceLogSet(string performanceLogXmlFile, string counterSetName, string performanceLogPath, PerformanceLogSet.PerformanceLogFormat? logFormat) : this(performanceLogXmlFile, counterSetName, performanceLogPath, logFormat, new PerformanceLogSet.FileNameFormatPattern?(PerformanceLogSet.FileNameFormatPattern.MMddHHmm), TimeSpan.FromSeconds(15.0), TimeSpan.FromMinutes(5.0))
		{
		}

		public PerformanceLogSet(string performanceLogXmlFile, string counterSetName, string performanceLogPath, PerformanceLogSet.PerformanceLogFormat? logFormat, PerformanceLogSet.FileNameFormatPattern? fileNameFormat, TimeSpan sampleInterval, TimeSpan maximumDuration)
		{
			Directory.CreateDirectory(performanceLogPath);
			this.counterSetName = counterSetName;
			this.performanceLogXmlFile = performanceLogXmlFile;
			this.performanceLogPath = performanceLogPath;
			this.logFormat = logFormat;
			this.fileNameFormat = fileNameFormat;
			this.sampleInterval = sampleInterval;
			this.maximumDuration = maximumDuration;
		}

		public string CounterSetName
		{
			get
			{
				return this.counterSetName;
			}
		}

		public PerformanceLogSet.PerformanceLogSetStatus Status
		{
			get
			{
				PerformanceLogSet.PerformanceLogSetStatus result = PerformanceLogSet.PerformanceLogSetStatus.DoesNotExist;
				try
				{
					IRegisteredTask task = PerformanceLogSet.GetTask(this.counterSetName);
					if (task.State == 4)
					{
						result = PerformanceLogSet.PerformanceLogSetStatus.Running;
					}
					else
					{
						result = PerformanceLogSet.PerformanceLogSetStatus.Stopped;
					}
				}
				catch (FileNotFoundException)
				{
				}
				catch (Exception ex)
				{
					Logger.LogErrorMessage("Exception querying task scheduler: {0}", new object[]
					{
						ex
					});
				}
				return result;
			}
		}

		public static void DeleteTask(string counterSetName)
		{
			ITaskService taskService = new TaskSchedulerClass();
			taskService.Connect(Missing.Value, Missing.Value, Missing.Value, Missing.Value);
			try
			{
				ITaskFolder folder = taskService.GetFolder("\\Microsoft\\Windows\\PLA");
				folder.DeleteTask(counterSetName, 0);
			}
			catch (Exception)
			{
			}
		}

		public static void StopCounterSet(string counterSetName)
		{
			try
			{
				IRegisteredTask task = PerformanceLogSet.GetTask(counterSetName);
				task.Stop(0);
			}
			catch (FileNotFoundException)
			{
			}
		}

		public bool LogStatus()
		{
			return this.Status == PerformanceLogSet.PerformanceLogSetStatus.Running;
		}

		public bool Exists()
		{
			return this.Status != PerformanceLogSet.PerformanceLogSetStatus.DoesNotExist;
		}

		public void StartLog(bool synchronous)
		{
			try
			{
				Logger.LogInformationMessage("Starting counter set name {0}", new object[]
				{
					this.counterSetName
				});
				DataCollectorSet dataCollectorSet = this.ConstructDataCollectorSet();
				dataCollectorSet.start(synchronous);
			}
			catch (COMException ex)
			{
				if (-2147216609 != ex.ErrorCode)
				{
					throw;
				}
			}
		}

		public void StopLog(bool synchronous)
		{
			try
			{
				Logger.LogInformationMessage("Stopping counter set name {0}", new object[]
				{
					this.counterSetName
				});
				PerformanceLogSet.StopCounterSet(this.counterSetName);
				this.ConstructDataCollectorSet();
			}
			catch (COMException ex)
			{
				if (-2144337918 != ex.ErrorCode && -2144337660 != ex.ErrorCode)
				{
					throw;
				}
			}
		}

		public void DeleteLogSettings()
		{
			if (this.Status != PerformanceLogSet.PerformanceLogSetStatus.DoesNotExist)
			{
				try
				{
					this.StopLog(true);
					Logger.LogInformationMessage("Deleting counter set name {0}", new object[]
					{
						this.counterSetName
					});
					DataCollectorSet dataCollectorSet = this.ConstructDataCollectorSet();
					dataCollectorSet.Delete();
				}
				catch (COMException ex)
				{
					if (-2144337918 != ex.ErrorCode)
					{
						throw;
					}
				}
			}
		}

		public void CreateLogSettings()
		{
			Logger.LogInformationMessage("Creating counter set name {0}", new object[]
			{
				this.counterSetName
			});
			DataCollectorSet dataCollectorSet = this.ConstructDataCollectorSet();
			try
			{
				if (dataCollectorSet.Status != null)
				{
					dataCollectorSet.Stop(true);
					dataCollectorSet.Delete();
				}
			}
			catch (Exception)
			{
			}
			dataCollectorSet.Commit(this.counterSetName, null, 3);
		}

		private static IRegisteredTask GetTask(string counterSetName)
		{
			ITaskService taskService = new TaskSchedulerClass();
			taskService.Connect(null, null, null, null);
			ITaskFolder folder = taskService.GetFolder("\\Microsoft\\Windows\\PLA");
			return folder.GetTask(counterSetName);
		}

		private DataCollectorSet ConstructDataCollectorSet()
		{
			if (string.IsNullOrEmpty(this.performanceLogXmlFile))
			{
				throw new ArgumentException("performanceLogXmlFile");
			}
			if (!File.Exists(this.performanceLogXmlFile))
			{
				throw new ArgumentException(string.Format("PerformanceLogXmlFile {0} does not exist", this.performanceLogXmlFile));
			}
			string xml = File.ReadAllText(this.performanceLogXmlFile);
			if (PerformanceLogSet.twentyFourHours < this.sampleInterval)
			{
				throw new ArgumentOutOfRangeException("sampleInterval", string.Format("Must be less than or equal to {0} hours", PerformanceLogSet.twentyFourHours.ToString()));
			}
			if (PerformanceLogSet.twentyFourHours < this.maximumDuration)
			{
				throw new ArgumentOutOfRangeException("maximumDuration", string.Format("Must be less than or equal to {0} hours", PerformanceLogSet.twentyFourHours.ToString()));
			}
			DataCollectorSet dataCollectorSet = new DataCollectorSetClass();
			dataCollectorSet.SetXml(xml);
			dataCollectorSet.Security = null;
			IDataCollectorCollection dataCollectors = dataCollectorSet.DataCollectors;
			if (1 != dataCollectors.Count || dataCollectors[0].DataCollectorType != null)
			{
				throw new ArgumentException("performanceLogXmlFile is invalid. It must contain only 1 data collected of type Performance Counter");
			}
			IPerformanceCounterDataCollector performanceCounterDataCollector = (IPerformanceCounterDataCollector)dataCollectors[0];
			if (this.counterSetName == null)
			{
				throw new ArgumentNullException("counterSetName");
			}
			dataCollectorSet.DisplayName = this.counterSetName;
			performanceCounterDataCollector.FileName = this.counterSetName;
			if (this.performanceLogPath != null)
			{
				dataCollectorSet.RootPath = this.performanceLogPath;
			}
			if (this.logFormat != null)
			{
				performanceCounterDataCollector.LogFileFormat = this.logFormat.Value;
			}
			if (this.fileNameFormat != null)
			{
				performanceCounterDataCollector.FileNameFormat = this.fileNameFormat.Value;
			}
			if (0.0 != this.maximumDuration.TotalSeconds)
			{
				dataCollectorSet.SegmentMaxDuration = (uint)this.maximumDuration.TotalSeconds;
			}
			if (0.0 != this.sampleInterval.TotalSeconds)
			{
				performanceCounterDataCollector.SampleInterval = (uint)this.sampleInterval.TotalSeconds;
			}
			dataCollectorSet.Commit(this.counterSetName, null, 4096);
			return dataCollectorSet;
		}

		private const int HResultDataCollectorSetNotFound = -2144337918;

		private const int HResultDataCollectorSetNotRunning = -2144337660;

		private const int HResultDataCollectorSetAlreadyRunning = -2147216609;

		private static readonly TimeSpan twentyFourHours = TimeSpan.FromHours(24.0);

		private readonly string counterSetName;

		private readonly string performanceLogXmlFile;

		private readonly string performanceLogPath;

		private readonly PerformanceLogSet.PerformanceLogFormat? logFormat;

		private readonly PerformanceLogSet.FileNameFormatPattern? fileNameFormat;

		private readonly TimeSpan sampleInterval;

		private readonly TimeSpan maximumDuration;

		public enum PerformanceLogFormat
		{
			CSV,
			TSV,
			SQL,
			BIN
		}

		public enum FileNameFormatPattern
		{
			MMddHH = 256,
			NNNNNN = 512,
			yyyyDDD = 1024,
			yyyyMM = 2048,
			yyyyMMdd = 4096,
			yyyyMMddHH = 8192,
			MMddHHmm = 16384
		}

		public enum PerformanceLogSetStatus
		{
			DoesNotExist,
			Stopped,
			Running
		}
	}
}
