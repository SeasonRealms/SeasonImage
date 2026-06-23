// Copyright (c) SeasonEngine and contributors.
// Licensed under the MIT License.
//https://github.com/SeasonRealms/SeasonImage

namespace SeasonImage;

public readonly record struct StableDiffusionProgress(int Step, int TotalSteps, float ElapsedSeconds)
{
    public float Progress => TotalSteps <= 0 ? 0f : (float)Step / TotalSteps;
}

public enum StableDiffusionLogLevel
{
    Debug = 0,
    Info = 1,
    Warn = 2,
    Error = 3
}

public enum StableDiffusionCancelMode
{
    All = 0,
    NewLatents = 1,
    Reset = 2
}

public enum StableDiffusionRngType
{
    StdDefault = 0,
    Cuda = 1,
    Cpu = 2,
    Count = 3
}

public enum StableDiffusionPredictionType
{
    Eps = 0,
    V = 1,
    EdmV = 2,
    Flow = 3,
    FluxFlow = 4,
    Flux2Flow = 5
}

public enum StableDiffusionLoraApplyMode
{
    Auto = 0,
    Immediately = 1,
    AtRuntime = 2
}

public enum StableDiffusionSampleMethod
{
    Euler = 0,
    EulerA = 1,
    Heun = 2,
    Dpm2 = 3,
    Dpmpp2sA = 4,
    Dpmpp2M = 5,
    Dpmpp2Mv2 = 6,
    IPndm = 7,
    IPndmV = 8,
    Lcm = 9,
    DdimTrailing = 10,
    Tcd = 11,
    ResMultistep = 12,
    Res2S = 13,
    ErSde = 14,
    EulerCfgPp = 15,
    EulerACfgPp = 16,
    EulerGe = 17
}

public enum StableDiffusionScheduler
{
    Discrete = 0,
    Karras = 1,
    Exponential = 2,
    Ays = 3,
    Gits = 4,
    SgmUniform = 5,
    Simple = 6,
    Smoothstep = 7,
    KlOptimal = 8,
    Lcm = 9,
    BongTangent = 10,
    Ltx2 = 11
}

public enum StableDiffusionWeightType
{
    F32 = 0,
    F16 = 1,
    Q4_0 = 2,
    Q4_1 = 3,
    Q5_0 = 6,
    Q5_1 = 7,
    Q8_0 = 8,
    Q8_1 = 9,
    Q2K = 10,
    Q3K = 11,
    Q4K = 12,
    Q5K = 13,
    Q6K = 14,
    Q8K = 15,
    IQ2XXS = 16,
    IQ2XS = 17,
    IQ3XXS = 18,
    IQ1S = 19,
    IQ4NL = 20,
    IQ3S = 21,
    IQ2S = 22,
    IQ4XS = 23,
    I8 = 24,
    I16 = 25,
    I32 = 26,
    I64 = 27,
    F64 = 28,
    IQ1M = 29,
    BF16 = 30,
    TQ1_0 = 34,
    TQ2_0 = 35,
    MXFP4 = 39,
    NVFP4 = 40,
    Q1_0 = 41
}

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
