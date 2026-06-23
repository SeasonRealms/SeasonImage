// Copyright (c) SeasonEngine and contributors.
// Licensed under the MIT License.
// https://github.com/SeasonRealms/SeasonImage

namespace SeasonImage;

internal static class NativeMethods
{
    private const string LibraryName = "stable-diffusion";

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSdCtxParams
    {
        public IntPtr model_path;
        public IntPtr clip_l_path;
        public IntPtr clip_g_path;
        public IntPtr clip_vision_path;
        public IntPtr t5xxl_path;
        public IntPtr llm_path;
        public IntPtr llm_vision_path;
        public IntPtr diffusion_model_path;
        public IntPtr high_noise_diffusion_model_path;
        public IntPtr uncond_diffusion_model_path;
        public IntPtr embeddings_connectors_path;
        public IntPtr vae_path;
        public IntPtr audio_vae_path;
        public IntPtr taesd_path;
        public IntPtr control_net_path;
        public IntPtr embeddings;
        public uint embedding_count;
        public IntPtr photo_maker_path;
        public IntPtr pulid_weights_path;
        public IntPtr tensor_type_rules;
        public int n_threads;
        public int wtype;
        public int rng_type;
        public int sampler_rng_type;
        public int prediction;
        public int lora_apply_mode;
        public byte enable_mmap;
        public byte flash_attn;
        public byte diffusion_flash_attn;
        public byte tae_preview_only;
        public byte diffusion_conv_direct;
        public byte vae_conv_direct;
        public byte circular_x;
        public byte circular_y;
        public byte force_sdxl_vae_conv_scale;
        public byte chroma_use_dit_mask;
        public byte chroma_use_t5_mask;
        public int chroma_t5_mask_pad;
        public byte qwen_image_zero_cond_t;
        public int vae_format;
        public IntPtr max_vram;
        public byte stream_layers;
        public IntPtr backend;
        public IntPtr params_backend;
        public IntPtr rpc_servers;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSdImage
    {
        public uint width;
        public uint height;
        public uint channel;
        public IntPtr data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSdSlgParams
    {
        public IntPtr layers;
        public nuint layer_count;
        public float layer_start;
        public float layer_end;
        public float scale;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSdGuidanceParams
    {
        public float txt_cfg;
        public float img_cfg;
        public float distilled_guidance;
        public NativeSdSlgParams slg;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSdSampleParams
    {
        public NativeSdGuidanceParams guidance;
        public int scheduler;
        public int sample_method;
        public int sample_steps;
        public float eta;
        public int shifted_timestep;
        public IntPtr custom_sigmas;
        public int custom_sigmas_count;
        public float flow_shift;
        public IntPtr extra_sample_args;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSdPmParams
    {
        public IntPtr id_images;
        public int id_images_count;
        public IntPtr id_embed_path;
        public float style_strength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSdPulidParams
    {
        public IntPtr id_embedding_path;
        public float id_weight;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSdTilingParams
    {
        public byte enabled;
        public byte temporal_tiling;
        public int tile_size_x;
        public int tile_size_y;
        public float target_overlap;
        public float rel_size_x;
        public float rel_size_y;
        public IntPtr extra_tiling_args;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSdCacheParams
    {
        public int mode;
        public float reuse_threshold;
        public float start_percent;
        public float end_percent;
        public float error_decay_rate;
        public byte use_relative_threshold;
        public byte reset_error_on_compute;
        public int Fn_compute_blocks;
        public int Bn_compute_blocks;
        public float residual_diff_threshold;
        public int max_warmup_steps;
        public int max_cached_steps;
        public int max_continuous_cached_steps;
        public int taylorseer_n_derivatives;
        public int taylorseer_skip_interval;
        public IntPtr scm_mask;
        public byte scm_policy_dynamic;
        public float spectrum_w;
        public int spectrum_m;
        public float spectrum_lam;
        public int spectrum_window_size;
        public float spectrum_flex_window;
        public int spectrum_warmup_steps;
        public float spectrum_stop_percent;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSdHiresParams
    {
        public byte enabled;
        public int upscaler;
        public IntPtr model_path;
        public float scale;
        public int target_width;
        public int target_height;
        public int steps;
        public float denoising_strength;
        public int upscale_tile_size;
        public IntPtr custom_sigmas;
        public int custom_sigmas_count;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSdLora
    {
        public byte is_high_noise;
        public float multiplier;
        public IntPtr path;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSdImgGenParams
    {
        public IntPtr loras;
        public uint lora_count;
        public IntPtr prompt;
        public IntPtr negative_prompt;
        public int clip_skip;
        public NativeSdImage init_image;
        public IntPtr ref_images;
        public int ref_images_count;
        public byte auto_resize_ref_image;
        public byte increase_ref_index;
        public NativeSdImage mask_image;
        public int width;
        public int height;
        public NativeSdSampleParams sample_params;
        public float strength;
        public long seed;
        public int batch_count;
        public NativeSdImage control_image;
        public float control_strength;
        public NativeSdPmParams pm_params;
        public NativeSdPulidParams pulid_params;
        public NativeSdTilingParams vae_tiling_params;
        public NativeSdCacheParams cache;
        public NativeSdHiresParams hires;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void NativeLogCallback(int level, IntPtr text, IntPtr data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void NativeProgressCallback(int step, int steps, float time, IntPtr data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void NativePreviewCallback(
        int step,
        int frameCount,
        IntPtr frames,
        [MarshalAs(UnmanagedType.I1)] bool isNoisy,
        IntPtr data);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern void sd_set_log_callback(NativeLogCallback? callback, IntPtr data);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern void sd_set_progress_callback(NativeProgressCallback? callback, IntPtr data);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern void sd_set_preview_callback(
        NativePreviewCallback? callback,
        int mode,
        int interval,
        byte denoised,
        byte noisy,
        IntPtr data);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern int sd_get_num_physical_cores();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern IntPtr sd_get_system_info();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern void sd_ctx_params_init(ref NativeSdCtxParams sd_ctx_params);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern IntPtr new_sd_ctx(ref NativeSdCtxParams sd_ctx_params);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern void free_sd_ctx(IntPtr sd_ctx);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool sd_ctx_supports_image_generation(IntPtr sd_ctx);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool sd_ctx_supports_video_generation(IntPtr sd_ctx);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern void sd_sample_params_init(ref NativeSdSampleParams sample_params);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern int sd_get_default_sample_method(IntPtr sd_ctx);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern int sd_get_default_scheduler(IntPtr sd_ctx, int sample_method);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern void sd_img_gen_params_init(ref NativeSdImgGenParams img_gen_params);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern IntPtr generate_image(IntPtr sd_ctx, ref NativeSdImgGenParams img_gen_params);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern void free_sd_images(IntPtr result_images, int num_images);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern void sd_cancel_generation(IntPtr sd_ctx, int mode);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern IntPtr sd_commit();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern IntPtr sd_version();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool convert(
        IntPtr inputPath,
        IntPtr vaePath,
        IntPtr outputPath,
        int outputType,
        IntPtr tensorTypeRules,
        [MarshalAs(UnmanagedType.I1)] bool convertName);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern IntPtr new_upscaler_ctx(
        IntPtr esrganPath,
        [MarshalAs(UnmanagedType.I1)] bool direct,
        int nThreads,
        int tileSize,
        IntPtr backend,
        IntPtr paramsBackend);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern void free_upscaler_ctx(IntPtr upscalerCtx);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern NativeSdImage upscale(IntPtr upscalerCtx, NativeSdImage inputImage, uint upscaleFactor);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern int get_upscale_factor(IntPtr upscalerCtx);

    [DllImport("ucrtbase", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    private static extern IntPtr malloc(nuint size);

    [DllImport("ucrtbase", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    private static extern void free(IntPtr ptr);

    internal static string PtrToString(IntPtr ptr)
    {
        return ptr == IntPtr.Zero ? string.Empty : Marshal.PtrToStringUTF8(ptr) ?? string.Empty;
    }

    internal static byte ToNativeBool(bool value) => value ? (byte)1 : (byte)0;

    internal static IReadOnlyList<StableDiffusionImageResult> CopyImages(IntPtr images, int count)
    {
        if (images == IntPtr.Zero || count <= 0)
        {
            return [];
        }

        var result = new List<StableDiffusionImageResult>(count);
        var stride = Marshal.SizeOf<NativeSdImage>();

        for (var i = 0; i < count; i++)
        {
            var ptr = IntPtr.Add(images, i * stride);
            var native = Marshal.PtrToStructure<NativeSdImage>(ptr);
            if (native.data == IntPtr.Zero)
            {
                continue;
            }

            result.Add(CopyImage(native));
        }

        return result;
    }

    internal static StableDiffusionImageResult CopyImage(NativeSdImage native)
    {
        if (native.data == IntPtr.Zero || native.width == 0 || native.height == 0 || native.channel == 0)
        {
            return new StableDiffusionImageResult(0, 0, 0, []);
        }

        checked
        {
            var byteCount = (int)(native.width * native.height * native.channel);
            var bytes = new byte[byteCount];
            Marshal.Copy(native.data, bytes, 0, byteCount);
            return new StableDiffusionImageResult((int)native.width, (int)native.height, (int)native.channel, bytes);
        }
    }

    internal static void ReleaseReturnedImage(NativeSdImage image)
    {
        if (image.data == IntPtr.Zero)
        {
            return;
        }

        var wrapper = malloc((nuint)Marshal.SizeOf<NativeSdImage>());
        if (wrapper == IntPtr.Zero)
        {
            free(image.data);
            return;
        }

        Marshal.StructureToPtr(image, wrapper, false);
        free_sd_images(wrapper, 1);
    }
}
