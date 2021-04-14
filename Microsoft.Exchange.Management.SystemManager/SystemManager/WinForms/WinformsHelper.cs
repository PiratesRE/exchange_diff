using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.MapiTasks;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public static class WinformsHelper
	{
		public static void SetDataObjectToClipboard(object data, bool copy)
		{
			try
			{
				Clipboard.SetDataObject(data, copy);
			}
			catch (ExternalException)
			{
			}
		}

		public static object SetNetName(object dagNetworkId, object netName)
		{
			return new DagNetworkObjectId((string)dagNetworkId)
			{
				NetName = (string)netName
			}.ToString();
		}

		public static string GetTextFromClipboard()
		{
			string result = string.Empty;
			try
			{
				result = Clipboard.GetText();
			}
			catch (ExternalException)
			{
			}
			return result;
		}

		public static string GetExecutingAssemblyDirectory()
		{
			string location = Assembly.GetExecutingAssembly().Location;
			return Path.GetDirectoryName(location);
		}

		public static string GetAbsolutePath(string fileName)
		{
			return WinformsHelper.GetExecutingAssemblyDirectory() + "\\" + fileName;
		}

		public static bool IsEmptyValue(object propertyValue)
		{
			bool result = false;
			if (propertyValue == null)
			{
				result = true;
			}
			else if (DBNull.Value.Equals(propertyValue))
			{
				result = true;
			}
			else if (propertyValue is IEnumerable && WinformsHelper.IsEmptyCollection(propertyValue as IEnumerable))
			{
				result = true;
			}
			else if (propertyValue is Guid && Guid.Empty.Equals(propertyValue))
			{
				result = true;
			}
			else if (string.IsNullOrEmpty(propertyValue.ToString()))
			{
				result = true;
			}
			return result;
		}

		private static bool IsEmptyCollection(IEnumerable enumerable)
		{
			bool result = true;
			if (enumerable != null)
			{
				using (IEnumerator enumerator = enumerable.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						result = false;
					}
				}
			}
			return result;
		}

		public static Type GetNullableTypeArgument(Type type)
		{
			Type result = type;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && type.GetGenericArguments().Length == 1)
			{
				result = type.GetGenericArguments()[0];
			}
			return result;
		}

		public static Stream GetManifestResource(DataDrivenCategory dataDrivenCategory)
		{
			string assemblyString;
			string name;
			ManagementGUICommon.GetRegisterAssembly(dataDrivenCategory, ref assemblyString, ref name);
			return Assembly.Load(assemblyString).GetManifestResourceStream(name);
		}

		internal static void OpenUrl(Uri url)
		{
			try
			{
				Process.Start(url.OriginalString);
			}
			catch (Win32Exception innerException)
			{
				throw new UrlHandlerNotFoundException(url.OriginalString, innerException);
			}
		}

		internal static void ShowNonFileUrl(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string uriString = e.Link.LinkData as string;
			try
			{
				Uri uri = new Uri(uriString);
				if (uri.IsFile)
				{
					throw new InvalidOperationException(Strings.NonFileUrlError);
				}
				WinformsHelper.OpenUrl(uri);
			}
			catch (UriFormatException)
			{
			}
		}

		internal static Point GetCentralLocation(Size formSize)
		{
			int width = Screen.PrimaryScreen.WorkingArea.Width;
			int height = Screen.PrimaryScreen.WorkingArea.Height;
			return new Point((width - formSize.Width) / 2, (height - formSize.Height) / 2);
		}

		internal static void ShowExportDialog(IWin32Window owner, DataListView listControl, IUIService uiService)
		{
			using (System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog())
			{
				saveFileDialog.OverwritePrompt = true;
				saveFileDialog.Title = Strings.ExportListFileDialogTitle;
				saveFileDialog.Filter = string.Format("{0}|*.txt|{1}|*.csv|{2}|*.txt|{3}|*.csv", new object[]
				{
					Strings.ExportListFileFilterTextTab,
					Strings.ExportListFileFilterTextComma,
					Strings.ExportListFileFilterUnicodeTab,
					Strings.ExportListFileFilterUnicodeComma
				});
				if (DialogResult.OK == saveFileDialog.ShowDialog(owner))
				{
					Encoding fileEncoding = (saveFileDialog.FilterIndex <= 2) ? Encoding.Default : Encoding.Unicode;
					char separator = (saveFileDialog.FilterIndex % 2 == 1) ? '\t' : ',';
					try
					{
						listControl.ExportListToFile(saveFileDialog.FileName, fileEncoding, separator);
					}
					catch (IOException ex)
					{
						uiService.ShowError(Strings.ExportListFileIOError(ex.Message));
					}
					catch (UnauthorizedAccessException ex2)
					{
						uiService.ShowError(Strings.ExportListFileIOError(ex2.Message));
					}
				}
			}
		}

		internal static bool OverrideCorruptedValuesWithDefault(ADObject dataSource)
		{
			bool result = false;
			try
			{
				dataSource.OverrideCorruptedValuesWithDefault();
			}
			catch (Exception)
			{
			}
			finally
			{
				result = (ObjectState.Changed == dataSource.ObjectState);
			}
			return result;
		}

		public static string GetDllResourceString(string dllPath, int resourceId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (SafeLibraryHandle safeLibraryHandle = SafeLibraryHandle.LoadLibrary(dllPath))
			{
				if (!safeLibraryHandle.IsInvalid)
				{
					int num = NativeMethods.LoadString(safeLibraryHandle, resourceId, stringBuilder, 0);
					if (num != 0)
					{
						stringBuilder.EnsureCapacity(num + 1);
						NativeMethods.LoadString(safeLibraryHandle, resourceId, stringBuilder, stringBuilder.Capacity);
					}
				}
			}
			return stringBuilder.ToString();
		}

		internal static string GenerateFormName<TForm>(string objectId) where TForm : Form
		{
			return string.Format("{0}_{1}", typeof(TForm).Name, objectId);
		}

		internal static string GenerateFormName<TForm>(ADObjectId objectId) where TForm : Form
		{
			return WinformsHelper.GenerateFormName<TForm>(objectId.ObjectGuid.ToString());
		}

		public static bool IsValidRegKey(string path)
		{
			bool result = false;
			RegistryKey registryKey = WinformsHelper.GetRegistryKey(Registry.LocalMachine, path);
			if (registryKey != null)
			{
				result = true;
				registryKey.Close();
			}
			return result;
		}

		public static bool IsMailToShellCommandAvailable()
		{
			bool result = false;
			RegistryKey registryKey = WinformsHelper.GetRegistryKey(Registry.ClassesRoot, "mailto");
			if (registryKey != null)
			{
				result = (registryKey.GetValue("URL Protocol", null) != null);
				registryKey.Close();
			}
			return result;
		}

		private static RegistryKey GetRegistryKey(RegistryKey rootRegKey, string path)
		{
			RegistryKey result = null;
			try
			{
				result = rootRegKey.OpenSubKey(path);
			}
			catch (SecurityException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
			return result;
		}

		public static bool IsRemoteEnabled()
		{
			if (PSConnectionInfoSingleton.GetInstance().Type != OrganizationType.ToolOrEdge)
			{
				return true;
			}
			if (EnvironmentAnalyzer.IsWorkGroup())
			{
				return false;
			}
			string name = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\AdminTools";
			bool result = true;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue("EMC.RemotePowerShellEnabled");
						if (value != null && string.Equals("false", value.ToString(), StringComparison.OrdinalIgnoreCase))
						{
							result = false;
						}
					}
				}
			}
			catch (SecurityException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
			return result;
		}

		public static IList GetAddedList(this MultiValuedPropertyBase mvp)
		{
			return mvp.Added;
		}

		public static IList GetRemovedList(this MultiValuedPropertyBase mvp)
		{
			return mvp.Removed;
		}

		public static void InvokeAsync(MethodInvoker callback, IWin32Window owner)
		{
			using (InvisibleForm invisibleForm = new InvisibleForm())
			{
				invisibleForm.BackgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e2)
				{
					callback();
				};
				invisibleForm.ShowDialog(owner);
			}
		}

		public static object DeSerialize(byte[] data)
		{
			object result = null;
			if (data != null)
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null, new string[]
					{
						"System.DelegateSerializationHolder"
					});
					result = binaryFormatter.Deserialize(memoryStream);
				}
			}
			return result;
		}

		public static byte[] Serialize(object data)
		{
			if (data == null)
			{
				return null;
			}
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
				binaryFormatter.Serialize(memoryStream, data);
				result = memoryStream.ToArray();
			}
			return result;
		}

		public static bool ByteArrayEquals(byte[] source, byte[] destination)
		{
			bool result = true;
			if (source.Length == destination.Length)
			{
				for (int i = 0; i < source.Length; i++)
				{
					if (source[i] != destination[i])
					{
						result = false;
						break;
					}
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static ADObjectId GenerateADObjectId(Guid guid, string distinguishedName)
		{
			return new ADObjectId(distinguishedName, guid);
		}

		public static string GetADShortName(string fullName)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(fullName))
			{
				int num = fullName.LastIndexOf('/');
				result = ((num < 0) ? fullName : fullName.Substring(num + 1, fullName.Length - num - 1));
			}
			return result;
		}

		public static string GetFQDNShortName(object fqdn)
		{
			return WinformsHelper.GetServerShortName(fqdn as string);
		}

		public static string CalculateMoveRequestTypeForMailbox(object mailboxMoveStatus, object mailboxMoveFlags, object mailboxMoveRemoteHostName)
		{
			RequestStatus requestStatus = (RequestStatus)mailboxMoveStatus;
			RequestFlags requestFlags = (RequestFlags)mailboxMoveFlags;
			if (requestStatus != RequestStatus.None && (requestFlags & RequestFlags.IntraOrg) != RequestFlags.None)
			{
				return Strings.MoveRequestTypeIntraOrg;
			}
			if ((requestStatus == RequestStatus.None && !string.IsNullOrEmpty(mailboxMoveRemoteHostName as string)) || (requestStatus != RequestStatus.None && (requestFlags & RequestFlags.CrossOrg) != RequestFlags.None))
			{
				return Strings.MoveRequestTypeCrossOrg;
			}
			return null;
		}

		public static string CalculateMoveRequestTypeForMigration(object mailboxMoveFlags)
		{
			RequestFlags requestFlags = WinformsHelper.ParseRequestFlags(mailboxMoveFlags.ToString());
			return ((requestFlags & RequestFlags.CrossOrg) != RequestFlags.None) ? Strings.MoveRequestTypeCrossOrg : Strings.MoveRequestTypeIntraOrg;
		}

		public static MailboxMoveType CalcuateSupportedMailboxMoveType(object recipientType, object archiveState)
		{
			MailboxMoveType result = MailboxMoveType.None;
			RecipientType recipientType2 = (RecipientType)recipientType;
			if (recipientType2 == RecipientType.MailUser || recipientType2 == RecipientType.UserMailbox)
			{
				bool flag = (ArchiveState)archiveState == ArchiveState.Local;
				switch (recipientType2)
				{
				case RecipientType.UserMailbox:
					result = (flag ? MailboxMoveType.BothUserAndArchive : MailboxMoveType.OnlyUserMailbox);
					break;
				case RecipientType.MailUser:
					if (flag)
					{
						result = MailboxMoveType.OnlyArchiveMailbox;
					}
					break;
				}
			}
			return result;
		}

		public static object CalculateCanNewMoveRequest(object recipientType, object mailboxMoveStatus, object mailboxMoveFlags, object mailboxMoveRemoteHostName, object archiveState)
		{
			bool flag = false;
			RecipientType recipientType2 = (RecipientType)recipientType;
			if ((recipientType2 == RecipientType.MailUser || recipientType2 == RecipientType.UserMailbox) && string.IsNullOrEmpty(WinformsHelper.CalculateMoveRequestTypeForMailbox(mailboxMoveStatus, mailboxMoveFlags, mailboxMoveRemoteHostName)))
			{
				flag = (recipientType2 == RecipientType.UserMailbox || ((ArchiveState)archiveState == ArchiveState.Local && WinformsHelper.IsCloudOrganization()));
			}
			return flag;
		}

		public static DataTableLoaderView SetDataSource(DataListView dataListView, UIPresentationProfile uiPresentationProfile, DataTableLoader dataTableLoader)
		{
			DataTableLoaderView dataTableLoaderView = (dataTableLoader == null) ? null : DataTableLoaderView.Create(dataTableLoader);
			WinformsHelper.SetDataSource(dataListView, uiPresentationProfile, dataTableLoaderView);
			return dataTableLoaderView;
		}

		public static IList GetSelectedVirtualDirectory(IList directories, IList servers)
		{
			if (directories == null || servers == null)
			{
				return null;
			}
			return (from dir in directories.OfType<ADVirtualDirectory>()
			where servers.Contains(dir.Server)
			select dir).ToArray<ADVirtualDirectory>();
		}

		public static FilteredDataTableLoaderView SetFilteredDataSource(DataListView dataListView, UIPresentationProfile uiPresentationProfile, DataTableLoader dataTableLoader)
		{
			FilteredDataTableLoaderView filteredDataTableLoaderView = (dataTableLoader == null) ? null : FilteredDataTableLoaderView.Create(dataTableLoader);
			WinformsHelper.SetDataSource(dataListView, uiPresentationProfile, filteredDataTableLoaderView);
			return filteredDataTableLoaderView;
		}

		public static void SetDataSource(DataListView dataListView, UIPresentationProfile uiPresentationProfile, DataTableLoaderView dataTableLoaderView)
		{
			if (dataListView == null)
			{
				throw new ArgumentNullException("dataListView");
			}
			if (!WinformsHelper.CheckDataSource(dataListView.DataSource))
			{
				throw new ArgumentException("dataListView");
			}
			if (dataListView.DataSource != null)
			{
				DataTableLoaderView dataTableLoaderView2 = (dataListView.DataSource as AdvancedBindingSource).DataSource as DataTableLoaderView;
				dataListView.DataSource = null;
				if (dataTableLoaderView2 != null)
				{
					dataTableLoaderView2.Dispose();
				}
			}
			if (dataTableLoaderView != null)
			{
				WinformsHelper.SyncSortSupportDescriptions(dataListView, uiPresentationProfile, dataTableLoaderView);
				dataListView.DataSource = new AdvancedBindingSource
				{
					DataSource = dataTableLoaderView
				};
			}
		}

		public static DataTableLoaderView SetDataSource(ObjectList objectList, UIPresentationProfile uiPresentationProfile, DataTableLoader dataTableLoader)
		{
			DataTableLoaderView dataTableLoaderView = (dataTableLoader == null) ? null : DataTableLoaderView.Create(dataTableLoader);
			WinformsHelper.SetDataSource(objectList, uiPresentationProfile, dataTableLoaderView);
			return dataTableLoaderView;
		}

		public static FilteredDataTableLoaderView SetFilteredDataSource(ObjectList objectList, UIPresentationProfile uiPresentationProfile, DataTableLoader dataTableLoader)
		{
			FilteredDataTableLoaderView filteredDataTableLoaderView = (dataTableLoader == null) ? null : FilteredDataTableLoaderView.Create(dataTableLoader);
			WinformsHelper.SetDataSource(objectList, uiPresentationProfile, filteredDataTableLoaderView);
			return filteredDataTableLoaderView;
		}

		public static void SetDataSource(ObjectList objectList, UIPresentationProfile uiPresentationProfile, DataTableLoaderView dataTableLoaderView)
		{
			if (objectList == null)
			{
				throw new ArgumentNullException("objectList");
			}
			if (!WinformsHelper.CheckDataSource(objectList.DataSource))
			{
				throw new ArgumentException("objectList");
			}
			if (objectList.DataSource != null)
			{
				DataTableLoaderView dataTableLoaderView2 = (objectList.DataSource as AdvancedBindingSource).DataSource as DataTableLoaderView;
				objectList.DataSource = null;
				if (dataTableLoaderView2 != null)
				{
					dataTableLoaderView2.Dispose();
				}
			}
			if (dataTableLoaderView != null)
			{
				WinformsHelper.SyncSortSupportDescriptions(objectList.ListView, uiPresentationProfile, dataTableLoaderView);
				objectList.DataSource = new AdvancedBindingSource
				{
					DataSource = dataTableLoaderView
				};
			}
		}

		private static bool CheckDataSource(object dataSource)
		{
			return dataSource == null || (dataSource is AdvancedBindingSource && ((dataSource as AdvancedBindingSource).DataSource == null || (dataSource as AdvancedBindingSource).DataSource is DataTableLoaderView));
		}

		private static void SyncSortSupportDescriptions(DataListView dataListView, UIPresentationProfile uiPresentationProfile, DataTableLoaderView dataTableLoaderView)
		{
			if (uiPresentationProfile != null)
			{
				foreach (ResultsColumnProfile resultsColumnProfile in uiPresentationProfile.DisplayedColumnCollection)
				{
					dataTableLoaderView.SortSupportDescriptions.Add(new SortSupportDescription(resultsColumnProfile.Name, resultsColumnProfile.SortMode, resultsColumnProfile.CustomComparer, resultsColumnProfile.CustomFormatter, resultsColumnProfile.FormatProvider, resultsColumnProfile.FormatString, resultsColumnProfile.DefaultEmptyText));
				}
			}
			using (IEnumerator<ExchangeColumnHeader> enumerator = dataListView.AvailableColumns.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ExchangeColumnHeader columnHeader = enumerator.Current;
					if (columnHeader.IsSortable)
					{
						if (!dataTableLoaderView.SortSupportDescriptions.Any((SortSupportDescription obj) => string.Compare(obj.ColumnName, columnHeader.Name, StringComparison.OrdinalIgnoreCase) == 0))
						{
							dataTableLoaderView.SortSupportDescriptions.Add(new SortSupportDescription(columnHeader.Name, SortMode.NotSpecified, null, columnHeader.CustomFormatter, columnHeader.FormatProvider, columnHeader.FormatString, columnHeader.DefaultEmptyText));
						}
					}
				}
			}
		}

		public static UMLanguageListSource ConvertUMLanguageListToUMLanguageListSource(IList umLanguageList)
		{
			return new UMLanguageListSource((umLanguageList == null) ? new UMLanguage[0] : umLanguageList.Cast<UMLanguage>().ToArray<UMLanguage>());
		}

		public static object FindItemAndGetPropertyValue(IList items, string keyPropertyName, object keyPropertyValue, string valuePropertyName)
		{
			foreach (object obj in items)
			{
				PropertyInfo property = obj.GetType().GetProperty(keyPropertyName);
				PropertyInfo property2 = obj.GetType().GetProperty(valuePropertyName);
				if (keyPropertyValue.Equals(property.GetValue(obj, null)))
				{
					return property2.GetValue(obj, null);
				}
			}
			return null;
		}

		public static IList GetAddedFederatedDomains(object originalAccountNS, object accountNS, object federatedDomains)
		{
			SmtpDomain rhs = originalAccountNS as SmtpDomain;
			SmtpDomain smtpDomain = accountNS as SmtpDomain;
			MultiValuedProperty<FederatedDomain> multiValuedProperty = federatedDomains as MultiValuedProperty<FederatedDomain>;
			List<SmtpDomain> list = new List<SmtpDomain>();
			if (multiValuedProperty != null && smtpDomain != null)
			{
				IList list2 = multiValuedProperty.Added;
				if (!smtpDomain.Equals(rhs))
				{
					list2 = multiValuedProperty;
				}
				foreach (object obj in list2)
				{
					FederatedDomain federatedDomain = (FederatedDomain)obj;
					if (!federatedDomain.Domain.Equals(smtpDomain))
					{
						list.Add(federatedDomain.Domain);
					}
				}
			}
			return list.ToArray();
		}

		public static IList GetRemovedFederatedDomains(object originalAccountNS, object accountNS, object federatedDomains)
		{
			SmtpDomain smtpDomain = originalAccountNS as SmtpDomain;
			SmtpDomain objB = accountNS as SmtpDomain;
			MultiValuedProperty<FederatedDomain> multiValuedProperty = federatedDomains as MultiValuedProperty<FederatedDomain>;
			List<SmtpDomain> list = new List<SmtpDomain>();
			if (multiValuedProperty != null && smtpDomain != null)
			{
				foreach (FederatedDomain federatedDomain in multiValuedProperty.Removed)
				{
					if (!federatedDomain.Domain.Equals(smtpDomain))
					{
						list.Add(federatedDomain.Domain);
					}
				}
				if (!object.Equals(smtpDomain, objB))
				{
					foreach (FederatedDomain federatedDomain2 in multiValuedProperty)
					{
						if (!federatedDomain2.Domain.Equals(smtpDomain) && !WinformsHelper.FederatedDomainsContains(multiValuedProperty.Added, federatedDomain2.Domain))
						{
							list.Add(federatedDomain2.Domain);
						}
					}
					list.Add(smtpDomain);
				}
			}
			return list.ToArray();
		}

		private static bool FederatedDomainsContains(object[] fedDomainsList, SmtpDomain domain)
		{
			bool result = false;
			if (fedDomainsList != null)
			{
				foreach (FederatedDomain federatedDomain in fedDomainsList)
				{
					if (domain.Equals(federatedDomain.Domain))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public static string GenerateFederationDNSRecordExample(object acceptedDomains, object appID)
		{
			IList list = acceptedDomains as IList;
			string arg = appID as string;
			string arg2 = "example.com";
			if (list != null)
			{
				foreach (object obj in list)
				{
					AcceptedDomain acceptedDomain = (AcceptedDomain)obj;
					if (acceptedDomain.DomainType == AcceptedDomainType.Authoritative)
					{
						arg2 = acceptedDomain.DomainName.Domain;
						if (acceptedDomain.Default)
						{
							break;
						}
					}
				}
			}
			return string.Format("{0} IN TXT AppID={1}", arg2, arg);
		}

		public static MultiValuedProperty<FederatedDomain> GenerateMvpFederatedDomains(object fedDomains)
		{
			MultiValuedProperty<FederatedDomain> multiValuedProperty = new MultiValuedProperty<FederatedDomain>();
			if (!fedDomains.IsNullValue())
			{
				foreach (object obj in ((IList)fedDomains))
				{
					FederatedDomain item = (FederatedDomain)obj;
					multiValuedProperty.Add(item);
				}
			}
			multiValuedProperty.ResetChangeTracking();
			return multiValuedProperty;
		}

		public static MultiValuedProperty<ADObjectId> GetDagServers(object dagList, object dagIdentity)
		{
			if (dagList == null)
			{
				throw new ArgumentNullException("dagList");
			}
			if (!(dagList is IList))
			{
				throw new ArgumentException("dagList");
			}
			if (dagIdentity == null)
			{
				throw new ArgumentNullException("dagIdentity");
			}
			MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>();
			foreach (object obj in ((IList)dagList))
			{
				DatabaseAvailabilityGroup databaseAvailabilityGroup = (DatabaseAvailabilityGroup)obj;
				if (databaseAvailabilityGroup != null && databaseAvailabilityGroup.Identity.Equals(dagIdentity))
				{
					using (MultiValuedProperty<ADObjectId>.Enumerator enumerator2 = databaseAvailabilityGroup.Servers.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							ADObjectId item = enumerator2.Current;
							multiValuedProperty.Add(item);
						}
						break;
					}
				}
			}
			multiValuedProperty.ResetChangeTracking();
			return multiValuedProperty;
		}

		public static ADObjectId[] GetServersInDag(object dagList, object dagIdentity)
		{
			if (dagList == null)
			{
				throw new ArgumentNullException("dagList");
			}
			if (!(dagList is IList))
			{
				throw new ArgumentException("dagList");
			}
			if (dagIdentity == null)
			{
				throw new ArgumentNullException("dagIdentity");
			}
			List<ADObjectId> list = new List<ADObjectId>();
			foreach (object obj in ((IList)dagList))
			{
				DatabaseAvailabilityGroup databaseAvailabilityGroup = (DatabaseAvailabilityGroup)obj;
				if (databaseAvailabilityGroup != null && !databaseAvailabilityGroup.Identity.Equals(dagIdentity))
				{
					list.AddRange(databaseAvailabilityGroup.Servers);
				}
			}
			return list.ToArray();
		}

		public static string GetTargetTypeToStringValue(object obj, Type targetType)
		{
			if (obj == null)
			{
				return null;
			}
			if (targetType.IsEnum && !obj.IsNullValue())
			{
				return Enum.ToObject(targetType, obj).ToString();
			}
			return obj.ToString();
		}

		public static object GetPropertyValue(object component, PropertyDescriptor propertyDescriptor)
		{
			object obj = propertyDescriptor.GetValue(component);
			if (obj != null && DBNull.Value != obj && propertyDescriptor.PropertyType.IsEnum)
			{
				obj = Enum.ToObject(propertyDescriptor.PropertyType, obj);
			}
			return obj;
		}

		public static string ConvertValueToString(DataColumn orignalColumn, object rawValue)
		{
			if (orignalColumn.DataType.IsEnum && !DBNull.Value.Equals(rawValue))
			{
				rawValue = Enum.ToObject(orignalColumn.DataType, rawValue);
			}
			ICustomTextConverter customTextConverter = (orignalColumn.ExtendedProperties["TextConverter"] as ICustomTextConverter) ?? TextConverter.DefaultConverter;
			return customTextConverter.Format(null, rawValue, null);
		}

		public static Icon GetIconFromIconLibrary(string iconName)
		{
			PropertyInfo property = typeof(Icons).GetProperty(iconName, BindingFlags.Static | BindingFlags.Public);
			if (!(property != null))
			{
				return null;
			}
			return (Icon)property.GetValue(null, null);
		}

		internal static bool IsConnectedWithLocalServer()
		{
			string localServerName = EnvironmentAnalyzer.GetLocalServerName();
			return WinformsHelper.IsCurrentConnectedServer(localServerName);
		}

		internal static bool IsCurrentConnectedServer(string serverFqdn)
		{
			string currentConnectedServerName = WinformsHelper.GetCurrentConnectedServerName();
			return string.Equals(currentConnectedServerName, serverFqdn, StringComparison.OrdinalIgnoreCase);
		}

		internal static string GetCurrentConnectedServerName()
		{
			string result = string.Empty;
			if (WinformsHelper.IsRemoteEnabled())
			{
				result = PSConnectionInfoSingleton.GetInstance().ServerName;
			}
			else
			{
				result = EnvironmentAnalyzer.GetLocalServerName();
			}
			return result;
		}

		public static string GetServerShortName(string serverName)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				return string.Empty;
			}
			int num = serverName.IndexOf(".", StringComparison.OrdinalIgnoreCase);
			if (num < 0)
			{
				return serverName;
			}
			return serverName.Substring(0, num);
		}

		public static void BindRadioButtonToOptionValue<T>(AutoHeightRadioButton radioButton, T optionValue, object dataSource, string dataMember)
		{
			if (radioButton == null)
			{
				throw new ArgumentNullException("radioButton");
			}
			Binding dataBinding = radioButton.DataBindings.Add("Checked", dataSource, dataMember, true, DataSourceUpdateMode.Never);
			dataBinding.Format += delegate(object sender, ConvertEventArgs e)
			{
				e.Value = (!e.Value.IsNullValue() && optionValue.Equals((T)((object)e.Value)));
			};
			dataBinding.Parse += delegate(object sender, ConvertEventArgs e)
			{
				e.Value = optionValue;
				dataBinding.DataSourceUpdateMode = DataSourceUpdateMode.Never;
			};
			radioButton.CheckedChangedRaising += delegate(object sender, HandledEventArgs e)
			{
				dataBinding.DataSourceUpdateMode = (radioButton.Checked ? DataSourceUpdateMode.OnPropertyChanged : DataSourceUpdateMode.Never);
			};
		}

		public static bool HasLocalArchive(object mailboxes)
		{
			DataTable dataTable = mailboxes as DataTable;
			if (dataTable == null)
			{
				return false;
			}
			return (from q in dataTable.AsEnumerable()
			select q["ArchiveDatabase"]).Any((object db) => !DBNull.Value.Equals(db));
		}

		public static string GenerateInternalUrl(object vdir, object serverFqdn)
		{
			string result = string.Empty;
			string text = serverFqdn as string;
			if (!DBNull.Value.Equals(vdir) && !string.IsNullOrEmpty(text))
			{
				Dictionary<Type, string> dictionary = new Dictionary<Type, string>
				{
					{
						typeof(ADOwaVirtualDirectory),
						"https://" + text + "/owa"
					},
					{
						typeof(ADEcpVirtualDirectory),
						"https://" + text + "/ecp"
					},
					{
						typeof(ADWebServicesVirtualDirectory),
						string.Empty
					},
					{
						typeof(ADMobileVirtualDirectory),
						string.Empty
					},
					{
						typeof(ADAutodiscoverVirtualDirectory),
						string.Empty
					},
					{
						typeof(ADOabVirtualDirectory),
						string.Empty
					}
				};
				result = dictionary[vdir.GetType()];
			}
			return result;
		}

		public static string GenerateResetVdirCmdlet(string verb, object vdir)
		{
			if (!DBNull.Value.Equals(vdir))
			{
				Dictionary<Type, string> dictionary = new Dictionary<Type, string>
				{
					{
						typeof(ADOwaVirtualDirectory),
						"OwaVirtualDirectory"
					},
					{
						typeof(ADEcpVirtualDirectory),
						"EcpVirtualDirectory"
					},
					{
						typeof(ADWebServicesVirtualDirectory),
						"WebServicesVirtualDirectory"
					},
					{
						typeof(ADMobileVirtualDirectory),
						"ActiveSyncVirtualDirectory"
					},
					{
						typeof(ADAutodiscoverVirtualDirectory),
						"AutodiscoverVirtualDirectory"
					},
					{
						typeof(ADOabVirtualDirectory),
						"OabVirtualDirectory"
					}
				};
				return string.Format("{0}-{1}", verb, dictionary[vdir.GetType()]);
			}
			return string.Empty;
		}

		public static string GetWebSiteNameForVdir(object vdir)
		{
			string result = string.Empty;
			if (!DBNull.Value.Equals(vdir))
			{
				string name = (vdir as ADObject).Name;
				Regex regex = new Regex("\\(.*\\)");
				Match match = regex.Match(name);
				if (!match.Success)
				{
					throw new ArgumentException(string.Format("{0} is not a valid virtual directory name", name));
				}
				result = match.Value.Trim(new char[]
				{
					'(',
					')'
				});
			}
			return result;
		}

		public static object GetIdentityForVdir(object vdir)
		{
			ADObject adobject = vdir as ADObject;
			if (adobject == null)
			{
				return null;
			}
			return adobject.Identity;
		}

		public static MultiValuedProperty<PublicFolderAccessRight> GetPublicFolderAccessRights(object publicFolderPermission)
		{
			return PublicFolderAccessRight.CreatePublicFolderAccessRightCollection((PublicFolderPermission)publicFolderPermission);
		}

		public static bool IsCloudOrganization()
		{
			return PSConnectionInfoSingleton.GetInstance().Type == OrganizationType.Cloud;
		}

		public static bool IsCurrentOrganizationAllowed(IList<OrganizationType> organizationTypeList)
		{
			return organizationTypeList == null || organizationTypeList.Count == 0 || organizationTypeList.Contains(PSConnectionInfoSingleton.GetInstance().Type);
		}

		public static bool IsCmdletAllowedInScope(string commandText, string parameter)
		{
			return EMCRunspaceConfigurationSingleton.GetInstance().IsCmdletAllowedInScope(commandText, new string[]
			{
				parameter
			});
		}

		public static string NewCertificateSubjectKeyIdentifier()
		{
			return Guid.NewGuid().ToString("N");
		}

		public static IList GetFederationCertificateDomainName()
		{
			MultiValuedProperty<SmtpDomainWithSubdomains> multiValuedProperty = new MultiValuedProperty<SmtpDomainWithSubdomains>();
			multiValuedProperty.Add(new SmtpDomainWithSubdomains("Federation", false));
			multiValuedProperty.ResetChangeTracking();
			return multiValuedProperty;
		}

		public static bool IsWin7OrLater()
		{
			return Environment.OSVersion.Version.Major >= 7 || (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 1);
		}

		public static object GetRegistryKeyValue(string keyPath, string valueName)
		{
			object result = null;
			try
			{
				result = Registry.GetValue(keyPath, valueName, null);
			}
			catch (SecurityException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
			return result;
		}

		public static bool IsIDCRLNotReady()
		{
			if (WinformsHelper.IsWin7OrLater())
			{
				string keyPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\MSOIdentityCRL";
				return null == WinformsHelper.GetRegistryKeyValue(keyPath, "MSOIDCRLVersion");
			}
			return false;
		}

		public static RequestStatus ParseRequestStatus(string status)
		{
			return (RequestStatus)Enum.Parse(typeof(RequestStatus), status);
		}

		public static RequestFlags ParseRequestFlags(string status)
		{
			return (RequestFlags)Enum.Parse(typeof(RequestFlags), status);
		}
	}
}
