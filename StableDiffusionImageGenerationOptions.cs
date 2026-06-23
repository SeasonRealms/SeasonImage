// Copyright (c) SeasonEngine and contributors.
// Licensed under the MIT License.
//https://github.com/SeasonRealms/SeasonImage

namespace SeasonImage;

public sealed class StableDiffusionImageGenerationOptions
{
    public string Prompt { get; set; } = string.Empty;

    public string? NegativePrompt { get; set; }

    public int Width { get; set; } = 1024;

    public int Height { get; set; } = 1024;

    public int SampleSteps { get; set; } = 20;

    public float GuidanceScale { get; set; } = 7.0f;

    public float ImageGuidanceScale { get; set; } = 1.0f;

    public float DistilledGuidanceScale { get; set; } = 3.5f;

    public float Strength { get; set; } = 1.0f;

    public long Seed { get; set; } = -1;

    public int BatchCount { get; set; } = 1;

    public int? ClipSkip { get; set; }

    public float Eta { get; set; }

    public StableDiffusionSampleMethod? SampleMethod { get; set; }

    public StableDiffusionScheduler? Scheduler { get; set; }

    public string? ExtraSampleArguments { get; set; }
}
