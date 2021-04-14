using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("4bd682dd-7554-40e9-9a9b-82654ede7e62")]
	[ComImport]
	internal interface IPropertyValue
	{
		PropertyType Type { get; }

		bool IsNumericScalar { get; }

		byte GetUInt8();

		short GetInt16();

		ushort GetUInt16();

		int GetInt32();

		uint GetUInt32();

		long GetInt64();

		ulong GetUInt64();

		float GetSingle();

		double GetDouble();

		char GetChar16();

		bool GetBoolean();

		string GetString();

		Guid GetGuid();

		DateTimeOffset GetDateTime();

		TimeSpan GetTimeSpan();

		Point GetPoint();

		Size GetSize();

		Rect GetRect();

		byte[] GetUInt8Array();

		short[] GetInt16Array();

		ushort[] GetUInt16Array();

		int[] GetInt32Array();

		uint[] GetUInt32Array();

		long[] GetInt64Array();

		ulong[] GetUInt64Array();

		float[] GetSingleArray();

		double[] GetDoubleArray();

		char[] GetChar16Array();

		bool[] GetBooleanArray();

		string[] GetStringArray();

		object[] GetInspectableArray();

		Guid[] GetGuidArray();

		DateTimeOffset[] GetDateTimeArray();

		TimeSpan[] GetTimeSpanArray();

		Point[] GetPointArray();

		Size[] GetSizeArray();

		Rect[] GetRectArray();
	}
}
