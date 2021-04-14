using System;
using System.Globalization;
using System.IO;
using System.Security;

namespace Microsoft.Exchange.Diagnostics
{
	internal class ConfigFileFingerPrint : IEquatable<ConfigFileFingerPrint>
	{
		internal ConfigFileFingerPrint(string filePath)
		{
			Exception ex = null;
			bool flag = false;
			try
			{
				this.filePath = filePath;
				if (File.Exists(filePath))
				{
					FileInfo fileInfo = new FileInfo(filePath);
					this.CreatedTimeUtc = fileInfo.CreationTimeUtc;
					this.ModifiedTimeUtc = fileInfo.LastWriteTimeUtc;
					this.Size = fileInfo.Length;
					this.fileExists = true;
				}
				return;
			}
			catch (NotSupportedException ex2)
			{
				flag = true;
				ex = ex2;
			}
			catch (PathTooLongException ex3)
			{
				flag = true;
				ex = ex3;
			}
			catch (ArgumentException ex4)
			{
				flag = true;
				ex = ex4;
			}
			catch (UnauthorizedAccessException ex5)
			{
				flag = true;
				ex = ex5;
			}
			catch (SecurityException ex6)
			{
				flag = true;
				ex = ex6;
			}
			catch (DirectoryNotFoundException ex7)
			{
				ex = ex7;
			}
			catch (FileNotFoundException ex8)
			{
				ex = ex8;
			}
			catch (IOException ex9)
			{
				flag = true;
				ex = ex9;
			}
			if (ex != null)
			{
				this.fileExists = false;
				if (flag)
				{
					InternalBypassTrace.FaultInjectionConfigurationTracer.TraceError(0, 0L, "Unexpected exception reading file {0}, Exception={1}", new object[]
					{
						this.filePath,
						ex
					});
				}
			}
		}

		private ConfigFileFingerPrint()
		{
		}

		internal long Size { get; private set; }

		internal DateTime CreatedTimeUtc { get; private set; }

		internal DateTime ModifiedTimeUtc { get; private set; }

		public bool Equals(ConfigFileFingerPrint other)
		{
			return other != null && (this.fileExists == other.fileExists && StringComparer.OrdinalIgnoreCase.Equals(this.filePath, other.filePath) && this.Size == other.Size && this.ModifiedTimeUtc == other.ModifiedTimeUtc) && this.CreatedTimeUtc == other.CreatedTimeUtc;
		}

		public override bool Equals(object obj)
		{
			ConfigFileFingerPrint other = obj as ConfigFileFingerPrint;
			return this.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.traceValue))
			{
				if (this.fileExists)
				{
					this.traceValue = string.Format(CultureInfo.InvariantCulture, "File={0}, Created={1}, Modified={2}, Size={3}", new object[]
					{
						this.filePath,
						this.CreatedTimeUtc,
						this.ModifiedTimeUtc,
						this.Size
					});
				}
				else
				{
					this.traceValue = string.Format(CultureInfo.InvariantCulture, "File={0}, Does not exist", new object[]
					{
						this.filePath
					});
				}
			}
			return this.traceValue;
		}

		private string traceValue;

		private string filePath;

		private bool fileExists;
	}
}
