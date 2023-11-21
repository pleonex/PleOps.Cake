# Git flow

Following git flow style, there are three types of branches: main, feature and
release branches.

```mermaid
---
title: "Git flow"
config:
  theme: base
  gitGraph:
    showCommitLabel: false
    mainBranchOrder: 2
---

gitGraph LR:
  commit tag: "Preview"

  # Feature into main
  branch feature/name order: 1
  commit tag: "Dev"
  commit tag: "Dev"
  checkout main
  merge feature/name tag: "Preview"

  # Create release branch (LTS)
  branch release/3.2 order: 3
  commit tag: "Preview"

  # Hotfix after manual test failures
  branch feature/hotfix order: 4
  commit tag: "Dev"
  commit tag: "Dev"
  checkout release/3.2
  merge feature/hotfix tag: "v3.2.1001 - Production"

  # In parallel, new features to main
  checkout main
  branch feature/name2 order: 0
  commit tag: "Dev"
  commit tag: "Dev"
  checkout main
  merge feature/name2 tag: "Preview"

  # Merge release branch
  checkout main
  merge release/3.2  tag: "Preview"

  # Patch on LTS
  checkout release/3.2
  branch feature/hotfix2 order: 5
  commit tag: "Dev"
  checkout release/3.2
  merge feature/hotfix2 tag: "v3.2.1002 - Production"

  # Merge hotfix to main as cherry-picks from now on
  checkout main
  commit type: HIGHLIGHT tag: "cherry-pick hotfix2 - Preview"
```

## Main branch

Also known as `develop`, it's the current development branch with latest
features and fixes.

This branch should be **protected** so no direct pushes are allowed. Changes
should come via _pull requests_ from feature branches.

Pushing in this branch new comments will trigger a new _preview_ build type.
This means that artifacts are deployed to the preview / staging feeds. This will
only happens when a pull request is merged.

> [!NOTE]  
> If you are working on a small, personal project alone, it should be fine not
> using pull requests and pushing directly.

## Feature branches

Development branches for small features and bug fixes.

They are prefixed with `feature/` for all type of work (including bug fixes).
This simplifies the configuration of tools (like GitVersion) and gives
consistency in the branching schema.

Building locally or in pull requests creates a _development_ build. No artifacts
are deployed, but they should be accessible to download and test from CI output.

> [!TIP]  
> Merging pull requests with the _squash_ strategy could be a good idea as it
> keeps a clean git history. The only exception is the first merge from the
> release branch into the main.

## Release branches

Branches that help with the release and support process of a product release. In
a release branch you can stabilize the release without stopping development in
the _main_ branch of new features that won't go into that release.

They are prefixed with `release/` followed by the major and minor version
numbers.

Pushing changes into a release branch should happen only via pull requests that
are carefully reviewed. A new commit will trigger a new _preview_ build.

Once the release is ready (stakeholders signs-off) git tag the latest commit.
This will trigger a new _production build_. Depending on the build system and
_sign-off_ process a _production build_ may not involve re-building again but
downloading the latest artifacts and deploying them. For instance by using in
Azure DevOps a _manual approval_ task and keeping the _preview_ builds from a
release branch blocked.

After the release is out, a _release_ (also known as _support_) branch can be
used to work on regular patch release like for _long-term support (LTS)_
versions. Otherwise, remove them as they do not offer any advantage. You can
always re-create it from the latest git tag.

> [!NOTE]  
> In a small, personal projects you most likely won't need these branches. Just
> tag directly `main`, for instance by creating a _GitHub release_. You can
> always check-out a _tagged commit_, branch and create a _patch release_ from
> there if needed.

## Epic branches

Special type of a development branch. It helps to implement large features that
could affect the development of other current features.

It acts as a **temporary** parallel branch to `main`. Feature branches related
to the feature are merged into the epic. Once the feature is stable enough, the
epic is merged into the `main` branch as it were a _big_ feature branch.

Try to minimize the usage of _epic_ branches to prevent huge merge conflicts
later.

> [!INFO]  
> This schema does not use a `master` branch as the original
> [_git flow_](https://nvie.com/posts/a-successful-git-branching-model/)
> proposed. This branch has not much use. It's possible to get the latest stable
> version by listing _git tags_. Not having the branch prevents typical issues
> with the merges and conflicts.
