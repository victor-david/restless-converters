# Restless Converters

[![Nuget](https://img.shields.io/nuget/v/Restless.Converters.svg?style=flat-square)](https://www.nuget.org/packages/Restless.Converters/) 

This project provides an Html to Xaml converter and a custom paste handler that enables you to copy/paste (or drag/drop) information from a web site 
into the FlowDocument of a Rich Text Box.

The Html to Xaml converter does not attempt to completely replicate the incoming html. Instead, it enables you to specify (for instance) the appearance of an incoming **H1** element, 
rather than copying the style used on the web site. Other tags may be similarly customized.

## Usage

```c#
string html = GetHtml();
string xaml = HtmlToXamlConverter.Create().SetHtml(html).Convert();
```

You can also supply an optional **ConverterOptions** object

```c#
string html = GetHtml();
string xaml = HtmlToXamlConverter.Create(new ConverterOptions()
{
    AddDefaultBlockConfigs = true,
    IsOutputIndented = true,
    SetPreserve = false
}) .SetHtml(html).Convert();
```

## Converter Options
Converter options affect how the xaml is generated.

| Property | Type | Description |
| --- | --- | --- |
| SectionConfig | BlockConfig | Provides access to the configuration that is applied to Xaml Section nodes |
| AddDefaultBlockConfigs | bool | Determines whether default block configurations are applied. Defaults include settings for **H1**, **H2**, **UL**, **OL**, and others. You can change any defaults before conversion or supply your own, or both. |
| ProcessUnknown | bool | Determines whether unknown nodes are processed. When an unknown node is processed, it appears in the xaml with its name and inner text. The default is **false**. Mostly a debugging aide. |
| SetPreserve | bool | Determines whether the xaml output has **xml:space preserve** added. The default is **false**. |
| IsOutputIndented | bool | Determines whether xaml output is indented. When **false**, xaml is all on one long line. When **true**, the xaml is broken up into lines, indented, and easier to read. The default is **false**. |

## Paste Handler
Paste handler provides a way to register controls for custom paste handling. 

