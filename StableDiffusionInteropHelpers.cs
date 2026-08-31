// Copyright (c) SeasonEngine and contributors.
// Licensed under the MIT License.
// https://github.com/SeasonRealms/SeasonImage

namespace Season.Image;

internal sealed class Utf8StringArena : IDisposable
{
    private readonly List<IntPtr> _buffers = [];

    public IntPtr Add(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return IntPtr.Zero;
        }

        var ptr = Marshal.StringToCoTaskMemUTF8(value);
        _buffers.Add(ptr);
        return ptr;
    }

    public void Dispose()
    {
        foreach (var ptr in _buffers)
        {
            if (ptr != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        _buffers.Clear();
    }
}

internal sealed class PinnedNativeImage : IDisposable
{
    private GCHandle _handle;
    private bool _disposed;

    public PinnedNativeImage(StableDiffusionInputImage image)
    {
        ArgumentNullException.ThrowIfNull(image);
        _handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
        Width = image.Width;
        Height = image.Height;
        Native = new NativeMethods.NativeSdImage
        {
            width = (uint)image.Width,
            height = (uint)image.Height,
            channel = (uint)image.Channels,
            data = _handle.AddrOfPinnedObject()
        };
    }

    public int Width { get; }
    public int Height { get; }
    public NativeMethods.NativeSdImage Native { get; }

    public static PinnedNativeImage? Create(StableDiffusionInputImage? image)
    {
        return image is null ? null : new PinnedNativeImage(image);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (_handle.IsAllocated)
        {
            _handle.Free();
        }
    }
}

internal sealed class NativeSdLoraArray : IDisposable
{
    private readonly NativeMethods.NativeSdLora[] _native;
    private GCHandle _handle;
    private bool _disposed;

    private NativeSdLoraArray(NativeMethods.NativeSdLora[] native)
    {
        _native = native;
        _handle = GCHandle.Alloc(_native, GCHandleType.Pinned);
    }

    public int Count => _native.Length;
    public IntPtr Pointer => _handle.AddrOfPinnedObject();

    public static NativeSdLoraArray? Create(IReadOnlyList<StableDiffusionLora>? loras, Utf8StringArena strings)
    {
        if (loras is null || loras.Count == 0)
        {
            return null;
        }

        var native = new NativeMethods.NativeSdLora[loras.Count];
        for (var i = 0; i < loras.Count; i++)
        {
            var lora = loras[i] ?? throw new ArgumentException("LoRA items cannot contain null values.", nameof(loras));
            if (string.IsNullOrWhiteSpace(lora.Path))
            {
                throw new ArgumentException("LoRA path cannot be empty.", nameof(loras));
            }

            native[i] = new NativeMethods.NativeSdLora
            {
                is_high_noise = NativeMethods.ToNativeBool(lora.IsHighNoise),
                multiplier = lora.Multiplier,
                path = strings.Add(lora.Path)
            };
        }

        return new NativeSdLoraArray(native);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (_handle.IsAllocated)
        {
            _handle.Free();
        }
    }
}

internal sealed class NativeSdImageArray : IDisposable
{
    private readonly PinnedNativeImage[] _pinnedImages;
    private readonly NativeMethods.NativeSdImage[] _nativeImages;
    private GCHandle _handle;
    private bool _disposed;

    private NativeSdImageArray(PinnedNativeImage[] pinnedImages, NativeMethods.NativeSdImage[] nativeImages)
    {
        _pinnedImages = pinnedImages;
        _nativeImages = nativeImages;
        _handle = GCHandle.Alloc(_nativeImages, GCHandleType.Pinned);
    }

    public int Count => _nativeImages.Length;
    public IntPtr Pointer => _handle.AddrOfPinnedObject();
    public int? FirstWidth => _pinnedImages.Length > 0 ? _pinnedImages[0].Width : null;
    public int? FirstHeight => _pinnedImages.Length > 0 ? _pinnedImages[0].Height : null;

    public static NativeSdImageArray? Create(IReadOnlyList<StableDiffusionInputImage>? images)
    {
        if (images is null || images.Count == 0)
        {
            return null;
        }

        var pinnedImages = new PinnedNativeImage[images.Count];
        var nativeImages = new NativeMethods.NativeSdImage[images.Count];

        try
        {
            for (var i = 0; i < images.Count; i++)
            {
                var image = images[i] ?? throw new ArgumentException("Image items cannot contain null values.", nameof(images));
                pinnedImages[i] = new PinnedNativeImage(image);
                nativeImages[i] = pinnedImages[i].Native;
            }

            return new NativeSdImageArray(pinnedImages, nativeImages);
        }
        catch
        {
            foreach (var pinned in pinnedImages)
            {
                pinned?.Dispose();
            }

            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_handle.IsAllocated)
        {
            _handle.Free();
        }

        foreach (var pinned in _pinnedImages)
        {
            pinned.Dispose();
        }
    }
}
