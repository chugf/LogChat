# 💬 LogChat — 开源免费AI聊天客户端

> 支持 ChatGPT / 讯飞星火 / DeepSeek / 语音识别 / TTS 合成 / Live2D 动画角色等，开箱即用，免环境部署！

---

## 🌈 项目预览

[![项目截图](https://github.com/user-attachments/assets/69b7c163-575e-4ccf-b0f9-0ac06dea1a16)](https://github.com/user-attachments/assets/69b7c163-575e-4ccf-b0f9-0ac06dea1a16)

---

## 🚀 支持的模型

- 🧠 ChatGPT（官方API支持）
- 🌟 讯飞星火（general / generalv2 / v3 / v3.5）
- 🔎 DeepSeek
- 🧪 ChatGLM3（开发中）
- 🐲 百度千帆大模型（计划中）
- 🖥️ Ollama（计划中）

---

## 🛠️ 技术支持组件

- 💬 LLM：大语言模型聊天系统
- 🗣️ TTS：VITS / GPT-SoVits 语音合成
- 🎙️ STT：百度语音识别（中文普通话）
- 👧 Live2D：支持角色动画、控件自定义、动作系统

---

## 🧩 架构说明

- 🖥️ UI：基于 Qt Creator 开发界面
- 🔗 接口：
  - ChatGPT API / DeepSeek API / 讯飞SDK（Windows）
  - 百度翻译 API / 百度语音识别 API
- 🔊 TTS：
  - VITS-simple-api / GPT-SoVits 接口联动
- 🧍‍♀️ Live2D：
  - Unity 驱动，支持模型导入和动画绑定

---

## 📦 快速开始

1. 解压并运行 `LogChat.exe`（请确保路径中无空格）
2. ChatGPT 使用需科学上网，或设置自定义代理 URL
3. TTS需要安装并运行 [VITS-simple-api](https://github.com/Artrajz/vits-simple-api/tree/main) 或 [GPT-SoVits](https://github.com/RVC-Boss/GPT-SoVITS)
4. 如模型不支持语言，配置 [百度翻译 API](https://fanyi-api.baidu.com/)
5. 语音识别需配置 [百度语音识别 API](https://ai.baidu.com/ai-doc/SPEECH/Jlbxdezuf)

---

## 📋 已实现功能清单

### ✅ 聊天功能
- ✅ ChatGPT 接入
- ✅ DeepSeek 支持
- ✅ 讯飞星火模型（general v1~v3.5）
- 🔜 ChatGLM3
- 🔜 百度千帆 / Ollama / 反代理等

### ✅ TTS/语音合成
- ✅ VITS-simple-api
- ✅ GPT-SoVits
- 🔜 模型情绪调节、语速调节等高级功能

### ✅ STT/语音识别
- ✅ 百度语音识别（普通话）

### ✅ Live2D 角色动画
- ✅ 模型导入
- ✅ 位置与偏移控制
- ✅ 自定义控件渲染
- ✅ 动画动作绑定
- 🔜 控件谐波支持

### 🧰 其他计划功能
- 🔜 屏幕截图（OCR提取文字）
- 🔜 时钟组件
- 🔜 快捷启动器

---

## 📚 参考与依赖

| 模块 | 链接 |
|------|------|
| 💬 ChatGPT API | [OpenAI](https://platform.openai.com/docs/api-reference/introduction) |
| 💬 DeepSeek API | [官方文档](https://platform.deepseek.com/api-docs/zh-cn/) |
| 🌟 讯飞星火 SDK | [Windows SDK 文档](https://www.xfyun.cn/doc/spark/WindowsSDK.html) |
| 🗣️ 百度语音识别 | [文档](https://ai.baidu.com/ai-doc/SPEECH/Jlbxdezuf) |
| 🌐 百度翻译 API | [文档](https://fanyi-api.baidu.com/doc/11) |
| 🔊 VITS-simple-api | [GitHub](https://github.com/Artrajz/vits-simple-api/tree/v0.2.0) |
| 🔊 GPT-SoVits | [GitHub](https://github.com/RVC-Boss/GPT-SoVITS) |
| 👧 Live2D SDK 文档 | [官方](https://docs.live2d.com/zh-CHS/cubism-sdk-manual/top/) |
| 👧 中文翻译文档 | [GitHub](https://github.com/gtf35/live2d_unity_sdk_chinese_document) |

---

## 🎨 可用 Live2D 模型资源（转载）

- [亚托莉](https://www.bilibili.com/video/BV1zg4y1b7Yu)
- [丛雨](https://www.bilibili.com/video/BV1mb4y1i7xu)
- [娅萌工作室模型](https://www.bilibili.com/video/BV1kX4y1677W)

---

## 🙌 贡献者名单

<!-- readme: collaborators,contributors -start -->
<table>
<tr>
    <td align="center">
        <a href="https://github.com/Zao-chen">
            <img src="https://avatars.githubusercontent.com/u/77674075?v=4" width="100;" alt="Zao-chen"/>
            <br />
            <sub><b>Zao_chen</b></sub>
        </a>
    </td>
    <td align="center">
        <a href="https://github.com/log159">
            <img src="https://avatars.githubusercontent.com/u/121474554?v=4" width="100;" alt="log159"/>
            <br />
            <sub><b>Log</b></sub>
        </a>
    </td>
</tr>
</table>
<!-- readme: collaborators,contributors -end -->

---

## 📬 联系与反馈

欢迎提 Issue 或 PR，也可通过 B 站私信联系开发者！

---
