# Repository guidance

This file applies to the entire repository. Read the root `README.md` and
nearby implementation and tests before making changes.

## Solution layout

- `brows-win32-previewhandler.slnx` contains one package project, its test
  project, and a WPF sample application.
- `README.md` documents installation, the public control contract, and XAML
  usage. It is included in the NuGet package; there are no project READMEs.
- `source/Brows.Win32.PreviewHandler.Windows/` contains the WPF control and
  native integration. `Windows/Controls/PreviewHandler.xaml` and its code-behind
  define the public control; `PreviewHwndHost.xaml` and its code-behind host the
  native child window. `Windows/PreviewHandlerCLSIDChangedEvent.cs` defines the
  public CLSID event types.
- In that source project, `Win32/PreviewWorker.cs` coordinates preview operations
  on an STA worker, `PreviewWorkerCLSID.cs` resolves registered handlers, and
  `PreviewHandlerWrapper.cs` activates and initializes COM handlers.
  `Win32/PlatformInvoke/` contains native declarations and structures;
  `Win32/InteropServices/` contains the COM wrapper and `ComTypes/` interfaces.
- `tests/Brows.Win32.PreviewHandler.Windows.Tests/` contains NUnit tests for
  control defaults, event arguments, wrappers, and interop helpers.
- `samples/Brows.Win32.PreviewHandler.Windows.Sample/` contains the runnable
  WPF sample and references the source project.
- `Directory.Build.props` and `Directory.Packages.props` hold shared build
  settings and centrally managed package versions. The `source/`, `tests/`, and
  `samples/` build properties import root settings and add group settings.
  `global.json` selects the SDK, and `.github/workflows/workflow.yml` defines CI.

The root namespace is `Brows`; the public control is
`Brows.Windows.Controls.PreviewHandler`. Check project metadata when working on
package identity: the project name (and default package ID) is
`Brows.Win32.PreviewHandler.Windows`, while the README currently uses
`Brows.Win32.Windows.PreviewHandler`.

## Preserve existing work

- When working in a Git repository, inspect its status and existing diffs before
  editing. Preserve existing work, avoid reverting unrelated changes, and keep
  changes scoped to the requested task.
- Do not commit generated build or test output, packages, or IDE state unless the
  task specifically requires them.

## Build and test

Identify the solution, affected projects, and target frameworks before building.
Use the SDK selected by `global.json` when present, and the operating system,
targeting packs, and runtimes required by the affected projects.

Build and test on Windows with WPF support, the .NET Framework targeting packs
for `net462` and `net48`, and the runtimes for `net8.0-windows` and
`net10.0-windows`. All three projects inherit these four target frameworks.
`global.json` requests SDK `10.0.0` with `rollForward: feature`; C# 14 is configured
in `Directory.Build.props`. Use these commands from the repository root:

```powershell
dotnet restore brows-win32-previewhandler.slnx
dotnet build brows-win32-previewhandler.slnx --no-restore --configuration Release
dotnet test brows-win32-previewhandler.slnx --no-restore --configuration Release --no-build
dotnet pack brows-win32-previewhandler.slnx --no-restore --configuration Release --no-build
```

For focused iteration, select the test project, framework, and NUnit fixture:

```powershell
dotnet test tests/Brows.Win32.PreviewHandler.Windows.Tests/Brows.Win32.PreviewHandler.Windows.Tests.csproj `
  --configuration Release --framework net10.0-windows --filter FullyQualifiedName~PreviewHandlerTests
```

For hosting, resizing, or preview lifecycle changes, also run the sample on
Windows and exercise it with a file type that has a registered handler:

```powershell
dotnet run --project samples/Brows.Win32.PreviewHandler.Windows.Sample `
  --configuration Release --framework net10.0-windows
```

Preview availability depends on installed handlers and process architecture.
Record the handler/file type and architecture used for manual checks. Only the
source library is packable; the sample and tests inherit `IsPackable=false`.
Build artifacts are routed to the root `out/` directory.

