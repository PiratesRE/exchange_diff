using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Management;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Common.DiskManagement;
using Microsoft.Exchange.Common.DiskManagement.Utilities;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common.Bitlocker.Utilities
{
	public static class BitlockerUtil
	{
		public static Exception AddCertificateBasedKeyProtectors(ManagementObject encryptableVolume, out int returnValueCertificate)
		{
			int returnValue = -1;
			returnValueCertificate = -1;
			Exception result = null;
			ArrayList bitlockerCertThumbPrints = BitlockerUtil.GetBitlockerCertThumbPrints(out result);
			if (bitlockerCertThumbPrints == null)
			{
				return new BitlockerCertificatesNotFoundException();
			}
			using (IEnumerator enumerator = bitlockerCertThumbPrints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string bitlockerCertThumbPrint = (string)enumerator.Current;
					result = Util.HandleExceptions(delegate
					{
						ManagementBaseObject managementBaseObject = null;
						returnValue = WMIUtil.CallWMIMethod(encryptableVolume, "ProtectKeyWithCertificateThumbprint", new string[]
						{
							"FriendlyName",
							"CertThumbprint"
						}, new object[]
						{
							bitlockerCertThumbPrint,
							bitlockerCertThumbPrint
						}, out managementBaseObject);
					});
				}
			}
			returnValueCertificate = returnValue;
			return result;
		}

		public static Exception ValidateAndEncryptEmptyWin8Volume(ManagementObject encryptableVolume, bool fipsCompliant, out int returnValueEncrypt)
		{
			int returnValue = -1;
			returnValueEncrypt = -1;
			if (encryptableVolume == null)
			{
				return new EncryptableVolumeArgNullException("ValidateAndEncryptEmptyWin8Volume");
			}
			string deviceID = encryptableVolume.GetPropertyValue("DeviceId").ToString();
			if (!Util.IsOperatingSystemWin8OrHigher())
			{
				return new UsedOnlyEncryptionOnNonWin8ServerException(deviceID);
			}
			Exception ex = Util.HandleExceptions(delegate
			{
				Exception ex = null;
				if (!BitlockerUtil.IsVolumeEmpty(deviceID, out ex))
				{
					Util.ThrowIfNotNull(ex);
					throw new UsedOnlySpaceEncryptionAttemptOnANonEmptyVolumeException(deviceID);
				}
				if (!fipsCompliant)
				{
					ex = BitlockerUtil.AddNumericalPassword(encryptableVolume, "563563-218372-416746-433752-541937-608069-594110-446754", out returnValue);
					Util.ThrowIfNotNull(ex);
				}
				ex = BitlockerUtil.AddCertificateBasedKeyProtectors(encryptableVolume, out returnValue);
				Util.ThrowIfNotNull(ex);
				ex = BitlockerUtil.Encrypt(encryptableVolume, true, out returnValue);
				Util.ThrowIfNotNull(ex);
				TimeSpan timeSpan = TimeSpan.FromSeconds(300.0);
				BitlockerUtil.BitlockerConversionState bitlockerConversionState;
				while ((bitlockerConversionState = BitlockerUtil.GetBitlockerConversionState(encryptableVolume, out returnValue, out ex)) != BitlockerUtil.BitlockerConversionState.FullyEncrypted)
				{
					Thread.Sleep(TimeSpan.FromSeconds(1.0));
					timeSpan = timeSpan.Subtract(TimeSpan.FromSeconds(1.0));
					if (timeSpan.CompareTo(TimeSpan.Zero) < 0)
					{
						break;
					}
				}
				if (bitlockerConversionState != BitlockerUtil.BitlockerConversionState.FullyEncrypted)
				{
					throw new Win8EmptyVolumeNotFullyEncryptedAfterWaitException(deviceID, 300, bitlockerConversionState.ToString());
				}
			});
			returnValueEncrypt = returnValue;
			return Util.ReturnWMIErrorExceptionOnExceptionOrError(returnValueEncrypt, "ValidateAndEncryptEmptyWin8Volume", ex);
		}

		public static Exception ValidateAndEncryptEmptyWin7Volume(ManagementObject encryptableVolume, bool fipsCompliant, out int returnValueEncrypt)
		{
			int returnValue = -1;
			returnValueEncrypt = -1;
			if (encryptableVolume == null)
			{
				return new EncryptableVolumeArgNullException("ValidateAndEncryptEmptyWin7Volume");
			}
			string deviceID = encryptableVolume.GetPropertyValue("DeviceId").ToString();
			if (Util.IsOperatingSystemWin8OrHigher())
			{
				return new FullVolumeEncryptionOnWin8ServerException(deviceID);
			}
			Exception ex = Util.HandleExceptions(delegate
			{
				Exception ex = null;
				if (!BitlockerUtil.IsVolumeEmpty(deviceID, out ex))
				{
					Util.ThrowIfNotNull(ex);
					throw new FullVolumeEncryptionAttemptOnANonEmptyVolumeException(deviceID);
				}
				string mountPoint;
				string eventXML;
				bool flag = BitlockerUtil.DoesVolumeHaveBadBlocks(deviceID, out mountPoint, out ex, out eventXML);
				if (flag)
				{
					throw new Win7EmptyVolumeNotStartedDueToPreviousFailureBadBlocksException(deviceID, mountPoint, eventXML);
				}
				if (!fipsCompliant)
				{
					ex = BitlockerUtil.AddNumericalPassword(encryptableVolume, "563563-218372-416746-433752-541937-608069-594110-446754", out returnValue);
					Util.ThrowIfNotNull(ex);
				}
				ex = BitlockerUtil.AddCertificateBasedKeyProtectors(encryptableVolume, out returnValue);
				Util.ThrowIfNotNull(ex);
				ex = BitlockerUtil.Encrypt(encryptableVolume, false, out returnValue);
				Util.ThrowIfNotNull(ex);
				BitlockerUtil.BitlockerConversionState bitlockerConversionState = BitlockerUtil.GetBitlockerConversionState(encryptableVolume, out returnValue, out ex);
				Util.ThrowIfNotNull(ex);
				if (bitlockerConversionState != BitlockerUtil.BitlockerConversionState.EncryptionInProgress)
				{
					throw new Win7EmptyVolumeNotEncryptingAfterStartingEncryptionException(deviceID, bitlockerConversionState.ToString());
				}
			});
			returnValueEncrypt = returnValue;
			return Util.ReturnWMIErrorExceptionOnExceptionOrError(returnValueEncrypt, "ValidateAndEncryptEmptyWin7Volume", ex);
		}

		public static Exception Encrypt(ManagementObject encryptableVolume, bool usedOnly, out int returnValueEncrypt)
		{
			Exception ex = null;
			int returnValue = -1;
			if (BitlockerLockUtil.IsVolumeLocked(encryptableVolume, out ex))
			{
				returnValueEncrypt = 0;
				return ex;
			}
			ex = Util.HandleExceptions(delegate
			{
				ManagementBaseObject managementBaseObject = null;
				if (Util.IsOperatingSystemWin8OrHigher() && usedOnly)
				{
					returnValue = WMIUtil.CallWMIMethod(encryptableVolume, "Encrypt", new string[]
					{
						"EncryptionMethod",
						"EncryptionFlags"
					}, new object[]
					{
						4,
						1
					}, out managementBaseObject);
					return;
				}
				returnValue = WMIUtil.CallWMIMethod(encryptableVolume, "Encrypt", new string[]
				{
					"EncryptionMethod"
				}, new object[]
				{
					4
				}, out managementBaseObject);
			});
			returnValueEncrypt = returnValue;
			Util.AssertReturnValueExceptionInconsistency(returnValueEncrypt, "Encrypt", ex);
			return Util.ReturnWMIErrorExceptionOnExceptionOrError(returnValueEncrypt, "Encrypt", ex);
		}

		public static Exception Decrypt(ManagementObject encryptableVolume, out int returnValueDecrypt)
		{
			ManagementBaseObject outParams = null;
			int returnValue = -1;
			Exception ex = Util.HandleExceptions(delegate
			{
				returnValue = WMIUtil.CallWMIMethod(encryptableVolume, "Decrypt", null, null, out outParams);
			});
			returnValueDecrypt = returnValue;
			Util.AssertReturnValueExceptionInconsistency(returnValueDecrypt, "Decrypt", ex);
			return Util.ReturnWMIErrorExceptionOnExceptionOrError(returnValueDecrypt, "Decrypt", ex);
		}

		public static Exception AddNumericalPassword(ManagementObject encryptableVolume, string numericalPassword, out int returnValueNumericalPassword)
		{
			Exception ex = null;
			if (BitlockerLockUtil.IsVolumeLocked(encryptableVolume, out ex))
			{
				returnValueNumericalPassword = 0;
				return ex;
			}
			int returnValue = -1;
			ManagementBaseObject outParams = null;
			ex = Util.HandleExceptions(delegate
			{
				returnValue = WMIUtil.CallWMIMethod(encryptableVolume, "ProtectKeyWithNumericalPassword", new string[]
				{
					"FriendlyName",
					"NumericalPassword"
				}, new object[]
				{
					"FriendlyName",
					numericalPassword
				}, out outParams);
			});
			returnValueNumericalPassword = returnValue;
			Util.AssertReturnValueExceptionInconsistency(returnValueNumericalPassword, "AddNumericalPassword", ex);
			return Util.ReturnWMIErrorExceptionOnExceptionOrError(returnValueNumericalPassword, "AddNumericalPassword", ex);
		}

		public static Exception Pause(ManagementObject encryptableVolume, out int returnValuePaused)
		{
			Exception ex = null;
			int returnValue = -1;
			returnValuePaused = -1;
			if (encryptableVolume == null)
			{
				return new EncryptableVolumeArgNullException("Pause");
			}
			string volume = encryptableVolume.GetPropertyValue("DeviceId").ToString();
			if (BitlockerLockUtil.IsVolumeLocked(encryptableVolume, out ex))
			{
				ex = new VolumeLockedException(volume);
				return ex;
			}
			ex = Util.HandleExceptions(delegate
			{
				ManagementBaseObject managementBaseObject = null;
				returnValue = WMIUtil.CallWMIMethod(encryptableVolume, "PauseConversion", null, null, out managementBaseObject);
			});
			returnValuePaused = returnValue;
			Util.AssertReturnValueExceptionInconsistency(returnValuePaused, "Pause", ex);
			return Util.ReturnWMIErrorExceptionOnExceptionOrError(returnValuePaused, "Pause", ex);
		}

		public static Exception Resume(ManagementObject encryptableVolume, out int returnValueResume)
		{
			Exception ex = null;
			int returnValue = -1;
			returnValueResume = -1;
			if (encryptableVolume == null)
			{
				return new EncryptableVolumeArgNullException("Resume");
			}
			string volume = encryptableVolume.GetPropertyValue("DeviceId").ToString();
			if (BitlockerLockUtil.IsVolumeLocked(encryptableVolume, out ex))
			{
				ex = new VolumeLockedException(volume);
				return ex;
			}
			ex = Util.HandleExceptions(delegate
			{
				ManagementBaseObject managementBaseObject = null;
				returnValue = WMIUtil.CallWMIMethod(encryptableVolume, "ResumeConversion", null, null, out managementBaseObject);
			});
			returnValueResume = returnValue;
			Util.AssertReturnValueExceptionInconsistency(returnValueResume, "Resume", ex);
			return Util.ReturnWMIErrorExceptionOnExceptionOrError(returnValueResume, "Resume", ex);
		}

		public static ManagementObjectCollection GetEncryptableVolumes()
		{
			Exception ex;
			return WMIUtil.GetManagementObjectCollection("Win32_EncryptableVolume", "\\ROOT\\CIMV2\\Security\\MicrosoftVolumeEncryption", out ex);
		}

		public static ArrayList GetBitlockerCertThumbPrints(out Exception ex)
		{
			ArrayList bitlockerCertThumbPrints = null;
			ex = Util.HandleExceptions(delegate
			{
				X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.ReadOnly);
				X509Certificate2Collection certificates = x509Store.Certificates;
				foreach (X509Certificate2 x509Certificate in certificates)
				{
					if (x509Certificate.FriendlyName.StartsWith("bitlocker"))
					{
						if (bitlockerCertThumbPrints == null)
						{
							bitlockerCertThumbPrints = new ArrayList();
						}
						bitlockerCertThumbPrints.Add(x509Certificate.Thumbprint);
					}
				}
				x509Store.Close();
			});
			return bitlockerCertThumbPrints;
		}

		public static bool IsFilePathOnLockedVolume(string filePath, out Exception ex)
		{
			ex = null;
			List<string> lockedVolumes = BitlockerLockUtil.GetLockedVolumes(out ex);
			if (ex != null)
			{
				ex = new LockedVolumesFindException(ex.Message, ex);
				return false;
			}
			if (lockedVolumes == null || lockedVolumes.Count == 0)
			{
				return false;
			}
			Dictionary<string, string> mountPointVolumeIDMappings = BitlockerUtil.GetMountPointVolumeIDMappings(out ex);
			if (mountPointVolumeIDMappings == null)
			{
				ex = new MountPointsFindException(ex.Message, ex);
				return false;
			}
			bool flag = false;
			foreach (string text in mountPointVolumeIDMappings.Keys)
			{
				if (filePath.StartsWith(text))
				{
					flag = true;
					if (lockedVolumes.Contains(mountPointVolumeIDMappings[text]))
					{
						return true;
					}
				}
			}
			if (!flag)
			{
				ex = new InvalidFilePathException(filePath);
			}
			return false;
		}

		public static List<string> GetEncryptingVolumes(out Exception ex)
		{
			List<string> encryptingVolumes = null;
			ex = Util.HandleExceptions(delegate
			{
				ManagementObjectCollection encryptableVolumes = BitlockerUtil.GetEncryptableVolumes();
				if (encryptableVolumes != null)
				{
					using (encryptableVolumes)
					{
						foreach (ManagementBaseObject managementBaseObject in encryptableVolumes)
						{
							ManagementObject managementObject = (ManagementObject)managementBaseObject;
							using (managementObject)
							{
								string item = managementObject.GetPropertyValue("DeviceId").ToString();
								Exception ex2;
								bool flag = BitlockerUtil.IsVolumeEncrypting(managementObject, out ex2);
								Util.ThrowIfNotNull(ex2);
								if (flag)
								{
									if (encryptingVolumes == null)
									{
										encryptingVolumes = new List<string>();
									}
									encryptingVolumes.Add(item);
								}
							}
						}
					}
				}
			});
			return encryptingVolumes;
		}

		public static List<string> GetEncryptionPausedVolumes(out Exception ex)
		{
			List<string> encryptingVolumes = null;
			ex = Util.HandleExceptions(delegate
			{
				ManagementObjectCollection encryptableVolumes = BitlockerUtil.GetEncryptableVolumes();
				if (encryptableVolumes != null)
				{
					using (encryptableVolumes)
					{
						foreach (ManagementBaseObject managementBaseObject in encryptableVolumes)
						{
							ManagementObject managementObject = (ManagementObject)managementBaseObject;
							using (managementObject)
							{
								string item = managementObject.GetPropertyValue("DeviceId").ToString();
								Exception ex2;
								bool flag = BitlockerUtil.IsVolumeEncryptionPaused(managementObject, out ex2);
								Util.ThrowIfNotNull(ex2);
								if (flag)
								{
									if (encryptingVolumes == null)
									{
										encryptingVolumes = new List<string>();
									}
									encryptingVolumes.Add(item);
								}
							}
						}
					}
				}
			});
			return encryptingVolumes;
		}

		public static bool IsFilePathOnEncryptingVolume(string filePath, out Exception ex)
		{
			ex = null;
			List<string> encryptingVolumes = BitlockerUtil.GetEncryptingVolumes(out ex);
			if (ex != null)
			{
				throw new EncryptingVolumesFindException(ex.Message, ex);
			}
			if (encryptingVolumes == null || encryptingVolumes.Count == 0)
			{
				return false;
			}
			Dictionary<string, string> mountPointVolumeIDMappings = BitlockerUtil.GetMountPointVolumeIDMappings(out ex);
			if (mountPointVolumeIDMappings == null)
			{
				throw new MountPointsFindException(ex.Message, ex);
			}
			bool flag = false;
			foreach (string text in mountPointVolumeIDMappings.Keys)
			{
				if (filePath.StartsWith(text))
				{
					flag = true;
					if (encryptingVolumes.Contains(mountPointVolumeIDMappings[text]))
					{
						return true;
					}
				}
			}
			if (flag)
			{
				return false;
			}
			throw new InvalidFilePathException(filePath);
		}

		public static bool IsVolumeFullyEncrypted(ManagementObject encryptableVolume, out Exception ex)
		{
			int num;
			return BitlockerUtil.GetBitlockerConversionState(encryptableVolume, out num, out ex) == BitlockerUtil.BitlockerConversionState.FullyEncrypted;
		}

		public static bool IsVolumeEncrypting(ManagementObject encryptableVolume, out Exception ex)
		{
			int num;
			BitlockerUtil.BitlockerConversionState bitlockerConversionState = BitlockerUtil.GetBitlockerConversionState(encryptableVolume, out num, out ex);
			return bitlockerConversionState == BitlockerUtil.BitlockerConversionState.EncryptionInProgress || bitlockerConversionState == BitlockerUtil.BitlockerConversionState.EncryptionPaused;
		}

		public static bool IsVolumeEncryptionPaused(ManagementObject encryptableVolume, out Exception ex)
		{
			int num;
			BitlockerUtil.BitlockerConversionState bitlockerConversionState = BitlockerUtil.GetBitlockerConversionState(encryptableVolume, out num, out ex);
			return bitlockerConversionState == BitlockerUtil.BitlockerConversionState.EncryptionPaused;
		}

		public static string GetBootVolumeDeviceID(out Exception ex)
		{
			string bootVolumeDeviceID = null;
			ex = Util.HandleExceptions(delegate
			{
				SelectQuery query = new SelectQuery("SELECT * FROM Win32_Volume Where BootVolume=True");
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query))
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						using (managementObject)
						{
							bootVolumeDeviceID = managementObject.GetPropertyValue("DeviceId").ToString();
							break;
						}
					}
				}
			});
			return bootVolumeDeviceID;
		}

		public static string GetSystemVolumeDeviceID(out Exception ex)
		{
			string systemVolumeDeviceID = null;
			ex = Util.HandleExceptions(delegate
			{
				SelectQuery query = new SelectQuery("SELECT * FROM Win32_Volume Where SystemVolume=True");
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query))
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						using (managementObject)
						{
							systemVolumeDeviceID = managementObject.GetPropertyValue("DeviceId").ToString();
							break;
						}
					}
				}
			});
			return systemVolumeDeviceID;
		}

		public static ManagementObject GetEncryptableVolume(string deviceId, out Exception ex)
		{
			ManagementObject encryptableVolume = null;
			ex = Util.HandleExceptions(delegate
			{
				ManagementObjectCollection encryptableVolumes = BitlockerUtil.GetEncryptableVolumes();
				if (encryptableVolumes != null)
				{
					using (encryptableVolumes)
					{
						foreach (ManagementBaseObject managementBaseObject in encryptableVolumes)
						{
							ManagementObject managementObject = (ManagementObject)managementBaseObject;
							using (managementObject)
							{
								string text = managementObject.GetPropertyValue("DeviceId").ToString();
								if (text.Equals(deviceId))
								{
									encryptableVolume = managementObject;
									break;
								}
							}
						}
					}
				}
			});
			return encryptableVolume;
		}

		public static ManagementObject GetWin32Volume(string deviceId, out Exception ex)
		{
			ManagementObject win32Volume = null;
			ex = Util.HandleExceptions(delegate
			{
				SelectQuery query = new SelectQuery(string.Format("SELECT * FROM Win32_Volume WHERE DeviceId=\"{0}\"", deviceId.Replace("\\", "\\\\")));
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query))
				{
					using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							ManagementObject win32Volume = (ManagementObject)enumerator.Current;
							win32Volume = win32Volume;
						}
					}
				}
			});
			return win32Volume;
		}

		public static bool IsVolumeEmpty(string deviceID, out Exception ex)
		{
			bool isEmpty = false;
			ManagementObject win32Volume = BitlockerUtil.GetWin32Volume(deviceID, out ex);
			if (win32Volume != null)
			{
				win32Volume.GetPropertyValue("Name").ToString();
				ex = Util.HandleExceptions(delegate
				{
					long num = Convert.ToInt64(win32Volume.GetPropertyValue("Capacity").ToString()) - Convert.ToInt64(win32Volume.GetPropertyValue("FreeSpace").ToString());
					if (num <= 524288000L)
					{
						isEmpty = true;
					}
				});
			}
			return isEmpty;
		}

		public static bool IsBootVolume(ManagementObject encryptableVolume, out Exception ex)
		{
			ex = null;
			if (encryptableVolume == null)
			{
				return false;
			}
			string deviceID = encryptableVolume.GetPropertyValue("DeviceID").ToString();
			return BitlockerUtil.IsBootVolume(deviceID, out ex);
		}

		public static bool IsBootVolume(string deviceID, out Exception ex)
		{
			ex = null;
			string bootVolumeDeviceID = BitlockerUtil.GetBootVolumeDeviceID(out ex);
			return bootVolumeDeviceID != null && bootVolumeDeviceID.Equals(deviceID);
		}

		public static bool IsSystemVolume(string deviceID, out Exception ex)
		{
			string systemVolumeDeviceID = BitlockerUtil.GetSystemVolumeDeviceID(out ex);
			return systemVolumeDeviceID != null && systemVolumeDeviceID.Equals(deviceID);
		}

		public static string GetDeviceId(ManagementObject encryptableVolume)
		{
			if (encryptableVolume == null)
			{
				return string.Empty;
			}
			return encryptableVolume.GetPropertyValue("DeviceID").ToString();
		}

		public static Dictionary<string, string> GetMountPointVolumeIDMappings(out Exception ex)
		{
			Dictionary<string, string> mountPointVolumeIdMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			ex = null;
			ManagementObjectCollection mountPointToVolumeMappings = WMIUtil.GetManagementObjectCollection("Win32_MountPoint", "\\ROOT\\CIMV2", out ex);
			if (mountPointToVolumeMappings == null)
			{
				return null;
			}
			ex = Util.HandleExceptions(delegate
			{
				using (ManagementObjectCollection mountPointToVolumeMappings = mountPointToVolumeMappings)
				{
					foreach (ManagementBaseObject managementBaseObject in mountPointToVolumeMappings)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						using (managementObject)
						{
							string input = managementObject.GetPropertyValue("Volume").ToString();
							string input2 = managementObject.GetPropertyValue("Directory").ToString();
							string pattern = "\"(.*)\"";
							Match match = Regex.Match(input, pattern);
							Match match2 = Regex.Match(input2, pattern);
							if (match.Success && match2.Success)
							{
								mountPointVolumeIdMappings.Add(Util.RemoveEscapeCharacters(match2.Groups[1].Value), Util.RemoveEscapeCharacters(match.Groups[1].Value));
							}
						}
					}
				}
			});
			return mountPointVolumeIdMappings;
		}

		public static BitlockerUtil.BitlockerConversionState GetBitlockerConversionState(ManagementObject encryptableVolume, out int returnValueConversionState, out Exception ex)
		{
			BitlockerUtil.BitlockerConversionState bitlockerConversionState = BitlockerUtil.BitlockerConversionState.Unknown;
			ManagementBaseObject outParams = null;
			ex = null;
			int returnValue = -1;
			ex = Util.HandleExceptions(delegate
			{
				returnValue = WMIUtil.CallWMIMethod(encryptableVolume, "GetConversionStatus", null, null, out outParams);
				if (returnValue == 0)
				{
					int num = Convert.ToInt32(outParams["ConversionStatus"]);
					if (Enum.IsDefined(typeof(BitlockerUtil.BitlockerConversionState), num))
					{
						bitlockerConversionState = (BitlockerUtil.BitlockerConversionState)num;
					}
				}
			});
			returnValueConversionState = returnValue;
			return bitlockerConversionState;
		}

		public static int GetBitlockerEncryptionPercentage(ManagementObject encryptableVolume, out int returnValueEncryptionPercentage, out Exception ex)
		{
			ManagementBaseObject outParams = null;
			int encryptionPercentageWMIReturnValue = -1;
			int returnValue = -1;
			ex = null;
			ex = Util.HandleExceptions(delegate
			{
				returnValue = WMIUtil.CallWMIMethod(encryptableVolume, "GetConversionStatus", null, null, out outParams);
				if (returnValue == 0)
				{
					int.TryParse(outParams["EncryptionPercentage"].ToString(), out encryptionPercentageWMIReturnValue);
				}
			});
			returnValueEncryptionPercentage = returnValue;
			return encryptionPercentageWMIReturnValue;
		}

		public static bool IsVolumeMountedOnVirtualDisk(string deviceId, out Exception ex)
		{
			bool virtualDisksExist = false;
			ex = Util.HandleExceptions(delegate
			{
				SelectQuery query = new SelectQuery("Select * FROM Win32_DiskDrive WHERE Model like '%Virtual Disk%'");
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query))
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						using (managementObject)
						{
							virtualDisksExist = true;
						}
					}
				}
			});
			return virtualDisksExist;
		}

		public static Exception IsVolumeEncryptedOrEncrypting(string deviceId, out bool isVolumeEncrypting, out bool isVolumeEncrypted)
		{
			isVolumeEncrypting = false;
			isVolumeEncrypted = false;
			Exception ex = null;
			Exception ex2 = null;
			Exception ex3 = null;
			if (!BitlockerUtil.IsSystemVolume(deviceId, out ex) && !BitlockerUtil.IsBootVolume(deviceId, out ex2))
			{
				if (ex != null)
				{
					return ex;
				}
				if (ex2 != null)
				{
					return ex2;
				}
				ManagementObject encryptableVolume = BitlockerUtil.GetEncryptableVolume(deviceId, out ex3);
				if (encryptableVolume == null)
				{
					return ex3;
				}
				int num;
				BitlockerUtil.BitlockerConversionState bitlockerConversionState = BitlockerUtil.GetBitlockerConversionState(encryptableVolume, out num, out ex3);
				if (ex3 != null)
				{
					return ex3;
				}
				if (bitlockerConversionState != BitlockerUtil.BitlockerConversionState.FullyDecrypted && bitlockerConversionState != BitlockerUtil.BitlockerConversionState.FullyEncrypted && bitlockerConversionState != BitlockerUtil.BitlockerConversionState.EncryptionPaused && bitlockerConversionState != BitlockerUtil.BitlockerConversionState.Unknown)
				{
					isVolumeEncrypting = true;
				}
				if (bitlockerConversionState == BitlockerUtil.BitlockerConversionState.FullyEncrypted)
				{
					isVolumeEncrypted = true;
				}
				ExAssert.RetailAssert(!isVolumeEncrypting || !isVolumeEncrypted, DiskManagementStrings.VolumeCannotBeBothEncryptingAndEncrypted(deviceId));
			}
			return ex3;
		}

		public static bool IsEncryptionPausedDueToBadBlocks(string deviceId, out Exception ex, out string mountPoint, out string eventXML)
		{
			mountPoint = null;
			eventXML = null;
			ManagementObject encryptableVolume = BitlockerUtil.GetEncryptableVolume(deviceId, out ex);
			if (encryptableVolume == null)
			{
				return false;
			}
			bool flag = BitlockerUtil.IsVolumeEncryptionPaused(encryptableVolume, out ex);
			if (!flag)
			{
				return flag;
			}
			return BitlockerUtil.DoesVolumeHaveBadBlocks(deviceId, out mountPoint, out ex, out eventXML);
		}

		public static bool DoesVolumeHaveBadBlocks(string volumeId, out string mountPoint, out Exception ex, out string eventXML)
		{
			bool flag = false;
			mountPoint = null;
			eventXML = null;
			Dictionary<string, string> mountPointVolumeIDMappings = BitlockerUtil.GetMountPointVolumeIDMappings(out ex);
			if (mountPointVolumeIDMappings == null)
			{
				ex = new MountPointsFindException(ex.Message, ex);
				return false;
			}
			if (!mountPointVolumeIDMappings.ContainsValue(volumeId))
			{
				ex = new InvalidVolumeIdException(volumeId);
				return false;
			}
			foreach (string text in mountPointVolumeIDMappings.Keys)
			{
				if (mountPointVolumeIDMappings[text].Equals(volumeId))
				{
					flag = BitlockerUtil.DoesMountPointHaveBadBlocks(text, out ex, out eventXML);
					if (flag)
					{
						mountPoint = text;
						break;
					}
				}
			}
			return flag;
		}

		public static bool DoesMountPointHaveBadBlocks(string mountPoint, out Exception ex, out string eventXML)
		{
			bool doesMountPointHaveBadBlocks = false;
			string evtXml = null;
			DateTime utcNow = DateTime.UtcNow;
			string queryString = string.Format("*[System[EventID=24588 or EventID=24593 or EventID=24594 or EventID=24595]][System[Provider[@Name='{0}']]][System[TimeCreated[@SystemTime >= '{2}']]][System[TimeCreated[@SystemTime <= '{3}']]][EventData[Data[@Name='Volume']='{1}']]", new object[]
			{
				"Microsoft-Windows-Bitlocker-Driver",
				mountPoint,
				DateTime.UtcNow.AddDays(-31.0).ToUniversalTime().ToString("o"),
				utcNow.ToUniversalTime().ToString("o")
			});
			ex = Util.HandleExceptions(delegate
			{
				EventLogQuery eventQuery = new EventLogQuery("System", PathType.LogName, queryString);
				using (EventLogReader eventLogReader = new EventLogReader(eventQuery))
				{
					EventRecord eventRecord;
					if ((eventRecord = eventLogReader.ReadEvent()) != null)
					{
						evtXml = eventRecord.ToXml();
						doesMountPointHaveBadBlocks = true;
					}
				}
			});
			eventXML = evtXml;
			return doesMountPointHaveBadBlocks;
		}

		public enum BitlockerConversionState
		{
			FullyDecrypted,
			FullyEncrypted,
			EncryptionInProgress,
			DecryptionInProgress,
			EncryptionPaused,
			DecryptionPaused,
			Unknown = -1
		}
	}
}
