``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical cores and 2 physical cores
Frequency=2648438 Hz, Resolution=377.5810 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host] : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Core   : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|      Method |  N |     Mean |      Error |     StdDev |  Gen 0 | Allocated |
|------------ |--- |---------:|-----------:|-----------:|-------:|----------:|
|        **List** | **10** | **218.7 ns** |  **6.5358 ns** | **18.7525 ns** | **0.2134** |     **336 B** |
| GrowthArray | 10 | 116.3 ns |  9.6878 ns | 28.5647 ns | 0.1830 |     288 B |
|        **List** | **25** | **199.0 ns** |  **3.9498 ns** |  **6.4897 ns** | **0.3915** |     **616 B** |
| GrowthArray | 25 | 191.0 ns | 14.7254 ns | 43.4182 ns | 0.2797 |     440 B |
|        **List** | **50** | **321.5 ns** |  **6.6240 ns** | **13.6797 ns** | **0.7315** |    **1152 B** |
| GrowthArray | 50 | 204.4 ns |  0.9245 ns |  0.8648 ns | 0.4575 |     720 B |
