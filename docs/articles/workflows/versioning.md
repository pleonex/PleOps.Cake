# Versioning

> [!IMPORTANT]  
> The implementation of this workflow is not finished yet.

The version of the products follows a _pragmatic_ alternative to SemVer. For
that, the tool [GitVersion](https://gitversion.net/) can help to create it with
some adjustments.

Versions have three numbers: `major.minor.patch` with an optional suffix that
may be supported in some technologies as a hint.

- Upgrading to a **patch version (third digit)** should be close to a drop-in
  replacement. It has important bug fixes over the release. It does not have
  breaking changes but ABI-compatibility (DLL replacements) is not guaranteed
  (it may require to re-compile).
- Upgrading to a **minor version** should be relatively easy. It may have new
  features and bug fixes. While breaking changes should be avoided, it may have
  small ones (e.g. method name typos).
- Upgrading to a **major version** may require large changes. It may have new
  larger features and best practices causing breaking changes. Some deprecated
  methods may have been removed. This version may be incompatible with other
  software that depends on it (e.g. network protocols or serializers).

Given this definition and following the proposed [git flow](./gitflow.md), the
version assigned in each build is deterministic and comes from the git history
and current branch.

- **Development** builds:
  - Third digit: number of commits in branch
  - Suffix: `-wip.` + branch name without `feature/`.
    - If it's from a pull request (temp merge): `-pr.`
- **Preview** builds:
  - Third digit: number of commits in branch + `1000`
  - Suffix: `-preview.`
    - If it's from a release branch: `-rc.`
- **Stable** builds:
  - Third digit: number of commits in branch + `30000`
  - Suffix: none

As some [technologies](#technology-support) do not support version suffixes, we
use the third digit as an indicator of the type of build. Depending on the range
of this digit, we can know if it's a development build or ready for production.

The third digit ranges have been chosen so that we can **sort** and **compare**
version numbers alphabetically. This is important for some technologies like
NuGet that may upgrade versions. Stable builds (> 30,000) will have higher
priority over preview (> 1,000) and development (0).

```mermaid
---
title: "Build versions per commit"
config:
  theme: base
  gitGraph:
    mainBranchOrder: 2
---

gitGraph LR:
  # Start from a release
  commit id: "3.1" tag: "v3.1"

  # Feature into main
  branch feature/name order: 1
  commit id: "3.2.1-wip-name0001"
  commit id: "3.2.2-wip-name0002"
  checkout main
  merge feature/name id: "3.2.501"

  # Create release branch (LTS)
  branch release/3.2 order: 3
  commit id: "3.2.1000"

  # Hotfix after manual test failures
  branch feature/hotfix order: 4
  commit id: "3.2.1-wip-hotfix0001"
  commit id: "3.2.2-wip-hotfix0002"
  checkout release/3.2
  merge feature/hotfix id: "3.2.1001" tag: "v3.2.1001"

  # In parallel, new features to main
  checkout main
  branch feature/name2 order: 0
  commit id: "3.2.1-wip-name20001"
  commit id: "3.2.2-wip-name20002"
  checkout main
  merge feature/name2 id: "3.2.502-dev2"

  # Merge release branch to bump number
  checkout main
  merge release/3.2 id: "3.3.502-dev2"

  # Patch on LTS
  checkout release/3.2
  branch feature/hotfix2 order: 5
  commit id: "3.2.1-wip-hotfix20001"
  checkout release/3.2
  merge feature/hotfix2 id: "3.2.1002" tag: "v3.2.1002"

  # Merge hotfix to main as cherry-picks from now on
  checkout main
  commit id: "3.3.503-dev3" type: HIGHLIGHT tag: "cherry-pick hotfix2"
```

> [!NOTE]  
> This diagram assumes a _git squash_ strategy for merging branches.

## Increasing the version

GitVersion increases the `minor` number automatically after a release (git tag).
The `major` and `minor` can be defined manually at the time of creating from its
name. For instance `release/x.y` will set the version to `x.y.30000`.

The third digit is configured to increase after each commit in the branch.

This schema guarantees that at any point, rebuilding the project on a given
commit will give the same version, allowing deterministic builds.

> [!TIP]  
> Depending on your pull request review platform, you could also increase major
> or minor via commit messages. Check-out the
> [GitVersion docs](https://gitversion.net/docs/reference/version-increments#manually-incrementing-the-version)
> for more information.

## Technology support

The _pragmatic_ SemVer-based version is proved to work with the following
technologies:

- **NuGet**: uses SemVer with three digits and optional suffix
  - NuGet sorts higher non-suffixed versions. Then it compares with reverse
    alphabetical order (no suffix > rc > preview).
  - ProGet feeds does not support dots (`.`) in the suffix and long suffixes.
- **.NET assemblies**: uses a version with four digits lower or equal to 65534
  - Ignore the version suffix
- **MSI**: uses version with three digits.
  - It does not support suffixes.
  - Major and minor must be between `[0, 255]`.
  - The third digit must be between `[0, 65535]`.
- **Windows**: uses version with four digits.
  - `VERSIONINFO` and `FILEVERSION` does not allow suffixes
  - `PRODUCTVERSION` allows any value as version text.
  - Major and minor must be between `[0, 255]`, other digits between
    `[0, 65534]`.
