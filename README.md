# Git Ez Tag

Interactive CLI Tool for adding and pushing Git Tags easily.

```
Usage: ez-tag [options]

Options:
  -p|--push                          Executes 'git push --tags' after adding the Tag
  -i|--increase <Major|Minor|Patch>  Auto increase the given part of the version
  -l|--lightweight                   Skip Tag annotation
  -?|-h|--help                       Show help information
```

## What does it?

This dotnet global tool implements an opinionated way of adding and pushing Git Tags.
Based on your latest Tag, it suggest your next one with an increased **minor** element*.

\* This only works, if your latest Tag matches the simplified semantic versioning pattern of `Major.Minor.Patch`

## Usage

```
$ ez-tag
[Information] Found Repository at 'C:\Development\MyProject'.
[Information] Latest Tag is '1.1.0'
> What's your next Tag? [1.2.0]
> What's your annotation for Tag '1.2.0'? [1.2.0]
[Information] Added tag '1.2.0'
> Push Tags? [Y/n]
```

## Used Git commands
Here's a list of the used Git commands.
**Don't** use this tool, if they don't fit your workflow.


Is the repository dirty?
```
diff --no-ext-diff --quiet --exit-code
```

Get the latest Tag.
```
git describe --abbrev=0
```

Add lightwight Tag
```
git tag TAGNAME
```

Add annotated Tag
```
git tag -a TAGNAME -m ANNOTATION
```

Push Tags
```
git push --tags
```