For code changes, run relevant tests and build all affected target frameworks.
Use full solution validation when changing shared build settings, dependencies,
or behavior spanning projects. Documentation-only changes need a content and
diff review. Report the checks performed and any validation that could not run.

For efficient testing, run the affected test first, then its fixture or project,
before broader validation required by the change. Confirm that a regression test
fails because of the reported bug, rather than a build or environment problem.
Use `--no-build` only when the binaries include the latest code changes.

## WPF and native integration

- Keep WPF control access on its dispatcher and COM preview operations serialized
  through the existing single-worker `STAThreadPool`. Avoid blocking the UI
  thread while awaiting preview work.
- Preserve cancellation and request identity checks in the control and worker.
  Older requests must not overwrite newer preview state or unload newer handlers.
  Check clear, disable, refresh, and unload paths when changing this lifecycle.
- Keep handler unload and COM resource release reliable on success, failure,
  cancellation, and control unload. Preserve ownership of native pointers,
  streams, shell items, and the child HWND.
- Preserve the stream, file, and shell-item initialization fallbacks unless the
  task changes handler compatibility. The package consumes registered handlers;
  it does not install or register them.
- Preserve the `PART_HwndHost` template part and physical-pixel conversion when
  changing the control template or resizing behavior. WPF sizes are device
  independent; the native handler receives a pixel `RECT`.
- Treat dependency properties, read-only property keys, routed events, and the
  XAML namespace mappings in `Properties/AssemblyInfo.cs` as public contracts.
  Keep `README.md` and sample usage aligned with contract changes.
- For interop changes, preserve native layout, pointer widths, marshaling,
  interface GUIDs, COM method order, and HRESULT handling. Keep declarations in
  `PlatformInvoke/` and `InteropServices/ComTypes/` rather than duplicating them
  in control code.

## Code and project conventions

- Follow `.editorconfig`: four spaces for C#; two spaces for
  project XML, XAML, JSON, and Markdown. C# and project/XAML files use UTF-8 with
  BOM; Markdown uses UTF-8 without BOM. Keep C# and XAML lines within the
  configured 120-column guideline.
- Use file-scoped namespaces and opening braces on the same line. Follow the
  configured C# style and nearby code. `.editorconfig` prefers explicit local
  types, while existing implementation and tests frequently use `var`; avoid
  unrelated rewrites to reconcile that difference.
  Use PascalCase for types and members and an `I` prefix for interfaces.
- Never prefix type names with their namespace. Always use `using` statements
  to import namespaced types.
- Method parameters of type `CancellationToken` must always come last in the
  parameter list.
- Use the configured C# language version. APIs must be available on every
  framework targeted by the affected project. Preserve framework-specific
  conditions and compatibility shims; a newer language version does not supply
  newer runtime APIs.
- Check shared build settings and global usings before adding project-specific
  settings or imports. Preserve existing build-property import structures.
- When central package management is configured, manage dependency versions in
  `Directory.Packages.props` and omit versions from project package references.
  Keep dependency changes focused on the task.
- Do not add to the public API unless explicitly instructed to do so. Keep new
  types and members internal or private when the requested change permits it.
- Declare new implementation classes `sealed` or `abstract` where appropriate.
  Preserve WPF partial declarations and the public control's inheritance contract;
  do not seal public controls as incidental cleanup.
- Prefer immutable types. When state mutation is required, keep it private. Prefer
  read-only properties supplied through constructors and init-only properties.
  Pure data objects should be `record` types.
- Put each top-level type in a separate file named after the type. Keep closely
  related helper types nested with their owning type when that matches the
  existing design.
- Namespaces must match the directory structure. The namespace of a type is the
  project's root namespace combined with the folders that contain the type's
  file, relative to the project root. Preserve the test namespace described below
  and assembly-level declarations in `Properties/AssemblyInfo.cs`.
