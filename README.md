 CIU246

 This might be too much for this course, but if you want to have a look at some good old conventions, have a look below :D

## Git Conventions

Follow the style branching model as described [here](http://nvie.com/posts/a-successful-git-branching-model/). In short:

Never code directly into the **master** branch, instead start of with the **develop** branch. When you are starting a new feature (or bug fix) create a new branch from **develop** and name your new branch **feature/myFeature**, such as **feature/gitLogin**. Note and mimic lowercase and uppercase convention!

### Example workflow

Example of workflow for starting a new coding session:

1. `git pull` on **develop**
2. `checkout` your **feature/bug** branch and `merge` with **develop**.

#### Commits

Try to follow the [AngularJS Git Commit Message Conventions](https://docs.google.com/document/d/1QrDFcIiPjSLDn3EL15IJygNPiHORgU1_OOAqWjiDU5Y/mobilebasic) by Google.

Summarily, try to start your message with a one-word description of the type of activity you've been doing followed by a colon. It should be one of the following:

* feat (feature)
* fix (bug fix)
* docs (documentation)
* style (formatting, missing semi colons, …)
* refactor
* test (when adding missing tests)
* chore (maintain)

Continue with a description of what you've done. Follow these rules:

* imperative present tense, i.e. "change" not "changed" or "changes".
* don't capitalize first letter
* no dot (.) at the end

Example:

	fix: add missing semi colon

Commit early and often! If your commit message is too long to fit the message subject, it is probably too big of a commit.

#### Use of develop and master

Before `push` to **develop**:

1. Do a `git pull` on **develop**.
2. `checkout` your **feature/bug** branch and `merge` with **develop**.
3. Fix any merge conflicts locally in your **feature/bug** branch (not in **develop**).
4. `checkout` **develop** and `merge` with your **feature/bug** branch.
5. `push` to **develop**.
6. Only `push` to **master** when having stable working builds (they should pass all tests).