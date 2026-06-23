// Copyright (c) SeasonEngine and contributors.
// Licensed under the MIT License.
// https://github.com/SeasonRealms/SeasonImage

namespace SeasonImage;

public sealed class StableDiffusionContextOptions
{
    public string? ModelPath { get; set; }
    public string? ClipLPath { get; set; }
    public string? ClipGPath { get; set; }
    public string? ClipVisionPath { get; set; }
    public string? T5XxlPath { get; set; }
    public string? LlmPath { get; set; }
    public string? LlmVisionPath { get; set; }
    public string? DiffusionModelPath { get; set; }
    public string? HighNoiseDiffusionModelPath { get; set; }
    public string? UncondDiffusionModelPath { get; set; }
    public string? VaePath { get; set; }
    public string? TaesdPath { get; set; }
    public string? ControlNetPath { get; set; }
    public string? PhotoMakerPath { get; set; }
    public string? PulidWeightsPath { get; set; }
    public string? TensorTypeRules { get; set; }
    public int ThreadCount { get; set; } = Environment.ProcessorCount;
    public StableDiffusionWeightType WeightType { get; set; } = StableDiffusionWeightType.F16;
    public StableDiffusionRngType RngType { get; set; } = StableDiffusionRngType.Cuda;
    public StableDiffusionRngType? SamplerRngType { get; set; }
    public StableDiffusionPredictionType? Prediction { get; set; }
    public StableDiffusionLoraApplyMode LoraApplyMode { get; set; } = StableDiffusionLoraApplyMode.Auto;
    public bool EnableMmap { get; set; } = true;
    public bool FlashAttention { get; set; }
    public bool DiffusionFlashAttention { get; set; }
    public bool TaePreviewOnly { get; set; }
    public bool StreamLayers { get; set; }
    public string? MaxVram { get; set; }
    public string? Backend { get; set; }
    public string? ParamsBackend { get; set; }
    public string? RpcServers { get; set; }
}

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
    public StableDiffusionInputImage? InitImage { get; set; }
    public StableDiffusionInputImage? MaskImage { get; set; }
    public StableDiffusionInputImage? ControlImage { get; set; }
    public float ControlStrength { get; set; } = 1.0f;
    public IReadOnlyList<StableDiffusionLora>? Loras { get; set; }
}

public sealed class StableDiffusionConvertOptions
{
    public string InputPath { get; set; } = string.Empty;
    public string? VaePath { get; set; }
    public string OutputPath { get; set; } = string.Empty;
    public StableDiffusionWeightType OutputType { get; set; } = StableDiffusionWeightType.F16;
    public string? TensorTypeRules { get; set; }
    public bool ConvertTensorNames { get; set; }
}

public sealed class StableDiffusionUpscalerOptions
{
    public string ModelPath { get; set; } = string.Empty;
    public bool Direct { get; set; }
    public int ThreadCount { get; set; } = Environment.ProcessorCount;
    public int TileSize { get; set; }
    public string? Backend { get; set; }
    public string? ParamsBackend { get; set; }
}

public sealed class StableDiffusionPreviewCallbackOptions
{
    public StableDiffusionPreviewMode Mode { get; set; } = StableDiffusionPreviewMode.Projection;
    public int Interval { get; set; } = 1;
    public bool IncludeDenoised { get; set; } = true;
    public bool IncludeNoisy { get; set; }
}

public sealed class StableDiffusionLora
{
    public string Path { get; set; } = string.Empty;
    public float Multiplier { get; set; } = 1.0f;
    public bool IsHighNoise { get; set; }
}
