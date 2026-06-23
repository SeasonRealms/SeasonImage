// Copyright (c) SeasonEngine and contributors.
// Licensed under the MIT License.
// https://github.com/SeasonRealms/SeasonImage

namespace SeasonImage;

public sealed class StableDiffusionContext : IDisposable
{
    private IntPtr _handle;
    private bool _disposed;

    internal StableDiffusionContext(StableDiffusionContextOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.ModelPath) &&
            string.IsNullOrWhiteSpace(options.DiffusionModelPath))
        {
            throw new ArgumentException(
                "Either ModelPath or DiffusionModelPath must be provided.",
                nameof(options));
        }

        using var strings = new Utf8StringArena();

        var native = new NativeMethods.NativeSdCtxParams();
        NativeMethods.sd_ctx_params_init(ref native);

        native.model_path = strings.Add(options.ModelPath);
        native.clip_l_path = strings.Add(options.ClipLPath);
        native.clip_g_path = strings.Add(options.ClipGPath);
        native.clip_vision_path = strings.Add(options.ClipVisionPath);
        native.t5xxl_path = strings.Add(options.T5XxlPath);
        native.llm_path = strings.Add(options.LlmPath);
        native.llm_vision_path = strings.Add(options.LlmVisionPath);
        native.diffusion_model_path = strings.Add(options.DiffusionModelPath);
        native.high_noise_diffusion_model_path = strings.Add(options.HighNoiseDiffusionModelPath);
        native.uncond_diffusion_model_path = strings.Add(options.UncondDiffusionModelPath);
        native.vae_path = strings.Add(options.VaePath);
        native.taesd_path = strings.Add(options.TaesdPath);
        native.control_net_path = strings.Add(options.ControlNetPath);
        native.photo_maker_path = strings.Add(options.PhotoMakerPath);
        native.pulid_weights_path = strings.Add(options.PulidWeightsPath);
        native.tensor_type_rules = strings.Add(options.TensorTypeRules);
        native.max_vram = strings.Add(options.MaxVram);
        native.backend = strings.Add(options.Backend);
        native.params_backend = strings.Add(options.ParamsBackend);
        native.rpc_servers = strings.Add(options.RpcServers);

        native.n_threads = options.ThreadCount > 0 ? options.ThreadCount : Environment.ProcessorCount;
        native.wtype = (int)options.WeightType;
        native.rng_type = (int)options.RngType;
        if (options.SamplerRngType is not null)
        {
            native.sampler_rng_type = (int)options.SamplerRngType.Value;
        }

        if (options.Prediction is not null)
        {
            native.prediction = (int)options.Prediction.Value;
        }

        native.lora_apply_mode = (int)options.LoraApplyMode;
        native.enable_mmap = NativeMethods.ToNativeBool(options.EnableMmap);
        native.flash_attn = NativeMethods.ToNativeBool(options.FlashAttention);
        native.diffusion_flash_attn = NativeMethods.ToNativeBool(options.DiffusionFlashAttention);
        native.tae_preview_only = NativeMethods.ToNativeBool(options.TaePreviewOnly);
        native.stream_layers = NativeMethods.ToNativeBool(options.StreamLayers);

        _handle = NativeMethods.new_sd_ctx(ref native);
        if (_handle == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                "Failed to create stable diffusion context. Check model paths and native runtime dependencies.");
        }
    }

    public bool SupportsImageGeneration
    {
        get
        {
            ThrowIfDisposed();
            return NativeMethods.sd_ctx_supports_image_generation(_handle);
        }
    }

    public bool SupportsVideoGeneration
    {
        get
        {
            ThrowIfDisposed();
            return NativeMethods.sd_ctx_supports_video_generation(_handle);
        }
    }

    public IReadOnlyList<StableDiffusionImageResult> GenerateImages(StableDiffusionImageGenerationOptions options)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Prompt);

        using var strings = new Utf8StringArena();
        using var initImage = PinnedNativeImage.Create(options.InitImage);
        using var maskImage = PinnedNativeImage.Create(options.MaskImage);
        using var controlImage = PinnedNativeImage.Create(options.ControlImage);
        using var loras = NativeSdLoraArray.Create(options.Loras, strings);

        var native = new NativeMethods.NativeSdImgGenParams();
        NativeMethods.sd_img_gen_params_init(ref native);
        NativeMethods.sd_sample_params_init(ref native.sample_params);

        native.prompt = strings.Add(options.Prompt);
        native.negative_prompt = strings.Add(options.NegativePrompt);
        native.clip_skip = options.ClipSkip ?? native.clip_skip;
        native.width = ResolveDimension(options.Width, initImage?.Width);
        native.height = ResolveDimension(options.Height, initImage?.Height);
        native.strength = options.Strength;
        native.seed = options.Seed;
        native.batch_count = options.BatchCount <= 0 ? 1 : options.BatchCount;
        native.control_strength = options.ControlStrength;
        native.init_image = initImage?.Native ?? default;
        native.mask_image = maskImage?.Native ?? default;
        native.control_image = controlImage?.Native ?? default;

        if (loras is not null)
        {
            native.loras = loras.Pointer;
            native.lora_count = (uint)loras.Count;
        }

        var sampleMethod = options.SampleMethod ??
                           (StableDiffusionSampleMethod)NativeMethods.sd_get_default_sample_method(_handle);
        var scheduler = options.Scheduler ??
                        (StableDiffusionScheduler)NativeMethods.sd_get_default_scheduler(_handle, (int)sampleMethod);

        native.sample_params.sample_method = (int)sampleMethod;
        native.sample_params.scheduler = (int)scheduler;
        native.sample_params.sample_steps = options.SampleSteps;
        native.sample_params.eta = options.Eta;
        native.sample_params.guidance.txt_cfg = options.GuidanceScale;
        native.sample_params.guidance.img_cfg = options.ImageGuidanceScale;
        native.sample_params.guidance.distilled_guidance = options.DistilledGuidanceScale;
        native.sample_params.extra_sample_args = strings.Add(options.ExtraSampleArguments);

        IntPtr imagesPtr = NativeMethods.generate_image(_handle, ref native);
        if (imagesPtr == IntPtr.Zero)
        {
            throw new InvalidOperationException("Native image generation failed.");
        }

        try
        {
            return NativeMethods.CopyImages(imagesPtr, native.batch_count);
        }
        finally
        {
            NativeMethods.free_sd_images(imagesPtr, native.batch_count);
        }
    }

    public void Cancel(StableDiffusionCancelMode mode = StableDiffusionCancelMode.All)
    {
        ThrowIfDisposed();
        NativeMethods.sd_cancel_generation(_handle, (int)mode);
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
            NativeMethods.free_sd_ctx(_handle);
            _handle = IntPtr.Zero;
        }

        GC.SuppressFinalize(this);
    }

    ~StableDiffusionContext()
    {
        Dispose();
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private static int ResolveDimension(int configuredValue, int? fallback)
    {
        if (configuredValue > 0)
        {
            return configuredValue;
        }

        if (fallback is > 0)
        {
            return fallback.Value;
        }

        throw new ArgumentOutOfRangeException(
            nameof(configuredValue),
            "Width and Height must be positive, or an init image must provide them.");
    }
}
