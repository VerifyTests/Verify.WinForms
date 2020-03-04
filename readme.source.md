# <img src="/src/icon.png" height="30px"> Verify.Xaml

[![Build status](https://ci.appveyor.com/api/projects/status/o2iy3b7k9le0ntps?svg=true)](https://ci.appveyor.com/project/SimonCropp/verify-winforms)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.WinForms.svg)](https://www.nuget.org/packages/Verify.WinForms/)

Extends [Verify](https://github.com/SimonCropp/Verify) to allow verification of WinForms UIs.

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-verify.winforms?utm_source=nuget-verify.winforms&utm_medium=referral&utm_campaign=enterprise).

toc


## NuGet package

https://nuget.org/packages/Verify.WinForms/


## Usage

Enable VerifyXaml once at assembly load time:

snippet: Enable

A visual element (Form/Control etc) can then be verified as follows:

snippet: FormUsage

With the state of the element being rendered as a verified file:

[TheTests.FormUsage.verified.png](/src/Tests/TheTests.FormUsage.verified.png):

<img src="/src/Tests/TheTests.FormUsage.verified.png" width="200px">


## OS specific rendering

The rendering of Form elements can very slightly between different OS versions. This can make verification on different machines (eg CI) problematic. There are several approaches to mitigate this:

 * Using a [custom comparer](https://github.com/SimonCropp/Verify/blob/master/docs/comparer.md)


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

[Gem](https://thenounproject.com/term/gem/2247823/) designed by [Adnen Kadri](https://thenounproject.com/adnen.kadri/) from [The Noun Project](https://thenounproject.com/creativepriyanka).