- Update public API documentation, relevant READMEs, and usage examples when
  changing the documented contract. Do not add XML documentation comments to
  private or internal types or members.
- Keep changes scoped. Avoid unrelated formatting, framework, package-version,
  or release-workflow changes. Do not commit generated build/test output,
  packages, or IDE state such as `out/`, `bin/`, `obj/`, and `.vs/`.

Within a C# type, use this member order, omitting items that do not exist:

1. Static constructor.
2. Private constants.
3. Private fields.
4. Private events.
5. Private properties.
6. Private instance constructors.
7. Private methods.
8. Internal constants.
9. Internal events.
10. Internal properties.
11. Internal instance constructors.
12. Internal methods.
13. Protected constants.
14. Protected events.
15. Protected properties.
16. Protected instance constructors.
17. Protected methods.
18. Public constants.
19. Public fields, only where interop layout or framework conventions require them.
20. Public events.
21. Public properties.
22. Public instance constructors.
23. Public methods.
24. Explicit interface implementations, after everything else.

### Properties

Prefer compiler-generated backing fields. C# 14 is enabled; use `field` when
custom property logic can use an automatic backing field, as in `PreviewWorker`.
Keep accessors without custom logic auto-implemented. Validate setter input before
assigning it to `field`. WPF dependency-property wrappers must continue to use
`GetValue` and `SetValue`, including read-only dependency-property keys.

For synchronization fields, use `System.Threading.Lock` when the target
framework is `net9.0` or greater. In multi-targeted code, use
`NET9_0_OR_GREATER` to select `Lock` for those targets and `object` for earlier
targets.

Use a manually declared field when an automatic backing field would make the
code more complicated, such as when the field must be `volatile`.

### C# style

- Follow `.editorconfig` and nearby code: four spaces for C#, opening braces on
  the same line, and CRLF line endings.
- Use block-bodied methods and constructors with opening and closing braces. Do
  not use expression-bodied methods (`=>`). Expression-bodied properties and
  accessors are fine when they keep the property clear.
- Always use braces around statement bodies, including `if`/`else`, `do`, `for`,
  `foreach`, `while`, `using`, and `lock`, even when the body is one statement.
- Keep simple conditions inline. For compound or complex `if` or `while` conditions,
  calculate a descriptively named `bool` variable beforehand; update or recompute it
  explicitly inside loops.

  > Examples of acceptable and unacceptable conditions:

  ```csharp
  /*
   * These simple conditions are acceptable inline.
   */
  if (arg is null) { }
  if (num >= 0) { }
  if (string.IsNullOrEmpty(str)) { }

  /*
   * Calculate compound conditions beforehand and give them a descriptive name.
   */
  var inputIsInvalid = input is null || number < 0 || string.IsNullOrEmpty(text);
  if (inputIsInvalid) { }
  ```

- Prefer early `return` over `else` blocks.
- Check reference values for null with `is null` and `is not null`. Prefer these
  patterns over `== null` and `!= null`.
- Prefer `/* ... */` block comments, with ` * ` at the start of each interior
  line. Use `//` only for a single-line clarification; align a continuation line
  with the comment when needed.
- In XML documentation, put each element's opening tag, content, and closing tag
  on separate lines. Document public APIs; do not add XML documentation comments
  to private or internal members.
- Use PascalCase for types and members, and prefix interfaces with `I`. Follow
  the member ordering above and local declaration style. Prefer private
  `readonly` fields where possible, keep fields non-public, and put each type in
  a separate file named after the type.

## Tests

The single test project references the source project, whose
`Properties/AssemblyInfo.cs` grants it access to internal members. NUnit and Moq
are available through `tests/Directory.Build.props`, which also imports their
global usings.

Existing fixtures use the `Tests` suffix and the namespace
`Brows.Win32.PreviewHandler.Windows.Tests`. `PreviewHandlerTests.cs` groups
control and preview-related tests; `PlatformInvokeTests.cs` currently contains
`HRESULTExtensionTests`, `LOGFONTWTests`, and `IIDTests`. Follow these conventions;
new standalone fixture files should be named after their fixture.

