# Restless Converters

This project provides an Html to Xaml converter in order to be able to paste (or drag and drop) information from a web site into a Rich Text Box.

The Html to Xaml converter does not attempt to completely replicate the Html. For example, it enables you to specify what an incoming **H1** element
should look like, rather than copying the style used on the web site. Other tags may be similarly customized.

## Usage

```c#
string html = GetHtml();
HtmlToXamlConverter converter = new(html);
string xaml = converter.Convert();
```

You can also supply an optional **ConverterOptions** object

```c#
string html = GetHtml();
HtmlToXamlConverter converter = new(html, new ConverterOptions()
{
    AddDefaultBlockConfigs = true,
    IsOutputIndented = true,
    SetPreserve = false
});
string xaml = converter.Convert();
```

or, if you like call chaining with static constructor

```c#
string xaml = HtmlToXamlConverter.Create(GetHtml(), new ConverterOptions()
{
    AddDefaultBlockConfigs = false,
    IsOutputIndented = false,
    SetPreserve = true,
}).Convert();
```
## Work in Progress
This is a work in progress.






