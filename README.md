# ASFBuffBot

反序列化暂时有问题，插件可能无法正常工作，请使用自编译版本的 ASF

反序列化暂时有问题，插件可能无法正常工作，请使用自编译版本的 ASF

反序列化暂时有问题，插件可能无法正常工作，请使用自编译版本的 ASF

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/28d15406751f42f499e2f53fde5bb808)](https://www.codacy.com/gh/chr233/ASFBuffBot/dashboard)
![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/chr233/ASFBuffBot/autobuild.yml?logo=github)
[![License](https://img.shields.io/github/license/chr233/ASFBuffBot?logo=apache)](https://github.com/chr233/ASFBuffBot/blob/master/license)

[![GitHub Release](https://img.shields.io/github/v/release/chr233/ASFBuffBot?logo=github)](https://github.com/chr233/ASFBuffBot/releases)
[![GitHub Release](https://img.shields.io/github/v/release/chr233/ASFBuffBot?include_prereleases&label=pre-release&logo=github)](https://github.com/chr233/ASFBuffBot/releases)
![GitHub last commit](https://img.shields.io/github/last-commit/chr233/ASFBuffBot?logo=github)

![GitHub Repo stars](https://img.shields.io/github/stars/chr233/ASFBuffBot?logo=github)
[![GitHub Download](https://img.shields.io/github/downloads/chr233/ASFBuffBot/total?logo=github)](https://img.shields.io/github/v/release/chr233/ASFBuffBot)

[![Bilibili](https://img.shields.io/badge/bilibili-Chr__-00A2D8.svg?logo=bilibili)](https://space.bilibili.com/5805394)
[![Steam](https://img.shields.io/badge/steam-Chr__-1B2838.svg?logo=steam)](https://steamcommunity.com/id/Chr_)

[![Steam](https://img.shields.io/badge/steam-donate-1B2838.svg?logo=steam)](https://steamcommunity.com/tradeoffer/new/?partner=221260487&token=xgqMgL-i)
[![爱发电](https://img.shields.io/badge/爱发电-chr__-ea4aaa.svg?logo=github-sponsors)](https://afdian.net/@chr233)

ASFBuffBot 介绍 & 使用指南: [https://keylol.com/t887696-1-1](https://keylol.com/t887696-1-1)

[Русская Версия](README.ru.md) | [English Version](README.en.md)

## 安装方式

### 初次安装 / 手动更新

1. 从 [GitHub Releases](https://github.com/chr233/ASFBuffBot/releases) 下载插件的最新版本
2. 解压后将 `ASFBuffBot.dll` 丢进 `ArchiSteamFarm` 目录下的 `plugins` 文件夹
3. 重新启动 `ArchiSteamFarm` , 使用命令 `ASFBUFFBOT` 或者 `ABB` 来检查插件是否正常工作

### 使用命令升级插件

> 插件自动更新功能以前一直
> ASF 版本升级有可能出现不兼容情况, 如果发现插件无法加载请尝试更新 ASF

- `ABBVERSION` / `ABBV` 检查插件更新
- `ABBUPDATE` / `ABBU` 自动更新插件到最新版本 (需要手动重启 ASF)

### 更新日志

| ASFBuffBot 版本                                                      | 适配 ASF 版本 | 更新说明                                                                                   |
| -------------------------------------------------------------------- | :-----------: | ------------------------------------------------------------------------------------------ |
| [1.1.2.3](https://github.com/chr233/ASFBuffBot/releases/tag/1.1.0.0) |   5.5.0.11    | ASF -> 5.5.0.11                                                                            |
| [1.1.1.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.1.0.0) |   5.4.10.3    | ASF -> 5.4.10.3                                                                            |
| [1.1.0.2](https://github.com/chr233/ASFBuffBot/releases/tag/1.1.0.0) |    5.4.8.3    | 适配手机验证码登录, 支持存储 `Cookies`, 储存文件格式修改, 需要重新设定启用自动发货的机器人 |
| [1.0.8.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.8.0) |    5.4.5.2    | 新增 `RejectNotMatch` 选项                                                                 |
| [1.0.7.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.7.0) |    5.4.5.2    | 增加命令 `UPDATECOOKIESBOT` 用于手动设置机器人 cookies                                     |
| [1.0.6.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.6.0) |    5.4.5.2    | bug 修复                                                                                   |
| [1.0.5.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.5.0) |    5.4.5.2    | 支持自动登录 Buff, bug 修复                                                                |
| [1.0.4.1](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.4.1) |    5.4.5.2    | 支持多账号, bug 修复                                                                       |
| [1.0.0.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.0.0) |    5.4.4.5    | 第一个版本, 单账号模式                                                                     |

## 插件配置说明

> 本插件的配置不是必须的, 保持默认配置即可使用大部分功能

ASF.json

```json
{
  //ASF 配置
  "CurrentCulture": "...",
  "IPCPassword": "...",
  "...": "...",
  "ASFEnhance": {
    //ASFBuffBot 配置
    "Statistic": true,
    "BuffCheckInterval": 180,
    "BotInterval": 30,
    "CustomUserAgent": null,
    "RejectNotMatch": false
  }
}
```

| 配置项              | 类型   | 默认值  | 说明                                                                                    |
| ------------------- | ------ | ------- | --------------------------------------------------------------------------------------- |
| `Statistic`         | bool   | `true`  | 是否允许发送统计数据, 仅用于统计插件用户数量, 不会发送任何其他信息                      |
| `BuffCheckInterval` | int    | `180`   | 每一轮 Buff 发货检查的周期, 单位秒, 访问频率过快容易被 ban                              |
| `BotInterval`       | int    | `30`    | 在一轮发货检查中每个机器人的检查间隔, 单位秒                                            |
| `CustomUserAgent`   | string | `null`  | 自定义 `User-Agent` 用于向 Buff 发送请求                                                |
| `RejectNotMatch`    | bool   | `false` | 交易物品不匹配时是否自动拒绝交易                                                        |
| `AlwaysSendSmsCode` | bool   | `false` | 设为 `true` 时每次登录 Buff 强制发送手机验证码, 设为 `false` 时仅在需要时发送手机验证码 |

> 禁用命令功能已经迁移至 `ASFEnhance` 插件中
> 当某条命令被禁用时, 仍然可以使用 `ABB.xxx` 的形式调用被禁用的命令, 例如 `ABB.UPDATECOOKIES`

## 插件指令说明

### 插件更新

| 命令         | 缩写   | 权限            | 说明                                              |
| ------------ | ------ | --------------- | ------------------------------------------------- |
| `ASFBUFFBOT` | `ABB`  | `FamilySharing` | 查看 ASFBuffBot 的版本                            |
| `ABBVERSION` | `ABBV` | `Operator`      | 检查 ASFBuffBot 是否为最新版本                    |
| `ABBUPDATE`  | `ABBU` | `Owner`         | 自动更新 ASFBuffBot 到最新版本 (需要手动重启 ASF) |

### 功能指令

| 命令                           | 缩写  | 权限     | 说明                                                            |
| ------------------------------ | ----- | -------- | --------------------------------------------------------------- |
| `ENABLEBUFF [Bots]`            | `EB`  | `Master` | 为指定机器人开启自动发货功能, 将会尝试自动登录到 `buff.163.com` |
| `VERIFYCODE Bot Code`          | `VC`  | `Master` | 输入手机验证码, 如果登录 Buff 要求验证手机号                    |
| `DISABLEBUFF [Bots]`           | `DB`  | `Master` | 为指定机器人开启自动发货功能                                    |
| `BUFFSTATUS [Bots]`            | `BS`  | `Master` | 查看指定机器人自动发货状态                                      |
| `UPDATECOOKIESBOT Bot Cookies` | `UCB` | `Master` | 查看指定机器人自动发货状态                                      |