- Mark WPF control fixtures with `[Apartment(ApartmentState.STA)]`, as in
  `PreviewHandlerTests`. Use dispatcher-aware coordination when testing
  asynchronous control behavior.
- Keep automated tests independent of third-party preview handlers and registry
  changes. Existing wrapper tests deliberately avoid COM activation; use the
  sample for checks requiring a real installed handler.
- Use NUnit constraint assertions (`Assert.That`, `Is`, and related constraints)
  and the existing exception assertion style. Use Moq where mocking is useful.
- Use the test framework, assertion style, and helpers already used by the affected
  projects.
- Add regression coverage for behavior changes. Confirm that a regression test
  fails because of the reported defect, rather than because of a build or
  environment problem.
- Prefer explicit task signals with bounded waits over timing-dependent sleeps.
- Keep tests and shared test helpers compatible with all applicable target
  frameworks.

## Code reviews

When asked to review code, inspect the entire requested scope thoroughly and
identify all bugs and issues you can find. Write the findings to `Review.md` in
the directory `_agents` in the repository root. Number each issue so it can be
referenced later. For every issue, describe the problem and a way to resolve it.
Cite specific file names and line numbers where applicable. Review existing
findings in `Review.md` before editing it so relevant issues are retained or
updated without duplication. Keep review issue numbers stable: never renumber
existing issues or reuse their numbers, including those marked resolved. However,
do not search the Git history for a `Review.md` if one does not exist: in that
case, simply create a new `Review.md` and start with issue #1. Assign each new
issue a number greater than the highest issue number already used in `Review.md`.

> The full path to `Review.md` should be `{root}/_agents/Review.md`.

When asked to fix issues from `Review.md`, work only on the issue numbers
specified in the request. Prefer red/green testing: first add a test that fails
because of each issue, then fix the code, then rerun the previously failing test
to confirm it passes. Once an issue is resolved, keep it in `Review.md` and mark
it as resolved in that issue's heading; do not remove it.

If worktrees are explicitly requested for fixing multiple issues, create a
separate Git worktree for each requested issue number. Place each worktree
directory directly in the parent directory of the original repository root,
as a sibling of that repository. Name it
`_agent-{original-directory-name}-{issue-number}`, where
`original-directory-name` is the name of the original repository's root directory.
Name each worktree branch
`agent/{issue-number}/{short-description}`. Fix, test, and update `Review.md`
for that issue in its own worktree. Commit each issue's changes on
its worktree branch. The first line of the commit message must start with
`Fix #{issue-number}.`, followed by a blank line and a description of what was
broken and how it was fixed. Once the commit is ready, notify the requester
that the worktree branch is ready to be merged.

> The `{short-description}` in branch names should be hyphenated, for example,
> `agent/5/validate-arguments`.

Before reporting completion, review the final diff for unintended changes,
whitespace problems, and accidental public API additions. Check new and untracked
files as well as tracked changes, and confirm that the changes match the request.

Report completion consistently: summarize what changed, list resolved issue
numbers when applicable, and state which tests or checks ran and their results.
Identify any checks that could not run. For worktree fixes, include the branch
name and commit hash that are ready to be merged.

## Planning

When asked to plan code changes, write your plan to `Plan-{short-description}.md`
in the directory `_agents` in the repository root. The `{short-description}` should
be PascalCase, with no spaces, dashes, or underscores in the string.

> For example, the full path to a plan for updating security code would be
> `{root}/_agents/Plan-SecurityUpdate.md`

Your plan should include enough information for another agent with lesser capability
to follow it. Include in the plan the means of accomplishing tasks: you will start
subagents for any tasks that can be logically separated from others, and you will
review their work as it completes and merge their changes into your main worktree
branch for the updates.
