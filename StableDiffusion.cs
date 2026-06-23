// Copyright (c) SeasonEngine and contributors.
// Licensed under the MIT License.
//https://github.com/SeasonRealms/SeasonImage

namespace SeasonImage;

public static class StableDiffusion
{
    private static NativeMethods.NativeLogCallback? s_logThunk;
    private static Action<StableDiffusionLogLevel, string>? s_logCallback;

    private static NativeMethods.NativeProgressCallback? s_progressThunk;
    private static Action<StableDiffusionProgress>? s_progressCallback;

    public static bool IsSupported => OperatingSystem.IsWindows();

    public static string Version => NativeMethods.PtrToString(NativeMethods.sd_version());

    public static string Commit => NativeMethods.PtrToString(NativeMethods.sd_commit());

    public static int PhysicalCoreCount => NativeMethods.sd_get_num_physical_cores();

    public static string GetSystemInfo()
    {
        EnsureSupported();
        return NativeMethods.PtrToString(NativeMethods.sd_get_system_info());
    }

    public static void SetLogCallback(Action<StableDiffusionLogLevel, string>? callback)
    {
        EnsureSupported();
        s_logCallback = callback;
        if (callback is null)
        {
            s_logThunk = null;
            NativeMethods.sd_set_log_callback(null, IntPtr.Zero);
            return;
        }

        s_logThunk = static (level, text, _) =>
        {
            var managed = s_logCallback;
            if (managed is null)
            {
                return;
            }

            managed((StableDiffusionLogLevel)level, NativeMethods.PtrToString(text));
        };

        NativeMethods.sd_set_log_callback(s_logThunk, IntPtr.Zero);
    }

    public static void SetProgressCallback(Action<StableDiffusionProgress>? callback)
    {
        EnsureSupported();
        s_progressCallback = callback;
        if (callback is null)
        {
            s_progressThunk = null;
            NativeMethods.sd_set_progress_callback(null, IntPtr.Zero);
            return;
        }

        s_progressThunk = static (step, steps, time, _) =>
        {
            var managed = s_progressCallback;
            if (managed is null)
            {
                return;
            }

            managed(new StableDiffusionProgress(step, steps, time));
        };

        NativeMethods.sd_set_progress_callback(s_progressThunk, IntPtr.Zero);
    }

    public static StableDiffusionContext CreateContext(StableDiffusionContextOptions options)
    {
        EnsureSupported();
        ArgumentNullException.ThrowIfNull(options);
        return new StableDiffusionContext(options);
    }

    internal static void EnsureSupported()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("SeasonImage currently ships stable-diffusion native binaries only for Windows.");
        }
    }
}

