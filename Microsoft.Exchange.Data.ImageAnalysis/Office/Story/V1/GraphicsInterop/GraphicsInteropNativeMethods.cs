using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Office.Story.V1.GraphicsInterop.Misc;

namespace Microsoft.Office.Story.V1.GraphicsInterop
{
	internal static class GraphicsInteropNativeMethods
	{
		[DllImport("ole32.dll")]
		public static extern int PropVariantClear(ref PROPVARIANT pvar);

		public static Guid GetGuidForInterface<T>()
		{
			GuidAttribute guidAttribute = (from GuidAttribute guid in typeof(T).GetTypeInfo().GetCustomAttributes(typeof(GuidAttribute), false)
			select guid).FirstOrDefault<GuidAttribute>();
			if (guidAttribute == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to get GUID for type '{0}' as GuidAttribute is not present on that type.", new object[]
				{
					typeof(T)
				}));
			}
			return new Guid(guidAttribute.Value);
		}

		public static T CreateComInstanceFromInterface<T>()
		{
			return GraphicsInteropNativeMethods.CreateComInstanceFromInterface<T>(GraphicsInteropNativeMethods.GetGuidForInterface<T>());
		}

		public static T CreateComInstanceFromInterface<T>(Guid coClassId)
		{
			T result = default(T);
			Type typeFromCLSID = Marshal.GetTypeFromCLSID(coClassId);
			if (null != typeFromCLSID)
			{
				result = (T)((object)Activator.CreateInstance(typeFromCLSID));
			}
			return result;
		}

		public static void SafeReleaseComObject(object comObject)
		{
			if (comObject != null)
			{
				Marshal.ReleaseComObject(comObject);
			}
		}

		public static bool Succeeded(int hr)
		{
			return hr >= 0;
		}

		public static bool Failed(int hr)
		{
			return hr < 0;
		}

		public static void CheckNativeResult(int hr)
		{
			if (GraphicsInteropNativeMethods.Failed(hr))
			{
				throw Marshal.GetExceptionForHR(hr);
			}
		}

		public static int GetThreadId()
		{
			return Environment.CurrentManagedThreadId;
		}

		private const string KERNEL32DLL = "kernel32.dll";

		private const string MFPLATDLL = "Mfplat.dll";

		private const string DXGIDLL = "Dxgi.dll";

		private const string D2D1DLL = "D2D1.dll";

		private const string DWRITEDLL = "Dwrite.dll";

		private const string D3D11DLL = "d3d11.dll";

		private const string OLE32DLL = "ole32.dll";

		public const int MF_SDK_VERSION = 2;

		public const int MF_API_VERSION = 112;

		public const int MF_VERSION = 131184;

		public const int MFSTARTUP_NOSOCKET = 1;

		public const int D3D11_SDK_VERSION = 7;

		public const int D3D10_SDK_VERSION = 29;

		public const int D3D10_1_SDK_VERSION = 32;

		public const float D3D11_MIN_DEPTH = 0f;

		public const float D3D11_MAX_DEPTH = 1f;

		public const int S_OK = 0;

		public const int E_NOT_SUFFICIENT_BUFFER = -2147024774;

		public const int E_POINTER = -2147467261;

		public const int E_UNEXPECTED = -2147418113;

		public const int E_OUTOFMEMORY = -2147024882;

		public const int E_INVALIDARG = -2147024809;

		public const int E_HANDLE = -2147024890;

		public const int E_ABORT = -2147467260;

		public const int E_FAIL = -2147467259;

		public const int E_ACCESSDENIED = -2147024891;

		public const int E_NOTIMPL = -2147467263;

		public const int WINCODEC_ERR_PROPERTYNOTFOUND = -2003292352;

		public const int DXGI_ERROR_DEVICE_REMOVED = -2005270523;

		public const int MF_E_ATTRIBUTENOTFOUND = -1072875802;

		public const int D2DERR_RECREATE_TARGET = -2003238900;
	}
}
