``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical cores and 2 physical cores
Frequency=2648441 Hz, Resolution=377.5806 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host] : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Core   : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|      Method |     N |         Mean |       Error |        StdDev |       Median |
|------------ |------ |-------------:|------------:|--------------:|-------------:|
|        **List** |     **4** |     **31.81 ns** |   **0.4316 ns** |     **0.4037 ns** |     **31.86 ns** |
| GrowthArray |     4 |           NA |          NA |            NA |           NA |
|        **List** |     **8** |     **37.38 ns** |   **0.7412 ns** |     **0.6934 ns** |     **37.32 ns** |
| GrowthArray |     8 |     44.11 ns |   0.9320 ns |     2.0260 ns |     44.86 ns |
|        **List** |    **16** |     **41.54 ns** |   **0.8998 ns** |     **0.8417 ns** |     **41.56 ns** |
| GrowthArray |    16 |     83.92 ns |   1.2139 ns |     1.1355 ns |     83.85 ns |
|        **List** |    **32** |     **65.65 ns** |   **1.1466 ns** |     **1.0725 ns** |     **65.62 ns** |
| GrowthArray |    32 |    125.60 ns |   2.0228 ns |     1.7932 ns |    126.11 ns |
|        **List** |    **64** |     **71.10 ns** |   **1.2012 ns** |     **1.0031 ns** |     **71.30 ns** |
| GrowthArray |    64 |    195.51 ns |   1.4289 ns |     1.2667 ns |    195.54 ns |
|        **List** |   **128** |     **82.34 ns** |   **1.6941 ns** |     **2.3190 ns** |     **81.93 ns** |
| GrowthArray |   128 |    266.26 ns |   2.8733 ns |     2.5471 ns |    266.46 ns |
|        **List** |   **256** |    **111.17 ns** |   **2.5791 ns** |     **6.1295 ns** |    **108.27 ns** |
| GrowthArray |   256 |    351.37 ns |   5.6482 ns |     5.0070 ns |    350.01 ns |
|        **List** |   **512** |    **182.68 ns** |   **3.7071 ns** |     **8.3676 ns** |    **181.37 ns** |
| GrowthArray |   512 |    467.41 ns |  12.6729 ns |    14.0859 ns |    465.93 ns |
|        **List** |  **1024** |    **300.75 ns** |  **18.0556 ns** |    **50.9262 ns** |    **315.78 ns** |
| GrowthArray |  1024 |    637.12 ns |  12.6983 ns |    26.7851 ns |    623.43 ns |
|        **List** |  **2048** |    **696.41 ns** |  **13.8668 ns** |    **28.0116 ns** |    **685.96 ns** |
| GrowthArray |  2048 |    994.56 ns |  38.4675 ns |   112.8186 ns |    942.85 ns |
|        **List** |  **4096** |  **1,788.36 ns** |  **54.3020 ns** |   **153.1597 ns** |  **1,867.73 ns** |
| GrowthArray |  4096 |  2,252.95 ns |  51.3590 ns |   148.1826 ns |  2,285.07 ns |
|        **List** |  **8192** |  **3,665.63 ns** |  **66.0027 ns** |    **64.8235 ns** |  **3,675.80 ns** |
| GrowthArray |  8192 |  3,465.64 ns | 482.5737 ns | 1,337.2096 ns |  3,736.89 ns |
|        **List** | **16384** |  **5,343.96 ns** | **283.8974 ns** |   **832.6218 ns** |  **4,968.51 ns** |
| GrowthArray | 16384 |  4,810.53 ns | 192.5424 ns |   564.6935 ns |  4,801.41 ns |
|        **List** | **32768** | **12,288.56 ns** | **245.4125 ns** |   **659.2847 ns** | **12,322.75 ns** |
| GrowthArray | 32768 | 13,077.74 ns | 259.9822 ns |   675.7284 ns | 13,213.96 ns |
|        **List** | **65536** | **27,982.46 ns** | **530.6277 ns** |   **761.0104 ns** | **27,983.44 ns** |
| GrowthArray | 65536 | 28,524.39 ns | 570.5254 ns | 1,071.5850 ns | 28,466.92 ns |

Benchmarks with issues:
  ListVsGrowthArray_Copying.GrowthArray: Core(Runtime=Core) [N=4]
