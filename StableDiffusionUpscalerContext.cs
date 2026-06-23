// Copyright (c) SeasonEngine and contributors.
// Licensed under the MIT License.
// https://github.com/SeasonRealms/SeasonImage

namespace SeasonImage;

public sealed class StableDiffusionUpscalerContext : IDisposable
{
    private IntPtr _handle;
    private bool _disposed;

    internal StableDiffusionUpscalerContext(StableDiffusionUpscalerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ModelPath);

        using var strings = new Utf8StringArena();

        _handle = NativeMethods.new_upscaler_ctx(
            strings.Add(options.ModelPath),
            options.Direct,
            options.ThreadCount > 0 ? options.ThreadCount : Environment.ProcessorCount,
            options.TileSize,
            strings.Add(options.Backend),
            strings.Add(options.ParamsBackend));

        if (_handle == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to create upscaler context.");
        }
    }

    public int NativeUpscaleFactor
    {
        get
        {
            ThrowIfDisposed();
            return NativeMethods.get_upscale_factor(_handle);
        }
    }

    public StableDiffusionImageResult Upscale(StableDiffusionInputImage inputImage, uint upscaleFactor = 0)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(inputImage);

        using var pinned = new PinnedNativeImage(inputImage);
        var nativeImage = NativeMethods.upscale(_handle, pinned.Native, upscaleFactor);

        try
        {
            return NativeMethods.CopyImage(nativeImage);
        }
        finally
        {
            NativeMethods.ReleaseReturnedImage(nativeImage);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_handle != IntPtr.Zero)
        {
            NativeMethods.free_upscaler_ctx(_handle);
            _handle = IntPtr.Zero;
        }

        GC.SuppressFinalize(this);
    }

    ~StableDiffusionUpscalerContext()
    {
        Dispose();
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
