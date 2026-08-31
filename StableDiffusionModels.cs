// Copyright (c) SeasonEngine and contributors.
// Licensed under the MIT License.
// https://github.com/SeasonRealms/SeasonImage

namespace Season.Image;

public sealed class StableDiffusionInputImage
{
    public StableDiffusionInputImage(int width, int height, int channels, byte[] data)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        if (channels <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(channels));
        }

        ArgumentNullException.ThrowIfNull(data);

        checked
        {
            if (data.Length != width * height * channels)
            {
                throw new ArgumentException(
                    "Image byte count does not match width * height * channels.",
                    nameof(data));
            }
        }

        Width = width;
        Height = height;
        Channels = channels;
        Data = data;
    }

    public int Width { get; }
    public int Height { get; }
    public int Channels { get; }
    public byte[] Data { get; }

    public static StableDiffusionInputImage FromRgba(int width, int height, byte[] rgba)
    {
        return new StableDiffusionInputImage(width, height, 4, rgba);
    }

    public static StableDiffusionInputImage FromRgb(int width, int height, byte[] rgb)
    {
        return new StableDiffusionInputImage(width, height, 3, rgb);
    }

    public static StableDiffusionInputImage FromRgbaDiscardAlpha(int width, int height, byte[] rgba)
    {
        ArgumentNullException.ThrowIfNull(rgba);

        checked
        {
            if (rgba.Length != width * height * 4)
            {
                throw new ArgumentException(
                    "RGBA byte count does not match width * height * 4.",
                    nameof(rgba));
            }
        }

        var rgb = new byte[width * height * 3];
        for (int src = 0, dst = 0; src < rgba.Length; src += 4)
        {
            rgb[dst++] = rgba[src];
            rgb[dst++] = rgba[src + 1];
            rgb[dst++] = rgba[src + 2];
        }

        return new StableDiffusionInputImage(width, height, 3, rgb);
    }
}

public sealed class StableDiffusionImageResult
{
    public StableDiffusionImageResult(int width, int height, int channels, byte[] data)
    {
        Width = width;
        Height = height;
        Channels = channels;
        Data = data;
    }

    public int Width { get; }
    public int Height { get; }
    public int Channels { get; }
    public byte[] Data { get; }

    public StableDiffusionInputImage ToInputImage()
    {
        return new StableDiffusionInputImage(Width, Height, Channels, Data);
    }
}

public sealed class StableDiffusionPreview
{
    public StableDiffusionPreview(int step, bool isNoisy, IReadOnlyList<StableDiffusionImageResult> frames)
    {
        Step = step;
        IsNoisy = isNoisy;
        Frames = frames;
    }

    public int Step { get; }
    public bool IsNoisy { get; }
    public IReadOnlyList<StableDiffusionImageResult> Frames { get; }
}

public sealed class StableDiffusionVideoResult
{
    public StableDiffusionVideoResult(IReadOnlyList<StableDiffusionImageResult> frames, int fps)
    {
        ArgumentNullException.ThrowIfNull(frames);
        Frames = frames;
        Fps = fps;
    }

    public IReadOnlyList<StableDiffusionImageResult> Frames { get; }
    public int Fps { get; }
    public int FrameCount => Frames.Count;
}

public readonly record struct StableDiffusionProgress(int Step, int TotalSteps, float ElapsedSeconds)
{
    public float Progress => TotalSteps <= 0 ? 0f : (float)Step / TotalSteps;
}
