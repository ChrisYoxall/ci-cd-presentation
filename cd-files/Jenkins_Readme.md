
## Jenkins

https://hub.docker.com/r/jenkins/jenkins
Instructions: https://github.com/jenkinsci/docker/blob/master/README.md
Latest: https://www.jenkins.io/changelog-stable/

### Build and run custom Jenkins image
Build a custom Jenkins image with a fairly minimal set of plugins:

    docker build -t custom-jenkins/jenkins:2.492.3 -f ./Dockerfile_custom_jenkins .

And run it by doing:

    docker run -d --rm -p 8080:8080 \
        -v jenkins_home:/var/jenkins_home \
        -v /var/run/docker.sock:/var/run/docker.sock \
        --group-add=$(stat -c '%g' /var/run/docker.sock) \
        custom-jenkins/jenkins:2.492.3

Can get details on the installed plugins from the 'Manage Jenkins > Plugins' screen. To get the name of the plugin to use when installing from
the CLI you can either click on a plugin and get the name from the URL, or running this groovy script from the Jenkins script console:

    Jenkins.instance.pluginManager.plugins.each{
        plugin -> println ("${plugin.getShortName()}:${plugin.getVersion()}")
    }

NOTE: This is a very unsecure way to run Jenkins, and you should always set up agents to run jobs. We will run
jobs on the main controller.

### Set up a build & deploy job

The build and deploy process is in the [jenkinsfile](./cd-files/jenkinsfile). It's considered good practice
to define your pipeline in a script and commit to version control just like any other piece of code.

To set up pipeline:

- From the Jenkins dashboard click 'New Item'
- Enter a name into the top text box, click 'Pipeline' to select it, and then hit 'OK' down the bottom of the screen
- You will now be at the 'Configuration' screen for the new pipeline. Set the following fields in the 'Pipeline' section:
    - Definition: Pipeline script from SCM
    - Repository URL: https://github.com/ChrisYoxall/ci-cd-presentation.git
    - Branch-Specifier: main
    - Script Path: cd-files/jenkinsfile
    - Click save

Run the pipeline. If you look in the configuration screen again for the pipeline after a run you will some sections
such as a trigger and parameter have been added which came from the Jenkinsfile.
