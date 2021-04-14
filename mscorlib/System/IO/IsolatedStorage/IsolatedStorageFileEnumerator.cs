using System;
using System.Collections;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace System.IO.IsolatedStorage
{
	internal sealed class IsolatedStorageFileEnumerator : IEnumerator
	{
		[SecurityCritical]
		internal IsolatedStorageFileEnumerator(IsolatedStorageScope scope)
		{
			this.m_Scope = scope;
			this.m_fiop = IsolatedStorageFile.GetGlobalFileIOPerm(scope);
			this.m_rootDir = IsolatedStorageFile.GetRootDir(scope);
			this.m_fileEnum = new TwoLevelFileEnumerator(this.m_rootDir);
			this.Reset();
		}

		[SecuritySafeCritical]
		public bool MoveNext()
		{
			this.m_fiop.Assert();
			this.m_fReset = false;
			while (this.m_fileEnum.MoveNext())
			{
				IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile();
				TwoPaths twoPaths = (TwoPaths)this.m_fileEnum.Current;
				bool flag = false;
				if (IsolatedStorageFile.NotAssemFilesDir(twoPaths.Path2) && IsolatedStorageFile.NotAppFilesDir(twoPaths.Path2))
				{
					flag = true;
				}
				Stream stream = null;
				Stream stream2 = null;
				Stream stream3 = null;
				IsolatedStorageScope scope;
				string domainName;
				string assemName;
				string appName;
				if (flag)
				{
					if (!this.GetIDStream(twoPaths.Path1, out stream) || !this.GetIDStream(twoPaths.Path1 + "\\" + twoPaths.Path2, out stream2))
					{
						continue;
					}
					stream.Position = 0L;
					if (IsolatedStorage.IsRoaming(this.m_Scope))
					{
						scope = (IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming);
					}
					else if (IsolatedStorage.IsMachine(this.m_Scope))
					{
						scope = (IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine);
					}
					else
					{
						scope = (IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly);
					}
					domainName = twoPaths.Path1;
					assemName = twoPaths.Path2;
					appName = null;
				}
				else if (IsolatedStorageFile.NotAppFilesDir(twoPaths.Path2))
				{
					if (!this.GetIDStream(twoPaths.Path1, out stream2))
					{
						continue;
					}
					if (IsolatedStorage.IsRoaming(this.m_Scope))
					{
						scope = (IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming);
					}
					else if (IsolatedStorage.IsMachine(this.m_Scope))
					{
						scope = (IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine);
					}
					else
					{
						scope = (IsolatedStorageScope.User | IsolatedStorageScope.Assembly);
					}
					domainName = null;
					assemName = twoPaths.Path1;
					appName = null;
					stream2.Position = 0L;
				}
				else
				{
					if (!this.GetIDStream(twoPaths.Path1, out stream3))
					{
						continue;
					}
					if (IsolatedStorage.IsRoaming(this.m_Scope))
					{
						scope = (IsolatedStorageScope.User | IsolatedStorageScope.Roaming | IsolatedStorageScope.Application);
					}
					else if (IsolatedStorage.IsMachine(this.m_Scope))
					{
						scope = (IsolatedStorageScope.Machine | IsolatedStorageScope.Application);
					}
					else
					{
						scope = (IsolatedStorageScope.User | IsolatedStorageScope.Application);
					}
					domainName = null;
					assemName = null;
					appName = twoPaths.Path1;
					stream3.Position = 0L;
				}
				if (isolatedStorageFile.InitStore(scope, stream, stream2, stream3, domainName, assemName, appName) && isolatedStorageFile.InitExistingStore(scope))
				{
					this.m_Current = isolatedStorageFile;
					return true;
				}
			}
			this.m_fEnd = true;
			return false;
		}

		public object Current
		{
			get
			{
				if (this.m_fReset)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumNotStarted"));
				}
				if (this.m_fEnd)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumEnded"));
				}
				return this.m_Current;
			}
		}

		public void Reset()
		{
			this.m_Current = null;
			this.m_fReset = true;
			this.m_fEnd = false;
			this.m_fileEnum.Reset();
		}

		private bool GetIDStream(string path, out Stream s)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_rootDir);
			stringBuilder.Append(path);
			stringBuilder.Append('\\');
			stringBuilder.Append("identity.dat");
			s = null;
			try
			{
				byte[] buffer;
				using (FileStream fileStream = new FileStream(stringBuilder.ToString(), FileMode.Open))
				{
					int i = (int)fileStream.Length;
					buffer = new byte[i];
					int num = 0;
					while (i > 0)
					{
						int num2 = fileStream.Read(buffer, num, i);
						if (num2 == 0)
						{
							__Error.EndOfFile();
						}
						num += num2;
						i -= num2;
					}
				}
				s = new MemoryStream(buffer);
			}
			catch
			{
				return false;
			}
			return true;
		}

		private const char s_SepExternal = '\\';

		private IsolatedStorageFile m_Current;

		private IsolatedStorageScope m_Scope;

		private FileIOPermission m_fiop;

		private string m_rootDir;

		private TwoLevelFileEnumerator m_fileEnum;

		private bool m_fReset;

		private bool m_fEnd;
	}
}
