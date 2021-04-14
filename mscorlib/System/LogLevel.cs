﻿using System;

namespace System
{
	[Serializable]
	internal enum LogLevel
	{
		Trace,
		Status = 20,
		Warning = 40,
		Error = 50,
		Panic = 100
	}
}
