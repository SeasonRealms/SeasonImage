// Copyright (c) SeasonEngine and contributors.
// Licensed under the MIT License.
// https://github.com/SeasonRealms/SeasonImage

namespace Season.Image;

public static class StableDiffusion
{
    private static NativeMethods.NativeLogCallback? s_logThunk;
    private static Action<StableDiffusionLogLevel, string>? s_logCallback;

    private static NativeMethods.NativeProgressCallback? s_progressThunk;
    private static Action<StableDiffusionProgress>? s_progressCallback;

    private static NativeMethods.NativePreviewCallback? s_previewThunk;
    private static Action<StableDiffusionPreview>? s_previewCallback;

    public static bool IsSupported =>
        OperatingSystem.IsWindows() ||
        (OperatingSystem.IsMacCatalyst() && RuntimeInformation.ProcessArchitecture == Architecture.Arm64);

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

    public static void SetPreviewCallback(
        Action<StableDiffusionPreview>? callback,
        StableDiffusionPreviewCallbackOptions? options = null)
    {
        EnsureSupported();
        s_previewCallback = callback;

        if (callback is null)
        {
            s_previewThunk = null;
            NativeMethods.sd_set_preview_callback(
                null,
                (int)StableDiffusionPreviewMode.None,
                0,
                NativeMethods.ToNativeBool(false),
                NativeMethods.ToNativeBool(false),
                IntPtr.Zero);
            return;
        }

        options ??= new StableDiffusionPreviewCallbackOptions();

        s_previewThunk = static (step, frameCount, frames, isNoisy, _) =>
        {
            var managed = s_previewCallback;
            if (managed is null)
            {
                return;
            }

            var copiedFrames = NativeMethods.CopyImages(frames, frameCount);
            managed(new StableDiffusionPreview(step, isNoisy, copiedFrames));
        };

        NativeMethods.sd_set_preview_callback(
            s_previewThunk,
            (int)options.Mode,
            options.Interval <= 0 ? 1 : options.Interval,
            NativeMethods.ToNativeBool(options.IncludeDenoised),
            NativeMethods.ToNativeBool(options.IncludeNoisy),
            IntPtr.Zero);
    }

    public static StableDiffusionContext CreateContext(StableDiffusionContextOptions options)
    {
        EnsureSupported();
        ArgumentNullException.ThrowIfNull(options);
        return new StableDiffusionContext(options);
    }

    public static StableDiffusionUpscalerContext CreateUpscaler(StableDiffusionUpscalerOptions options)
    {
        EnsureSupported();
        ArgumentNullException.ThrowIfNull(options);
        return new StableDiffusionUpscalerContext(options);
    }

    public static bool Convert(StableDiffusionConvertOptions options)
    {
        EnsureSupported();
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.InputPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.OutputPath);

        using var strings = new Utf8StringArena();

        return NativeMethods.convert(
            strings.Add(options.InputPath),
            strings.Add(options.VaePath),
            strings.Add(options.OutputPath),
            (int)options.OutputType,
            strings.Add(options.TensorTypeRules),
            options.ConvertTensorNames);
    }

    internal static void EnsureSupported()
    {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsMacCatalyst())
        {
            throw new PlatformNotSupportedException(
                "SeasonImage currently ships stable-diffusion native binaries only for Windows and Mac Catalyst (Apple Silicon).");
        }

        // The Mac Catalyst artifact is a pure arm64 slice, so on an Intel Mac - or under
        // Rosetta - the dylib cannot even be opened. Report that reason instead of
        // letting the first P/Invoke surface a bare DllNotFoundException.
        if (OperatingSystem.IsMacCatalyst() && RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
        {
            throw new PlatformNotSupportedException(
                "SeasonImage ships Mac Catalyst stable-diffusion native binaries for Apple Silicon (arm64) only; " +
                $"this process runs as {RuntimeInformation.ProcessArchitecture}.");
        }
    }
}
