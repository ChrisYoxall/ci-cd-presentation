#!/bin/bash

# Fix Docker socket permissions
if [ -e /var/run/docker.sock ]; then
  chmod 666 /var/run/docker.sock
fi

# Ensure jenkins user has permissions to the workspace directory
chown -R jenkins:jenkins /var/jenkins_home/workspace

# Continue with normal Jenkins startup
exec /usr/local/bin/jenkins.sh