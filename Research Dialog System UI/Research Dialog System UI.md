# Research Dialog System UI

This research will focus on selecting the best/most efficient UI system for the Dialog System of A Passing Friend.



## Contents

[TOC]



## I: Candidate UI Systems

There a a few UI Systems that I have selected as candidates. Below they will be measured against our selected criteria.

First however, the candidates:

| The Candidates      | Links                                               |
| ------------------- | --------------------------------------------------- |
| **Unity UI (UGUI)** | https://docs.unity3d.com/Manual/com.unity.ugui.html |
| **UI Toolkit**      | https://docs.unity3d.com/Manual/UIElements.html     |
| **IMGUI**           | https://github.com/ocornut/imgui                    |
| **Delight**         | https://delight-dev.github.io/                      |



And then the criteria:

| **Type of UI**          | **UI Toolkit** | **Unity UI (UGUI)** | **IMGUI**       | **Delight** | **Notes**                                                    |
| :---------------------- | :------------- | :------------------ | :-------------- | ----------- | :----------------------------------------------------------- |
| **2022.1.14f1 support** | ✔              | ✔                   | ✔               | ✔           | This refers to the 2022.1.14f1 version of Unity, if that wasn't already obvious. |
| **Runtime (debug)**     | ✔ *            | ✔                   | ✔               | ✔           | This refers to temporary runtime UI used for debug purposes. |
| **Runtime (in-game)**   | ✔ *            | ✔                   | Not Recommended | ✔           | For performance reasons, Unity does not recommend IMGUI for in-game runtime UI. |
| **Unity Editor**        | ✔              | ❌                   | ✔               | ❌           | You cannot use Unity UI to make UI for the Unity Editor.     |
| Learning Difficulty     |                |                     |                 |             | The difficulty of learning to use the UI System. This will be for those with prior programming/unity knowledge. |





## II: Source List

[Comparison UI systems]: https://docs.unity3d.com/2020.2/Documentation/Manual/UI-system-compare.html	"Comparison UI Systems"
[UI Toolkit]: https://docs.unity3d.com/Manual/UIToolkits.html	"UI Toolkits"
[UGUI To UI Toolkit]: https://docs.unity3d.com/Manual/UIE-Transitioning-From-UGUI.html	"UGUI To UI Toolkit"
[UI Toolkit]: https://docs.unity3d.com/Manual/UIElements.html	"UI Toolkit"
[5 best Unity UI kits]: https://www.tldevtech.com/best-unity-ui-kits/	"5 best Unity UI kits"
[Delight]: https://delight-dev.github.io/	"Delight"
[IMGUI]: https://github.com/ocornut/imgui	"IMGUI"
