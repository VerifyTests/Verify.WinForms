# <img src="/src/icon.png" height="30px"> Verify.WinForms

[![Discussions](https://img.shields.io/badge/Verify-Discussions-yellow?svg=true&label=)](https://github.com/orgs/VerifyTests/discussions)
[![Build status](https://ci.appveyor.com/api/projects/status/lgjcs6xhxhhw0f02?svg=true)](https://ci.appveyor.com/project/SimonCropp/verify-winforms)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.WinForms.svg)](https://www.nuget.org/packages/Verify.WinForms/)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow verification of WinForms UIs.

**See [Milestones](../../milestones?state=closed) for release notes.**

## NuGet package

https://nuget.org/packages/Verify.WinForms/


## Usage

<!-- snippet: Enable -->
<a id='snippet-enable'></a>
```cs
[ModuleInitializer]
public static void Init() =>
    VerifyWinForms.Initialize();
```
<sup><a href='/src/Tests/ModuleInit.cs#L3-L9' title='Snippet source file'>snippet source</a> | <a href='#snippet-enable' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Form

A visual element (Form/Control etc) can be verified as follows:

<!-- snippet: FormUsage -->
<a id='snippet-formusage'></a>
```cs
[Test]
public Task FormUsage() =>
    Verify(new MyForm());
```
<sup><a href='/src/Tests/TheTests.cs#L6-L12' title='Snippet source file'>snippet source</a> | <a href='#snippet-formusage' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

With the state of the element being rendered as a verified file:

[TheTests.FormUsage.Net.verified.png](/src/Tests/TheTests.FormUsage.Net.verified.png):

<img src="/src/Tests/TheTests.FormUsage.Net.verified.png" width="200px">


### ContextMenuStrip

A `ContextMenuStrip` can be verified as follows:

<!-- snippet: ContextMenuStrip -->
<a id='snippet-contextmenustrip'></a>
```cs
[Test]
public Task ContextMenuStrip()
{
    var menu = new ContextMenuStrip();
    var items = menu.Items;

    items.Add(new ToolStripMenuItem("About"));
    items.Add(new ToolStripMenuItem("Exit"));
    return Verify(menu);
}
```
<sup><a href='/src/Tests/TheTests.cs#L22-L35' title='Snippet source file'>snippet source</a> | <a href='#snippet-contextmenustrip' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

With the state of the element being rendered as a verified file:

[TheTests.FormUsage.Net.verified.png](/src/Tests/TheTests.ContextMenuStrip.Net.verified.png):

<img src="/src/Tests/TheTests.ContextMenuStrip.Net.verified.png" width="200px">


## OS specific rendering

The rendering of Form elements can very slightly between different OS versions. This can make verification on different machines (eg CI) problematic. There are several approaches to mitigate this:

 * Using a [custom comparer](https://github.com/VerifyTests/Verify/blob/master/docs/comparer.md)



## Icon

[Gem](https://thenounproject.com/term/gem/2247823/) designed by [Adnen Kadri](https://thenounproject.com/adnen.kadri/) from [The Noun Project](https://thenounproject.com).
