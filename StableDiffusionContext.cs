// Copyright (c) SeasonEngine and contributors.
// Licensed under the MIT License.
//https://github.com/SeasonRealms/SeasonImage

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
            throw new ArgumentException("Either ModelPath or DiffusionModelPath must be provided.", nameof(options));
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
        native.tensor_type_rules = strings.Add(options.TensorTypeRules);
        native.photo_maker_path = strings.Add(options.PhotoMakerPath);
        native.pulid_weights_path = strings.Add(options.PulidWeightsPath);
        native.max_vram = strings.Add(options.MaxVram);
        native.backend = strings.Add(options.Backend);
        native.params_backend = strings.Add(options.ParamsBackend);
        native.rpc_servers = strings.Add(options.RpcServers);

        native.n_threads = options.ThreadCount > 0 ? options.ThreadCount : Environment.ProcessorCount;
        native.wtype = (int)options.WeightType;
        native.rng_type = (int)options.RngType;
        native.sampler_rng_type = options.SamplerRngType is null
            ? native.sampler_rng_type
            : (int)options.SamplerRngType.Value;
        native.prediction = options.Prediction is null ? native.prediction : (int)options.Prediction.Value;
        native.lora_apply_mode = (int)options.LoraApplyMode;
        native.enable_mmap = NativeMethods.ToNativeBool(options.EnableMmap);
        native.flash_attn = NativeMethods.ToNativeBool(options.FlashAttention);
        native.diffusion_flash_attn = NativeMethods.ToNativeBool(options.DiffusionFlashAttention);
        native.tae_preview_only = NativeMethods.ToNativeBool(options.TaePreviewOnly);
        native.stream_layers = NativeMethods.ToNativeBool(options.StreamLayers);

        _handle = NativeMethods.new_sd_ctx(ref native);
        if (_handle == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to create stable diffusion context. Check model paths and native backend dependencies.");
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

        if (string.IsNullOrWhiteSpace(options.Prompt))
        {
            throw new ArgumentException("Prompt is required.", nameof(options));
        }

        if (options.Width <= 0 || options.Height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Width and Height must be positive.");
        }

        var batchCount = options.BatchCount <= 0 ? 1 : options.BatchCount;

        using var strings = new Utf8StringArena();

        var native = new NativeMethods.NativeSdImgGenParams();
        NativeMethods.sd_img_gen_params_init(ref native);

        native.prompt = strings.Add(options.Prompt);
        native.negative_prompt = strings.Add(options.NegativePrompt);
        native.width = options.Width;
        native.height = options.Height;
        native.batch_count = batchCount;
        native.seed = options.Seed;
        native.strength = options.Strength;

        if (options.ClipSkip is not null)
        {
            native.clip_skip = options.ClipSkip.Value;
        }

        NativeMethods.sd_sample_params_init(ref native.sample_params);
        var sampleMethod = options.SampleMethod ?? (StableDiffusionSampleMethod)NativeMethods.sd_get_default_sample_method(_handle);
        var scheduler = options.Scheduler ?? (StableDiffusionScheduler)NativeMethods.sd_get_default_scheduler(_handle, (int)sampleMethod);

        native.sample_params.sample_method = (int)sampleMethod;
        native.sample_params.scheduler = (int)scheduler;
        native.sample_params.sample_steps = options.SampleSteps;
        native.sample_params.eta = options.Eta;
        native.sample_params.guidance.txt_cfg = options.GuidanceScale;
        native.sample_params.guidance.img_cfg = options.ImageGuidanceScale;
        native.sample_params.guidance.distilled_guidance = options.DistilledGuidanceScale;

        if (!string.IsNullOrWhiteSpace(options.ExtraSampleArguments))
        {
            native.sample_params.extra_sample_args = strings.Add(options.ExtraSampleArguments);
        }

        IntPtr imagePtr = NativeMethods.generate_image(_handle, ref native);
        if (imagePtr == IntPtr.Zero)
        {
            throw new InvalidOperationException("Native image generation failed.");
        }

        try
        {
            return NativeMethods.CopyImages(imagePtr, batchCount);
        }
        finally
        {
            NativeMethods.free_sd_images(imagePtr, batchCount);
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
}
