``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.388 (2004/?/20H1)
Intel Core i7-6700HQ CPU 2.60GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.100-preview.7.20366.6
  [Host]        : .NET Core 5.0.0 (CoreCLR 5.0.20.36411, CoreFX 5.0.20.36411), X64 RyuJIT
  .NET 4.8      : .NET Framework 4.8 (4.8.4084.0), X64 RyuJIT
  .NET Core 3.1 : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.0 (CoreCLR 5.0.20.36411, CoreFX 5.0.20.36411), X64 RyuJIT


```
|                              Method |           Job |       Runtime | InsertCount |       Mean |      Error |     StdDev |     Median | Ratio | RatioSD |      Gen 0 |     Gen 1 |    Gen 2 |    Allocated |
|------------------------------------ |-------------- |-------------- |------------ |-----------:|-----------:|-----------:|-----------:|------:|--------:|-----------:|----------:|---------:|-------------:|
|              **EfCoreInsertBatchAsync** |      **.NET 4.8** |      **.NET 4.8** |          **10** |   **2.768 ms** |  **0.0553 ms** |  **0.0518 ms** |   **2.762 ms** |  **1.00** |    **0.00** |    **42.9688** |   **11.7188** |        **-** |    **143.82 KB** |
|             VenflowInsertBatchAsync |      .NET 4.8 |      .NET 4.8 |          10 |   1.320 ms |  0.0264 ms |  0.0426 ms |   1.317 ms |  0.47 |    0.02 |     3.9063 |         - |        - |        17 KB |
| VenflowInsertBatchWithPKReturnAsync |      .NET 4.8 |      .NET 4.8 |          10 |   1.420 ms |  0.0522 ms |  0.1531 ms |   1.348 ms |  0.51 |    0.05 |     3.9063 |         - |        - |        17 KB |
|              RepoDbInsertBatchAsync |      .NET 4.8 |      .NET 4.8 |          10 |   1.656 ms |  0.0582 ms |  0.1669 ms |   1.593 ms |  0.58 |    0.05 |     7.8125 |         - |        - |     25.38 KB |
|                                     |               |               |             |            |            |            |            |       |         |            |           |          |              |
|              EfCoreInsertBatchAsync | .NET Core 3.1 | .NET Core 3.1 |          10 |   2.554 ms |  0.0510 ms |  0.0764 ms |   2.545 ms |  1.00 |    0.00 |    39.0625 |    7.8125 |        - |    126.87 KB |
|             VenflowInsertBatchAsync | .NET Core 3.1 | .NET Core 3.1 |          10 |   1.274 ms |  0.0254 ms |  0.0372 ms |   1.263 ms |  0.50 |    0.02 |     1.9531 |         - |        - |      11.8 KB |
| VenflowInsertBatchWithPKReturnAsync | .NET Core 3.1 | .NET Core 3.1 |          10 |   1.351 ms |  0.0413 ms |  0.1205 ms |   1.307 ms |  0.52 |    0.05 |     1.9531 |         - |        - |      11.8 KB |
|              RepoDbInsertBatchAsync | .NET Core 3.1 | .NET Core 3.1 |          10 |   1.481 ms |  0.0227 ms |  0.0347 ms |   1.478 ms |  0.58 |    0.02 |     5.8594 |         - |        - |     19.54 KB |
|                                     |               |               |             |            |            |            |            |       |         |            |           |          |              |
|              EfCoreInsertBatchAsync | .NET Core 5.0 | .NET Core 5.0 |          10 |   2.501 ms |  0.0498 ms |  0.0698 ms |   2.483 ms |  1.00 |    0.00 |    39.0625 |    7.8125 |        - |    127.04 KB |
|             VenflowInsertBatchAsync | .NET Core 5.0 | .NET Core 5.0 |          10 |   1.311 ms |  0.0503 ms |  0.1436 ms |   1.255 ms |  0.53 |    0.06 |     1.9531 |         - |        - |      11.8 KB |
| VenflowInsertBatchWithPKReturnAsync | .NET Core 5.0 | .NET Core 5.0 |          10 |   1.189 ms |  0.0235 ms |  0.0337 ms |   1.195 ms |  0.48 |    0.02 |     1.9531 |         - |        - |      11.8 KB |
|              RepoDbInsertBatchAsync | .NET Core 5.0 | .NET Core 5.0 |          10 |   1.423 ms |  0.0262 ms |  0.0219 ms |   1.430 ms |  0.57 |    0.02 |     5.8594 |         - |        - |     19.48 KB |
|                                     |               |               |             |            |            |            |            |       |         |            |           |          |              |
|              **EfCoreInsertBatchAsync** |      **.NET 4.8** |      **.NET 4.8** |         **100** |  **12.234 ms** |  **0.2001 ms** |  **0.1671 ms** |  **12.273 ms** |  **1.00** |    **0.00** |   **296.8750** |   **93.7500** |        **-** |   **1294.77 KB** |
|             VenflowInsertBatchAsync |      .NET 4.8 |      .NET 4.8 |         100 |   2.174 ms |  0.1026 ms |  0.2977 ms |   2.013 ms |  0.18 |    0.02 |    31.2500 |         - |        - |    102.17 KB |
| VenflowInsertBatchWithPKReturnAsync |      .NET 4.8 |      .NET 4.8 |         100 |   2.234 ms |  0.2070 ms |  0.5973 ms |   1.980 ms |  0.18 |    0.03 |    31.2500 |         - |        - |    102.13 KB |
|              RepoDbInsertBatchAsync |      .NET 4.8 |      .NET 4.8 |         100 |   4.563 ms |  0.1025 ms |  0.2857 ms |   4.549 ms |  0.37 |    0.02 |    62.5000 |         - |        - |    209.94 KB |
|                                     |               |               |             |            |            |            |            |       |         |            |           |          |              |
|              EfCoreInsertBatchAsync | .NET Core 3.1 | .NET Core 3.1 |         100 |  10.730 ms |  0.2195 ms |  0.5972 ms |  10.524 ms |  1.00 |    0.00 |   265.6250 |   78.1250 |        - |   1194.18 KB |
|             VenflowInsertBatchAsync | .NET Core 3.1 | .NET Core 3.1 |         100 |   2.058 ms |  0.1223 ms |  0.3586 ms |   1.957 ms |  0.19 |    0.03 |    27.3438 |         - |        - |     88.16 KB |
| VenflowInsertBatchWithPKReturnAsync | .NET Core 3.1 | .NET Core 3.1 |         100 |   2.088 ms |  0.1128 ms |  0.3238 ms |   2.014 ms |  0.19 |    0.03 |    27.3438 |         - |        - |     88.16 KB |
|              RepoDbInsertBatchAsync | .NET Core 3.1 | .NET Core 3.1 |         100 |   4.556 ms |  0.1329 ms |  0.3639 ms |   4.500 ms |  0.43 |    0.04 |    46.8750 |         - |        - |    164.07 KB |
|                                     |               |               |             |            |            |            |            |       |         |            |           |          |              |
|              EfCoreInsertBatchAsync | .NET Core 5.0 | .NET Core 5.0 |         100 |   9.927 ms |  0.1957 ms |  0.1634 ms |   9.929 ms |  1.00 |    0.00 |   265.6250 |   62.5000 |        - |   1193.69 KB |
|             VenflowInsertBatchAsync | .NET Core 5.0 | .NET Core 5.0 |         100 |   2.007 ms |  0.0869 ms |  0.2548 ms |   1.962 ms |  0.20 |    0.03 |    27.3438 |         - |        - |     88.15 KB |
| VenflowInsertBatchWithPKReturnAsync | .NET Core 5.0 | .NET Core 5.0 |         100 |   2.109 ms |  0.0939 ms |  0.2709 ms |   2.079 ms |  0.21 |    0.04 |    27.3438 |    5.8594 |        - |     88.15 KB |
|              RepoDbInsertBatchAsync | .NET Core 5.0 | .NET Core 5.0 |         100 |   4.605 ms |  0.1326 ms |  0.3630 ms |   4.564 ms |  0.46 |    0.03 |    46.8750 |         - |        - |    163.94 KB |
|                                     |               |               |             |            |            |            |            |       |         |            |           |          |              |
|              **EfCoreInsertBatchAsync** |      **.NET 4.8** |      **.NET 4.8** |        **1000** |  **96.396 ms** |  **1.8884 ms** |  **2.8264 ms** |  **96.607 ms** |  **1.00** |    **0.00** |  **2000.0000** |  **800.0000** |        **-** |  **12766.06 KB** |
|             VenflowInsertBatchAsync |      .NET 4.8 |      .NET 4.8 |        1000 |  11.014 ms |  0.7429 ms |  2.1552 ms |  10.622 ms |  0.11 |    0.02 |   156.2500 |   78.1250 |        - |    969.34 KB |
| VenflowInsertBatchWithPKReturnAsync |      .NET 4.8 |      .NET 4.8 |        1000 |  10.937 ms |  0.6167 ms |  1.7892 ms |  10.619 ms |  0.11 |    0.02 |   156.2500 |   78.1250 |        - |    969.54 KB |
|              RepoDbInsertBatchAsync |      .NET 4.8 |      .NET 4.8 |        1000 |  39.463 ms |  0.7721 ms |  1.8349 ms |  39.164 ms |  0.41 |    0.02 |   500.0000 |  100.0000 |        - |   2045.09 KB |
|                                     |               |               |             |            |            |            |            |       |         |            |           |          |              |
|              EfCoreInsertBatchAsync | .NET Core 3.1 | .NET Core 3.1 |        1000 |  89.273 ms |  1.7793 ms |  2.8732 ms |  88.710 ms |  1.00 |    0.00 |  1833.3333 |  833.3333 |        - |  11809.36 KB |
|             VenflowInsertBatchAsync | .NET Core 3.1 | .NET Core 3.1 |        1000 |   8.862 ms |  0.3265 ms |  0.9577 ms |   8.391 ms |  0.10 |    0.01 |   140.6250 |   62.5000 |        - |    854.13 KB |
| VenflowInsertBatchWithPKReturnAsync | .NET Core 3.1 | .NET Core 3.1 |        1000 |   8.804 ms |  0.3489 ms |  0.9898 ms |   8.324 ms |  0.10 |    0.01 |   140.6250 |   62.5000 |        - |    854.13 KB |
|              RepoDbInsertBatchAsync | .NET Core 3.1 | .NET Core 3.1 |        1000 |  36.148 ms |  0.7155 ms |  1.3785 ms |  35.975 ms |  0.40 |    0.02 |   461.5385 |  153.8462 |        - |   1600.29 KB |
|                                     |               |               |             |            |            |            |            |       |         |            |           |          |              |
|              EfCoreInsertBatchAsync | .NET Core 5.0 | .NET Core 5.0 |        1000 |  86.992 ms |  1.7392 ms |  1.7860 ms |  87.041 ms |  1.00 |    0.00 |  1833.3333 |  666.6667 |        - |     11802 KB |
|             VenflowInsertBatchAsync | .NET Core 5.0 | .NET Core 5.0 |        1000 |   8.552 ms |  0.3162 ms |  0.9122 ms |   8.140 ms |  0.10 |    0.01 |   125.0000 |   62.5000 |        - |    854.12 KB |
| VenflowInsertBatchWithPKReturnAsync | .NET Core 5.0 | .NET Core 5.0 |        1000 |   8.736 ms |  0.4083 ms |  1.1844 ms |   8.121 ms |  0.10 |    0.01 |   125.0000 |   62.5000 |        - |    854.12 KB |
|              RepoDbInsertBatchAsync | .NET Core 5.0 | .NET Core 5.0 |        1000 |  38.161 ms |  0.7582 ms |  2.1010 ms |  38.171 ms |  0.44 |    0.03 |   500.0000 |  166.6667 |        - |   1599.52 KB |
|                                     |               |               |             |            |            |            |            |       |         |            |           |          |              |
|              **EfCoreInsertBatchAsync** |      **.NET 4.8** |      **.NET 4.8** |       **10000** | **948.969 ms** | **18.7329 ms** | **18.3982 ms** | **944.477 ms** |  **1.00** |    **0.00** | **21000.0000** | **7000.0000** |        **-** | **127690.98 KB** |
|             VenflowInsertBatchAsync |      .NET 4.8 |      .NET 4.8 |       10000 |  94.377 ms |  2.6052 ms |  7.5994 ms |  92.088 ms |  0.10 |    0.01 |  1500.0000 |  666.6667 | 166.6667 |     10077 KB |
| VenflowInsertBatchWithPKReturnAsync |      .NET 4.8 |      .NET 4.8 |       10000 |  95.301 ms |  2.8616 ms |  8.3475 ms |  92.229 ms |  0.10 |    0.01 |  1500.0000 |  666.6667 | 166.6667 |  10077.97 KB |
|              RepoDbInsertBatchAsync |      .NET 4.8 |      .NET 4.8 |       10000 | 649.190 ms | 12.9558 ms | 25.2693 ms | 650.669 ms |  0.68 |    0.02 |  6000.0000 | 1000.0000 |        - |  20611.05 KB |
|                                     |               |               |             |            |            |            |            |       |         |            |           |          |              |
|              EfCoreInsertBatchAsync | .NET Core 3.1 | .NET Core 3.1 |       10000 | 804.554 ms | 15.0753 ms | 14.1014 ms | 808.920 ms |  1.00 |    0.00 | 19000.0000 | 7000.0000 |        - | 118204.28 KB |
|             VenflowInsertBatchAsync | .NET Core 3.1 | .NET Core 3.1 |       10000 |  81.511 ms |  2.7037 ms |  7.7138 ms |  77.862 ms |  0.10 |    0.01 |  1333.3333 |  666.6667 | 166.6667 |   8530.54 KB |
| VenflowInsertBatchWithPKReturnAsync | .NET Core 3.1 | .NET Core 3.1 |       10000 |  79.818 ms |  2.0380 ms |  5.6811 ms |  77.629 ms |  0.10 |    0.01 |  1285.7143 |  571.4286 | 142.8571 |   8530.95 KB |
|              RepoDbInsertBatchAsync | .NET Core 3.1 | .NET Core 3.1 |       10000 | 453.889 ms |  8.8771 ms | 21.6082 ms | 447.688 ms |  0.57 |    0.03 |  4000.0000 | 1000.0000 |        - |  16057.98 KB |
|                                     |               |               |             |            |            |            |            |       |         |            |           |          |              |
|              EfCoreInsertBatchAsync | .NET Core 5.0 | .NET Core 5.0 |       10000 | 851.376 ms | 16.2747 ms | 33.6101 ms | 854.831 ms |  1.00 |    0.00 | 19000.0000 | 7000.0000 |        - | 194427.63 KB |
|             VenflowInsertBatchAsync | .NET Core 5.0 | .NET Core 5.0 |       10000 |  84.478 ms |  2.9189 ms |  8.4681 ms |  81.299 ms |  0.10 |    0.01 |  1285.7143 |  571.4286 | 142.8571 |   8534.51 KB |
| VenflowInsertBatchWithPKReturnAsync | .NET Core 5.0 | .NET Core 5.0 |       10000 |  84.380 ms |  2.6278 ms |  7.3686 ms |  82.252 ms |  0.10 |    0.01 |  1285.7143 |  571.4286 | 142.8571 |   8533.22 KB |
|              RepoDbInsertBatchAsync | .NET Core 5.0 | .NET Core 5.0 |       10000 | 501.349 ms |  9.9165 ms | 13.9015 ms | 505.091 ms |  0.60 |    0.03 |  4000.0000 | 1000.0000 |        - |  16050.68 KB |