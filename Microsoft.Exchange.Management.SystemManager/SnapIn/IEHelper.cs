using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal class IEHelper
	{
		public IEHelper()
		{
			Application.ApplicationExit += delegate(object param0, EventArgs param1)
			{
				if (this.IsIEOpened)
				{
					try
					{
						this.ie.Quit();
					}
					catch (COMException)
					{
					}
					catch (InvalidComObjectException)
					{
					}
					catch (TargetException)
					{
					}
				}
			};
		}

		public void NavigateInSingleIE(string url, IUIService uiService)
		{
			if (!this.IsIEOpened)
			{
				try
				{
					this.ie = (IEHelper.IWebBrowser2)new IEHelper.InternetExplorerClass();
					this.ie.Visible = true;
					this.currentIEHandle = new IntPtr(this.ie.HWND);
				}
				catch (COMException)
				{
					this.StartIEByProcess(url, uiService);
				}
				catch (InvalidComObjectException)
				{
					this.StartIEByProcess(url, uiService);
				}
				catch (TargetException)
				{
					this.StartIEByProcess(url, uiService);
				}
			}
			this.NavigateIE(url);
			this.BringIEToFront();
		}

		private void StartIEByProcess(string url, IUIService uiService)
		{
			bool flag = false;
			try
			{
				this.helpProcess = new Process
				{
					StartInfo = new ProcessStartInfo("iexplore.exe", this.IsIE8OrNewer() ? " -new -nomerge " : " -new ")
				};
				if (this.helpProcess.Start())
				{
					this.helpProcess.WaitForInputIdle(2000);
					flag = true;
				}
			}
			catch (InvalidOperationException)
			{
			}
			catch (Win32Exception)
			{
			}
			if (!flag)
			{
				try
				{
					WinformsHelper.OpenUrl(new Uri(url));
				}
				catch (UrlHandlerNotFoundException ex)
				{
					uiService.ShowError(ex.Message);
				}
			}
		}

		private bool IsIE8OrNewer()
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\Version Vector"))
				{
					string text = (string)registryKey.GetValue("IE");
					if (!string.IsNullOrEmpty(text))
					{
						int num = 0;
						int num2 = 1;
						int num3 = text.IndexOf('.');
						if (num3 > 0)
						{
							if (int.TryParse(text.Substring(0, num3), out num))
							{
								num2 = num;
							}
						}
						else if (int.TryParse(text, out num))
						{
							num2 = num;
						}
						return num2 >= 8;
					}
				}
			}
			catch (SecurityException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
			catch (IOException)
			{
			}
			return false;
		}

		private bool NavigateIE(string url)
		{
			bool result = false;
			if (this.ie != null)
			{
				object obj = null;
				try
				{
					this.ie.Navigate(url, ref obj, ref obj, ref obj, ref obj);
					result = true;
				}
				catch (COMException)
				{
				}
				catch (InvalidComObjectException)
				{
				}
				catch (TargetException)
				{
				}
			}
			return result;
		}

		private void BringIEToFront()
		{
			if (this.currentIEHandle != IntPtr.Zero)
			{
				if (IEHelper.IsIconic(this.currentIEHandle))
				{
					IEHelper.ShowWindow(this.currentIEHandle, 9);
					return;
				}
				IEHelper.LockSetForegroundWindow(2U);
				IEHelper.BringWindowToTop(this.currentIEHandle);
				IEHelper.SetForegroundWindow(this.currentIEHandle);
			}
		}

		private bool IsIEOpened
		{
			get
			{
				bool flag = false;
				try
				{
					flag = (this.ie != null && this.ie.Visible);
				}
				catch (COMException)
				{
				}
				catch (InvalidComObjectException)
				{
				}
				catch (TargetException)
				{
				}
				return flag || this.FindIEByHandle();
			}
		}

		private bool FindIEByHandle()
		{
			bool result = false;
			if (this.helpProcess != null && !this.helpProcess.HasExited)
			{
				this.helpProcess.Refresh();
				this.currentIEHandle = this.helpProcess.MainWindowHandle;
			}
			if (IEHelper.IsWindow(this.currentIEHandle))
			{
				try
				{
					IEnumerable enumerable = (IEnumerable)new IEHelper.ShellWindowsClass();
					foreach (object obj in enumerable)
					{
						IEHelper.IWebBrowser2 webBrowser = obj as IEHelper.IWebBrowser2;
						if (webBrowser != null && this.currentIEHandle == (IntPtr)webBrowser.HWND)
						{
							result = true;
							this.ie = webBrowser;
							break;
						}
					}
				}
				catch (COMException)
				{
				}
				catch (InvalidComObjectException)
				{
				}
				catch (TargetException)
				{
				}
				catch (FileNotFoundException)
				{
				}
			}
			return result;
		}

		[DllImport("User32.Dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool IsIconic(IntPtr hWnd);

		[DllImport("User32.Dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern bool BringWindowToTop(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern bool IsWindow(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern bool LockSetForegroundWindow(uint uLockCode);

		private const int LSFW_UNLOCK = 2;

		private Process helpProcess;

		private IEHelper.IWebBrowser2 ie;

		private IntPtr currentIEHandle;

		[Guid("0002DF01-0000-0000-C000-000000000046")]
		[ComImport]
		public class InternetExplorerClass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern InternetExplorerClass();
		}

		[Guid("9BA05972-F6A8-11CF-A442-00A0C90A8F39")]
		[ComImport]
		public class ShellWindowsClass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern ShellWindowsClass();
		}

		[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
		[SuppressUnmanagedCodeSecurity]
		[Guid("D30C1661-CDAF-11D0-8A3E-00C04FC9E26E")]
		[ComImport]
		public interface IWebBrowser2
		{
			[DispId(104)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			void Navigate([MarshalAs(UnmanagedType.BStr)] [In] string URL, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] ref object Flags, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] ref object TargetFrameName, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] ref object PostData, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] ref object Headers);

			[DispId(300)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			void Quit();

			[DispId(402)]
			bool Visible { [DispId(402)] [MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(402)] [MethodImpl(MethodImplOptions.InternalCall)] [param: In] set; }

			[DispId(-515)]
			int HWND { [DispId(-515)] [MethodImpl(MethodImplOptions.InternalCall)] get; }
		}
	}
}
