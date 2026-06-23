// Copyright (c) SeasonEngine and contributors.
// Licensed under the MIT License.
//https://github.com/SeasonRealms/SeasonImage

namespace SeasonImage;

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
}

