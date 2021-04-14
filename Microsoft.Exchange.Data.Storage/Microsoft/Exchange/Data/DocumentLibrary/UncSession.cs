using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UncSession
	{
		private UncSession(UncObjectId objectId, IPrincipal authenticatedUser)
		{
			WindowsPrincipal windowsPrincipal = authenticatedUser as WindowsPrincipal;
			if (windowsPrincipal == null)
			{
				throw new ArgumentException("authenticatedUser");
			}
			this.identity = (WindowsIdentity)windowsPrincipal.Identity;
			this.id = new UncObjectId(new Uri("\\\\" + objectId.Path.Host), UriFlags.Unc);
		}

		public static UncSession Open(ObjectId objectId, IPrincipal authenticatedUser)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			if (authenticatedUser == null)
			{
				throw new ArgumentNullException("authenticatedUser");
			}
			UncObjectId uncObjectId = objectId as UncObjectId;
			if (uncObjectId == null)
			{
				throw new ArgumentException("objectId");
			}
			return new UncSession(uncObjectId, authenticatedUser);
		}

		public ObjectId Id
		{
			get
			{
				return this.id;
			}
		}

		public string Title
		{
			get
			{
				return this.id.Path.Host;
			}
		}

		public Uri Uri
		{
			get
			{
				return this.id.Path;
			}
		}

		public ITableView GetView(QueryFilter filter, SortBy[] sortBy, params PropertyDefinition[] propsToReturn)
		{
			WindowsImpersonationContext windowsImpersonationContext = Utils.ImpersonateUser(this.identity);
			ITableView result;
			try
			{
				IntPtr zero = IntPtr.Zero;
				try
				{
					int num = 0;
					int num3;
					int num4;
					int num2 = UncSession.NetShareEnum(this.id.Path.Host, 1, out zero, -1, out num3, out num4, ref num);
					if (num2 == 5)
					{
						throw new AccessDeniedException(this.Id, Strings.ExAccessDenied(this.id.Path.LocalPath));
					}
					if (num2 == 2250 || num2 == 53)
					{
						throw new ObjectMovedOrDeletedException(this.Id, Strings.ExObjectMovedOrDeleted(this.id.Path.LocalPath));
					}
					List<object[]> list = new List<object[]>();
					if (num2 == 0 && num3 > 0)
					{
						int num5 = Marshal.SizeOf(typeof(UncSession.SHARE_INFO_1));
						IntPtr ptr = zero;
						int num6 = 0;
						int num7 = Utils.GetViewMaxRows;
						while (num6 < num3 && num7 > 0)
						{
							UncSession.SHARE_INFO_1 share_INFO_ = (UncSession.SHARE_INFO_1)Marshal.PtrToStructure(ptr, typeof(UncSession.SHARE_INFO_1));
							if (share_INFO_.ShareType == UncSession.ShareType.Disk)
							{
								UncObjectId uncObjectId = new UncObjectId(new Uri(Path.Combine("\\\\" + this.id.Path.Host, share_INFO_.NetName)), UriFlags.UncDocumentLibrary);
								object[] array = new object[propsToReturn.Length];
								DirectoryInfo directoryInfo = null;
								bool flag = true;
								int i = 0;
								while (i < propsToReturn.Length)
								{
									DocumentLibraryPropertyId documentLibraryPropertyId = DocumentLibraryPropertyId.None;
									DocumentLibraryPropertyDefinition documentLibraryPropertyDefinition = propsToReturn[i] as DocumentLibraryPropertyDefinition;
									if (documentLibraryPropertyDefinition != null)
									{
										documentLibraryPropertyId = documentLibraryPropertyDefinition.PropertyId;
									}
									DocumentLibraryPropertyId documentLibraryPropertyId2 = documentLibraryPropertyId;
									switch (documentLibraryPropertyId2)
									{
									case DocumentLibraryPropertyId.None:
										array[i] = new PropertyError(propsToReturn[i], PropertyErrorCode.NotFound);
										break;
									case DocumentLibraryPropertyId.Uri:
										array[i] = uncObjectId.Path;
										break;
									case DocumentLibraryPropertyId.ContentLength:
									case DocumentLibraryPropertyId.CreationTime:
									case DocumentLibraryPropertyId.LastModifiedTime:
									case DocumentLibraryPropertyId.IsFolder:
										goto IL_1F2;
									case DocumentLibraryPropertyId.IsHidden:
										array[i] = share_INFO_.NetName.EndsWith("$");
										break;
									case DocumentLibraryPropertyId.Id:
										array[i] = uncObjectId;
										break;
									case DocumentLibraryPropertyId.Title:
										array[i] = share_INFO_.NetName;
										break;
									default:
										if (documentLibraryPropertyId2 != DocumentLibraryPropertyId.Description)
										{
											goto IL_1F2;
										}
										array[i] = share_INFO_.Remark;
										break;
									}
									IL_248:
									i++;
									continue;
									IL_1F2:
									if (flag)
									{
										try
										{
											if (directoryInfo == null && flag)
											{
												directoryInfo = new DirectoryInfo(uncObjectId.Path.LocalPath);
												FileAttributes attributes = directoryInfo.Attributes;
											}
										}
										catch (UnauthorizedAccessException)
										{
											flag = false;
										}
										catch (IOException)
										{
											flag = false;
										}
									}
									if (flag)
									{
										array[i] = UncDocumentLibraryItem.GetValueFromFileSystemInfo(documentLibraryPropertyDefinition, directoryInfo);
										goto IL_248;
									}
									array[i] = new PropertyError(documentLibraryPropertyDefinition, PropertyErrorCode.NotFound);
									goto IL_248;
								}
								list.Add(array);
								num7--;
							}
							num6++;
							ptr = (IntPtr)(ptr.ToInt64() + (long)num5);
						}
					}
					result = new ArrayTableView(filter, sortBy, propsToReturn, list);
				}
				finally
				{
					if (IntPtr.Zero != zero)
					{
						UncSession.NetApiBufferFree(zero);
					}
				}
			}
			catch
			{
				Utils.UndoContext(ref windowsImpersonationContext);
				throw;
			}
			finally
			{
				Utils.UndoContext(ref windowsImpersonationContext);
			}
			return result;
		}

		internal WindowsIdentity Identity
		{
			get
			{
				return this.identity;
			}
		}

		[DllImport("netapi32", CharSet = CharSet.Unicode)]
		internal static extern int NetShareEnum(string lpServerName, int dwLevel, out IntPtr lpBuffer, int dwPrefMaxLen, out int entriesRead, out int totalEntries, ref int hResume);

		[DllImport("netapi32")]
		internal static extern int NetApiBufferFree(IntPtr lpBuffer);

		[DllImport("netapi32", CharSet = CharSet.Unicode)]
		internal static extern int NetShareGetInfo(string serverName, string netName, int level, out IntPtr lpBuffer);

		internal const int NO_ERROR = 0;

		internal const int ERROR_ACCESS_DENIED = 5;

		internal const int ERROR_NETWORK_PATH_NOT_FOUND = 53;

		internal const int ERROR_NOT_CONNECTED = 2250;

		internal const int NERR_NetNameNotFound = 2310;

		internal const int NERR_DeviceNotShared = 2311;

		internal const int NERR_ClientNameNotFound = 2312;

		private readonly WindowsIdentity identity;

		private readonly UncObjectId id;

		[Flags]
		internal enum ShareType : uint
		{
			Disk = 0U,
			Printer = 1U,
			Device = 2U,
			IPC = 3U,
			Special = 2147483648U
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct SHARE_INFO_1
		{
			[MarshalAs(UnmanagedType.LPWStr)]
			public string NetName;

			public UncSession.ShareType ShareType;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string Remark;
		}
	}
}
