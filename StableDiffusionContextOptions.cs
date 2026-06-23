// Copyright (c) SeasonEngine and contributors.
// Licensed under the MIT License.
//https://github.com/SeasonRealms/SeasonImage

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
