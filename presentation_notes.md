


Timing:
* roughly 20 minutes on theory, up to slide seven
* roughly 20 minutes talking GitHub Actions, working through demo
* 20 minutes to play with and hopefully run the demo workflows



### SLIDE ONE (TITLE PAGE) - CI / CD

* I'm assuming you want to be or learn more about software development?
* Write code every day is my advice. Do you know how to build and deploy it manually to a function or managed service. 
* CI/CD may not be something you need to know much about, particularly when you are new, but
    the odds are you will have to fix it at some point. Teaches you about your platform which
    will help troubleshooting.


### SLIDE TWO - About me


### SLIDE THREE - Part 1: CI 

* Splitting this up into two presentations. Today is CI.


### SLIDE FOUR - CI Definition

Frequently merge code changes into a central repository, followed by automated checks.

* Frequent usually means at least once a day.
    - Can be lots of people working on a code bases. What if a team of 3 each has a feature that takes 3-5 days?
        Code diverges and more chance of code conflicts.
    - Daily merges mean merges are small. Much easier to approve a PR that is 100 lines than 500 lines.
        If you can commit more often than once a day - then do so.
    - If you have gone down the wrong path then it minimises wasted effort.
    
* Central repository
    - Doesn't mean you merge other changes into your code. You also merge. Aim is to try and keep
        one copy of code
    - Doesn't matter what version control system you use or the branching strategy (i.e. GitFlow, Feature
        Branches, Trunk Based)
        
* Automated checks
    - **ASK** What are some of the checks?
    - Linting, build (it's a check), unit tests (possibly coverage), tools for code quality & security (lots of
        options here and you may run several). User acceptance testing.
    - Integration tests are a bit of a grey area. Yes you need at least some, however:
        * Does this include end-to-end (UI) tests?
        * Performance tests?
        * Should always do some integration tests but may have larger tests suits that don't run as part of build.
            Really need the pipeline to be fast (i.e. 10 minutes or so)
    - Need to keep pipelines as fast as possible. Needs to be automated and needs continuous optimisation
    - If something breaks, and a small change has just been merged, it's much easier to troubleshoot.


### SLIDE FIVE - Benefits

* Improve quality
* Improves collaboration
* Reduce development time. Will talk more about DORA in CD session, but there is proof that smaller changes
    made regularly is faster overall than making a series of big changes.

**ASK** - Do you think this makes sense?

Picture if people didn't do this. Could have 2-3 people merging big changes towards the end of a sprint. Errors
can be hard to pin down.

I struggle to remember what code I wrote last week. Finding an issue on the same day as it's written makes
it much easier to fix.


### SLIDE SIX - Related Concepts

Want to relate CI to other terms you may hear in the industry.

* Keep the pipeline green.
    - Ensure you watch the result of your commit or get notified if there is an issue.
    - Fixing a broken pipeline is always the top priority. If it's broke you can't release.
    - Have heard of things like 'dunce hats' or 'trophies' for people who break the build.
* Feature Toggling.
* Shift Left.
    - Moving things that traditionally happened later in the development cycle, earlier (i.e. User acceptance
        testing, testing in general, penetration testing).

        
### SLIDE SEVEN - How do we do it?

Covered the principles. Hopefully you want to implement it. What next?

* It's not just a technical change:
    can be a change in working amongst the team - talk about it.
    get buy in from rest of team
    don't just go and implement a pipeline that supports CI if people will still commit weekly
    do you measure how frequently people merge? Can be a touchy subject
* Pipeline work requires coding.
    plan out what you want to do
    get time allocated
    treat pipelines like code (keep it simple, don't repeat yourself - make reusable components, comment and document) 
* Takes more time than people expect.
    - Partly because it's hard to test.
    - Don't underestimate the value of a pipeline, use this to justify time. Vital to get code out safely.
    - It may not be obvious code that needs to be maintained, but it can become large & complex.
* In general, pipelines implement the same steps you would do to do build and check locally.


### SLIDE EIGHT - Tools

Lots of tools for CI.

* Was lazy - I searched for common tools and found this list. Tons of lists out there, all similar.
* How do you decide?
    - Normal process of evaluation for company fit, costs, etc.
    - Choose something with good examples & guides
    - Had people turn down jobs as they didn't want to use Jenkins


### SLIDE NINE - GitHub Actions

I've always found the terminology a bit confusing:
    - GitHub Actions is the name of the platform
    - Actions are also

*Documentation is pretty good https://docs.github.com/en/actions:
    - Go to page. Click on the nice green 'Overview' button
    - Note section in menu to the left on 'Continuous Integration'.
    - Click on the link at the bottom for 'actions/starter-workflows
    - Show some other documentation. Only going to cover basics today
        - Go to the section in the menu 'Write workflows'. Show 'syntax' 'choose what workflows do' sections.
* To achieve a task, create a 'GitHub Actions workflow' 
    - defined in YAML
    - are triggered by an event (i.e. PR created, scheduled run, webhook, manually)
* Workflows are made up of Jobs
* Jobs are made up of Steps
* Steps run workflows, scripts, or run an Action
* An Action is a custom application on the GitHub Actions platform to perform a common task
    - Can write your own
    - Lots of Actions available on the marketplace
* Runners
    - A server that runs your workflows when they're triggered.
    - Get a newly provisioned VM for each workflow. State management.
    - GitHub provides Ubuntu Linux, Microsoft Windows, and macOS runners to run your workflows.


### SLIDE TEN - Demo

From https://dev.to/github/the-githubtoken-in-github-actions-how-it-works-change-permissions-customizations-3cgp:
* The way this works is that when you enable GitHub Actions in a repository, GitHub installs a GitHub App on that. The GITHUB_TOKEN secret is a GitHub App installation access token.


### SLIDE 11 - Final Notes

* Its nice if developers can run steps locally. Consider this when designing a pipeline
* Use Actions from reputable sources. Use popular Actions. Consider what version/commit to run or even fork into
        your repo

