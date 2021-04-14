using System;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class AppContextSwitches
	{
		public static bool NoAsyncCurrentCulture
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchNoAsyncCurrentCulture, ref AppContextSwitches._noAsyncCurrentCulture);
			}
		}

		public static bool EnforceJapaneseEraYearRanges
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchEnforceJapaneseEraYearRanges, ref AppContextSwitches._enforceJapaneseEraYearRanges);
			}
		}

		public static bool FormatJapaneseFirstYearAsANumber
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchFormatJapaneseFirstYearAsANumber, ref AppContextSwitches._formatJapaneseFirstYearAsANumber);
			}
		}

		public static bool EnforceLegacyJapaneseDateParsing
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchEnforceLegacyJapaneseDateParsing, ref AppContextSwitches._enforceLegacyJapaneseDateParsing);
			}
		}

		public static bool ThrowExceptionIfDisposedCancellationTokenSource
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchThrowExceptionIfDisposedCancellationTokenSource, ref AppContextSwitches._throwExceptionIfDisposedCancellationTokenSource);
			}
		}

		public static bool UseConcurrentFormatterTypeCache
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchUseConcurrentFormatterTypeCache, ref AppContextSwitches._useConcurrentFormatterTypeCache);
			}
		}

		public static bool PreserveEventListnerObjectIdentity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchPreserveEventListnerObjectIdentity, ref AppContextSwitches._preserveEventListnerObjectIdentity);
			}
		}

		public static bool UseLegacyPathHandling
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchUseLegacyPathHandling, ref AppContextSwitches._useLegacyPathHandling);
			}
		}

		public static bool BlockLongPaths
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchBlockLongPaths, ref AppContextSwitches._blockLongPaths);
			}
		}

		public static bool SetActorAsReferenceWhenCopyingClaimsIdentity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchSetActorAsReferenceWhenCopyingClaimsIdentity, ref AppContextSwitches._cloneActor);
			}
		}

		public static bool DoNotAddrOfCspParentWindowHandle
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchDoNotAddrOfCspParentWindowHandle, ref AppContextSwitches._doNotAddrOfCspParentWindowHandle);
			}
		}

		public static bool IgnorePortablePDBsInStackTraces
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchIgnorePortablePDBsInStackTraces, ref AppContextSwitches._ignorePortablePDBsInStackTraces);
			}
		}

		public static bool UseNewMaxArraySize
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchUseNewMaxArraySize, ref AppContextSwitches._useNewMaxArraySize);
			}
		}

		public static bool UseLegacyExecutionContextBehaviorUponUndoFailure
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchUseLegacyExecutionContextBehaviorUponUndoFailure, ref AppContextSwitches._useLegacyExecutionContextBehaviorUponUndoFailure);
			}
		}

		public static bool UseLegacyFipsThrow
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchCryptographyUseLegacyFipsThrow, ref AppContextSwitches._useLegacyFipsThrow);
			}
		}

		public static bool DoNotMarshalOutByrefSafeArrayOnInvoke
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchDoNotMarshalOutByrefSafeArrayOnInvoke, ref AppContextSwitches._doNotMarshalOutByrefSafeArrayOnInvoke);
			}
		}

		public static bool UseNetCoreTimer
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue(AppContextDefaultValues.SwitchUseNetCoreTimer, ref AppContextSwitches._useNetCoreTimer);
			}
		}

		private static bool DisableCaching { get; set; }

		static AppContextSwitches()
		{
			bool disableCaching;
			if (AppContext.TryGetSwitch("TestSwitch.LocalAppContext.DisableCaching", out disableCaching))
			{
				AppContextSwitches.DisableCaching = disableCaching;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool GetCachedSwitchValue(string switchName, ref int switchValue)
		{
			return switchValue >= 0 && (switchValue > 0 || AppContextSwitches.GetCachedSwitchValueInternal(switchName, ref switchValue));
		}

		private static bool GetCachedSwitchValueInternal(string switchName, ref int switchValue)
		{
			bool flag;
			AppContext.TryGetSwitch(switchName, out flag);
			if (AppContextSwitches.DisableCaching)
			{
				return flag;
			}
			switchValue = (flag ? 1 : -1);
			return flag;
		}

		private static int _noAsyncCurrentCulture;

		private static int _enforceJapaneseEraYearRanges;

		private static int _formatJapaneseFirstYearAsANumber;

		private static int _enforceLegacyJapaneseDateParsing;

		private static int _throwExceptionIfDisposedCancellationTokenSource;

		private static int _useConcurrentFormatterTypeCache;

		private static int _preserveEventListnerObjectIdentity;

		private static int _useLegacyPathHandling;

		private static int _blockLongPaths;

		private static int _cloneActor;

		private static int _doNotAddrOfCspParentWindowHandle;

		private static int _ignorePortablePDBsInStackTraces;

		private static int _useNewMaxArraySize;

		private static int _useLegacyExecutionContextBehaviorUponUndoFailure;

		private static int _useLegacyFipsThrow;

		private static int _doNotMarshalOutByrefSafeArrayOnInvoke;

		private static int _useNetCoreTimer;
	}
}
