# ASFBuffBot

Temporary problem with deserialisation, plugin may not work properly，请使用自编译版本的 ASF

Temporary problem with deserialisation, plugin may not work properly，请使用自编译版本的 ASF

Temporary problem with deserialisation, plugin may not work properly，请使用自编译版本的 ASF

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

[中文说明](README.md) | [Русская Версия](README.ru.md)

## Installation

### First-Time Install / Manually Update

1. Download the plugin via [GitHub Releases](https://github.com/chr233/ASFBuffBot/releases) page
2. Unzip the `ASFBuffBot.dll` and copy it into the `plugins` folder in the `ArchiSteamFarm`'s directory
3. Restart the `ArchiSteamFarm` and use `ASFBUFFBOT`or `ABB` commands to check if the plugin is working

### Use Command to Update

> You can update the plugin by using the command that comes with the plugin.
> ASF version upgrade may be incompatible, if you find that the plugin can not be loaded, please try to update ASF

- `ABBVERSION` / `ABBV` check the latest version of ASFBuffBot
- `ABBUPDATE` / `ABBU` auto update ASFBuffBot (Maybe need to update ASF manually)

### ChangeLog

| ASFBuffBot Version                                                   | Compatible ASF version | Description                                                                                |
| -------------------------------------------------------------------- | :--------------------: | ------------------------------------------------------------------------------------------ |
| [1.1.2.3](https://github.com/chr233/ASFBuffBot/releases/tag/1.1.0.0) |        5.5.0.11        | ASF -> 5.5.0.11                                                                            |
| [1.1.1.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.1.0.0) |        5.4.10.3        | ASF -> 5.4.10.3                                                                            |
| [1.1.0.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.1.0.0) |        5.4.8.3         | 适配手机验证码登录, 支持存储 `Cookies`, 储存文件格式修改, 需要重新设定启用自动发货的机器人 |
| [1.0.8.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.8.0) |        5.4.5.2         | Add `RejectNotMatch` option                                                                |
| [1.0.7.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.7.0) |        5.4.5.2         | Add command `UPDATECOOKIESBOT` to manually set bot cookies                                 |
| [1.0.6.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.6.0) |        5.4.5.2         | Bug fixes                                                                                  |
| [1.0.5.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.5.0) |        5.4.5.2         | Support for auto-login Buff, bug fixes                                                     |
| [1.0.4.1](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.4.1) |        5.4.5.2         | Multi-account support, bug fixes                                                           |
| [1.0.0.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.0.0) |        5.4.4.5         | First version, single-account mode                                                         |

## Plugin Configuration

> The configuration of this plugin is not required, and most functions is available in default settings

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

| Configuration       | Type   | Default | Description                                                                                                       |
| ------------------- | ------ | ------- | ----------------------------------------------------------------------------------------------------------------- |
| `Statistic`         | bool   | `true`  | Allow send statistics data, it's used to count number of users, this will not send any other information          |
| `BuffCheckInterval` | int    | `180`   | 每一轮 Buff 发货检查的周期, 单位秒, 访问频率过快容易被 ban                                                        |
| `BotInterval`       | int    | `30`    | 在一轮发货检查中每个机器人的检查间隔, 单位秒                                                                      |
| `CustomUserAgent`   | string | `null`  | Custom `User-Agent` to send requests to Buff                                                                      |
| `RejectNotMatch`    | bool   | `false` | 交易物品不匹配时是否自动拒绝交易                                                                                  |
| `AlwaysSendSmsCode` | bool   | `false` | Set to `true` to force send SMS code every time you log in Buff, set to `false` to send SMS code only when needed |

> Disabled commands have been migrated to the `ASFEnhance` plugin
> When a command is disabled, the disabled command can still be called as `ABB.xxx`, e.g. `ABB.UPDATECOOKIES`

## Commands Usage

### Update Commands

| Command      | Shorthand | Access          | Description                                                         |
| ------------ | --------- | --------------- | ------------------------------------------------------------------- |
| `ASFBUFFBOT` | `ABB`     | `FamilySharing` | Get the version of the ASFBuffBot                                   |
| `ABBVERSION` | `ABBV`    | `Operator`      | Check ASFBuffBot's latest version                                   |
| `ABBUPDATE`  | `ABBU`    | `Owner`         | Update ASFBuffBot to the latest version (need restart ASF manually) |

### Functional commands

| Command                        | Shorthand | Access   | Description                                                     |
| ------------------------------ | --------- | -------- | --------------------------------------------------------------- |
| `ENABLEBUFF [Bots]`            | `EB`      | `Master` | 为指定机器人开启自动发货功能, 将会尝试自动登录到 `buff.163.com` |
| `VERIFYCODE Bot Code`          | `VC`      | `Master` | 输入手机验证码, 如果登录 Buff 要求验证手机号                    |
| `DISABLEBUFF [Bots]`           | `DB`      | `Master` | 为指定机器人开启自动发货功能                                    |
| `BUFFSTATUS [Bots]`            | `BS`      | `Master` | 查看指定机器人自动发货状态                                      |
| `UPDATECOOKIESBOT Bot Cookies` | `UCB`     | `Master` | 查看指定机器人自动发货状态                                      |
