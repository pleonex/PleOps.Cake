# Git flow

Following git flow style, there would be three types of branches:

- **main** (aka `develop`). It's the current development branch with latest
  features and fixes.
  - This branch should be protected. Changes come via _pull requests_ from
    feature branches.
  - Building creates a _preview_ build. Artifacts are deployed to the preview /
    staging feeds.
- **feature branches**: development branches for small features and bug fixes.
  - Prefixed always with `feature/` for simplicity.
  - Building creates a _development_ build (same as local builds). No artifacts
    are deployed, but they are accessible to download and test from CI output.
- **release branch** (optional): branch to work on the release process of a
  release without stopping development in the _main_ branch.
  - If the product provides _long-term support (LTS)_, use this branch to
    maintain patches for the version.
  - As an alternative for small projects, just tag directly `main`. You can
    always check-out a _tagged commit_, branch and create a _patch release_ from
    there.

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

> [!TIP]  
> Merging pull requests with the _squash_ strategy could be a good idea as it
> keeps a clean git history. The only exception is the first merge from the
> release branch into the main.

<!-- warning ignore -->
