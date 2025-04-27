FROM debian:12.10-slim AS volta-env

# Install curl
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl ca-certificates && \
    rm -rf /var/lib/apt/lists/*

# Create a non-root user
RUN groupadd -r appgroup && useradd -r -g appgroup -m appuser
USER appuser
WORKDIR /home/appuser

# Install Volta
RUN curl https://get.volta.sh | bash

FROM volta-env AS build

# Add Volta to PATH and enable experimental PNPM support
ENV VOLTA_HOME="/home/appuser/.volta"
ENV PATH="$VOLTA_HOME/bin:$PATH"
ENV VOLTA_FEATURE_PNPM=1

WORKDIR /app

# Copy package.json and install dependencies
COPY --chown=appuser:appgroup package.json ./
RUN pnpm i

# Copy the rest of the files in
COPY --chown=appuser:appgroup . .

# Run tests and linting
RUN pnpm lint
RUN pnpm test

# Build the application
RUN pnpm build

FROM nginx:alpine AS final

# Create a non root user. Since running as a non-root user will tell Nginx to use the /tmp directory
# for temp files.
RUN addgroup -S nginxgroup && adduser -S nginxuser -G nginxgroup \
 && mkdir -p /tmp/nginx/client_temp /tmp/nginx/proxy_temp /tmp/nginx/fastcgi_temp /tmp/nginx/uwsgi_temp /tmp/nginx/scgi_temp \
 && chown -R nginxuser:nginxgroup /usr/share/nginx /etc/nginx /tmp/nginx \
 && rm -f /etc/nginx/conf.d/default.conf

# Copy in custom nginx config file with changes to allow running as a non-root user
COPY nginx.conf /etc/nginx/nginx.conf

# Copy files from the build stage
COPY --from=build --chown=nginxuser:nginxgroup /app/dist /usr/share/nginx/html

# Switch to non-root user
USER nginxuser

EXPOSE 8080

# Avoid nginx:alpine's default entrypoint scripts
ENTRYPOINT []
CMD ["nginx", "-g", "daemon off;"]