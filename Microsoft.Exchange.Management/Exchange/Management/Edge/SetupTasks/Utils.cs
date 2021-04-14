using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	internal sealed class Utils
	{
		public static bool IsPortValid(int port)
		{
			return port >= 0 && port <= 65535;
		}

		public static bool IsPortAvailable(int port)
		{
			Socket socket = null;
			try
			{
				socket = Utils.GetSocketBoundToPort(port);
			}
			catch (SocketException ex)
			{
				if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
				{
					Utils.RunAndLogProcessOutput("netstat", "-abn -p TCP");
					return false;
				}
				throw;
			}
			finally
			{
				if (socket != null)
				{
					socket.Close();
				}
			}
			return true;
		}

		public static int GetAvailablePort(object takenPortObj)
		{
			Socket socket = null;
			Socket socket2 = null;
			int port2;
			try
			{
				if (takenPortObj != null)
				{
					int port = (int)takenPortObj;
					socket = Utils.GetSocketBoundToPort(port);
				}
				socket2 = Utils.GetSocketBoundToPort(0);
				port2 = ((IPEndPoint)socket2.LocalEndPoint).Port;
			}
			finally
			{
				if (socket != null)
				{
					socket.Close();
				}
				if (socket2 != null)
				{
					socket2.Close();
				}
			}
			return port2;
		}

		public static Socket GetSocketBoundToPort(int port)
		{
			Socket socket = null;
			Socket socket2 = null;
			try
			{
				if (Socket.OSSupportsIPv4)
				{
					IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.ExclusiveAddressUse = true;
					socket.Bind(localEP);
				}
				if (Socket.OSSupportsIPv6)
				{
					int port2 = port;
					if (socket != null)
					{
						port2 = ((IPEndPoint)socket.LocalEndPoint).Port;
					}
					IPEndPoint localEP2 = new IPEndPoint(IPAddress.IPv6Any, port2);
					socket2 = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
					socket2.ExclusiveAddressUse = true;
					socket2.Bind(localEP2);
				}
			}
			catch (Exception)
			{
				if (socket != null)
				{
					socket.Close();
					socket = null;
				}
				if (socket2 != null)
				{
					socket2.Close();
					socket2 = null;
				}
				throw;
			}
			if (socket != null && socket2 != null)
			{
				socket2.Close();
				socket2 = null;
			}
			if (socket == null)
			{
				return socket2;
			}
			return socket;
		}

		public static void ValidateDirectory(string path, string propertyName)
		{
			string fullPath = Path.GetFullPath(path);
			if (Directory.Exists(fullPath))
			{
				return;
			}
			int num = Utils.FindNextSeparator(fullPath, 0);
			if (-1 == num)
			{
				throw new InvalidDriveInPathException(fullPath);
			}
			string text = fullPath.Substring(0, num + 1);
			if (!Directory.Exists(text))
			{
				throw new InvalidDriveInPathException(fullPath);
			}
			if (text.Length == fullPath.Length)
			{
				return;
			}
			string path2;
			do
			{
				num = Utils.FindNextSeparator(fullPath, num + 1);
				path2 = ((num == -1) ? fullPath : fullPath.Substring(0, num));
			}
			while (-1 != num && Directory.Exists(path2));
			Utils.CreateDirectory(fullPath, propertyName);
			Directory.Delete(path2, true);
		}

		public static void CreateDirectory(string path, string propertyName)
		{
			DirectorySecurity directorySecurity = new DirectorySecurity();
			for (int i = 0; i < Utils.DirectoryAccessRules.Length; i++)
			{
				directorySecurity.AddAccessRule(Utils.DirectoryAccessRules[i]);
			}
			using (WindowsIdentity current = WindowsIdentity.GetCurrent())
			{
				directorySecurity.SetOwner(current.User);
			}
			directorySecurity.SetAccessRuleProtection(true, false);
			try
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path, directorySecurity);
				}
			}
			catch (UnauthorizedAccessException)
			{
				throw new NoPermissionsForPathException(path);
			}
			catch (ArgumentException)
			{
				throw new InvalidCharsInPathException(path);
			}
			catch (NotSupportedException)
			{
				throw new InvalidCharsInPathException(path);
			}
			catch (PathTooLongException)
			{
				throw new PathIsTooLongException(path);
			}
			catch (DirectoryNotFoundException)
			{
				throw new InvalidDriveInPathException(path);
			}
			catch (IOException)
			{
				throw new ReadOnlyPathException(path);
			}
		}

		public static int FindNextSeparator(string s, int pos)
		{
			return s.IndexOfAny(Utils.PathSeparators, pos);
		}

		public static void DeleteRegSubKeyTreeIfExist(RegistryKey key, string regKeyPath)
		{
			try
			{
				key.DeleteSubKeyTree(regKeyPath);
			}
			catch (ArgumentException)
			{
			}
		}

		public static string MakeIniFileSetting(string key, string value)
		{
			StringBuilder stringBuilder = new StringBuilder(key);
			stringBuilder.Append('=');
			stringBuilder.Append(value);
			return stringBuilder.ToString();
		}

		public static int LogRunProcess(string fileName, string arguments, string path)
		{
			string processName = path + " " + fileName;
			TaskLogger.Log(Strings.LogRunningCommand(processName, arguments));
			int num = Utils.RunProcess(fileName, arguments, path);
			TaskLogger.Log(Strings.LogProcessExitCode(fileName, num));
			return num;
		}

		public static int RunProcess(string fileName, string arguments, string path)
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = fileName;
			processStartInfo.Arguments = arguments;
			processStartInfo.CreateNoWindow = true;
			processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			processStartInfo.UseShellExecute = true;
			if (path != null)
			{
				processStartInfo.WorkingDirectory = path;
			}
			int exitCode;
			using (Process process = Process.Start(processStartInfo))
			{
				process.WaitForExit();
				exitCode = process.ExitCode;
			}
			return exitCode;
		}

		public static void RunAndLogProcessOutput(string fileName, string arguments)
		{
			using (Process process = Process.Start(new ProcessStartInfo
			{
				FileName = fileName,
				Arguments = arguments,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				UseShellExecute = false,
				RedirectStandardOutput = true
			}))
			{
				StreamReader standardOutput = process.StandardOutput;
				string processOutput;
				while ((processOutput = standardOutput.ReadLine()) != null)
				{
					TaskLogger.Log(Strings.OccupiedPortsInformation(processOutput));
				}
				standardOutput.Close();
			}
		}

		public static string GetTempDir()
		{
			return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
		}

		public static string GetSetupLogDir()
		{
			return ConfigurationContext.Setup.SetupLoggingPath;
		}

		public static void DeleteFileIfExist(string filePath)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}

		public static string GetWindowsDir()
		{
			return Path.GetDirectoryName(Environment.SystemDirectory);
		}

		public static bool GetServiceExists(string serviceName)
		{
			bool result = false;
			using (ServiceController serviceController = new ServiceController(serviceName))
			{
				try
				{
					ServiceControllerStatus status = serviceController.Status;
					result = true;
				}
				catch (InvalidOperationException ex)
				{
					Win32Exception ex2 = ex.InnerException as Win32Exception;
					if (ex2 == null || 1060 != ex2.NativeErrorCode)
					{
						throw;
					}
					result = false;
				}
			}
			return result;
		}

		internal const string ServicesSubkeyPath = "System\\CurrentControlSet\\Services";

		internal const string ServiceStartModeValueName = "Start";

		private static readonly char[] PathSeparators = new char[]
		{
			Path.DirectorySeparatorChar,
			Path.AltDirectorySeparatorChar
		};

		private static readonly FileSystemAccessRule[] DirectoryAccessRules = new FileSystemAccessRule[]
		{
			new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow),
			new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow),
			new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow)
		};
	}
}
