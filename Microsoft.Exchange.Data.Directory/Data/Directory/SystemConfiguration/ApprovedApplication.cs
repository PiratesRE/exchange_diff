using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[ImmutableObject(true)]
	[Serializable]
	public class ApprovedApplication : IComparable, IComparable<ApprovedApplication>
	{
		protected ApprovedApplication(string appHash, string appName, string cabName, bool isFromFile)
		{
			if (appHash == null)
			{
				throw new ArgumentNullException("appHash");
			}
			if (appName == null)
			{
				throw new ArgumentNullException("appName");
			}
			this.appHash = appHash;
			this.appName = appName;
			this.cabName = cabName;
			this.isFromFile = isFromFile;
		}

		protected ApprovedApplication(string cabFile)
		{
			if (cabFile == null)
			{
				throw new ArgumentNullException("cabFile");
			}
			this.cabName = cabFile;
			this.isCab = true;
		}

		public string AppString
		{
			get
			{
				return this.ApprovedApplicationString;
			}
		}

		public string AppHash
		{
			get
			{
				return this.appHash;
			}
		}

		public string AppName
		{
			get
			{
				return this.appName;
			}
		}

		public string CabName
		{
			get
			{
				return this.cabName;
			}
		}

		internal bool IsCab
		{
			get
			{
				return this.isCab;
			}
		}

		internal bool IsFromFile
		{
			get
			{
				return this.isFromFile;
			}
		}

		public static ApprovedApplication Parse(string appValue)
		{
			if (appValue == null)
			{
				throw new ArgumentNullException("appValue");
			}
			string text = null;
			bool flag = false;
			string text2;
			string text3;
			if (appValue.Length > 40 && appValue[40] == ':')
			{
				string[] array = appValue.Split(new char[]
				{
					':'
				});
				if (array == null || array.Length < 2)
				{
					throw new ArgumentException("appValue");
				}
				text2 = array[0];
				text3 = array[1];
				text = ((array.Length > 2) ? array[2] : null);
			}
			else
			{
				FileInfo fileInfo = new FileInfo(appValue);
				if (!fileInfo.Exists)
				{
					throw new FileNotFoundException(appValue);
				}
				if (StringComparer.OrdinalIgnoreCase.Compare(fileInfo.Extension, ".cab") == 0)
				{
					return new ApprovedApplication(appValue);
				}
				text3 = fileInfo.Name;
				text2 = ApprovedApplication.GetSHA1Hash(appValue, false);
				flag = true;
			}
			return new ApprovedApplication(text2, text3, text, flag);
		}

		public static MultiValuedProperty<ApprovedApplication> ParseCab(string cabFile)
		{
			if (cabFile == null)
			{
				throw new ArgumentNullException("cabFile");
			}
			string text = null;
			MultiValuedProperty<ApprovedApplication> result;
			try
			{
				FileInfo fileInfo = new FileInfo(cabFile);
				text = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));
				Directory.CreateDirectory(text);
				ApprovedApplication.OpenCabinetFile(fileInfo.FullName, text);
				result = ApprovedApplication.BuildApprovedApplicationList(text, fileInfo.Name);
			}
			finally
			{
				if (text != null)
				{
					Directory.Delete(text, true);
				}
			}
			return result;
		}

		public static bool operator ==(ApprovedApplication a, ApprovedApplication b)
		{
			return a == b || (a != null && b != null && StringComparer.OrdinalIgnoreCase.Equals(a.AppHash, b.AppHash) && StringComparer.OrdinalIgnoreCase.Equals(a.AppName, b.AppName));
		}

		public static bool operator !=(ApprovedApplication a, ApprovedApplication b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return StringComparer.OrdinalIgnoreCase.GetHashCode(this.AppHash) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(this.AppName);
		}

		public override bool Equals(object obj)
		{
			return this == obj as ApprovedApplication;
		}

		public int CompareTo(object other)
		{
			if (other == null)
			{
				return 1;
			}
			ApprovedApplication approvedApplication = other as ApprovedApplication;
			if (approvedApplication == null)
			{
				throw new ArgumentException(DirectoryStrings.ExInvalidTypeArgumentException("other", other.GetType(), typeof(ApprovedApplication)), "other");
			}
			return this.CompareTo(approvedApplication);
		}

		public int CompareTo(ApprovedApplication other)
		{
			if (null == other)
			{
				return 1;
			}
			int num = StringComparer.OrdinalIgnoreCase.Compare(this.AppName, other.AppName);
			if (num != 0)
			{
				return num;
			}
			return StringComparer.OrdinalIgnoreCase.Compare(this.AppHash, other.AppHash);
		}

		public bool Equals(ApprovedApplication other)
		{
			return this == other;
		}

		internal string ApprovedApplicationString
		{
			get
			{
				if (this.appString == null)
				{
					if (string.IsNullOrEmpty(this.CabName))
					{
						this.appString = string.Format("{0}{1}{2}", this.AppHash, ':', this.AppName);
					}
					else
					{
						this.appString = string.Format("{0}{1}{2}{1}{3}", new object[]
						{
							this.AppHash,
							':',
							this.AppName,
							this.CabName
						});
					}
				}
				return this.appString;
			}
		}

		internal static void SHA1TransformBlock(HashAlgorithm sha1, Stream stream, long readSize)
		{
			byte[] array = new byte[4096];
			int num;
			do
			{
				num = stream.Read(array, 0, (int)((readSize > (long)array.Length) ? ((long)array.Length) : readSize));
				if (num > 0)
				{
					sha1.TransformBlock(array, 0, num, array, 0);
				}
				readSize -= (long)num;
			}
			while (num > 0);
		}

		internal static string GetSHA1Hash(string fileName, bool hashPEOnly)
		{
			byte[] array = null;
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (SHA1Cng sha1Cng = new SHA1Cng())
				{
					List<long[]> list = null;
					if (ApprovedApplication.IsPEFile(fileName, out list))
					{
						for (int i = 0; i < list.Count; i++)
						{
							long num = list[i][0];
							long offset = list[i][1];
							ApprovedApplication.SHA1TransformBlock(sha1Cng, fileStream, num - fileStream.Position);
							fileStream.Seek(offset, SeekOrigin.Current);
						}
						if (fileStream.Position < fileStream.Length)
						{
							ApprovedApplication.SHA1TransformBlock(sha1Cng, fileStream, fileStream.Length - fileStream.Position);
						}
						sha1Cng.TransformFinalBlock(new byte[0], 0, 0);
						array = sha1Cng.Hash;
					}
					else if (!hashPEOnly)
					{
						array = sha1Cng.ComputeHash(fileStream);
					}
				}
			}
			StringBuilder stringBuilder = null;
			if (array != null)
			{
				stringBuilder = new StringBuilder(2 * array.Length);
				for (int j = 0; j < array.Length; j++)
				{
					stringBuilder.AppendFormat("{0:x2}", array[j]);
				}
			}
			if (stringBuilder == null)
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		internal static void OpenCabinetFile(string cabName, string outputPath)
		{
			Process process = Process.Start(new ProcessStartInfo("extrac32.exe", string.Format("/Y /E /L \"{0}\" \"{1}\"", outputPath, cabName))
			{
				CreateNoWindow = true
			});
			process.WaitForExit();
			process.Close();
		}

		internal static bool IsPEFile(string fileName, out List<long[]> skipInterval)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			skipInterval = new List<long[]>();
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					if (fileStream.Length <= 60L)
					{
						return false;
					}
					fileStream.Seek(60L, SeekOrigin.Begin);
					int num = (int)binaryReader.ReadByte();
					if (fileStream.Length < (long)(num + 4))
					{
						return false;
					}
					fileStream.Seek((long)num, SeekOrigin.Begin);
					if (binaryReader.ReadByte() != 80 || binaryReader.ReadByte() != 69 || binaryReader.ReadByte() != 0 || binaryReader.ReadByte() != 0)
					{
						return false;
					}
					if (fileStream.Length < (long)(num + 4 + 20 + 2))
					{
						return false;
					}
					fileStream.Seek(20L, SeekOrigin.Current);
					int num2 = (int)((ushort)binaryReader.ReadInt16());
					int num3 = (num2 == 523) ? 16 : 0;
					if (fileStream.Length < (long)(num + 4 + 20 + 96 + num3))
					{
						return false;
					}
					long[] item = new long[]
					{
						(long)(num + 4 + 20 + 64),
						4L
					};
					skipInterval.Add(item);
					fileStream.Seek((long)(90 + num3), SeekOrigin.Current);
					long num4 = (long)((ulong)binaryReader.ReadInt32());
					if (num4 < 5L)
					{
						return true;
					}
					if (fileStream.Length < (long)(num + 4 + 20 + 96 + num3) + 8L * num4)
					{
						return false;
					}
					fileStream.Seek(32L, SeekOrigin.Current);
					long num5 = (long)((ulong)binaryReader.ReadInt32());
					long num6 = (long)((ulong)binaryReader.ReadInt32());
					if (fileStream.Length < num5 + num6)
					{
						return false;
					}
					item = new long[]
					{
						(long)(num + 4 + 20 + 96 + 32 + num3),
						8L
					};
					skipInterval.Add(item);
					if (num5 > 0L && num6 > 0L)
					{
						item = new long[]
						{
							num5,
							num6
						};
						skipInterval.Add(item);
					}
				}
			}
			return true;
		}

		private static MultiValuedProperty<ApprovedApplication> BuildApprovedApplicationList(string path, string cabName)
		{
			MultiValuedProperty<ApprovedApplication> multiValuedProperty = new MultiValuedProperty<ApprovedApplication>();
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
			foreach (FileInfo fileInfo in files)
			{
				fileInfo.IsReadOnly = false;
				string sha1Hash = ApprovedApplication.GetSHA1Hash(fileInfo.FullName, true);
				if (sha1Hash != null)
				{
					multiValuedProperty.Add(new ApprovedApplication(sha1Hash, fileInfo.Name, cabName, true));
				}
			}
			return multiValuedProperty;
		}

		public override string ToString()
		{
			return this.ApprovedApplicationString;
		}

		internal const char SeparatorCharacter = ':';

		private string appHash;

		private string appName;

		private string cabName;

		private bool isCab;

		private bool isFromFile;

		private string appString;
	}
}
