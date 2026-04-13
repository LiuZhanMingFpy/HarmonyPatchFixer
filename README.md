# HarmonyPatchFixer

[English](#english) | [中文](#中文)

---

<a id="english"></a>
## English

A universal fixer mod for **Slay the Spire 2** that repairs Harmony mods broken by calling `PatchAll()` without specifying their assembly.

### The Problem

Some STS2 mods call `Harmony.PatchAll()` without passing an explicit assembly parameter:

```csharp
_harmony.PatchAll();
```

On certain environments, this call fails to apply patches — the mod loads without errors but its `[HarmonyPatch]` classes are never activated. The exact root cause is unclear; it may be related to Mono runtime assembly binding behavior.

The fix is simple and verified: calling `PatchAll(asm)` with the mod's own assembly explicitly resolves the issue.

### How It Works

After a 2-second delay (to let all mods initialize), HarmonyPatchFixer scans all loaded assemblies, identifies mod assemblies by their load path (`mods` folder), and calls `PatchAll(asm)` with the correct assembly for each:

```csharp
fixHarmony.PatchAll(asm);
```

### Installation

1. Download `HarmonyPatchFixer.dll` and `HarmonyPatchFixer.json` from [Releases](https://github.com/LiuZhanMingFpy/HarmonyPatchFixer/releases)
2. Create a folder `mods/HarmonyPatchFixer` in your STS2 game directory
3. Place both files inside that folder
4. Launch the game

### Building from Source

Requires:
- .NET 9 SDK
- A copy of Slay the Spire 2

1. Clone this repo
2. Update the `HintPath` values in `HarmonyPatchFixer.csproj` to point to your STS2 installation's `data_sts2_windows_x86_64` folder
3. Run `dotnet build -c Release`
4. Copy the output `HarmonyPatchFixer.dll` + `HarmonyPatchFixer.json` to your `mods/HarmonyPatchFixer` folder

---

<a id="中文"></a>
## 中文

适用于 **杀戮尖塔 2 (Slay the Spire 2)** 的通用修复 Mod，修复因调用 `PatchAll()` 时未指定程序集而导致失效的 Harmony Mod。

### 问题背景

部分 STS2 Mod 在调用 `Harmony.PatchAll()` 时没有传入程序集参数：

```csharp
_harmony.PatchAll();
```

在某些环境下，此调用无法正确应用补丁——Mod 加载时不会报错，但其 `[HarmonyPatch]` 类从未被激活。具体根因尚不明确，可能与 Mono 运行时的程序集绑定行为有关。

修复方法很简单且已验证：调用 `PatchAll(asm)` 并显式传入 Mod 自身的程序集即可解决问题。

### 工作原理

在所有 Mod 初始化完成后（延迟 2 秒），HarmonyPatchFixer 扫描所有已加载的程序集，通过加载路径（`mods` 文件夹）识别 Mod 程序集，并为每个 Mod 调用传入正确程序集的 `PatchAll(asm)`：

```csharp
fixHarmony.PatchAll(asm);
```

### 安装方法

1. 从 [Releases](https://github.com/LiuZhanMingFpy/HarmonyPatchFixer/releases) 下载 `HarmonyPatchFixer.dll` 和 `HarmonyPatchFixer.json`
2. 在你的 STS2 游戏目录下创建文件夹 `mods/HarmonyPatchFixer`
3. 将这两个文件放入该文件夹
4. 启动游戏

### 从源码构建

需要：
- .NET 9 SDK
- 杀戮尖塔 2 游戏本体

1. 克隆此仓库
2. 修改 `HarmonyPatchFixer.csproj` 中的 `HintPath`，指向你的 STS2 安装目录下的 `data_sts2_windows_x86_64` 文件夹
3. 运行 `dotnet build -c Release`
4. 将生成的 `HarmonyPatchFixer.dll` 和 `HarmonyPatchFixer.json` 复制到 `mods/HarmonyPatchFixer` 文件夹
