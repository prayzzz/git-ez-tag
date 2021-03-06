# git-ez-tag

![Nuget](https://img.shields.io/nuget/v/GitEzTag.svg?style=flat-square)

Interactive CLI Tool for adding and pushing Git Tags easily.

_This tool is currently under development. Name, options and behavior are suspect to change._

```
dotnet tool install --global GitEzTag
```

## What does it?

This dotnet global tool implements an opinionated way of adding and pushing Git Tags.
Based on your latest Tag, it suggest your next one with an increased **minor** element*. 
Also, the name of the Tag is suggested as message, because it's required for annotated Tags.

\* This only works, if your latest Tag matches the simplified semantic versioning pattern of `Major.Minor.Patch`

## Usage

```
Usage: ez-tag [options]

Options:
  -p|--push                          Pushes the added Tag
  -i|--increase <Major|Minor|Patch>  Auto increase the given part of the version
  -l|--lightweight                   Skip Tag annotation
  -?|-h|--help                       Show help information
```

**Sample Output**

```
$ ez-tag
[INFO] Found Repository at 'C:\Development\MyProject'.
[INFO] Latest Tag is '1.1.0'
> What's your next Tag? [1.2.0]
[INFO] New Tag is '1.2.0'
> What's your message for Tag '1.2.0'? [1.2.0]
[INFO] Added tag '1.2.0'
> Push Tag '1.2.0' now? [Y/n]
[INFO] Pushed 'refs/tags/1.2.0' to origin

```

## Used Git commands
Here's a list of the used Git commands.
**Don't** use this tool, if they don't fit your workflow.


Is the repository dirty?
```
git diff --no-ext-diff --quiet --exit-code
```

Get the latest Tag.
```
git describe --abbrev=0
```

Add lightwight Tag.
```
git tag TAGNAME
```

Add annotated Tag.
```
git tag -a TAGNAME -m ANNOTATION
```

Push added Tag.
```
git push -u origin refs/tags/TAGNAME
```
