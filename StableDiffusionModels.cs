// Copyright (c) SeasonEngine and contributors.
// Licensed under the MIT License.
// https://github.com/SeasonRealms/SeasonImage

namespace SeasonImage;

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

public readonly record struct StableDiffusionProgress(int Step, int TotalSteps, float ElapsedSeconds)
{
    public float Progress => TotalSteps <= 0 ? 0f : (float)Step / TotalSteps;
}
