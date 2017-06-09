#!/usr/bin/env bash
$DOCKER_USERNAME = $1;
$HEROKU_AUTH_TOKEN = $2;
$APP_NAME = $3;

echo "Starting deploy!";

docker login --email=$DOCKER_USERNAME --username=$DOCKER_USERNAME --password=$HEROKU_AUTH_TOKEN registry.heroku.com
docker build -t $APP_NAME ./InfoBot/bin/Release/netcoreapp1.1/publish;
docker tag "$APP_NAME" registry.heroku.com/$APP_NAME/web
docker push registry.heroku.com/$APP_NAME/web

echo "Deploy finished!"