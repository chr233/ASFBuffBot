# ASFBuffBot

Временная проблема с десериализацией, плагин может работать некорректно，请使用自编译版本的 ASF

Временная проблема с десериализацией, плагин может работать некорректно，请使用自编译版本的 ASF

Временная проблема с десериализацией, плагин может работать некорректно，请使用自编译版本的 ASF

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

[中文说明](README.md) | [English Version](README.en.md)

## Установка

### Первая установка / Обновление в ручном режиме

1. Загрузите плагин через страницу [GitHub Releases](https://github.com/chr233/ASFBuffBot/releases)
2. Распакуйте файл `ASFBuffBot.dll` и скопируйте его в папку `plugins` в директории `ArchiSteamFarm`
3. Перезапустить `ArchiSteamFarm` , и используйте команду `ASFBUFFBOT` или `ABB` для проверки работоспособности плагина

### Команды для обновления

> Для обновления плагина можно использовать собственную команду плагина
> Обновление версии ASF может быть несовместимым, если вы обнаружили, что плагин не может быть загружен, попробуйте обновить ASF

- `ABBVERSION` / `ABBV` проверить последнюю версию ASFBuffBot
- `ABBUPDATE` / `ABBU` автоматическое обновление ASFBuffBot (возможно, потребуется обновить ASF вручную)

### 更新日志

| Версия ASFBuffBot                                                    | овместимая версия ASF | Описание                                                                                   |
| -------------------------------------------------------------------- | :-------------------: | ------------------------------------------------------------------------------------------ |
| [1.1.2.3](https://github.com/chr233/ASFBuffBot/releases/tag/1.1.0.0) |       5.5.0.11        | ASF -> 5.5.0.11                                                                            |
| [1.1.1.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.1.0.0) |       5.4.10.3        | ASF -> 5.4.10.3                                                                            |
| [1.1.0.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.1.0.0) |        5.4.8.3        | 适配手机验证码登录, 支持存储 `Cookies`, 储存文件格式修改, 需要重新设定启用自动发货的机器人 |
| [1.0.8.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.8.0) |        5.4.5.2        | Добавлен параметр `RejectNotMatch`.                                                        |
| [1.0.7.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.7.0) |        5.4.5.2        | Добавлена команда `UPDATECOOKIESBOT` для ручной установки cookie бота                      |
| [1.0.6.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.6.0) |        5.4.5.2        | Исправления ошибок                                                                         |
| [1.0.5.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.5.0) |        5.4.5.2        | Поддержка автологина Buff, исправлены ошибки                                               |
| [1.0.4.1](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.4.1) |        5.4.5.2        | Поддержка нескольких аккаунтов, исправление ошибок                                         |
| [1.0.0.0](https://github.com/chr233/ASFBuffBot/releases/tag/1.0.0.0) |        5.4.4.5        | Первая версия, режим работы с одним аккаунтом                                              |

## Конфигурация плагина

> Настройка этого плагина не требуется, большинство функций доступно в настройках по умолчанию

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

| Конфигурация        | Тип    | По умолчанию | Описание                                                                                                                                                                                    |
| ------------------- | ------ | ------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Statistic`         | bool   | `true`       | Разрешить отправку данных для статистики. Она используется для подсчета количества пользователей, при этом никакой другой информации отправляться не будет                                  |
| `BuffCheckInterval` | int    | `180`        | 每一轮 Buff 发货检查的周期, 单位秒, 访问频率过快容易被 ban                                                                                                                                  |
| `BotInterval`       | int    | `30`         | 在一轮发货检查中每个机器人的检查间隔, 单位秒                                                                                                                                                |
| `CustomUserAgent`   | string | `null`       | Пользовательский `User-Agent` для отправки запросов в Buff                                                                                                                                  |
| `RejectNotMatch`    | bool   | `false`      | Следует ли автоматически отклонять сделку при несоответствии между торгуемыми предметами                                                                                                    |
| `AlwaysSendSmsCode` | bool   | `false`      | Установите значение `true`, чтобы заставить Buff отправлять код проверки мобильного телефона при каждом входе в систему, а значение `false` - чтобы отправлять код только при необходимости |

> Функциональность отключения команд была перенесена в плагин `ASFEnhance`
> Когда команда отключена, ее можно по-прежнему вызывать как `ABB.xxx`, например, `ABB.UPDATECOOKIES`

## Использование Команд

### Команды Обновления

| Команда      | Сокращение | Доступ          | Описание                                                                  |
| ------------ | ---------- | --------------- | ------------------------------------------------------------------------- |
| `ASFBUFFBOT` | `ABB`      | `FamilySharing` | Получить версию ASFBuffBot                                                |
| `ABBVERSION` | `ABBV`     | `Operator`      | Проверить последнюю версию ASFBuffBot                                     |
| `ABBUPDATE`  | `ABBU`     | `Owner`         | Обновить ASFBuffBot до последней версии (необходим ручной перезапуск ASF) |

### Функциональное Управление

| Команда                        | Сокращение | Доступ   | Описание                                                                                                         |
| ------------------------------ | ---------- | -------- | ---------------------------------------------------------------------------------------------------------------- |
| `ENABLEBUFF [Bots]`            | `EB`       | `Master` | Включите автовход для конкретного бота, он будет пытаться войти на `buff.163.com` автоматически                  |
| `VERIFYCODE Bot Code`          | `VC`       | `Master` | Введите код мобильного телефона, если вы вошли в систему, Buff запросит подтверждение номера мобильного телефона |
| `DISABLEBUFF [Bots]`           | `DB`       | `Master` | Включение автовыхода для определенных ботов                                                                      |
| `BUFFSTATUS [Bots]`            | `BS`       | `Master` | Проверка статуса данного бота                                                                                    |
| `UPDATECOOKIESBOT Bot Cookies` | `UCB`      | `Master` | 查看指定机器人自动发货状态(неверное оригинальное описание)                                                       |
