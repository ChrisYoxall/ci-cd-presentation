#!/bin/bash
if [ -e /var/run/docker.sock ]; then
  chmod 666 /var/run/docker.sock
fi
exec /usr/local/bin/jenkins.sh