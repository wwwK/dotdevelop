MonoDevelop is always looking for contributors to help with development. If you have some time and want to help, there are instructions below.

### Reporting Bugs

Filing bugs is a very easy way to help contribute to any project. See the [Reporting Bugs](https://github.com/mono/monodevelop/wiki/Reporting-Bugs) guide for more info.

### Submitting Patches

Patches have to be submitted by using the Pull Request mechanism that github provides. Here is a summary of how it works:

1.  If you don't already have one, you have to create your own fork of the MonoDevelop repository. You can do it using the Fork command available in the [main page](http://github.com/mono/monodevelop) of the project in github.
2.  Make and commit your changes in your forked repository.
3.  In the main page of your forked repository, click on the Pull Request command.
4.  In the Pull Request dialog, enter a descriptive comment of the change.

### Commit Rules

There are a few rules to follow when committing code:

1.  Enter a meaningful explanation in the commit log message.
2.  The first line of the commit message should be a short (ideally \<50 columns) message, followed by a blank line. The rest of the commit message could contain a more detailed explanation of the change, wrapped to 80 columns.
3.  When entering a message describing a change, take into account that the goal is to explain **why** the change was done, rather than **what** was changed. For example, the message "Added property X" is in general useless since it is obvious looking at the patch.
4.  All changes included in a commit should be related and as atomic as possible. Here are some examples:
    -   You do an API change that affects several projects across the source tree. All changes can go in a single commit.
    -   You implement a new feature A, for example, in MonoDevelop.Projects. Then you implement another new feature B that uses A in MonoDevelop.Ide. Although those changes are related (B needs A), they should be committed separately, since they belong to two different features.

6.  Don't do reformatting commits, unless you're the original author of the code. This includes changing line endings.
7.  Never remove copyright notices from the code.
8.  Never remove licensing info from code.
9.  Never commit code you didn't write yourself or code that doesn't have a suitable license.

### Coding Guidelines

New code written for MonoDevelop should follow the generic [Mono Coding Guidelines](http://www.mono-project.com/Coding_Guidelines), with the following exceptions:

-   use camelCase for field names
-   code must be indented using tabs, not spaces, and tabs have a width equal to 4 spaces
-   the opening brace { of a class or namespace should be put on a new line, like methods
-   lines should be wrapped to 120 chars, not 80

The MonoDevelop solution is configured with formatting policies that conform to these guidelines.

Public/protected API should be documented using XML doc comments, but keep them concise and useful. If they're generic/obvious and don't add any value, for example a constructor with a summary "create a new instance of Foo", they should be omitted.

It's recommended to follow the .NET Framework Design Guidelines. If you do not have the excellent and detailed [book](http://www.amazon.com/dp/0321545613), there is also a free [digest](http://blogs.msdn.com/b/kcwalina/archive/2008/04/09/fdgdigest.aspx) and a section on [MSDN](http://msdn.microsoft.com/en-us/library/ms229042.aspx). There are also some [docs](http://msdn.microsoft.com/en-us/library/czefa0ke(v=vs.71).aspx "http://msdn.microsoft.com/en-us/library/czefa0ke(v=vs.71).aspx") for .NET 1.1 that mostly remain applicable to newer versions of the framework.

### A Note on Licensing

All new code and modifications to existing code should be licensed under [MIT X11](http://www.opensource.org/licenses/mit-license.php). The MS-PL and Apache 2 licenses are acceptable for new libraries and new addins.

Code and libraries licensed under version 2 of the [LGPL](http://www.opensource.org/licenses/lgpl-license.php) license are acceptable under extraordinary circumstances.

Code licensed under the [GPL](http://www.opensource.org/licenses/gpl-license.php) cannot be accepted.

Although addins that do not follow these rules cannot be accepted into the MonoDevelop source repository, they can be hosted elsewhere. However, this generally means that they are much less likely to be maintained and distributed.