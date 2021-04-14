using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.IO.IsolatedStorage
{
	[ComVisible(true)]
	public sealed class IsolatedStorageFile : IsolatedStorage, IDisposable
	{
		internal IsolatedStorageFile()
		{
		}

		public static IsolatedStorageFile GetUserStoreForDomain()
		{
			return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
		}

		public static IsolatedStorageFile GetUserStoreForAssembly()
		{
			return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
		}

		public static IsolatedStorageFile GetUserStoreForApplication()
		{
			return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Application, null);
		}

		[ComVisible(false)]
		public static IsolatedStorageFile GetUserStoreForSite()
		{
			throw new NotSupportedException(Environment.GetResourceString("IsolatedStorage_NotValidOnDesktop"));
		}

		public static IsolatedStorageFile GetMachineStoreForDomain()
		{
			return IsolatedStorageFile.GetStore(IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine, null, null);
		}

		public static IsolatedStorageFile GetMachineStoreForAssembly()
		{
			return IsolatedStorageFile.GetStore(IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine, null, null);
		}

		public static IsolatedStorageFile GetMachineStoreForApplication()
		{
			return IsolatedStorageFile.GetStore(IsolatedStorageScope.Machine | IsolatedStorageScope.Application, null);
		}

		[SecuritySafeCritical]
		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, Type domainEvidenceType, Type assemblyEvidenceType)
		{
			if (domainEvidenceType != null)
			{
				IsolatedStorageFile.DemandAdminPermission();
			}
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile();
			isolatedStorageFile.InitStore(scope, domainEvidenceType, assemblyEvidenceType);
			isolatedStorageFile.Init(scope);
			return isolatedStorageFile;
		}

		internal void EnsureStoreIsValid()
		{
			if (this.m_bDisposed)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			if (this.m_closed)
			{
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
		}

		[SecuritySafeCritical]
		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, object domainIdentity, object assemblyIdentity)
		{
			if (assemblyIdentity == null)
			{
				throw new ArgumentNullException("assemblyIdentity");
			}
			if (IsolatedStorage.IsDomain(scope) && domainIdentity == null)
			{
				throw new ArgumentNullException("domainIdentity");
			}
			IsolatedStorageFile.DemandAdminPermission();
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile();
			isolatedStorageFile.InitStore(scope, domainIdentity, assemblyIdentity, null);
			isolatedStorageFile.Init(scope);
			return isolatedStorageFile;
		}

		[SecuritySafeCritical]
		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, Evidence domainEvidence, Type domainEvidenceType, Evidence assemblyEvidence, Type assemblyEvidenceType)
		{
			if (assemblyEvidence == null)
			{
				throw new ArgumentNullException("assemblyEvidence");
			}
			if (IsolatedStorage.IsDomain(scope) && domainEvidence == null)
			{
				throw new ArgumentNullException("domainEvidence");
			}
			IsolatedStorageFile.DemandAdminPermission();
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile();
			isolatedStorageFile.InitStore(scope, domainEvidence, domainEvidenceType, assemblyEvidence, assemblyEvidenceType, null, null);
			isolatedStorageFile.Init(scope);
			return isolatedStorageFile;
		}

		[SecuritySafeCritical]
		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, Type applicationEvidenceType)
		{
			if (applicationEvidenceType != null)
			{
				IsolatedStorageFile.DemandAdminPermission();
			}
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile();
			isolatedStorageFile.InitStore(scope, applicationEvidenceType);
			isolatedStorageFile.Init(scope);
			return isolatedStorageFile;
		}

		[SecuritySafeCritical]
		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, object applicationIdentity)
		{
			if (applicationIdentity == null)
			{
				throw new ArgumentNullException("applicationIdentity");
			}
			IsolatedStorageFile.DemandAdminPermission();
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile();
			isolatedStorageFile.InitStore(scope, null, null, applicationIdentity);
			isolatedStorageFile.Init(scope);
			return isolatedStorageFile;
		}

		public override long UsedSize
		{
			[SecuritySafeCritical]
			get
			{
				if (base.IsRoaming())
				{
					throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_CurrentSizeUndefined"));
				}
				object internalLock = this.m_internalLock;
				long usage;
				lock (internalLock)
				{
					if (this.m_bDisposed)
					{
						throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
					}
					if (this.m_closed)
					{
						throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
					}
					if (this.InvalidFileHandle)
					{
						this.m_handle = IsolatedStorageFile.Open(this.m_InfoFile, this.GetSyncObjectName());
					}
					usage = (long)IsolatedStorageFile.GetUsage(this.m_handle);
				}
				return usage;
			}
		}

		[CLSCompliant(false)]
		[Obsolete("IsolatedStorageFile.CurrentSize has been deprecated because it is not CLS Compliant.  To get the current size use IsolatedStorageFile.UsedSize")]
		public override ulong CurrentSize
		{
			[SecuritySafeCritical]
			get
			{
				if (base.IsRoaming())
				{
					throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_CurrentSizeUndefined"));
				}
				object internalLock = this.m_internalLock;
				ulong usage;
				lock (internalLock)
				{
					if (this.m_bDisposed)
					{
						throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
					}
					if (this.m_closed)
					{
						throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
					}
					if (this.InvalidFileHandle)
					{
						this.m_handle = IsolatedStorageFile.Open(this.m_InfoFile, this.GetSyncObjectName());
					}
					usage = IsolatedStorageFile.GetUsage(this.m_handle);
				}
				return usage;
			}
		}

		[ComVisible(false)]
		public override long AvailableFreeSpace
		{
			[SecuritySafeCritical]
			get
			{
				if (base.IsRoaming())
				{
					return long.MaxValue;
				}
				object internalLock = this.m_internalLock;
				long usage;
				lock (internalLock)
				{
					if (this.m_bDisposed)
					{
						throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
					}
					if (this.m_closed)
					{
						throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
					}
					if (this.InvalidFileHandle)
					{
						this.m_handle = IsolatedStorageFile.Open(this.m_InfoFile, this.GetSyncObjectName());
					}
					usage = (long)IsolatedStorageFile.GetUsage(this.m_handle);
				}
				return this.Quota - usage;
			}
		}

		[ComVisible(false)]
		public override long Quota
		{
			get
			{
				if (base.IsRoaming())
				{
					return long.MaxValue;
				}
				return base.Quota;
			}
			[SecuritySafeCritical]
			internal set
			{
				bool flag = false;
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					this.Lock(ref flag);
					object internalLock = this.m_internalLock;
					lock (internalLock)
					{
						if (this.InvalidFileHandle)
						{
							this.m_handle = IsolatedStorageFile.Open(this.m_InfoFile, this.GetSyncObjectName());
						}
						IsolatedStorageFile.SetQuota(this.m_handle, value);
					}
				}
				finally
				{
					if (flag)
					{
						this.Unlock();
					}
				}
				base.Quota = value;
			}
		}

		[ComVisible(false)]
		public static bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		[CLSCompliant(false)]
		[Obsolete("IsolatedStorageFile.MaximumSize has been deprecated because it is not CLS Compliant.  To get the maximum size use IsolatedStorageFile.Quota")]
		public override ulong MaximumSize
		{
			get
			{
				if (base.IsRoaming())
				{
					return 9223372036854775807UL;
				}
				return base.MaximumSize;
			}
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public override bool IncreaseQuotaTo(long newQuotaSize)
		{
			if (newQuotaSize <= this.Quota)
			{
				throw new ArgumentException(Environment.GetResourceString("IsolatedStorage_OldQuotaLarger"));
			}
			if (this.m_StoreScope != (IsolatedStorageScope.User | IsolatedStorageScope.Application))
			{
				throw new NotSupportedException(Environment.GetResourceString("IsolatedStorage_OnlyIncreaseUserApplicationStore"));
			}
			IsolatedStorageSecurityState isolatedStorageSecurityState = IsolatedStorageSecurityState.CreateStateToIncreaseQuotaForApplication(newQuotaSize, this.Quota - this.AvailableFreeSpace);
			try
			{
				isolatedStorageSecurityState.EnsureState();
			}
			catch (IsolatedStorageException)
			{
				return false;
			}
			this.Quota = newQuotaSize;
			return true;
		}

		[SecuritySafeCritical]
		internal void Reserve(ulong lReserve)
		{
			if (base.IsRoaming())
			{
				return;
			}
			ulong quota = (ulong)this.Quota;
			object internalLock = this.m_internalLock;
			lock (internalLock)
			{
				if (this.m_bDisposed)
				{
					throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
				}
				if (this.m_closed)
				{
					throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
				}
				if (this.InvalidFileHandle)
				{
					this.m_handle = IsolatedStorageFile.Open(this.m_InfoFile, this.GetSyncObjectName());
				}
				IsolatedStorageFile.Reserve(this.m_handle, quota, lReserve, false);
			}
		}

		internal void Unreserve(ulong lFree)
		{
			if (base.IsRoaming())
			{
				return;
			}
			ulong quota = (ulong)this.Quota;
			this.Unreserve(lFree, quota);
		}

		[SecuritySafeCritical]
		internal void Unreserve(ulong lFree, ulong quota)
		{
			if (base.IsRoaming())
			{
				return;
			}
			object internalLock = this.m_internalLock;
			lock (internalLock)
			{
				if (this.m_bDisposed)
				{
					throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
				}
				if (this.m_closed)
				{
					throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
				}
				if (this.InvalidFileHandle)
				{
					this.m_handle = IsolatedStorageFile.Open(this.m_InfoFile, this.GetSyncObjectName());
				}
				IsolatedStorageFile.Reserve(this.m_handle, quota, lFree, true);
			}
		}

		[SecuritySafeCritical]
		public void DeleteFile(string file)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			this.m_fiop.Assert();
			this.m_fiop.PermitOnly();
			long num = 0L;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this.Lock(ref flag);
				try
				{
					string fullPath = this.GetFullPath(file);
					num = LongPathFile.GetLength(fullPath);
					LongPathFile.Delete(fullPath);
				}
				catch
				{
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DeleteFile"));
				}
				this.Unreserve(IsolatedStorageFile.RoundToBlockSize((ulong)num));
			}
			finally
			{
				if (flag)
				{
					this.Unlock();
				}
			}
			CodeAccessPermission.RevertAll();
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public bool FileExists(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (this.m_bDisposed)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			if (this.m_closed)
			{
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			this.m_fiop.Assert();
			this.m_fiop.PermitOnly();
			string fullPath = this.GetFullPath(path);
			string text = LongPath.NormalizePath(fullPath);
			if (path.EndsWith(Path.DirectorySeparatorChar.ToString() + ".", StringComparison.Ordinal))
			{
				if (text.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
				{
					text += ".";
				}
				else
				{
					text = text + Path.DirectorySeparatorChar.ToString() + ".";
				}
			}
			try
			{
				IsolatedStorageFile.Demand(new FileIOPermission(FileIOPermissionAccess.Read, new string[]
				{
					text
				}, false, false));
			}
			catch
			{
				return false;
			}
			bool result = LongPathFile.Exists(text);
			CodeAccessPermission.RevertAll();
			return result;
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public bool DirectoryExists(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (this.m_bDisposed)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			if (this.m_closed)
			{
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			this.m_fiop.Assert();
			this.m_fiop.PermitOnly();
			string fullPath = this.GetFullPath(path);
			string text = LongPath.NormalizePath(fullPath);
			if (fullPath.EndsWith(Path.DirectorySeparatorChar.ToString() + ".", StringComparison.Ordinal))
			{
				if (text.EndsWith(Path.DirectorySeparatorChar))
				{
					text += ".";
				}
				else
				{
					text = text + Path.DirectorySeparatorChar.ToString() + ".";
				}
			}
			try
			{
				IsolatedStorageFile.Demand(new FileIOPermission(FileIOPermissionAccess.Read, new string[]
				{
					text
				}, false, false));
			}
			catch
			{
				return false;
			}
			bool result = LongPathDirectory.Exists(text);
			CodeAccessPermission.RevertAll();
			return result;
		}

		[SecuritySafeCritical]
		public void CreateDirectory(string dir)
		{
			if (dir == null)
			{
				throw new ArgumentNullException("dir");
			}
			this.m_fiop.Assert();
			this.m_fiop.PermitOnly();
			string fullPath = this.GetFullPath(dir);
			string text = LongPath.NormalizePath(fullPath);
			try
			{
				IsolatedStorageFile.Demand(new FileIOPermission(FileIOPermissionAccess.Read, new string[]
				{
					text
				}, false, false));
			}
			catch
			{
				throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_CreateDirectory"));
			}
			string[] array = this.DirectoriesToCreate(text);
			if (array != null && array.Length != 0)
			{
				this.Reserve((ulong)(1024L * (long)array.Length));
				try
				{
					LongPathDirectory.CreateDirectory(array[array.Length - 1]);
				}
				catch
				{
					this.Unreserve((ulong)(1024L * (long)array.Length));
					try
					{
						LongPathDirectory.Delete(array[0], true);
					}
					catch
					{
					}
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_CreateDirectory"));
				}
				CodeAccessPermission.RevertAll();
				return;
			}
			if (LongPathDirectory.Exists(fullPath))
			{
				return;
			}
			throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_CreateDirectory"));
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public DateTimeOffset GetCreationTime(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"), "path");
			}
			if (this.m_bDisposed)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			if (this.m_closed)
			{
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			this.m_fiop.Assert();
			this.m_fiop.PermitOnly();
			string fullPath = this.GetFullPath(path);
			string text = LongPath.NormalizePath(fullPath);
			try
			{
				IsolatedStorageFile.Demand(new FileIOPermission(FileIOPermissionAccess.Read, new string[]
				{
					text
				}, false, false));
			}
			catch
			{
				return new DateTimeOffset(1601, 1, 1, 0, 0, 0, TimeSpan.Zero).ToLocalTime();
			}
			DateTimeOffset creationTime = LongPathFile.GetCreationTime(text);
			CodeAccessPermission.RevertAll();
			return creationTime;
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public DateTimeOffset GetLastAccessTime(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"), "path");
			}
			if (this.m_bDisposed)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			if (this.m_closed)
			{
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			this.m_fiop.Assert();
			this.m_fiop.PermitOnly();
			string fullPath = this.GetFullPath(path);
			string text = LongPath.NormalizePath(fullPath);
			try
			{
				IsolatedStorageFile.Demand(new FileIOPermission(FileIOPermissionAccess.Read, new string[]
				{
					text
				}, false, false));
			}
			catch
			{
				return new DateTimeOffset(1601, 1, 1, 0, 0, 0, TimeSpan.Zero).ToLocalTime();
			}
			DateTimeOffset lastAccessTime = LongPathFile.GetLastAccessTime(text);
			CodeAccessPermission.RevertAll();
			return lastAccessTime;
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public DateTimeOffset GetLastWriteTime(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"), "path");
			}
			if (this.m_bDisposed)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			if (this.m_closed)
			{
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			this.m_fiop.Assert();
			this.m_fiop.PermitOnly();
			string fullPath = this.GetFullPath(path);
			string text = LongPath.NormalizePath(fullPath);
			try
			{
				IsolatedStorageFile.Demand(new FileIOPermission(FileIOPermissionAccess.Read, new string[]
				{
					text
				}, false, false));
			}
			catch
			{
				return new DateTimeOffset(1601, 1, 1, 0, 0, 0, TimeSpan.Zero).ToLocalTime();
			}
			DateTimeOffset lastWriteTime = LongPathFile.GetLastWriteTime(text);
			CodeAccessPermission.RevertAll();
			return lastWriteTime;
		}

		[ComVisible(false)]
		public void CopyFile(string sourceFileName, string destinationFileName)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName");
			}
			if (destinationFileName == null)
			{
				throw new ArgumentNullException("destinationFileName");
			}
			if (sourceFileName.Trim().Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"), "sourceFileName");
			}
			if (destinationFileName.Trim().Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"), "destinationFileName");
			}
			this.CopyFile(sourceFileName, destinationFileName, false);
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public void CopyFile(string sourceFileName, string destinationFileName, bool overwrite)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName");
			}
			if (destinationFileName == null)
			{
				throw new ArgumentNullException("destinationFileName");
			}
			if (sourceFileName.Trim().Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"), "sourceFileName");
			}
			if (destinationFileName.Trim().Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"), "destinationFileName");
			}
			if (this.m_bDisposed)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			if (this.m_closed)
			{
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			this.m_fiop.Assert();
			this.m_fiop.PermitOnly();
			string text = LongPath.NormalizePath(this.GetFullPath(sourceFileName));
			string text2 = LongPath.NormalizePath(this.GetFullPath(destinationFileName));
			try
			{
				IsolatedStorageFile.Demand(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, new string[]
				{
					text
				}, false, false));
				IsolatedStorageFile.Demand(new FileIOPermission(FileIOPermissionAccess.Write, new string[]
				{
					text2
				}, false, false));
			}
			catch
			{
				throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Operation"));
			}
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this.Lock(ref flag);
				long num = 0L;
				try
				{
					num = LongPathFile.GetLength(text);
				}
				catch (FileNotFoundException)
				{
					throw new FileNotFoundException(Environment.GetResourceString("IO.PathNotFound_Path", new object[]
					{
						sourceFileName
					}));
				}
				catch (DirectoryNotFoundException)
				{
					throw new DirectoryNotFoundException(Environment.GetResourceString("IO.PathNotFound_Path", new object[]
					{
						sourceFileName
					}));
				}
				catch
				{
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Operation"));
				}
				long num2 = 0L;
				if (LongPathFile.Exists(text2))
				{
					try
					{
						num2 = LongPathFile.GetLength(text2);
					}
					catch
					{
						throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Operation"));
					}
				}
				if (num2 < num)
				{
					this.Reserve(IsolatedStorageFile.RoundToBlockSize((ulong)(num - num2)));
				}
				try
				{
					LongPathFile.Copy(text, text2, overwrite);
				}
				catch (FileNotFoundException)
				{
					if (num2 < num)
					{
						this.Unreserve(IsolatedStorageFile.RoundToBlockSize((ulong)(num - num2)));
					}
					throw new FileNotFoundException(Environment.GetResourceString("IO.PathNotFound_Path", new object[]
					{
						sourceFileName
					}));
				}
				catch
				{
					if (num2 < num)
					{
						this.Unreserve(IsolatedStorageFile.RoundToBlockSize((ulong)(num - num2)));
					}
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Operation"));
				}
				if (num2 > num && overwrite)
				{
					this.Unreserve(IsolatedStorageFile.RoundToBlockSizeFloor((ulong)(num2 - num)));
				}
			}
			finally
			{
				if (flag)
				{
					this.Unlock();
				}
			}
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public void MoveFile(string sourceFileName, string destinationFileName)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName");
			}
			if (destinationFileName == null)
			{
				throw new ArgumentNullException("destinationFileName");
			}
			if (sourceFileName.Trim().Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"), "sourceFileName");
			}
			if (destinationFileName.Trim().Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"), "destinationFileName");
			}
			if (this.m_bDisposed)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			if (this.m_closed)
			{
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			this.m_fiop.Assert();
			this.m_fiop.PermitOnly();
			string text = LongPath.NormalizePath(this.GetFullPath(sourceFileName));
			string text2 = LongPath.NormalizePath(this.GetFullPath(destinationFileName));
			try
			{
				IsolatedStorageFile.Demand(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, new string[]
				{
					text
				}, false, false));
				IsolatedStorageFile.Demand(new FileIOPermission(FileIOPermissionAccess.Write, new string[]
				{
					text2
				}, false, false));
			}
			catch
			{
				throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Operation"));
			}
			try
			{
				LongPathFile.Move(text, text2);
			}
			catch (FileNotFoundException)
			{
				throw new FileNotFoundException(Environment.GetResourceString("IO.PathNotFound_Path", new object[]
				{
					sourceFileName
				}));
			}
			catch
			{
				throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Operation"));
			}
			CodeAccessPermission.RevertAll();
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName)
		{
			if (sourceDirectoryName == null)
			{
				throw new ArgumentNullException("sourceDirectoryName");
			}
			if (destinationDirectoryName == null)
			{
				throw new ArgumentNullException("destinationDirectoryName");
			}
			if (sourceDirectoryName.Trim().Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"), "sourceDirectoryName");
			}
			if (destinationDirectoryName.Trim().Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"), "destinationDirectoryName");
			}
			if (this.m_bDisposed)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			if (this.m_closed)
			{
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			this.m_fiop.Assert();
			this.m_fiop.PermitOnly();
			string text = LongPath.NormalizePath(this.GetFullPath(sourceDirectoryName));
			string text2 = LongPath.NormalizePath(this.GetFullPath(destinationDirectoryName));
			try
			{
				IsolatedStorageFile.Demand(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, new string[]
				{
					text
				}, false, false));
				IsolatedStorageFile.Demand(new FileIOPermission(FileIOPermissionAccess.Write, new string[]
				{
					text2
				}, false, false));
			}
			catch
			{
				throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Operation"));
			}
			try
			{
				LongPathDirectory.Move(text, text2);
			}
			catch (DirectoryNotFoundException)
			{
				throw new DirectoryNotFoundException(Environment.GetResourceString("IO.PathNotFound_Path", new object[]
				{
					sourceDirectoryName
				}));
			}
			catch
			{
				throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Operation"));
			}
			CodeAccessPermission.RevertAll();
		}

		[SecurityCritical]
		private string[] DirectoriesToCreate(string fullPath)
		{
			List<string> list = new List<string>();
			int num = fullPath.Length;
			if (num >= 2 && fullPath[num - 1] == this.SeparatorExternal)
			{
				num--;
			}
			int i = LongPath.GetRootLength(fullPath);
			while (i < num)
			{
				i++;
				while (i < num && fullPath[i] != this.SeparatorExternal)
				{
					i++;
				}
				string text = fullPath.Substring(0, i);
				if (!LongPathDirectory.InternalExists(text))
				{
					list.Add(text);
				}
			}
			if (list.Count != 0)
			{
				return list.ToArray();
			}
			return null;
		}

		[SecuritySafeCritical]
		public void DeleteDirectory(string dir)
		{
			if (dir == null)
			{
				throw new ArgumentNullException("dir");
			}
			this.m_fiop.Assert();
			this.m_fiop.PermitOnly();
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this.Lock(ref flag);
				try
				{
					string text = LongPath.NormalizePath(this.GetFullPath(dir));
					if (text.Equals(LongPath.NormalizePath(this.GetFullPath(".")), StringComparison.Ordinal))
					{
						throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DeleteDirectory"));
					}
					LongPathDirectory.Delete(text, false);
				}
				catch
				{
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DeleteDirectory"));
				}
				this.Unreserve(1024UL);
			}
			finally
			{
				if (flag)
				{
					this.Unlock();
				}
			}
			CodeAccessPermission.RevertAll();
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void Demand(CodeAccessPermission permission)
		{
			permission.Demand();
		}

		[ComVisible(false)]
		public string[] GetFileNames()
		{
			return this.GetFileNames("*");
		}

		[SecuritySafeCritical]
		public string[] GetFileNames(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (this.m_bDisposed)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			if (this.m_closed)
			{
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			this.m_fiop.Assert();
			this.m_fiop.PermitOnly();
			string[] fileDirectoryNames = IsolatedStorageFile.GetFileDirectoryNames(this.GetFullPath(searchPattern), searchPattern, true);
			CodeAccessPermission.RevertAll();
			return fileDirectoryNames;
		}

		[ComVisible(false)]
		public string[] GetDirectoryNames()
		{
			return this.GetDirectoryNames("*");
		}

		[SecuritySafeCritical]
		public string[] GetDirectoryNames(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (this.m_bDisposed)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			if (this.m_closed)
			{
				throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
			}
			this.m_fiop.Assert();
			this.m_fiop.PermitOnly();
			string[] fileDirectoryNames = IsolatedStorageFile.GetFileDirectoryNames(this.GetFullPath(searchPattern), searchPattern, false);
			CodeAccessPermission.RevertAll();
			return fileDirectoryNames;
		}

		private static string NormalizeSearchPattern(string searchPattern)
		{
			string text = searchPattern.TrimEnd(Path.TrimEndChars);
			Path.CheckSearchPattern(text);
			return text;
		}

		[ComVisible(false)]
		public IsolatedStorageFileStream OpenFile(string path, FileMode mode)
		{
			return new IsolatedStorageFileStream(path, mode, this);
		}

		[ComVisible(false)]
		public IsolatedStorageFileStream OpenFile(string path, FileMode mode, FileAccess access)
		{
			return new IsolatedStorageFileStream(path, mode, access, this);
		}

		[ComVisible(false)]
		public IsolatedStorageFileStream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
		{
			return new IsolatedStorageFileStream(path, mode, access, share, this);
		}

		[ComVisible(false)]
		public IsolatedStorageFileStream CreateFile(string path)
		{
			return new IsolatedStorageFileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, this);
		}

		[SecuritySafeCritical]
		public override void Remove()
		{
			string text = null;
			this.RemoveLogicalDir();
			this.Close();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(IsolatedStorageFile.GetRootDir(base.Scope));
			if (base.IsApp())
			{
				stringBuilder.Append(base.AppName);
				stringBuilder.Append(this.SeparatorExternal);
			}
			else
			{
				if (base.IsDomain())
				{
					stringBuilder.Append(base.DomainName);
					stringBuilder.Append(this.SeparatorExternal);
					text = stringBuilder.ToString();
				}
				stringBuilder.Append(base.AssemName);
				stringBuilder.Append(this.SeparatorExternal);
			}
			string text2 = stringBuilder.ToString();
			new FileIOPermission(FileIOPermissionAccess.AllAccess, text2).Assert();
			if (this.ContainsUnknownFiles(text2))
			{
				return;
			}
			try
			{
				LongPathDirectory.Delete(text2, true);
			}
			catch
			{
				return;
			}
			if (base.IsDomain())
			{
				CodeAccessPermission.RevertAssert();
				new FileIOPermission(FileIOPermissionAccess.AllAccess, text).Assert();
				if (!this.ContainsUnknownFiles(text))
				{
					try
					{
						LongPathDirectory.Delete(text, true);
					}
					catch
					{
					}
				}
			}
		}

		[SecuritySafeCritical]
		private void RemoveLogicalDir()
		{
			this.m_fiop.Assert();
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this.Lock(ref flag);
				if (Directory.Exists(this.RootDirectory))
				{
					ulong lFree = (ulong)(base.IsRoaming() ? 0L : (this.Quota - this.AvailableFreeSpace));
					ulong quota = (ulong)this.Quota;
					try
					{
						LongPathDirectory.Delete(this.RootDirectory, true);
					}
					catch
					{
						throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DeleteDirectories"));
					}
					this.Unreserve(lFree, quota);
				}
			}
			finally
			{
				if (flag)
				{
					this.Unlock();
				}
			}
		}

		private bool ContainsUnknownFiles(string rootDir)
		{
			string[] fileDirectoryNames;
			string[] fileDirectoryNames2;
			try
			{
				fileDirectoryNames = IsolatedStorageFile.GetFileDirectoryNames(rootDir + "*", "*", true);
				fileDirectoryNames2 = IsolatedStorageFile.GetFileDirectoryNames(rootDir + "*", "*", false);
			}
			catch
			{
				throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DeleteDirectories"));
			}
			if (fileDirectoryNames2 != null && fileDirectoryNames2.Length != 0)
			{
				if (fileDirectoryNames2.Length > 1)
				{
					return true;
				}
				if (base.IsApp())
				{
					if (IsolatedStorageFile.NotAppFilesDir(fileDirectoryNames2[0]))
					{
						return true;
					}
				}
				else if (base.IsDomain())
				{
					if (IsolatedStorageFile.NotFilesDir(fileDirectoryNames2[0]))
					{
						return true;
					}
				}
				else if (IsolatedStorageFile.NotAssemFilesDir(fileDirectoryNames2[0]))
				{
					return true;
				}
			}
			if (fileDirectoryNames == null || fileDirectoryNames.Length == 0)
			{
				return false;
			}
			if (base.IsRoaming())
			{
				return fileDirectoryNames.Length > 1 || IsolatedStorageFile.NotIDFile(fileDirectoryNames[0]);
			}
			return fileDirectoryNames.Length > 2 || (IsolatedStorageFile.NotIDFile(fileDirectoryNames[0]) && IsolatedStorageFile.NotInfoFile(fileDirectoryNames[0])) || (fileDirectoryNames.Length == 2 && IsolatedStorageFile.NotIDFile(fileDirectoryNames[1]) && IsolatedStorageFile.NotInfoFile(fileDirectoryNames[1]));
		}

		[SecuritySafeCritical]
		public void Close()
		{
			if (base.IsRoaming())
			{
				return;
			}
			object internalLock = this.m_internalLock;
			lock (internalLock)
			{
				if (!this.m_closed)
				{
					this.m_closed = true;
					if (this.m_handle != null)
					{
						this.m_handle.Dispose();
					}
					GC.SuppressFinalize(this);
				}
			}
		}

		public void Dispose()
		{
			this.Close();
			this.m_bDisposed = true;
		}

		~IsolatedStorageFile()
		{
			this.Dispose();
		}

		private static bool NotIDFile(string file)
		{
			return string.Compare(file, "identity.dat", StringComparison.Ordinal) != 0;
		}

		private static bool NotInfoFile(string file)
		{
			return string.Compare(file, "info.dat", StringComparison.Ordinal) != 0 && string.Compare(file, "appinfo.dat", StringComparison.Ordinal) != 0;
		}

		private static bool NotFilesDir(string dir)
		{
			return string.Compare(dir, "Files", StringComparison.Ordinal) != 0;
		}

		internal static bool NotAssemFilesDir(string dir)
		{
			return string.Compare(dir, "AssemFiles", StringComparison.Ordinal) != 0;
		}

		internal static bool NotAppFilesDir(string dir)
		{
			return string.Compare(dir, "AppFiles", StringComparison.Ordinal) != 0;
		}

		[SecuritySafeCritical]
		public static void Remove(IsolatedStorageScope scope)
		{
			IsolatedStorageFile.VerifyGlobalScope(scope);
			IsolatedStorageFile.DemandAdminPermission();
			string rootDir = IsolatedStorageFile.GetRootDir(scope);
			new FileIOPermission(FileIOPermissionAccess.Write, rootDir).Assert();
			try
			{
				LongPathDirectory.Delete(rootDir, true);
				LongPathDirectory.CreateDirectory(rootDir);
			}
			catch
			{
				throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DeleteDirectories"));
			}
		}

		[SecuritySafeCritical]
		public static IEnumerator GetEnumerator(IsolatedStorageScope scope)
		{
			IsolatedStorageFile.VerifyGlobalScope(scope);
			IsolatedStorageFile.DemandAdminPermission();
			return new IsolatedStorageFileEnumerator(scope);
		}

		internal string RootDirectory
		{
			get
			{
				return this.m_RootDir;
			}
		}

		internal string GetFullPath(string path)
		{
			if (path == string.Empty)
			{
				return this.RootDirectory;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.RootDirectory);
			if (path[0] == this.SeparatorExternal)
			{
				stringBuilder.Append(path.Substring(1));
			}
			else
			{
				stringBuilder.Append(path);
			}
			return stringBuilder.ToString();
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		private static string GetDataDirectoryFromActivationContext()
		{
			if (IsolatedStorageFile.s_appDataDir == null)
			{
				ActivationContext activationContext = AppDomain.CurrentDomain.ActivationContext;
				if (activationContext == null)
				{
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_ApplicationMissingIdentity"));
				}
				string text = activationContext.DataDirectory;
				if (text != null && text[text.Length - 1] != '\\')
				{
					text += "\\";
				}
				IsolatedStorageFile.s_appDataDir = text;
			}
			return IsolatedStorageFile.s_appDataDir;
		}

		[SecuritySafeCritical]
		internal void Init(IsolatedStorageScope scope)
		{
			IsolatedStorageFile.GetGlobalFileIOPerm(scope).Assert();
			this.m_StoreScope = scope;
			StringBuilder stringBuilder = new StringBuilder();
			if (IsolatedStorage.IsApp(scope))
			{
				stringBuilder.Append(IsolatedStorageFile.GetRootDir(scope));
				if (IsolatedStorageFile.s_appDataDir == null)
				{
					stringBuilder.Append(base.AppName);
					stringBuilder.Append(this.SeparatorExternal);
				}
				try
				{
					LongPathDirectory.CreateDirectory(stringBuilder.ToString());
				}
				catch
				{
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Init"));
				}
				this.CreateIDFile(stringBuilder.ToString(), IsolatedStorageScope.Application);
				this.m_InfoFile = stringBuilder.ToString() + "appinfo.dat";
				stringBuilder.Append("AppFiles");
			}
			else
			{
				stringBuilder.Append(IsolatedStorageFile.GetRootDir(scope));
				if (IsolatedStorage.IsDomain(scope))
				{
					stringBuilder.Append(base.DomainName);
					stringBuilder.Append(this.SeparatorExternal);
					try
					{
						LongPathDirectory.CreateDirectory(stringBuilder.ToString());
						this.CreateIDFile(stringBuilder.ToString(), IsolatedStorageScope.Domain);
					}
					catch
					{
						throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Init"));
					}
					this.m_InfoFile = stringBuilder.ToString() + "info.dat";
				}
				stringBuilder.Append(base.AssemName);
				stringBuilder.Append(this.SeparatorExternal);
				try
				{
					LongPathDirectory.CreateDirectory(stringBuilder.ToString());
					this.CreateIDFile(stringBuilder.ToString(), IsolatedStorageScope.Assembly);
				}
				catch
				{
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Init"));
				}
				if (IsolatedStorage.IsDomain(scope))
				{
					stringBuilder.Append("Files");
				}
				else
				{
					this.m_InfoFile = stringBuilder.ToString() + "info.dat";
					stringBuilder.Append("AssemFiles");
				}
			}
			stringBuilder.Append(this.SeparatorExternal);
			string text = stringBuilder.ToString();
			try
			{
				LongPathDirectory.CreateDirectory(text);
			}
			catch
			{
				throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Init"));
			}
			this.m_RootDir = text;
			this.m_fiop = new FileIOPermission(FileIOPermissionAccess.AllAccess, text);
			if (scope == (IsolatedStorageScope.User | IsolatedStorageScope.Application))
			{
				this.UpdateQuotaFromInfoFile();
			}
		}

		[SecurityCritical]
		private void UpdateQuotaFromInfoFile()
		{
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this.Lock(ref flag);
				object internalLock = this.m_internalLock;
				lock (internalLock)
				{
					if (this.InvalidFileHandle)
					{
						this.m_handle = IsolatedStorageFile.Open(this.m_InfoFile, this.GetSyncObjectName());
					}
					long quota = 0L;
					if (IsolatedStorageFile.GetQuota(this.m_handle, out quota))
					{
						base.Quota = quota;
					}
				}
			}
			finally
			{
				if (flag)
				{
					this.Unlock();
				}
			}
		}

		[SecuritySafeCritical]
		internal bool InitExistingStore(IsolatedStorageScope scope)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.m_StoreScope = scope;
			stringBuilder.Append(IsolatedStorageFile.GetRootDir(scope));
			if (IsolatedStorage.IsApp(scope))
			{
				stringBuilder.Append(base.AppName);
				stringBuilder.Append(this.SeparatorExternal);
				this.m_InfoFile = stringBuilder.ToString() + "appinfo.dat";
				stringBuilder.Append("AppFiles");
			}
			else
			{
				if (IsolatedStorage.IsDomain(scope))
				{
					stringBuilder.Append(base.DomainName);
					stringBuilder.Append(this.SeparatorExternal);
					this.m_InfoFile = stringBuilder.ToString() + "info.dat";
				}
				stringBuilder.Append(base.AssemName);
				stringBuilder.Append(this.SeparatorExternal);
				if (IsolatedStorage.IsDomain(scope))
				{
					stringBuilder.Append("Files");
				}
				else
				{
					this.m_InfoFile = stringBuilder.ToString() + "info.dat";
					stringBuilder.Append("AssemFiles");
				}
			}
			stringBuilder.Append(this.SeparatorExternal);
			FileIOPermission fileIOPermission = new FileIOPermission(FileIOPermissionAccess.AllAccess, stringBuilder.ToString());
			fileIOPermission.Assert();
			if (!LongPathDirectory.Exists(stringBuilder.ToString()))
			{
				return false;
			}
			this.m_RootDir = stringBuilder.ToString();
			this.m_fiop = fileIOPermission;
			if (scope == (IsolatedStorageScope.User | IsolatedStorageScope.Application))
			{
				this.UpdateQuotaFromInfoFile();
			}
			return true;
		}

		protected override IsolatedStoragePermission GetPermission(PermissionSet ps)
		{
			if (ps == null)
			{
				return null;
			}
			if (ps.IsUnrestricted())
			{
				return new IsolatedStorageFilePermission(PermissionState.Unrestricted);
			}
			return (IsolatedStoragePermission)ps.GetPermission(typeof(IsolatedStorageFilePermission));
		}

		internal void UndoReserveOperation(ulong oldLen, ulong newLen)
		{
			oldLen = IsolatedStorageFile.RoundToBlockSize(oldLen);
			if (newLen > oldLen)
			{
				this.Unreserve(IsolatedStorageFile.RoundToBlockSize(newLen - oldLen));
			}
		}

		internal void Reserve(ulong oldLen, ulong newLen)
		{
			oldLen = IsolatedStorageFile.RoundToBlockSize(oldLen);
			if (newLen > oldLen)
			{
				this.Reserve(IsolatedStorageFile.RoundToBlockSize(newLen - oldLen));
			}
		}

		internal void ReserveOneBlock()
		{
			this.Reserve(1024UL);
		}

		internal void UnreserveOneBlock()
		{
			this.Unreserve(1024UL);
		}

		internal static ulong RoundToBlockSize(ulong num)
		{
			if (num < 1024UL)
			{
				return 1024UL;
			}
			ulong num2 = num % 1024UL;
			if (num2 != 0UL)
			{
				num += 1024UL - num2;
			}
			return num;
		}

		internal static ulong RoundToBlockSizeFloor(ulong num)
		{
			if (num < 1024UL)
			{
				return 0UL;
			}
			ulong num2 = num % 1024UL;
			num -= num2;
			return num;
		}

		[SecurityCritical]
		internal static string GetRootDir(IsolatedStorageScope scope)
		{
			if (IsolatedStorage.IsRoaming(scope))
			{
				if (IsolatedStorageFile.s_RootDirRoaming == null)
				{
					string text = null;
					IsolatedStorageFile.GetRootDir(scope, JitHelpers.GetStringHandleOnStack(ref text));
					IsolatedStorageFile.s_RootDirRoaming = text;
				}
				return IsolatedStorageFile.s_RootDirRoaming;
			}
			if (IsolatedStorage.IsMachine(scope))
			{
				if (IsolatedStorageFile.s_RootDirMachine == null)
				{
					IsolatedStorageFile.InitGlobalsMachine(scope);
				}
				return IsolatedStorageFile.s_RootDirMachine;
			}
			if (IsolatedStorageFile.s_RootDirUser == null)
			{
				IsolatedStorageFile.InitGlobalsNonRoamingUser(scope);
			}
			return IsolatedStorageFile.s_RootDirUser;
		}

		[SecuritySafeCritical]
		[HandleProcessCorruptedStateExceptions]
		private static void InitGlobalsMachine(IsolatedStorageScope scope)
		{
			string text = null;
			IsolatedStorageFile.GetRootDir(scope, JitHelpers.GetStringHandleOnStack(ref text));
			new FileIOPermission(FileIOPermissionAccess.AllAccess, text).Assert();
			string text2 = IsolatedStorageFile.GetMachineRandomDirectory(text);
			if (text2 == null)
			{
				Mutex mutex = IsolatedStorageFile.CreateMutexNotOwned(text);
				if (!mutex.WaitOne())
				{
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Init"));
				}
				try
				{
					text2 = IsolatedStorageFile.GetMachineRandomDirectory(text);
					if (text2 == null)
					{
						string randomFileName = Path.GetRandomFileName();
						string randomFileName2 = Path.GetRandomFileName();
						try
						{
							IsolatedStorageFile.CreateDirectoryWithDacl(text + randomFileName);
							IsolatedStorageFile.CreateDirectoryWithDacl(text + randomFileName + "\\" + randomFileName2);
						}
						catch
						{
							throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Init"));
						}
						text2 = randomFileName + "\\" + randomFileName2;
					}
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
			IsolatedStorageFile.s_RootDirMachine = text + text2 + "\\";
		}

		[SecuritySafeCritical]
		private static void InitGlobalsNonRoamingUser(IsolatedStorageScope scope)
		{
			string text = null;
			if (scope == (IsolatedStorageScope.User | IsolatedStorageScope.Application))
			{
				text = IsolatedStorageFile.GetDataDirectoryFromActivationContext();
				if (text != null)
				{
					IsolatedStorageFile.s_RootDirUser = text;
					return;
				}
			}
			IsolatedStorageFile.GetRootDir(scope, JitHelpers.GetStringHandleOnStack(ref text));
			new FileIOPermission(FileIOPermissionAccess.AllAccess, text).Assert();
			bool flag = false;
			string oldRandomDirectory = null;
			string text2 = IsolatedStorageFile.GetRandomDirectory(text, out flag, out oldRandomDirectory);
			if (text2 == null)
			{
				Mutex mutex = IsolatedStorageFile.CreateMutexNotOwned(text);
				if (!mutex.WaitOne())
				{
					throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Init"));
				}
				try
				{
					text2 = IsolatedStorageFile.GetRandomDirectory(text, out flag, out oldRandomDirectory);
					if (text2 == null)
					{
						if (flag)
						{
							text2 = IsolatedStorageFile.MigrateOldIsoStoreDirectory(text, oldRandomDirectory);
						}
						else
						{
							text2 = IsolatedStorageFile.CreateRandomDirectory(text);
						}
					}
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
			IsolatedStorageFile.s_RootDirUser = text + text2 + "\\";
		}

		internal bool Disposed
		{
			get
			{
				return this.m_bDisposed;
			}
		}

		private bool InvalidFileHandle
		{
			[SecuritySafeCritical]
			get
			{
				return this.m_handle == null || this.m_handle.IsClosed || this.m_handle.IsInvalid;
			}
		}

		[SecuritySafeCritical]
		[HandleProcessCorruptedStateExceptions]
		internal static string MigrateOldIsoStoreDirectory(string rootDir, string oldRandomDirectory)
		{
			string randomFileName = Path.GetRandomFileName();
			string randomFileName2 = Path.GetRandomFileName();
			string text = rootDir + randomFileName;
			string destDirName = text + "\\" + randomFileName2;
			try
			{
				LongPathDirectory.CreateDirectory(text);
				LongPathDirectory.Move(rootDir + oldRandomDirectory, destDirName);
			}
			catch
			{
				throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Init"));
			}
			return randomFileName + "\\" + randomFileName2;
		}

		[SecuritySafeCritical]
		[HandleProcessCorruptedStateExceptions]
		internal static string CreateRandomDirectory(string rootDir)
		{
			string text;
			string path;
			do
			{
				text = Path.GetRandomFileName() + "\\" + Path.GetRandomFileName();
				path = rootDir + text;
			}
			while (LongPathDirectory.Exists(path));
			try
			{
				LongPathDirectory.CreateDirectory(path);
			}
			catch
			{
				throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Init"));
			}
			return text;
		}

		internal static string GetRandomDirectory(string rootDir, out bool bMigrateNeeded, out string sOldStoreLocation)
		{
			bMigrateNeeded = false;
			sOldStoreLocation = null;
			string[] fileDirectoryNames = IsolatedStorageFile.GetFileDirectoryNames(rootDir + "*", "*", false);
			for (int i = 0; i < fileDirectoryNames.Length; i++)
			{
				if (fileDirectoryNames[i].Length == 12)
				{
					string[] fileDirectoryNames2 = IsolatedStorageFile.GetFileDirectoryNames(rootDir + fileDirectoryNames[i] + "\\*", "*", false);
					for (int j = 0; j < fileDirectoryNames2.Length; j++)
					{
						if (fileDirectoryNames2[j].Length == 12)
						{
							return fileDirectoryNames[i] + "\\" + fileDirectoryNames2[j];
						}
					}
				}
			}
			for (int k = 0; k < fileDirectoryNames.Length; k++)
			{
				if (fileDirectoryNames[k].Length == 24)
				{
					bMigrateNeeded = true;
					sOldStoreLocation = fileDirectoryNames[k];
					return null;
				}
			}
			return null;
		}

		internal static string GetMachineRandomDirectory(string rootDir)
		{
			string[] fileDirectoryNames = IsolatedStorageFile.GetFileDirectoryNames(rootDir + "*", "*", false);
			for (int i = 0; i < fileDirectoryNames.Length; i++)
			{
				if (fileDirectoryNames[i].Length == 12)
				{
					string[] fileDirectoryNames2 = IsolatedStorageFile.GetFileDirectoryNames(rootDir + fileDirectoryNames[i] + "\\*", "*", false);
					for (int j = 0; j < fileDirectoryNames2.Length; j++)
					{
						if (fileDirectoryNames2[j].Length == 12)
						{
							return fileDirectoryNames[i] + "\\" + fileDirectoryNames2[j];
						}
					}
				}
			}
			return null;
		}

		[SecurityCritical]
		internal static Mutex CreateMutexNotOwned(string pathName)
		{
			return new Mutex(false, "Global\\" + IsolatedStorageFile.GetStrongHashSuitableForObjectName(pathName));
		}

		internal static string GetStrongHashSuitableForObjectName(string name)
		{
			MemoryStream memoryStream = new MemoryStream();
			new BinaryWriter(memoryStream).Write(name.ToUpper(CultureInfo.InvariantCulture));
			memoryStream.Position = 0L;
			return Path.ToBase32StringSuitableForDirName(new SHA1CryptoServiceProvider().ComputeHash(memoryStream));
		}

		private string GetSyncObjectName()
		{
			if (this.m_SyncObjectName == null)
			{
				this.m_SyncObjectName = IsolatedStorageFile.GetStrongHashSuitableForObjectName(this.m_InfoFile);
			}
			return this.m_SyncObjectName;
		}

		[SecuritySafeCritical]
		internal void Lock(ref bool locked)
		{
			locked = false;
			if (base.IsRoaming())
			{
				return;
			}
			object internalLock = this.m_internalLock;
			lock (internalLock)
			{
				if (this.m_bDisposed)
				{
					throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
				}
				if (this.m_closed)
				{
					throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
				}
				if (this.InvalidFileHandle)
				{
					this.m_handle = IsolatedStorageFile.Open(this.m_InfoFile, this.GetSyncObjectName());
				}
				locked = IsolatedStorageFile.Lock(this.m_handle, true);
			}
		}

		[SecuritySafeCritical]
		internal void Unlock()
		{
			if (base.IsRoaming())
			{
				return;
			}
			object internalLock = this.m_internalLock;
			lock (internalLock)
			{
				if (this.m_bDisposed)
				{
					throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
				}
				if (this.m_closed)
				{
					throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
				}
				if (this.InvalidFileHandle)
				{
					this.m_handle = IsolatedStorageFile.Open(this.m_InfoFile, this.GetSyncObjectName());
				}
				IsolatedStorageFile.Lock(this.m_handle, false);
			}
		}

		[SecurityCritical]
		internal static FileIOPermission GetGlobalFileIOPerm(IsolatedStorageScope scope)
		{
			if (IsolatedStorage.IsRoaming(scope))
			{
				if (IsolatedStorageFile.s_PermRoaming == null)
				{
					IsolatedStorageFile.s_PermRoaming = new FileIOPermission(FileIOPermissionAccess.AllAccess, IsolatedStorageFile.GetRootDir(scope));
				}
				return IsolatedStorageFile.s_PermRoaming;
			}
			if (IsolatedStorage.IsMachine(scope))
			{
				if (IsolatedStorageFile.s_PermMachine == null)
				{
					IsolatedStorageFile.s_PermMachine = new FileIOPermission(FileIOPermissionAccess.AllAccess, IsolatedStorageFile.GetRootDir(scope));
				}
				return IsolatedStorageFile.s_PermMachine;
			}
			if (IsolatedStorageFile.s_PermUser == null)
			{
				IsolatedStorageFile.s_PermUser = new FileIOPermission(FileIOPermissionAccess.AllAccess, IsolatedStorageFile.GetRootDir(scope));
			}
			return IsolatedStorageFile.s_PermUser;
		}

		[SecurityCritical]
		private static void DemandAdminPermission()
		{
			if (IsolatedStorageFile.s_PermAdminUser == null)
			{
				IsolatedStorageFile.s_PermAdminUser = new IsolatedStorageFilePermission(IsolatedStorageContainment.AdministerIsolatedStorageByUser, 0L, false);
			}
			IsolatedStorageFile.s_PermAdminUser.Demand();
		}

		internal static void VerifyGlobalScope(IsolatedStorageScope scope)
		{
			if (scope != IsolatedStorageScope.User && scope != (IsolatedStorageScope.User | IsolatedStorageScope.Roaming) && scope != IsolatedStorageScope.Machine)
			{
				throw new ArgumentException(Environment.GetResourceString("IsolatedStorage_Scope_U_R_M"));
			}
		}

		[SecuritySafeCritical]
		internal void CreateIDFile(string path, IsolatedStorageScope scope)
		{
			try
			{
				using (FileStream fileStream = new FileStream(path + "identity.dat", FileMode.OpenOrCreate))
				{
					MemoryStream identityStream = base.GetIdentityStream(scope);
					byte[] buffer = identityStream.GetBuffer();
					fileStream.Write(buffer, 0, (int)identityStream.Length);
					identityStream.Close();
				}
			}
			catch
			{
			}
		}

		[SecuritySafeCritical]
		internal static string[] GetFileDirectoryNames(string path, string userSearchPattern, bool file)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path", Environment.GetResourceString("ArgumentNull_Path"));
			}
			userSearchPattern = IsolatedStorageFile.NormalizeSearchPattern(userSearchPattern);
			if (userSearchPattern.Length == 0)
			{
				return new string[0];
			}
			bool flag = false;
			char c = path[path.Length - 1];
			if (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar || c == '.')
			{
				flag = true;
			}
			string text = LongPath.NormalizePath(path);
			if (flag && text[text.Length - 1] != c)
			{
				text += "\\*";
			}
			string text2 = LongPath.GetDirectoryName(text);
			if (text2 != null)
			{
				text2 += "\\";
			}
			try
			{
				new FileIOPermission(FileIOPermissionAccess.Read, new string[]
				{
					(text2 == null) ? text : text2
				}, false, false).Demand();
			}
			catch
			{
				throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Operation"));
			}
			string[] array = new string[10];
			int num = 0;
			Win32Native.WIN32_FIND_DATA win32_FIND_DATA = default(Win32Native.WIN32_FIND_DATA);
			SafeFindHandle safeFindHandle = Win32Native.FindFirstFile(Path.AddLongPathPrefix(text), ref win32_FIND_DATA);
			int lastWin32Error;
			if (safeFindHandle.IsInvalid)
			{
				lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == 2)
				{
					return new string[0];
				}
				__Error.WinIOError(lastWin32Error, userSearchPattern);
			}
			int num2 = 0;
			do
			{
				bool flag2;
				if (file)
				{
					flag2 = win32_FIND_DATA.IsFile;
				}
				else
				{
					flag2 = win32_FIND_DATA.IsNormalDirectory;
				}
				if (flag2)
				{
					num2++;
					if (num == array.Length)
					{
						Array.Resize<string>(ref array, 2 * array.Length);
					}
					array[num++] = win32_FIND_DATA.cFileName;
				}
			}
			while (Win32Native.FindNextFile(safeFindHandle, ref win32_FIND_DATA));
			lastWin32Error = Marshal.GetLastWin32Error();
			safeFindHandle.Close();
			if (lastWin32Error != 0 && lastWin32Error != 18)
			{
				__Error.WinIOError(lastWin32Error, userSearchPattern);
			}
			if (!file && num2 == 1 && (win32_FIND_DATA.dwFileAttributes & 16) != 0)
			{
				return new string[]
				{
					win32_FIND_DATA.cFileName
				};
			}
			if (num == array.Length)
			{
				return array;
			}
			Array.Resize<string>(ref array, num);
			return array;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern ulong GetUsage(SafeIsolatedStorageFileHandle handle);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern SafeIsolatedStorageFileHandle Open(string infoFile, string syncName);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern void Reserve(SafeIsolatedStorageFileHandle handle, ulong plQuota, ulong plReserve, [MarshalAs(UnmanagedType.Bool)] bool fFree);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern void GetRootDir(IsolatedStorageScope scope, StringHandleOnStack retRootDir);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Lock(SafeIsolatedStorageFileHandle handle, [MarshalAs(UnmanagedType.Bool)] bool fLock);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern void CreateDirectoryWithDacl(string path);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetQuota(SafeIsolatedStorageFileHandle scope, out long quota);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern void SetQuota(SafeIsolatedStorageFileHandle scope, long quota);

		private const int s_BlockSize = 1024;

		private const int s_DirSize = 1024;

		private const string s_name = "file.store";

		internal const string s_Files = "Files";

		internal const string s_AssemFiles = "AssemFiles";

		internal const string s_AppFiles = "AppFiles";

		internal const string s_IDFile = "identity.dat";

		internal const string s_InfoFile = "info.dat";

		internal const string s_AppInfoFile = "appinfo.dat";

		private static volatile string s_RootDirUser;

		private static volatile string s_RootDirMachine;

		private static volatile string s_RootDirRoaming;

		private static volatile string s_appDataDir;

		private static volatile FileIOPermission s_PermUser;

		private static volatile FileIOPermission s_PermMachine;

		private static volatile FileIOPermission s_PermRoaming;

		private static volatile IsolatedStorageFilePermission s_PermAdminUser;

		private FileIOPermission m_fiop;

		private string m_RootDir;

		private string m_InfoFile;

		private string m_SyncObjectName;

		[SecurityCritical]
		private SafeIsolatedStorageFileHandle m_handle;

		private bool m_closed;

		private bool m_bDisposed;

		private object m_internalLock = new object();

		private IsolatedStorageScope m_StoreScope;
	}
}
