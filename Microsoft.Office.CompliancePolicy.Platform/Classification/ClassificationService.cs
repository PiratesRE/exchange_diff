using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	public sealed class ClassificationService
	{
		public ClassificationService(IClassificationRuleStore classificationRuleStore, ClassificationConfiguration classificationConfiguration, ExecutionLog executionLog)
		{
			if (classificationRuleStore == null)
			{
				throw new ArgumentNullException("classificationRuleStore");
			}
			if (classificationConfiguration == null)
			{
				throw new ArgumentNullException("classificationConfiguration");
			}
			if (executionLog == null)
			{
				throw new ArgumentNullException("executionLog");
			}
			this.classificationRuleStore = classificationRuleStore;
			this.classificationConfiguration = classificationConfiguration;
			this.executionLog = executionLog;
		}

		private MicrosoftClassificationEngine ClassificationEngine
		{
			get
			{
				if (this.classificationEngine == null)
				{
					lock (this.lockObj)
					{
						if (this.classificationEngine == null)
						{
							this.executionLog.LogOneEntry(ExecutionLog.EventType.Information, "ClassificationService", this.correlationId, "Initializing ClassificationEngine", new object[0]);
							this.classificationEngine = new MicrosoftClassificationEngine();
							this.classificationEngine.Init(this.classificationConfiguration.PropertyBag, this.classificationRuleStore);
						}
					}
				}
				return this.classificationEngine;
			}
		}

		public bool Classify(IClassificationItem classificationItem)
		{
			if (classificationItem == null)
			{
				this.executionLog.LogOneEntry(ExecutionLog.EventType.Verbose, "ClassificationService", this.correlationId, "Unable to classify: classificationItem is null", new object[0]);
				return false;
			}
			Stream content = classificationItem.Content;
			if (content == null)
			{
				this.executionLog.LogOneEntry(ExecutionLog.EventType.Verbose, "ClassificationService", this.correlationId, "Unable to classify [{0}]: no classificationItem doesn't have any content", new object[]
				{
					classificationItem.ItemId
				});
				return false;
			}
			RULE_PACKAGE_DETAILS[] rulePackageDetails = this.classificationRuleStore.GetRulePackageDetails(classificationItem);
			if (rulePackageDetails == null || rulePackageDetails.Length == 0)
			{
				this.executionLog.LogOneEntry(ExecutionLog.EventType.Verbose, "ClassificationService", this.correlationId, "Unable to classify [{0}]: no rules identified for classificationItem", new object[]
				{
					classificationItem.ItemId
				});
				return false;
			}
			this.executionLog.LogOneEntry(ExecutionLog.EventType.Verbose, "ClassificationService", this.correlationId, "Classifying [{0}]", new object[]
			{
				classificationItem.ItemId
			});
			ICAClassificationResultCollection classificationResults = this.ClassificationEngine.ClassifyTextStream(new ClassificationService.ReadOnlyNoSeekStreamWrapper(content), (uint)rulePackageDetails.Length, rulePackageDetails);
			this.executionLog.LogOneEntry(ExecutionLog.EventType.Verbose, "ClassificationService", this.correlationId, "Setting classification results for [{0}]", new object[]
			{
				classificationItem.ItemId
			});
			classificationItem.SetClassificationResults(classificationResults);
			return true;
		}

		public const string ConfidenceLevelResultPropertyName = "AFF85B32-1BA9-4EDE-9286-F08A7EE5A421";

		public const string CountResultPropertyName = "BD770258-EA9C-4162-B79C-7AD408EC7CD5";

		private const string ClientString = "ClassificationService";

		private readonly string correlationId = Guid.NewGuid().ToString();

		private object lockObj = new object();

		private ClassificationConfiguration classificationConfiguration;

		private IClassificationRuleStore classificationRuleStore;

		private ExecutionLog executionLog;

		private MicrosoftClassificationEngine classificationEngine;

		private class ReadOnlyNoSeekStreamWrapper : IStream
		{
			internal ReadOnlyNoSeekStreamWrapper(Stream stream)
			{
				if (stream == null)
				{
					throw new ArgumentNullException("stream");
				}
				if (!stream.CanRead)
				{
					throw new ArgumentException("stream argument must allow reads");
				}
				this.stream = stream;
			}

			public void Clone(out IStream ppstm)
			{
				ppstm = null;
				Marshal.ThrowExceptionForHR(-2147287039);
			}

			public void Commit(int grfCommitFlags)
			{
				Marshal.ThrowExceptionForHR(-2147287039);
			}

			public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
			{
				long num = 0L;
				long num2 = 0L;
				if (pstm != null)
				{
					int num3 = 0;
					IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(num3));
					byte[] array = new byte[8192];
					bool flag = false;
					while (!flag && cb > 0L)
					{
						int count = (cb > 8192L) ? 8192 : ((int)cb);
						int num4 = this.stream.Read(array, 0, count);
						num += (long)num4;
						if (num4 > 0)
						{
							pstm.Write(array, num4, intPtr);
							num3 = Marshal.ReadInt32(intPtr);
							num2 += (long)num3;
							cb -= (long)num4;
						}
						else
						{
							flag = true;
						}
					}
				}
				if (pcbRead != IntPtr.Zero)
				{
					Marshal.WriteInt64(pcbRead, num);
				}
				if (pcbWritten != IntPtr.Zero)
				{
					Marshal.WriteInt64(pcbWritten, num2);
				}
			}

			public void LockRegion(long libOffset, long cb, int dwLockType)
			{
				Marshal.ThrowExceptionForHR(-2147287039);
			}

			public void Read(byte[] pv, int cb, IntPtr pcbRead)
			{
				int val = this.stream.Read(pv, 0, cb);
				if (pcbRead != IntPtr.Zero)
				{
					Marshal.WriteInt32(pcbRead, val);
				}
			}

			public void Revert()
			{
				Marshal.ThrowExceptionForHR(-2147287039);
			}

			public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
			{
				Marshal.ThrowExceptionForHR(-2147287039);
			}

			public void SetSize(long libNewSize)
			{
				Marshal.ThrowExceptionForHR(-2147287039);
			}

			public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
			{
				pstatstg = new System.Runtime.InteropServices.ComTypes.STATSTG
				{
					type = 2,
					cbSize = this.stream.Length,
					grfLocksSupported = 0,
					grfStateBits = 0
				};
			}

			public void UnlockRegion(long libOffset, long cb, int dwLockType)
			{
				Marshal.ThrowExceptionForHR(-2147287039);
			}

			public void Write(byte[] pv, int cb, IntPtr pcbWritten)
			{
				Marshal.ThrowExceptionForHR(-2147287039);
			}

			private const int INVALIDFUNCTION = -2147287039;

			private const int BufferSize = 8192;

			private Stream stream;
		}
	}
}
